using LegendSharp;
using MiscUtil.Conversion;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace LegendDialogue
{
    public class FlagCharSubstitution : Substitution
    {
        const short SubId = 1;
        string flagKey;
        public FlagCharSubstitution(string flagKey)
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
                if (game.player.flags.ContainsKey(flagKey) && game.player.flags[flagKey] is NumericalFlag)
                {
                    return Char.ConvertFromUtf32((int)((NumericalFlag)game.player.flags[flagKey]).value);
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
            if (game.player.flags.ContainsKey(flagKey) && game.player.flags[flagKey] is NumericalFlag)
            {
                return new StringSubstitution(Char.ConvertFromUtf32((int)((NumericalFlag)game.player.flags[flagKey]).value));
            }
            else
            {
                return new StringSubstitution("");
            }
        }
    }
}
