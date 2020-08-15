using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Effects;
using Common;

namespace SlideJigsawGameLite {
    class SlideJigsawMain : IGridGame, INotifyPropertyChanged {
        public GameType Type { get; private set; } = GameType.SlideJigsaw;
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public MainGameWindow GameWindow { get; set; }
        public SlideJigsawGame Game { get; set; }
        public string GameSizeStatus {
            get {
                return $"{GameWindow.RowsSet} x {GameWindow.ColumnsSet}";
            }
        }
        public string ProcessStatus { get { return null; } }

        #region 控件
        private SlideJigsawMain() { }
        public SlideJigsawMain(MainGameWindow gameWindow) {
            this.Game = new SlideJigsawGame(BlockCreatAction);
            this.GameWindow = gameWindow;
            GameWindow.ToggleDetector.Visibility = Visibility.Collapsed;
            GameWindow.SliderMinesSet.Visibility = Visibility.Collapsed;
            gameWindow.LabelProcess.Visibility = Visibility.Collapsed;
            GameWindow.MaximumRows = 10;
            GameWindow.MinimumRows = 3;
            GameWindow.MaximumColumns = 10;
            GameWindow.MinimumColumns = 3;
            GameWindow.MinimumMines = 0;
        }
        private void GameBlock_ButtonClick(object sender, RoutedEventArgs e) {
            App.PlayFXSound(nameof(App.BlockClickSound));
            this.Game.SwapWithNullBlock((sender as IGameBlock).Coordinate);
            if (this.Game.IsGameCompleted) {
                CalGame();
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
            GameWindow.Cursor = App.LoadingGameCursor;
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
            GameWindow.Cursor = App.NormalCursor;
            OnPropertyChanged(nameof(ProcessStatus));
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
            GameWindow.gamePlayAreaGrid.Effect.BeginAnimation(BlurEffect.RadiusProperty, App.AnimationForBlurEffect);
            //MessageBox.Show("yztxdy");
            //MessageBox.Show("YZTXDY"); ;
        }
        #endregion
    }
}
