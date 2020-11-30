using BattleTanksCommon.Network.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using System;
using System.Collections.Generic;
using System.Text;

namespace BattleTanksClient.Entities
{
    /// <summary>
    /// Draws a Player entity class. This has no interaction with the data except
    /// for accessing it to determine where to draw things
    /// </summary>
    public class RenderablePlayer : IRenderable
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public Player Data { get; }
        private Sprite _bodySprite;
        private Sprite _barrelSprite;

        public RenderablePlayer(Player player, TextureAtlas atlas)
        {
            Data = player;
            _bodySprite = new Sprite(GetPlayerHullSprite(atlas));
            _barrelSprite = new Sprite(GetPlayerBarrelSprite(atlas))
            {
                OriginNormalized = new Vector2(0.5f, 0f)
            };
        }

        /// <summary>
        /// This is a LOCAL UPDATE only, server can potentially override
        /// the position on updates.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Data.Position += Data.Velocity * deltaTime;
            Data.BarrelPosition += Data.Velocity * deltaTime;
            Data.Velocity = Vector2.Zero;
        }

        public void Render(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_bodySprite, Data.PositionTransform);
            spriteBatch.Draw(_barrelSprite, Data.BarrelTransform);
            DrawHealthBar(spriteBatch);
        }

        private void DrawHealthBar(SpriteBatch spriteBatch)
        {
            // Health bar should clamp at 3x player width
            float healthBarWidth = Data.Width * 3;
            // Get percentage of the bar to draw in green
            float currentHealthPercent = Data.Health.AsRatio();
            float greenWidth = healthBarWidth * currentHealthPercent;
            // Make our rectangles
            var xpos = Data.Position.X - healthBarWidth / 2;
            var ypos = Data.Position.Y - Data.Height - 10;
            var redRectangle = new RectangleF(xpos, ypos, healthBarWidth, 10);
            var greenRectangle = new RectangleF(xpos, ypos, greenWidth, 10);
            // Draw
            spriteBatch.FillRectangle(redRectangle, Color.Red);
            spriteBatch.FillRectangle(greenRectangle, Color.Green);
        }

        public TextureRegion2D GetPlayerHullSprite(TextureAtlas atlas)
        {
            switch (Data.Color)
            {
                case 0:
                    return atlas.GetRegion("tankBody_red");
                case 1:
                    return atlas.GetRegion("tankBody_green");
                case 2:
                    return atlas.GetRegion("tankBody_dark");
                case 3:
                    return atlas.GetRegion("tankBody_sand");
                default:
                    return atlas.GetRegion("tankBody_red");
            }
        }

        public TextureRegion2D GetPlayerBarrelSprite(TextureAtlas atlas)
        {
            switch (Data.Color)
            {
                case 0:
                    return atlas.GetRegion("tankRed_barrel1");
                case 1:
                    return atlas.GetRegion("tankGreen_barrel1");
                case 2:
                    return atlas.GetRegion("tankDark_barrel1");
                case 3:
                    return atlas.GetRegion("tankSand_barrel1");
                default:
                    return atlas.GetRegion("tankRed_barrel1");
            }
        }

        public void LookAt(Vector2 point)
        {
            float rotation = (float)Math.Atan2(point.Y - (Data.BarrelPosition.Y + _barrelSprite.Origin.Y), point.X - (Data.BarrelPosition.X + _barrelSprite.Origin.X));
            //Data.BarrelRotation = rotation;
        }
    }
}
