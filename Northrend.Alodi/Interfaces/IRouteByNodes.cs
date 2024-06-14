using Northrend.Alodi.Classes;

namespace Northrend.Alodi.Interfaces
{
    public interface IRouteByNodes
    {
        decimal Distance { get; }
        List<INode> Nodes { get; }
        IEnumerable<(int i, int j)> CellsPositionsOnMap { get; }
        void Add(INode node);
        bool IsExistNodeByName(string nodeName);
        IRouteByNodes Clone();
        bool Equals(IRouteByNodes? other);
       
    }
}