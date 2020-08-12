using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesweepGameLite {
    public interface IGameBlock {
        bool IsFlaged { get; set; }
        bool IsOpen { get; set; }
        bool IsMineBlock { get; set; }
        BlockCoordinate Coordinate { get; set; }
        int NearMinesCount { get; set; }
    }
}
