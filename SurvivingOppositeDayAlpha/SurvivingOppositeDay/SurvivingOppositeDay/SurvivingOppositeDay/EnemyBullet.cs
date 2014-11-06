using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurvivingOppositeDay
{
    class EnemyBullet: BasicBullet
    {
        public BulletType bulletType;

        public EnemyBullet(Game game, SpriteBatch spriteBatch, Texture2D texture, Vector2 position, bool add = false)
            : base(game, spriteBatch, texture, position, add)
        {
           
        }
    }
    public enum BulletType { FireBall, Bullet}
}
