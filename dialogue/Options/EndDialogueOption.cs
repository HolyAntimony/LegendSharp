using System;
using System.Collections.Generic;
using System.Text;

namespace LegendSharp
{
    public class EndDialogueOption : Option
    {
        public EndDialogueOption(string text, Requirement[] requirements = null)
        {
            this.text = text;
            this.requirements = requirements;
            this.uuid = Guid.NewGuid();
        }
    }
}
