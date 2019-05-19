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
        public bool moved = false;
        public bool movedChunks = false;

        public HashSet<Game> cachedBy = new HashSet<Game>();


        public Entity(String sprite, int posX, int posY, Legend legend)
        {
            uuid = System.Guid.NewGuid();
            this.sprite = sprite;
            this.legend = legend;
            chunk = legend.world.GetChunkFromPos(pos);
            this.pos = new Position(posX, posY);
            chunk = legend.world.GetChunkFromPos(pos);
            legend.world.AddEntity(this, legend.config);
        }

        public Entity(String sprite, int posX, int posY, FACING facing, Legend legend) : this(sprite, posX, posY, legend)
        {
            this.facing = facing;
        }
    }
}
