using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurvivingOppositeDay
{
    class Explosion : BasicSprite
    {
        Animation animation;
        Texture2D initialTexture;
        public int Frames { get; set; }
        Timer frameTimer;
        TimeSpan frameTimeSpan;
        private bool nextFrame = true;

        public Explosion(Game game, SpriteBatch spriteBatch, Texture2D texture, Vector2 position, int Frames, bool add = false)
            : base(game, spriteBatch, texture, position, add)
        {
            IEnumerable<BasicEnemy> enemies = Game.Components.OfType<BasicEnemy>();

            animation = new Animation(texture, Frames);

            Origin = Tools.Math.Vectors.FromPoint(animation.AnimationRectangle.Center);
            initialTexture = texture;

            // Frame timer
            frameTimeSpan = TimeSpan.FromSeconds(0.1);
            frameTimer = new Timer();
            frameTimer.OnExpire += () => nextFrame = true;
            frameTimer.Start(frameTimeSpan);

            //frame 1
            animation.OnFrameAdvance += frame =>
            {
                if (frame == 1)
                {
                    foreach (BasicEnemy enemy in enemies)
                    {
                        if (enemy.Enabled && this.Enabled && enemy.collisionRectangle.Intersects(this.collisionRectangle))
                        {
                            enemy.Health -= 10;
                            if (enemy.Health <= 0)
                            {
                                Game1.score += 100;
                                enemy.Enabled = false;
                                enemy.Remove = true;
                            }
                        }
                    }
                }
            };

            //frame 2
            animation.OnFrameAdvance += frame =>
            {
                if (frame == 2)
                {
                    foreach (BasicEnemy enemy in enemies)
                    {
                        if (enemy.Enabled && this.Enabled && enemy.collisionRectangle.Intersects(this.collisionRectangle))
                        {
                            enemy.Health -= 10;
                            if (enemy.Health <= 0)
                            {
                                Game1.score += 100;
                                enemy.Enabled = false;
                                enemy.Remove = true;
                            }
                        }
                    }
                }
            };

            animation.OnFrameAdvance += frame => 
            { 
                if (frame == 3) 
                { 
                    foreach (BasicEnemy enemy in enemies) 
                    {  
                        if(enemy.Enabled && this.Enabled && enemy.collisionRectangle.Intersects(this.collisionRectangle)) 
                        {  
                            enemy.Health -= 10;
                            if (enemy.Health <= 0)
                            {
                                Game1.score += 100;
                                enemy.Enabled = false;
                                enemy.Remove = true;
                            }
                        }
                    } 
                } 
            };

            //frame 4
            animation.OnFrameAdvance += frame =>
            {
                if (frame == 4)
                {
                    foreach (BasicEnemy enemy in enemies)
                    {
                        if (enemy.Enabled && this.Enabled && enemy.collisionRectangle.Intersects(this.collisionRectangle))
                        {
                            enemy.Health -= 10;
                            if (enemy.Health <= 0)
                            {
                                Game1.score += 100;
                                enemy.Enabled = false;
                                enemy.Remove = true;
                            }
                        }
                    }
                }
            };

            //frame 5
            animation.OnFrameAdvance += frame =>
            {
                if (frame == 5)
                {
                    foreach (BasicEnemy enemy in enemies)
                    {
                        if (enemy.Enabled && this.Enabled && enemy.collisionRectangle.Intersects(this.collisionRectangle))
                        {
                            enemy.Health -= 10;
                            if (enemy.Health <= 0)
                            {
                                Game1.score += 100;
                                enemy.Enabled = false;
                                enemy.Remove = true;
                            }
                        }
                    }
                }
            };

            //frame 6
            animation.OnFrameAdvance += frame =>
            {
                if (frame == 6)
                {
                    foreach (BasicEnemy enemy in enemies)
                    {
                        if (enemy.Enabled && this.Enabled && enemy.collisionRectangle.Intersects(this.collisionRectangle))
                        {
                            enemy.Health -= 10;
                            if (enemy.Health <= 0)
                            {
                                Game1.score += 100;
                                enemy.Enabled = false;
                                enemy.Remove = true;
                            }
                        }
                    }
                }
            };

            //frame 7
            animation.OnFrameAdvance += frame =>
            {
                if (frame == 7)
                {
                    foreach (BasicEnemy enemy in enemies)
                    {
                        if (enemy.Enabled && this.Enabled && enemy.collisionRectangle.Intersects(this.collisionRectangle))
                        {
                            enemy.Health -= 10;
                            if (enemy.Health <= 0)
                            {
                                Game1.score += 100;
                                enemy.Enabled = false;
                                enemy.Remove = true;
                            }
                        }
                    }
                }
            };
        }

        public override void Update(GameTime gameTime)
        {
            if (nextFrame)
            {
                if (animation.AnimationEnd())
                {
                    Remove = true;
                }
                else
                {
                    animation.MoveRight();
                    nextFrame = false;
                    frameTimer.Start(frameTimeSpan);
                }
            }

            AnimationRectangle = animation.AnimationRectangle;
            frameTimer.Update(gameTime.ElapsedGameTime);
        }

        public override void Draw(GameTime gameTime)
        {

            if (animation != null)
            {
                AnimationRectangle = animation.AnimationRectangle;
                Texture = animation.Texture;
            }
            else
            {
                Texture = initialTexture;
                AnimationRectangle = initialTexture.Bounds;
            }

            base.Draw(gameTime);
        }
    }
}
