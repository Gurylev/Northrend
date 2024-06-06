using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northrend.Alodi.Classes
{
    public enum NodeStatus
    {
        Excluded = -1, //исключено из расчета
        ClearWater = 0, //чистая вода, движение по прямой
        RoutingWithoutIcebreaker = 1, //роутинг, ледокола нет
        RoutingWithIcebreaker = 2, //роутинг, возможен ледокол
    }
}
