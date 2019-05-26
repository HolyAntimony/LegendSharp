using LegendDialogue;
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
                Position newPos = new Position(movePacket.x, movePacket.y);
                player.facing = (FACING) movePacket.facing;
                legend.world.MoveEntity(player, newPos, legend.config);
                if (player.pos != newPos)
                {
                    handler.SendPacket(new PlayerPositionPacket(player.pos.x, player.pos.y));
                }
            }
            else if (packet is MovePacket)
            {
                MovePacket movePacket = (MovePacket)packet;
                Position newPos = new Position(movePacket.x, movePacket.y);
                legend.world.MoveEntity(player, newPos, legend.config);
                if (player.pos != newPos)
                {
                    handler.SendPacket(new PlayerPositionPacket(player.pos.x, player.pos.y));
                }
            }
            else if (packet is SendMessagePacket)
            {
                SendMessagePacket messagePacket = (SendMessagePacket)packet;
                var msg = new ChatMessage(this.username, messagePacket.message, this.userId);
                legend.world.SendToNearby(msg, player.pos, legend.config);
            }
            else if (packet is EntityInteractPacket)
            {
                EntityInteractPacket interactPacket = (EntityInteractPacket)packet;
                TryInteract(interactPacket.interactType, interactPacket.guid);
            }
        }

        public override void OpenDialogue(string dialogueKey)
        {
            if (legend.config.dialogue.ContainsKey(dialogueKey))
            {
                Dialogue dialogue = legend.config.dialogue[dialogueKey];
                DialoguePacket dialoguePacket = new DialoguePacket(dialogue, this);
                handler.SendPacket(dialoguePacket);
                dialogueOpen = dialogue;
            }
        }

        public override void UpdateEntityPos(Entity entity)
        {
            handler.SendPacket(new EntityMovePacket(entity.pos.x, entity.pos.y, (int)entity.facing, entity.uuid));
        }

        public override void AddToCache(Entity entity)
        {
            cachedEntities.Add(entity);
            handler.SendPacket(new EntityPacket(entity));
        }

        public override void RemoveFromCache(Entity entity)
        {
            cachedEntities.Remove(entity);
            handler.SendPacket(new InvalidateCachePacket(entity.uuid));
        }

        public override void SendMessage(ChatMessage message, Position pos)
        {
            handler.SendPacket(new ChatPacket(message.author, message.message, message.authorId, message.uuid, pos.x, pos.y));
        }
    }
}
