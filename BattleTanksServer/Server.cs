using BattleTanksClient.Network;
using BattleTanksCommon.Entities;
using BattleTanksCommon.Network.Packets;
using ENet;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace BattleTanksServer
{
    public class Server : Game
    {
        private EntityManager _manager;

        /// <summary>
        /// Creates a new server listening for traffic on a specific host/port pair.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        public Server(string host, ushort port)
        {
            Library.Initialize();
            _server = new NetworkServer(host, port);
            _server.OnLoginPacket += OnLoginPacket;
            _manager = new EntityManager();
        }

        public void StartServer()
        {
            _server.StartServer();
        }

        public void EndServer()
        {
            _server.EndServer();
        }

        private NetworkServer _server;

        /// <summary>
        /// Updates the server gamestate.
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Update(GameTime gameTime)
        {

        }

        private List<int> _players = new List<int>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="packet"></param>
        private void OnLoginPacket(object sender, LoginPacket packet)
        {
            // Create a new player entity to track
            var id = packet.PlayerId;
            _players.Add(id);
            foreach (var player in _players)
                _server.EnqueuePacket(new NewPlayerPacket
                {
                    MsgType = (byte)Packets.NEW_PLAYER_PACKET,
                    PlayerId = player,
                    PlayerX = 100,
                    PlayerY = 100
                });
        }
    }
}
