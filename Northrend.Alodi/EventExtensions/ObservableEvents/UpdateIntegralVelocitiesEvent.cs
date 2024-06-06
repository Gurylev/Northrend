using Northrend.Alodi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northrend.Alodi.EventExtensions.ObservableEvents
{    
    public record IntegralVelocitiesMapData(IMap mapData);
    
    public class UpdateIntegralVelocitiesEvent : ObservableEvent<IntegralVelocitiesMapData>
    {

    }
}
