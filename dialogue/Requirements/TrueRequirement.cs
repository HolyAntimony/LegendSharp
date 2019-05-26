using System;
using System.Collections.Generic;
using System.Text;

namespace LegendDialogue
{
    public class TrueRequirement : Requirement
    {
        public TrueRequirement()
        {
        }

        public override bool Validate(Dictionary<String, Flag> flags)
        {
            return true;
        }
    }
}
