using BattleTanksCommon.Network.Packets;
using ENet;
using MessagePack;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BattleTanksClient.Network
{
    /// <summary>
    /// Class for handling network events as a server
    /// </summary>
    public class NetworkServer
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private Address _address;
        private Host _server;

        private bool _running;
        private BlockingCollection<NetworkPacket> _outboundPackets;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public EventHandler<LoginPacketArgs> OnLoginPacket;
        public EventHandler<NewPlayerPacketArgs> OnNewPlayerPacket;
        public EventHandler<PlayerDeltaUpdatePacketArgs> OnPlayerDeltaUpdatePacket;
        public EventHandler<EntitySpawnPacketArgs> OnEntitySpawnPacket;

        public NetworkServer(string ip, ushort port)
        {
            _address = new Address();
            _address.SetIP(ip);
            _address.Port = port;
            _outboundPackets = new BlockingCollection<NetworkPacket>(1000);
        }

        public unsafe void StartServer()
        {
            // Initialize ENet
            Library.Initialize();
            _running = true;
            _server = new Host();
            _server.Create(_address, 50);
            // Start thread for sending outbound packets
            Task.Run(() =>
            {
                while (_running && !_outboundPackets.IsCompleted)
                {
                    NetworkPacket outboundPacket = null;
                    try
                    {
                        outboundPacket = _outboundPackets.Take();
                    }
                    catch (InvalidOperationException) { }

                    if (outboundPacket != null)
                    {
                        Logger.Info("Sending packet");
                        var outboundNetworkPacket = default(Packet);
                        outboundNetworkPacket.Create(outboundPacket.ToByteArray(), PacketFlags.Reliable);
                        _server.Broadcast(0, ref outboundNetworkPacket);
                    }
                }
                Logger.Info("Ending server outbound packet sender");
            }, _cancellationTokenSource.Token);
            // Start thread for listening to incoming packets
            while (_running)
            {
                var polled = false;
                while (!polled)
                {
                    if (_server.CheckEvents(out var netEvent) <= 0)
                    {
                        if (_server.Service(15, out netEvent) <= 0)
                            break;
                        polled = true;
                    }

                    switch (netEvent.Type)
                    {
                        case EventType.None:
                            break;

                        case EventType.Connect:
                            Logger.Info("Client connected to ID: " + netEvent.Peer.ID);
                            //SendLogin();
                            break;

                        case EventType.Disconnect:
                            Logger.Info("Client disconnected server");
                            break;

                        case EventType.Timeout:
                            Logger.Info("Client connection timeout");
                            break;

                        case EventType.Receive:
                            Logger.Info("Packet received - Channel ID: " + netEvent.ChannelID + ", Data length: " + netEvent.Packet.Length);
                            // Convert packet data back to a INetworkPacket, notify subscribers
                            Parse(netEvent);
                            netEvent.Packet.Dispose();
                            break;
                    }
                }
            }
            Logger.Info("Ending server incoming packet listener");
            _server.Flush();

        }

        public void EndServer()
        {
            _running = false;
            _cancellationTokenSource.Cancel();
            _server.Dispose();
            Library.Deinitialize();
        }

        /// <summary>
        /// Tries to enqueue the packet. This doesn't necessarily immediately send the packet
        /// however it should be non-blocking.
        /// </summary>
        /// <param name="packet">The packet to send.</param>
        /// <returns>True if the packet was enqueued.</returns>
        public bool EnqueuePacket(NetworkPacket packet)
        {
            Logger.Info("Enqueuing packet");
            return _outboundPackets.TryAdd(packet);
        }

        public bool SendResponsePacket(Peer peer, NetworkPacket networkPacket)
        {
            var packet = default(Packet);
            packet.Create(networkPacket.ToByteArray());
            return peer.Send(0, ref packet);
        }

        private unsafe void Parse(Event netEvent)
        {
            var buffer = new byte[netEvent.Packet.Length];
            netEvent.Packet.CopyTo(buffer);
            var packet = MessagePackSerializer.Deserialize<NetworkPacket>(buffer);
            switch (packet)
            {
                case PlayerDeltaUpdatePacket data:
                    OnPlayerDeltaUpdatePacket?.Invoke(this, new PlayerDeltaUpdatePacketArgs(netEvent, data));
                    break;
                case EntitySpawnPacket data:
                    OnEntitySpawnPacket?.Invoke(this, new EntitySpawnPacketArgs(netEvent, data));
                    break;
                case NewPlayerPacket data:
                    OnNewPlayerPacket?.Invoke(this, new NewPlayerPacketArgs(netEvent, data));
                    break;
                case LoginPacket data:
                    OnLoginPacket?.Invoke(this, new LoginPacketArgs(netEvent, data));
                    break;
                default:
                    Logger.Info($"Unknown msgType encountered: {packet.MsgType}");
                    break;
            }
        }
    }
}
