using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace BattleTanksCommon.Network.Entities
{
    public abstract class Entity
    {
        public bool IsDestroyed { get; private set; }

        public int Id { get; set; }

        protected Entity()
        {
            IsDestroyed = false;
        }

        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch);

        public virtual void Destroy()
        {
            IsDestroyed = true;
        }
    }
}
