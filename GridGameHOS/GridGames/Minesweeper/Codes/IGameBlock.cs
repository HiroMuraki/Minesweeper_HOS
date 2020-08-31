using GridGameHOS.Common;

namespace GridGameHOS.Minesweeper {
    public interface IGameBlock : IBlocks {
        /// <summary>
        /// 方块是否被标记插旗
        /// </summary>
        bool IsFlaged { get; set; }
        /// <summary>
        /// 方块是否打开
        /// </summary>
        bool IsOpen { get; set; }
        /// <summary>
        /// 方块是否为地雷
        /// </summary>
        bool IsMineBlock { get; set; }
        BlockCoordinate Coordinate { get; set; }
        /// <summary>
        /// 记录附近的雷数
        /// </summary>
        int NearMinesCount { get; set; }
    }
}
