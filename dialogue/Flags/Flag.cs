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
                return new IntegerFlag(flagNumericalValue);
            }
            else
            {
                return new IntegerFlag(0);
            }
        }

        public abstract BsonValue GetValue();
    }
}
