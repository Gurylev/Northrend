using Northrend.Alodi.Interfaces;
using System.Xml.Linq;

namespace Northrend.Alodi.Classes
{
    public class RouteNode : IEquatable<RouteNode>, IRouteNode
    {
        readonly List<INode> mNodes = [];
        public List<INode> Nodes => mNodes;

        decimal mDistance = 0;
        public decimal Distance => mDistance;

        public RouteNode() { }
        public RouteNode(IEnumerable<INode> nodes)
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

        public RouteNode Clone()
            => new(Nodes);

        public bool Equals(RouteNode? other)
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
