using Northrend.Alodi.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northrend.Alodi.Interfaces
{
    public interface INode : ICoordinate
    {
        ushort Id { get; }
        string Name { get; }
        Dictionary<string, IAreaInformation> NextNodes { get; }
        ICell? Cell { get; }

        void AddNextNode(string name, decimal distance, NodeStatus nodeStatus);
        void AddCell(ICell cell);
    }
}
