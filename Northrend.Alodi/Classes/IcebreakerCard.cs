using Northrend.Alodi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northrend.Alodi.Classes
{
    public class IcebreakerCard : IIcebreakerCard
    {
        public string Name { get; init; }
        public decimal SpeedInKnots { get; init; }
        public IceClass Class { get; init; }
        public INode StartPoint { get; init; }
        public DateTime DateAtPoint { get; init; }

        public IcebreakerCard(string name, decimal speedInKnots, IceClass iceClass, INode startPoint, DateTime dateAtPoint)
        {
            Name = name;
            SpeedInKnots = speedInKnots;
            Class = iceClass;
            StartPoint = startPoint;
            DateAtPoint = dateAtPoint;
        }
    }
}
