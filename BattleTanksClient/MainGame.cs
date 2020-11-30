using BattleTanksClient.Controllers;
using BattleTanksClient.Entities;
using BattleTanksClient.Network;
using BattleTanksCommon.KenneyAssets;
using BattleTanksCommon.Network.Entities;
using BattleTanksCommon.Network.Packets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Extended.ViewportAdapters;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace BattleTanksClient
{
    public class MainGame : Game
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public TextureAtlas Atlas { get; set; }
        public OrthographicCamera Camera { get; set; }
        private ViewportAdapter _viewportAdapter;
        private TiledMapRenderer _mapRenderer;
        private NetworkClient _networkClient;
        private Entities.EntityManager _entityManager;

        public MainGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Window.AllowUserResizing = true;
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _viewportAdapter = new ScalingViewportAdapter(_graphics.GraphicsDevice, 1600, 900);
            Camera = new OrthographicCamera(_viewportAdapter);
            _spriteBatch = new SpriteBatch(_graphics.GraphicsDevice);

            _networkClient = new NetworkClient("127.0.0.1", 7000);
            _networkClient.StartClient();
            var packet = LoginPacket.CreatePacket(
                Encoding.UTF8.GetBytes("test"),
                Encoding.UTF8.GetBytes("test")
            );
            _networkClient.OnLoginResponsePacket += OnLoginResponsePacket;
            _networkClient.OnLobbyStateChangePacket += OnLobbyStateChangePacket;
            _networkClient.EnqueuePacket(packet);
            _entityManager = new Entities.EntityManager(this, _networkClient);
            base.Initialize();
        }

        protected override void EndRun()
        {
            base.EndRun();
            _networkClient.EndClient();
        }

        protected override void LoadContent()
        {
            if (File.Exists(@"Content\allSprites_default.xml"))
            {
                Atlas = TextureAtlasData.CreateFromFile(Content, @"Content\allSprites_default.xml");
            }
            
            var map = Content.Load<TiledMap>("map01");
            _mapRenderer = new TiledMapRenderer(_graphics.GraphicsDevice);
            LoadMap(map);
        }

        public void LoadMap(TiledMap currentMap)
        {
            _mapRenderer.LoadMap(currentMap);
            _entityManager.LoadMap(currentMap);
        }

        protected override void Update(GameTime gameTime)
        {
            _mapRenderer.Update(gameTime);

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();

            _entityManager.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            var viewMatrix = Camera.GetViewMatrix();
            var projectionMatrix = Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0, 0f, -1f);

            // Map
            _mapRenderer.Draw(ref viewMatrix, ref projectionMatrix);

            // Entities
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend, transformMatrix: viewMatrix);
            _entityManager.Render(_spriteBatch);
            _spriteBatch.End();

            // UI
            _spriteBatch.Begin(samplerState: SamplerState.PointWrap, transformMatrix: _viewportAdapter.GetScaleMatrix());
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// The current server assigned playerID
        /// </summary>
        public int PlayerId { get; set; }

        private void OnLoginResponsePacket(object sender, LoginResponsePacketArgs args)
        {
            var packet = args.Packet;
            Logger.Info($"Received a LoginResponse packet with response code: {packet.LoginResponseCode}");
            if (packet.LoginResponseCode == LoginResponseCode.Success)
            {
                PlayerId = packet.PlayerId;
                // Request to join a lobby
                var lobbyPacket = LobbyStateChangePacket.CreatePacket(LobbyStateCode.JoinLobbyRequest, PlayerId, 1);
                _networkClient.EnqueuePacket(lobbyPacket);
            }
        }

        private void OnLobbyStateChangePacket(object sender, LobbyStateChangePacketArgs args)
        {
            var packet = args.Packet;
            switch (packet.LobbyStateCode)
            {
                case LobbyStateCode.PlayerInLobbyQueue:
                    Logger.Info("Currently in a lobby");
                    break;
                case LobbyStateCode.LobbyStart:
                    Logger.Info("Lobby is starting");
                    break;
            }
        }
    }
}
