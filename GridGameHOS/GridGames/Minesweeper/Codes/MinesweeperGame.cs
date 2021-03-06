﻿using GridGameHOS.Common;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using static GridGameHOS.Common.GeneralAction;

namespace GridGameHOS.Minesweeper {
    public class MinesweeperGame : IGridGame, INotifyPropertyChanged {
        public GameType Type { get; private set; } = GameType.Minesweeper;
        public MinesweeperMain Game { get; set; }
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
        public MainGameWindow GameWindow { get; set; }
        public string GameSizeStatus {
            get {
                return $"{GameWindow.RowsSet} x {GameWindow.ColumnsSet}";
            }
        }
        public string ProcessStatus {
            get {
                return $"{Game.FlagsCount} | {GameWindow.MinesSet}";
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void GameBlock_OpenBlock(object sender, RoutedEventArgs e) {
            PlayFXSound(nameof(BlockClickSound));
            BlockCoordinate coordinate = (sender as GameBlock).Coordinate;
            //嗅探猫
            if (GameWindow.ToggleDetector.IsChecked == true) {
                GameWindow.ToggleDetector.IsChecked = false;
                GameWindow.ToggleDetector.IsEnabled = false;
                GameWindow.DetectorBox.Visibility = Visibility.Collapsed;
                if (Game[coordinate].IsMineBlock) {
                    Game.FlagBlock(coordinate);
                } else {
                    Game.OpenBlock(coordinate);
                }
                Game.IsGameStarted = true;
                CalGame(Game.IsGameCompleted);
                return;
            }
            //首开保护
            if (!Game.IsGameStarted) {
                Game.ResetLayout(coordinate);
                OnPropertyChanged(nameof(BlocksArray));
                Game.IsGameStarted = true;
            }
            //普通打开
            Game.OpenBlock(coordinate);
            CalGame(Game.IsGameCompleted);
        }
        private void GameBlock_FlagBlock(object sender, RoutedEventArgs e) {
            PlayFXSound(nameof(BlockFlagSound));
            Game.FlagBlock((sender as GameBlock).Coordinate);
            GameWindow.OnPropertyChanged(nameof(GameWindow.ProcessStatus));
        }
        private void GameBlock_QuickOpen(object sender, RoutedEventArgs e) {
            PlayFXSound(nameof(BlockClickSound));
            Game.OpenNearBlocks((sender as GameBlock).Coordinate);
            CalGame(Game.IsGameCompleted);
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
            Game = new MinesweeperMain(BlockCreateAction);
            GameWindow = gameWindow;
            GameWindow.ToggleDetector.Visibility = Visibility.Visible;
            GameWindow.SliderMinesSet.Visibility = Visibility.Visible;
            GameWindow.LabelProcess.Visibility = Visibility.Visible;
            GameWindow.MaximumRows = 18;
            GameWindow.MinimumRows = 6;
            GameWindow.MaximumColumns = 30;
            GameWindow.MinimumColumns = 6;
            GameWindow.MinimumMines = 5;
            GameWindow.ToggleDetector.Click += ToggleDetector_Click;
            GameWindow.AllowDrop = true;
            GameWindow.DragEnter += GameWindow_DragEnter;
            GameWindow.DragLeave += GameWindow_DragLeave;
            GameWindow.BorderGamePanelCover.AllowDrop = true;
            GameWindow.BorderGamePanelCover.DragEnter += BorderGamePanelCover_DragEnter;
            GameWindow.BorderGamePanelCover.DragLeave += BorderGamePanelCover_DragLeave;
            GameWindow.BorderGamePanelCover.Drop += BorderGamePanelCover_Drop;
        }

        private void BorderGamePanelCover_Drop(object sender, DragEventArgs e) {
            string filePath = ((string[])e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            LayoutSetting setting = GameLayoutLoader.ReadFromFile(filePath);
            GameWindow.StartCustomGame(() => { StartCustomGame(setting); });

            PlayOpacityTransform(GameWindow.BorderGamePanelCover,
                GameWindow.BorderGamePanelCover.Opacity, 0, 150);
            PlayBlurTransfrom(GameWindow.GamePlayAreaGrid, 15, 0, 150);
            GameWindow.BorderGamePanelCover.IsHitTestVisible = false;
        }
        private void BorderGamePanelCover_DragLeave(object sender, DragEventArgs e) {
            PlayOpacityTransform(GameWindow.BorderGamePanelCover,
                GameWindow.BorderGamePanelCover.Opacity, 0, 150);
            PlayBlurTransfrom(GameWindow.GamePlayAreaGrid, 15, 0, 150);
        }
        private void BorderGamePanelCover_DragEnter(object sender, DragEventArgs e) {
            PlayOpacityTransform(GameWindow.BorderGamePanelCover,
                GameWindow.BorderGamePanelCover.Opacity, 0.65, 150);
            PlayBlurTransfrom(GameWindow.GamePlayAreaGrid, 0, 15, 150);
        }
        private void GameWindow_DragLeave(object sender, DragEventArgs e) {
            GameWindow.BorderGamePanelCover.IsHitTestVisible = false;
        }
        private void GameWindow_DragEnter(object sender, DragEventArgs e) {
            GameWindow.BorderGamePanelCover.IsHitTestVisible = true;
        }

        /// <summary>
        /// 创建方块的方法
        /// </summary>
        /// <returns></returns>
        private IGameBlock BlockCreateAction() {
            GameBlock block = new GameBlock();
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
            GameWindow.CalCurrentGame(isGameCompleted);
            if (isGameCompleted == false) {
                Game.OpenAllBlocks();
            }
        }
        /// <summary>
        /// 开始当前游戏
        /// </summary>
        public void StartGame() {
            if (Game.RowSize != GameWindow.RowsSet
                || Game.ColumnSize != GameWindow.ColumnsSet
                || Game.MineSize != GameWindow.MinesSet) {
                Game.SetGame(GameWindow.RowsSet, GameWindow.ColumnsSet, GameWindow.MinesSet);
            }
            Game.StartGame();
        }
        /// <summary>
        /// 开始自定义游戏
        /// </summary>
        /// <param name="setting"></param>
        public void StartCustomGame(LayoutSetting setting) {
            Game.StartCustomGame(setting);
            GameWindow.RowsSet = Game.RowSize;
            GameWindow.ColumnsSet = Game.ColumnSize;
            GameWindow.MinesSet = Game.MineSize;
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
            GameWindow.ToggleDetector.Click -= ToggleDetector_Click;
            GameWindow.AllowDrop = false;
            GameWindow.DragEnter -= GameWindow_DragEnter;
            GameWindow.DragLeave -= GameWindow_DragLeave;
            GameWindow.BorderGamePanelCover.AllowDrop = false;
            GameWindow.BorderGamePanelCover.DragEnter -= BorderGamePanelCover_DragEnter;
            GameWindow.BorderGamePanelCover.DragLeave -= BorderGamePanelCover_DragLeave;
            GameWindow.BorderGamePanelCover.Drop -= BorderGamePanelCover_Drop;
        }
    }
}
