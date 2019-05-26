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
        public List<Entity> entities = new List<Entity>();
        public List<Player> players = new List<Player>();
        public List<Entity> movedEntities = new List<Entity>();
        public Dictionary<Guid, Entity> entitiesByUuid = new Dictionary<Guid, Entity>();

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

        public Chunk GetChunkFromPos(Position pos)
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

        public Chunk GetChunk(Position pos)
        {
            return chunks[pos];
        }

        public Entity GetEntity(Guid guid)
        {
            if (entitiesByUuid.ContainsKey(guid))
            {
                return entitiesByUuid[guid];
            }
            else
            {
                return null;
            }
        }

        public int GetSimpleDistance(Position pos1, Position pos2)
        {
            int xDistance = Math.Abs(pos1.x - pos2.x);
            int yDistance = Math.Abs(pos1.y - pos2.y);
            return xDistance > yDistance ? xDistance : yDistance;
        }

        public bool InCacheRange(Entity entity, Position pos, Config config)
        {
            int xDistance = Math.Abs(entity.chunk.pos.x - pos.x);
            int yDistance = Math.Abs(entity.chunk.pos.y - pos.y);
            //Console.WriteLine("xDist {0} yDist {1} less than {2}, {3}", xDistance, yDistance, config.entityDistanceX, config.entityDistanceY);
            if (xDistance <= config.entityDistanceX && yDistance <= config.entityDistanceY)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Cache(Game game, Entity entity)
        {
            if (entity is Player)
            {
                if (((Player)entity).game == game)
                {
                    return;
                }
            }
            Console.WriteLine("Adding {0} to {1}'s cache", entity.uuid, game.player.uuid);
            game.AddToCache(entity);
            game.justCached = true;
            entity.cachedBy.Add(game);
        }

        public void Uncache(Game game, Entity entity)
        {
            Console.WriteLine("Uncache {0} from {1}", entity.uuid, game.player.uuid);
            if (entity is Player)
            {
                if (((Player)entity).game == game)
                {
                    return;
                }
            }
            game.RemoveFromCache(entity);
            entity.cachedBy.Remove(game);
        }

        public void MoveEntityChunk(Entity entity, Position prevPos, Position newPos, Config config)
        {
            if ( prevPos.x >> 3 != newPos.x >> 3 || prevPos.y >> 3 != newPos.y >> 3)
            {
                Chunk prevChunk = GetChunkFromPos(prevPos);
                Chunk newChunk = GetChunkFromPos(newPos);
                prevChunk.entities.Remove(entity);
                newChunk.entities.Add(entity);
                entity.chunk = newChunk;
            }
        }

        public bool MoveEntity(Entity entity, Position newPos, Config config, bool force = false)
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
                    if (prevPos.x != newPos.x || prevPos.y != newPos.y || force)
                    {
                        MoveEntityChunk(entity, prevPos, entity.pos, config);
                    }
                    entity.moved = true;
                    if (prevPos.x >> 3 != entity.pos.x >> 3 || prevPos.y >> 3 != entity.pos.y >> 3)
                    {
                        entity.movedChunks = true;
                    }
                    movedEntities.Add(entity);
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

        public void AddEntity(Entity entity, Config config)
        {
            Chunk targetChunk = GetChunkFromPos(entity.pos);
            targetChunk.entities.Add(entity);
            entities.Add(entity);
            movedEntities.Add(entity);
            entitiesByUuid.Add(entity.uuid, entity);
            entity.moved = true;
            entity.movedChunks = true;
            //UpdateEntityCaches(entity, config);
            if (entity is Player)
            {
                players.Add((Player)entity);
            }
        }
        
        public void RemoveEntity(Entity entity)
        {
            List<Game> toUncache = new List<Game>();
            Chunk entityChunk = entity.chunk;
            entityChunk.entities.Remove(entity);
            entities.Remove(entity);
            entitiesByUuid.Remove(entity.uuid);
            if (entity is Player)
            {
                players.Remove((Player) entity);
                var playerEntity = (Player)entity;
                foreach (var cachedEntity in playerEntity.game.cachedEntities)
                {
                    cachedEntity.cachedBy.Remove(playerEntity.game);
                }
            }
            foreach (Game game in entity.cachedBy)
            {
                toUncache.Add(game);
            }
            foreach (Game game in toUncache)
            {
                Uncache(game, entity);
            }
        }

        public void SendToNearby(ChatMessage message, Position pos, Config config)
        {
            Position chunkPos = GetChunkFromPos(pos).pos;
            int chunkMinX = Math.Max(0, chunkPos.x - (config.chatRadius << 3));
            int chunkMaxX = Math.Min((width >> 3) - 1, chunkPos.x + (config.chatRadius << 3));
            int chunkMinY = Math.Max(0, chunkPos.y - (config.chatRadius << 3));
            int chunkMaxY = Math.Min((height >> 3) - 1, chunkPos.y + (config.chatRadius << 3));
            for (int x = chunkMinX; x <= chunkMaxX; x++)
            {
                for (int y = chunkMinY; y <= chunkMaxY; y++)
                {
                    foreach (Entity chunkEntity in chunks[new Position(x, y)].entities)
                    {
                        if (chunkEntity is Player && Position.WithinDistance(chunkEntity.pos, pos, config.chatRadius))
                        {
                            ((Player)chunkEntity).game.SendMessage(message, pos);
                        }
                    }
                }
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
