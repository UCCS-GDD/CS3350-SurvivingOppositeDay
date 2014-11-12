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
        WeaponType weaponType;
        Timer fireTimer;
        TimeSpan fireTimeSpan;
        private bool canFire = true;
        float moveSpeed;
        public int Health { get; set; }

        public Player(Game game, SpriteBatch spriteBatch, Texture2D texture, Vector2 position, bool add = false)
            : base(game, spriteBatch, texture, position, add)
        {
            Health = 100;
            moveSpeed = 3;
            weaponType = WeaponType.WaterGun;

            // Fire Rate timer
            fireTimeSpan = TimeSpan.FromSeconds(0.25);
            fireTimer = new Timer();
            fireTimer.OnExpire += () => canFire = true;
            fireTimer.Start(fireTimeSpan);
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


            base.Update(gameTime);
        }

        private void FireWeapon()
        {
            if (weaponType == WeaponType.WaterGun)
            {
                if (InputTriggeredEvent != null)
                {
                    InputTriggeredEvent(InputTypes.LeftMouse);
                    canFire = false;

                    //start timer
                    fireTimer.Start(fireTimeSpan);
                }
            }
        }
    }

    public enum InputTypes { LeftMouse }
    public enum WeaponType { WaterGun, SlingShot, DonutGun, MachineGun, SniperRifle, GernadeLauncher}
}
