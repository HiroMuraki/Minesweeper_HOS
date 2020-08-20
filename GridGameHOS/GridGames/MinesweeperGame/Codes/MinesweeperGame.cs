using Common;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Effects;
using static Common.GeneralAction;

namespace MinesweepGameLite {
    public class MinesweeperGame : IGridGame, INotifyPropertyChanged {
        public GameType Type { get; private set; } = GameType.Minesweeper;
        public MinesweeperMain Game { get; set; }
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
        public MainGameWindow GameWindow { get; set; }
        public string GameSizeStatus {
            get {
                return $"{this.GameWindow.RowsSet} x {this.GameWindow.ColumnsSet}";
            }
        }
        public string ProcessStatus {
            get {
                return $"{this.Game.FlagsCount} | {this.GameWindow.MinesSet}";
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void GameBlock_OpenBlock(object sender, RoutedEventArgs e) {
            PlayFXSound(nameof(BlockClickSound));
            BlockCoordinate coordinate = (sender as GameBlockCoordinated).Coordinate;
            //嗅探猫
            if (GameWindow.ToggleDetector.IsChecked == true) {
                GameWindow.ToggleDetector.IsChecked = false;
                GameWindow.ToggleDetector.IsEnabled = false;
                GameWindow.DetectorBox.Visibility = Visibility.Collapsed;
                if (this.Game[coordinate].IsMineBlock) {
                    this.Game.FlagBlock(coordinate);
                } else {
                    this.Game.OpenBlock(coordinate);
                }
                this.Game.IsGameStarted = true;
                this.CalGame(this.Game.IsGameCompleted);
                return;
            }
            //首开保护
            if (!this.Game.IsGameStarted) {
                this.Game.ResetLayout(coordinate);
                this.OnPropertyChanged(nameof(BlocksArray));
                this.Game.IsGameStarted = true;
            }
            //普通打开
            this.Game.OpenBlock(coordinate);
            CalGame(this.Game.IsGameCompleted);
        }
        private void GameBlock_FlagBlock(object sender, RoutedEventArgs e) {
            PlayFXSound(nameof(BlockFlagSound));
            this.Game.FlagBlock((sender as GameBlockCoordinated).Coordinate);
            GameWindow.OnPropertyChanged(nameof(GameWindow.ProcessStatus));
        }
        private void GameBlock_QuickOpen(object sender, RoutedEventArgs e) {
            PlayFXSound(nameof(BlockClickSound));
            this.Game.OpenNearBlocks((sender as GameBlockCoordinated).Coordinate);
            CalGame(this.Game.IsGameCompleted);
        }
        private void ToggleDetector_Click(object sender, RoutedEventArgs e) {
            PlayFXSound(nameof(MenuButtonClickSound));
            if (GameWindow.ToggleDetector.IsChecked == true) {
                GameWindow.Cursor = DetectorAimerCursor;
            } else {
                GameWindow.Cursor = NormalCursor;
            }
        }

        private MinesweeperGame() { }
        /// <summary>
        /// 带参构造函数，传入一个MainGameWindow类，用于关联游戏窗口
        /// </summary>
        /// <param name="gameWindow">关联的游戏窗口</param>
        public MinesweeperGame(MainGameWindow gameWindow) {
            this.Game = new MinesweeperMain(BlockCreateAction);
            this.GameWindow = gameWindow;
            GameWindow.ToggleDetector.Visibility = Visibility.Visible;
            GameWindow.SliderMinesSet.Visibility = Visibility.Visible;
            GameWindow.LabelProcess.Visibility = Visibility.Visible;
            GameWindow.MaximumRows = 18;
            GameWindow.MinimumRows = 6;
            GameWindow.MaximumColumns = 30;
            GameWindow.MinimumColumns = 6;
            GameWindow.MinimumMines = 5;
            GameWindow.ToggleDetector.Click += ToggleDetector_Click;
        }
        /// <summary>
        /// 创建方块的方法
        /// </summary>
        /// <returns></returns>
        private IGameBlock BlockCreateAction() {
            GameBlockCoordinated block = new GameBlockCoordinated();
            block.Width = block.Height = 50;
            block.OpenBlock += GameBlock_OpenBlock;
            block.FlagBlock += GameBlock_FlagBlock;
            block.DoubleOpenBlock += GameBlock_QuickOpen;
            return block;
        }
        /// <summary>
        /// 结算游戏，若传入null，则无其它操作，传入false，打开所有方块，传入true，正常结算
        /// </summary>
        /// <param name="isGameCompleted">游戏是否完成</param>
        private void CalGame(bool? isGameCompleted) {
            this.GameWindow.CalCurrentGame(isGameCompleted);
            if (isGameCompleted == false) {
                this.Game.OpenAllBlocks();
            }
        }
        /// <summary>
        /// 开始当前游戏
        /// </summary>
        public void StartGame() {
            if (this.Game.RowSize != GameWindow.RowsSet
                || this.Game.ColumnSize != GameWindow.ColumnsSet
                || this.Game.MineSize != GameWindow.MinesSet) {
                this.Game.SetGame(GameWindow.RowsSet, GameWindow.ColumnsSet, GameWindow.MinesSet);
            }
            this.Game.StartGame();
        }
        /// <summary>
        /// 快速游戏
        /// </summary>
        /// <param name="level">传入整数数字（0-2），开始制定难度的游戏</param>
        public void QuickGame(int level) {
            switch (level) {
                case 0:
                    GameWindow.RowsSet = 9;
                    GameWindow.ColumnsSet = 9;
                    GameWindow.MinesSet = 10;
                    break;
                case 1:
                    GameWindow.RowsSet = 16;
                    GameWindow.ColumnsSet = 16;
                    GameWindow.MinesSet = 40;
                    break;
                case 2:
                    GameWindow.RowsSet = 16;
                    GameWindow.ColumnsSet = 30;
                    GameWindow.MinesSet = 99;
                    break;
            }
        }
        /// <summary>
        /// 卸载游戏时的操作，由游戏窗口调用
        /// </summary>
        public void UnloadGame() {
            this.GameWindow.ToggleDetector.Click -= ToggleDetector_Click;
        }
    }
}
