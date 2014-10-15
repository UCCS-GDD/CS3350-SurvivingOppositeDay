using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurvivingOppositeDay
{
    class BasicEnemy: Character
    {
        public Vector2 playerPosition;
        public BasicEnemy(Game game, SpriteBatch spriteBatch, Texture2D texture, Vector2 position, bool add = false)
            : base(game, spriteBatch, texture, position, add)
        {
            LinearVelocity = 1;
        }

        public override void Update(GameTime gameTime)
        {
            playerPosition -= Position;
            Rotation = (float)Math.Atan2(playerPosition.Y, playerPosition.X);

            base.Update(gameTime);
        }

        public void GetPlayerPosition(Vector2 position)
        {
            playerPosition = position;
        }
    }
}
