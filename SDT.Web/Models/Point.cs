using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDT.Web.Models
{
    public class Point : IComparable<Point>
    {
        private int x;
        private int y;

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int CompareTo(Point other)
        {
            if (this.x < other.x)
            {
                return 1;
            }
            if (this.x > other.x)
            {
                return -1;
            }
            if (this.x == other.x)
            {
                if (this.y < other.y)
                {
                    return 1;
                }
                if (this.y > other.y)
                {
                    return -1;
                }
            }
           
                return 0;
            
        }

        public override bool Equals(object obj)
        {
            var point = obj as Point;
            return point != null &&
                   x == point.x &&
                   y == point.y;
        }

        public override int GetHashCode()
        {
            var hashCode = 1502939027;
            hashCode = hashCode * -1521134295 + x.GetHashCode();
            hashCode = hashCode * -1521134295 + y.GetHashCode();
            return hashCode;
        }
    }
}