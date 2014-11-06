using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurvivingOppositeDay
{
    class FireFighterEnemy : BasicEnemy
    {
        public event Action<EnemyAction, BasicEnemy> EnemyActionTriggeredEvent;
        Timer fireTimer;
        TimeSpan fireTimeSpan;
        private bool canFire = false;
        private Vector2 PositionDifference { get; set; }

        public FireFighterEnemy(Game game, SpriteBatch spriteBatch, Texture2D texture, Vector2 position, bool add = false)
            : base(game, spriteBatch, texture, position, add)
        {
            // Fire Rate timer
            fireTimeSpan = TimeSpan.FromSeconds(0.1f);
            fireTimer = new Timer();
            fireTimer.OnExpire += () => canFire = true;
            fireTimer.Start(fireTimeSpan);

            // enemy type
            enemyType = EnemyType.FireFighter;

            Health = 200;
        }

        public override void Update(GameTime gameTime)
        {
            // Difference in position relative to player
            PositionDifference = new Vector2(Math.Abs(playerPosition.X - Position.X), Math.Abs(playerPosition.Y - Position.Y));

            // Checks if Fire Fighters in Fire Range
            if (PositionDifference.X <= 150
                && PositionDifference.Y <= 150)
            {
                LinearVelocity = 0;

                // fires weapon if Fire Rate is ready
                if (canFire)
                {
                    FireWeapon();
                }
            }

            //Moves if out of range of player
            if (PositionDifference.X > 150
                || PositionDifference.Y > 150)
            {
                LinearVelocity = 1;
            }

            //Update timer
            fireTimer.Update(gameTime.ElapsedGameTime);

            base.Update(gameTime);
        }

        private void FireWeapon()
        {
            if (EnemyActionTriggeredEvent != null)
            {
                EnemyActionTriggeredEvent(EnemyAction.FireBullet, this);
                canFire = false;

                //start timer
                fireTimer.Start(fireTimeSpan);
            }
        }
    }
}
