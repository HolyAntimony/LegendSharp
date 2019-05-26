using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace LegendSharp
{
    public class BaseItem
    {
        public String type;
        public String sprite;
        public String name;
        public String description;
        public String itemId;

        public BaseItem(String sprite, String name, String description, String itemId, String type = "item")
        {
            this.type = type;
            this.sprite = sprite;
            this.name = name;
            this.description = description;
            this.itemId = itemId;
        }

        public static BaseItem DecodeBaseItem(BsonDocument itemDocument, String itemId)
        {
            String itemType = itemDocument["item_type"].AsString;
            String itemSprite = itemDocument["sprite"].AsString;
            String itemName = itemDocument["name"].AsString;
            String itemDescription = itemDocument["description"].AsString;

            if (itemType == "weapon")
            {
                String weaponClass = itemDocument["weapon_class"].AsString;
                double damage = itemDocument["damage"].AsDouble;
                String damageType = itemDocument["damage_type"].AsString;
                return new BaseWeapon(itemSprite, itemName, itemDescription, itemId, weaponClass, damage, damageType);
            }
            else
            {
                return new BaseItem(itemSprite, itemName, itemDescription, itemId, itemType);
            }
        }
    }
}
