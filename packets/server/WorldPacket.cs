using System;


namespace Packets
{
    public class WorldPacket : Packet {
        public override short id { get{ return 3;} }
        public override string name { get{ return "World";} }

        public LegendSharp.World world;

        public static DataType[] schema = {
            new DataUInt(),
            new DataUInt(),
            new DataWorld(),
            new DataBumpWorld()
        };

        public WorldPacket(LegendSharp.World world)
        {
            this.world = world;
        }
        public WorldPacket(byte[] received_data)
        {
            var decoded = Packets.decodeData(schema, received_data);
            uint height = (uint) decoded[0];
            uint width = (uint) decoded[1];
            int[,] worldData = (int [,]) decoded[2];
            int[,] bumpData = (int [,]) decoded[3];
            world = new LegendSharp.World(worldData, bumpData, (int) height, (int) width);
        }

        public override byte[] encode()
        {
            var output = Packets.encodeData(schema, new object[] {(uint) world.height, (uint) world.width, world, world});
            return output;
        }
    }

}