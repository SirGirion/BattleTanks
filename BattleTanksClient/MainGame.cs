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

        public MainGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Window.AllowUserResizing = true;
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            var mouseListener = new MouseListener(new MouseListenerSettings());
            var keyboardListener = new KeyboardListener(new KeyboardListenerSettings());
            Components.Add(new InputListenerComponent(this, mouseListener, keyboardListener));
            //_movementController = new MovementController(mouseListener, keyboardListener);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);

            _camera = new OrthographicCamera(_viewportAdapter);
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            _testTexture = Content.Load<Texture2D>("allSprites_retina");
            if (File.Exists(@"Content\allSprites_default.xml"))
            {
                _atlas = TextureAtlasData.CreateFromFile(Content, @"Content\allSprites_default.xml");
                _player = new Player(_atlas.GetRegion("tankBody_red"), _atlas.GetRegion("tankRed_barrel1"));
                //_player.SetMainSprite(_atlas);
                //_movementController.RegisterPlayer(_player);
            }
        }

        protected override void Update(GameTime gameTime)
        {
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
                    _player.Accelerate(acceleration);

                if (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down))
                    _player.Accelerate(-acceleration);

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

            _player.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend, transformMatrix: _camera.GetViewMatrix());
            _player.Draw(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
