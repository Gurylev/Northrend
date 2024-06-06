using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Northrend.Alodi.Interfaces;

namespace Northrend.Alodi.Classes
{
    public class AreaInformation : IAreaInformation
    {
        public decimal Distance { get; init; }

        public NodeStatus Status { get; init; }

        public AreaInformation(decimal distance, NodeStatus status)
        {
            Distance = distance;
            Status = status;
        }
    }
}
