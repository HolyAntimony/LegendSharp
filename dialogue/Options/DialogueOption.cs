using System;
using System.Collections.Generic;
using System.Text;

namespace LegendDialogue
{
    public class DialogueOption : Option
    {
        public string dialogueKey;
        public DialogueOption(String text, String dialogueKey, Requirement[] requirements = null)
        {
            this.text = text;
            this.requirements = requirements;
            this.dialogueKey = dialogueKey;
            this.uuid = Guid.NewGuid();
        }
    }
}
