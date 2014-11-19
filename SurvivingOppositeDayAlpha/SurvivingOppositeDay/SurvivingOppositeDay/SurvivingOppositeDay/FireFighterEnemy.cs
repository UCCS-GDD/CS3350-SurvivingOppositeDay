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
            PositionDifference = Position - playerPosition;
            playerPosition -= Position;
            Rotation = (float)Math.Atan2(playerPosition.Y, playerPosition.X);

            // Checks if Fire Fighters in Fire Range
            if (PositionDifference.Length() <= 150)
            {
                LinearVelocity = 0;

                // fires weapon if Fire Rate is ready
                if (canFire)
                {
                    FireWeapon();
                }
            }

            //Moves if out of range of player
            if (PositionDifference.Length() > 150)
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
