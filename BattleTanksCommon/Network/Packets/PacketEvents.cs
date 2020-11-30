using ENet;
using System;
using System.Collections.Generic;
using System.Text;

namespace BattleTanksCommon.Network.Packets
{
    public class LoginPacketArgs : EventArgs
    {
        public LoginPacketArgs(Event netEvent, LoginPacket packet)
        {
            NetEvent = netEvent;
            Packet = packet;
        }

        public Event NetEvent { get; }
        public LoginPacket Packet { get; }
    }

    public class NewPlayerPacketArgs : EventArgs
    {
        public NewPlayerPacketArgs(Event netEvent, NewPlayerPacket packet)
        {
            NetEvent = netEvent;
            Packet = packet;
        }

        public Event NetEvent { get; }
        public NewPlayerPacket Packet { get; }
    }

    public class PlayerUpdatePacketArgs : EventArgs
    {
        public PlayerUpdatePacketArgs(Event netEvent, PlayerUpdatePacket packet)
        {
            NetEvent = netEvent;
            Packet = packet;
        }

        public Event NetEvent { get; }
        public PlayerUpdatePacket Packet { get; }
    }

    public class EntitySpawnPacketArgs : EventArgs
    {
        public EntitySpawnPacketArgs(Event netEvent, EntitySpawnPacket packet)
        {
            NetEvent = netEvent;
            Packet = packet;
        }

        public Event NetEvent { get; }
        public EntitySpawnPacket Packet { get; }
    }

    public class LoginResponsePacketArgs : EventArgs
    {
        public LoginResponsePacketArgs(Event netEvent, LoginResponsePacket packet)
        {
            NetEvent = netEvent;
            Packet = packet;
        }

        public Event NetEvent { get; }
        public LoginResponsePacket Packet { get; }
    }

    public class LobbyStateChangePacketArgs : EventArgs
    {
        public LobbyStateChangePacketArgs(Event netEvent, LobbyStateChangePacket packet)
        {
            NetEvent = netEvent;
            Packet = packet;
        }

        public Event NetEvent { get; }
        public LobbyStateChangePacket Packet { get; }
    }

    public class KeyPressPacketArgs : EventArgs
    {
        public KeyPressPacketArgs(Event netEvent, KeyPressPacket packet)
        {
            NetEvent = netEvent;
            Packet = packet;
        }

        public Event NetEvent { get; }
        public KeyPressPacket Packet { get; }
    }

    public class MouseStatePacketArgs : EventArgs
    {
        public MouseStatePacketArgs(Event netEvent, MouseStatePacket packet)
        {
            NetEvent = netEvent;
            Packet = packet;
        }

        public Event NetEvent { get; }
        public MouseStatePacket Packet { get; }
    }

    public class EntityRemovedPacketArgs : EventArgs
    {
        public EntityRemovedPacketArgs(Event netEvent, EntityRemovedPacket packet)
        {
            NetEvent = netEvent;
            Packet = packet;
        }

        public Event NetEvent { get; }
        public EntityRemovedPacket Packet { get; }
    }
}
