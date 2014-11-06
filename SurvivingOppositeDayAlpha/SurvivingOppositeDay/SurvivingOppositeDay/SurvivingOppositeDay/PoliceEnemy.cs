using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurvivingOppositeDay
{
    class PoliceEnemy : BasicEnemy
    {
        public event Action<EnemyAction, BasicEnemy> EnemyActionTriggeredEvent;
        Timer fireTimer;
        TimeSpan fireTimeSpan;
        private bool canFire = false;
        public PoliceEnemy(Game game, SpriteBatch spriteBatch, Texture2D texture, Vector2 position, bool add = false)
            : base(game, spriteBatch, texture, position, add)
        {
            // Fire Rate timer
            fireTimeSpan = TimeSpan.FromSeconds(2);
            fireTimer = new Timer();
            fireTimer.OnExpire += () => canFire = true;
            fireTimer.Start(fireTimeSpan);

            // enemy type
            enemyType = EnemyType.Police;

            Health = 200;
        }

        public override void Update(GameTime gameTime)
        {
            // fires weapon if Fire Rate is ready
            if (canFire)
            {
                FireWeapon();
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
