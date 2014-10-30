using Microsoft.Xna.Framework;
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
        private int burstWaterGun;
        float moveSpeed;

        public int Ammo { get; set; }

        public Player(Game game, SpriteBatch spriteBatch, Texture2D texture, Vector2 position, bool add = false)
            : base(game, spriteBatch, texture, position, add)
        {
            Ammo = 100;
            Health = 100;
            moveSpeed = 3;
            weaponType = WeaponType.WaterGun;

            // Fire Rate timer
            fireTimeSpan = TimeSpan.FromSeconds(0.25);
            fireTimer = new Timer();
            fireTimer.OnExpire += () => canFire = true;
            fireTimer.Start(fireTimeSpan);

            //set burst
            burstWaterGun = 5;
        }

        public override void Update(GameTime gameTime)
        {
            keyboardState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();
            Vector2 mouseLocation = new Vector2(mouseState.X, mouseState.Y);
            mouseLocation -= Position;
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
                Position.Y -= moveSpeed;
            }
            if (keyboardState.IsKeyDown(Keys.S))
            {
                Position.Y += moveSpeed;
            }
            if (keyboardState.IsKeyDown(Keys.A))
            {
                Position.X -= moveSpeed;
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                Position.X += moveSpeed;
            }
            #endregion
            #region Change Weapon
            //Change weapon
            if (keyboardState.IsKeyDown(Keys.D1))
            {
                weaponType = WeaponType.WaterGun;
            }
            if (keyboardState.IsKeyDown(Keys.D2))
            {
                weaponType = WeaponType.SlingShot;
            }
            if (keyboardState.IsKeyDown(Keys.D3))
            {
                weaponType = WeaponType.DonutGun;
            }
            #endregion

            base.Update(gameTime);
        }

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
            //start timer
            fireTimer.Start(fireTimeSpan);
        }
    }

    public enum InputTypes { LeftMouse }
    public enum WeaponType { WaterGun, SlingShot, DonutGun, MachineGun, SniperRifle, GernadeLauncher}
}
