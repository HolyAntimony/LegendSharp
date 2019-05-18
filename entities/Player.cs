using System;
using System.Collections.Generic;
using System.Text;

namespace LegendSharp
{
    public class Player : Entity
    {

        public int inventorySize;
        public Game game;
        public Inventory inventory;

        public Player(String sprite, int posX, int posY, int inventorySize, Inventory inventory, Legend legend, Game game) : base(sprite, posX, posY, legend)
        {
            this.inventorySize = inventorySize;
            this.inventory = inventory;
            this.game = game;
        }
    }
}
