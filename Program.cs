using Packets;
using System;
using System.Collections.Generic;

namespace LegendSharp
{
    public class LegendSharp
    {
        static void Main(string[] args)
        {
            AsynchronousSocketListener.StartListening();
            Console.WriteLine("Hello World!");
        }
    }
}
