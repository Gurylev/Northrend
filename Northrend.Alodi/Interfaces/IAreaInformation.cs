using Northrend.Alodi.Classes;

namespace Northrend.Alodi.Interfaces
{
    public interface IAreaInformation
    {
        decimal Distance { get; init; }
        NodeStatus Status { get; init; }
    }
}