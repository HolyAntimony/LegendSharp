using System;
using System.Collections.Generic;
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

        public Legend()
        {
            LoadConfig();
        }

        public void LoadConfig()
        {
            JObject configJSON = JObject.Parse(File.ReadAllText(@"config/config.json"));
            String worldLocation = "config/" + configJSON.GetValue("world_map").ToString();
            String bumpLocation = "config/" + configJSON.GetValue("bump_map").ToString();
            String portalsLocation = "config/" + configJSON.GetValue("portals").ToString();
            String entitiesLocation = "config/" + configJSON.GetValue("entities").ToString();
            String dialogueLocation = "config/" + configJSON.GetValue("dialogue").ToString();
            String itemsLocation = "config/" + configJSON.GetValue("items").ToString();
            String encountersLocation = "config/" + configJSON.GetValue("encounters").ToString();
            String enemiesLocation = "config/" + configJSON.GetValue("enemies").ToString();

            BsonDocument defaultUser = BsonDocument.Parse(configJSON.GetValue("default_user").ToString());
            IPAddress ip = IPAddress.Parse(configJSON.GetValue("ip").ToString());
            int port = configJSON.GetValue("port").ToObject<int>();
            int chatRadius = configJSON.GetValue("chat_radius").ToObject<int>();
            int entityRadius = configJSON.GetValue("entity_radius").ToObject<int>();
            int tickRate = configJSON.GetValue("tick_rate").ToObject<int>();

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
