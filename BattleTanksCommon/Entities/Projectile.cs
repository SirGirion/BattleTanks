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
using System.Text;

namespace BattleTanksCommon.Entities
{
    public class ProjectileFlashEffect : Entity, IStationaryEntity
    {
        public float Rotation
        {
            get => _transform.Rotation - MathHelper.ToRadians(90);
            set => _transform.Rotation = value + MathHelper.ToRadians(90);
        }
        public Vector2 Position
        {
            get => _transform.Position;
            set => _transform.Position = value;
        }

        private Transform2 _transform;
        private Sprite _sprite;
        private int _maxLife;
        private int _life;

        public ProjectileFlashEffect(TextureRegion2D texture, Vector2 position, float rotation, int life = 1000)
        {
            _sprite = new Sprite(texture)
            {
                OriginNormalized = new Vector2(0.5f, 0)
            };
            _transform = new Transform2
            {
                Position = position,
                Scale = Vector2.One
            };
            Rotation = rotation - MathHelper.Pi;
            _maxLife = _life = life;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_sprite, _transform);
        }

        public override void Update(GameTime gameTime)
        {
            _life -= (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            _sprite.Alpha = _life / (float)_maxLife;
            if (_life <= 0)
                Destroy();
        }
    }

    public class Projectile : BasicMovingEntity
    {
        public const int ProjectileLifeTime = 3000;

        private Sprite _projectileSprite;
        private int _life;

        public DamageSource DamageSource { get; set; }

        public Projectile(TextureRegion2D texture, Vector2 position, float movementSpeed)
        {
            _transform = new Transform2
            {
                Position = position,
                Scale = Vector2.One
            };
            Bounds = new RectangleF(_transform.Position.ToPoint(), new Size2(texture.Width, texture.Height));
            _projectileSprite = new Sprite(texture);
            MovementSpeed = movementSpeed;
            _life = ProjectileLifeTime;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_projectileSprite, _transform);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if ((_life -= (int)gameTime.ElapsedGameTime.TotalMilliseconds) <= 0)
                Destroy();
        }

        public override void OnCollision(CollisionEventArgs collisionInfo)
        {
            Destroy();
        }
    }
}
