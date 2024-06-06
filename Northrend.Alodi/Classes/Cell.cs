using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Northrend.Alodi.Interfaces;

namespace Northrend.Alodi.Classes
{
    public class Cell : ICell
    {
        public decimal Longitude { get; init; }

        public decimal Latitude { get; init; }

        Dictionary<string, decimal> mIntegralVelocities = [];
        public Dictionary<string, decimal> IntegralVelocities => mIntegralVelocities;

        public Cell(decimal longitude, decimal latitude) 
        {
            Longitude = longitude;
            Latitude = latitude;
        }

        public void AddIntegralVelocity(string date, decimal integralVelocity)
            => mIntegralVelocities.Add(date, integralVelocity);
    }
}
