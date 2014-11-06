using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurvivingOppositeDay
{
    class BasicBullet: BasicSprite 
    {
        public BasicBullet(Game game, SpriteBatch spriteBatch, Texture2D texture, Vector2 position, bool add = false)
            : base(game, spriteBatch, texture, position, add)
        {
           
        }

        public void SetDirection(float angle, float magnitude)
        {
            LinearVelocity = magnitude;
            Rotation = angle;
        }

        public override void Update(GameTime gameTime)
        {
            if (!Game1.Playfield.Contains(Tools.Math.Points.FromVector(Position)))
            {
                Game.Components.Remove(this);
            }
            base.Update(gameTime);
        }
    }
}
