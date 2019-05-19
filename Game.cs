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

        public HashSet<Entity> cachedEntities;

        public bool justCached = false;

        Legend legend;


        public Game(String userId, String username, Config config, Legend legend)
        {
            cachedEntities = new HashSet<Entity>();
            this.config = config;
            active = true;
            this.userId = userId;
            this.username = username;
            this.legend = legend;

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
                legend,
                this
            );

            foreach (var item in player.inventory.items)
            {
                Console.WriteLine("{0}: [{1} / {2}] \"{3}\" ", item.GetName(), item.GetSprite(), item.GetItemType(), item.GetDescription());
            }

        }

        public void Stop()
        {
            active = false;
            if (this.player != null)
            {
                legend.world.RemoveEntity(player);
            }
            var userFilter = new BsonDocument();
            userFilter.Set("user", this.userId);

            var userUpdate = new BsonDocument();

            var userData = new BsonDocument();
            userData.Set("pos_x", this.player.pos.x);
            userData.Set("pos_y", this.player.pos.y);
            userData.Set("sprite", this.player.sprite);
            userData.Set("inventory_size", this.player.inventorySize);

            userUpdate.Set("$set", userData);

            legend.userCollection.UpdateOne(userFilter, userUpdate);
        }

        public virtual void UpdateEntityPos(Entity entity)
        {

        }

        public virtual void AddToCache(Entity entity)
        {
            cachedEntities.Add(entity);
        }

        public virtual void RemoveFromCache(Entity entity)
        {
            cachedEntities.Remove(entity);
        }

        public virtual void SendMessage(ChatMessage message, Position pos)
        {

        }
    }
}
