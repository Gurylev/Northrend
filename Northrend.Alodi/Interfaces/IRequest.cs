using Northrend.Alodi.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northrend.Alodi.Interfaces
{
    public interface IRequest
    {
        string Name { get; init; }
        IceClass Class { get; init; }
        decimal SpeedInKnots { get; init; }
        INode StartPoint { get; init; }
        INode EndPoint { get; init; }
        DateTime StartDate { get; init; }
    }
}
