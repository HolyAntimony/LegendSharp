using System;
using System.Collections.Generic;
using System.Text;

namespace LegendSharp
{
    public class Position
    {
        public int x;
        public int y;

        public Position(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Position))
            {
                return false;
            }

            var position = (Position)obj;
            return x == position.x &&
                   y == position.y;
        }

        public override int GetHashCode()
        {
            var hashCode = 1502939027;
            hashCode = hashCode * -1521134295 + x.GetHashCode();
            hashCode = hashCode * -1521134295 + y.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(Position pos1, Position pos2)
        {
            if (pos1.x == pos2.x && pos1.y == pos2.y)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool operator !=(Position pos1, Position pos2)
        {
            if (pos1.x != pos2.x || pos1.y != pos2.y)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}
