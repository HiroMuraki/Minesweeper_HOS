using GridGameHOS.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace GridGameHOS.Minesweeper {
    public class MinesweeperMain : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #region 后备字段
        private int rowSize;
        private int columnsSize;
        private int minesSize;
        private int gameSize;
        private int flagsCount;
        private bool isGameStarted;
        #endregion
        private Func<IGameBlock> BlockCreateAction { get; set; }
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
                return columnsSize;
            }
            private set {
                columnsSize = value;
                OnPropertyChanged(nameof(ColumnSize));
            }
        }
        public int MineSize {
            get {
                return minesSize;
            }
            private set {
                minesSize = value;
                OnPropertyChanged(nameof(MineSize));
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
        public int FlagsCount {
            get {
                return flagsCount;
            }
            private set {
                flagsCount = value;
                OnPropertyChanged(nameof(FlagsCount));
            }
        }
        public bool IsGameStarted {
            get {
                return isGameStarted;
            }
            set {
                isGameStarted = value;
                OnPropertyChanged(nameof(IsGameStarted));
            }
        }
        public bool? IsGameCompleted {
            get {
                bool? isGameCompleted = true;
                foreach (BlockCoordinate coordinate in GetAllCoordinates()) {
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
        public Dictionary<BlockCoordinate, IGameBlock> Blocks { get; private set; }
        public IGameBlock this[BlockCoordinate coordinate] {
            get {
                return Blocks[coordinate];
            }
            private set {
                Blocks[coordinate] = value;
            }
        }
        /// <summary>
        /// 带参构造函数，传入一个创建方块的方法
        /// </summary>
        /// <param name="blockCreateAction">创建方块的方法</param>
        public MinesweeperMain(Func<IGameBlock> blockCreateAction) {
            BlockCreateAction = blockCreateAction;
        }
        /// <summary>
        /// 设置游戏的行数，列数和雷数
        /// </summary>
        /// <param name="rowSize">行数</param>
        /// <param name="columnSize">列数</param>
        /// <param name="minesCount">雷数</param>
        public void SetGame(int rowSize, int columnSize, int minesCount) {
            RowSize = rowSize;
            ColumnSize = columnSize;
            MineSize = minesCount;
            GameSize = RowSize * ColumnSize;
            FlagsCount = 0;
            IsGameStarted = false;
            Blocks = new Dictionary<BlockCoordinate, IGameBlock>();
            for (int row = 0; row < RowSize; row++) {
                for (int col = 0; col < ColumnSize; col++) {
                    BlockCoordinate coordinate = new BlockCoordinate(row, col);
                    this[coordinate] = BlockCreateAction();
                    if (row * ColumnSize + col < MineSize) {
                        this[coordinate].IsMineBlock = true;
                    }
                }
            }
        }
        /// <summary>
        /// 开始当前游戏
        /// </summary>
        public void StartGame() {
            Shuffle();
            OnPropertyChanged(nameof(Blocks));
            IsGameStarted = false;
            FlagsCount = 0;
        }
        /// <summary>
        /// 开始自定义游戏
        /// </summary>
        /// <param name="setting"></param>
        public void StartCustomGame(LayoutSetting setting) {
            SetGame(setting.RowSize, setting.ColumnSize, setting.MineSize);
            for (int row = 0; row < RowSize; row++) {
                for (int col = 0; col < ColumnSize; col++) {
                    BlockCoordinate coordinate = new BlockCoordinate(row, col);
                    this[coordinate].Coordinate = coordinate;
                    int index = row * ColumnSize + col;
                    if (setting.LayoutDataArray[index] == '1') {
                        this[coordinate].IsMineBlock = true;
                    } else {
                        this[coordinate].IsMineBlock = false;
                    }
                }
            }
            foreach (BlockCoordinate cCoordinate in GetAllCoordinates()) {
                this[cCoordinate].NearMinesCount
                    = GetNearCounts(cCoordinate, (BlockCoordinate nCoordinate) => this[nCoordinate].IsMineBlock);
            }
            OnPropertyChanged(nameof(Blocks));
            isGameStarted = true;
            FlagsCount = 0;
        }
        /// <summary>
        /// 打开所有方块
        /// </summary>
        public void OpenAllBlocks() {
            foreach (BlockCoordinate coordinate in GetAllCoordinates()) {
                if (this[coordinate].IsFlaged && !this[coordinate].IsMineBlock) {
                    this[coordinate].IsFlaged = false;
                    this[coordinate].IsOpen = true;
                    continue;
                }
                OpenBlock(coordinate);
            }
        }
        /// <summary>
        /// 重置方块位置，指定方块周围的8个坐标与自身坐标将不会有雷
        /// </summary>
        /// <param name="protectedCoordiante">待保护的坐标位置</param>
        public void ResetLayout(BlockCoordinate protectedCoordiante) {
            List<BlockCoordinate> nearbyPos = new List<BlockCoordinate>();
            List<BlockCoordinate> avaliableBlankPos = new List<BlockCoordinate>();
            nearbyPos.Add(protectedCoordiante);
            foreach (BlockCoordinate nCoordinate in GetNearCoordinates(protectedCoordiante)) {
                nearbyPos.Add(nCoordinate);
            }
            foreach (BlockCoordinate nCoordinate in GetAllCoordinates()) {
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
            foreach (BlockCoordinate coordinate in GetAllCoordinates()) {
                this[coordinate].IsOpen = false;
                this[coordinate].IsFlaged = false;
                this[coordinate].NearMinesCount =
                    GetNearCounts(coordinate, (BlockCoordinate nCoordinate) => this[nCoordinate].IsMineBlock);
            }

            OnPropertyChanged(nameof(Blocks));
            IsGameStarted = false;
            FlagsCount = 0;
        }
        //普通方法
        /// <summary>
        /// 打开指定方块，为递归打开
        /// </summary>
        /// <param name="coordinate">待打开的方块坐标</param>
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
                foreach (BlockCoordinate nearCoordinate in GetNearCoordinates(coordinate)) {
                    OpenBlock(nearCoordinate);
                }
            }
            return;
        }
        /// <summary>
        /// 为指定方块插旗
        /// </summary>
        /// <param name="coordinate">待插旗的方块坐标</param>
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
        /// <summary>
        /// 打开指定方块周围的方块
        /// </summary>
        /// <param name="coordinate">中心方块</param>
        public void OpenNearBlocks(BlockCoordinate coordinate) {
            int nearFlagedBlocks = GetNearCounts(coordinate, (BlockCoordinate nCoordinate) => this[nCoordinate].IsFlaged);
            if (this[coordinate].NearMinesCount <= nearFlagedBlocks) {
                foreach (BlockCoordinate nearCoordinate in GetNearCoordinates(coordinate)) {
                    OpenBlock(nearCoordinate);
                }
            }
        }
        //包装方法
        /// <summary>
        /// 打乱方块位置
        /// </summary>
        private void Shuffle() {
            Random random = new Random();
            for (int i = 0; i < GameSize; i++) {
                int indexA = i;
                int indexB = random.Next(i, GameSize);
                BlockCoordinate coordinateA = new BlockCoordinate(indexA / ColumnSize, indexA % ColumnSize);
                BlockCoordinate coordinateB = new BlockCoordinate(indexB / ColumnSize, indexB % ColumnSize);
                SwapBlock(coordinateA, coordinateB);
            }
            foreach (BlockCoordinate coordinate in GetAllCoordinates()) {
                this[coordinate].IsOpen = false;
                this[coordinate].IsFlaged = false;
                this[coordinate].NearMinesCount =
                    GetNearCounts(coordinate, (BlockCoordinate nCoordinate) => this[nCoordinate].IsMineBlock);
            }
        }
        /// <summary>
        /// 交换两个方块的位置
        /// </summary>
        /// <param name="coordinateA"></param>
        /// <param name="coordinateB"></param>
        private void SwapBlock(BlockCoordinate coordinateA, BlockCoordinate coordinateB) {
            IGameBlock T = this[coordinateA];
            this[coordinateA] = this[coordinateB];
            this[coordinateA].Coordinate = coordinateA;
            this[coordinateB] = T;
            this[coordinateB].Coordinate = coordinateB;
        }
        /// <summary>
        /// 获取指定方块周围符合predicate的方块的数量
        /// </summary>
        /// <param name="nCoordinate">中心方块</param>
        /// <param name="predicate">判定方法</param>
        /// <returns></returns>
        private int GetNearCounts(BlockCoordinate nCoordinate, Predicate<BlockCoordinate> predicate) {
            int count = 0;
            foreach (BlockCoordinate coordinate in GetNearCoordinates(nCoordinate)) {
                if (predicate(coordinate)) {
                    ++count;
                }
            }
            return count;
        }
        /// <summary>
        /// 获取指定方块周围8个方块的坐标
        /// </summary>
        /// <param name="cCoordinate">中心方块坐标</param>
        /// <returns></returns>
        private IEnumerable<BlockCoordinate> GetNearCoordinates(BlockCoordinate cCoordinate) {
            int cRow = cCoordinate.Row;
            int cCol = cCoordinate.Col;
            for (int i = -1; i < 2; i++) {
                for (int j = -1; j < 2; j++) {
                    int nRow = cRow + i;
                    int nCol = cCol + j;
                    if ((nRow == cRow && nCol == cCol)
                        || (uint)nRow >= RowSize
                        || (uint)nCol >= ColumnSize) {
                        continue;
                    }
                    yield return new BlockCoordinate(nRow, nCol);
                }
            }
        }
        /// <summary>
        /// 获取当前游戏所有可用的坐标值
        /// </summary>
        /// <returns></returns>
        private IEnumerable<BlockCoordinate> GetAllCoordinates() {
            foreach (BlockCoordinate coordinate in Blocks.Keys) {
                yield return coordinate;
            }
        }
        /// <summary>
        /// 字符串格式化方法
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            for (int row = 0; row < RowSize; row++) {
                for (int col = 0; col < ColumnSize; col++) {
                    BlockCoordinate coordinate = new BlockCoordinate(row, col);
                    sb.Append($"{(this[coordinate].IsMineBlock ? 1 : 0)} ");
                }
                sb.Append("\n");
            }
            return sb.ToString();
        }
    }
}
