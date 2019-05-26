using LegendDialogue;
using System;
using System.Collections.Generic;
using System.Text;

namespace LegendSharp
{
    public class SetFlagAction : PlayerAction
    {
        string flagKey;
        Flag flagValue;
        public SetFlagAction(string flagKey, Flag flagValue)
        {
            this.flagKey = flagKey;
            this.flagValue = flagValue;
        }
        public override void Run(Game game)
        {
            if (game.active)
            {
                game.player.flags[flagKey] = flagValue;
            }
        }
    }
}
