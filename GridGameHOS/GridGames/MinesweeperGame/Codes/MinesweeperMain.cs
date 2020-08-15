using Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace MinesweepGameLite {
    public class MinesweeperMain : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #region 后备字段
        private int rowSize;
        private int columnsSize;
        private int minesSize;
        private int gameSize;
        private int flagsCount;
        private bool isGameStarted;
        #endregion
        public Func<IGameBlock> BlockCreateAction;
        public int RowSize {
            get {
                return this.rowSize;
            }
            private set {
                this.rowSize = value;
                OnPropertyChanged(nameof(RowSize));
            }
        }
        public int ColumnSize {
            get {
                return this.columnsSize;
            }
            private set {
                this.columnsSize = value;
                OnPropertyChanged(nameof(ColumnSize));
            }
        }
        public int MineSize {
            get {
                return this.minesSize;
            }
            private set {
                this.minesSize = value;
                OnPropertyChanged(nameof(MineSize));
            }
        }
        public int GameSize {
            get {
                return this.gameSize;
            }
            private set {
                this.gameSize = value;
                OnPropertyChanged(nameof(GameSize));
            }
        }
        public int FlagsCount {
            get {
                return this.flagsCount;
            }
            private set {
                this.flagsCount = value;
                OnPropertyChanged(nameof(FlagsCount));
            }
        }
        public bool IsGameStarted {
            get {
                return this.isGameStarted;
            }
            set {
                this.isGameStarted = value;
                OnPropertyChanged(nameof(IsGameStarted));
            }
        }
        public bool? IsGameCompleted {
            get {
                bool? isGameCompleted = true;
                foreach (BlockCoordinate coordinate in this.GetAllCoordinates()) {
                    if (this[coordinate].IsMineBlock && this[coordinate].IsOpen) {
                        isGameCompleted = false;
                        break;
                    }
                    if (!this[coordinate].IsMineBlock && !this[coordinate].IsOpen) {
                        isGameCompleted = null;
                    }
                }
                return isGameCompleted;
            }
        }
        private Dictionary<BlockCoordinate, IGameBlock> Blocks;
        public ObservableCollection<IGameBlock> BlocksArray {
            get {
                ObservableCollection<IGameBlock> blocksArray = new ObservableCollection<IGameBlock>();
                foreach (BlockCoordinate coordinate in this.GetAllCoordinates()) {
                    blocksArray.Add(this[coordinate]);
                }
                return blocksArray;
            }
        }
        public IGameBlock this[BlockCoordinate coordinate] {
            get {
                return this.Blocks[coordinate];
            }
            private set {
                this.Blocks[coordinate] = value;
            }
        }
        public MinesweeperMain(Func<IGameBlock> blockCreateAction) {
            this.BlockCreateAction = blockCreateAction;
        }
        public void SetGame(int rowSize, int columnSize, int minesCount) {
            this.RowSize = rowSize;
            this.ColumnSize = columnSize;
            this.MineSize = minesCount;
            this.GameSize = this.RowSize * this.ColumnSize;
            this.FlagsCount = 0;
            this.IsGameStarted = false;
            this.Blocks = new Dictionary<BlockCoordinate, IGameBlock>();
            for (int row = 0; row < this.RowSize; row++) {
                for (int col = 0; col < this.ColumnSize; col++) {
                    BlockCoordinate coordinate = new BlockCoordinate(row, col);
                    this[coordinate] = BlockCreateAction();
                    if (row * this.ColumnSize + col < this.MineSize) {
                        this[coordinate].IsMineBlock = true;
                    }
                }
            }
        }
        public void StartGame() {
            this.Shuffle();
            OnPropertyChanged(nameof(BlocksArray));
            this.IsGameStarted = false;
            this.FlagsCount = 0;
        }
        public void OpenAllBlocks() {
            foreach (BlockCoordinate coordinate in this.GetAllCoordinates()) {
                this.OpenBlock(coordinate);
            }
        }
        public void ResetLayout(BlockCoordinate protectedCoordiante) {
            List<BlockCoordinate> nearbyPos = new List<BlockCoordinate>();
            List<BlockCoordinate> avaliableBlankPos = new List<BlockCoordinate>();
            nearbyPos.Add(protectedCoordiante);
            foreach (BlockCoordinate nCoordinate in this.GetNearCoordinates(protectedCoordiante)) {
                nearbyPos.Add(nCoordinate);
            }
            foreach (BlockCoordinate nCoordinate in this.GetAllCoordinates()) {
                if (!this[nCoordinate].IsMineBlock && !nearbyPos.Contains(nCoordinate)) {
                    avaliableBlankPos.Add(nCoordinate);
                }
            }
            Random random = new Random();
            for (int i = 0; i < nearbyPos.Count; i++) {
                int blankPos = random.Next(0, avaliableBlankPos.Count);
                SwapBlock(nearbyPos[i], avaliableBlankPos[blankPos]);
                avaliableBlankPos.RemoveAt(blankPos);
            }
            foreach (BlockCoordinate coordinate in this.GetAllCoordinates()) {
                this[coordinate].IsOpen = false;
                this[coordinate].IsFlaged = false;
                this[coordinate].NearMinesCount =
                    GetNearCounts(coordinate, (BlockCoordinate nCoordinate) => this[nCoordinate].IsMineBlock);
            }

            OnPropertyChanged(nameof(BlocksArray));
            this.IsGameStarted = false;
            this.FlagsCount = 0;
        }
        //普通方法
        public void OpenBlock(BlockCoordinate coordinate) {
            if (this[coordinate] == null
                || this[coordinate].IsOpen
                || this[coordinate].IsFlaged) {
                return;
            }
            this[coordinate].IsOpen = true;
            if (this[coordinate].IsMineBlock) {
                return;
            }
            if (this[coordinate].NearMinesCount == 0) {
                foreach (BlockCoordinate nearCoordinate in this.GetNearCoordinates(coordinate)) {
                    this.OpenBlock(nearCoordinate);
                }
            }
            return;
        }
        public void FlagBlock(BlockCoordinate coordinate) {
            if (!this[coordinate].IsOpen) {
                if (!this[coordinate].IsFlaged) {
                    this[coordinate].IsFlaged = true;
                    ++FlagsCount;
                } else {
                    this[coordinate].IsFlaged = false;
                    --FlagsCount;
                }
            }
        }
        public void OpenNearBlocks(BlockCoordinate coordinate) {
            int nearFlagedBlocks = GetNearCounts(coordinate, (BlockCoordinate nCoordinate) => this[nCoordinate].IsFlaged);
            if (this[coordinate].NearMinesCount <= nearFlagedBlocks) {
                foreach (BlockCoordinate nearCoordinate in this.GetNearCoordinates(coordinate)) {
                    this.OpenBlock(nearCoordinate);
                }
            }
        }
        //包装方法
        private void Shuffle() {
            Random random = new Random();
            for (int i = 0; i < this.GameSize; i++) {
                int indexA = i;
                int indexB = random.Next(i, this.GameSize);
                BlockCoordinate coordinateA = new BlockCoordinate(indexA / this.ColumnSize, indexA % this.ColumnSize);
                BlockCoordinate coordinateB = new BlockCoordinate(indexB / this.ColumnSize, indexB % this.ColumnSize);
                SwapBlock(coordinateA, coordinateB);
            }
            foreach (BlockCoordinate coordinate in this.GetAllCoordinates()) {
                this[coordinate].IsOpen = false;
                this[coordinate].IsFlaged = false;
                this[coordinate].NearMinesCount =
                    GetNearCounts(coordinate, (BlockCoordinate nCoordinate) => this[nCoordinate].IsMineBlock);
            }
        }
        private void SwapBlock(BlockCoordinate coordinateA, BlockCoordinate coordinateB) {
            IGameBlock T = this[coordinateA];
            this[coordinateA] = this[coordinateB];
            this[coordinateA].Coordinate = coordinateA;
            this[coordinateB] = T;
            this[coordinateB].Coordinate = coordinateB;
        }
        private int GetNearCounts(BlockCoordinate nCoordinate, Predicate<BlockCoordinate> predicate) {
            int count = 0;
            foreach (BlockCoordinate coordinate in this.GetNearCoordinates(nCoordinate)) {
                if (predicate(coordinate)) {
                    ++count;
                }
            }
            return count;
        }
        private IEnumerable<BlockCoordinate> GetNearCoordinates(BlockCoordinate cCoordinate) {
            int cRow = cCoordinate.Row;
            int cCol = cCoordinate.Col;
            for (int i = -1; i < 2; i++) {
                for (int j = -1; j < 2; j++) {
                    int nRow = cRow + i;
                    int nCol = cCol + j;
                    if ((nRow == cRow && nCol == cCol)
                        || (uint)nRow >= this.RowSize
                        || (uint)nCol >= this.ColumnSize) {
                        continue;
                    }
                    yield return new BlockCoordinate(nRow, nCol);
                }
            }
        }
        private IEnumerable<BlockCoordinate> GetAllCoordinates() {
            foreach (BlockCoordinate coordinate in this.Blocks.Keys) {
                yield return coordinate;
            }
        }
        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            for (int row = 0; row < this.RowSize; row++) {
                for (int col = 0; col < this.ColumnSize; col++) {
                    BlockCoordinate coordinate = new BlockCoordinate(row, col);
                    sb.Append($"{(this[coordinate].IsMineBlock ? 1 : 0)} ");
                }
                sb.Append("\n");
            }
            return sb.ToString();
        }
    }
}
