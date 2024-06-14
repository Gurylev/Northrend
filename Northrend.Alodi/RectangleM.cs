using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Northrend.Alodi
{
    public struct RectangleM
    {
        decimal mX;
        decimal mY;
        decimal mWidth;
        decimal mHeight;

        public RectangleM(decimal x, decimal y, decimal width, decimal height)
        {
            this.mX = x;
            this.mY = y;
            this.mWidth = width;
            this.mHeight = height;
        }

        public bool Contains(decimal x, decimal y, decimal percent =0.3m)
        {
            return
            mX + mX * percent / 100 >= x && x > mX + mWidth - mX * percent / 100
            && mY + mY * percent / 100 >= y && y > mY + mHeight - mY * percent / 100;
        }

        public override string ToString() => $"{{X = {mX}, Y = {mY}, Width = {mWidth}, Height= {mHeight} }}";
    }
}

