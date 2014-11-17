using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurvivingOppositeDay
{
    public class BasicSprite: DrawableGameComponent
    {
        public SpriteBatch SpriteBatch { get; private set; }
        public Color Color { get; set; }
        public Texture2D Texture { get; set; }
        //public Vector2 Position { get; set; }
        public Vector2 Position;

        public Vector2 PreviousPosition { get; private set; }
        public Rectangle AnimationRectangle { get; set; }
        public float Rotation { get; set; }
        public Vector2 Origin { get; set; }
        public Vector2 Scale { get; set; }
        public SpriteEffects SpriteEffects { get; set; }
        public float LayerDepth { get; set; }
        public float LinearVelocity { get; set; }
        public float RotationalVelocity { get; set; }
        public float LinearAcceleration { get; set; }
        public float RotationalAcceleration { get; set; }
        public bool Remove { get; set; }

        public new Game1 Game { get; private set; }

        public Rectangle collisionRectangle;
        

        public BasicSprite(Game game, SpriteBatch spriteBatch, Texture2D texture, Vector2 position, bool add = false)
            : base(game)
        {
            // auto-add functionality
            if(add)
                game.Components.Add(this);

            SpriteBatch = spriteBatch;
            Color = Color.White;
            Texture = texture;
            Position = position;
            PreviousPosition = position;
            AnimationRectangle = Texture.Bounds;
            Origin = Tools.Math.Vectors.FromPoint(Texture.Bounds.Center);
            Scale = Vector2.One;

            collisionRectangle = new Rectangle((int)Position.X - (Texture.Width/2), (int)Position.Y - (Texture.Height/2), Texture.Width, Texture.Height);

            Game = base.Game as Game1;
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch.Draw(Texture, Position, AnimationRectangle, Color, Rotation, Origin, Scale, SpriteEffects, LayerDepth);
            //SpriteBatch.Draw(Game1.TestTexture, collisionRectangle, Color.White);
            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            PreviousPosition = Position;

            float deltaT = gameTime.ElapsedGameTime.Milliseconds;

            RotationalVelocity += RotationalAcceleration * deltaT;
            Rotation += RotationalVelocity * deltaT;

            LinearVelocity += LinearAcceleration * deltaT;
            Position += Tools.Math.Vectors.FromTrig(Rotation, LinearVelocity);


            collisionRectangle.X = (int)Position.X - (Texture.Width / 2);
            collisionRectangle.Y = (int)Position.Y - (Texture.Height/2);

            base.Update(gameTime);
        }
    }
}
