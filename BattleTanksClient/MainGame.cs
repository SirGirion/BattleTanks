using BattleTanksClient.CommonExtensions;
using BattleTanksClient.Controllers;
using BattleTanksCommon.Entities;
using BattleTanksCommon.KenneyAssets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.TextureAtlases;
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
        private MovementController _movementController;

        public MainGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            var mouseListener = new MouseListener(new MouseListenerSettings());
            var keyboardListener = new KeyboardListener(new KeyboardListenerSettings());
            Components.Add(new InputListenerComponent(this, mouseListener, keyboardListener));
            _movementController = new MovementController(mouseListener, keyboardListener);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            _testTexture = Content.Load<Texture2D>("allSprites_retina");
            if (File.Exists(@"Content\allSprites_default.xml"))
            {
                _atlas = TextureAtlasData.CreateFromFile(Content, @"Content\allSprites_default.xml");
                _player = new Player("tank_red");
                _player.SetMainSprite(_atlas);
                _movementController.RegisterPlayer(_player);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            //_spriteBatch.Draw(_testTexture, new Vector2(0, 0), Color.White);
            _player.Draw(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
