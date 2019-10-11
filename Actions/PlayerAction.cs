using LegendDialogue;
using LegendItems;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace LegendSharp
{
    public abstract class PlayerAction
    {
        public abstract void Run(Game game);
    }
}
