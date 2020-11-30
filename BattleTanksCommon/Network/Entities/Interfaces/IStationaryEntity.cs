using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace BattleTanksCommon.Network.Entities.Interfaces
{
    /// <summary>
    /// Entity that exists at a certain point/rotation.
    /// </summary>
    public interface IStationaryEntity
    {
        Vector2 Position { get; set; }
        float Rotation { get; set; }
    }
}
