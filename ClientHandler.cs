using Packets;
using System;
using System.Net;  
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;  
using System.Collections.Generic;
using MiscUtil.Conversion;
using Newtonsoft.Json.Linq;

namespace LegendSharp
{
    public class ClientHandler
    {
        Socket handler;
        ClientGame game;
        bool loggedIn = false;
        String loggedInUser;
        String loggedInUsername;

        Legend legend;

        readonly Type[] GAME_PACKETS = new Type[] {
            typeof(RequestWorldPacket),
            typeof(MoveAndFacePacket),
            typeof(SendMessagePacket),
            typeof(EntityInteractPacket),
            typeof(GUIOptionPacket)
        };

        public ClientHandler(Socket handler, Legend legend)
        {
            this.handler = handler;
            this.game = null;
            this.legend = legend;
        }

        public void SendPacket(Packet packet)
        {
            AsynchronousSocketListener.Send(handler, packet);
        }

        public void OnPacket(Packet packet)
        {
            System.Console.WriteLine("Received packet {0}, ID: {1}", packet.name, packet.id);
            if (packet is PingPacket)
            {
                OnPing((PingPacket)packet);
            }
            else if (packet is LoginPacket)
            {
                OnLogin((LoginPacket)packet);
            }
            else if (packet is JoinGamePacket)
            {
                OnJoinGame((JoinGamePacket)packet);
            }
            else if (this.game != null && this.game.active && Array.IndexOf(GAME_PACKETS, packet.GetType()) > -1 )
            {
                this.game.HandlePacket(packet);
            }
        }

        public void OnDisconnect()
        {
            if (this.game != null)
            {
                this.game.Stop();
            }
        }


        public void OnPing(PingPacket packet)
        {
            var response = new PongPacket(packet.message);
            SendPacket(response);
        }

        public void OnLogin(LoginPacket packet)
        {
            try
            {
                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri("https://discordapp.com/api/users/@me"),
                    Method = HttpMethod.Get,
                };
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", packet.accessToken);
                HttpResponseMessage requestRespose = AsynchronousSocketListener.httpClient.SendAsync(request).Result; ;
                requestRespose.EnsureSuccessStatusCode();
                string result = requestRespose.Content.ReadAsStringAsync().Result;
                JObject parsedResult = JObject.Parse(result);
                if (parsedResult["id"] != null)
                {
                    String id = parsedResult["id"].ToString();
                    String username = parsedResult["username"].ToString() + "#" + parsedResult["discriminator"].ToString();
                    Console.WriteLine("Successful login from {0} ({1})", id, username);

                    loggedIn = true;
                    loggedInUser = id;
                    loggedInUsername = username;

                    var response = new LoginResultPacket(1, id);
                    SendPacket(response);
                }
                else
                {
                    Console.WriteLine("Login Failed.");
                    var response = new LoginResultPacket(0);
                    SendPacket(response);
                }
            }
            catch (System.Net.Http.HttpRequestException)
            {
                Console.WriteLine("Login Errored.");
                var response = new LoginResultPacket(0);
                SendPacket(response);
            }
        }

        public void OnJoinGame(JoinGamePacket packet)
        {
            if (loggedIn && ( game == null || !game.active ) )
            {
                if (legend.HasGame(loggedInUser))
                {
                    legend.StopGame(loggedInUser);
                }

                game = new ClientGame(this, loggedInUser, loggedInUsername, legend.config, legend);
                legend.AddGame(loggedInUser, game);
            }
        }

    }
}