namespace BattleTanksCommon.Network.Entities.Components
{
    public class HealthComponent
    {
        public float MaxHealth { get; set; }
        public float CurrentHealth { get; set; }

        public HealthComponent(float maxHealth)
        {
            MaxHealth = maxHealth;
            CurrentHealth = MaxHealth;
        }

        public HealthComponent(float maxHealth, float currentHealth)
        {
            MaxHealth = maxHealth;
            CurrentHealth = currentHealth;
        }

        public float AsPercent()
        {
            return 100 * CurrentHealth / MaxHealth;
        }

        public float AsRatio()
        {
            return CurrentHealth / MaxHealth;
        }
    }
}
