using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace SlideJigsawGameLite {
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SlideJigsawGameWindow : Window, INotifyPropertyChanged {
        #region 属性与字段
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public SliderJigsawGame CurrentGame { get; set; }
        private int rowsSet;
        public int RowsSet {
            get {
                return rowsSet;
            }
            set {
                rowsSet = value;
                OnPropertyChanged(nameof(RowsSet));
            }
        }
        private int columnsSet;
        public int ColumnsSet {
            get {
                return columnsSet;
            }
            set {
                columnsSet = value;
                OnPropertyChanged(nameof(ColumnsSet));
            }
        }

        public int MinimumRows { get { return 3; } }
        public int MaximumRows { get { return 10; } }
        public int MinimumColumns { get { return 3; } }
        public int MaximumColumns { get { return 10; } }
        #endregion

        #region 控件
        public SlideJigsawGameWindow() {
            InitializeComponent();
            this.DataContext = this;
            this.CurrentGame = new SliderJigsawGame(BlockCreatAction);
            this.RowsSet = this.ColumnsSet = 4;
            StartCurrentGame();
        }
        private void GameStartButton_Click(object sender, RoutedEventArgs e) {
            this.StartCurrentGame();
        }
        private void GameBlock_ButtonClick(object sender, RoutedEventArgs e) {
            this.CurrentGame.SwapWithNullBlock((sender as IGameBlock).Coordinate);
            if (this.CurrentGame.IsGameCompleted) {
                CalGame();
            }
        }
        #endregion

        #region 包装方法
        private void StartCurrentGame() {
            this.CurrentGame.SetGame(this.RowsSet, this.ColumnsSet);
            this.CurrentGame.StartGame();
            this.gameStartButton.IsOn = null;
        }
        private void CalGame() {
            this.gameStartButton.IsOn = true;
            MessageBox.Show("yztxdy");
        }
        private IGameBlock BlockCreatAction() {
            GameBlockCoordinated block = new GameBlockCoordinated();
            block.Width = block.Height = 50;
            block.ButtonClick += GameBlock_ButtonClick;
            return block;
        }
        #endregion

        #region 窗口操作
        private void Window_Close(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }
        private void Window_KeyDown(object sender, KeyEventArgs e) {
            switch (e.Key) {
                case Key.Up:
                case Key.W:
                    this.CurrentGame.SwapWithNullBlock(this.CurrentGame.NullBlockCoordiante.Add(1, 0));
                    break;
                case Key.Down:
                case Key.S:
                    this.CurrentGame.SwapWithNullBlock(this.CurrentGame.NullBlockCoordiante.Add(-1, 0));
                    break;
                case Key.Left:
                case Key.A:
                    this.CurrentGame.SwapWithNullBlock(this.CurrentGame.NullBlockCoordiante.Add(0, 1));
                    break;
                case Key.Right:
                case Key.D:
                    this.CurrentGame.SwapWithNullBlock(this.CurrentGame.NullBlockCoordiante.Add(0, -1));
                    break;
            }
        }
        private void Window_Move(object sender, MouseButtonEventArgs e) {
            if (e.ClickCount == 2) {
                if (this.WindowState == WindowState.Maximized) {
                    this.WindowState = WindowState.Normal;
                } else {
                    this.WindowState = WindowState.Maximized;
                }
            } else {
                this.DragMove();
            }
        }
        #endregion

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e) {
            if (this.Width < 500 || this.Height < 500) {
                this.setting.Visibility = Visibility.Collapsed;
            } else {
                this.setting.Visibility = Visibility.Visible;
            }
        }
    }
}
