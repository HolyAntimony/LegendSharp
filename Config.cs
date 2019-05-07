using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace LegendSharp
{
    public class Config
    {
        public BsonDocument defaultUser;
        public IPAddress ip;
        public int port;
        public int chatRadius;
        public int entityRadius;
        public int tickRate;
        public Dictionary<String, BaseItem> baseItems;
    }
}
