using MiscUtil.Conversion;
using MongoDB.Bson;
using Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace LegendSharp
{
    public class ItemSubstitution : Substitution
    {
        const short SubId = 2;
        Item item;
        public ItemSubstitution(Item item)
        {
            this.item = item;
        }

        public override string Evaluate(Game game)
        {
            return item.ToString();
        }

        public override byte[] Encode(BigEndianBitConverter converter)
        {
            byte[] idData = converter.GetBytes(SubId);
            DataItem dataItem = new DataItem();
            byte[] itemData = dataItem.encode(item, converter);

            byte[] output = new byte[itemData.Length + 2];

            System.Buffer.BlockCopy(idData, 0, output, 0, 2);
            System.Buffer.BlockCopy(itemData, 0, output, 0, itemData.Length);

            return output;
        }

        public override Substitution Simplify(Game game)
        {
            return new ItemSubstitution(item.GetStatic());
        }
    }
}
