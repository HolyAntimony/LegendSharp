using MongoDB.Bson;
using Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace LegendSharp
{
    class ClientGame : Game
    {
        ClientHandler handler;
        Legend legend;

        public ClientGame(ClientHandler handler, String userId, String username, Config config, Legend legend) : base(userId, username, config, legend)
        {
            this.handler = handler;
            this.legend = legend;
            handler.SendPacket(new ReadyPacket(1));
            handler.SendPacket(new PlayerPositionPacket(this.player.pos.x, this.player.pos.y));
        }

        public void HandlePacket(Packet packet)
        {
            if (packet is RequestWorldPacket)
            {
                handler.SendPacket(new WorldPacket(legend.world));
            }
            else if (packet is MoveAndFacePacket)
            {
                MoveAndFacePacket movePacket = (MoveAndFacePacket)packet;
                Position newPos = new Position()
                {
                    x = movePacket.x,
                    y = movePacket.y
                };
                player.facing = (FACING) movePacket.facing;
                player.Move(newPos);
                if (player.pos != newPos)
                {
                    handler.SendPacket(new PlayerPositionPacket(player.pos.x, player.pos.y));
                }
            }
            else if (packet is MovePacket)
            {
                MovePacket movePacket = (MovePacket)packet;
                Position newPos = new Position()
                {
                    x = movePacket.x,
                    y = movePacket.y
                };
                player.Move(newPos);
                if (player.pos != newPos)
                {
                    handler.SendPacket(new PlayerPositionPacket(player.pos.x, player.pos.y));
                }
            }
        }
    }
}
