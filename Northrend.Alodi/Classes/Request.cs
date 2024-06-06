using Northrend.Alodi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northrend.Alodi.Classes
{
    public class Request : IRequest
    {
        public string Name { get; init; }
        public IceClass Class { get; init; }
        public decimal SpeedInKnots { get; init; }
        public INode StartPoint { get; init; }
        public INode EndPoint { get; init ; }
        public DateTime StartDate { get; init; }

        public Request(string name, IceClass iceClass, decimal speedInKnots, INode startPoint, INode endPoint, DateTime dateTime)
        {
            Name = name;
            Class = iceClass;
            SpeedInKnots = speedInKnots;
            StartPoint = startPoint;
            EndPoint = endPoint;
            StartDate = dateTime;
        }
    }
}
