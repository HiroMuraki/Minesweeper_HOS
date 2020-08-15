using Common;

namespace SlideJigsawGameLite {
    public interface IGameBlock {
        int BlockID { get; set; }
        BlockCoordinate Coordinate { get; set; }
    }
}
