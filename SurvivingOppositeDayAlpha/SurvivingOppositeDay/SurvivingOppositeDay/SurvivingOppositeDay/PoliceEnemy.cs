using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DynamicFSM;

namespace SurvivingOppositeDay
{
    class PoliceEnemy : BasicEnemy
    {
        StateMachine<PoliceStates> stateMachine;
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

            //State Machine
            stateMachine = new StateMachine<PoliceStates>();

            //Actions for States
            Action policeWalkingAction = () => LinearVelocity = 0;
            Action policeChasingAction = () => LinearVelocity = 1;

            //Transitions between States
            Func<bool> policeFindPlayerTransition = () =>
            {
                return PositionDifference.Length() <= 1;
            };

            stateMachine.AddState(PoliceStates.Walking, policeWalkingAction);
            stateMachine.AddState(PoliceStates.Chasing, policeChasingAction);

            stateMachine.AddTransition(PoliceStates.Walking, PoliceStates.Chasing, policeFindPlayerTransition);
        }

        public override void Update(GameTime gameTime)
        {
            stateMachine.Run();

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
    enum PoliceStates { Walking, Chasing, Donuting}
}
