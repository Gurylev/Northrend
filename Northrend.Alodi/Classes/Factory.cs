﻿using Northrend.Alodi.Interfaces;
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
        public static ICell CreateCell(decimal longitude, decimal latitude)
            => new Cell(longitude, latitude);

        public static IMap CreateMap(int x, int y)
            => new Map(x, y);

        public static INode CreateNode(ushort id, string name, decimal latitude, decimal longitude)
            => new Node(id, name, latitude, longitude);

        public static INodes CreateNodeMap()
            => new NodesMap();
    }
}