using Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace SlideJigsawGameLite {
    public class SlideJigsawMain : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #region 后备字段
        private int rowSize;
        private int columnSize;
        private int gameSize;
        private BlockCoordinate nullBlockCoordinate;
        #endregion
        public int RowSize {
            get {
                return rowSize;
            }
            private set {
                rowSize = value;
                OnPropertyChanged(nameof(RowSize));
            }
        }
        public int ColumnSize {
            get {
                return columnSize;
            }
            private set {
                columnSize = value;
                OnPropertyChanged(nameof(ColumnSize));
            }
        }
        public int GameSize {
            get {
                return gameSize;
            }
            private set {
                gameSize = value;
                OnPropertyChanged(nameof(GameSize));
            }
        }
        public BlockCoordinate NullBlockCoordiante {
            get {
                return this.nullBlockCoordinate;
            }
            private set {
                this.nullBlockCoordinate = value;
                OnPropertyChanged(nameof(NullBlockCoordiante));
            }
        }
        public bool IsGameCompleted {
            get {
                Console.WriteLine(ToString());
                for (int i = 0; i < this.GameSize - 1; i++) {
                    if (this.BlocksArray[i].BlockID != i + 1) {
                        return false;
                    }
                }
                if (this.BlocksArray[this.GameSize - 1].BlockID != 0) {
                    return false;
                }
                return true;
            }
        }

        public Func<IGameBlock> BlockCreateAction;
        public Dictionary<BlockCoordinate, IGameBlock> Blocks;
        public ObservableCollection<IGameBlock> BlocksArray {
            get {
                ObservableCollection<IGameBlock> blocks = new ObservableCollection<IGameBlock>();
                foreach (BlockCoordinate coordinate in this.GetAllCoordinates()) {
                    blocks.Add(this[coordinate]);
                }
                return blocks;
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
        public SlideJigsawMain(Func<IGameBlock> blockCreateAction) {
            this.BlockCreateAction = blockCreateAction;
        }
        public void SetGame(int rowSet, int colSet) {
            this.RowSize = rowSet;
            this.ColumnSize = colSet;
            this.GameSize = this.RowSize * this.ColumnSize;
            this.Blocks = new Dictionary<BlockCoordinate, IGameBlock>();
            for (int row = 0; row < this.RowSize; row++) {
                for (int col = 0; col < this.ColumnSize; col++) {
                    BlockCoordinate coordinate = new BlockCoordinate(row, col);
                    this[coordinate] = BlockCreateAction();
                    this[coordinate].Coordinate = coordinate;
                    this[coordinate].BlockID = row * this.ColumnSize + col;
                }
            }
            OnPropertyChanged(nameof(BlocksArray));
        }
        public void StartGame() {
            this.Shuffle();
        }
        private void Shuffle() {
            Random random = new Random();
            for (int i = 0; i < this.GameSize; i++) {
                int indexA = i;
                int indexB = random.Next(i, this.GameSize);
                BlockCoordinate coordinateA = new BlockCoordinate(indexA / this.ColumnSize, indexA % this.ColumnSize);
                BlockCoordinate coordinateB = new BlockCoordinate(indexB / this.ColumnSize, indexB % this.ColumnSize);
                this.Swap(coordinateA, coordinateB);
            }
            foreach (BlockCoordinate coordinate in this.GetAllCoordinates()) {
                if (this[coordinate].BlockID == 0) {
                    this.NullBlockCoordiante = coordinate;
                }
            }
        }

        private bool IsNullBlockNearby(BlockCoordinate coordinate) {
            foreach (BlockCoordinate nCoordinate in GetNearCoordinates(coordinate)) {
                if (this[nCoordinate].BlockID == 0) {
                    return true;
                }
            }
            return false;
        }
        public void SwapWithNullBlock(BlockCoordinate coordinate) {
            if (IsNullBlockNearby(coordinate)) {
                if (Swap(coordinate, NullBlockCoordiante)) {
                    this.NullBlockCoordiante = coordinate;
                }
            }
        }
        private IEnumerable<BlockCoordinate> GetAllCoordinates() {
            foreach (BlockCoordinate coordinate in this.Blocks.Keys) {
                yield return coordinate;
            }
        }
        private IEnumerable<BlockCoordinate> GetNearCoordinates(BlockCoordinate coordinate) {
            int cRow = coordinate.Row;
            int cCol = coordinate.Col;
            for (int i = -1; i < 2; i++) {
                for (int j = -1; j < 2; j++) {
                    int nRow = cRow + i;
                    int nCol = cCol + j;
                    if ((nRow == cRow && nCol == cCol)
                        || (nRow != cRow && nCol != cCol)
                        || (uint)nRow >= this.RowSize
                        || (uint)nCol >= this.ColumnSize) {
                        continue;
                    }
                    yield return new BlockCoordinate(nRow, nCol);
                }
            }
        }
        private bool Swap(BlockCoordinate coordinateA, BlockCoordinate coordinateB) {
            if ((uint)coordinateA.Row >= this.RowSize
                || (uint)coordinateA.Col >= this.ColumnSize
                || (uint)coordinateB.Row >= this.RowSize
                || (uint)coordinateB.Col >= this.ColumnSize) {
                return false;
            }
            int T = this[coordinateA].BlockID;
            this[coordinateA].BlockID = this[coordinateB].BlockID;
            this[coordinateB].BlockID = T;
            return true;
        }
        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            for (int row = 0; row < this.rowSize; row++) {
                for (int col = 0; col < this.columnSize; col++) {
                    sb.Append($"{this[new BlockCoordinate(row, col)].BlockID} ");
                }
                sb.Append("\n");
            }
            return sb.ToString();
        }
    }
}
