﻿using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Effects;
using System.Windows.Threading;

namespace MinesweepGameLite {
    /// <summary>
    /// 扫雷游戏
    /// </summary>
    public partial class MinesweeperGameWindow : Window, INotifyPropertyChanged {
        #region 字段与属性
        public event PropertyChangedEventHandler PropertyChanged;
        public MinesweeperGame CurrentGame { get; set; }
        private DispatcherTimer usingTimeTimer = new DispatcherTimer();
        public int RowsSet {
            get {
                return this.rowsSet;
            }
            set {
                this.rowsSet = value;
                OnPropertyChanged(nameof(RowsSet));
                OnPropertyChanged(nameof(MinesSet));
                OnPropertyChanged(nameof(MaximumMines));
                OnPropertyChanged(nameof(GameSizeStatus));
                OnPropertyChanged(nameof(ProcessStatus));
            }
        }
        public int ColumnsSet {
            get {
                return this.columnsSet;
            }
            set {
                this.columnsSet = value;
                OnPropertyChanged(nameof(ColumnsSet));
                OnPropertyChanged(nameof(MinesSet));
                OnPropertyChanged(nameof(MaximumMines));
                OnPropertyChanged(nameof(GameSizeStatus));
                OnPropertyChanged(nameof(ProcessStatus));
            }
        }
        public int MinesSet {
            get {
                if (this.minesSet > this.MaximumMines) {
                    this.minesSet = this.MaximumMines;
                }
                return this.minesSet;
            }
            set {
                this.minesSet = value;
                OnPropertyChanged(nameof(MinesSet));
                OnPropertyChanged(nameof(MaximumMines));
                OnPropertyChanged(nameof(ProcessStatus));
            }
        }
        public int UsingTime {
            get {
                return this.usingTime;
            }
            set {
                this.usingTime = value;
                OnPropertyChanged(nameof(UsingTime));
                OnPropertyChanged(nameof(UsingTimeStatus));
            }
        }
        public string GameSizeStatus {
            get {
                return $"{this.rowsSet} x {this.columnsSet}";
            }
        }
        public string ProcessStatus {
            get {
                return $"{this.CurrentGame.FlagsCount}/{this.MinesSet}";
            }
        }
        public string UsingTimeStatus {
            get {
                return $"{UsingTime}秒";
            }
        }
        #endregion
        #region 后备字段
        private int usingTime;
        private int rowsSet;
        private int columnsSet;
        private int minesSet;
        #endregion

        #region 常量-用于xaml绑定
        public int MaximumRows { get { return 18; } }
        public int MinimumRows { get { return 6; } }
        public int MaximumColumns { get { return 30; } }
        public int MinimumColumns { get { return 6; } }
        public int MinimumMines { get { return 5; } }
        public int MaximumMines {
            get {
                return (this.RowsSet * this.ColumnsSet) >> 2;
            }
        }
        #endregion

