using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace LegendDialogue
{
    public abstract class Flag
    {
        
        public static Flag DecodeFlag(BsonValue flagValue)
        {
            if (flagValue.IsInt32)
            {
                int flagNumericalValue = flagValue.AsInt32;
                return new NumericalFlag((double) flagNumericalValue);
            }
            else if (flagValue.IsDouble)
            {
                double flagNumericalValue = flagValue.AsDouble;
                return new NumericalFlag((double) flagNumericalValue);
            }
            else
            {
                return new NumericalFlag(0);
            }
        }

        public abstract BsonValue GetValue();
    }
}
