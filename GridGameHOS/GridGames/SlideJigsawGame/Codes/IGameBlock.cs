using Common;

namespace SlideJigsawGameLite {
    public interface IGameBlock : IBlocks {
        /// <summary>
        /// 用于显示方块的ID
        /// </summary>
        int BlockID { get; set; }
        /// <summary>
        /// 用于显示方块的坐标
        /// </summary>
        BlockCoordinate Coordinate { get; set; }
    }
}
