using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurvivingOppositeDay
{
    class Pedestrian : Character
    {
        Timer turnTimer;
        TimeSpan turnTimeSpan;
        private bool canTurn;
        Random random;
        public Pedestrian(Game game, SpriteBatch spriteBatch, Texture2D texture, Vector2 position, int seed, bool add = false)
            : base(game, spriteBatch, texture, position, add)
        {
            CharacterHealth = 1;
            LinearVelocity = 3;
            turnTimeSpan = TimeSpan.FromSeconds(1);
            turnTimer = new Timer();
            turnTimer.OnExpire += () => canTurn = true;
            turnTimer.Start(turnTimeSpan);
            random = new Random(seed);
        }

        public override void Update(GameTime gameTime)
        {
            if (canTurn)
            {
                RotationalVelocity = (random.Next(3) - 1) * (random.Next(10));
                canTurn = false;
                turnTimer.Start(turnTimeSpan);
            }

            //Out of room, appears at the other side
            if (Position.X > 1980)
                Position.X = 20;
            if (Position.X < 20)
                Position.X = 1980;
            if (Position.Y > 1980)
                Position.Y = 20;
            if (Position.Y < 20)
                Position.Y = 1980;

            turnTimer.Update(gameTime.ElapsedGameTime);
            base.Update(gameTime);
            RotationalVelocity = 0;
        }
    }
}
