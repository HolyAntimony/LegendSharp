using Packets;
using System;

namespace LegendSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            AsynchronousSocketListener.StartListening();
            Console.WriteLine("Hello World!");
        }
    }
}
