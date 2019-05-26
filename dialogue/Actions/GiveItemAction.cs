using System;
using System.Collections.Generic;
using System.Text;

namespace LegendSharp
{
    public class GiveItemAction : Action
    {
        Item item;

        public GiveItemAction(Item item)
        {
            this.item = item;
        }

        public override void Run(Game game)
        {
            if (game.active)
            {
                game.player.inventory.AddItem(item);
            }
        }
    }
}
