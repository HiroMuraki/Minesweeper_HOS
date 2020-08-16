using Common;

namespace SlideJigsawGameLite {
    public interface IGameBlock : IBlocks {
        int BlockID { get; set; }
        BlockCoordinate Coordinate { get; set; }
    }
}
