﻿namespace Common {
    /// <summary>
    /// 用于为网格类游戏标记坐标
    /// </summary>
    public struct BlockCoordinate {
        public int Row;
        public int Col;
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
