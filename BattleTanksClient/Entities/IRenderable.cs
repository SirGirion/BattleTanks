using Microsoft.Xna.Framework.Graphics;

namespace BattleTanksClient.Entities
{
    public interface IRenderable
    {
        /// <summary>
        /// Method for rendering an entity.
        /// </summary>
        /// <param name="spriteBatch"></param>
        void Render(SpriteBatch spriteBatch);
    }
}
