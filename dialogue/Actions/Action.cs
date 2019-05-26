using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace LegendSharp
{
    public abstract class Action
    {
        public abstract void Run(Game game);

        public static Action DecodeAction(BsonDocument actionDocument, Config config)
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
                return new GiveItemAction(Item.DecodeItem(itemDocument, config));
            }
            else
            {
                return new NullAction();
            }
        }
    }
}
