using Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static Common.GeneralAction;

namespace TwoZeroFourEightLite {
    public class TwoZeroFourEightGame : IGridGame, INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        public MainGameWindow GameWindow { get; set; }
        public TwoZeroFourEightMain Game { get; private set; }
        public GameType Type { get; set; } = GameType.Minesweeper;
        public string GameSizeStatus {
            get {
                return $"{this.GameWindow.RowsSet} x {this.GameWindow.RowsSet} | {this.GameWindow.ColumnsSet << 9}";
            }
        }
        public string ProcessStatus {
            get {
                return $"{this.Game.TargetNumber}";
            }
        }
        public ObservableCollection<IBlocks> BlocksArray {
            get {
                return new ObservableCollection<IBlocks>(this.Game.Blocks.Values);
            }
        }
        private TwoZeroFourEightGame() {

        }
        public TwoZeroFourEightGame(MainGameWindow gameWindow) {
            this.Game = new TwoZeroFourEightMain(BlockCreateAction);
            this.GameWindow = gameWindow;
            this.GameWindow.KeyDown += Window_KeyDown;
            this.GameWindow.MaximumRows = 6;
            this.GameWindow.MinimumRows = 4;
            this.GameWindow.MaximumColumns = 8;
            this.GameWindow.MinimumColumns = 1;
            this.GameWindow.SliderMinesSet.Visibility = Visibility.Collapsed;
            this.GameWindow.LabelProcess.Visibility = Visibility.Visible;
        }

        public void StartGame() {
            this.Game.StartGame(GameWindow.RowsSet, GameWindow.ColumnsSet << 9);
            this.Game.GenerateNumber();
            this.Game.GenerateNumber();
            OnPropertyChanged(nameof(BlocksArray));
        }

        public void OnPropertyChanged(string propertyName) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public void QuickGame(int level) {
            switch (level) {
                case 0:
                    GameWindow.RowsSet = 4;
                    GameWindow.ColumnsSet = 4;
                    break;
                case 1:
                    GameWindow.RowsSet = 5;
                    GameWindow.ColumnsSet = 8;
                    break;
                case 2:
                    GameWindow.RowsSet = 4;
                    GameWindow.ColumnsSet = 8;
                    break;
            }
            this.StartGame();
        }
        public void UnloadGame() {
            this.GameWindow.KeyDown -= Window_KeyDown;
        }
        private void Window_KeyDown(object sender, KeyEventArgs e) {
            PlayFXSound(nameof(BlockClickSound));
            switch (e.Key) {
                case Key.W:
                case Key.Up:
                    this.Game.MoveToNorth();
                    break;
                case Key.S:
                case Key.Down:
                    this.Game.MoveToSouth();
                    break;
                case Key.A:
                case Key.Left:
                    this.Game.MoveToWest();
                    break;
                case Key.D:
                case Key.Right:
                    this.Game.MoveToEast();
                    break;
            }
            this.Game.GenerateNumber();
            if (this.Game.IsGameCompleted) {
                GameWindow.CalCurrentGame(true);
            }
        }

        public IGameBlock BlockCreateAction() {
            GameBlock block = new GameBlock();
            block.Width = block.Height = 50;
            return block;
        }
    }
}
