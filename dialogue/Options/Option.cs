using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace LegendDialogue
{
    public abstract class Option
    {
        public string text;
        public Requirement[] requirements;
        public Guid uuid;

        public bool IsDisplayed(Dictionary<string, Flag> flags)
        {
            foreach (Requirement requirement in requirements)
            {
                if (!requirement.Validate(flags))
                {
                    return false;
                }
            }
            return true;
        }

    }
}
