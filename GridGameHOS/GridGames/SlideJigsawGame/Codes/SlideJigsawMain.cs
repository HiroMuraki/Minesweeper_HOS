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
                List<IGameBlock> blocksArray = new List<IGameBlock>(this.Blocks.Values);
                for (int i = 0; i < this.GameSize - 1; i++) {
                    if (blocksArray[i].BlockID != i + 1) {
                        return false;
                    }
                }
                if (blocksArray[this.GameSize - 1].BlockID != 0) {
                    return false;
                }
                return true;
            }
        }

        private Func<IGameBlock> BlockCreateAction { get; set; }
        public Dictionary<BlockCoordinate, IGameBlock> Blocks;
        public IGameBlock this[BlockCoordinate coordinate] {
            get {
                return this.Blocks[coordinate];
            }
            private set {
                this.Blocks[coordinate] = value;
            }
        }
        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="blockCreateAction">传入一个返回IGameBlock的方法，该方法生成一个方块样式</param>
        public SlideJigsawMain(Func<IGameBlock> blockCreateAction) {
            this.BlockCreateAction = blockCreateAction;
        }
        /// <summary>
        /// 设置游戏的行数和列数
        /// </summary>
        /// <param name="rowSet">行数</param>
        /// <param name="colSet">列数</param>
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
        }
        /// <summary>
        /// 开始游戏
        /// </summary>
        public void StartGame() {
            this.Shuffle();
        }
        /// <summary>
        /// 打乱方块顺序
        /// </summary>
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
        /// <summary>
        /// 判断指定位置方块的周围是否有空方块
        /// </summary>
        /// <param name="coordinate">传入指定的方块位置</param>
        /// <returns>周围是否有空方块</returns>
        private bool IsNullBlockNearby(BlockCoordinate coordinate) {
            foreach (BlockCoordinate nCoordinate in GetNearCrossCoordinates(coordinate)) {
                if (this[nCoordinate].BlockID == 0) {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 与空方快交换位置
        /// </summary>
        /// <param name="coordinate">传入待交换的方块坐标</param>
        public void SwapWithNullBlock(BlockCoordinate coordinate) {
            if (IsNullBlockNearby(coordinate)) {
                if (Swap(coordinate, NullBlockCoordiante)) {
                    this.NullBlockCoordiante = coordinate;
                }
            }
        }
        /// <summary>
        /// 获取所有方块位置
        /// </summary>
        /// <returns>返回一个所有方块位置的迭代器</returns>
        private IEnumerable<BlockCoordinate> GetAllCoordinates() {
            foreach (BlockCoordinate coordinate in this.Blocks.Keys) {
                yield return coordinate;
            }
        }
        /// <summary>
        /// 获取指定坐标周围十字范围的坐标
        /// </summary>
        /// <param name="coordinate">中心坐标</param>
        /// <returns>周围十字范围的坐标</returns>
        private IEnumerable<BlockCoordinate> GetNearCrossCoordinates(BlockCoordinate coordinate) {
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
        /// <summary>
        /// 交换两个方块的属性，传入坐标
        /// </summary>
        /// <param name="coordinateA"></param>
        /// <param name="coordinateB"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 字符串格式化方法
        /// </summary>
        /// <returns></returns>
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
