using Northrend.Alodi.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northrend.Alodi.Interfaces
{
    public interface IIcebreakerCard
    {
        string Name { get; init; }
        decimal SpeedInKnots { get; init; }
        IceClass Class { get; init; }
        INode StartPoint { get; init; }
        DateTime DateAtPoint { get; init; }
    }
}
