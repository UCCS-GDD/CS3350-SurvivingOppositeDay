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
        public event Action<EnemyAction, BasicEnemy> EnemyActionTriggeredEvent;
        public Vector2 playerPosition;
        Timer fireTimer;
        TimeSpan fireTimeSpan;
        private bool canFire = false;
        public BasicEnemy(Game game, SpriteBatch spriteBatch, Texture2D texture, Vector2 position, bool add = false)
            : base(game, spriteBatch, texture, position, add)
        {
            LinearVelocity = 1;
            Health = 100;
            
            // Fire Rate timer
            fireTimeSpan = TimeSpan.FromSeconds(2);
            fireTimer = new Timer();
            fireTimer.OnExpire += () => canFire = true;
            fireTimer.Start(fireTimeSpan);
        }

        public override void Update(GameTime gameTime)
        {
            playerPosition -= Position;
            Rotation = (float)Math.Atan2(playerPosition.Y, playerPosition.X);

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

        public void GetPlayerPosition(Vector2 position)
        {
            playerPosition = position;
        }
    }
    public enum EnemyAction { FireBullet }
}
