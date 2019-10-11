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

        public bool HasOption(Guid uuid)
        {
            foreach (Option option in options)
            {
                if (option.uuid == uuid)
                {
                    return true;
                }
            }
            return false;
        }

        public Option GetOption(Guid uuid)
        {
            foreach (Option option in options)
            {
                if (option.uuid == uuid)
                {
                    return option;
                }
            }
            return null;
        }
    }
}
