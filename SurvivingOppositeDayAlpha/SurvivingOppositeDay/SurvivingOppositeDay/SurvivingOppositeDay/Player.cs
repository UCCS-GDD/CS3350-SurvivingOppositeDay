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
        SoundEffectInstance soundEffectInstanceDonutGun;
        SoundEffectInstance soundEffectInstanceSlingShot;
        SoundEffectInstance soundEffectInstanceWaterGun;
        //SoundEffectInstance soundEffectInstanceGernadeLauncher;
        //SoundEffectInstance soundEffectInstanceSniperRifle;
        //SoundEffectInstance soundEffectInstanceMachineGun;

        // icon flags
        //1 = watergun 2 = slingshot 3 = donutgun 4 = machinegun 5 = sniperrifle 6 = gernadelauncher
        public static int weaponIconFlag = 4;


        // pickups 
        public int[] pickups;

        public Player(Game game, SpriteBatch spriteBatch, Texture2D texture, Vector2 position, bool add = false)
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
            //soundEffectInstanceGernadeLauncher = Game1.soundDictionary["GernadeLauncher"].CreateInstance();
            //soundEffectInstanceSniperRifle = Game1.soundDictionary["SniperRifle"].CreateInstance();
            //soundEffectInstanceMachineGun = Game1.soundDictionary["MachineGun"].CreateInstance();

            //pickups
            pickups = new int[4];
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
            if (canFire)
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
                }
            }

            if (keyboardState.IsKeyDown(Keys.S))
            {
                if (Position.Y < 1980)
                {
                    Position.Y += moveSpeed;
                }
            }
            if (keyboardState.IsKeyDown(Keys.A))
            {
                if (Position.X > 20)
                {
                    Position.X -= moveSpeed;
                }
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                if (Position.X < 1980)
                {
                    Position.X += moveSpeed;
                }
            }
            #endregion
            #region Change Weapon
            //Change weapon
            if (keyboardState.IsKeyDown(Keys.D1))
            {
                if (this.pickups[1] == 1)
                {
                    weaponType = WeaponType.WaterGun;
                    soundEffectInstanceWaterGun.Play();
                    weaponIconFlag = 1;
                }
                else
                {
                weaponType = WeaponType.MachineGun;
                //soundEffectInstanceMachineGun.Play();
                weaponIconFlag = 4;
                }
                
            }
            if (keyboardState.IsKeyDown(Keys.D2))
            {
                if (this.pickups[2] == 1)
                {
                    weaponType = WeaponType.SlingShot;
                    soundEffectInstanceSlingShot.Play();
                    weaponIconFlag = 2;
                }
                else
                {
                    weaponType = WeaponType.SniperRifle;
                    //soundEffectInstanceSniperRifle.Play();
                    weaponIconFlag = 5;
                }
            }
            if (keyboardState.IsKeyDown(Keys.D3))
            {
                if (this.pickups[3] == 1)
                {
                    weaponType = WeaponType.DonutGun;
                    soundEffectInstanceDonutGun.Play();
                    weaponIconFlag = 3;
                }
                else
                {
                    weaponType = WeaponType.GernadeLauncher;
                    //soundEffectInstanceGernadeLauncher.Play();
                    weaponIconFlag = 6;
                }
                
            }
            if (keyboardState.IsKeyDown(Keys.D4))
            {
                weaponType = WeaponType.MachineGun;
            }
            if (keyboardState.IsKeyDown(Keys.D5))
            {
                weaponType = WeaponType.SniperRifle;
            }
            if (keyboardState.IsKeyDown(Keys.D6))
            {
                weaponType = WeaponType.GernadeLauncher;
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
