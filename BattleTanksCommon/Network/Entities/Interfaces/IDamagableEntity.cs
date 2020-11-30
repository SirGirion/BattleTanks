using BattleTanksCommon.Network.Entities.Components;

namespace BattleTanksCommon.Network.Entities.Interfaces
{
    public interface IDamagableEntity
    {
        HealthComponent Health { get; set; }
        void ApplyDamage(DamageSource damageSource);
    }
}
