using BattleTanksClient.Entities;
using BattleTanksCommon.Entities.Components;
using BattleTanksCommon.Entities.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace BattleTanksCommon.Entities
{
    public class Player : BasicMovingEntity, IDamagableEntity
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

        public HealthComponent Health { get; set; }

        private float _width;
        private float _height;

        public Player(TextureRegion2D bodyTexture, TextureRegion2D barrelTexture, ProjectileFactory projectileFactory)
        {
            _width = bodyTexture.Width;
            _height = bodyTexture.Height;

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

            Bounds = new CircleF(_transform.Position.ToPoint(), (_width + _height) / 4);

            WeaponComponent = new WeaponComponent("redBarrel", "bulletRed1", 750)
            {
                DamageSource = new DamageSource
                {
                    Damage = 20f
                }
            };
            _factory = projectileFactory;

            Health = new HealthComponent(200f, 20f);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_bodySprite, _transform);
            spriteBatch.Draw(_barrelSprite, _barrelTransform);
            //spriteBatch.DrawPoint(_barrelSprite.OriginNormalized + BarrelPosition, Color.Green);
            //spriteBatch.DrawPoint(Position + _bodySprite.OriginNormalized, Color.Red);
            DrawHealthBar(spriteBatch);
        }

        private void DrawHealthBar(SpriteBatch spriteBatch)
        {
            // Health bar should clamp at 3x player width
            float healthBarWidth = _width * 3;
            // Get percentage of the bar to draw in green
            float currentHealthPercent = Health.AsRatio();
            float greenWidth = healthBarWidth * currentHealthPercent;
            // Make our rectangles
            var xpos = Position.X - healthBarWidth / 2;
            var ypos = Position.Y - _height - 10;
            var redRectangle = new RectangleF(xpos, ypos, healthBarWidth, 10);
            var greenRectangle = new RectangleF(xpos, ypos, greenWidth, 10);
            // Draw
            spriteBatch.FillRectangle(redRectangle, Color.Red);
            spriteBatch.FillRectangle(greenRectangle, Color.Green);
        }

        public override void Update(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Position += Velocity * deltaTime;
            BarrelPosition += Velocity * deltaTime;
            Velocity = Vector2.Zero;


            WeaponComponent.Update(gameTime);

            if (Health.CurrentHealth < Health.MaxHealth)
                Health.CurrentHealth += 10 * deltaTime;
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

        public void ApplyDamage(DamageSource damageSource)
        {
            Health.CurrentHealth -= damageSource.Damage;
        }

        public override void OnCollision(CollisionEventArgs collisionInfo)
        {
            // If we collided with a projectile, apply damage
            if (collisionInfo.Other is Projectile projectile)
                ApplyDamage(projectile.DamageSource);
            else
            {
                Position -= collisionInfo.PenetrationVector;
                BarrelPosition -= collisionInfo.PenetrationVector;
            }
        }
    }
}
