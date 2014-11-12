using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurvivingOppositeDay
{
    public class Animation
    {
        Rectangle rectangle;
        public Rectangle AnimationRectangle { get { return rectangle; } }
        public Texture2D Texture { get; private set; }
        public int NumberOfFrames { get; private set; }
        public int CurrentFrame { get; private set; }

        public Animation(Texture2D texture, uint numberOfFrames)
        {
            Texture = texture;
            NumberOfFrames = (int)numberOfFrames;
            SetRectangle();
        }

        public void MoveRight(bool loop = false)
        {
            if (CurrentFrame == NumberOfFrames - 1)
            {
                if (loop)
                {
                    rectangle.X = 0;
                    CurrentFrame = 0;
                }
            }
            else
            {
                rectangle.X += rectangle.Width;
                CurrentFrame++;
            }
        }

        public void MoveLeft(bool loop = false)
        {
            if (CurrentFrame == 0)
            {
                if (loop)
                {
                    rectangle.X = rectangle.Width * (NumberOfFrames - 1);
                    CurrentFrame = NumberOfFrames - 1;
                }
            }
            else
            {
                rectangle.X -= rectangle.Width;
                CurrentFrame--;
            }
        }

        public void JumpToFrame(int frame)
        {
            if (frame < 0)
            {
                // out of bounds on left, snap to frame 0
                rectangle.X = 0;
                CurrentFrame = 0;
            }
            else if (frame > NumberOfFrames - 1)
            {
                // out of bounds on right, snap to last frame
                rectangle.X = rectangle.Width * (NumberOfFrames - 1);
                CurrentFrame = NumberOfFrames - 1;
            }
            else
            {
                // not out of bounds, snap to parameter frame
                rectangle.X = rectangle.Width * frame;
                CurrentFrame = frame;
            }
        }

        private void SetRectangle()
        {
            rectangle.Width = Texture.Width / NumberOfFrames;
            rectangle.Height = Texture.Height;
        }
    }
}
