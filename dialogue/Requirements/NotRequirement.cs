using System;
using System.Collections.Generic;
using System.Text;

namespace LegendDialogue
{
    public class NotRequirement : Requirement
    {
        Requirement requirement;
        public NotRequirement(Requirement requirement)
        {
            this.requirement = requirement;
        }

        public override bool Validate(Dictionary<String, Flag> flags)
        {
            return !requirement.Validate(flags);
        }
    }
}
