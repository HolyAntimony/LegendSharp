using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;

namespace LegendDialogue
{
    public class NumericalFlag : Flag
    {
        public double value;

        public NumericalFlag(double value)
        {
            this.value = value;
        }

        public NumericalFlag(NumericalFlag value)
        {
            this.value = value.value;
        }

        public override BsonValue GetValue()
        {
            return new BsonDouble(this.value);
        }

        public override bool Equals(object obj)
        {
            if (obj is NumericalFlag)
            {
                //Console.WriteLine("{0} vs {1}", value, ((NumericalFlag)obj).value);
                if (((NumericalFlag)obj).value == value)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (obj is double)
            {
                return value == (double)obj;
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

        public static bool operator ==(NumericalFlag numericalFlag1, NumericalFlag numericalFlag2)
        {
            return numericalFlag1.value == numericalFlag2.value;
        }

        public static NumericalFlag operator +(NumericalFlag numericalFlag1, int amount)
        {
            return new NumericalFlag(numericalFlag1.value + amount);
        }

        public static NumericalFlag operator +(NumericalFlag numericalFlag1, double amount)
        {
            return new NumericalFlag(numericalFlag1.value + amount);
        }

        public static bool operator !=(NumericalFlag numericalFlag1, NumericalFlag numericalFlag2)
        {
            return numericalFlag1.value != numericalFlag2.value;
        }

        public static bool operator >(NumericalFlag numericalFlag1, NumericalFlag numericalFlag2)
        {
            return numericalFlag1.value > numericalFlag2.value;
        }

        public static bool operator <(NumericalFlag numericalFlag1, NumericalFlag numericalFlag2)
        {
            return numericalFlag1.value < numericalFlag2.value;
        }

        public static NumericalFlag operator +(NumericalFlag numericalFlag1, NumericalFlag numericalFlag2)
        {
            return new NumericalFlag(numericalFlag1.value + numericalFlag2.value);
        }
    }
}
