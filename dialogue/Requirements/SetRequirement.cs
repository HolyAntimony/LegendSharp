using System;
using System.Collections.Generic;
using System.Text;

namespace LegendSharp
{
    public class SetRequirement : Requirement
    {
        String flagName;
        public SetRequirement(String flagName)
        {
            this.flagName = flagName;
        }

        public override bool Validate(Dictionary<String, Flag> flags)
        {
            if (flags.ContainsKey(flagName))
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
