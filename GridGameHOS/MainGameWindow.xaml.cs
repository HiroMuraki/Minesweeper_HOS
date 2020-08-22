using MinesweeperGameLite;
using SlideJigsawGameLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Effects;
using System.Windows.Threading;
using TwoZeroFourEightLite;
using static Common.GeneralAction;

namespace Common {
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
                return this.maximumRows;
            }
            set {
                this.maximumRows = value;
                OnPropertyChanged(nameof(MaximumRows));
            }
        }
        public int MinimumRows {
            get {
                return this.minimumRows;
            }
            set {
                this.minimumRows = value;
                OnPropertyChanged(nameof(MinimumRows));
            }
        }
        public int MaximumColumns {
            get {
                return this.maximumColumns;
            }
            set {
                this.maximumColumns = value;
                OnPropertyChanged(nameof(MaximumColumns));
            }
        }
        public int MinimumColumns {
            get {
                return this.minimumColumns;
            }
            set {
                this.minimumColumns = value;
                OnPropertyChanged(nameof(MinimumColumns));
            }
        }
        public int MinimumMines {
            get {
                return this.minimumMines;
            }
            set {
                this.minimumMines = value;
                OnPropertyChanged(nameof(MinimumMines));
            }
        }
        public int MaximumMines {
            get {
                return (this.RowsSet * this.ColumnsSet) >> 2;
            }
        }
        #endregion
        #region 主要属性
        public event PropertyChangedEventHandler PropertyChanged;
        public IGridGame CurrentGame {
            get {
                return this.currentGame;
            }
            set {
                this.currentGame = value;
                OnPropertyChanged(nameof(CurrentGame));
            }
        }
        public DispatcherTimer UsingTimeTimer = new DispatcherTimer();
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
                return this.CurrentGame.GameSizeStatus;
            }
        }
        public string ProcessStatus {
            get {
                return this.CurrentGame.ProcessStatus;
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
            this.Cursor = NormalCursor;
            this.DataContext = this;
            this.UsingTimeTimer.Interval = TimeSpan.FromSeconds(1);
            this.UsingTimeTimer.Tick += TimerUsingTimer_Tick;
            //添加游戏列表
            this.SelectorGameList.AllowedLabels = new List<string>();
            foreach (string gameType in GameDictionary.Values) {
                this.SelectorGameList.AllowedLabels.Add(gameType);
            }
        }
        public MainGameWindow(GameType gameType) : this() {
            this.LoadGame(gameType);
            this.SelectorGameList.CurrentLabel = GameDictionary[gameType];
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
                    this.currentGame.QuickGame(0);
                    break;
                case "btnQuickStartB":
                    this.CurrentGame.QuickGame(1);
                    break;
                case "btnQuickStartC":
                    this.CurrentGame.QuickGame(2);
                    break;
            }
            this.StartCurrentGame();
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
            if (this.SettingMenu.Visibility == Visibility.Visible) {
                this.SettingMenu.Visibility = Visibility.Collapsed;
            } else {
                this.SettingMenu.Visibility = Visibility.Visible;
            }
        }
        /// <summary>
        /// 显示/隐藏技能槽
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DetecotrSwitchButton_Click(object sender, RoutedEventArgs e) {
            PlayFXSound(nameof(MenuButtonClickSound));
            if (this.DetectorBox.Visibility == Visibility.Visible) {
                this.DetectorBox.Visibility = Visibility.Collapsed;
            } else {
                this.DetectorBox.Visibility = Visibility.Visible;
            }
        }
        /// <summary>
        /// 切换游戏类型标签
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectorGameList_LabelSwitched(object sender, RoutedEventArgs e) {
            PlayScaleTransform(this.SelectorGameList.CurrentDisplayLabel, 0, 1, 255);
            this.CurrentGame.UnloadGame();
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
                case Key.M:
                    HiddenSettingMenuButton_Click(new object(), new RoutedEventArgs());
                    break;
                case Key.Space:
                    btnStartGame_ButtonClick(this.BtnStartGame, new RoutedEventArgs());
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
                    btnStartGame_ButtonRightClick(this.BtnStartGame, new RoutedEventArgs());
                    break;
            }
        }
        /// <summary>
        /// 鼠标样式修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e) {
            this.Cursor = ClickedCursor;
            this.ToggleDetector.IsChecked = false;
        }
        private void Window_MouseUp(object sender, MouseButtonEventArgs e) {
            this.Cursor = NormalCursor;
        }
        #endregion

        #region 包装方法
        /// <summary>
        /// INotifyPropertyChanged时间的包装方法
        /// </summary>
        /// <param name="propertyName"></param>
        public void OnPropertyChanged(string propertyName) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// 载入游戏时的操作
        /// </summary>
        /// <param name="gameType"></param>
        public void LoadGame(GameType gameType) {
            this.CurrentGame?.UnloadGame();
            switch (gameType) {
                case GameType.Minesweeper:
                    this.CurrentGame = new MinesweeperGame(this);
                    break;
                case GameType.SlideJigsaw:
                    this.CurrentGame = new SlideJigsawGame(this);
                    break;
                case GameType.TwoZeroFourEight:
                    this.CurrentGame = new TwoZeroFourEightGame(this);
                    break;
            }
            this.btnStartGame_ButtonClick(this.btnQuickStartA, new RoutedEventArgs());
        }
        /// <summary>
        /// 开始当前游戏，由窗口调用
        /// </summary>
        private void StartCurrentGame() {
            this.Cursor = LoadingGameCursor;
            //开始游戏
            this.CurrentGame.StartGame();
            //重置统计
            this.BorderGamePanelCover.IsHitTestVisible = false;
            this.GamePlayAreaGrid.Effect = null;
            this.GameCompleteBarImage.IsEnabled = false;
            this.ToggleDetector.IsEnabled = true;
            this.BtnStartGame.IsOn = null;
            this.UsingTime = 0;
            this.UsingTimeTimer.Start();
            this.Cursor = NormalCursor;
            this.OnPropertyChanged(nameof(this.CurrentGame));
        }
        public void StartCustomGame(Action gameStartAction) {
            this.Cursor = LoadingGameCursor;
            //开始游戏
            gameStartAction?.Invoke();
            //重置统计
            this.BorderGamePanelCover.IsHitTestVisible = false;
            this.GamePlayAreaGrid.Effect = null;
            this.GameCompleteBarImage.IsEnabled = false;
            this.ToggleDetector.IsEnabled = true;
            this.BtnStartGame.IsOn = null;
            this.UsingTime = 0;
            this.UsingTimeTimer.Start();
            this.Cursor = NormalCursor;
            this.OnPropertyChanged(nameof(this.CurrentGame));
        }
        /// <summary>
        /// 结算游戏，由游戏调用，若传入null，则无其它操作，传入false，仅关闭隐藏开始蒙版与计时器，传入true，正常结算
        /// </summary>
        /// <param name="isGameCompleted"></param>
        public void CalCurrentGame(bool? isGameCompleted = true) {
            if (isGameCompleted == null) {
                return;
            }
            this.UsingTimeTimer.Stop();
            this.BorderGamePanelCover.IsHitTestVisible = true;
            if (isGameCompleted == true) {
                this.BtnStartGame.IsOn = true;
                this.GameCompleteBarImage.IsEnabled = true;
                this.GamePlayAreaGrid.Effect = new BlurEffect {
                    KernelType = KernelType.Gaussian,
                    Radius = 0
                };
                this.GamePlayAreaGrid.Effect.BeginAnimation(BlurEffect.RadiusProperty, AnimationForBlurEffect);
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
            this.RowsSet = rnd.Next(MinimumRows, MaximumRows);
            this.ColumnsSet = rnd.Next(MinimumColumns, MaximumColumns);
            this.MinesSet = rnd.Next(MinimumMines, MaximumMines);
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
