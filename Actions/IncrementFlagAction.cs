using LegendDialogue;
using System;
using System.Collections.Generic;
using System.Text;

namespace LegendSharp
{
    public class IncrementFlagAction : PlayerAction
    {
        string flagKey;
        NumericalFlag flagValue;
        public IncrementFlagAction(string flagKey, NumericalFlag flagValue)
        {
            this.flagKey = flagKey;
            this.flagValue = flagValue;
        }
        public override void Run(Game game)
        {
            if (game.active)
            {
                if (game.player.flags.ContainsKey(flagKey))
                {
                    if (game.player.flags[flagKey] is NumericalFlag)
                    {
                        game.player.flags[flagKey] = (NumericalFlag)game.player.flags[flagKey] + flagValue;
                    }
                }
                else
                {
                    game.player.flags[flagKey] = new NumericalFlag(flagValue);
                }
                //game.player.flags[flagKey] += flagValue;
            }
        }
    }
}
