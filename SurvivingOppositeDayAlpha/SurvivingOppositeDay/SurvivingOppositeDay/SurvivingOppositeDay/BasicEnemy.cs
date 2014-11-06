using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurvivingOppositeDay
{
    class BasicEnemy: Character
    {
        public Vector2 playerPosition;
        public int Health { get; set; }
        public EnemyType enemyType;
        public Vector2 PositionDifference { get; set; }

        public BasicEnemy(Game game, SpriteBatch spriteBatch, Texture2D texture, Vector2 position, bool add = false)
            : base(game, spriteBatch, texture, position, add)
        {
            //LinearVelocity = 1;
            Health = 100;
        }

        public override void Update(GameTime gameTime)
        {
            //playerPosition -= Position;
            Rotation = (float)Math.Atan2(playerPosition.Y, playerPosition.X);
            PositionDifference = Position - playerPosition;

            //KeyboardState keyboard = Keyboard.GetState();
            
            //if (keyboard.IsKeyDown(Keys.A))
            //{
            //    Position.X += 1;
            //}
            base.Update(gameTime);
        }

        public void GetPlayerPosition(Vector2 position)
        {
            playerPosition = position;
        }
    }
    public enum EnemyAction { FireBullet }
    public enum EnemyType { FireFighter, Paramedic, Police}
}
