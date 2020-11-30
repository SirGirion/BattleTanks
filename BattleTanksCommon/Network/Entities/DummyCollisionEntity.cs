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
    /// Entity whose sole purpose is to act as a collision entity.
    /// </summary>
    public class DummyCollisionEntity : Entity, IStationaryEntity, ICollisionActor
    {
        public IShapeF Bounds { get; set; }

        public Vector2 Position { get; set; }
        public float Rotation { get; set; } = 0f;

        public override void Draw(SpriteBatch spriteBatch)
        {
            // NOOP
        }

        /// <summary>
        /// Handles collision. An entity of this type won't respond to collisions
        /// because it cannot move.
        /// </summary>
        /// <param name="collisionInfo"></param>
        public void OnCollision(CollisionEventArgs collisionInfo)
        {
        }

        public override void Update(GameTime gameTime)
        {
            // NOOP
        }
    }
}
