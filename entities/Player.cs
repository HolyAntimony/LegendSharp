using System;
using System.Collections.Generic;
using System.Text;

namespace LegendSharp
{
    public class Player : Entity
    {

        int inventorySize;
        public Inventory inventory;

        public Player(String sprite, int posX, int posY, int inventorySize, Inventory inventory, Legend legend) : base(sprite, posX, posY, legend)
        {
            this.inventorySize = inventorySize;
            this.inventory = inventory;
        }
    }
}
