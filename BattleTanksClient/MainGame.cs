using BattleTanksClient.CommonExtensions;
using BattleTanksClient.Controllers;
using BattleTanksCommon.Entities;
using BattleTanksCommon.KenneyAssets;
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
        private Texture2D _testTexture;
        private TextureAtlas _atlas;
        private Player _player;
        //private MovementController _movementController;
        private OrthographicCamera _camera;
        private MouseState _previousMouseState;
        private ViewportAdapter _viewportAdapter;
        private IEntityManager _entityManager;
        private TiledMapRenderer _mapRenderer;

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
        }

        protected override void LoadContent()
        {
            _viewportAdapter = new ScalingViewportAdapter(GraphicsDevice, 800, 450);

            _camera = new OrthographicCamera(_viewportAdapter);
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            if (File.Exists(@"Content\allSprites_default.xml"))
            {
                _atlas = TextureAtlasData.CreateFromFile(Content, @"Content\allSprites_default.xml");
                _player = _entityManager.AddEntity(new Player(_atlas.GetRegion("tankBody_red"), _atlas.GetRegion("tankRed_barrel1")));
            }
            _mapRenderer = new TiledMapRenderer(GraphicsDevice, Content.Load<TiledMap>("map01"));
        }

        protected override void Update(GameTime gameTime)
        {
            _mapRenderer.Update(gameTime);

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            var deltaTime = gameTime.GetElapsedSeconds();
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            if (_player != null && !_player.IsDestroyed)
            {
                const float acceleration = 5f;

                if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up))
                    _player.Move(-1);

                if (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down))
                    _player.Move(1);

                if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left))
                    _player.Rotation -= deltaTime * 3f;

                if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right))
                    _player.Rotation += deltaTime * 3f;

                //if (keyboardState.IsKeyDown(Keys.Space) || mouseState.LeftButton == ButtonState.Pressed)
                //    _player.Fire();

                if (_previousMouseState.X != mouseState.X || _previousMouseState.Y != mouseState.Y)
                    _player.LookAt(_camera.ScreenToWorld(new Vector2(mouseState.X, mouseState.Y)));

                _camera.LookAt(_player.Position + _player.Velocity * 0.2f);
                //_camera.Zoom = 1.0f - _player.Velocity.Length() / 500f;
            }

            _entityManager.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            var viewMatrix = _camera.GetViewMatrix();
            var projectionMatrix = Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0, 0f, -1f);

            _mapRenderer.Draw(ref viewMatrix, ref projectionMatrix);

            // UI
            _spriteBatch.Begin(samplerState: SamplerState.PointWrap, transformMatrix: _viewportAdapter.GetScaleMatrix());
            _spriteBatch.End();

            // entities
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend, transformMatrix: _camera.GetViewMatrix());
            _entityManager.Draw(_spriteBatch);
            _spriteBatch.End();

            _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix());

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
