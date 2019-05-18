using System;
using System.Collections.Generic;
using System.Text;

namespace LegendSharp
{
    public class Entity
    {
        public Position pos = new Position(0, 0);
        public FACING facing = FACING.DOWN;
        public String sprite;
        public Chunk chunk;
        public Guid uuid;
        Legend legend;

        public HashSet<Game> cachedBy = new HashSet<Game>();


        public Entity(String sprite, int posX, int posY, Legend legend)
        {
            this.sprite = sprite;
            this.legend = legend;
            chunk = legend.world.GetChunk(pos);
            this.pos = new Position(posX, posY);
            chunk = legend.world.GetChunk(pos);
            legend.world.AddEntity(this, legend.config);
            uuid = System.Guid.NewGuid();
        }

        public Entity(String sprite, int posX, int posY, FACING facing, Legend legend) : this(sprite, posX, posY, legend)
        {
            this.facing = facing;
        }
    }
}
