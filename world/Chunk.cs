using System;
using System.Collections.Generic;
using System.Text;

namespace LegendSharp
{
    public class Chunk
    {
        public Position pos;
        public List<Entity> entities;

        public override bool Equals(object obj)
        {
            var chunk = obj as Chunk;
            return chunk != null &&
                   EqualityComparer<Position>.Default.Equals(pos, chunk.pos);
        }

        public override int GetHashCode()
        {
            return 991532785 + EqualityComparer<Position>.Default.GetHashCode(pos);
        }

        public static bool operator ==(Chunk chunk1, Chunk chunk2)
        {
            if (chunk1.pos == chunk2.pos)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool operator !=(Chunk chunk1, Chunk chunk2)
        {
            if (chunk1.pos != chunk2.pos)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
