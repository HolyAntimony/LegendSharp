using System;
using System.Collections.Generic;
using System.Text;

namespace LegendSharp
{
    public class FalseRequirement : Requirement
    {
        public FalseRequirement()
        {
        }

        public override bool Validate(Dictionary<String, Flag> flags)
        {
            return false;
        }
    }
}
