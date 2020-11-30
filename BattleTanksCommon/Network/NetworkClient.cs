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
    /// Class for handling network events as a client
    /// </summary>
    public class NetworkClient
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private Address _address;
        private Host _client;
        private Peer _peer;

        private bool _running;
        private BlockingCollection<NetworkPacket> _outboundPackets;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public EventHandler<LoginPacketArgs> OnLoginPacket;
        public EventHandler<NewPlayerPacketArgs> OnNewPlayerPacket;
        public EventHandler<PlayerUpdatePacketArgs> OnPlayerUpdatePacket;
        public EventHandler<EntitySpawnPacketArgs> OnEntitySpawnPacket;
        public EventHandler<LoginResponsePacketArgs> OnLoginResponsePacket;
        public EventHandler<LobbyStateChangePacketArgs> OnLobbyStateChangePacket;
        public EventHandler<EntityRemovedPacketArgs> OnEntityRemovedPacket;

        public NetworkClient(string ip, ushort port)
        {
            _address = new Address();
            _address.SetIP(ip);
            _address.Port = port;
            _outboundPackets = new BlockingCollection<NetworkPacket>(1000);
        }

        public unsafe void StartClient()
        {
            // Initialize ENet
            Library.Initialize();
            _running = true;
            // Start thread for listening to incoming packets
            Task.Run(() =>
            {
                _client = new Host();
                _client.Create();
                _peer = _client.Connect(_address);
                while (_running)
                {
                    var polled = false;
                    while (!polled)
                    {
                        if (_client.CheckEvents(out var netEvent) <= 0)
                        {
                            if (_client.Service(15, out netEvent) <= 0)
                                break;
                            polled = true;
                        }

                        switch (netEvent.Type)
                        {
                            case EventType.None:
                                break;

                            case EventType.Connect:
                                Logger.Info("Client connected to ID: " + _peer.ID);
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
                try
                {
                    _client?.Flush();
                }
                catch (InvalidOperationException)
                {
                    Logger.Warn("Client disconnected too soon.");
                }
            });
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
                        Debug.WriteLine("Sending packet");
                        var outboundNetworkPacket = default(Packet);
                        outboundNetworkPacket.Create(outboundPacket.ToByteArray(), PacketFlags.Unsequenced);
                        _peer.Send(0, ref outboundNetworkPacket);
                    }
                }
            }, _cancellationTokenSource.Token);
        }

        public void EndClient()
        {
            _running = false;
            _cancellationTokenSource.Cancel();
            _peer.DisconnectNow(0);
            _client?.Dispose();
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
            Debug.WriteLine("Enqueuing packet");
            return _outboundPackets.TryAdd(packet);
        }

        //protected virtual void OnPacketRecevied
        private unsafe void Parse(Event netEvent)
        {
            var buffer = new byte[netEvent.Packet.Length];
            netEvent.Packet.CopyTo(buffer);
            var packet = MessagePackSerializer.Deserialize<NetworkPacket>(buffer);
            switch (packet)
            {
                case PlayerUpdatePacket data:
                    OnPlayerUpdatePacket?.Invoke(this, new PlayerUpdatePacketArgs(netEvent, data));
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
                case LoginResponsePacket data:
                    OnLoginResponsePacket?.Invoke(this, new LoginResponsePacketArgs(netEvent, data));
                    break;
                case LobbyStateChangePacket data:
                    OnLobbyStateChangePacket?.Invoke(this, new LobbyStateChangePacketArgs(netEvent, data));
                    break;
                case EntityRemovedPacket data:
                    OnEntityRemovedPacket?.Invoke(this, new EntityRemovedPacketArgs(netEvent, data));
                    break;
                default:
                    Logger.Info($"Unknown msgType encountered: {packet.MsgType}");
                    break;
            }
        }
    }
}
