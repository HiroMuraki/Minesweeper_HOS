using Common;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Effects;
using static Common.GeneralAction;

namespace SlideJigsawGameLite {
    class SlideJigsawGame : IGridGame, INotifyPropertyChanged {
        public GameType Type { get; private set; } = GameType.SlideJigsaw;
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public MainGameWindow GameWindow { get; set; }
        public SlideJigsawMain Game { get; set; }
        public int RowSize {
            get {
                return this.Game.RowSize;
            }
        }
        public int ColumnSize {
            get {
                return this.Game.ColumnSize;
            }
        }
        public ObservableCollection<IBlocks> BlocksArray {
            get {
                return new ObservableCollection<IBlocks>(this.Game.Blocks.Values);
            }
        }
        public string GameSizeStatus {
            get {
                return $"{GameWindow.RowsSet} x {GameWindow.ColumnsSet}";
            }
        }
        public string ProcessStatus { get { return null; } }

        #region 控件
        private SlideJigsawGame() { }
        /// <summary>
        /// 带参构造函数，传入一个MainGameWindow类，用于关联游戏窗口
        /// </summary>
        /// <param name="gameWindow">关联的游戏窗口</param>
        public SlideJigsawGame(MainGameWindow gameWindow) {
            this.Game = new SlideJigsawMain(BlockCreatAction);
            this.GameWindow = gameWindow;
            GameWindow.ToggleDetector.Visibility = Visibility.Collapsed;
            GameWindow.SliderMinesSet.Visibility = Visibility.Collapsed;
            GameWindow.LabelProcess.Visibility = Visibility.Collapsed;
            GameWindow.MaximumRows = 10;
            GameWindow.MinimumRows = 3;
            GameWindow.MaximumColumns = 10;
            GameWindow.MinimumColumns = 3;
            GameWindow.MinimumMines = 0;
            GameWindow.KeyDown += this.Window_KeyDown;
        }
        /// <summary>
        /// 点击方块时的操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameBlock_ButtonClick(object sender, RoutedEventArgs e) {
            PlayFXSound(nameof(BlockClickSound));
            this.Game.SwapWithNullBlock((sender as IGameBlock).Coordinate);
            if (this.Game.IsGameCompleted) {
                CalGame();
            }
        }
        /// <summary>
        /// 方块移动的快捷键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyDown(object sender, KeyEventArgs e) {
            switch (e.Key) {
                case Key.Up:
                case Key.W:
                    PlayFXSound(nameof(BlockClickSound));
                    this.Game.SwapWithNullBlock(this.Game.NullBlockCoordiante.South);
                    break;
                case Key.Down:
                case Key.S:
                    PlayFXSound(nameof(BlockClickSound));
                    this.Game.SwapWithNullBlock(this.Game.NullBlockCoordiante.North);
                    break;
                case Key.Left:
                case Key.A:
                    PlayFXSound(nameof(BlockClickSound));
                    this.Game.SwapWithNullBlock(this.Game.NullBlockCoordiante.East);
                    break;
                case Key.Right:
                case Key.D:
                    PlayFXSound(nameof(BlockClickSound));
                    this.Game.SwapWithNullBlock(this.Game.NullBlockCoordiante.West);
                    break;
            }
            if (this.Game.IsGameCompleted) {
                CalGame();
            }
        }
        #endregion

        #region 包装方法
        /// <summary>
        /// 创建方块的方法
        /// </summary>
        /// <returns></returns>
        private IGameBlock BlockCreatAction() {
            GameBlock block = new GameBlock();
            block.Width = block.Height = 50;
            block.ButtonClick += GameBlock_ButtonClick;
            return block;
        }
        /// <summary>
        /// 开始当前游戏
        /// </summary>
        public void StartGame() {
            this.Game.SetGame(GameWindow.RowsSet, GameWindow.ColumnsSet);
            this.Game.StartGame();
        }
        /// <summary>
        /// 快速游戏
        /// </summary>
        /// <param name="level">传入一个整数数字（0-2），开始制定难度的游戏</param>
        public void QuickGame(int level) {
            switch (level) {
                case 0:
                    GameWindow.RowsSet = 3;
                    GameWindow.ColumnsSet = 3;
                    break;
                case 1:
                    GameWindow.RowsSet = 4;
                    GameWindow.ColumnsSet = 4;
                    break;
                case 2:
                    GameWindow.RowsSet = 5;
                    GameWindow.ColumnsSet = 5;
                    break;
            }
        }
        /// <summary>
        /// 卸载游戏时进行的操作，由游戏主窗口调用
        /// </summary>
        public void UnloadGame() {
            GameWindow.KeyDown -= this.Window_KeyDown;
        }
        /// <summary>
        /// 结算游戏
        /// </summary>
        private void CalGame() {
            this.GameWindow.CalCurrentGame();
            //MessageBox.Show("yztxdy");
            //MessageBox.Show("YZTXDY"); ;
        }
        #endregion
    }
}
