using LegendSharp;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace LegendDialogue
{
    public class Dialogue
    {
        public string text;
        public string author;
        public string sprite;
        public Option[] options;
        public PlayerAction[] actions;
        public Substitution[] substitutions;

        public Dialogue(string text, string author, string sprite, Option[] options, PlayerAction[] actions = null, Substitution[] substitutions = null)
        {
            this.text = text;
            this.author = author;
            this.sprite = sprite;
            this.options = options;
            this.actions = actions;
            this.substitutions = substitutions;
        }

        public List<OptionView> GetOptionViews(Dictionary<string, Flag> flags)
        {
            List<OptionView> optionViews = new List<OptionView>();
            foreach (Option option in options)
            {
                if (option.IsDisplayed(flags))
                {
                    optionViews.Add(new OptionView(option));
                }
            }
            return optionViews;
        }

        public List<Substitution> GetSubstitutionViews(Dictionary<string, Flag> flags, Game game)
        {
            List<Substitution> substitutionViews = new List<Substitution>();
            foreach (Substitution substitution in substitutions)
            {
                var sub = substitution.Simplify(game);
                substitutionViews.Add(sub);
            }
            return substitutionViews;
        }

        public string GetText(Game game)
        {
            List<String> textSubstitutions = new List<string>();
            foreach (Substitution sub in substitutions)
            {
                textSubstitutions.Add(sub.Evaluate(game));
            }
            return String.Format(text, textSubstitutions.ToArray());
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
                    options.Add(Option.DecodeOption(optionDocument.AsBsonDocument));
                }
            }

            List<PlayerAction> actions = new List<PlayerAction>();
            if (dialogueDocument.Contains("actions"))
            {
                var bsonActions = dialogueDocument.GetElement("actions").Value.AsBsonArray;
                foreach (var bsonAction in bsonActions)
                {
                    var actionDocument = bsonAction.AsBsonDocument;
                    actions.Add(PlayerAction.DecodeAction(actionDocument, config.baseItems));
                }
            }

            List<Substitution> substitutions = new List<Substitution>();
            
            if (dialogueDocument.Contains("substitutions"))
            {
                var bsonSubs = dialogueDocument.GetElement("substitutions").Value.AsBsonArray;
                foreach (var subValue in bsonSubs)
                {
                    var subDocument = subValue.AsBsonDocument;
                    substitutions.Add(Substitution.DecodeSubstitution(subDocument, config));
                }
            }

            return new Dialogue(text, author, sprite, options.ToArray(), actions.ToArray(), substitutions.ToArray());
        }
    }
}
