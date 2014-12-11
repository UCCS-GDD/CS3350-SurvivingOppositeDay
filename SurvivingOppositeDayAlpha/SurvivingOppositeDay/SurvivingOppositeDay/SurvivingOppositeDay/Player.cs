using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurvivingOppositeDay
{
    public class Player: Character
    {
        public event Action<InputTypes> InputTriggeredEvent;
        KeyboardState keyboardState;
        MouseState previousMouse;
        public WeaponType weaponType;
        Timer fireTimer;
        TimeSpan fireTimeSpan;
        private bool canFire = true;
        private bool canMoveLeft = true;
        private bool canMoveRight = true;
        private bool canMoveUp = true;
        private bool canMoveDown = true;
        private int burstWaterGun;
        float moveSpeed;
        public int Health { get; set; }
        public int Ammo { get; set; }
        public static SoundEffectInstance soundEffectInstanceDonutGun;
        public static SoundEffectInstance soundEffectInstanceSlingShot;
        public static SoundEffectInstance soundEffectInstanceWaterGun;
        public static SoundEffectInstance soundEffectInstanceMachineGun;
        public static SoundEffectInstance soundEffectInstanceSniperRifle;
        public static SoundEffectInstance soundEffectInstanceGrenadeLauncher;

        public new Game1 Game { get; private set; }

        bool mmFlag = true;
        bool srFlag = true;
        bool glFlag = true;
        bool wgFlag = true;
        bool ssFlag = true;
        bool dgFlag = true;

        // icon flags
        //1 = watergun 2 = slingshot 3 = donutgun 4 = machinegun 5 = sniperrifle 6 = gernadelauncher
        public static int weaponIconFlag = 4;

        // pickups 
        public int[] pickups;

        bool canMove = true;

        public Player(Game1 game, SpriteBatch spriteBatch, Texture2D texture, Vector2 position, bool add = false)
            : base(game, spriteBatch, texture, position, add)
        {
            Ammo = 100;
            Health = 100;
            moveSpeed = 3;
            weaponType = WeaponType.MachineGun;

            // Fire Rate timer
            fireTimeSpan = TimeSpan.FromSeconds(0.25);
            fireTimer = new Timer();
            fireTimer.OnExpire += () => canFire = true;
            fireTimer.Start(fireTimeSpan);

            //set burst
            burstWaterGun = 5;

            //Sound Effects
            soundEffectInstanceDonutGun = Game1.soundDictionary["DonutGun"].CreateInstance();
            soundEffectInstanceSlingShot = Game1.soundDictionary["SlingShot"].CreateInstance();
            soundEffectInstanceWaterGun = Game1.soundDictionary["WaterGun"].CreateInstance();
            soundEffectInstanceMachineGun = Game1.soundDictionary["MachineGun"].CreateInstance();
            soundEffectInstanceSniperRifle = Game1.soundDictionary["SniperRifle"].CreateInstance();
            soundEffectInstanceGrenadeLauncher = Game1.soundDictionary["GrenadeLauncher"].CreateInstance();

            //pickups
            pickups = new int[4];

            Game = game;
        }

        public override void Update(GameTime gameTime)
        {
            keyboardState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();
            Vector2 mouseLocation = new Vector2(mouseState.X, mouseState.Y);
            Vector2 playerRotation = new Vector2(Game1.Screen.Width / 2, Game1.Screen.Height / 2);
            mouseLocation -= playerRotation;
            Rotation = (float)Math.Atan2(mouseLocation.Y, mouseLocation.X);

            // fires weapon if Fire Rate is ready
            if (canFire && Game1.clickable == true)
            {
                // check for left mouse pressed and fire event
                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    FireWeapon();
                }
            }

            previousMouse = mouseState;

            //Update timer
            fireTimer.Update(gameTime.ElapsedGameTime);

            #region Movement
            //Movement
            if (keyboardState.IsKeyDown(Keys.W))
            {
                if (Position.Y > 20)
                {
                    Position.Y -= moveSpeed;

                    // collision with top left house
                    if (Game1.roomStateMachine.Current == RoomState.MainRoom)
                    {
                        if (Position.Y <= 345 && Position.X <= 400)
                        {
                            Position.Y = 350;
                        }
                    }

                //    // checking walls of police station
                //    if (Game1.roomStateMachine.Current == RoomState.PoliceRoom)
                //    {
                //        // left entrance wall
                //        if (Position.Y <= 1145 && Position.Y >= 1100 && Position.X <= 715)
                //        {
                //            Position.Y = 1150;
                //        }

                //        // right  entrance wall
                //        if (Position.Y <= 1145 && Position.Y >= 1100 && Position.X >= 1315)
                //        {
                //            Position.Y = 1150;
                //        }

                //        // top wall
                //        if (Position.Y <= 40)
                //        {
                //            Position.Y = 45;
                //        }
                //    }

                //    // checking walls of paramedic room
                //    if (Game1.roomStateMachine.Current == RoomState.ParamedicRoom)
                //    {
                //        // top entrance wall
                //        if (Position.Y <= 725 && Position.X >= 870 && Position.X <= 930 && !keyboardState.IsKeyDown(Keys.A))
                //        {
                //            Position.Y = 730;
                //        }

                //        // top wall
                //        if (Position.Y <= 45)
                //        {
                //            Position.Y = 50;
                //        }
                //    }

                //    // checking walls of firefighter room
                //    if (Game1.roomStateMachine.Current == RoomState.FireFighterRoom)
                //    {
                //        // top entrance wall
                //        if (Position.Y <= 725 && Position.X >= 1100 && Position.X <= 1140)
                //        {
                //            Position.Y = 730;
                //        }

                //        // top wall
                //        if (Position.Y <= 45)
                //        {
                //            Position.Y = 50;
                //        }
                //    }
                }

                ////// get list of collision objects
                ////CollisionCollection colliders = Game.Colliders[Game1.roomStateMachine.Current];

                //// if player collides with an object in collision list
                ////foreach (Rectangle collider in colliders.Colliders)
                ////{
                ////    if(collider)
                ////     set player to previous position
                ////    Position = PreviousPosition;
                ////}
            }

            if (keyboardState.IsKeyDown(Keys.S))
            {
                if (Position.Y < 1980)
                {
                    Position.Y += moveSpeed;

                    // collision with top left house
                    if (Game1.roomStateMachine.Current == RoomState.MainRoom)
                    {
                        if (Position.Y >= 1655 && Position.X >= 1600)
                        {
                            Position.Y = 1650;
                        }
                    }

                //    // checking walls of police station
                //    if (Game1.roomStateMachine.Current == RoomState.PoliceRoom)
                //    {
                //        // left entrance wall 
                //        if (Position.Y <= 1145 && Position.Y >= 1090 && Position.X <= 715)
                //        {
                //            Position.Y = 1085;
                //        }

                //        // right  entrance wall
                //        if (Position.Y <= 1145 && Position.Y >= 1090 && Position.X >= 1315)
                //        {
                //            Position.Y = 1085;
                //        }
                //    }

                //    // checking walls of paramedic room
                //    if (Game1.roomStateMachine.Current == RoomState.ParamedicRoom)
                //    {
                //        // top entrance wall
                //        if (Position.Y >= 1310 && Position.X >= 870 && Position.X <= 930)
                //        {
                //            Position.Y = 1305;
                //        }
                //    }

                //    // checking walls of firefighter room
                //    if (Game1.roomStateMachine.Current == RoomState.FireFighterRoom)
                //    {
                //        // bottom entrance wall
                //        if (Position.Y >= 1310 && Position.X >= 1100 && Position.X <= 1140)
                //        {
                //            Position.Y = 1305;
                //        }
                //    }
                }
            }
            if (keyboardState.IsKeyDown(Keys.A))
            {
                if (Position.X > 20)
                {
                    Position.X -= moveSpeed;

                    // collision with top left house
                    if (Game1.roomStateMachine.Current == RoomState.MainRoom)
                    {
                        if (Position.Y <= 345 && Position.X <= 400)
                        {
                            Position.X = 405;
                        }
                    }

                //    // checking walls of police station
                //    if (Game1.roomStateMachine.Current == RoomState.PoliceRoom)
                //    {
                //        // left entrance wall
                //        if (Position.Y <= 1145 && Position.Y >= 1100 && Position.X <= 725)
                //        {
                //            Position.X = 730;
                //        }

                //        // top wall
                //        if (Position.X <= 45)
                //        {
                //            Position.X = 50;
                //        }
                //    }

                //    // checking walls of paramedic room
                //    if (Game1.roomStateMachine.Current == RoomState.ParamedicRoom)
                //    {
                //        // top entrance wall 
                //        if (Position.Y <= 730 && Position.X >= 870 && Position.X <= 930)
                //        {
                //            Position.X = 935;
                //        }

                //        // bottom  entrance wall
                //        if (Position.X >= 870 && Position.X <= 930 && Position.Y >= 1315)
                //        {
                //            Position.X = 935;
                //        }
                //    }

                //    // checking walls of firefighter room
                //    if (Game1.roomStateMachine.Current == RoomState.FireFighterRoom)
                //    {
                //        // top entrance wall 
                //        if (Position.Y <= 730 && Position.X >= 1100 && Position.X <= 1140)
                //        {
                //            Position.X = 1145;
                //        }

                //        // bottom  entrance wall
                //        if (Position.X >= 1100 && Position.X <= 1140 && Position.Y >= 1315)
                //        {
                //            Position.X = 1145;
                //        }

                //        // back wall
                //        if (Position.X <= 45)
                //        {
                //            Position.X = 50;
                //        }
                //    }
                }
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                if (Position.X < 1980)
                {
                    Position.X += moveSpeed;

                    // collision with top left house
                    if (Game1.roomStateMachine.Current == RoomState.MainRoom)
                    {
                        if (Position.Y >= 1655 && Position.X >= 1600)
                        {
                            Position.X = 1595;
                        }
                    }

                //    // checking walls of police station
                //    if (Game1.roomStateMachine.Current == RoomState.PoliceRoom)
                //    {
                //        // left entrance wall
                //        if (Position.Y <= 1145 && Position.Y >= 1100 && Position.X >= 1310)
                //        {
                //            Position.X = 1305;
                //        }
                //    }

                //    // checking walls of paramedic room
                //    if (Game1.roomStateMachine.Current == RoomState.ParamedicRoom)
                //    {
                //        // top entrance wall 
                //        if (Position.Y <= 730 && Position.X >= 870 && Position.X <= 930)
                //        {
                //            Position.X = 865;
                //        }

                //        // bottom  entrance wall
                //        if (Position.X >= 870 && Position.X <= 930 && Position.Y >= 1315)
                //        {
                //            Position.X = 865;
                //        }
                //    }

                //    // checking walls of firefighter room
                //    if (Game1.roomStateMachine.Current == RoomState.FireFighterRoom)
                //    {
                //        // top entrance wall 
                //        if (Position.Y <= 730 && Position.X >= 1090 && Position.X <= 1140)
                //        {
                //            Position.X = 1085;
                //        }

                //        // bottom  entrance wall
                //        if (Position.X >= 1090 && Position.X <= 1140 && Position.Y >= 1315)
                //        {
                //            Position.X = 1085;
                //        }
                //    }
                }
            }
            #endregion
            #region Change Weapon
            ////Change weapon
            //if (keyboardState.IsKeyDown(Keys.D1))
            //{
            //    weaponType = WeaponType.WaterGun;
            //    soundEffectInstanceWaterGun.Play();
            //    wgiFlag = true;
            //    ssiFlag = false;
            //    dgiFlag = false;
            //}
            //if (keyboardState.IsKeyDown(Keys.D2))
            //{
            //    weaponType = WeaponType.SlingShot;
            //    soundEffectInstanceSlingShot.Play();
            //    wgiFlag = false;
            //    ssiFlag = true;
            //    dgiFlag = false;
            //}
            //if (keyboardState.IsKeyDown(Keys.D3))
            //{
            //    weaponType = WeaponType.DonutGun;
            //    soundEffectInstanceDonutGun.Play();
            //    wgiFlag = false;
            //    ssiFlag = false;
            //    dgiFlag = true;
            //}
            //if (keyboardState.IsKeyDown(Keys.D4))
            //{
            //    weaponType = WeaponType.MachineGun;
            //}
            //if (keyboardState.IsKeyDown(Keys.D5))
            //{
            //    weaponType = WeaponType.SniperRifle;
            //}
            //if (keyboardState.IsKeyDown(Keys.D6))
            //{
            //    weaponType = WeaponType.GernadeLauncher;
            //}

            //Change weapon
            if (keyboardState.IsKeyDown(Keys.D1))
            {
                if (this.pickups[1] == 1)
                {
                    weaponType = WeaponType.WaterGun;
                    if (wgFlag == true)
                    {
                        soundEffectInstanceWaterGun.Play();
                        wgFlag = false;
                    }
                    weaponIconFlag = 1;
                }
                else
                {
                    weaponType = WeaponType.MachineGun;
                    if (mmFlag == true)
                    {
                        soundEffectInstanceMachineGun.Play();
                        mmFlag = false;
                    }
                    weaponIconFlag = 4;
                }
                srFlag = true;
                glFlag = true;
                ssFlag = true;
                dgFlag = true;

            }
            if (keyboardState.IsKeyDown(Keys.D2))
            {
                if (this.pickups[2] == 1)
                {
                    weaponType = WeaponType.SlingShot;
                    if (ssFlag == true)
                    {
                        soundEffectInstanceSlingShot.Play();
                        ssFlag = false;
                    }
                    weaponIconFlag = 2;
                }
                else
                {
                    weaponType = WeaponType.SniperRifle;
                    if (srFlag == true)
                    {
                        soundEffectInstanceSniperRifle.Play();
                        srFlag = false;
                    }
                    weaponIconFlag = 5;
                }
                mmFlag = true;
                glFlag = true;
                wgFlag = true;
                dgFlag = true;
            }
            if (keyboardState.IsKeyDown(Keys.D3))
            {
                if (this.pickups[3] == 1)
                {
                    weaponType = WeaponType.DonutGun;
                    if (dgFlag == true)
                    {
                        soundEffectInstanceDonutGun.Play();
                        dgFlag = false;
                    }
                    weaponIconFlag = 3;
                }
                else
                {
                    weaponType = WeaponType.GernadeLauncher;
                    if (glFlag == true)
                    {
                        soundEffectInstanceGrenadeLauncher.Play();
                        glFlag = false;
                    }
                    weaponIconFlag = 6;
                }
                mmFlag = true;
                srFlag = true;
                wgFlag = true;
                ssFlag = true;

            }
            #endregion

            base.Update(gameTime);
        }

        //public override void Draw(GameTime gameTime)
        //{
        //    Position = new Vector2(Game1.Screen.Width / 2 - this.Texture.Width / 2, Game1.Screen.Height / 2 - this.Texture.Height / 2);

        //    base.Draw(gameTime);
        //}

        private void FireWeapon()
        {
            #region WaterGun - Fire
            //WaterGun fire
            if (weaponType == WeaponType.WaterGun)
            {
                if(Ammo > 0)
                {
                    if (InputTriggeredEvent != null)
                    {
                        InputTriggeredEvent(InputTypes.LeftMouse);
                        canFire = false;

                        //lower ammo
                        Ammo -= 1;
                        burstWaterGun -= 1;

                        //Sets burst fire rate
                        if (weaponType == WeaponType.WaterGun)
                        {
                            if (burstWaterGun > 0)
                            {
                                if (!fireTimer.Running)
                                {
                                    //burst fire rate
                                    fireTimeSpan = TimeSpan.FromSeconds(0.05);
                                }
                            }
                            else
                            {
                                if (!fireTimer.Running)
                                {
                                    //reload fire rate
                                    fireTimeSpan = TimeSpan.FromSeconds(1);
                                    burstWaterGun = 5;
                                }
                            }
                        }
                    }
                }
            }
            #endregion
            #region SlingShot - Fire
            //SlingShot fire
            if (weaponType == WeaponType.SlingShot)
            {
                if (Ammo >= 5)
                {
                    if (InputTriggeredEvent != null)
                    {
                        InputTriggeredEvent(InputTypes.LeftMouse);
                        canFire = false;

                        //sets fire rate
                        fireTimeSpan = TimeSpan.FromSeconds(1);

                        //lower ammo
                        Ammo -= 5;
                    }
                }
            }
            #endregion
            #region DonutGun - Fire
            //DonutGun Fire
            if (weaponType == WeaponType.DonutGun)
            {
                if (Ammo >= 10)
                {
                    if (InputTriggeredEvent != null)
                    {
                        InputTriggeredEvent(InputTypes.LeftMouse);
                        canFire = false;

                        //sets fire rate
                        fireTimeSpan = TimeSpan.FromSeconds(2);

                        //lower ammo
                        Ammo -= 10;
                    }
                }
            }
            #endregion
            #region MachineGun - Fire
            if (weaponType == WeaponType.MachineGun)
            {
                if (Ammo > 0)
                {
                    if (InputTriggeredEvent != null)
                    {
                        InputTriggeredEvent(InputTypes.LeftMouse);
                        canFire = false;

                        //lower ammo
                        Ammo -= 1;
                        burstWaterGun -= 1;

                        //Sets burst fire rate
                        if (weaponType == WeaponType.MachineGun)
                        {
                            if (burstWaterGun > 0)
                            {
                                if (!fireTimer.Running)
                                {
                                    //burst fire rate
                                    fireTimeSpan = TimeSpan.FromSeconds(0.05);
                                }
                            }
                            else
                            {
                                if (!fireTimer.Running)
                                {
                                    //reload fire rate
                                    fireTimeSpan = TimeSpan.FromSeconds(1);
                                    burstWaterGun = 5;
                                }
                            }
                        }
                    }
                }
            }
            #endregion
            #region SniperRifle - Fire
            //SlingShot fire
            if (weaponType == WeaponType.SniperRifle)
            {
                if (Ammo >= 5)
                {
                    if (InputTriggeredEvent != null)
                    {
                        InputTriggeredEvent(InputTypes.LeftMouse);
                        canFire = false;

                        //sets fire rate
                        fireTimeSpan = TimeSpan.FromSeconds(1);

                        //lower ammo
                        Ammo -= 5;
                    }
                }
            }
            #endregion
            #region GernadeLauncher - Fire
            //DonutGun Fire
            if (weaponType == WeaponType.GernadeLauncher)
            {
                if (Ammo >= 10)
                {
                    if (InputTriggeredEvent != null)
                    {
                        InputTriggeredEvent(InputTypes.LeftMouse);
                        canFire = false;

                        //sets fire rate
                        fireTimeSpan = TimeSpan.FromSeconds(2);

                        //lower ammo
                        Ammo -= 10;
                    }
                }
            }
            #endregion
            //start timer
            fireTimer.Start(fireTimeSpan);
        }
    }

    public enum InputTypes { LeftMouse }
    public enum WeaponType { WaterGun, SlingShot, DonutGun, MachineGun, SniperRifle, GernadeLauncher}
}
