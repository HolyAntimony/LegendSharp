using System;
using System.Collections.Generic;
using System.Text;

namespace LegendDialogue
{
    public class EqualsRequirement : Requirement
    {
        String flagName;
        Flag value;
        public EqualsRequirement(String flagName, Flag value)
        {
            this.flagName = flagName;
            this.value = value;
        }

        public override bool Validate(Dictionary<String, Flag> flags)
        {
            if (flags.ContainsKey(flagName) && flags[flagName].Equals(value))
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
