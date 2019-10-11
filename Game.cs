using LegendDialogue;
using LegendItems;
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

        public Dialogue dialogueOpen = null;

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
            inventory.ItemAddedToInventory += new Inventory.ItemAddedToInventoryHandler(ItemAddedToInventory);
            inventory.ItemModified += new Inventory.ItemModifiedHandler(ItemModified);

            foreach (BsonValue itemValue in userData["inventory"].AsBsonArray)
            {
                Item item = LegendDB.DecodeItem(itemValue.AsBsonDocument, config.baseItems);
                inventory.AddItem(item, false);
            }

            Dictionary<string, Flag> flags = new Dictionary<string, Flag>();

            foreach (var flagPair in userData["flags"].AsBsonDocument)
            {
                string flagKey = flagPair.Name;
                Flag flagValue = LegendDB.DecodeFlag(flagPair.Value);
                flags[flagKey] = flagValue;
            }

            player = new Player(
                userData.GetValue("sprite").ToString(),
                userData.GetValue("pos_x").ToInt32(),
                userData.GetValue("pos_y").ToInt32(),
                userData.GetValue("inventory_size").ToInt32(),
                inventory,
                flags,
                legend,
                this
            );

            inventory.guid = player.uuid;

            foreach (var item in player.inventory.items)
            {
                Console.WriteLine("{0}: [{1} / {2}] \"{3}\" x{4} ", item.GetName(), item.GetSprite(), item.GetItemType(), item.GetDescription(), item.GetQuantity());
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

            BsonArray bsonInventory = new BsonArray();
            foreach (Item item in this.player.inventory.items)
            {
                BsonDocument itemDocument = new BsonDocument();
                itemDocument.Set("base", item.baseItem.itemId);
                Console.WriteLine("Saving {0} x{1}", item.GetName(), item.GetQuantity());
                itemDocument.Set("quantity", item.GetQuantity());
                if (item.HasDescription())
                {
                    itemDocument.Set("description", item.GetDescription());
                }
                if (item.HasItemType())
                {
                    itemDocument.Set("item_type", item.GetItemType());
                }
                if (item.HasName())
                {
                    itemDocument.Set("name", item.GetName());
                }
                if (item.HasSprite())
                {
                    itemDocument.Set("sprite", item.GetSprite());
                }
                if (item.HasMaxStack())
                {
                    itemDocument.Set("max_stack", item.GetMaxStack());
                }
                if (item is Weapon)
                {
                    Weapon weapon = (Weapon)item;
                    if (weapon.HasDamage())
                    {
                        itemDocument.Set("damage", weapon.GetDamage());
                    }
                    if (weapon.HasDamageType())
                    {
                        itemDocument.Set("damage_type", weapon.GetDamageType());
                    }
                    if (weapon.HasWeaponClass())
                    {
                        itemDocument.Set("weapon_class", weapon.GetWeaponClass());
                    }
                }
                bsonInventory.Add(itemDocument);
            }            

            userData.Set("inventory", bsonInventory);

            BsonDocument flags = new BsonDocument();
            foreach (KeyValuePair<string, Flag> entry in player.flags)
            {
                flags.Set(entry.Key, entry.Value.GetValue());
            }

            userData.Set("flags", flags);

            userUpdate.Set("$set", userData);

            legend.userCollection.UpdateOne(userFilter, userUpdate);
        }

        public void ItemAddedToInventory(Inventory inventory, Item item, int index)
        {
            AddToInventory(inventory.guid, item, index);
        }

        public void ItemModified(Inventory inventory, Item item, int index)
        {
            ChangeInInventory(inventory.guid, item, index);
        }

        public void TryInteract(short interactType, Guid guid)
        {
            Entity entity = legend.world.GetEntity(guid);
            if (entity != null && legend.world.GetSimpleDistance(player.pos, entity.pos) <= legend.config.interactRange)
            {
                entity.Interact(interactType, player);
            }
        }

        public virtual void OpenDialogue(string dialogueKey)
        {

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

        public virtual void AddToInventory(Guid guid, Item item, int index)
        {
            Console.WriteLine("Game called");
        }

        public virtual void RemoveFromInventory(int index, Item item)
        {

        }

        public virtual void ChangeInInventory(Guid guid, Item item, int index)
        {

        }
    }
}
