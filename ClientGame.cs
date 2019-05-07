using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace LegendSharp
{
    class ClientGame : Game
    {
        ClientHandler handler;

        public ClientGame(ClientHandler handler, String userId, String username, Config config, Legend legend) : base(userId, username, config, legend)
        {
            this.handler = handler;
        }
    }
}
