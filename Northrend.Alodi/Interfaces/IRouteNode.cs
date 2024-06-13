using Northrend.Alodi.Classes;

namespace Northrend.Alodi.Interfaces
{
    public interface IRouteNode
    {
        decimal Distance { get; }
        List<INode> Nodes { get; }

        void Add(INode node);
        bool IsExistNodeByName(string nodeName);
        RouteNode Clone();
        bool Equals(RouteNode? other);
    }
}