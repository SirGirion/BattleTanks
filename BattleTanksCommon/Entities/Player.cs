using BattleTanksClient.Entities;
using BattleTanksCommon.Entities.Components;
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
    public class Player : BasicMovingEntity
    {
        //private readonly Transform2 _transform;
        private readonly Transform2 _barrelTransform;

        private Sprite _bodySprite;
        private Sprite _barrelSprite;

        public Vector2 BarrelPosition
        {
            get => _barrelTransform.Position;
            set => _barrelTransform.Position = value;
        }

        public float BarrelRotation
        {
            get => _barrelTransform.Rotation + MathHelper.ToRadians(90);
            set => _barrelTransform.Rotation = value - MathHelper.ToRadians(90);
        }

        public WeaponComponent WeaponComponent { get; set; }
        private ProjectileFactory _factory;

        public Player(TextureRegion2D bodyTexture, TextureRegion2D barrelTexture, ProjectileFactory projectileFactory)
        {
            MovementSpeed = 32.0f;
            RotationSpeed = 2.5f;
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
            WeaponComponent = new WeaponComponent("redBarrel", "bulletRed1", 750);
            _factory = projectileFactory;
        }

        public void SetPosition(float x, float y)
        {
            Position = BarrelPosition = new Vector2(x, y);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_bodySprite, _transform);
            spriteBatch.Draw(_barrelSprite, _barrelTransform);
            //spriteBatch.DrawPoint(_barrelSprite.OriginNormalized + BarrelPosition, Color.Green);
            //spriteBatch.DrawPoint(Position + _bodySprite.OriginNormalized, Color.Red);
        }

        public override void Update(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Position += Velocity * deltaTime;
            BarrelPosition += Velocity * deltaTime;
            Velocity = Vector2.Zero;


            WeaponComponent.Update(gameTime);
        }

        public void LookAt(Vector2 point)
        {
            BarrelRotation = (float)Math.Atan2(point.Y - (BarrelPosition.Y + _barrelSprite.Origin.Y), point.X - (BarrelPosition.X + _barrelSprite.Origin.X));
        }
        

        public void Fire()
        {
            if (WeaponComponent.Fire())
            {
                // Spawn the bullet
                var bulletPosition = BarrelPosition + (Vector2.UnitX * _barrelSprite.TextureRegion.Height).Rotate(BarrelRotation);
                _factory.SpawnProjectile(WeaponComponent, bulletPosition, BarrelRotation, 20.0f);
            }
        }
    }
}
