using System;
using System.Collections.Generic;
using System.Text;

namespace LegendDialogue
{
    public class NullRequirement : Requirement
    {
        String flagName;
        public NullRequirement(String flagName)
        {
            this.flagName = flagName;
        }

        public override bool Validate(Dictionary<String, Flag> flags)
        {
            if (!flags.ContainsKey(flagName))
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
