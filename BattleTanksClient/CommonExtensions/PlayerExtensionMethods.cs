using BattleTanksCommon.Entities;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using System;
using System.Collections.Generic;
using System.Text;

namespace BattleTanksClient.CommonExtensions
{
    public static class PlayerExtensionMethods
    {
        public static void Draw(this Player player, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(player.MainSprite, player.Position, player.Rotation.ToAngle());
        }

        public static void SetMainSprite(this Player player, TextureAtlas atlas)
        {
            player.MainSprite = new Sprite(atlas.GetRegion(player.MainSpriteName));
        }
    }
}
