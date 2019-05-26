using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace LegendSharp
{
    public abstract class Option
    {
        public string text;
        public Requirement[] requirements;
        public Guid uuid;

        public bool IsDisplayed(Dictionary<string, Flag> flags)
        {
            foreach (Requirement requirement in requirements)
            {
                if (!requirement.Validate(flags))
                {
                    return false;
                }
            }
            return true;
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
                    requirements.Add(Requirement.DecodeRequirement(requirementDocument));
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
    }
}
