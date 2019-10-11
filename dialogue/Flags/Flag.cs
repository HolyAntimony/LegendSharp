using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace LegendDialogue
{
    public abstract class Flag
    {
        public abstract BsonValue GetValue();
    }
}
