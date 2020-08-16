using MinesweepGameLite;
using SlideJigsawGameLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Effects;
using System.Windows.Threading;
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

        #region 按钮与控件
        public MainGameWindow() {
            InitializeComponent();
            this.Cursor = NormalCursor;
            this.DataContext = this;
            this.UsingTimeTimer.Interval = TimeSpan.FromSeconds(1);
            this.UsingTimeTimer.Tick += TimerUsingTimer_Tick;
            LoadGame(App.DefaultGame);
            //添加游戏列表
            this.SelectorGameList.AllowedLabels = new List<string>();
            foreach (string gameType in GameDictionary.Values) {
                this.SelectorGameList.AllowedLabels.Add(gameType);
            }
            this.SelectorGameList.CurrentLabel = GameDictionary[App.DefaultGame];
        }
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
        private void btnStartGame_ButtonRightClick(object sender, RoutedEventArgs e) {
            PlayFXSound(nameof(MenuButtonClickSound));
            RandomMode();
        }
        private void HiddenSettingMenuButton_Click(object sender, RoutedEventArgs e) {
            PlayFXSound(nameof(MenuButtonClickSound));
            if (this.SettingMenu.Visibility == Visibility.Visible) {
                this.SettingMenu.Visibility = Visibility.Collapsed;
            } else {
                this.SettingMenu.Visibility = Visibility.Visible;
            }
        }
        private void SelectorGameList_LabelSwitched(object sender, RoutedEventArgs e) {
            this.CurrentGame.UnloadGame();
            switch (SelectorGameList.CurrentLabel) {
                case "扫雷":
                    LoadGame(GameType.Minesweeper);
                    break;
                case "滑块拼图":
                    LoadGame(GameType.SlideJigsaw);
                    break;
            }
        }
        private void borderGamePanelCover_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            PlayFXSound(nameof(MenuButtonClickSound));
            StartCurrentGame();
        }
        private void MenuButton_MouseEnter(object sender, MouseEventArgs e) {
            PlayFXSound(nameof(MenuMouseHoverSound));
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
        private void Window_MouseDown(object sender, MouseButtonEventArgs e) {
            this.Cursor = ClickedCursor;
            this.ToggleDetector.IsChecked = false;
        }
        private void Window_MouseUp(object sender, MouseButtonEventArgs e) {
            this.Cursor = NormalCursor;
        }
        #endregion

        #region 包装方法
        public void OnPropertyChanged(string propertyName) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public void LoadGame(GameType gameType) {
            this.CurrentGame?.UnloadGame();
            switch (gameType) {
                case GameType.Minesweeper:
                    this.CurrentGame = new MinesweeperGame(this);
                    break;
                case GameType.SlideJigsaw:
                    this.CurrentGame = new SlideJigsawGame(this);
                    break;
            }
            this.btnStartGame_ButtonClick(this.btnQuickStartA, new RoutedEventArgs());
        }
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
            this.OnPropertyChanged(nameof(this.ProcessStatus));
        }
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
        private void RandomMode() {
            Random rnd = new Random();
            this.RowsSet = rnd.Next(MinimumRows, MaximumRows);
            this.ColumnsSet = rnd.Next(MinimumColumns, MaximumColumns);
            this.MinesSet = rnd.Next(MinimumMines, MaximumMines);
            StartCurrentGame();
        }
        private void TimerUsingTimer_Tick(object sender, EventArgs e) {
            ++UsingTime;
        }
        #endregion
    }
}
