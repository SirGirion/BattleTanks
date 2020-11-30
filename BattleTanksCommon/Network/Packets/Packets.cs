using MessagePack;

namespace BattleTanksCommon.Network.Packets
{
    [Union(1, typeof(LoginPacket))]
    [Union(2, typeof(NewPlayerPacket))]
    [Union(3, typeof(PlayerUpdatePacket))]
    [Union(4, typeof(EntitySpawnPacket))]
    [Union(5, typeof(LoginResponsePacket))]
    [Union(6, typeof(LobbyStateChangePacket))]
    [Union(7, typeof(KeyPressPacket))]
    [Union(8, typeof(MouseStatePacket))]
    [Union(9, typeof(EntityRemovedPacket))]
    public abstract class NetworkPacket
    {
        [Key(0)]
        public Packets MsgType;
    }

    public enum Packets : byte
    {
        LoginPacket = 1,
        NewPlayerPacket = 2,
        PlayerDeltaUpdatePacket = 3,
        EntitySpawnPacket = 4,
        LoginResponsePacket = 5,
        LobbyStateChangePacket = 6,
        KeyPressPacket = 7,
        MouseStatePacket = 8,
        EntityRemovedPacket = 9
    }

    public enum LoginResponseCode : byte
    {
        Success = 0,
        InvalidUserPass = 1
    }

    public enum LobbyStateCode : byte
    {
        JoinLobbyRequest = 0, // Player is trying to join a lobby
        PlayerInLobbyQueue = 1, // Player is in a lobby
        LobbyStart = 2, // Lobby has started
        LobbyEnd = 3, // Lobby has ended
        PlayerLeaveLobby = 4, // Player has left lobby
        PlayerJoinLobby = 5
    }

    [MessagePackObject]
    public class LoginPacket : NetworkPacket
    {
        [Key(1)]
        public byte[] Username;
        [Key(2)]
        public byte[] Password;

        public static LoginPacket CreatePacket(byte[] username, byte[] password) => new LoginPacket
        {
            MsgType = Packets.LoginPacket,
            Username = username,
            Password = password
        };

        public override string ToString()
        {
            return $"MsgType: {MsgType}, Username: {Username}, Password(len): {Password.Length}";
        }
    }

    [MessagePackObject]
    public class NewPlayerPacket : NetworkPacket
    {
        [Key(1)]
        public int PlayerId;
        [Key(2)]
        public int X;
        [Key(3)]
        public int Y;
        [Key(4)]
        public byte Color;

        public static NewPlayerPacket CreatePacket(int playerId, int x, int y, byte color) => new NewPlayerPacket
        {
            MsgType = Packets.NewPlayerPacket,
            PlayerId = playerId,
            X = x,
            Y = y,
            Color = color
        };

        public override string ToString()
        {
            return $"MsgType: {MsgType}, PlayerId: {PlayerId}, X: {X}, Y: {Y}, Color: {Color}";
        }
    }

    [MessagePackObject]
    public class PlayerUpdatePacket : NetworkPacket
    {
        [Key(1)]
        public int PlayerId;
        [Key(2)]
        public float X;
        [Key(3)]
        public float Y;
        [Key(4)]
        public float BarrelX;
        [Key(5)]
        public float BarrelY;
        [Key(6)]
        public float Rotation;
        [Key(7)]
        public float BarrelRotation;
        [Key(8)]
        public float VelocityX;
        [Key(9)]
        public float VelocityY;

        public static PlayerUpdatePacket CreatePacket(int playerId, float x, float y, float barrelX, float barrelY, float rotation, float barrelRotation, float velocityX, float velocityY) => new PlayerUpdatePacket
        {
            MsgType = Packets.PlayerDeltaUpdatePacket,
            PlayerId = playerId,
            X = x,
            Y = y,
            BarrelX = barrelX,
            BarrelY = barrelY,
            Rotation = rotation,
            BarrelRotation = barrelRotation,
            VelocityX = velocityX,
            VelocityY = velocityY
        };

        public override string ToString()
        {
            return $"MsgType: {MsgType}, PlayerId: {PlayerId}, X: {X}, Y: {Y}, BarrelX: {BarrelX}, BarrelY: {BarrelY}, Rotation: {Rotation}, BarrelRotation: {BarrelRotation}, VelocityX: {VelocityX}, VelocityY: {VelocityY}";
        }
    }

    [MessagePackObject]
    public class EntitySpawnPacket : NetworkPacket
    {
        [Key(1)]
        public int EntityId;
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
        public LoginResponseCode LoginResponseCode;
        [Key(2)]
        public int PlayerId;

        public static LoginResponsePacket CreatePacket(LoginResponseCode loginResponseCode, int playerId) => new LoginResponsePacket
        {
            MsgType = Packets.LoginResponsePacket,
            LoginResponseCode = loginResponseCode,
            PlayerId = playerId
        };
    }

    [MessagePackObject]
    public class LobbyStateChangePacket : NetworkPacket
    {
        [Key(1)]
        public LobbyStateCode LobbyStateCode;
        [Key(2)]
        public int PlayerId;
        [Key(3)]
        public int Data;

        public static LobbyStateChangePacket CreatePacket(LobbyStateCode lobbyStateCode, int playerId, int data) => new LobbyStateChangePacket
        {
            MsgType = Packets.LobbyStateChangePacket,
            LobbyStateCode = lobbyStateCode,
            PlayerId = playerId,
            Data = data
        };
    }

    [MessagePackObject]
    public class KeyPressPacket : NetworkPacket
    {
        [Key(1)]
        public int PlayerId;
        /// <summary>
        /// Key(s) that were pressed, corresponds to the following IDs
        /// 1 -> ForwardInput
        /// 2 -> BackwardInput
        /// 4 -> LeftInput
        /// 8 -> RightInput
        /// 16 -> FireInput
        /// </summary>
        [Key(2)]
        public byte KeyFlags;

        public static KeyPressPacket CreatePacket(int playerId, byte keyFlags) => new KeyPressPacket
        {
            MsgType = Packets.KeyPressPacket,
            PlayerId = playerId,
            KeyFlags = keyFlags
        };
    }


    [MessagePackObject]
    public class MouseStatePacket : NetworkPacket
    {
        [Key(1)]
        public int PlayerId;
        [Key(2)]
        public float X;
        [Key(3)]
        public float Y;

        public static MouseStatePacket CreatePacket(int playerId, float x, float y) => new MouseStatePacket
        {
            MsgType = Packets.MouseStatePacket,
            PlayerId = playerId,
            X = x,
            Y = y
        };
    }

    [MessagePackObject]
    public class EntityRemovedPacket : NetworkPacket
    {
        [Key(1)]
        public int EntityId;

        public static EntityRemovedPacket CreatePacket(int entityId) => new EntityRemovedPacket
        {
            MsgType = Packets.EntityRemovedPacket,
            EntityId = entityId
        };
    }
}
