using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northrend.Alodi.Interfaces
{
    public interface INode : ICoordinate
    {
        IEnumerable<INode> PreviousNodes { get; init; }
        IEnumerable<INode> NextNodes { get; init; }
    }
}
