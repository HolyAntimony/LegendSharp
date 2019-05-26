using System;
using System.Collections.Generic;
using System.Text;

namespace LegendDialogue
{
    public class IntegerFlag : Flag
    {
        public int value;

        public IntegerFlag(int value)
        {
            this.value = value;
        }

        public override bool Equals(object obj)
        {
            if (obj is IntegerFlag)
            {
                if ( ((IntegerFlag) obj).value == value)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (obj is int)
            {
                return value == (int)obj;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return -1584136870 + value.GetHashCode();
        }

        public override string ToString()
        {
            return value.ToString();
        }

        public static bool operator ==(IntegerFlag integerFlag1, IntegerFlag integerFlag2)
        {
            return integerFlag1.value == integerFlag2.value;
        }

        public static bool operator !=(IntegerFlag integerFlag1, IntegerFlag integerFlag2)
        {
            return integerFlag1.value != integerFlag2.value;
        }

    }
}
