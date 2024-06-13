using Northrend.Alodi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Northrend.Alodi.Classes
{
    public class Node : INode
    {
        readonly Dictionary<string, IAreaInformation> mNextNodes = [];

        public Dictionary<string, IAreaInformation> NextNodes => mNextNodes;

        public ushort Id { get; init; }
        public string Name { get; init; }
        public decimal Longitude { get ; init; }
        public decimal Latitude { get; init; }

        ICell? mCell = null;
        public ICell? Cell => mCell;

       
        public Node(ushort id, string name, decimal longitude, decimal latitude)
        {
            Id = id;
            Name = name;
            Longitude = longitude;
            Latitude = latitude;
        }

        public void AddNextNode(string name, decimal distance, NodeStatus nodeStatus)
            => mNextNodes.Add(name, new AreaInformation(distance, nodeStatus));

        public void AddCell(ICell cell)
            => mCell = cell;
    }
}
