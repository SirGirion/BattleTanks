using BattleTanksCommon.Entities;
using Microsoft.Xna.Framework.Input;
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
        private Player _player;

        public MovementController(MouseListener mouseListener, KeyboardListener keyboardListener)
        {
            mouseListener.MouseClicked += OnMouseClicked;
            //keyboardListener.KeyPressed += OnKeyPressed;
            keyboardListener.KeyReleased += OnKeyReleased;
            keyboardListener.KeyTyped += OnKeyPressed;
        }


        private void OnKeyPressed(object sender, KeyboardEventArgs args)
        {
            if (args.Key == Keys.A)
            {
                Debug.WriteLine("A pressed");
                _player.RotateLeft();
            }
            else if (args.Key == Keys.D)
            {
                _player.RotateRight();
            } 
            else if (args.Key == Keys.W)
            {
                Debug.WriteLine("W pressed");
                _player.Move(-1);
            }
            else if (args.Key == Keys.S)
            {
                _player.Move(1);
            }
        }

        private void OnKeyReleased(object sender, KeyboardEventArgs args)
        {
            if (args.Key == Keys.A)
            {

            }
            else if (args.Key == Keys.D)
            {

            }
        }

        private void OnMouseClicked(object sender, MouseEventArgs args)
        {

        }

        /// <summary>
        /// Sets the Player object being managed by this controller.
        /// </summary>
        /// <param name="player">The Player to handle movement for.</param>
        public void RegisterPlayer(Player player)
        {
            _player = player;
        }
    }
}
