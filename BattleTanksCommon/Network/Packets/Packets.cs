using System.Runtime.InteropServices;

namespace BattleTanksCommon.Network.Packets
{
    public interface INetworkPacket
    {
    }

    public enum Packets : byte
    {
        LOGIN_PACKET = 1,
        NEW_PLAYER_PACKET = 2,
        PLAYER_DELTA_UPDATE_PACKET = 3,
        ENTITY_SPAWN_PACKET = 4
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LoginPacket : INetworkPacket
    {
        public byte MsgType;
        public int ClientId;
        public int PlayerId;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct NewPlayerPacket : INetworkPacket
    {
        public byte MsgType;
        public int PlayerId;
        public int PlayerX;
        public int PlayerY;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PlayerDeltaUpdatePacket : INetworkPacket
    {
        public byte MsgType;
        public int PlayerId;
        public int DeltaPlayerX;
        public int DeltaPlayerY;
        public short DeltaPlayerRotation;
        public int DeltaPlayerVelocityX;
        public int DeltaPlayerVelocityY;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct EntitySpawnPacket : INetworkPacket
    {
        public byte MsgType;
        public uint EntityId;
        public int EntityX;
        public int EntityY;
        public short Rotation;
        public int VelocityX;
        public int VelocityY;
    }

    public class NetworkHelpers
    {
        /// <summary>
        /// Tries to convert a float to quasi-fixed point representation, max float value
        /// supported is int.MAX_VALUE / places
        /// </summary>
        /// <param name="value">The floating point value</param>
        /// <param name="places"></param>
        /// <returns></returns>
        public int ToFixedPoint(float value, int places = 1000)
        {
            return (int)(value * places);
        }

        public float FromFixedPoint(int value, int places = 1000)
        {
            return (float)value / places;
        }
    }
}
