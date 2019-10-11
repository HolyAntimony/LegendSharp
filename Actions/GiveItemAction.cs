using LegendItems;
using System;
using System.Collections.Generic;
using System.Text;

namespace LegendSharp
{
    public class GiveItemAction : PlayerAction
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
                game.player.inventory.AddItem(new Item(item));
            }
        }
    }
}
