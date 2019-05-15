using System;
using System.Collections.Generic;
using System.Text;

namespace LegendSharp
{
    public class NPC : Entity
    {
        public string dialogueKey;
        public NPC(string sprite, int posX, int posY, FACING facing, string dialogueKey, Legend legend) : base(sprite, posX, posY, facing, legend)
        {
            this.dialogueKey = dialogueKey;
        }
    }
}
