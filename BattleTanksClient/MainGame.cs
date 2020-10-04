using BattleTanksClient.Controllers;
using BattleTanksClient.Entities;
using BattleTanksClient.Network;
using BattleTanksCommon.Entities;
using BattleTanksCommon.KenneyAssets;
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

namespace BattleTanksClient
{
    public class MainGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private TextureAtlas _atlas;
        private Player _player;
        private MovementController _movementController;
        private OrthographicCamera _camera;
        private ViewportAdapter _viewportAdapter;
        private EntityManager _entityManager;
        private TiledMapRenderer _mapRenderer;
        private ProjectileFactory _projectileFactory;
        private NetworkClient _networkClient;

        public MainGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            _entityManager = new EntityManager();
            Content.RootDirectory = "Content";
            Window.AllowUserResizing = true;
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
            _networkClient = new NetworkClient("127.0.0.1", 7000);
            _networkClient.StartClient();
            _networkClient.OnNewPlayerPacket += OnNewPlayerPacket;
            _networkClient.OnEntitySpawnPacket += OnEntitySpawnPacket;
            // Send a login packet to server
            var packet = new LoginPacket()
            {
                MsgType = (byte)Packets.LOGIN_PACKET,
                ClientId = _networkClient.ClientId,
                PlayerId = _networkClient.PlayerId
            };
            _networkClient.EnqueuePacket(packet);
        }

        protected override void EndRun()
        {
            base.EndRun();
            _networkClient.EndClient();
        }

        protected override void LoadContent()
        {
            _viewportAdapter = new ScalingViewportAdapter(_graphics.GraphicsDevice, 1600, 900);

            _camera = new OrthographicCamera(_viewportAdapter);
            _spriteBatch = new SpriteBatch(_graphics.GraphicsDevice);

            if (File.Exists(@"Content\allSprites_default.xml"))
            {
                _atlas = TextureAtlasData.CreateFromFile(Content, @"Content\allSprites_default.xml");
                _projectileFactory = new ProjectileFactory(_entityManager, _atlas);
            }

            
            var map = Content.Load<TiledMap>("map01");
            _mapRenderer = new TiledMapRenderer(_graphics.GraphicsDevice);
            LoadMap(map);
            _player = _entityManager.AddEntity(
                    new Player(_atlas.GetRegion("tankBody_red"), _atlas.GetRegion("tankRed_barrel1"), _projectileFactory)
                );
            _entityManager.AddEntity(
                    new Player(_atlas.GetRegion("tankBody_red"), _atlas.GetRegion("tankRed_barrel1"), _projectileFactory)
                );
            _movementController = new MovementController(_player, _camera);
        }

        // Camera clamping variables
        private float _minCameraX;
        private float _maxCameraX;
        private float _minCameraY;
        private float _maxCameraY;

        public void LoadMap(TiledMap currentMap)
        {
            _mapRenderer.LoadMap(currentMap);

            // Compute our camera bounds
            _minCameraX = _camera.BoundingRectangle.Width / 2;
            _maxCameraX = currentMap.WidthInPixels - _minCameraX;
            _minCameraY = _camera.BoundingRectangle.Height / 2;
            _maxCameraY = currentMap.HeightInPixels - _minCameraY;

            _entityManager.LoadMap(currentMap);
        }

        private Vector2 _cameraTarget = Vector2.Zero;

        protected override void Update(GameTime gameTime)
        {
            _mapRenderer.Update(gameTime);

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(_movementController.ExitGame))
                Exit();

            // TODO: Add your update logic here
            _movementController.Update(gameTime);

            if (_player != null)
            {
                // We want to clamp camera coordinates so that when the player gets to < CamWidth / 2 or World.MaxX - CamWidth / 2
                // the camera doesnt change in x. Do the same for y.
                var x = _player.Position.X;
                var y = _player.Position.Y;

                _cameraTarget.X = MathHelper.Clamp(x, _minCameraX, _maxCameraX);
                _cameraTarget.Y = MathHelper.Clamp(y, _minCameraY, _maxCameraY);

                _camera.LookAt(_cameraTarget);
            }

            _entityManager.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            var viewMatrix = _camera.GetViewMatrix();
            var projectionMatrix = Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0, 0f, -1f);

            // Map
            _mapRenderer.Draw(ref viewMatrix, ref projectionMatrix);

            // Entities
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend, transformMatrix: _camera.GetViewMatrix());
            _entityManager.Draw(_spriteBatch);
            _spriteBatch.End();

            // UI
            _spriteBatch.Begin(samplerState: SamplerState.PointWrap, transformMatrix: _viewportAdapter.GetScaleMatrix());
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void OnNewPlayerPacket(object sender, NewPlayerPacket packet)
        {
            // Server broadcasts a player packet to everyone, make sure we don't double count ourselves
            if (packet.PlayerId == _networkClient.PlayerId)
                return;
            var otherPlayer = _entityManager.AddEntity(
                    new Player(_atlas.GetRegion("tankBody_red"), _atlas.GetRegion("tankRed_barrel1"), _projectileFactory)
                );
            otherPlayer.SetPosition(packet.PlayerX, packet.PlayerX);
        }

        private void OnEntitySpawnPacket(object sender, EntitySpawnPacket packet)
        {

        }
    }
}
