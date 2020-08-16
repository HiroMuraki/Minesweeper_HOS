﻿using Common;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Effects;
using static Common.GeneralAction;

namespace MinesweepGameLite {
    public class MinesweeperGame : IGridGame, INotifyPropertyChanged {
        public GameType Type { get; private set; } = GameType.Minesweeper;
        public MinesweeperMain Game { get; set; }
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
            this.GameWindow.CalCurrentGame(isGameCompleted);
            if (isGameCompleted == false) {
                this.Game.OpenAllBlocks();
            }
        }
        public void StartGame() {
            if (this.Game.RowSize != GameWindow.RowsSet
                || this.Game.ColumnSize != GameWindow.ColumnsSet
                || this.Game.MineSize != GameWindow.MinesSet) {
                this.Game.SetGame(GameWindow.RowsSet, GameWindow.ColumnsSet, GameWindow.MinesSet);
            }
            this.Game.StartGame();
            OnPropertyChanged(nameof(BlocksArray));
        }
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
        public void UnloadGame() { }
    }
}
