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
        public Dictionary<string, Flag> flags;

        public Player(String sprite, int posX, int posY, int inventorySize, Inventory inventory, Dictionary<string, Flag> flags, Legend legend, Game game) : base(sprite, posX, posY, legend)
        {
            this.inventorySize = inventorySize;
            this.inventory = inventory;
            this.flags = flags;
            this.game = game;
        }
    }
}
