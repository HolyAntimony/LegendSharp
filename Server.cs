using Packets;
using System;
using System.Net;  
using System.Net.Http;
using System.Net.Sockets;  
using System.Text;  
using System.Threading;  
using System.Collections.Generic;
using MiscUtil.Conversion;

namespace LegendSharp
{
    public class StateObject {  
        // Client  socket.  
        public Socket workSocket = null;  
        // Size of receive buffer.  
        public const int BufferSize = 1024;  
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];  

        public const int PacketBufferSize = 1048576;

        public byte[] packetBuffer = new byte[PacketBufferSize];

        public uint packetLength = 0;
        public uint packetPosition = 0;

        public ClientHandler clientHandler;
    }  

    public class AsynchronousSocketListener {  
        // Thread signal.  
        public static ManualResetEvent allDone = new ManualResetEvent(false);  

        public static BigEndianBitConverter converter = new BigEndianBitConverter();

        public static HttpClient httpClient = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate });

        public static Legend legend = new Legend();
    
        public AsynchronousSocketListener() {  
        }  
    
        public static void StartListening() {  
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Legend Plus login bot v0.1");
            // Establish the local endpoint for the socket.  
            // The DNS name of the computer  
            // running the listener is "host.contoso.com".  
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());  
            IPAddress ipAddress = ipHostInfo.AddressList[0]; 
            ipAddress = IPAddress.Parse("10.1.0.152");
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 21321);  
    
            // Create a TCP/IP socket.  
            Socket listener = new Socket(ipAddress.AddressFamily,  
                SocketType.Stream, ProtocolType.Tcp );  
    
            // Bind the socket to the local endpoint and listen for incoming connections.  
            try {  
                listener.Bind(localEndPoint);  
                listener.Listen(100);  
    
                while (true) {  
                    // Set the event to nonsignaled state.  
                    allDone.Reset();  
    
                    // Start an asynchronous socket to listen for connections.  
                    Console.WriteLine("Waiting for a connection...");  
                    listener.BeginAccept(   
                        new AsyncCallback(AcceptCallback),  
                        listener );  
    
                    // Wait until a connection is made before continuing.  
                    allDone.WaitOne();  
                }  
    
            } catch (Exception e) {  
                Console.WriteLine(e.ToString());  
            }  
    
            Console.WriteLine("\nPress ENTER to continue...");  
            Console.Read();  
    
        }  
    
        public static void AcceptCallback(IAsyncResult ar) {  
            // Signal the main thread to continue.  
            allDone.Set();  
    
            // Get the socket that handles the client request.  
            Socket listener = (Socket) ar.AsyncState;  
            Socket handler = listener.EndAccept(ar);  
    
            // Create the state object.  
            StateObject state = new StateObject();  
            state.workSocket = handler;  
            state.clientHandler = new ClientHandler(handler, legend);

            var readyPacket = new Packets.ReadyPacket(0);
            AsynchronousSocketListener.Send(state.workSocket, readyPacket);

            handler.BeginReceive( state.buffer, 0, StateObject.BufferSize, 0,  
                new AsyncCallback(ReadCallback), state);  
        }  
    
        public static void ReadCallback(IAsyncResult ar) {  
            String content = String.Empty;  
    
            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            StateObject state = (StateObject) ar.AsyncState;  
            Socket handler = state.workSocket;  
    
            // Read data from the client socket.   
            try {
                int bytesRead = handler.EndReceive(ar);  
                //System.Console.WriteLine(BitConverter.ToString( state.buffer ));
                uint offset = 0;
                if (state.packetLength == state.packetPosition) {
                    offset = 4;
                    state.packetLength = converter.ToUInt32(state.buffer, 0);
                    state.packetPosition = 0;
                }
                if (bytesRead > 0) {  
                    // There  might be more data, so store the data received so far.  
                    Buffer.BlockCopy(state.buffer, (int) offset, state.packetBuffer, (int) state.packetPosition, Math.Min((int) bytesRead - (int) offset, (int) (state.packetLength - state.packetPosition)));
                    state.packetPosition += (uint) Math.Min((int) bytesRead - (int) offset, (int) (state.packetLength - state.packetPosition));
                    /*for (var i = offset; i < bytesRead && state.packetPosition < state.packetLength; i++)
                    {
                        state.packetBuffer[state.packetPosition] = state.buffer[i];
                        state.packetPosition++;
                    }*/
                }  
                if (state.packetLength == state.packetPosition) {
                    short packetId = converter.ToInt16(state.packetBuffer, 0);
                    byte[] packetData = new byte[state.packetLength - 2];
                    Buffer.BlockCopy(state.packetBuffer, 2, packetData, 0, (int)state.packetLength-2);
                    //System.Console.WriteLine(BitConverter.ToString( packetData ));
                    var packet = Packets.Packets.decode(packetId, packetData);
                    state.clientHandler.OnPacket(packet);
                }
                state.buffer = new byte[StateObject.BufferSize];
                handler.BeginReceive( state.buffer, 0, StateObject.BufferSize, 0,  
                    new AsyncCallback(ReadCallback), state); 
            }
            catch (SocketException)
            {
                state.clientHandler.OnDisconnect();
                Console.WriteLine("Client disconnected.");
                handler.Shutdown(SocketShutdown.Both);  
                handler.Close();  
            }
        }  
    
        public static void Send(Socket handler, String data) {  
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);  
    
            // Begin sending the data to the remote device.  
            handler.BeginSend(byteData, 0, byteData.Length, 0,  
                new AsyncCallback(SendCallback), handler);  
        }  

        public static void Send(Socket handler, byte[] byteData) {  
    
            // Begin sending the data to the remote device.  
            handler.BeginSend(byteData, 0, byteData.Length, 0,  
                new AsyncCallback(SendCallback), handler);  
        }  

        public static void Send(Socket handler, Packet packet) {
            byte[] byteData = Packets.Packets.encode(packet);
            Console.WriteLine("Sending packet #{0} {1}", packet.id, packet.name);
            handler.BeginSend(byteData, 0, byteData.Length, 0,  
                new AsyncCallback(SendCallback), handler);  
        }
    
        private static void SendCallback(IAsyncResult ar) {  
            try {  
                // Retrieve the socket from the state object.  
                Socket handler = (Socket) ar.AsyncState;  
    
                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);  
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);  
    
            } catch (Exception e) {  
                Console.WriteLine(e.ToString());  
            }  
        }  
    }
}