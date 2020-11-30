using BattleTanksCommon.Network.Entities.Components;
using BattleTanksCommon.Network.Entities.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using System;

namespace BattleTanksCommon.Network.Entities
{
    public class Player : BasicMovingEntity, IDamagableEntity
    {
        public Vector2 BarrelPosition
        {
            get => BarrelTransform.Position;
            set => BarrelTransform.Position = value;
        }

        public float BarrelRotation
        {
            get => BarrelTransform.Rotation + MathHelper.ToRadians(90);
            set => BarrelTransform.Rotation = value - MathHelper.ToRadians(90);
        }

        public Transform2 BarrelTransform { get; }

        public WeaponComponent WeaponComponent { get; set; }

        public HealthComponent Health { get; set; }

        public int Width;
        public int Height;

        public byte Color { get; }

        public Player(int id, int x, int y, byte color)
        {
            Id = id;
            Color = color;
            Width = 32;
            Height = 32;

            MovementSpeed = 32.0f;
            RotationSpeed = 2.5f;
            _positionTransform = new Transform2
            {
                Scale = Vector2.One,
                Position = new Vector2(x, y)
            };
            BarrelTransform = new Transform2
            {
                Scale = Vector2.One,
                Position = new Vector2(x, y)
            };

            Bounds = new CircleF(_positionTransform.Position.ToPoint(), (Width + Height) / 4);

            WeaponComponent = new WeaponComponent("redBarrel", "bulletRed1", 750)
            {
                DamageSource = new DamageSource
                {
                    Damage = 20f
                }
            };

            Health = new HealthComponent(200f, 20f);
        }

        public void SetPosition(float x, float y)
        {
            Position = BarrelPosition = new Vector2(x, y);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(_bodySprite, _transform);
            //spriteBatch.Draw(_barrelSprite, _barrelTransform);
            //DrawHealthBar(spriteBatch);
        }

        public bool Updated { get; private set; }
        private Vector2 _lastPosition = Vector2.Zero;
        private float _lastRotation = 0f;
        private float _lastBarrelRotation = 0f;

        public override void Update(GameTime gameTime)
        {
            Updated = false;
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            base.Update(gameTime);
            BarrelPosition += Velocity * deltaTime;
            Velocity = Vector2.Zero;

            if (Position != _lastPosition || Rotation != _lastRotation || BarrelRotation != _lastBarrelRotation)
                Updated = true;
            WeaponComponent.Update(gameTime);

            if (Health.CurrentHealth < Health.MaxHealth)
                Health.CurrentHealth += 10 * deltaTime;

            _lastPosition = new Vector2(Position.X, Position.Y);
            _lastRotation = Rotation;
            _lastBarrelRotation = BarrelRotation;
        }

        public void LookAt(Vector2 point)
        {
            // TODO: Get 6, 0 from sprites server side
            BarrelRotation = (float)Math.Atan2(point.Y - (BarrelPosition.Y + 6), point.X - (BarrelPosition.X + 0));
        }

        public void Fire()
        {
            //if (WeaponComponent.Fire())
            //{
            //    // Spawn the bullet
            //    var bulletPosition = BarrelPosition + (Vector2.UnitX * _barrelSprite.TextureRegion.Height).Rotate(BarrelRotation);
            //    _factory.SpawnProjectile(WeaponComponent, bulletPosition, BarrelRotation, 20.0f);
            //}
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
