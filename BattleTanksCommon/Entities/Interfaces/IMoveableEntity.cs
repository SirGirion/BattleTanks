using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace BattleTanksCommon.Entities.Interfaces
{
    public interface IMoveableEntity
    {
        Vector2 Position { get; set; }
        float MovementSpeed { get; set; }
    }
}
