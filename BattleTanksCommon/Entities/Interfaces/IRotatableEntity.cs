using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace BattleTanksCommon.Entities.Interfaces
{
    /// <summary>
    /// Interface for entities that can rotate.
    /// </summary>
    public interface IRotatableEntity
    {
        /// <summary>
        /// The current direction of rotation. This is a unit vector with (1, 0) representing to the right.
        /// </summary>
        Vector2 Rotation { get; set; }
        /// <summary>
        /// The max speed an entity is allowed to rotate.
        /// </summary>
        float RotationSpeed { get; set; }
    }
}
