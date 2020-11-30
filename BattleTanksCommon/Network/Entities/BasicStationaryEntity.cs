using BattleTanksCommon.Network.Entities.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace BattleTanksCommon.Network.Entities
{
    public class BasicStationaryEntity : Entity, IStationaryEntity
    {
        public Vector2 Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public float Rotation { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void Draw(SpriteBatch spriteBatch)
        {
        }

        public override void Update(GameTime gameTime)
        {
        }
    }
}
