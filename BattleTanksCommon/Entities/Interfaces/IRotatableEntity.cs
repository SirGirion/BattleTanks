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
        /// The current direction of the entity.
        /// </summary>
        Vector2 Direction { get; }
        /// <summary>
        /// The current rotation in degrees.
        /// </summary>
        float Rotation { get; set; }
        /// <summary>
        /// The max speed an entity is allowed to rotate.
        /// </summary>
        float RotationSpeed { get; set; }
    }
}
