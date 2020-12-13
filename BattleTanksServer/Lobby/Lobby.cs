using BattleTanksCommon.Network.Entities;
using BattleTanksCommon.Network.Packets;
using BattleTanksServer.Game.Entities;
using ENet;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleTanksServer.Lobby
{
    public class Lobby : ILobby
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public bool IsFinished { get; set; }

        public int PlayerCount => _players.Count;

        public int LobbyId { get; }

        /// <summary>
        /// Server instance this lobby is running on.
        /// </summary>
        internal Server Server { get; }
        private Dictionary<int, Peer> _players;
        public IReadOnlyCollection<Peer> PlayerConnections => _players.Values;

        private IEntityManager _entityManager;

        public Lobby(Server server, int lobbyId)
        {
            Server = server;
            _players = new Dictionary<int, Peer>();
            Server.NetworkServer.OnDisconnect += OnDisconnect;
            Server.NetworkServer.OnTimeout += OnTimeout;
            Server.NetworkServer.OnKeyPressPacket += OnKeyPressPacket;
            Server.NetworkServer.OnMouseStatePacket += OnMouseStatePacket;

            _entityManager = new NetworkEntityManager(this);
            _entityManager.LoadMap(null);
            LobbyId = lobbyId;
        }

        private void OnDisconnect(object sender, Peer peer)
        {
            // Find player that disconnected
            var idToRemove = -1;
            foreach (var (id, conn) in _players)
            {
                if (conn.ID == peer.ID)
                    idToRemove = id;
            }
            if (idToRemove != -1)
                RemovePlayer(idToRemove);
        }

        private void OnTimeout(object sender, Peer peer)
        {

        }

        public void AddPlayer(Tuple<Peer, int> player)
        {
            Logger.Info($"Lobby({LobbyId}): Adding player ID({player.Item2}");
            _players[player.Item2] = player.Item1;
            _entityManager.AddEntity(new Player(player.Item2, new Random().Next(0, 400), new Random().Next(0, 400), (byte)new Random().Next(0, 3)));
        }

        public void EndLobby()
        {
            Server.NetworkServer.OnDisconnect -= OnDisconnect;
            Server.NetworkServer.OnTimeout -= OnTimeout;
            Server.NetworkServer.OnKeyPressPacket -= OnKeyPressPacket;
            Server.NetworkServer.OnMouseStatePacket -= OnMouseStatePacket;
            IsFinished = true;
        }

        public void RemovePlayer(int playerId)
        {
            _players.Remove(playerId);
            _entityManager.RemoveEntity(playerId);
            if (PlayerCount == 0)
                EndLobby();
        }

        public void StartLobby()
        {
            foreach (var player in _entityManager.GetEntitiesOfType<Player>())
            {
                var newPlayer = NewPlayerPacket.CreatePacket(player.Id, (int)player.Position.X, (int)player.Position.Y, player.Color);
                Server.NetworkServer.ScopedBroadcast(_players.Values, newPlayer);
            }
            foreach (var (playerId, playerConnection) in _players)
            {
                // Send a LobbyStart
                var lobbyStart = LobbyStateChangePacket.CreatePacket(LobbyStateCode.LobbyStart, playerId, -1);
                Server.NetworkServer.SendResponsePacket(playerConnection, lobbyStart);
            }
        }

        public void Update(GameTime gameTime)
        {
            // Update the game state
            _entityManager.Update(gameTime);
            // Send updates
            foreach (var player in _entityManager.GetEntitiesOfType<Player>(p => p.Updated).ToList())
            {
                var updatePacket = PlayerUpdatePacket.CreatePacket(player.Id, player.Position.X, player.Position.Y, player.BarrelPosition.X, player.BarrelPosition.Y, player.Rotation, player.BarrelRotation, player.Velocity.X, player.Velocity.Y);
                Server.NetworkServer.ScopedBroadcast(_players.Values, updatePacket);
            }
        }

        private void OnKeyPressPacket(object sender, KeyPressPacketArgs args)
        {
            // First check if we even care about this playerID
            var packet = args.Packet;
            if (!_players.ContainsKey(packet.PlayerId))
                return;
            var player = _entityManager.GetEntitiesOfType<Player>(p => p.Id == packet.PlayerId).First();
            var keyFlag = (Key)packet.KeyFlags;
            if (keyFlag.IsForward())
                player.Accelerate(-5f);
            if (keyFlag.IsBackward())
                player.Accelerate(5f);
            if (keyFlag.IsLeft())
                player.Rotate(-1);
            if (keyFlag.IsRight())
                player.Rotate(1);
            if (keyFlag.IsNoRotation())
                player.Rotate(0);
            if (keyFlag.IsFire())
                player.Fire();
        }

        private void OnMouseStatePacket(object sender, MouseStatePacketArgs args)
        {
            // First check if we even care about this playerID
            var packet = args.Packet;
            var player = _entityManager.GetEntitiesOfType<Player>(p => p.Id == packet.PlayerId).First();
            if (player == null)
                return;
            player.LookAt(new Vector2(packet.X, packet.Y));
        }
    }
}
