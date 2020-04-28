﻿using BattleTanksCommon.Entities;
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

        public MovementController(Player player, OrthographicCamera camera)
        {
            _player = player;
            _camera = camera;
        }

        public void Update(GameTime gameTime)
        {
            var deltaTime = gameTime.GetElapsedSeconds();
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();

            if (_player != null && !_player.IsDestroyed)
            {
                if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up))
                    _player.Accelerate(-5f);

                if (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down))
                    _player.Accelerate(5f);

                if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left))
                    _player.Rotate(-deltaTime);

                if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right))
                    _player.Rotate(deltaTime);

                if (keyboardState.IsKeyDown(Keys.Space) || mouseState.LeftButton == ButtonState.Pressed)
                    _player.Fire();

                if (_previousMouseState.X != mouseState.X || _previousMouseState.Y != mouseState.Y)
                    _player.LookAt(_camera.ScreenToWorld(new Vector2(mouseState.X, mouseState.Y)));

                //_camera.Zoom = 1.0f - _player.Velocity.Length() / 500f;
                _previousMouseState = mouseState;
            }
        }
    }
}
