using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SurvivingOppositeDay
{
    class UserControlledSprite : Sprite
    {
        public UserControlledSprite(Texture2D texture, Vector2 position, Point frameSize, int collisionOffset, Point currentFrame, Point sheetSize, Vector2 speed)
            : base(texture, position, frameSize, collisionOffset, currentFrame, sheetSize, speed)
        {

        }

        public UserControlledSprite(Texture2D texture, Vector2 position, Point frameSize, int collisionOffset, Point currentFrame, Point sheetSize, Vector2 speed, int millisecondsPerFrame)
            : base(texture, position, frameSize, collisionOffset, currentFrame, sheetSize, speed, millisecondsPerFrame)
        {

        }

        public override Vector2 direction
        {
            get
            {
                Vector2 inputDirection = Vector2.Zero;

                if (Keyboard.GetState().IsKeyDown(Keys.A))
                    inputDirection.X -= 1;
                if (Keyboard.GetState().IsKeyDown(Keys.D))
                    inputDirection.X += 1;
                if (Keyboard.GetState().IsKeyDown(Keys.W))
                    inputDirection.Y -= 1;
                if (Keyboard.GetState().IsKeyDown(Keys.S))
                    inputDirection.Y += 1;
                return inputDirection * speed;
            }
        }

        public override void Update(GameTime gameTime, Rectangle clientBounds)
        {
            position += direction;
            MouseState mouseState = Mouse.GetState();
            Vector2 mouseLocation = new Vector2(mouseState.X, mouseState.Y);
            mouseLocation -= position;
            angle = (float)Math.Atan2(mouseLocation.Y, mouseLocation.X);
            base.Update(gameTime, clientBounds);
        }
    }
}
