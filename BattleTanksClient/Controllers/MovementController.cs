using BattleTanksCommon.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Input.InputListeners;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

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
        private readonly Player _player;
        /// <summary>
        /// Camera used for translating screen coordinates to world coordinates.
        /// </summary>
        private readonly OrthographicCamera _camera;

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

        public MovementController(Player player, OrthographicCamera camera)
        {
            _player = player;
            _camera = camera;
            ForwardInput = Keys.W;
            BackwardInput = Keys.S;
            LeftInput = Keys.A;
            RightInput = Keys.D;
            UsePowerUp = Keys.E;
            ExitGame = Keys.Escape;
            FireInputMouse = MouseButtons.LeftButton;
            FireInputKeyboard = Keys.None;
        }

        public void Update(GameTime gameTime)
        {
            var deltaTime = gameTime.GetElapsedSeconds();
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();

            if (_player != null && !_player.IsDestroyed)
            {
                if (keyboardState.IsKeyDown(this.ForwardInput))
                    _player.Accelerate(-5f);

                if (keyboardState.IsKeyDown(this.BackwardInput))
                    _player.Accelerate(5f);

                if (keyboardState.IsKeyDown(this.LeftInput))
                    _player.Rotate(-deltaTime);

                if (keyboardState.IsKeyDown(this.RightInput))
                    _player.Rotate(deltaTime);

                if (keyboardState.IsKeyDown(this.FireInputKeyboard) || this.CheckMouseState(mouseState))
                    _player.Fire();

                if (_previousMouseState.X != mouseState.X || _previousMouseState.Y != mouseState.Y)
                    _player.LookAt(_camera.ScreenToWorld(new Vector2(mouseState.X, mouseState.Y)));

                //_camera.Zoom = 1.0f - _player.Velocity.Length() / 500f;
                _previousMouseState = mouseState;
            }
        }

        private bool CheckMouseState(MouseState mouseState)
        {
            switch (this.FireInputMouse)
            {
                case MouseButtons.LeftButton:
                    return (mouseState.LeftButton == ButtonState.Pressed);
                case MouseButtons.RightButton:
                    return (mouseState.RightButton == ButtonState.Pressed);
                case MouseButtons.MiddleButton:
                    return (mouseState.MiddleButton == ButtonState.Pressed);
                case MouseButtons.XButton1:
                    return (mouseState.XButton1 == ButtonState.Pressed);
                case MouseButtons.XButton2:
                    return (mouseState.XButton2 == ButtonState.Pressed);
                default:
                    return false;
            }
        }
    }
}
