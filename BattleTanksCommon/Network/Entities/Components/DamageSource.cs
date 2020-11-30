using System;
using System.Collections.Generic;
using System.Text;

namespace BattleTanksCommon.Network.Entities.Components
{
    public enum DamageType
    {
        Normal,
        Explosive,
        Energy
    }

    public class DamageSource
    {
        public float Damage { get; set; }
        public DamageType Type { get; set; }
    }
}
