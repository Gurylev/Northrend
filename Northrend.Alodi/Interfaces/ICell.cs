using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northrend.Alodi.Interfaces
{
    public interface ICell : ICoordinate
    {
        Dictionary<string, decimal> IntegralVelocities { get; }

        void AddIntegralVelocity(string date, decimal integralVelocity);
    }
}
