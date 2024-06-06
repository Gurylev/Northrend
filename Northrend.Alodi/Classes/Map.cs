using Northrend.Alodi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northrend.Alodi.Classes
{
    public class Map : IMap
    {
        ICell[,] cells;

        public ICell[,] Cells => cells;

        public Map(int x, int y)
        {
            cells = new Cell[x, y];
        }

        public void AddCell(int x, int y, ICell cell)
            => cells[x, y] = cell;
    }
}
