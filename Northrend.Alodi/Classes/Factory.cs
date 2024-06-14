using Northrend.Alodi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Northrend.Alodi.Classes
{
    public static class Factory
    {
        public static ICell CreateCell(decimal longitude, decimal latitude, int positionX, int positionY)
            => new Cell(longitude, latitude, positionX, positionY);

        public static IMap CreateMap(int x, int y)
            => new Map(x, y);

        public static INode CreateNode(ushort id, string name, decimal latitude, decimal longitude)
            => new Node(id, name, latitude, longitude);

        public static INodesMap CreateNodeMap()
            => new NodesMap();

        public static IRequest CreateRequest(string name, IceClass iceClass, decimal speedInKnots, 
            INode startPoint, INode endPoint, DateTime dateTime)
            => new Request(name, iceClass, speedInKnots, startPoint, endPoint, dateTime);

        public static IIcebreakerCard CreateIcebreakerCard(string name, IceClass iceClass, decimal speedInKnots,
            INode startPoint, DateTime dateTime)
            => new IcebreakerCard(name, speedInKnots, iceClass, startPoint, dateTime);
    }
}
