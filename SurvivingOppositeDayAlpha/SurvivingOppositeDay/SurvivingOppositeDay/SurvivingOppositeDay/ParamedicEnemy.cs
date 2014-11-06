using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurvivingOppositeDay
{
    class ParamedicEnemy : BasicEnemy
    {

        public ParamedicEnemy(Game game, SpriteBatch spriteBatch, Texture2D texture, Vector2 position, bool add = false)
            : base(game, spriteBatch, texture, position, add)
        {
            // enemy type
            enemyType = EnemyType.Paramedic;

            // set paramedic health to 200
            Health = 200;
        }   

        public override void Update(GameTime gameTime)
        {
            LinearVelocity = 2;
            base.Update(gameTime);
        }


    }
}
