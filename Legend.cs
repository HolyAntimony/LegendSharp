using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;

namespace LegendSharp
{
    public enum FACING
    {
        LEFT = 0,
        UP = 1,
        DOWN = 2,
        RIGHT = 3,
    }

    public class Legend
    {
        Dictionary<String, Game> games = new Dictionary<String, Game>();
        public MongoClient mongoClient;
        public IMongoDatabase db;
        public IMongoCollection<BsonDocument> userCollection;

        public Config config;
        public World world;
        

        public Legend()
        {
            LoadConfig();
        }

        public void LoadConfig()
        {
            /*
             * GET Config & locations of other files
             * */
            JObject configJSON = JObject.Parse(File.ReadAllText(@"config/config.json")); //----
            String worldLocation = "config/" + configJSON.GetValue("world_map").ToString(); //----
            String bumpLocation = "config/" + configJSON.GetValue("bump_map").ToString();
            String portalsLocation = "config/" + configJSON.GetValue("portals").ToString(); //----
            String entitiesLocation = "config/" + configJSON.GetValue("entities").ToString();
            String dialogueLocation = "config/" + configJSON.GetValue("dialogue").ToString();
            String itemsLocation = "config/" + configJSON.GetValue("items").ToString(); //----
            String encountersLocation = "config/" + configJSON.GetValue("encounters").ToString();
            String enemiesLocation = "config/" + configJSON.GetValue("enemies").ToString();
            String tilesLocation = "config/" + configJSON.GetValue("tiles").ToString(); //----

            /*
             * Build config values into a config object
             * */

            BsonDocument defaultUser = BsonDocument.Parse(configJSON.GetValue("default_user").ToString());
            IPAddress ip = IPAddress.Parse(configJSON.GetValue("ip").ToString());
            int port = configJSON.GetValue("port").ToObject<int>();
            int chatRadius = configJSON.GetValue("chat_radius").ToObject<int>();
            int entityRadius = configJSON.GetValue("entity_radius").ToObject<int>();
            int tickRate = configJSON.GetValue("tick_rate").ToObject<int>();

            /*
             * Load base items
             * */

            JObject itemsJSON = JObject.Parse(File.ReadAllText(itemsLocation));

            Dictionary<String, BaseItem> baseItems = new Dictionary<String, BaseItem>();

            foreach (var itemPair in itemsJSON)
            {
                string itemId = itemPair.Key;
                JToken itemToken = itemPair.Value;
                BaseItem item = BaseItem.DecodeBaseItem(BsonDocument.Parse(itemToken.ToString()));
                baseItems[itemId] = item;
            }


            config = new Config()
            {
                defaultUser = defaultUser,
                ip = ip,
                port = port,
                chatRadius = chatRadius,
                entityRadius = entityRadius,
                tickRate = tickRate,
                baseItems = baseItems,
            };

            /*
             * Connect to database
             */
            String mongoServer = configJSON.GetValue("db_server").ToString();
            int mongoPort = configJSON.GetValue("db_port").ToObject<int>();
            String mongoUser = configJSON.GetValue("db_user").ToString();
            String mongoPassword = configJSON.GetValue("db_password").ToString();
            String mongoDatabase = configJSON.GetValue("db_database").ToString();

            MongoClientSettings mongoSettings = new MongoClientSettings
            {
                Server = new MongoServerAddress(mongoServer, mongoPort),
                Credential = MongoCredential.CreateCredential(mongoDatabase, mongoUser, mongoPassword)
            };

            mongoClient = new MongoClient(mongoSettings);
            db = mongoClient.GetDatabase(mongoDatabase);
            userCollection = db.GetCollection<BsonDocument>("users");

            /*
             * Get Portals
             * */
            Dictionary<Position, Position> portals = new Dictionary<Position, Position>();

            JArray portalsJArray = JArray.Parse(File.ReadAllText(portalsLocation));
            foreach (var portalToken in portalsJArray)
            {
                JObject portal = portalToken.ToObject<JObject>();
                var fromX = portal.GetValue("pos_x").ToObject<int>();
                var fromY = portal.GetValue("pos_y").ToObject<int>();
                var toX = portal.GetValue("to_x").ToObject<int>();
                var toY = portal.GetValue("to_y").ToObject<int>();
                Position from = new Position(fromX, fromY);
                Position to = new Position(toX, toY);
                portals[from] = to;
            }

            /*
 * Get Tiles
 * */
            JObject tilesJSON = JObject.Parse(File.ReadAllText(tilesLocation));

            List<JToken> tilesList = tilesJSON.GetValue("tiles").ToObject<List<JToken>>();
            Dictionary<Color, int> colorMap = new Dictionary<Color, int>();
            Dictionary<Color, int> bumpColorMap = new Dictionary<Color, int>
            {
                [Color.FromArgb(0, 0, 0)] = 0,
                [Color.FromArgb(255, 255, 255)] = 1,
                [Color.FromArgb(255, 0, 0)] = 2
            };

            for (var i = 0; i < tilesList.Count; i++)
            {
                var tileDocument = tilesList[i];
                Color color = System.Drawing.ColorTranslator.FromHtml(tileDocument.ToObject<JObject>().GetValue("color").ToString());
                colorMap[color] = i;
            }


            /*
             * Get world
             * */

            Bitmap worldImage = new Bitmap(worldLocation);
            Bitmap bumpImage = new Bitmap(bumpLocation);
            int worldHeight = worldImage.Height;
            int worldWidth = worldImage.Width;
            int[,] worldData = new int[worldHeight, worldWidth];
            int[,] bumpData = new int[worldHeight, worldWidth];

            for (int y = 0; y < worldHeight; y++)
            {
                for (int x = 0; x < worldWidth; x++)
                {
                    Color cell = worldImage.GetPixel(x, y);
                    int colIndex = 0;
                    if (colorMap.ContainsKey(cell))
                    {
                        colIndex = colorMap[cell];
                    }
                    worldData[y, x] = colIndex;

                    Color bumpCell = bumpImage.GetPixel(x, y);
                    int bumpColIndex = 0;
                    if (bumpColorMap.ContainsKey(bumpCell))
                    {
                        bumpColIndex = bumpColorMap[bumpCell];
                    }
                    bumpData[y, x] = bumpColIndex;
                }
            }
            world = new World(worldData, bumpData, worldHeight, worldWidth, portals);

            /*
             * Load Entities
             * */

            JArray entitiesJArray = JArray.Parse(File.ReadAllText(entitiesLocation));
            foreach (var entityToken in entitiesJArray)
            {
                JObject entityObject = entityToken.ToObject<JObject>();
                int posX = entityObject.GetValue("pos_x").ToObject<int>();
                int posY = entityObject.GetValue("pos_y").ToObject<int>();
                int facing = entityObject.GetValue("facing").ToObject<int>();
                string sprite = entityObject.GetValue("sprite").ToString();
                string entityType = entityObject.GetValue("type").ToString();
                if (entityType == "npc")
                {
                    string dialogueKey = entityObject.GetValue("dialogue").ToString();
                    NPC npc = new NPC(sprite, posX, posY, (FACING)facing, dialogueKey, this);
                }
            }
        }


        public BsonDocument GetUserData(String id)
        {
            List<BsonDocument> list = userCollection.FindSync(new BsonDocument("user", id)).ToList();
            if (list.Count > 0)
            {
                return list[0];
            }
            else
            {
                return null;
            }
        }

        public void InsertUserData(BsonDocument userData)
        {
            userCollection.InsertOne(userData);
        }

        public void AddGame(String id, Game game)
        {
            if (!games.ContainsKey(id))
            {
                games.Add(id, game);
            }
            else
            {
                games[id] = game;
            }
            
        }

        public Game GetGame(String id)
        {
            if (games.ContainsKey(id))
            {
                return games[id];
            }
            else
            {
                return null;
            }
        }

        public bool HasGame(String id)
        {
            return games.ContainsKey(id);
        }

        public void StopGame(String id)
        {
            if (games.ContainsKey(id))
            {
                games[id].Stop();
                games.Remove(id);
            }
        }
    }
}
