using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurvivingOppositeDay
{
    public class Character:BasicSprite 
    {
        public Dictionary<string, Animation> Animations { get; private set; }
        Animation currentAnimation;
        Texture2D initialTexture;
        public int Health { get; set; }
        public Character(Game game, SpriteBatch spriteBatch, Texture2D texture, Vector2 position, bool add = false)
            : base(game, spriteBatch, texture, position, add)
        {
            Animations = new Dictionary<string, Animation>();
            initialTexture = texture;
        }

        public override void Draw(GameTime gameTime)
        {
            
                if (currentAnimation != null)
                {
                    AnimationRectangle = currentAnimation.AnimationRectangle;
                    Texture = currentAnimation.Texture;
                }
                else
                {
                    Texture = initialTexture;
                    AnimationRectangle = initialTexture.Bounds;
                }
            
            base.Draw(gameTime);
        }

        public void SetAnimation(string name)
        {
            currentAnimation = Animations[name];
        }
    }
}
