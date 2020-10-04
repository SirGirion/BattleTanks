using BattleTanksCommon.Network.Packets;
using ENet;
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
        private Address _address;
        private Host _client;
        private Peer _peer;

        private bool _running;
        private BlockingCollection<INetworkPacket> _outboundPackets;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public EventHandler<LoginPacket> OnLoginPacket;
        public EventHandler<NewPlayerPacket> OnNewPlayerPacket;
        public EventHandler<PlayerDeltaUpdatePacket> OnPlayerDeltaUpdatePacket;
        public EventHandler<EntitySpawnPacket> OnEntitySpawnPacket;

        public int ClientId { get; }
        public int PlayerId { get; }

        public NetworkClient(string ip, ushort port)
        {
            _address = new Address();
            _address.SetIP(ip);
            _address.Port = port;
            _outboundPackets = new BlockingCollection<INetworkPacket>(1000);
            var rand = new Random();
            ClientId = rand.Next();
            PlayerId = rand.Next();
        }

        public unsafe void StartClient()
        {
            // Initialize ENet
            Library.Initialize();
            _running = true;
            _client = new Host();
            _client.Create();
            _peer = _client.Connect(_address);
            // Start thread for listening to incoming packets
            Task.Run(() =>
            {
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
                                Debug.WriteLine("Client connected to ID: " + _peer.ID);
                                //SendLogin();
                                break;

                            case EventType.Disconnect:
                                Debug.WriteLine("Client disconnected server");
                                break;

                            case EventType.Timeout:
                                Debug.WriteLine("Client connection timeout");
                                break;

                            case EventType.Receive:
                                Debug.WriteLine("Packet received - Channel ID: " + netEvent.ChannelID + ", Data length: " + netEvent.Packet.Length);
                                // Convert packet data back to a INetworkPacket, notify subscribers
                                Parse((byte*)netEvent.Packet.Data.ToPointer());
                                netEvent.Packet.Dispose();
                                break;
                        }
                    }
                }
                _client?.Flush();
            });
            // Start thread for sending outbound packets
            Task.Run(() =>
            {
                while (_running && !_outboundPackets.IsCompleted)
                {
                    INetworkPacket outboundPacket = null;
                    try
                    {
                        outboundPacket = _outboundPackets.Take();
                    }
                    catch (InvalidOperationException) { }

                    if (outboundPacket != null)
                    {
                        Debug.WriteLine("Sending packet");
                        var outboundNetworkPacket = default(Packet);
                        outboundNetworkPacket.Create(outboundPacket.ToByteArray(), PacketFlags.Reliable);
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
        public bool EnqueuePacket(INetworkPacket packet)
        {
            Debug.WriteLine("Enqueuing packet");
            return _outboundPackets.TryAdd(packet);
        }

        //protected virtual void OnPacketRecevied
        private unsafe void Parse(byte* data)
        {
            byte msgType = *data;
            if (msgType == (byte)Packets.PLAYER_DELTA_UPDATE_PACKET)
            {
                OnPlayerDeltaUpdatePacket?.Invoke(this, *(PlayerDeltaUpdatePacket*)data);
            }
            else if (msgType == (byte)Packets.ENTITY_SPAWN_PACKET)
            {
                OnEntitySpawnPacket?.Invoke(this, *(EntitySpawnPacket*)data);
            }
            else if (msgType == (byte)Packets.NEW_PLAYER_PACKET)
            {
                OnNewPlayerPacket?.Invoke(this, *(NewPlayerPacket*)data);
            }
            else if (msgType == (byte)Packets.LOGIN_PACKET)
            {
                OnLoginPacket?.Invoke(this, *(LoginPacket*)data);
            }
            else
            {

            }
        }
    }
}
