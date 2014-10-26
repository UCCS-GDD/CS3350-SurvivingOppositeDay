using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace SurvivingOppositeDay
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class UserControlledSprite : Sprite
    {
        int speed;
        
        public UserControlledSprite(Game game, String textureFile, Vector2 position, int speed) : base(game,textureFile,position)
        {
            this.textureFile = textureFile;
            this.position = position;
            this.speed = speed;
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.A))
            {
                position.X -= speed;
            }
            else if (keyboardState.IsKeyDown(Keys.D))
            {
                position.X += speed;
            }
            else if (keyboardState.IsKeyDown(Keys.W))
            {
                position.Y -= speed;
            }
            else if (keyboardState.IsKeyDown(Keys.S))
            {
                position.Y += speed;
            }

            base.Update(gameTime);
        }
    }
}
