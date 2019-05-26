using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace LegendDialogue
{
    public abstract class Requirement
    {
        public abstract bool Validate(Dictionary<string, Flag> flags);

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
                Flag flagValue = Flag.DecodeFlag(requirementDocument.GetValue("value"));
                return new EqualsRequirement(flagKey, flagValue);
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
    }
}
