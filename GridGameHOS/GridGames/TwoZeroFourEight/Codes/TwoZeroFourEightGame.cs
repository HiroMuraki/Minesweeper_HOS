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
                return $"{this.GameWindow.RowsSet} x {this.GameWindow.RowsSet} | {GetTargetNumber(this.GameWindow.ColumnsSet)}";
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
            this.GameWindow.MaximumColumns = 4;
            this.GameWindow.MinimumColumns = 1;
            this.GameWindow.SliderMinesSet.Visibility = Visibility.Collapsed;
            this.GameWindow.LabelProcess.Visibility = Visibility.Visible;
            this.GameWindow.ToggleDetector.Click += ToggleDetector_Click;
        }

        private void ToggleDetector_Click(object sender, RoutedEventArgs e) {
            PlayFXSound(nameof(MenuButtonClickSound));
            this.GameWindow.ToggleDetector.IsEnabled = false;
            this.GameWindow.ToggleDetector.IsChecked = false;
            this.Game.ClearNumber(2);
        }

        public void StartGame() {
            this.Game.StartGame(GameWindow.RowsSet, GetTargetNumber(GameWindow.ColumnsSet));
            this.Game.GenerateNumber();
            this.Game.GenerateNumber();
            this.GameWindow.ToggleDetector.IsEnabled = true;
            OnPropertyChanged(nameof(BlocksArray));
        }

        public void OnPropertyChanged(string propertyName) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
            this.StartGame();
        }
        public void UnloadGame() {
            this.GameWindow.ToggleDetector.Click -= ToggleDetector_Click;
            this.GameWindow.KeyDown -= Window_KeyDown;
        }
        private void Window_KeyDown(object sender, KeyEventArgs e) {
            switch (e.Key) {
                case Key.W:
                case Key.Up:
                    PlayFXSound(nameof(BlockClickSound));
                    this.Game.MoveToNorth();
                    this.Game.GenerateNumber();
                    break;
                case Key.S:
                case Key.Down:
                    PlayFXSound(nameof(BlockClickSound));
                    this.Game.MoveToSouth();
                    this.Game.GenerateNumber();
                    break;
                case Key.A:
                case Key.Left:
                    PlayFXSound(nameof(BlockClickSound));
                    this.Game.MoveToWest();
                    this.Game.GenerateNumber();
                    break;
                case Key.D:
                case Key.Right:
                    PlayFXSound(nameof(BlockClickSound));
                    this.Game.MoveToEast();
                    this.Game.GenerateNumber();
                    break;
            }
            if (this.Game.IsGameCompleted) {
                GameWindow.CalCurrentGame(true);
            }
        }

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
