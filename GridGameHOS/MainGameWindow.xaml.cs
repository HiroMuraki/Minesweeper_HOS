using GridGameHOS.Minesweeper;
using GridGameHOS.SlideJigsaw;
using GridGameHOS.TwoZeroFourEightLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using static GridGameHOS.Common.GeneralAction;

namespace GridGameHOS.Common {
    /// <summary>
    /// 扫雷游戏
    /// </summary>
    public partial class MainGameWindow : Window, INotifyPropertyChanged {
        #region 字段与属性
        #region 后备字段
        private IGridGame currentGame;
        private int usingTime;
        private int rowsSet;
        private int columnsSet;
        private int minesSet;
        private int maximumRows;
        private int minimumRows;
        private int maximumColumns;
        private int minimumColumns;
        private int minimumMines;
        #endregion
        #region 常量-用于xaml绑定
        public int MaximumRows {
            get {
                return maximumRows;
            }
            set {
                maximumRows = value;
                OnPropertyChanged(nameof(MaximumRows));
            }
        }
        public int MinimumRows {
            get {
                return minimumRows;
            }
            set {
                minimumRows = value;
                OnPropertyChanged(nameof(MinimumRows));
            }
        }
        public int MaximumColumns {
            get {
                return maximumColumns;
            }
            set {
                maximumColumns = value;
                OnPropertyChanged(nameof(MaximumColumns));
            }
        }
        public int MinimumColumns {
            get {
                return minimumColumns;
            }
            set {
                minimumColumns = value;
                OnPropertyChanged(nameof(MinimumColumns));
            }
        }
        public int MinimumMines {
            get {
                return minimumMines;
            }
            set {
                minimumMines = value;
                OnPropertyChanged(nameof(MinimumMines));
            }
        }
        public int MaximumMines {
            get {
                return (RowsSet * ColumnsSet) >> 2;
            }
        }
        #endregion
        #region 主要属性
        public event PropertyChangedEventHandler PropertyChanged;
        public IGridGame CurrentGame {
            get {
                return currentGame;
            }
            set {
                currentGame = value;
                OnPropertyChanged(nameof(CurrentGame));
            }
        }
        public DispatcherTimer UsingTimeTimer = new DispatcherTimer();
        public int RowsSet {
            get {
                return rowsSet;
            }
            set {
                rowsSet = value;
                OnPropertyChanged(nameof(RowsSet));
                OnPropertyChanged(nameof(MinesSet));
                OnPropertyChanged(nameof(MaximumMines));
                OnPropertyChanged(nameof(GameSizeStatus));
                OnPropertyChanged(nameof(ProcessStatus));
            }
        }
        public int ColumnsSet {
            get {
                return columnsSet;
            }
            set {
                columnsSet = value;
                OnPropertyChanged(nameof(ColumnsSet));
                OnPropertyChanged(nameof(MinesSet));
                OnPropertyChanged(nameof(MaximumMines));
                OnPropertyChanged(nameof(GameSizeStatus));
                OnPropertyChanged(nameof(ProcessStatus));
            }
        }
        public int MinesSet {
            get {
                if (minesSet > MaximumMines) {
                    minesSet = MaximumMines;
                }
                return minesSet;
            }
            set {
                minesSet = value;
                OnPropertyChanged(nameof(MinesSet));
                OnPropertyChanged(nameof(MaximumMines));
                OnPropertyChanged(nameof(ProcessStatus));
            }
        }
        public int UsingTime {
            get {
                return usingTime;
            }
            set {
                usingTime = value;
                OnPropertyChanged(nameof(UsingTime));
                OnPropertyChanged(nameof(UsingTimeStatus));
            }
        }
        public string GameSizeStatus {
            get {
                return CurrentGame.GameSizeStatus;
            }
        }
        public string ProcessStatus {
            get {
                return CurrentGame.ProcessStatus;
            }
        }
        public string UsingTimeStatus {
            get {
                return $"{UsingTime}秒";
            }
        }
        #endregion
        #endregion

