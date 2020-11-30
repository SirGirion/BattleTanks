using BattleTanksClient.Network;
using BattleTanksCommon.Network.Entities;
using BattleTanksCommon.Network.Packets;
using BattleTanksServer.Lobby;
using BattleTanksServer.Utils;
using ENet;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace BattleTanksServer
{
    public class DummyGraphicsDeviceService : IGraphicsDeviceService
    {
        public GraphicsDevice GraphicsDevice { get; }

        public event EventHandler<EventArgs> DeviceCreated;
        public event EventHandler<EventArgs> DeviceDisposing;
        public event EventHandler<EventArgs> DeviceReset;
        public event EventHandler<EventArgs> DeviceResetting;
    }

    public class Server : Game
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int d_sdl_setenv(string name, string value, int overwrite);
        public static d_sdl_setenv SDL_setenv;

        internal NetworkServer NetworkServer { get; }
        private EntityManager _manager;
        private LobbyQueue _lobbyQueue;
        private Random _rand;

        /// <summary>
        /// Creates a new server listening for traffic on a specific host/port pair.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        public Server(string host, ushort port)
        {
            SDL_setenv = FuncLoader.LoadFunction<d_sdl_setenv>(Sdl.NativeLibrary, nameof(SDL_setenv));
            SDL_setenv("SDL_VIDEODRIVER", "dummy", overwrite: 0);
            Services.AddService(typeof(IGraphicsDeviceService), new DummyGraphicsDeviceService());
            Library.Initialize();
            NetworkServer = new NetworkServer(host, port);
            NetworkServer.OnLoginPacket += OnLoginPacket;
            _manager = new EntityManager();
            _lobbyQueue = new LobbyQueue(this);
            _rand = new Random();
        }

        protected override void Initialize()
        {

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
                var playerId = AssignPlayerId(loginPacketArgs.NetEvent.Peer);
                // Send a LoginResponse packet to the client
                var response = LoginResponsePacket.CreatePacket(LoginResponseCode.Success, playerId);
                NetworkServer.SendResponsePacket(loginPacketArgs.NetEvent.Peer, response);
            }
            else
            {
                // Send a LoginResponse packet to the client
                var response = LoginResponsePacket.CreatePacket(LoginResponseCode.InvalidUserPass, -1);
                NetworkServer.SendResponsePacket(loginPacketArgs.NetEvent.Peer, response);
            }
        }

        private bool IsValidLogin(string username, string password)
        {
            return true;
        }

        /// <summary>
        /// Assigns a player their ID for the current session.
        /// </summary>
        /// <returns></returns>
        private int AssignPlayerId(Peer peer)
        {
            return (int)peer.ID;
        }
    }
}
