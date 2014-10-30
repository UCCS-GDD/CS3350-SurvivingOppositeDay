using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurvivingOppositeDay
{
    class HealthBar : BasicSprite
    {
        public Character Character { get; set; }
        public HealthBar(Game game, SpriteBatch spriteBatch, Texture2D texture, Vector2 position, bool add, Character character)
            :base(game, spriteBatch, texture, position, add)
        {
            Character = character;
        }

        public override void Update(GameTime gameTime)
        {
            
            AnimationRectangle = new Rectangle((int)Position.X, (int)Position.Y, Character.Health, 10);
            //Change color
            if (Character.Health <= 10)
                Color = Color.Red;
            else if (Character.Health <= 50)
                Color = Color.Yellow;
            else Color = Color.Green;

            base.Update(gameTime);
        }
    }
}
