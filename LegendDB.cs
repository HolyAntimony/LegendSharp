using LegendDialogue;
using LegendItems;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace LegendSharp
{
    class LegendDB
    {
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

        public static Item DecodeItem(BsonDocument itemDocument, Dictionary<String, BaseItem> baseItems)
        {
            String baseItemId = itemDocument.GetValue("base").AsString;
            BaseItem baseItem;
            if (baseItems.ContainsKey(baseItemId))
            {
                baseItem = baseItems[baseItemId];
            }
            else
            {
                baseItem = new BaseItem("unknown", "unknown", "You don't know anything about this item", "unknown");
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
                return new Weapon((BaseWeapon)baseItem, itemSprite, itemName, itemDescription, itemType, weaponClass, damage, damageType);
            }
            else
            {
                return new Item(baseItem, itemSprite, itemName, itemDescription, itemType);
            }

        }

        public static PlayerAction DecodeAction(BsonDocument actionDocument, Dictionary<String, BaseItem> baseItems)
        {
            string actionType = actionDocument.GetValue("type").AsString;
            if (actionType == "set_flag")
            {
                string flagKey = actionDocument.GetValue("flag").AsString;
                Flag flagValue = DecodeFlag(actionDocument.GetValue("value"));
                return new SetFlagAction(flagKey, flagValue);
            }
            else if (actionType == "give_item")
            {
                BsonDocument itemDocument = actionDocument.GetValue("item").AsBsonDocument;
                return new GiveItemAction(DecodeItem(itemDocument, baseItems));
            }
            else if (actionType == "increment_flag")
            {
                string flagKey = actionDocument.GetValue("flag").AsString;
                NumericalFlag amount = (NumericalFlag)DecodeFlag(actionDocument.GetValue("value"));
                return new IncrementFlagAction(flagKey, amount);
            }
            else
            {
                return new NullAction();
            }
        }

        public static Flag DecodeFlag(BsonValue flagValue)
        {
            if (flagValue.IsInt32)
            {
                int flagNumericalValue = flagValue.AsInt32;
                return new NumericalFlag((double)flagNumericalValue);
            }
            else if (flagValue.IsDouble)
            {
                double flagNumericalValue = flagValue.AsDouble;
                return new NumericalFlag((double)flagNumericalValue);
            }
            else
            {
                return new NumericalFlag(0);
            }
        }

        public static Option DecodeOption(BsonDocument optionDocument)
        {
            string text = optionDocument.GetValue("text").AsString;
            string optionType = optionDocument.GetValue("type").AsString;

            List<Requirement> requirements = new List<Requirement>();
            if (optionDocument.Contains("requirements"))
            {
                BsonArray requirementArray = optionDocument.GetValue("requirements").AsBsonArray;
                foreach (BsonValue requirementValue in requirementArray)
                {
                    BsonDocument requirementDocument = requirementValue.AsBsonDocument;
                    requirements.Add(DecodeRequirement(requirementDocument));
                }
            }

            if (optionType == "dialogue")
            {
                string dialogueKey = optionDocument.GetValue("dialogue").AsString;
                return new DialogueOption(text, dialogueKey, requirements.ToArray());
            }
            else
            {
                //End
                return new EndDialogueOption(text, requirements.ToArray());
            }
        }

        public static Requirement DecodeRequirement(BsonDocument requirementDocument)
        {
            string requirementType = requirementDocument.GetValue("type").AsString;
            if (requirementType == "set_flag")
            {
                string flagKey = requirementDocument.GetValue("flag").AsString;
                return new SetRequirement(flagKey);
            }
            else if (requirementType == "null_flag")
            {
                string flagKey = requirementDocument.GetValue("flag").AsString;
                return new NullRequirement(flagKey);
            }
            else if (requirementType == "equals")
            {
                string flagKey = requirementDocument.GetValue("flag").AsString;
                Flag flagValue = DecodeFlag(requirementDocument.GetValue("value"));
                return new EqualsRequirement(flagKey, flagValue);
            }
            else if (requirementType == "less_than")
            {
                string flagKey = requirementDocument.GetValue("flag").AsString;
                NumericalFlag flagValue = (NumericalFlag)DecodeFlag(requirementDocument.GetValue("value"));
                return new LessThanRequirement(flagKey, flagValue);
            }
            else if (requirementType == "more_than")
            {
                string flagKey = requirementDocument.GetValue("flag").AsString;
                NumericalFlag flagValue = (NumericalFlag)DecodeFlag(requirementDocument.GetValue("value"));
                return new MoreThanRequirement(flagKey, flagValue);
            }
            else if (requirementType == "not")
            {
                BsonDocument subRequirementDocument = requirementDocument.GetValue("requirement").AsBsonDocument;
                return new NotRequirement(DecodeRequirement(subRequirementDocument));
            }
            else if (requirementType == "and")
            {
                List<Requirement> requirements = new List<Requirement>();
                BsonArray requirementArray = requirementDocument.GetValue("requirements").AsBsonArray;
                foreach (BsonValue requirementValue in requirementArray)
                {
                    BsonDocument subRequirementDocument = requirementValue.AsBsonDocument;
                    requirements.Add(DecodeRequirement(requirementDocument));
                }
                return new AndRequirement(requirements.ToArray());
            }
            else if (requirementType == "or")
            {
                List<Requirement> requirements = new List<Requirement>();
                BsonArray requirementArray = requirementDocument.GetValue("requirements").AsBsonArray;
                foreach (BsonValue requirementValue in requirementArray)
                {
                    BsonDocument subRequirementDocument = requirementValue.AsBsonDocument;
                    requirements.Add(DecodeRequirement(requirementDocument));
                }
                return new OrRequirement(requirements.ToArray());
            }
            else if (requirementType == "true")
            {
                return new TrueRequirement();
            }
            else
            {
                return new FalseRequirement();
            }
        }

        public static Substitution DecodeSubstitution(BsonDocument subDocument, Config config)
        {
            string subType = subDocument.GetValue("type").AsString;
            if (subType == "flag")
            {
                string flagKey = subDocument.GetValue("flag").AsString;
                return new FlagSubstitution(flagKey);
            }
            else if (subType == "char")
            {
                string flagKey = subDocument.GetValue("flag").AsString;
                return new FlagCharSubstitution(flagKey);
            }
            else if (subType == "item")
            {
                BsonDocument itemDoc = subDocument.GetValue("item").AsBsonDocument;
                Item item = DecodeItem(itemDoc, config.baseItems);
                return new ItemSubstitution(item);
            }
            else if (subType == "text")
            {
                string text = subDocument.GetValue("text").AsString;
                return new StringSubstitution(text);
            }
            else
            {
                return new StringSubstitution("");
            }
        }

        public static Dialogue DecodeDialogue(BsonDocument dialogueDocument, Config config)
        {
            string text = dialogueDocument.GetValue("text").AsString;
            string author = dialogueDocument.GetValue("author").AsString;
            string sprite = dialogueDocument.GetValue("sprite").AsString;

            List<Option> options = new List<Option>();
            if (dialogueDocument.Contains("options"))
            {
                var bsonOptions = dialogueDocument.GetElement("options").Value.AsBsonArray;
                foreach (var bsonOption in bsonOptions)
                {
                    var optionDocument = bsonOption.AsBsonDocument;
                    options.Add(DecodeOption(optionDocument.AsBsonDocument));
                }
            }

            List<PlayerAction> actions = new List<PlayerAction>();
            if (dialogueDocument.Contains("actions"))
            {
                var bsonActions = dialogueDocument.GetElement("actions").Value.AsBsonArray;
                foreach (var bsonAction in bsonActions)
                {
                    var actionDocument = bsonAction.AsBsonDocument;
                    actions.Add(DecodeAction(actionDocument, config.baseItems));
                }
            }

            List<Substitution> substitutions = new List<Substitution>();

            if (dialogueDocument.Contains("substitutions"))
            {
                var bsonSubs = dialogueDocument.GetElement("substitutions").Value.AsBsonArray;
                foreach (var subValue in bsonSubs)
                {
                    var subDocument = subValue.AsBsonDocument;
                    substitutions.Add(DecodeSubstitution(subDocument, config));
                }
            }

            return new Dialogue(text, author, sprite, options.ToArray(), actions.ToArray(), substitutions.ToArray());
        }
    }
}
