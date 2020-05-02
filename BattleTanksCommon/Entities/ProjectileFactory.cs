using BattleTanksCommon.Entities;
using BattleTanksCommon.Entities.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended.TextureAtlases;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace BattleTanksClient.Entities
{
    public class ProjectileFactory
    {
        public const float DefaultMovementSpeed = 32.0f;
        public readonly static Vector2 DefaultPosition = Vector2.One;

        private TextureAtlas _atlas;
        private Dictionary<string, TextureRegion2D> _projectileTextures;
        private TextureRegion2D _flashTexture;

        private IEntityManager _entityManager;

        public ProjectileFactory(IEntityManager entityManager, TextureAtlas atlas)
        {
            _entityManager = entityManager;
            _atlas = atlas;
            _projectileTextures = new Dictionary<string, TextureRegion2D>();
            _flashTexture = _atlas.GetRegion("shotRed");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="weapon"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="movementSpeed"></param>
        public void SpawnProjectile(WeaponComponent weapon, Vector2 position, float rotation, float movementSpeed)
        {
            // First get texture for the projectile, add to dictionary if it isn't there.
            if (!_projectileTextures.TryGetValue(weapon.ProjectileName, out var texture))
            {
                texture = _atlas.GetRegion(weapon.ProjectileName);
                _projectileTextures[weapon.ProjectileName] = texture 
                    ?? throw new ArgumentException($"Invalid projectile name defined in WeaponComponent: {weapon.ProjectileName}");
            }

            // Spawn our entity
            var projectile = _entityManager.AddEntity(new Projectile(texture, position, movementSpeed)
            {
                Rotation = rotation,
                DamageSource = weapon.DamageSource
            });
            if (projectile == null)
            {
                Debug.WriteLine("Unable to add projectile.");
                return;
            }
            // Accelerate it
            projectile.Accelerate(movementSpeed);
            // Spawn the flash
            _entityManager.AddEntity(new ProjectileFlashEffect(_flashTexture, position, rotation, 200));
        }
    }
}
