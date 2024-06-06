using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northrend.Alodi.Interfaces
{
    public interface ICoordinate
    {
        decimal Longitude { get; init; }
        decimal Latitude { get; init; }
    }
}
