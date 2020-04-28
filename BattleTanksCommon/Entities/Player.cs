using BattleTanksCommon.Entities.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace BattleTanksCommon.Entities
{
    public class Player : Entity
    {
        private readonly Transform2 _transform;
        private readonly Transform2 _barrelTransform;

        private Sprite _bodySprite;
        private Sprite _barrelSprite;

        public float MovementSpeed { get; set; } = 32.0f;
        public float RotationSpeed { get => 2.5f; }

        public Vector2 Direction => Vector2.UnitX.Rotate(Rotation);

        public Vector2 Position
        {
            get => _transform.Position;
            set
            {
                _transform.Position = value;
                
            }
        }

        public Vector2 BarrelPosition
        {
            get => _barrelTransform.Position;
            set => _barrelTransform.Position = value;
        }

        public float Rotation
        {
            get => _transform.Rotation - MathHelper.ToRadians(90);
            set => _transform.Rotation = value + MathHelper.ToRadians(90);
        }

        public float BarrelRotation
        {
            get => _barrelTransform.Rotation + MathHelper.ToRadians(90);
            set => _barrelTransform.Rotation = value - MathHelper.ToRadians(90);
        }

        public Vector2 Velocity { get; set; }

        public Player(TextureRegion2D bodyTexture, TextureRegion2D barrelTexture)
        {
            _bodySprite = new Sprite(bodyTexture);
            _barrelSprite = new Sprite(barrelTexture)
            {
                OriginNormalized = new Vector2(0.5f, 0.0f)
            };
            _transform = new Transform2
            {
                Scale = Vector2.One,
                Position = new Vector2(400, 240)
            };
            _barrelTransform = new Transform2
            {
                Scale = Vector2.One,
                Position = new Vector2(400, 240)
            };
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_bodySprite, _transform);
            spriteBatch.Draw(_barrelSprite, _barrelTransform);
            spriteBatch.DrawPoint(_barrelSprite.OriginNormalized + BarrelPosition, Color.Green);
            spriteBatch.DrawPoint(Position + _bodySprite.OriginNormalized, Color.Red);
        }

        public override void Update(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Position += Velocity * deltaTime;
            BarrelPosition += Velocity * deltaTime;
            Velocity = Vector2.Zero;

            //if (_fireCooldown > 0)
            //{
            //    _fireCooldown -= deltaTime;
            //}
        }

        public void Accelerate(float acceleration)
        {
            Velocity = Direction * acceleration * MovementSpeed;
        }

        public void LookAt(Vector2 point)
        {
            BarrelRotation = (float)Math.Atan2(point.Y - (BarrelPosition.Y + _barrelSprite.Origin.Y), point.X - (BarrelPosition.X + _barrelSprite.Origin.X));
        }

        public void Move(int direction)
        {
            var movementDelta = Direction * direction * MovementSpeed;
            Position = Position.Translate(movementDelta.X, movementDelta.Y);
            BarrelPosition = BarrelPosition.Translate(movementDelta.X, movementDelta.Y);
        }

        public void Rotate(float deltaTime)
        {
            Rotation += deltaTime * RotationSpeed;
        }
    }
}
