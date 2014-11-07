using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurvivingOppositeDay
{
    class Camera
    {
        public Matrix transform;
        Viewport view;
        Vector2 center;

        public Camera(Viewport newView)
        {
            view = newView;
        }

        public void Update(GameTime gameTime, Player player)
        {
            center = new Vector2(player.Position.X + (player.collisionRectangle.Width / 2) - 400, player.Position.Y + (player.collisionRectangle.Height) - 300);
            transform = Matrix.CreateScale(new Vector3(1, 1, 0)) * Matrix.CreateTranslation(-center.X, -center.Y, 0);
            
        }

        //public void Update(Vector2 cameraPos)
        //{
        //    cameraPos = MathHelper.Clamp(cameraPos.X, P)
        //}
    }
}
