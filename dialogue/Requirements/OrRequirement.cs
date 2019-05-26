using System;
using System.Collections.Generic;
using System.Text;

namespace LegendSharp
{
    public class OrRequirement : Requirement
    {
        Requirement[] requirements;
        public OrRequirement(Requirement[] requirements)
        {
            this.requirements = requirements;
        }

        public override bool Validate(Dictionary<String, Flag> flags)
        {
            foreach (Requirement requirement in requirements)
            {
                if (requirement.Validate(flags))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
