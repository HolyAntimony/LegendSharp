using LegendDialogue;
using LegendItems;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace LegendSharp
{
    public abstract class PlayerAction
    {
        public abstract void Run(Game game);

        public static PlayerAction DecodeAction(BsonDocument actionDocument, Dictionary<String, BaseItem> baseItems)
        {
            string actionType = actionDocument.GetValue("type").AsString;
            if (actionType == "set_flag")
            {
                string flagKey = actionDocument.GetValue("flag").AsString;
                Flag flagValue = Flag.DecodeFlag(actionDocument.GetValue("value"));
                return new SetFlagAction(flagKey, flagValue);
            }
            else if (actionType == "give_item")
            {
                BsonDocument itemDocument = actionDocument.GetValue("item").AsBsonDocument;
                return new GiveItemAction(Item.DecodeItem(itemDocument, baseItems));
            }
            else
            {
                return new NullAction();
            }
        }
    }
}
