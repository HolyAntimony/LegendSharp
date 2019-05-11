using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace LegendSharp
{
    public class Game
    {
        Config config;

        public bool active = false;

        public String userId;

        public String username;

        public Player player;


        public Game(String userId, String username, Config config, Legend legend)
        {
            this.config = config;
            active = true;
            this.userId = userId;
            this.username = username;

            BsonDocument userData = legend.GetUserData(this.userId);
            if (userData == null)
            {
                userData = new BsonDocument(config.defaultUser);
                userData.Set("user", this.userId);
                legend.InsertUserData(userData);
            }

            Inventory inventory = new Inventory();

            foreach (BsonValue itemValue in userData["inventory"].AsBsonArray)
            {
                Item item = Item.DecodeItem(itemValue.AsBsonDocument, config);
                inventory.AddItem(item);
            }

            player = new Player(
                userData.GetValue("sprite").ToString(),
                userData.GetValue("pos_x").ToInt32(),
                userData.GetValue("pos_y").ToInt32(),
                userData.GetValue("inventory_size").ToInt32(),
                inventory,
                legend
            );

            foreach (var item in player.inventory.items)
            {
                Console.WriteLine("{0}: [{1} / {2}] \"{3}\" ", item.GetName(), item.GetSprite(), item.GetItemType(), item.GetDescription());
            }

        }

        public void Stop()
        {
            active = false;
        }
    }
}
