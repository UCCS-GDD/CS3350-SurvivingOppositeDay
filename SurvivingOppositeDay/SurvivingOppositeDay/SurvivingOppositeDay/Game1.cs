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
        public static Texture2D TestTexture { get; private set; }

        // player
        Player player;

        // ammo
        Texture2D ammo;
        Rectangle ammoDrawRectangle;

        //Health
        HealthBar healthBar;

        //Score 
        SpriteFont scoreFont;
        int score;
        const string SCORE_STRING = "Score: ";
        const string HEALTH_STRING = "Health: ";
        const string AMMO_STRING = "Ammo: ";
        const int TEXT_OFFSET = 550;
        const int TEXT_OFFSET_GAME_OVER = 300;
        string scoreText;
        string healthText;
        string ammoText;
        Vector2 scoreTextLocation;
        Vector2 scoreTextLocation2;
        
        //spawn timer
        Timer spawnTimer;
        TimeSpan spawnTimeSpan;
        private bool canSpawn = true;

        Timer ammoTimer;
        TimeSpan ammoTimeSpan;
        private bool canAmmo = true;

        // game over screen
        Texture2D gameOver;
        Rectangle drawRectangle;

        //Sound effect
        SoundEffect legitMusic;
        bool playingLegitMusic = false;

        // for testing
        //BasicSprite example;
        //Animation animation;
        //KeyboardState previous;
        //KeyboardState current;

        public static GameState gameState = GameState.Menu;
        
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
            spriteDictionary.Add("player", "Sprite/player2");
            spriteDictionary.Add("waterBullet", "Sprite/waterBullet");
            spriteDictionary.Add("needleBullet", "Sprite/needleBullet");
            spriteDictionary.Add("donutBullet", "Sprite/donutBullet");
            spriteDictionary.Add("ammoCrate", "Sprite/ammo");
            TestTexture = Content.Load<Texture2D>("Sprite/CollisionDebugTexture");

            //Sound
            legitMusic = Content.Load<SoundEffect>(@"Sound");

            //player
            player = new Player(this, spriteBatch, spriteDictionary["player"], Tools.Math.Vectors.FromPoint(Screen.Center));
            Components.Add(player);
            player.InputTriggeredEvent += SpawnBullet;

            //ammo
            ammo = Content.Load<Texture2D>("Sprite/ammo");
            ammoDrawRectangle = new Rectangle(300, 350, ammo.Width, ammo.Height);

            //health
            healthBar = new HealthBar(this, spriteBatch,Content.Load<Texture2D>(@"Sprite/healthbar"), new Vector2(500,50), true, player);

            //score stuff1
            scoreFont = Content.Load<SpriteFont>("Arial");
            score = 0;
            scoreText = SCORE_STRING + score;
            healthText = HEALTH_STRING + player.Health;
            ammoText = AMMO_STRING + player.Ammo;
            scoreTextLocation = new Vector2(TEXT_OFFSET, 20);
            scoreTextLocation2 = new Vector2(TEXT_OFFSET_GAME_OVER, 350);

            // spawn Rate timer
            spawnTimeSpan = TimeSpan.FromSeconds(10);
            spawnTimer = new Timer();
            spawnTimer.OnExpire += () => canSpawn = true;
            spawnTimer.Start(spawnTimeSpan);

            ammoTimeSpan = TimeSpan.FromSeconds(2);
            ammoTimer = new Timer();
            ammoTimer.OnExpire += () => canAmmo = true;
            ammoTimer.Start(ammoTimeSpan);

            //game over screen
            gameOver = Content.Load<Texture2D>("youDied");
            drawRectangle = new Rectangle(0, 0, gameOver.Width, gameOver.Height);

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
            #region WaterGun - Spawn
            //water gun bullet spawn
            if(player.weaponType == WeaponType.WaterGun)
            {
                if (inputEvent == InputTypes.LeftMouse)
                {
                    // Spawn Bullet
                    PlayerBullet bullet = new PlayerBullet(this, spriteBatch, spriteDictionary["waterBullet"], player.Position);
                    bullet.weaponType = WeaponType.WaterGun;
                    bullet.SetDirection(player.Rotation, 5);
                    Components.Add(bullet);
                }
            }
            #endregion
            #region SlingShot - Spawn
            //sling shot bullet spawn
            if (player.weaponType == WeaponType.SlingShot)
            {
                if (inputEvent == InputTypes.LeftMouse)
                {
                    // Spawn Bullet
                    PlayerBullet bullet = new PlayerBullet(this, spriteBatch, spriteDictionary["needleBullet"], player.Position);
                    bullet.weaponType = WeaponType.SlingShot;
                    bullet.SetDirection(player.Rotation, 5);
                    Components.Add(bullet);
                }
            }
            #endregion
            #region DonutGun - Spawn
            //donut gun bullet spawn
            if (player.weaponType == WeaponType.DonutGun)
            {
                if (inputEvent == InputTypes.LeftMouse)
                {
                    // Spawn Bullet
                    PlayerBullet bullet = new PlayerBullet(this, spriteBatch, spriteDictionary["donutBullet"], player.Position);
                    bullet.weaponType = WeaponType.DonutGun;
                    bullet.SetDirection(player.Rotation, 5);
                    Components.Add(bullet);
                }
            }
            #endregion
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
            
            //Game State Menu
            if (gameState == GameState.Menu)
            {
                gameState = GameState.Play;
            }

            IEnumerable<BasicEnemy> enemies = Components.OfType<BasicEnemy>();
            IEnumerable<BasicBullet> bullets = Components.OfType<BasicBullet>();
            //Game State Play
            if (gameState == GameState.Play)
            {
                //Play Legit Music
                if(!playingLegitMusic)
                {
                    SoundEffectInstance soundinstance = legitMusic.CreateInstance();
                    soundinstance.IsLooped = true;
                    soundinstance.Play();
                    playingLegitMusic = true;
                }
                

                //give player ammo
                
                //Give enemy player position
                
                foreach (BasicEnemy enemy in enemies)
                {
                    enemy.GetPlayerPosition(player.Position);
                }
                //enemies.ForEach(enemy => enemy.GetPlayerPosition(player.Position));

                if (this.player.collisionRectangle.Intersects(ammoDrawRectangle))
                {
                    if (canAmmo)
                    {
                        this.player.Ammo += 10;
                        canAmmo = false;
                        ammoTimer.Start(ammoTimeSpan);
                    }
                }

                ammoTimer.Update(gameTime.ElapsedGameTime);

                //check for Collisions
                List<DrawableGameComponent> removals = new List<DrawableGameComponent>();
                foreach (DrawableGameComponent collider in Components)
                {
                    foreach (DrawableGameComponent other in Components)
                    {
                        #region Player - Bullet Collision
                        if (collider is Player && other is EnemyBullet)
                        {
                            Player player = collider as Player;
                            EnemyBullet bullet = other as EnemyBullet;
                            
                            //checks collision
                            if (player.Enabled && bullet.Enabled && player.collisionRectangle.Intersects(bullet.collisionRectangle))
                            {
                                player.Health -= 10;

                                //turns them off components
                                if (player.Health <= 0)
                                {
                                    player.Health = 0;
                                    player.Enabled = false;
                                    removals.Add(player);
                                    gameState = GameState.Dead;
                                }
                                bullet.Enabled = false;

                                //add to list of things to remove
                                removals.Add(bullet);
                            }
                        }
                        #endregion
                        #region Enemy - Bullet Collision
                        if (collider is BasicEnemy && other is PlayerBullet)
                        {
                            BasicEnemy enemy = collider as BasicEnemy;
                            PlayerBullet bullet = other as PlayerBullet;

                            //checks collision
                            if (enemy.Enabled && bullet.Enabled && enemy.collisionRectangle.Intersects(bullet.collisionRectangle))
                            {
                                if (bullet.weaponType == WeaponType.WaterGun)
                                {
                                    //damage, 5 shots kills or one burst
                                    enemy.Health -= 20;
                                }
                                else if (bullet.weaponType == WeaponType.SlingShot)
                                {
                                    // damage, 2 shots kills and slows enemy
                                    enemy.Health -= 50;
                                    enemy.LinearVelocity *= 0.5f;
                                }
                                else if (bullet.weaponType == WeaponType.DonutGun)
                                {
                                    // damage, 1 shot kills
                                    enemy.Health -= 100;
                                }
                                else
                                {
                                    // Incase something goes wrong, still does damage
                                    enemy.Health -= 10;
                                }

                                //turns them off components
                                if (enemy.Health <= 0)
                                {
                                    //update score
                                    score += 100;
                                    enemy.Enabled = false;
                                    removals.Add(enemy);
                                }
                                bullet.Enabled = false;

                                //add to list of things to remove
                                removals.Add(bullet);
                            }
                        }
                        #endregion
                        #region Enemy - Player Collission
                        if (collider is BasicEnemy && other is Player)
                        {
                            BasicEnemy enemy = collider as BasicEnemy;
                            Player player = other as Player;

                            //check collision
                            if (enemy.Enabled && player.Enabled && player.collisionRectangle.Intersects(enemy.collisionRectangle))
                            {
                                // lower health
                                this.player.Health -= 1;
                            }

                            //check if player is dead
                            if (player.Health <= 0)
                            {
                                //turn off player and remove to list to remove
                                player.Enabled = false;
                                removals.Add(player);
                                gameState = GameState.Dead;
                            }
                        }
                        #endregion
                    }
                }

                //spawn enemies if spawn timer expires
                if (canSpawn)
                {
                    //enemies
                    for (int i = 1; i <= 5; i++)
                    {
                        BasicEnemy enemy = new BasicEnemy(this, spriteBatch, spriteDictionary["player"], new Vector2(Screen.Width - (150 * i), 0));
                        enemy.EnemyActionTriggeredEvent += SpawnEnemyBullet;
                        Components.Add(enemy);
                    }

                    //restart timer
                    canSpawn = false;
                    spawnTimer.Start(spawnTimeSpan);
                }

                //remove list of components to be removed
                foreach (DrawableGameComponent removal in removals)
                {
                    Components.Remove(removal);
                }

                //update health and score
                //healthText = HEALTH_STRING + this.player.Health;
                scoreText = SCORE_STRING + score;
                ammoText = AMMO_STRING + this.player.Ammo;

                //update timer
                spawnTimer.Update(gameTime.ElapsedGameTime);
            }

            if (gameState == GameState.Dead)
            {
                foreach (BasicEnemy enemy in enemies)
                {
                    enemy.LayerDepth = -1;
                }
                foreach (BasicBullet bullet in bullets)
                {
                    bullet.LayerDepth = -1;
                }
            }

            base.Update(gameTime);
        }

        private void SpawnEnemyBullet(EnemyAction enemyAction, BasicEnemy sender)
        {
            if (enemyAction == EnemyAction.FireBullet)
            {
                // Spawn Bullet
                EnemyBullet bullet = new EnemyBullet(this, spriteBatch, spriteDictionary["waterBullet"], sender.Position);
                bullet.SetDirection(sender.Rotation, 5);
                Components.Add(bullet);
            }
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

            //Game State Menu
            if (gameState == GameState.Menu)
            {
 
            }

            //Game State Play
            if (gameState == GameState.Play)
            {
                //Draw score
                spriteBatch.DrawString(scoreFont, scoreText, scoreTextLocation, Color.Black);
                
                //draw ammo
                spriteBatch.Draw(ammo, ammoDrawRectangle, Color.White);

                //Draw health
                //if (this.player.Health <= 10)
                //{
                //    spriteBatch.DrawString(scoreFont, healthText, scoreTextLocation - new Vector2(150, 0), Color.Red);
                //}
                //else if (this.player.Health <= 50)
                //{
                //    spriteBatch.DrawString(scoreFont, healthText, scoreTextLocation - new Vector2(150, 0), Color.Yellow);
                //}
                //else
                //{
                //    spriteBatch.DrawString(scoreFont, healthText, scoreTextLocation - new Vector2(150, 0), Color.Green);
                //}

                // Draw ammo
                if (this.player.Ammo <= 10)
                {
                    spriteBatch.DrawString(scoreFont, ammoText, scoreTextLocation - new Vector2(300, 0), Color.Red);
                }
                else if (this.player.Ammo <= 25)
                {
                    spriteBatch.DrawString(scoreFont, ammoText, scoreTextLocation - new Vector2(300, 0), Color.Yellow);
                }
                else
                {
                    spriteBatch.DrawString(scoreFont, ammoText, scoreTextLocation - new Vector2(300, 0), Color.Green);
                }
            }

            //Game State Dead
            if (gameState == GameState.Dead)
            {
                spriteBatch.Draw(gameOver, drawRectangle, Color.White);
                spriteBatch.DrawString(scoreFont, scoreText, scoreTextLocation2, Color.Black);
            }
            base.Draw(gameTime);
            spriteBatch.End();
        }
    }
    public enum GameState { Menu, Play, Dead }
}