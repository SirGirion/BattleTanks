using MessagePack;

namespace BattleTanksCommon.Network.Packets
{
    [Union(0, typeof(LoginPacket))]
    [Union(1, typeof(NewPlayerPacket))]
    [Union(2, typeof(PlayerDeltaUpdatePacket))]
    [Union(3, typeof(EntitySpawnPacket))]
    [Union(4, typeof(LoginResponsePacket))]
    [Union(5, typeof(LobbyStateChangePacket))]
    public abstract class NetworkPacket
    {
        [Key(0)]
        public byte MsgType;
    }

    public enum Packets : byte
    {
        LoginPacket = 1,
        NewPlayerPacket = 2,
        PlayerDeltaUpdatePacket = 3,
        EntitySpawnPacket = 4,
        LoginResponsePacket = 5
    }

    public enum LoginResponseCode : byte
    {
        Success = 0,
        InvalidUserPass = 1
    }

    [MessagePackObject]
    public class LoginPacket : NetworkPacket
    {
        [Key(1)]
        public byte[] Username;
        [Key(2)]
        public byte[] Password;
    }

    [MessagePackObject]
    public class NewPlayerPacket : NetworkPacket
    {
        [Key(1)]
        public int PlayerId;
        [Key(2)]
        public int PlayerX;
        [Key(3)]
        public int PlayerY;
    }

    [MessagePackObject]
    public class PlayerDeltaUpdatePacket : NetworkPacket
    {
        [Key(1)]
        public int PlayerId;
        [Key(2)]
        public int DeltaPlayerX;
        [Key(3)]
        public int DeltaPlayerY;
        [Key(4)]
        public short DeltaPlayerRotation;
        [Key(5)]
        public int DeltaPlayerVelocityX;
        [Key(6)]
        public int DeltaPlayerVelocityY;
    }

    [MessagePackObject]
    public class EntitySpawnPacket : NetworkPacket
    {
        [Key(1)]
        public uint EntityId;
        [Key(2)]
        public int EntityX;
        [Key(3)]
        public int EntityY;
        [Key(4)]
        public short Rotation;
        [Key(5)]
        public float VelocityX;
        [Key(6)]
        public float VelocityY;
    }

    [MessagePackObject]
    public class LoginResponsePacket : NetworkPacket
    {
        [Key(1)]
        public byte LoginResponseCode;
    }

    [MessagePackObject]
    public class LobbyStateChangePacket : NetworkPacket
    {
        [Key(1)]
        public byte LobbyStateCode;
    }
}
