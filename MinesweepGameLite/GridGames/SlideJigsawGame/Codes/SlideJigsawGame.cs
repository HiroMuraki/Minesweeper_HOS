using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Effects;
using static Common.GeneralAction;
using Common;

namespace SlideJigsawGameLite {
    class SlideJigsawGame : IGridGame, INotifyPropertyChanged {
        public GameType Type { get; private set; } = GameType.SlideJigsaw;
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public MainGameWindow GameWindow { get; set; }
        public SlideJigsawMain Game { get; set; }
        public string GameSizeStatus {
            get {
                return $"{GameWindow.RowsSet} x {GameWindow.ColumnsSet}";
            }
        }
        public string ProcessStatus { get { return null; } }

        #region 控件
        private SlideJigsawGame() { }
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
        private void GameBlock_ButtonClick(object sender, RoutedEventArgs e) {
            PlayFXSound(nameof(BlockClickSound));
            this.Game.SwapWithNullBlock((sender as IGameBlock).Coordinate);
            if (this.Game.IsGameCompleted) {
                CalGame();
            }
        }
        private void Window_KeyDown(object sender, KeyEventArgs e) {
            switch (e.Key) {
                case Key.Up:
                case Key.W:
                    this.Game.SwapWithNullBlock(this.Game.NullBlockCoordiante.Add(1, 0));
                    break;
                case Key.Down:
                case Key.S:
                    this.Game.SwapWithNullBlock(this.Game.NullBlockCoordiante.Add(-1, 0));
                    break;
                case Key.Left:
                case Key.A:
                    this.Game.SwapWithNullBlock(this.Game.NullBlockCoordiante.Add(0, 1));
                    break;
                case Key.Right:
                case Key.D:
                    this.Game.SwapWithNullBlock(this.Game.NullBlockCoordiante.Add(0, -1));
                    break;
            }
        }
        #endregion

        #region 包装方法
        private IGameBlock BlockCreatAction() {
            GameBlockCoordinated block = new GameBlockCoordinated();
            block.Width = block.Height = 50;
            block.ButtonClick += GameBlock_ButtonClick;
            return block;
        }
        public void StartGame() {
            GameWindow.Cursor = LoadingGameCursor;
            this.Game.SetGame(GameWindow.RowsSet, GameWindow.ColumnsSet);
            this.Game.StartGame();
            //重置统计
            GameWindow.borderGamePanelCover.IsHitTestVisible = false;
            GameWindow.gamePlayAreaGrid.Effect = null;
            GameWindow.gameCompleteBarImage.IsEnabled = false;
            GameWindow.ToggleDetector.IsEnabled = true;
            GameWindow.btnStartGame.IsOn = null;
            GameWindow.UsingTime = 0;
            GameWindow.UsingTimeTimer.Start();
            GameWindow.Cursor = NormalCursor;
            GameWindow.OnPropertyChanged(nameof(GameWindow.ProcessStatus));
        }
        public void QuickStartGame(int level) {
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
            this.StartGame();
        }
        public void UnloadGame() {
            GameWindow.KeyDown -= this.Window_KeyDown;
        }
        private void CalGame() {
            GameWindow.UsingTimeTimer.Stop();
            GameWindow.borderGamePanelCover.IsHitTestVisible = true;
            GameWindow.btnStartGame.IsOn = true;
            GameWindow.gameCompleteBarImage.IsEnabled = true;
            GameWindow.gamePlayAreaGrid.Effect = new BlurEffect {
                KernelType = KernelType.Gaussian,
                Radius = 0
            };
            GameWindow.gamePlayAreaGrid.Effect.BeginAnimation(BlurEffect.RadiusProperty, AnimationForBlurEffect);
            //MessageBox.Show("yztxdy");
            //MessageBox.Show("YZTXDY"); ;
        }
        
        #endregion
    }
}
