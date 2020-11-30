using BattleTanksClient.Entities;
using BattleTanksClient.Network;
using BattleTanksCommon.Network.Entities;
using BattleTanksCommon.Network.Packets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;

namespace BattleTanksClient.Controllers
{
    /// <summary>
    /// Controller responsible for handling user input for movement. This class
    /// is also responsible for notifying the game of any movement updates to be
    /// sent out through the network.
    /// </summary>
    public class MovementController
    {
        /// <summary>
        /// Player this controller will move.
        /// </summary>
        private RenderablePlayer _player;
        /// <summary>
        /// Camera used for translating screen coordinates to world coordinates.
        /// </summary>
        private readonly OrthographicCamera _camera;
        private readonly NetworkClient _client;

        private MouseState _previousMouseState;

        public enum MouseButtons
        {
            LeftButton,
            MiddleButton,
            RightButton,
            XButton1,
            XButton2
        }

        public Keys ForwardInput { get; set; }
        public Keys BackwardInput { get; set; }
        public Keys LeftInput { get; set; }
        public Keys RightInput { get; set; }
        public Keys UsePowerUp { get; set; }
        public Keys ExitGame { get; set; }
        public MouseButtons FireInputMouse { get; set; }
        public Keys FireInputKeyboard { get; set; }

        // Camera variables
        private float _minCameraX;
        private float _maxCameraX;
        private float _minCameraY;
        private float _maxCameraY;

        private Vector2 _cameraTarget = Vector2.Zero;

        public MovementController(OrthographicCamera camera, NetworkClient client)
        {
            _camera = camera;
            _client = client;
            ForwardInput = Keys.W;
            BackwardInput = Keys.S;
            LeftInput = Keys.A;
            RightInput = Keys.D;
            UsePowerUp = Keys.E;
            ExitGame = Keys.Escape;
            FireInputMouse = MouseButtons.LeftButton;
            FireInputKeyboard = Keys.None;
        }

        public void LoadMap(TiledMap currentMap)
        {
            // Compute our camera bounds
            _minCameraX = _camera.BoundingRectangle.Width / 2;
            _maxCameraX = currentMap.WidthInPixels - _minCameraX;
            _minCameraY = _camera.BoundingRectangle.Height / 2;
            _maxCameraY = currentMap.HeightInPixels - _minCameraY;
        }

        public void SetCurrentPlayer(RenderablePlayer player)
        {
            _player = player;
        }

        private bool _rotationKeyPressed = false;
        private bool _noRotationKeySent = false;

        public void Update(GameTime gameTime)
        {
            var deltaTime = gameTime.GetElapsedSeconds();
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();

            if (_player != null && !_player.Data.IsDestroyed)
            {
                _rotationKeyPressed = false;
                var keyFlag = Key.None;
                if (keyboardState.IsKeyDown(ForwardInput))
                {
                    _player.Data.Accelerate(-5f);
                    keyFlag |= Key.Forward;
                }

                if (keyboardState.IsKeyDown(BackwardInput))
                {
                    _player.Data.Accelerate(5f);
                    keyFlag |= Key.Backward;
                }

                if (keyboardState.IsKeyDown(LeftInput))
                {
                    _player.Data.Rotate(-deltaTime);
                    _rotationKeyPressed = true;
                    _noRotationKeySent = false;
                    keyFlag |= Key.Left;
                }

                if (keyboardState.IsKeyDown(RightInput))
                {
                    _player.Data.Rotate(deltaTime);
                    _rotationKeyPressed = true;
                    _noRotationKeySent = false;
                    keyFlag |= Key.Right;
                }

                if (!_rotationKeyPressed && !_noRotationKeySent)
                {
                    keyFlag |= Key.NoRotation;
                    _noRotationKeySent = true;
                }

                if (keyboardState.IsKeyDown(FireInputKeyboard) || CheckMouseState(mouseState))
                {
                    _player.Data.Fire();
                    keyFlag |= Key.Fire;
                }

                if (_previousMouseState.X != mouseState.X || _previousMouseState.Y != mouseState.Y)
                {
                    var worldCoords = _camera.ScreenToWorld(new Vector2(mouseState.X, mouseState.Y));
                    _player.LookAt(worldCoords);
                    _client.EnqueuePacket(MouseStatePacket.CreatePacket(_player.Data.Id, worldCoords.X, worldCoords.Y));
                }

                //_camera.Zoom = 1.0f - _player.Velocity.Length() / 500f;
                _previousMouseState = mouseState;
                _player.Update(gameTime);

                if (keyFlag != Key.None)
                    _client.EnqueuePacket(KeyPressPacket.CreatePacket(_player.Data.Id, (byte)keyFlag));
            }

            if (_player != null)
            {
                // We want to clamp camera coordinates so that when the player gets to < CamWidth / 2 or World.MaxX - CamWidth / 2
                // the camera doesnt change in x. Do the same for y.
                var x = _player.Data.Position.X;
                var y = _player.Data.Position.Y;

                _cameraTarget.X = MathHelper.Clamp(x, _minCameraX, _maxCameraX);
                _cameraTarget.Y = MathHelper.Clamp(y, _minCameraY, _maxCameraY);

                _camera.LookAt(_cameraTarget);
            }
        }

        private bool CheckMouseState(MouseState mouseState)
        {
            switch (FireInputMouse)
            {
                case MouseButtons.LeftButton:
                    return mouseState.LeftButton == ButtonState.Pressed;
                case MouseButtons.RightButton:
                    return mouseState.RightButton == ButtonState.Pressed;
                case MouseButtons.MiddleButton:
                    return mouseState.MiddleButton == ButtonState.Pressed;
                case MouseButtons.XButton1:
                    return mouseState.XButton1 == ButtonState.Pressed;
                case MouseButtons.XButton2:
                    return mouseState.XButton2 == ButtonState.Pressed;
                default:
                    return false;
            }
        }
    }
}
