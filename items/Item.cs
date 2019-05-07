using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace LegendSharp
{
    public class Item
    {
        String itemSprite;
        String itemName;
        String itemDescription;
        String itemType;
        BaseItem baseItem;
        public Item(BaseItem baseItem, String itemSprite = null, String itemName = null, String itemDescription = null, String itemType = null)
        {
            this.baseItem = baseItem;
            this.itemSprite = itemSprite;
            this.itemName = itemName;
            this.itemDescription = itemDescription;
            this.itemType = itemType;
        }

        public String GetSprite()
        {
            return itemSprite ?? baseItem.sprite;
        }

        public String GetName()
        {
            return itemName ?? baseItem.name;
        }

        public String GetDescription()
        {
            return itemDescription ?? itemDescription;
        }

        public String GetItemType()
        {
            return itemType ?? itemType;
        }

        public bool HasSprite()
        {
            return itemSprite != null;
        }

        public bool HasName()
        {
            return itemName != null;
        }

        public bool HasDescription()
        {
            return itemDescription != null;
        }

        public bool HasItemType()
        {
            return itemType != null;
        }

        public static Item DecodeItem(BsonDocument itemDocument, Config config)
        {
            String baseItemId = itemDocument.GetValue("base").AsString;
            BaseItem baseItem;
            if (config.baseItems.ContainsKey(baseItemId))
            {
                baseItem = config.baseItems[baseItemId];
            }
            else
            {
                baseItem = new BaseItem("unknown", "unknown", "You don't know anything about this item");
            }

            String itemType = null;
            String itemSprite = null;
            String itemName = null;
            String itemDescription = null;

            if (itemDocument.Contains("item_type"))
            {
                itemType = itemDocument["item_type"].AsString;
            }
            if (itemDocument.Contains("sprite"))
            {
                itemSprite = itemDocument["sprite"].AsString;
            }
            if (itemDocument.Contains("name"))
            {
                itemName = itemDocument["name"].AsString;
            }
            if (itemDocument.Contains("description"))
            {
                itemDescription = itemDocument["description"].AsString;
            }

            if (itemType == "weapon" || (itemType == null && baseItem.type == "weapon"))
            {
                String weaponClass = null;
                double damage = Double.NaN;
                String damageType = null;
                if (itemDocument.Contains("weapon_class"))
                {
                    weaponClass = itemDocument["weapon_class"].AsString;
                }
                if (itemDocument.Contains("damage"))
                {
                    damage = itemDocument["damage"].AsDouble;
                }
                if (itemDocument.Contains("damage_type"))
                {
                    damageType = itemDocument["damageType"].AsString;
                }
                return new Item(baseItem, itemSprite, itemName, itemDescription, itemType);
            }
            else
            {
                return new Item(baseItem, itemSprite, itemName, itemDescription, itemType);
            }

        }
    }
}
