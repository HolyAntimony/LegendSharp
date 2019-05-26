using LegendSharp;
using MiscUtil.Conversion;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace LegendDialogue
{
    public class FlagSubstitution : Substitution
    {
        const short SubId = 1;
        string flagKey;
        public FlagSubstitution(string flagKey)
        {
            this.flagKey = flagKey;
        }

        public override byte[] Encode(BigEndianBitConverter converter)
        {
            // This is why you should always simplify first
            byte[] idData = converter.GetBytes(SubId);
            byte[] textData = System.Text.Encoding.UTF8.GetBytes("{" + flagKey + "}");

            byte[] output = new byte[textData.Length + 2];

            System.Buffer.BlockCopy(idData, 0, output, 0, 2);
            System.Buffer.BlockCopy(textData, 0, output, 0, textData.Length);

            return output;
        }

        public override string Evaluate(Game game)
        {
            if (game.active)
            {
                if (game.player.flags.ContainsKey(flagKey))
                {
                    return game.player.flags[flagKey].ToString();
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }

        public override Substitution Simplify(Game game)
        {
            return new StringSubstitution(game.player.flags[flagKey].ToString());
        }
    }
}
