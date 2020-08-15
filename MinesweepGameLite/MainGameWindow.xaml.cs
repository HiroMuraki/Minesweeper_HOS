using MinesweepGameLite;
using SlideJigsawGameLite;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Input.StylusWisp;
using System.Windows.Threading;

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
            this.Cursor = App.NormalCursor;
            this.DataContext = this;
            this.UsingTimeTimer.Interval = TimeSpan.FromSeconds(1);
            this.UsingTimeTimer.Tick += TimerUsingTimer_Tick;
            LoadGame(App.DefaultGame);
        }
        public void LoadGame(GameType gameType) {
            switch (gameType) {
                case GameType.Minesweeper:
                    this.CurrentGame = new MinesweeperGame(this);
                    break;
                case GameType.SlideJigsaw:
                    this.CurrentGame = new SlideJigsawGame(this);
                    break;
            }
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
                    if (this.CurrentGame is MinesweeperGame) {
                        this.RowsSet = 9;
                        this.ColumnsSet = 9;
                        this.MinesSet = 10;
                    } else {
                        this.RowsSet = 3;
                        this.ColumnsSet = 3;
                        this.MinesSet = 0;
                    }
                    break;
                case "btnQuickStartB":
                    if (this.CurrentGame is MinesweeperGame) {
                        this.RowsSet = 16;
                        this.ColumnsSet = 16;
                        this.MinesSet = 40;
                    } else {
                        this.RowsSet = 4;
                        this.ColumnsSet = 4;
                        this.MinesSet = 0;
                    }
                    break;
                case "btnQuickStartC":
                    if (this.CurrentGame is MinesweeperGame) {
                        this.RowsSet = 16;
                        this.ColumnsSet = 30;
                        this.MinesSet = 99;
                    } else {
                        this.RowsSet = 5;
                        this.ColumnsSet = 5;
                        this.MinesSet = 0;
                    }
                    break;
            }
            this.StartCurrentGame();
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
        private void SwitchGameButton_Click(object sender, RoutedEventArgs e) {
            if (this.CurrentGame.Type == GameType.Minesweeper) {
                LoadGame(GameType.SlideJigsaw);
            } else {
                LoadGame(GameType.Minesweeper);
            }
        }
        private void borderGamePanelCover_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            App.PlayFXSound(nameof(App.MenuButtonClickSound));
            StartCurrentGame();
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
            this.ToggleDetector.IsChecked = false;
        }
        private void Window_MouseUp(object sender, MouseButtonEventArgs e) {
            this.Cursor = App.NormalCursor;
        }
        #endregion

        #region 包装方法
        public void OnPropertyChanged(string propertyName) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void StartCurrentGame() {
            this.CurrentGame.StartGame();
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
