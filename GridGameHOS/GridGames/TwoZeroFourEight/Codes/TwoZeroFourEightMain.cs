using Common;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using static Common.GeneralAction;

namespace TwoZeroFourEightLite {
    public class TwoZeroFourEightMain {
        public int RowSize { get; private set; }
        public int ColumnSize { get; private set; }
        public int TargetNumber { get; private set; }
        public bool IsGameCompleted {
            get {
                foreach (BlockCoordinate coordinate in this.GetAllCoordinates()) {
                    if (this[coordinate].Number == this.TargetNumber) {
                        return true;
                    }
                }
                return false;
            }
        }
        private Func<IGameBlock> BlockCreateAction { get; set; }
        public Dictionary<BlockCoordinate, IGameBlock> Blocks { get; private set; }
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
        /// <param name="blockCreateAction">创建游戏方块的方法</param>
        public TwoZeroFourEightMain(Func<IGameBlock> blockCreateAction) {
            this.BlockCreateAction = blockCreateAction;
        }
        //开始游戏
        public void StartGame(int gameSize, int targetNumber) {
            this.RowSize = this.ColumnSize = gameSize;
            this.TargetNumber = targetNumber;
            this.Blocks = new Dictionary<BlockCoordinate, IGameBlock>();
            for (int row = 0; row < this.RowSize; row++) {
                for (int col = 0; col < this.ColumnSize; col++) {
                    BlockCoordinate coordinate = new BlockCoordinate(row, col);
                    this[coordinate] = BlockCreateAction();
                    this[coordinate].Number = 0;
                }
            }
        }
        /// <summary>
        /// 将所有方块向上/下/左/右移动
        /// </summary>
        public void MoveToNorth() {
            for (int row = 0; row < this.RowSize; row++) {
                for (int col = 0; col < this.ColumnSize; col++) {
                    BlockCoordinate coordinate = new BlockCoordinate(row, col);
                    while (MoveTo(coordinate, DirectionEnum.North)) {
                        coordinate = coordinate.North;
                    }
                }
            }
        }
        public void MoveToSouth() {
            for (int row = this.RowSize - 1; row >= 0; row--) {
                for (int col = 0; col < this.ColumnSize; col++) {
                    BlockCoordinate coordinate = new BlockCoordinate(row, col);
                    while (MoveTo(coordinate, DirectionEnum.South)) {
                        coordinate = coordinate.South;
                    }
                }
            }
        }
        public void MoveToWest() {
            for (int col = 0; col < this.ColumnSize; col++) {
                for (int row = 0; row < this.RowSize; row++) {
                    BlockCoordinate coordinate = new BlockCoordinate(row, col);
                    while (MoveTo(coordinate, DirectionEnum.West)) {
                        coordinate = coordinate.West;
                    }
                }
            }
        }
        public void MoveToEast() {
            for (int col = this.ColumnSize - 1; col >= 0; col--) {
                for (int row = 0; row < this.RowSize; row++) {
                    BlockCoordinate coordinate = new BlockCoordinate(row, col);
                    while (MoveTo(coordinate, DirectionEnum.East)) {
                        coordinate = coordinate.East;
                    }
                }
            }
        }
        /// <summary>
        /// 将方块e向指定方向移动一格
        /// </summary>
        /// <param name="coordinate">方块所在的坐标</param>
        /// <param name="direction">方向枚举</param>
        /// <returns>是否移动成功</returns>
        private bool MoveTo(BlockCoordinate coordinate, DirectionEnum direction) {
            BlockCoordinate targetCoordinate;
            switch (direction) {
                case DirectionEnum.North:
                    targetCoordinate = coordinate.North;
                    break;
                case DirectionEnum.South:
                    targetCoordinate = coordinate.South;
                    break;
                case DirectionEnum.West:
                    targetCoordinate = coordinate.West;
                    break;
                case DirectionEnum.East:
                    targetCoordinate = coordinate.East;
                    break;
                default:
                    targetCoordinate = new BlockCoordinate(-1, -1);
                    break;
            }
            if ((uint)targetCoordinate.Row >= this.RowSize
                || (uint)targetCoordinate.Col >= this.ColumnSize) {
                return false;
            }
            if (this[targetCoordinate].Number == 0) {
                Swap(coordinate, targetCoordinate);
            } else if (this[coordinate].Number == this[targetCoordinate].Number) {
                this[targetCoordinate].Number = this[targetCoordinate].Number << 1;
                this[coordinate].Number = 0;
                PlayScaleTransform((this[targetCoordinate] as GameBlock).NumberIcon, 1.25, 1, 255);
            }
            return true;
        }
        /// <summary>
        /// 在随机位置生成数字
        /// </summary>
        public void GenerateNumber() {
            List<BlockCoordinate> blankCoordiantes = new List<BlockCoordinate>();
            foreach (BlockCoordinate coordinate in this.GetAllCoordinates()) {
                if (this[coordinate].Number == 0) {
                    blankCoordiantes.Add(coordinate);
                }
            }
            if (blankCoordiantes.Count == 0) {
                return;
            }
            Random rnd = new Random();
            BlockCoordinate numberCoordinate = blankCoordiantes[rnd.Next(blankCoordiantes.Count)];
            int number = rnd.Next(3) == 0 ? 4 : 2;
            this[numberCoordinate].Number = number;
            PlayScaleTransform((this[numberCoordinate] as GameBlock).NumberIcon, 0, 1, 255);
        }
        /// <summary>
        /// 清楚面板上的指定数字
        /// </summary>
        /// <param name="targetNumber"></param>
        public void ClearNumber(int targetNumber) {
            foreach (BlockCoordinate coordinate in this.GetAllCoordinates()) {
                if (this[coordinate].Number == targetNumber) {
                    this[coordinate].Number = 0;
                }
            }
        }
        /// <summary>
        /// 交换两个方块的内容
        /// </summary>
        /// <param name="coordinateA"></param>
        /// <param name="coordinateB"></param>
        private void Swap(BlockCoordinate coordinateA, BlockCoordinate coordinateB) {
            int T = this[coordinateA].Number;
            this[coordinateA].Number = this[coordinateB].Number;
            this[coordinateB].Number = T;
        }
        /// <summary>
        /// 获取所有方块坐标
        /// </summary>
        /// <returns></returns>
        private IEnumerable<BlockCoordinate> GetAllCoordinates() {
            foreach (BlockCoordinate coordinate in this.Blocks.Keys) {
                yield return coordinate;
            }
        }
    }
}
