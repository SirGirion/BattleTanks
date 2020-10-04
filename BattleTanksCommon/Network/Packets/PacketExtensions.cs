using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace BattleTanksCommon.Network.Packets
{
    public static class PacketExtensions
    {
        public static byte[] ToByteArray(this INetworkPacket packet)
        {
            var size = Marshal.SizeOf(packet);
            var arr = new byte[size];
            var ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(packet, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }
    }
}
