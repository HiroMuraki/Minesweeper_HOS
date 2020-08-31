using GridGameHOS.Common;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using static GridGameHOS.Common.GeneralAction;

namespace GridGameHOS.TwoZeroFourEightLite {
    public class TwoZeroFourEightGame : IGridGame, INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        public MainGameWindow GameWindow { get; set; }
        public TwoZeroFourEightMain Game { get; private set; }
        public GameType Type { get; set; } = GameType.Minesweeper;
        public string GameSizeStatus {
            get {
                return $"{GameWindow.RowsSet} x {GameWindow.RowsSet} | {GetTargetNumber(GameWindow.ColumnsSet)}";
            }
        }
        public string ProcessStatus {
            get {
                return $"{Game.Scores}";
            }
        }
        public int RowSize {
            get {
                return Game.RowSize;
            }
        }
        public int ColumnSize {
            get {
                return Game.ColumnSize;
            }
        }
        public ObservableCollection<IBlocks> BlocksArray {
            get {
                return new ObservableCollection<IBlocks>(Game.Blocks.Values);
            }
        }
        private TwoZeroFourEightGame() {

        }
        public TwoZeroFourEightGame(MainGameWindow gameWindow) {
            Game = new TwoZeroFourEightMain(BlockCreateAction);
            GameWindow = gameWindow;
            GameWindow.KeyDown += Window_KeyDown;
            GameWindow.MaximumRows = 6;
            GameWindow.MinimumRows = 4;
            GameWindow.MaximumColumns = 4;
            GameWindow.MinimumColumns = 1;
            GameWindow.SliderMinesSet.Visibility = Visibility.Collapsed;
            GameWindow.LabelProcess.Visibility = Visibility.Visible;
            GameWindow.ToggleDetector.Visibility = Visibility.Visible;
            GameWindow.ToggleDetector.Click += ToggleDetector_Click;
        }
        /// <summary>
        /// 游戏技能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToggleDetector_Click(object sender, RoutedEventArgs e) {
            PlayFXSound(nameof(MenuButtonClickSound));
            GameWindow.ToggleDetector.IsEnabled = false;
            GameWindow.ToggleDetector.IsChecked = false;
            GameWindow.DetectorBox.Visibility = Visibility.Collapsed;
            Game.TransToNormalType();
        }
        /// <summary>
        /// 开始游戏
        /// </summary>
        public void StartGame() {
            Game.StartGame(GameWindow.RowsSet, GetTargetNumber(GameWindow.ColumnsSet));
            Game.GenerateNumber();
            Game.GenerateNumber();
            GameWindow.ToggleDetector.IsEnabled = true;
        }
        public void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public void QuickGame(int level) {
            switch (level) {
                case 0:
                    GameWindow.RowsSet = 4;
                    GameWindow.ColumnsSet = 1;
                    break;
                case 1:
                    GameWindow.RowsSet = 4;
                    GameWindow.ColumnsSet = 3;
                    break;
                case 2:
                    GameWindow.RowsSet = 5;
                    GameWindow.ColumnsSet = 4;
                    break;
            }
            StartGame();
        }
        public void UnloadGame() {
            GameWindow.ToggleDetector.Click -= ToggleDetector_Click;
            GameWindow.KeyDown -= Window_KeyDown;
        }
        private void Window_KeyDown(object sender, KeyEventArgs e) {
            switch (e.Key) {
                case Key.W:
                case Key.Up:
                    PlayFXSound(nameof(BlockClickSound));
                    Game.MoveToNorth();
                    Game.GenerateNumber();
                    break;
                case Key.S:
                case Key.Down:
                    PlayFXSound(nameof(BlockClickSound));
                    Game.MoveToSouth();
                    Game.GenerateNumber();
                    break;
                case Key.A:
                case Key.Left:
                    PlayFXSound(nameof(BlockClickSound));
                    Game.MoveToWest();
                    Game.GenerateNumber();
                    break;
                case Key.D:
                case Key.Right:
                    PlayFXSound(nameof(BlockClickSound));
                    Game.MoveToEast();
                    Game.GenerateNumber();
                    break;
            }
            if (Game.IsGameCompleted) {
                GameWindow.CalCurrentGame(true);
            }
            GameWindow.OnPropertyChanged(nameof(ProcessStatus));
        }
        /// <summary>
        /// 根据滑杆设置获取对应的游戏目标
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        private int GetTargetNumber(int setting) {
            switch (setting) {
                case 1:
                    return 512;
                case 2:
                    return 1024;
                case 3:
                    return 2048;
                case 4:
                    return 4096;
                default:
                    return 2048;
            }
        }
        public IGameBlock BlockCreateAction() {
            GameBlock block = new GameBlock();
            block.Width = block.Height = 50;
            return block;
        }
    }
}
