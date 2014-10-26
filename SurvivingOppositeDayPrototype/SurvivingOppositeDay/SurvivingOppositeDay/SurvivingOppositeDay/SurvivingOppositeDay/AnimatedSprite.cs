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
    public class AnimatedSprite : Sprite
    {
        Point frameSize;
        Point currentFrame;
        Point sheetSize;

        public AnimatedSprite(Game game, String textureFile, Vector2 position, Point frameSize, 
            Point currentFrame, Point sheetSize) : base(game, textureFile, position)
        {
            this.textureFile = textureFile;
            this.position = position;
            this.frameSize = frameSize;
            this.currentFrame = currentFrame;
            this.sheetSize = sheetSize;
        }   

        protected override void LoadContent()
        {
            texture = Game.Content.Load<Texture2D>(textureFile);
        }

        public override void Update(GameTime gameTime)
        {
            ++currentFrame.X;
            if (currentFrame.X >= sheetSize.X)
            {
                currentFrame.X = 0;
                ++currentFrame.Y;
                if (currentFrame.Y >= sheetSize.Y)
                    currentFrame.Y = 0;
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch sp = ((Game1)this.Game).spriteBatch;
            sp.Begin();
            sp.Draw(texture, position, new Rectangle(currentFrame.X * frameSize.X, currentFrame.Y * frameSize.Y, frameSize.X, frameSize.Y), color);
            sp.End();

            base.Draw(gameTime);
        }
    }
}
