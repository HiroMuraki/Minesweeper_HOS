using Common;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Media.Effects;

namespace MinesweepGameLite {
    public class MinesweeperGame : IGridGame, INotifyPropertyChanged {
        public GameType Type { get; private set; } = GameType.Minesweeper;
        public MinesweeperMain Game { get; set; }
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
            App.PlayFXSound(nameof(App.BlockClickSound));
            BlockCoordinate coordinate = (sender as GameBlockCoordinated).Coordinate;
            //嗅探猫
            if (GameWindow.ToggleDetector.IsChecked == true) {
                GameWindow.ToggleDetector.IsChecked = false;
                GameWindow.ToggleDetector.IsEnabled = false;
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
                this.Game.IsGameStarted = true;
            }
            //普通打开
            this.Game.OpenBlock(coordinate);
            CalGame(this.Game.IsGameCompleted);
        }
        private void GameBlock_FlagBlock(object sender, RoutedEventArgs e) {
            App.PlayFXSound(nameof(App.BlockFlagSound));
            this.Game.FlagBlock((sender as GameBlockCoordinated).Coordinate);
            GameWindow.OnPropertyChanged(nameof(GameWindow.ProcessStatus));
        }
        private void GameBlock_QuickOpen(object sender, RoutedEventArgs e) {
            App.PlayFXSound(nameof(App.BlockClickSound));
            this.Game.OpenNearBlocks((sender as GameBlockCoordinated).Coordinate);
            CalGame(this.Game.IsGameCompleted);
        }
        private void ToggleDetector_Click(object sender, RoutedEventArgs e) {
            App.PlayFXSound(nameof(App.MenuButtonClickSound));
            if (GameWindow.ToggleDetector.IsChecked == true) {
                GameWindow.Cursor = App.DetectorAimerCursor;
            } else {
                GameWindow.Cursor = App.NormalCursor;
            }
        }

        private MinesweeperGame() { }
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
        private IGameBlock BlockCreateAction() {
            GameBlockCoordinated block = new GameBlockCoordinated();
            block.Width = block.Height = 50;
            block.OpenBlock += GameBlock_OpenBlock;
            block.FlagBlock += GameBlock_FlagBlock;
            block.DoubleOpenBlock += GameBlock_QuickOpen;
            return block;
        }
        private void CalGame(bool? isGameCompleted) {
            if (isGameCompleted == null) {
                return;
            }
            GameWindow.UsingTimeTimer.Stop();
            GameWindow.borderGamePanelCover.IsHitTestVisible = true;
            if (isGameCompleted == true) {
                GameWindow.btnStartGame.IsOn = true;
                GameWindow.gameCompleteBarImage.IsEnabled = true;
                GameWindow.gamePlayAreaGrid.Effect = new BlurEffect {
                    KernelType = KernelType.Gaussian,
                    Radius = 0
                };
                GameWindow.gamePlayAreaGrid.Effect.BeginAnimation(BlurEffect.RadiusProperty, App.AnimationForBlurEffect);
                //MessageBox.Show("YZTXDY"); ;
            } else {
                this.Game.OpenAllBlocks();
            }
        }
        public void StartGame() {
            GameWindow.Cursor = App.LoadingGameCursor;
            //开始游戏
            if (this.Game.RowSize != GameWindow.RowsSet
                || this.Game.ColumnSize != GameWindow.ColumnsSet
                || this.Game.MineSize != GameWindow.MinesSet) {
                this.Game.SetGame(GameWindow.RowsSet, GameWindow.ColumnsSet, GameWindow.MinesSet);
            }
            this.Game.StartGame();
            //重置统计
            GameWindow.borderGamePanelCover.IsHitTestVisible = false;
            GameWindow.gamePlayAreaGrid.Effect = null;
            GameWindow.gameCompleteBarImage.IsEnabled = false;
            GameWindow.ToggleDetector.IsEnabled = true;
            GameWindow.btnStartGame.IsOn = null;
            GameWindow.UsingTime = 0;
            GameWindow.UsingTimeTimer.Start();
            GameWindow.Cursor = App.NormalCursor;
            GameWindow.OnPropertyChanged(nameof(GameWindow.ProcessStatus));
        }
    }
}
