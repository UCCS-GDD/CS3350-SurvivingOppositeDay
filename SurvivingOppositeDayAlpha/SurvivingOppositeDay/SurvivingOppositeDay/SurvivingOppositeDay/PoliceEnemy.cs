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
        Random random;
        Timer turnTimer;
        TimeSpan turnTimeSpan;
        private bool canTurn;
        Timer pedestrianKillTimer;
        TimeSpan pedestrianKillTimeSpan;
        private bool pedestrianKilled = false;
        public Vector2 donutPosition;
        public PoliceEnemy(Game game, SpriteBatch spriteBatch, Texture2D texture, Vector2 position, int seed, bool add = false)
            : base(game, spriteBatch, texture, position, add)
        {
            // Fire Rate timer
            fireTimeSpan = TimeSpan.FromSeconds(2);
            fireTimer = new Timer();
            fireTimer.OnExpire += () => canFire = true;
            fireTimer.Start(fireTimeSpan);

            // Change direction timer
            turnTimeSpan = TimeSpan.FromSeconds(1);
            turnTimer = new Timer();
            turnTimer.OnExpire += () => canTurn = true;
            turnTimer.Start(turnTimeSpan);

            // Pedestrian Kill timer
            pedestrianKillTimeSpan = TimeSpan.FromSeconds(5);
            pedestrianKillTimer = new Timer();
            pedestrianKillTimer.OnExpire += () => pedestrianKilled = false;

            // enemy type
            enemyType = EnemyType.Police;

            Health = 200;

            //Random number
            random = new Random();

            //State Machine
            stateMachine = new StateMachine<PoliceStates>();

            //Actions for States
            Action policeWalkingAction = () =>
            {
                LinearVelocity = 1;
                if (canTurn)
                {
                    Rotation = (random.Next(3) - 1) * (random.Next(10));
                    canTurn = false;
                    turnTimer.Start(turnTimeSpan);
                }
            };
            Action policeChasingAction = () =>
            {
                playerPosition -= Position;
                Rotation = (float)Math.Atan2(playerPosition.Y, playerPosition.X);
                LinearVelocity = 2;
            };
            Action policeDonutingAction = () =>
            {
                IEnumerable<PlayerBullet> bullets = Game.Components.OfType<PlayerBullet>();
                foreach (PlayerBullet bullet in bullets)
                {
                    if (bullet.weaponType == WeaponType.DonutGun)
                    {
                        donutPosition = bullet.Position;
                        donutPosition -= Position;
                        Rotation = (float)Math.Atan2(donutPosition.Y, donutPosition.X);
                        LinearVelocity = 2;
                    }
                }
            };

            //Transitions between States
            Func<bool> policeFindPlayerTransition = () =>
            {
                return PositionDifference.Length() <= 200;
            };
            Func<bool> policeLosePlayerTransition = () =>
            {
                return PositionDifference.Length() > 200;
            };
            Func<bool> policeFindDonutTransition = () =>
            {
                IEnumerable<PlayerBullet> bullets = Game.Components.OfType<PlayerBullet>();
                foreach (PlayerBullet bullet in bullets)
                {
                    if (bullet.weaponType == WeaponType.DonutGun)
                    {
                        return true;
                    }
                }
                return false;
            };
            Func<bool> policeNoMoreDonutTransition = () =>
            {
                IEnumerable<PlayerBullet> bullets = Game.Components.OfType<PlayerBullet>();
                foreach (PlayerBullet bullet in bullets)
                {
                    if (bullet.weaponType == WeaponType.DonutGun)
                    {
                        return false;
                    }
                }
                return true;
            };
            Func<bool> policeKillPedestrianTransition = () =>
            {
                //currentPedestrianCount = Game.Components.OfType<Pedestrian>().Count();
                //if (currentPedestrianCount < previousPedestrianCount)
                //{
                //    if (!pedestrianKillTimer.Running)
                //    {
                //        pedestrianKillTimer.Start(pedestrianKillTimeSpan);
                //        pedestrianKilled = true;
                //    }
                //}
                //update timer you idiot
                if (pedestrianKilled)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            };

            stateMachine.AddState(PoliceStates.Walking, policeWalkingAction);
            stateMachine.AddState(PoliceStates.Chasing, policeChasingAction);
            stateMachine.AddState(PoliceStates.Donuting, policeDonutingAction);

            stateMachine.AddTransition(PoliceStates.Walking, PoliceStates.Chasing, policeFindPlayerTransition);
            stateMachine.AddTransition(PoliceStates.Chasing, PoliceStates.Walking, policeLosePlayerTransition);
            stateMachine.AddTransition(PoliceStates.Walking, PoliceStates.Donuting, policeFindDonutTransition);
            stateMachine.AddTransition(PoliceStates.Chasing, PoliceStates.Donuting, policeFindDonutTransition);
            stateMachine.AddTransition(PoliceStates.Donuting, PoliceStates.Walking, policeNoMoreDonutTransition);
            stateMachine.AddTransition(PoliceStates.Chasing, PoliceStates.Walking, policeKillPedestrianTransition);
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
            turnTimer.Update(gameTime.ElapsedGameTime);
            pedestrianKillTimer.Update(gameTime.ElapsedGameTime);
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
