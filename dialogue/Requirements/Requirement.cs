using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace LegendDialogue
{
    public abstract class Requirement
    {
        public abstract bool Validate(Dictionary<string, Flag> flags);

    }
}
