using LegendDialogue;
using LegendSharp;
using System;
using System.Collections.Generic;

namespace Packets
{
    public class DialoguePacket : Packet
    {
        public override short id { get { return 11; } }
        public override string name { get { return "Dialogue"; } }

        public static DataType[] schema = {
            new DataString(),
            new DataString(),
            new DataString(),
            new DataOptions(),
            new DataSubstitutions()
        };

        public string text;
        public string author;
        public string sprite;
        public List<OptionView> optionViews;
        public List<Substitution> substitutions;

        public DialoguePacket(Dialogue dialogue, Game game)
        {
            this.text = dialogue.text;
            this.author = dialogue.author;
            this.sprite = dialogue.sprite;
            this.optionViews = dialogue.GetOptionViews(game.player.flags);
            this.substitutions = dialogue.GetSubstitutionViews(game.player.flags, game);
        }

        public DialoguePacket(byte[] received_data)
        {
            var decoded = Packets.decodeData(schema, received_data);
            text = (string)decoded[0];
            author = (string)decoded[1];
            sprite = (string)decoded[2];
            optionViews = (List<OptionView>)decoded[3];
            substitutions = (List<Substitution>)decoded[4];
        }

        public override byte[] encode()
        {
            var output = Packets.encodeData(schema, new object[] { text, author, sprite, optionViews, substitutions });
            return output;
        }
    }

}