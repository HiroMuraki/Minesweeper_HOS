namespace GridGameHOS.Common {
    /// <summary>
    /// 用于为网格类游戏标记坐标
    /// </summary>
    public struct BlockCoordinate {
        public int Row;
        public int Col;
        public BlockCoordinate North {
            get {
                return Add(-1, 0);
            }
        }
        public BlockCoordinate South {
            get {
                return Add(1, 0);
            }
        }
        public BlockCoordinate West {
            get {
                return Add(0, -1);
            }
        }
        public BlockCoordinate East {
            get {
                return Add(0, 1);
            }
        }
        public BlockCoordinate(int row, int col) {
            Row = row;
            Col = col;
        }
        public BlockCoordinate Add(int i, int j) {
            return new BlockCoordinate(Row + i, Col + j);
        }
        public override string ToString() {
            return $"{Row},{Col}";
        }
    }
}
