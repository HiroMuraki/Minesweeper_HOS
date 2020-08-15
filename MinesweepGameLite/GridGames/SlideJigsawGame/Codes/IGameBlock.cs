using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace SlideJigsawGameLite {
    public interface IGameBlock {
        int BlockID { get; set; }
        BlockCoordinate Coordinate { get; set; }
    }
}
