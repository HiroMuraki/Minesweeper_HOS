namespace Common {
    /// <summary>
    /// 用于为网格类游戏标记坐标
    /// </summary>
    public struct BlockCoordinate {
        public int Row;
        public int Col;
        public BlockCoordinate North {
            get {
                return this.Add(-1, 0);
            }
        }
        public BlockCoordinate South {
            get {
                return this.Add(1, 0);
            }
        }
        public BlockCoordinate West {
            get {
                return this.Add(0, -1);
            }
        }
        public BlockCoordinate East {
            get {
                return this.Add(0, 1);
            }
        }
        public BlockCoordinate(int row, int col) {
            this.Row = row;
            this.Col = col;
        }
        public BlockCoordinate Add(int i, int j) {
            return new BlockCoordinate(this.Row + i, this.Col + j);
        }
        public override string ToString() {
            return $"{Row},{Col}";
        }
    }
}
