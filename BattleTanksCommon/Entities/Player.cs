using BattleTanksCommon.Entities.Interfaces;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.Text;

namespace BattleTanksCommon.Entities
{
    public class Player : IMoveableEntity, IRotatableEntity, IMultiTextureEntity
    {
        public Vector2 Position { get; set; } = new Vector2(100, 100);
        public float MovementSpeed { get; set; } = 2f;
        public Vector2 Rotation { get; set; } = new Vector2(0, 1);
        public float RotationSpeed { get; set; } = 0.1f;

        public Sprite MainSprite { get; set; }
        public string MainSpriteName { get; }

        public Player(string spriteName)
        {
            MainSpriteName = spriteName;
        }

        public void Rotate(float degrees)
        {

        }

        public void RotateLeft()
        {
            var degrees = -1 * RotationSpeed;
            Rotation = Rotation.Rotate(degrees);
        }

        public void RotateRight()
        {
            var degrees = 1 * RotationSpeed;
            Rotation = Rotation.Rotate(degrees);
        }

        /// <summary>
        /// Moves the player based on their movement speed and current rotation vector.
        /// </summary>
        /// <param name="direction">1 for forward, -1 for backward.</param>
        public void Move(int direction)
        {
            var movementDelta = Rotation * direction * MovementSpeed;
            Position = Position.Translate(movementDelta.X, movementDelta.Y);
        }
    }
}
