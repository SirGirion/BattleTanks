using BattleTanksCommon.Network.Entities.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using System;
using System.Collections.Generic;
using System.Text;

namespace BattleTanksCommon.Network.Entities
{
    /// <summary>
    /// Abstract class for entities that move. Extend this class if custom
    /// behavior is desired. Subclasses MUST implement the Draw() method themselves.
    /// </summary>
    public abstract class BasicMovingEntity : Entity, IMoveableEntity, IRotatableEntity, ICollisionActor
    {
        protected Transform2 _positionTransform;

        public Vector2 Direction => Vector2.UnitX.Rotate(Rotation);
        public float Rotation
        {
            get => _positionTransform.Rotation - MathHelper.ToRadians(90);
            set => _positionTransform.Rotation = value + MathHelper.ToRadians(90);
        }
        public float RotationSpeed { get; set; } = 0.0f;

        public Vector2 Position
        {
            get => _positionTransform.Position;
            set
            {
                _positionTransform.Position = value;
                Bounds.Position = _positionTransform.Position;
            }
        }

        public Transform2 PositionTransform => _positionTransform;

        public Vector2 Velocity { get; set; }

        public float MovementSpeed { get; set; } = 0.0f;

        public IShapeF Bounds { get; set; }

        public void Accelerate(float acceleration)
        {
            Velocity = Direction * acceleration * MovementSpeed;
        }

        private int _rotationDirection;

        public void Rotate(int direction)
        {
            _rotationDirection = direction;
        }

        public void Rotate(float deltaTime)
        {
            Rotation += deltaTime * RotationSpeed;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            throw new NotImplementedException("Your implementation of this class MUST implement this method!");
        }

        /// <summary>
        /// Updates the entities position by their velocity * deltaTime.
        /// </summary>
        /// <param name="gameTime">The current GameTime.</param>
        public override void Update(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Rotation += deltaTime * _rotationDirection * RotationSpeed;

            Position += Velocity * deltaTime;
        }

        public virtual void OnCollision(CollisionEventArgs collisionInfo)
        {
            throw new NotImplementedException("Your implementation of this class MUST implement this method!");
        }
    }
}
