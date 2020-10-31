using MessagePack;

namespace BattleTanksCommon.Network.Packets
{
    public static class PacketExtensions
    {
        public static byte[] ToByteArray(this NetworkPacket packet)
        {
            return MessagePackSerializer.Serialize(packet);
        }
    }
}
