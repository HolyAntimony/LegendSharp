using MiscUtil.Conversion;
using System;
using System.Collections.Generic;
using System.Text;

namespace LegendSharp
{
    public class World
    {
        int[,] worldMap;
        int[,] bumpMap;
        public int height;
        public int width;
        int worldWordSize;
        int bumpWordSize;
        byte[] worldData;
        byte[] bumpData;
        Dictionary<Position, Chunk> chunks;
        public Dictionary<Position, Position> portals;

        public World(int[,] worldMap, int[,] bumpMap, int height, int width, Dictionary<Position, Position> portals)
        {
            this.worldMap = worldMap;
            this.bumpMap = bumpMap;
            this.height = height;
            this.width = width;
            this.portals = portals;
            chunks = new Dictionary<Position, Chunk>();
            for (var x = 0; x < width >> 3; x++)
            {
                for (var y = 0; y < height >> 3; y++)
                {
                    Position chunkPos = new Position(x, y);
                    chunks[chunkPos] = new Chunk()
                    {
                        pos = chunkPos,
                        entities = new List<Entity>()
                    };
                }
            }
            CalculateWordSizes();
            CalculateData();
        }

        void CalculateWordSizes()
        {
            var highestWorldCell = 0;
            var highestBumpCell = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (worldMap[y, x] > highestWorldCell)
                    {
                        highestWorldCell = worldMap[y, x];
                    }
                    if (bumpMap[y, x] > highestBumpCell)
                    {
                        highestBumpCell = bumpMap[y, x];
                    }
                }
            }

            if (highestWorldCell <= Byte.MaxValue)
            {
                worldWordSize = 1;
            }
            else if (highestWorldCell <= ushort.MaxValue)
            {
                worldWordSize = 2;
            }
            else
            {
                worldWordSize = 4;
            }

            if (highestBumpCell <= Byte.MaxValue)
            {
                bumpWordSize = 1;
            }
            else if (highestBumpCell <= ushort.MaxValue)
            {
                bumpWordSize = 2;
            }
            else
            {
                bumpWordSize = 4;
            }

        }

        void CalculateData()
        {
            worldData = new byte[height * width * worldWordSize];
            bumpData = new byte[height * width * bumpWordSize];
            BigEndianBitConverter converter = new BigEndianBitConverter();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    byte[] worldDataUnit;
                    byte[] bumpDataUnit;

                    switch (worldWordSize)
                    {
                        case 1:
                            worldDataUnit = new byte[] { (Byte)worldMap[y, x] };
                            break;
                        case 2:
                            worldDataUnit = converter.GetBytes((ushort) worldMap[y, x]);
                            break;
                        default:
                            worldDataUnit = converter.GetBytes((uint) worldMap[y, x]);
                            break;
                    }

                    switch (bumpWordSize)
                    {
                        case 1:
                            bumpDataUnit = new byte[] { (Byte)bumpMap[y, x] };
                            break;
                        case 2:
                            bumpDataUnit = converter.GetBytes((ushort) bumpMap[y, x]);
                            break;
                        default:
                            bumpDataUnit = converter.GetBytes((uint) bumpMap[y, x]);
                            break;
                    }
                    //Console.WriteLine("Copying {0} to pos {1} S={2}", BitConverter.ToString(worldDataUnit), ((y * width) + x) * worldWordSize, worldWordSize);
                    System.Buffer.BlockCopy(worldDataUnit, 0, worldData, ((y * width) + x) * worldWordSize, worldWordSize);
                    System.Buffer.BlockCopy(bumpDataUnit, 0, bumpData, ((y * width) + x) * bumpWordSize, bumpWordSize);
                }
            }
            
        }

        public bool Collides(Position pos)
        {
            if (bumpMap[pos.y, pos.x] == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Chunk GetChunk(Position pos)
        {
            Position chunkPos = new Position(pos.x >> 3, pos.y >> 3);
            if (chunks.ContainsKey(chunkPos))
            {
                return chunks[chunkPos];
            }
            else
            {
                return chunks[new Position(0, 0)];
            }
        }

        public void MoveEntityChunk(Entity entity, Position prevPos, Position newPos)
        {
            if ( prevPos.x >> 3 != newPos.x >> 3 || prevPos.y >> 3 != newPos.y >> 3)
            {
                Chunk prevChunk = GetChunk(prevPos);
                Chunk newChunk = GetChunk(newPos);
                prevChunk.entities.Remove(entity);
                newChunk.entities.Add(entity);
                Console.WriteLine("Entity moved to chunk {0}, {1}", newChunk.pos.x, newChunk.pos.y);
            }
        }

        public bool MoveEntity(Entity entity, Position newPos, bool force = false)
        {
            Position prevPos = entity.pos;
            if (newPos.x < width && newPos.x >= 0 && newPos.y < height && newPos.y >= 0)
            {
                if (force || !Collides(newPos))
                {
                    if (portals.ContainsKey(newPos))
                    {
                        entity.pos = portals[newPos];
                    }
                    else
                    {
                        entity.pos = newPos;
                    }
                    MoveEntityChunk(entity, prevPos, entity.pos);
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

        public void SetWorldMap(int[,] worldMap)
        {
            this.worldMap = worldMap;
            CalculateWordSizes();
            CalculateData();
        }

        public void SetBumpMap(int[,] bumpMap)
        {
            this.bumpMap = bumpMap;
            CalculateWordSizes();
            CalculateData();
        }

        public int[,] GetWorldMap() => this.worldMap;

        public int[,] GetBumpMap() => this.bumpMap;

        public int GetWorldWordSize() => worldWordSize;

        public int GetBumpWordSize() => bumpWordSize;

        public byte[] GetWorldBytes()
        {
            return worldData;
        }

        public byte[] GetBumpBytes()
        {
            return bumpData;
        }

    }
}
