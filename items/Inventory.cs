using System;
using System.Collections.Generic;
using System.Text;

namespace LegendSharp
{
    public class Inventory
    {
        public List<Item> items = new List<Item>();
        
        public void AddItem(Item item)
        {
            items.Add(item);
        }
    }
}
