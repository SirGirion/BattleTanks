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

    public class PlayerDeltaUpdatePacketArgs : EventArgs
    {
        public PlayerDeltaUpdatePacketArgs(Event netEvent, PlayerDeltaUpdatePacket packet)
        {
            NetEvent = netEvent;
            Packet = packet;
        }

        public Event NetEvent { get; }
        public PlayerDeltaUpdatePacket Packet { get; }
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
}
