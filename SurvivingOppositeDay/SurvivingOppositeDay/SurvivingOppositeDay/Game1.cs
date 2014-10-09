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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

// Surviving Opposite Day
// Collin Wilson
// Cary McDavid
// Thai Tao
// Bruno Andrade
// Roby Beamer

namespace SurvivingOppositeDay
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private SpriteDictionary spriteDictionary;
        public static Rectangle Screen { get; private set; }

        // player
        Player player;

        //Enemy
        BasicEnemy enemy;
        BasicEnemy enemy2;
        BasicEnemy enemy3;
        BasicEnemy enemy4;
        BasicEnemy enemy5;
        
        //Enemy Counters
        int enemyCounter;
        int enemy2Counter;
        int enemy3Counter;
        int enemy4Counter;
        int enemy5Counter;

        //Score 
        SpriteFont scoreFont;
        int score;
        const string SCORE_STRING = "Score: ";
        const int TEXT_OFFSET = 550;
        string scoreText;
        Vector2 scoreTextLocation;

        // for testing
        //BasicSprite example;
        //Animation animation;
        //KeyboardState previous;
        //KeyboardState current;

        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
                        
            // set up static screen access
            Screen = new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            base.Initialize();

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // set up spriteDictionary
            spriteDictionary = new SpriteDictionary(Content);
            spriteDictionary.Add("exampleSprite", "Sprite/explosion");
            spriteDictionary.Add("player", "Sprite/player");
            spriteDictionary.Add("waterBullet", "Sprite/waterBullet");

            // player
            player = new Player(this, spriteBatch, spriteDictionary["player"], Tools.Math.Vectors.FromPoint(Screen.Center));
            Components.Add(player);
            player.InputTriggeredEvent += SpawnBullet;

            //enemies
            enemy = new BasicEnemy(this, spriteBatch, spriteDictionary["player"], new Vector2(Screen.Width / 2, 0));
            enemy2 = new BasicEnemy(this, spriteBatch, spriteDictionary["player"], new Vector2(Screen.Width, 0));
            enemy3 = new BasicEnemy(this, spriteBatch, spriteDictionary["player"], new Vector2(Screen.Width * 0.25f, 0));
            enemy4 = new BasicEnemy(this, spriteBatch, spriteDictionary["player"], new Vector2(Screen.Width / 3, 0));
            enemy5 = new BasicEnemy(this, spriteBatch, spriteDictionary["player"], new Vector2(Screen.Width / 1.5f, 0));
            Components.Add(enemy);
            Components.Add(enemy2);
            Components.Add(enemy3);
            Components.Add(enemy4);
            Components.Add(enemy5);

            //score stuff
            scoreFont = Content.Load<SpriteFont>("Arial");
            score = 0;
            scoreText = SCORE_STRING + score;
            scoreTextLocation = new Vector2(TEXT_OFFSET, 20);

            // Example 
            //example = new BasicSprite(this, spriteBatch, spriteDictionary["exampleSprite"], Tools.Math.Vectors.FromPoint(Screen.Center));
            //Components.Add(example);
            //animation = new Animation(spriteDictionary["exampleSprite"], 5);
            //enemy.Origin = Tools.Math.Vectors.FromPoint(animation.AnimationRectangle.Center);
            //enemy.Texture = animation.Texture;
            //TestAnimation();

            // Test destruction of outside screen items
            //Screen = new Rectangle(100, 100, 600, 400);

            //Components.OfType<BasicBullet>().ToList().ForEach(bullet => bullet.Position = Vector2.Zer0);
        }

        private void SpawnBullet(InputTypes inputEvent)
        {
            if (inputEvent == InputTypes.LeftMouse)
            {
                // Spawn Bullet
                BasicBullet bullet = new BasicBullet(this, spriteBatch, spriteDictionary["waterBullet"], player.Position);
                bullet.SetDirection(player.Rotation, 5);
                Components.Add(bullet);
            }
        }

        // remove later, for testing
        private void TestAnimation()
        {
            // update animation - probably with a timer - for now with keyboard
            //current = Keyboard.GetState();
            //if (current.IsKeyDown(Keys.Left) && previous.IsKeyUp(Keys.Left))
            //{
            //    animation.MoveLeft(true);
            //}
            //if (current.IsKeyDown(Keys.Right) && previous.IsKeyUp(Keys.Right))
            //{
            //    animation.MoveRight();
            //}

            //enemy.AnimationRectangle = animation.AnimationRectangle;

            //previous = current;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            //TestAnimation();

            //Give enemy player position
            enemy.GetPlayerPosition(player.Position);
            enemy2.GetPlayerPosition(player.Position);
            enemy3.GetPlayerPosition(player.Position);
            enemy4.GetPlayerPosition(player.Position);
            enemy5.GetPlayerPosition(player.Position);

            //check for Collisions
            foreach (BasicBullet bullet in Components.OfType<BasicBullet>().ToList())
	        {
                if (enemy.collisionRectangle.Contains(bullet.collisionRectangle))
                {
                    enemy.Position = new Vector2(Screen.Width / 2, 0);
                    Components.Remove(bullet);
                    score += 10;
                    scoreText = SCORE_STRING + score;
                    enemyCounter += 1;
                 
                    if (enemyCounter == 5)
                    {
                        Components.Remove(enemy);
                    }
                }
                
                if (enemy2.collisionRectangle.Contains(bullet.collisionRectangle))
                {
                    enemy2.Position = new Vector2(Screen.Width, 0);
                    Components.Remove(bullet);
                    score += 10;
                    scoreText = SCORE_STRING + score;
                    enemy2Counter += 1;
                 
                    if (enemy2Counter == 5)
                    {
                        Components.Remove(enemy2);
                    }
                }
                
                if (enemy3.collisionRectangle.Contains(bullet.collisionRectangle))
                {
                    enemy3.Position = new Vector2(Screen.Width * 0.25f, 0);
                    Components.Remove(bullet);
                    score += 10;
                    scoreText = SCORE_STRING + score;
                    enemy3Counter += 1;
                 
                    if (enemy3Counter == 5)
                    {
                        Components.Remove(enemy3);
                    }
                }
                
                if (enemy4.collisionRectangle.Contains(bullet.collisionRectangle))
                {
                    enemy4.Position = new Vector2(Screen.Width / 3, 0);
                    Components.Remove(bullet);
                    score += 10;
                    scoreText = SCORE_STRING + score;
                    enemy4Counter += 1;
                 
                    if (enemy4Counter == 5)
                    {
                        Components.Remove(enemy4);
                    }
                }
                
                if (enemy5.collisionRectangle.Contains(bullet.collisionRectangle))
                {
                    enemy5.Position = new Vector2(Screen.Width / 1.5f, 0);
                    
                    Components.Remove(bullet);
                    score += 10;
                    scoreText = SCORE_STRING + score;
                    enemy5Counter += 1;
                   
                    if (enemy5Counter == 5)
                    {
                        Components.Remove(enemy5);
                    }
                }
	        }

            if (enemy.collisionRectangle.Contains(player.collisionRectangle))
            {
                player.Health -= 10;
            }

            if (player.Health == 0)
            {
                Components.Remove(player);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            spriteBatch.DrawString(scoreFont, scoreText, scoreTextLocation, Color.Black);
            base.Draw(gameTime);
            spriteBatch.End();
        }
    }
}