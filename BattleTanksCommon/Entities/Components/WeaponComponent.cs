using Microsoft.Xna.Framework;

namespace BattleTanksCommon.Entities.Components
{
    public class WeaponComponent
    {
        /// <summary>
        /// Backing field of FireCooldown. Used to modify the cooldown.
        /// </summary>
        private int _fireCooldown;
        /// <summary>
        /// Cooldown between the weapon being "ready"; this is in milliseconds.
        /// </summary>
        public int FireCooldown
        {
            get => _fireCooldown;
            set => _fireCooldown = _cooldown = value;
        }
        /// <summary>
        /// Current status of the cooldown. Need a second field so we can reset
        /// the timer after a successful fire.
        /// </summary>
        private int _cooldown;

        /// <summary>
        /// Name of the weapon.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Name of the projectile fired by this weapon.
        /// </summary>
        public string ProjectileName { get; }

        /// <summary>
        /// Property indicating if this weapon is ready to fire.
        /// </summary>
        public bool CanFire => _cooldown <= 0;

        public DamageSource DamageSource { get; set; }

        /// <summary>
        /// Creates a new WeaponComponent object.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="fireCooldown"></param>
        public WeaponComponent(string name, string projectileName, int fireCooldown)
        {
            Name = name;
            ProjectileName = projectileName;
            FireCooldown = fireCooldown;
        }

        /// <summary>
        /// Updates the cooldown timer of this weapon.
        /// </summary>
        /// <param name="gameTime">The current GameTime.</param>
        public void Update(GameTime gameTime)
        {
            _cooldown -= (int)gameTime.ElapsedGameTime.TotalMilliseconds;
        }

        /// <summary>
        /// Tries to "fire" the weapon. If it is ready, the timer will be reset.
        /// </summary>
        /// <returns>True if the weapon fired.</returns>
        public bool Fire()
        {
            if (CanFire)
            {
                _cooldown = FireCooldown;
                return true;
            }
            return false;
        }
    }
}