        #region 按钮与控件
        private MainGameWindow() {
            InitializeComponent();
            Cursor = NormalCursor;
            DataContext = this;
            UsingTimeTimer.Interval = TimeSpan.FromSeconds(1);
            UsingTimeTimer.Tick += TimerUsingTimer_Tick;
            //添加游戏列表
            SelectorGameList.AllowedLabels = new List<string>();
            foreach (string gameType in GameDictionary.Values) {
                SelectorGameList.AllowedLabels.Add(gameType);
            }
        }
        public MainGameWindow(GameType gameType) : this() {
            LoadGame(gameType);
            SelectorGameList.CurrentLabel = GameDictionary[gameType];
        }
        /// <summary>
        /// 开始游戏按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStartGame_ButtonClick(object sender, RoutedEventArgs e) {
            PlayFXSound(nameof(MenuButtonClickSound));
            StatusButton currentButton = sender as StatusButton;
            if (currentButton == null) {
                return;
            }
            switch (currentButton.Name) {
                case "btnQuickStartA":
                    currentGame.QuickGame(0);
                    break;
                case "btnQuickStartB":
                    CurrentGame.QuickGame(1);
                    break;
                case "btnQuickStartC":
                    CurrentGame.QuickGame(2);
                    break;
            }
            StartCurrentGame();
        }
        /// <summary>
        /// 右键点击开始游戏按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStartGame_ButtonRightClick(object sender, RoutedEventArgs e) {
            PlayFXSound(nameof(MenuButtonClickSound));
            RandomMode();
        }
        /// <summary>
        /// 显示/隐藏游戏设置栏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HiddenSettingMenuButton_Click(object sender, RoutedEventArgs e) {
            PlayFXSound(nameof(MenuButtonClickSound));
            if (SettingMenu.Visibility == Visibility.Visible) {
                SettingMenu.Visibility = Visibility.Collapsed;
            } else {
                SettingMenu.Visibility = Visibility.Visible;
            }
        }
        /// <summary>
        /// 显示/隐藏技能槽
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DetecotrSwitchButton_Click(object sender, RoutedEventArgs e) {
            PlayFXSound(nameof(MenuButtonClickSound));
            if (DetectorBox.Visibility == Visibility.Visible) {
                DetectorBox.Visibility = Visibility.Collapsed;
            } else {
                DetectorBox.Visibility = Visibility.Visible;
            }
        }
        /// <summary>
        /// 切换游戏类型标签
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectorGameList_LabelSwitched(object sender, RoutedEventArgs e) {
            PlayScaleTransform(SelectorGameList.CurrentDisplayLabel, 0, 1, 255);
            CurrentGame.UnloadGame();
            switch (SelectorGameList.CurrentLabel) {
                case "扫雷":
                    LoadGame(GameType.Minesweeper);
                    break;
                case "滑块拼图":
                    LoadGame(GameType.SlideJigsaw);
                    break;
                case "2048H":
                    LoadGame(GameType.TwoZeroFourEight);
                    break;
            }
        }
        /// <summary>
        /// 当游戏完成后按下游戏区时的透明按钮，用于开始游戏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void borderGamePanelCover_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            PlayFXSound(nameof(MenuButtonClickSound));
            StartCurrentGame();
        }
        /// <summary>
        /// 当鼠标悬浮于菜单按钮时出发音效播放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuButton_MouseEnter(object sender, MouseEventArgs e) {
            PlayFXSound(nameof(MenuMouseHoverSound));
        }
        #endregion

        #region 窗口操作
        /// <summary>
        /// 窗口操作，包括移动窗口，最大化窗口，最小化窗口，关闭窗口以及快捷键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Move(object sender, MouseButtonEventArgs e) {
            if (e.ClickCount == 2) {
                if (WindowState == WindowState.Normal) {
                    WindowState = WindowState.Maximized;
                } else {
                    WindowState = WindowState.Normal;
                }
            } else {
                DragMove();
            }
        }
        private void Window_Minimized(object sender, RoutedEventArgs e) {
            WindowState = WindowState.Minimized;
        }
        private void Window_Maximized(object sender, RoutedEventArgs e) {
            if (WindowState == WindowState.Normal) {
                WindowState = WindowState.Maximized;
            } else {
                WindowState = WindowState.Normal;
            }
        }
        private void Window_Close(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }
        private void Window_KeyDown(object sender, KeyEventArgs e) {
            switch (e.Key) {
                case Key.M:
                    HiddenSettingMenuButton_Click(new object(), new RoutedEventArgs());
                    break;
                case Key.Space:
                    btnStartGame_ButtonClick(BtnStartGame, new RoutedEventArgs());
                    break;
                case Key.F1:
                    btnStartGame_ButtonClick(btnQuickStartA, new RoutedEventArgs());
                    break;
                case Key.F2:
                    btnStartGame_ButtonClick(btnQuickStartB, new RoutedEventArgs());
                    break;
                case Key.F3:
                    btnStartGame_ButtonClick(btnQuickStartC, new RoutedEventArgs());
                    break;
                case Key.F5:
                    btnStartGame_ButtonRightClick(BtnStartGame, new RoutedEventArgs());
                    break;
            }
        }
        /// <summary>
        /// 鼠标样式修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e) {
            Cursor = ClickedCursor;
            ToggleDetector.IsChecked = false;
        }
        private void Window_MouseUp(object sender, MouseButtonEventArgs e) {
            Cursor = NormalCursor;
        }
        #endregion

        #region 包装方法
        /// <summary>
        /// INotifyPropertyChanged时间的包装方法
        /// </summary>
        /// <param name="propertyName"></param>
        public void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// 载入游戏时的操作
        /// </summary>
        /// <param name="gameType"></param>
        public void LoadGame(GameType gameType) {
            CurrentGame?.UnloadGame();
            switch (gameType) {
                case GameType.Minesweeper:
                    CurrentGame = new MinesweeperGame(this);
                    break;
                case GameType.SlideJigsaw:
                    CurrentGame = new SlideJigsawGame(this);
                    break;
                case GameType.TwoZeroFourEight:
                    CurrentGame = new TwoZeroFourEightGame(this);
                    break;
            }
            btnStartGame_ButtonClick(btnQuickStartA, new RoutedEventArgs());
        }
        /// <summary>
        /// 开始当前游戏，由窗口调用
        /// </summary>
        private void StartCurrentGame() {
            Cursor = LoadingGameCursor;
            //开始游戏
            CurrentGame.StartGame();
            //重置统计
            BorderGamePanelCover.IsHitTestVisible = false;
            GamePlayAreaGrid.Effect = null;
            GameCompleteBarImage.IsEnabled = false;
            ToggleDetector.IsEnabled = true;
            BtnStartGame.IsOn = null;
            UsingTime = 0;
            UsingTimeTimer.Start();
            Cursor = NormalCursor;
            OnPropertyChanged(nameof(CurrentGame));
        }
        public void StartCustomGame(Action gameStartAction) {
            Cursor = LoadingGameCursor;
            //开始游戏
            gameStartAction?.Invoke();
            //重置统计
            BorderGamePanelCover.IsHitTestVisible = false;
            GamePlayAreaGrid.Effect = null;
            GameCompleteBarImage.IsEnabled = false;
            ToggleDetector.IsEnabled = true;
            BtnStartGame.IsOn = null;
            UsingTime = 0;
            UsingTimeTimer.Start();
            Cursor = NormalCursor;
            OnPropertyChanged(nameof(CurrentGame));
        }
        /// <summary>
        /// 结算游戏，由游戏调用，若传入null，则无其它操作，传入false，仅关闭隐藏开始蒙版与计时器，传入true，正常结算
        /// </summary>
        /// <param name="isGameCompleted"></param>
        public void CalCurrentGame(bool? isGameCompleted = true) {
            if (isGameCompleted == null) {
                return;
            }
            UsingTimeTimer.Stop();
            BorderGamePanelCover.IsHitTestVisible = true;
            if (isGameCompleted == true) {
                BtnStartGame.IsOn = true;
                GameCompleteBarImage.IsEnabled = true;
                PlayBlurTransfrom(GamePlayAreaGrid, 0, 15, 200);
                //MessageBox.Show("YZTXDY"); ;
            } else {
                //
            }
        }
        /// <summary>
        /// 随机模式
        /// </summary>
        private void RandomMode() {
            Random rnd = new Random();
            RowsSet = rnd.Next(MinimumRows, MaximumRows);
            ColumnsSet = rnd.Next(MinimumColumns, MaximumColumns);
            MinesSet = rnd.Next(MinimumMines, MaximumMines);
            StartCurrentGame();
        }
        /// <summary>
        /// 游戏计时器更新，每秒更新一次
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerUsingTimer_Tick(object sender, EventArgs e) {
            ++UsingTime;
        }
        #endregion
    }
}
