using System;
using System.Collections.Generic;
using System.Text;

namespace LegendSharp
{
    public class Entity
    {
        public Position pos;
        public FACING facing = FACING.DOWN;
        public String sprite;
        Legend legend;

        public Entity(String sprite, Legend legend)
        {
            this.sprite = sprite;
            this.legend = legend;
        }

        public Entity(String sprite, int posX, int posY, Legend legend) : this(sprite, legend)
        {
            this.pos = new Position()
            {
                x = posX,
                y = posY
            };
        }

        public Entity(String sprite, int posX, int posY, FACING facing, Legend legend) : this(sprite, posX, posY, legend)
        {
            this.facing = facing;
        }

        public bool Move(Position newPos, bool force=false)
        {
            if (newPos.x < legend.world.width && newPos.x >= 0 && newPos.y < legend.world.height && newPos.y >= 0)
            {
                if (force || !legend.world.Collides(newPos))
                {
                    if (legend.portals.ContainsKey(newPos))
                    {
                        pos = legend.portals[newPos];
                    }
                    else
                    {
                        pos = newPos;
                    }
                    return true;
                }
                else
                {
                    //Detect for interacts

                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
