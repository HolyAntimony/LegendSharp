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
        public ClientHandler(Socket handler)
        {
            this.handler = handler;
        }

        public void sendPacket(Packet packet)
        {
            AsynchronousSocketListener.Send(handler, packet);
        }

        public void onPacket(Packet packet)
        {
            System.Console.WriteLine("Received packet {0}, ID: {1}", packet.name, packet.id);
            if (packet is PingPacket)
            {
                var parsed = (PingPacket) packet;
                var response = new PongPacket(parsed.message);
                sendPacket(response);
            }
            else if (packet is LoginPacket)
            {
                var parsed = (LoginPacket) packet;
                AsynchronousSocketListener.httpClient.BaseAddress = new Uri("https://discordapp.com/api/");
                var request = new HttpRequestMessage() {
                    RequestUri = new Uri("https://discordapp.com/api/users/@me"),
                    Method = HttpMethod.Get,
                };
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", parsed.accessToken);
                HttpResponseMessage requestRespose = AsynchronousSocketListener.httpClient.SendAsync(request).Result;;
                requestRespose.EnsureSuccessStatusCode();
                string result = requestRespose.Content.ReadAsStringAsync().Result;
                JObject parsedResult = JObject.Parse(result);
                if (parsedResult["id"] != null)
                {
                    String id = parsedResult["id"].ToString();
                    String username = parsedResult["username"].ToString() + "#" + parsedResult["discriminator"].ToString();
                    Console.WriteLine("Successful login from {0} ({1})", id, username);
                    var response = new LoginResultPacket(1, id);
                    sendPacket(response);
                }
                else
                {
                    Console.WriteLine("Login Failed.");
                    var response = new LoginResultPacket(0);
                    sendPacket(response);
                }
            }
        }
    }
}