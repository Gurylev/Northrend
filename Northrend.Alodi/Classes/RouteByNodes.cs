using Northrend.Alodi.Interfaces;
using System.Xml.Linq;

namespace Northrend.Alodi.Classes
{
    public class RouteByNodes : IEquatable<IRouteByNodes>, IRouteByNodes
    {
        readonly List<INode> mNodes = [];
        public List<INode> Nodes => mNodes;

        decimal mDistance = 0;
        public decimal Distance => mDistance;

        public IEnumerable<(int i, int j)> CellsPositionsOnMap 
            => Nodes.Select(x => (x.Cell.PositionX, x.Cell.PositionY) );

        public RouteByNodes() { }
        public RouteByNodes(IEnumerable<INode> nodes)
        {
            foreach (var node in nodes)
                Add(node);
        }

        public void Add(INode node)
        {
            if (!mNodes.Count.Equals(0))
            {
                var lastNode = mNodes.Last();

                var nextNode = lastNode.NextNodes[node.Name];
                if (nextNode is not null)
                    mDistance += nextNode.Distance;
                else
                {
                    //ошибка
                }
            }
            mNodes.Add(node);
        }

        public bool IsExistNodeByName(string nodeName)
            => Nodes.Any(x => x.Name.Equals(nodeName, StringComparison.OrdinalIgnoreCase));

        public IRouteByNodes Clone()
            => new RouteByNodes(Nodes);

        public bool Equals(IRouteByNodes? other)
        {
            if (other is null)
                return false;

            for (int i = 0; i < Nodes.Count; i++)
                if (!Nodes[i].Name.Equals(other.Nodes[i].Name, StringComparison.OrdinalIgnoreCase))
                    return false;
            return true;
        }
    }
}