        #region 按钮与控件
        public MinesweeperGameWindow() {
            InitializeComponent();
            this.Cursor = App.NormalCursor;
            this.CurrentGame = new MinesweeperGame(BlockCreateAction);
            this.DataContext = this;
            this.usingTimeTimer.Interval = TimeSpan.FromSeconds(1);
            this.usingTimeTimer.Tick += TimerUsingTimer_Tick;
            btnStartGame_ButtonClick(this.btnQuickStartA, new RoutedEventArgs());
        }
        private void btnStartGame_ButtonClick(object sender, RoutedEventArgs e) {
            App.PlayFXSound(nameof(App.MenuButtonClickSound));
            StatusButton currentButton = sender as StatusButton;
            if (currentButton == null) {
                return;
            }
            switch (currentButton.Name) {
                case "btnQuickStartA":
                    this.RowsSet = 9;
                    this.ColumnsSet = 9;
                    this.MinesSet = 10;
                    break;
                case "btnQuickStartB":
                    this.RowsSet = 16;
                    this.ColumnsSet = 16;
                    this.MinesSet = 40;
                    break;
                case "btnQuickStartC":
                    this.RowsSet = 16;
                    this.ColumnsSet = 30;
                    this.MinesSet = 99;
                    break;
            }
            StartCurrentGame();
        }
        private void btnStartGame_ButtonRightClick(object sender, RoutedEventArgs e) {
            App.PlayFXSound(nameof(App.MenuButtonClickSound));
            RandomMode();
        }
        private void HiddenSettingMenuButton_Click(object sender, RoutedEventArgs e) {
            App.PlayFXSound(nameof(App.MenuButtonClickSound));
            if (this.SettingMenu.Visibility == Visibility.Visible) {
                this.SettingMenu.Visibility = Visibility.Collapsed;
            } else {
                this.SettingMenu.Visibility = Visibility.Visible;
            }
        }
        private void borderGamePanelCover_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            App.PlayFXSound(nameof(App.MenuButtonClickSound));
            StartCurrentGame();
        }
        private void GameBlock_OpenBlock(object sender, RoutedEventArgs e) {
            App.PlayFXSound(nameof(App.BlockClickSound));
            BlockCoordinate coordinate = (sender as GameBlockCoordinated).Coordinate;
            //嗅探猫
            if (this.toggleDetector.IsChecked == true) {
                this.toggleDetector.IsChecked = false;
                this.toggleDetector.IsEnabled = false;
                if (this.CurrentGame[coordinate].IsMineBlock) {
                    this.CurrentGame.FlagBlock(coordinate);
                } else {
                    this.CurrentGame.OpenBlock(coordinate);
                }
                this.CurrentGame.IsGameStarted = true;
                this.CalGame(this.CurrentGame.IsGameCompleted);
                return;
            }
            //首开保护
            if (!this.CurrentGame.IsGameStarted) {
                this.CurrentGame.ResetLayout(coordinate);
                this.CurrentGame.IsGameStarted = true;
            }
            //普通打开
            this.CurrentGame.OpenBlock(coordinate);
            CalGame(this.CurrentGame.IsGameCompleted);
        }
        private void GameBlock_FlagBlock(object sender, RoutedEventArgs e) {
            App.PlayFXSound(nameof(App.BlockFlagSound));
            this.CurrentGame.FlagBlock((sender as GameBlockCoordinated).Coordinate);
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProcessStatus)));
        }
        private void GameBlock_QuickOpen(object sender, RoutedEventArgs e) {
            App.PlayFXSound(nameof(App.BlockClickSound));
            this.CurrentGame.OpenNearBlocks((sender as GameBlockCoordinated).Coordinate);
            CalGame(this.CurrentGame.IsGameCompleted);
        }
        private void toggleDetector_Click(object sender, RoutedEventArgs e) {
            App.PlayFXSound(nameof(App.MenuButtonClickSound));
            if (this.toggleDetector.IsChecked == true) {
                this.Cursor = App.DetectorAimerCursor;
            } else {
                this.Cursor = App.NormalCursor;
            }
        }
        private void MenuButton_MouseEnter(object sender, MouseEventArgs e) {
            App.PlayFXSound(nameof(App.MenuMouseHoverSound));
        }
        #endregion

        #region 窗口操作
        private void Window_Move(object sender, MouseButtonEventArgs e) {
            if (e.ClickCount == 2) {
                if (this.WindowState == WindowState.Normal) {
                    this.WindowState = WindowState.Maximized;
                } else {
                    this.WindowState = WindowState.Normal;
                }
            } else {
                this.DragMove();
            }
        }
        private void Window_Minimized(object sender, RoutedEventArgs e) {
            this.WindowState = WindowState.Minimized;
        }
        private void Window_Maximized(object sender, RoutedEventArgs e) {
            if (this.WindowState == WindowState.Normal) {
                this.WindowState = WindowState.Maximized;
            } else {
                this.WindowState = WindowState.Normal;
            }
        }
        private void Window_Close(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }
        private void Window_KeyDown(object sender, KeyEventArgs e) {
            switch (e.Key) {
                case Key.Space:
                    StartCurrentGame();
                    break;
                case Key.F1:
                    btnStartGame_ButtonClick(this.btnQuickStartA, new RoutedEventArgs());
                    break;
                case Key.F2:
                    btnStartGame_ButtonClick(this.btnQuickStartB, new RoutedEventArgs());
                    break;
                case Key.F3:
                    btnStartGame_ButtonClick(this.btnQuickStartC, new RoutedEventArgs());
                    break;
                case Key.F5:
                    RandomMode();
                    break;
            }
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e) {
            this.Cursor = App.ClickedCursor;
            this.toggleDetector.IsChecked = false;
        }
        private void Window_MouseUp(object sender, MouseButtonEventArgs e) {
            this.Cursor = App.NormalCursor;
        }
        #endregion

        #region 包装方法
        private IGameBlock BlockCreateAction() {
            GameBlockCoordinated block = new GameBlockCoordinated();
            block.Width = block.Height = 50;
            block.OpenBlock += GameBlock_OpenBlock;
            block.FlagBlock += GameBlock_FlagBlock;
            block.DoubleOpenBlock += GameBlock_QuickOpen;
            return block;
        }
        private void OnPropertyChanged(string propertyName) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void StartCurrentGame() {
            this.Cursor = App.LoadingGameCursor;
            //开始游戏
            if (this.CurrentGame.RowSize != this.RowsSet
                || this.CurrentGame.ColumnSize != this.ColumnsSet
                || this.CurrentGame.MineSize != this.MinesSet) {
                this.CurrentGame.SetGame(this.RowsSet, this.ColumnsSet, this.MinesSet);
            }
            this.CurrentGame.StartGame();
            //重置统计
            this.borderGamePanelCover.IsHitTestVisible = false;
            this.gamePlayAreaGrid.Effect = null;
            this.gameCompleteBarImage.IsEnabled = false;
            this.toggleDetector.IsEnabled = true;
            this.btnStartGame.IsOn = null;
            this.UsingTime = 0;
            this.usingTimeTimer.Start();
            this.Cursor = App.NormalCursor;
            OnPropertyChanged(nameof(ProcessStatus));
        }
        private void RandomMode() {
            Random rnd = new Random();
            this.RowsSet = rnd.Next(MinimumRows, MaximumRows);
            this.ColumnsSet = rnd.Next(MinimumColumns, MaximumColumns);
            this.MinesSet = rnd.Next(MinimumMines, MaximumMines);
            StartCurrentGame();
        }
        private void CalGame(bool? isGameCompleted) {
            if (isGameCompleted == null) {
                return;
            }
            this.usingTimeTimer.Stop();
            this.borderGamePanelCover.IsHitTestVisible = true;
            if (isGameCompleted == true) {
                this.btnStartGame.IsOn = true;
                this.gameCompleteBarImage.IsEnabled = true;
                this.gamePlayAreaGrid.Effect = new BlurEffect {
                    KernelType = KernelType.Gaussian,
                    Radius = 0
                };
                this.gamePlayAreaGrid.Effect.BeginAnimation(BlurEffect.RadiusProperty, App.AnimationForBlurEffect);
                //MessageBox.Show("YZTXDY"); ;
            } else {
                this.CurrentGame.OpenAllBlocks();
            }
        }
        private void TimerUsingTimer_Tick(object sender, EventArgs e) {
            ++UsingTime;
        }
        #endregion
    }
}