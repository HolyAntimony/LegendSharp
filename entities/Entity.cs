using System;
using System.Collections.Generic;
using System.Text;

namespace LegendSharp
{
    public class Entity
    {
        public int posX = 0;
        public int posY = 0;
        public FACING facing = FACING.DOWN;
        public String sprite;

        public Entity(String sprite)
        {
            this.sprite = sprite;
        }

        public Entity(String sprite, int posX, int posY) : this(sprite)
        {
            this.posX = posX;
            this.posY = posY;
        }

        public Entity(String sprite, int posX, int posY, FACING facing) : this(sprite, posX, posY)
        {
            this.facing = facing;
        }
    }
}
