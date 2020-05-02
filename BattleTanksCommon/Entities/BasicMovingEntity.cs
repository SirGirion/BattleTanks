using BattleTanksCommon.Entities.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using System;
using System.Collections.Generic;
using System.Text;

namespace BattleTanksCommon.Entities
{
    /// <summary>
    /// Abstract class for entities that move. Extend this class if custom
    /// behavior is desired. Subclasses MUST implement the Draw() method themselves.
    /// </summary>
    public abstract class BasicMovingEntity : Entity, IMoveableEntity, IRotatableEntity, ICollisionActor
    {
        protected Transform2 _transform;

        public Vector2 Direction => Vector2.UnitX.Rotate(Rotation);
        public float Rotation
        {
            get => _transform.Rotation - MathHelper.ToRadians(90);
            set => _transform.Rotation = value + MathHelper.ToRadians(90);
        }
        public float RotationSpeed { get; set; } = 0.0f;

        public Vector2 Position
        {
            get => _transform.Position;
            set
            {
                _transform.Position = value;
                Bounds.Position = _transform.Position;
            }
        }
        public Vector2 Velocity { get; set; }

        public float MovementSpeed { get; set; } = 0.0f;

        public IShapeF Bounds { get; set; }

        public void Accelerate(float acceleration)
        {
            Velocity = Direction * acceleration * MovementSpeed;
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

            Position += Velocity * deltaTime;
        }

        public virtual void OnCollision(CollisionEventArgs collisionInfo)
        {
            throw new NotImplementedException("Your implementation of this class MUST implement this method!");
        }
    }
}
