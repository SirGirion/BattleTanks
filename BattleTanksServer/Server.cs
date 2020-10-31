using BattleTanksClient.Network;
using BattleTanksCommon.Entities;
using BattleTanksCommon.Network.Packets;
using BattleTanksServer.Lobby;
using ENet;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace BattleTanksServer
{
    public class Server : Game
    {
        internal NetworkServer NetworkServer { get; }
        private EntityManager _manager;
        private LobbyQueue _lobbyQueue;

        /// <summary>
        /// Creates a new server listening for traffic on a specific host/port pair.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        public Server(string host, ushort port)
        {
            Library.Initialize();
            NetworkServer = new NetworkServer(host, port);
            NetworkServer.OnLoginPacket += OnLoginPacket;
            _manager = new EntityManager();
            _lobbyQueue = new LobbyQueue(this);
        }

        public void StartServer()
        {
            NetworkServer.StartServer();
        }

        public void EndServer()
        {
            NetworkServer.EndServer();
        }

        /// <summary>
        /// Updates the server gamestate.
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Update(GameTime gameTime)
        {
            _lobbyQueue.Update(gameTime);
        }

        private List<int> _players = new List<int>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="packet"></param>
        private void OnLoginPacket(object sender, LoginPacketArgs loginPacketArgs)
        {
            var packet = loginPacketArgs.Packet;
            var username = Encoding.UTF8.GetString(packet.Username);
            var password = Encoding.UTF8.GetString(packet.Password);
            if (IsValidLogin(username, password))
            {
                // Send a LoginResponse packet to the client, broadcast a NewPlayer packet to the server
                var response = new LoginResponsePacket
                {
                    MsgType = (byte)Packets.LoginResponsePacket,
                    LoginResponseCode = (byte)LoginResponseCode.Success
                };
                NetworkServer.SendResponsePacket(loginPacketArgs.NetEvent.Peer, response);
            }
        }

        private bool IsValidLogin(string username, string password)
        {
            return true;
        }
    }
}
