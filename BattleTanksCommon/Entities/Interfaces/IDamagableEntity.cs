using BattleTanksCommon.Entities.Components;

namespace BattleTanksCommon.Entities.Interfaces
{
    public interface IDamagableEntity
    {
        HealthComponent Health { get; set; }
        void ApplyDamage(DamageSource damageSource);
    }
}
