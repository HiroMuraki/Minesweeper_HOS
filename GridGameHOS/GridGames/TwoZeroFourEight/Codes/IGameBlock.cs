using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwoZeroFourEightLite {
    public interface IGameBlock : IBlocks {
        int Number { get; set; }
    }
}
