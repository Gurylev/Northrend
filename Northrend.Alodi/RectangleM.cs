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

        public bool Contains(decimal x, decimal y)
        {
            //if(mWidth<0 &&  mHeight<0)
                return mX >= x && x > mX + mWidth && mY >= y && y > mY + mHeight;

           // return mX <= x && x < mX + mWidth && mY <= y && y < mY + mHeight;
        }

        public override string ToString() => $"{{X={mX},Y={mY},Width={mWidth},Height={mHeight}}}";
    }
}

