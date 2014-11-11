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
using DynamicFSM;

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
        public static SoundDictionary soundDictionary;
        public static Rectangle Playfield { get; private set; }
        public static Rectangle Screen { get; private set; }
        public static Texture2D TestTexture { get; private set; }

        // player
        Player player;

        // camera
        Camera camera;
        Vector2 cameraPos;

        // ammo
        Texture2D ammo;
        Rectangle ammoDrawRectangle;
        Texture2D garabeAmmo;
        Texture2D hoseAmmo;
        Texture2D syringeAmmo;
        Texture2D donutStandAmmo;
        Rectangle garabeAmmoRec;
        Rectangle hoseAmmoRec;
        Rectangle syringeAmmoRec;
        Rectangle donutStandAmmoRec;

        // health boxes
        Texture2D health;
        Rectangle healthDrawRactangle;
        Timer healthTimer;
        TimeSpan healthTimeSpan;
        private bool canGetHealth = true;

        //Score 
        SpriteFont scoreFont;
        public static int score;
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
        Vector2 ammoTextLocation;

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
        Song legitMusic;
        //bool playingLegitMusic = false;

        // health bar
        Texture2D healthTexture;
        Rectangle healthRectangle;
        Vector2 healthPosition;

        // background
        Texture2D backgroundTexture;
        Vector2 backgroundPosition;
        Rectangle backgroundRectangle;

        // gun selection icons
        Texture2D waterGunIcon;
        Texture2D slingShotIcon;
        Texture2D donutGunIcon;
        Vector2 waterGunIconPos;
        Vector2 slingShotIconPos;
        Vector2 donutGunIconPos;

        // Room State Machine
        StateMachine<RoomState> roomStateMachine;
        public RoomState currentRoom;
        public RoomState previousRoom;

        // transition Rectangles
        Rectangle policeTransitionRectangle;
        Rectangle mainTransitionRectangle;

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
            Playfield = new Rectangle(0, 0, 4000, 4000);
            Screen = new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            // health bar
            healthPosition = new Vector2(10, 10);
            
            // camera
            camera = new Camera(GraphicsDevice.Viewport);
            //cameraPos = player.Position;
            
            //textOffset = (int)player.Position.X - 100; 
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
            spriteDictionary.Add("playerMain", "Sprite/player_waterbase");
            spriteDictionary.Add("police", "Sprite/police");
            spriteDictionary.Add("firefighter", "Sprite/firefighter");
            spriteDictionary.Add("paramedic", "Sprite/paramedic");
            spriteDictionary.Add("waterBullet", "Sprite/waterBullet");
            spriteDictionary.Add("needleBullet", "Sprite/needleBullet");
            spriteDictionary.Add("donutBullet", "Sprite/donutBullet");
            spriteDictionary.Add("bullet", "Sprite/bullet");
            spriteDictionary.Add("sniperBullet", "Sprite/sniperBullet");
            spriteDictionary.Add("gernade", "Sprite/gernade");
            spriteDictionary.Add("ammoCrate", "Sprite/ammo");
            spriteDictionary.Add("redCross", "Sprite/redCross");
            spriteDictionary.Add("fireball", "Sprite/fireball");
            spriteDictionary.Add("jellyExplosion", "Sprite/JellyExplosion");
            spriteDictionary.Add("pedestrian", "Sprite/pedestrian");
            TestTexture = Content.Load<Texture2D>("Sprite/CollisionDebugTexture");

            //Sound
            legitMusic = Content.Load<Song>(@"Sounds/Sound");
            soundDictionary = new SoundDictionary(Content);
            soundDictionary.Add("DonutGun", "Sounds/DonutGun");
            soundDictionary.Add("SlingShot", "Sounds/SlingShot");
            soundDictionary.Add("WaterGun", "Sounds/WaterGun");
            soundDictionary.Add("SlingShotBullet", "Sounds/SlingShotBullet");
            soundDictionary.Add("WaterBullet", "Sounds/Waterbullet");

            //player
            player = new Player(this, spriteBatch, spriteDictionary["playerMain"], Tools.Math.Vectors.FromPoint(Screen.Center));
            Components.Add(player);
            player.InputTriggeredEvent += SpawnBullet;

            //ammo
            ammo = Content.Load<Texture2D>("Sprite/ammo");
            ammoDrawRectangle = new Rectangle(1950, 50, ammo.Width, ammo.Height);

            garabeAmmo = Content.Load<Texture2D>("Sprite/garabecan");
            hoseAmmo = Content.Load<Texture2D>("Sprite/hose");
            syringeAmmo = Content.Load<Texture2D>("Sprite/syringecontainer");
            donutStandAmmo = Content.Load<Texture2D>("Sprite/donutstand");

            garabeAmmoRec = new Rectangle(50, 50, garabeAmmo.Width, garabeAmmo.Height);
            hoseAmmoRec = new Rectangle(1950, 50, hoseAmmo.Width, hoseAmmo.Height);
            syringeAmmoRec = new Rectangle(50, 1950, syringeAmmo.Width, syringeAmmo.Height);
            donutStandAmmoRec = new Rectangle(1950, 1950, donutStandAmmo.Width, donutStandAmmo.Height);

            // health boxes
            health = Content.Load<Texture2D>("Sprite/redCross");
            healthDrawRactangle = new Rectangle(1350, 50, health.Width, health.Height);

            //score stuff1
            scoreFont = Content.Load<SpriteFont>("Arial");
            score = 0;
            scoreText = SCORE_STRING + score;
            healthText = HEALTH_STRING + player.Health;
            ammoText = AMMO_STRING + player.Ammo;
            //scoreTextLocation = new Vector2(TEXT_OFFSET, 20);
            //scoreTextLocation2 = new Vector2(TEXT_OFFSET_GAME_OVER, 350);

            // spawn Rate timer
            spawnTimeSpan = TimeSpan.FromSeconds(10);
            spawnTimer = new Timer();
            spawnTimer.OnExpire += () => canSpawn = true;
            spawnTimer.Start(spawnTimeSpan);

            ammoTimeSpan = TimeSpan.FromSeconds(2);
            ammoTimer = new Timer();
            ammoTimer.OnExpire += () => canAmmo = true;
            ammoTimer.Start(ammoTimeSpan);

            // health timer
            healthTimeSpan = TimeSpan.FromSeconds(2);
            healthTimer = new Timer();
            healthTimer.OnExpire += () => canGetHealth = true;
            healthTimer.Start(healthTimeSpan);

            //game over screen
            gameOver = Content.Load<Texture2D>("youDied");
            drawRectangle = new Rectangle(0, 0, gameOver.Width, gameOver.Height);

            // health bar
            healthTexture = Content.Load<Texture2D>("Sprite/health");

            // background
            backgroundTexture = Content.Load<Texture2D>("Sprite/background");
            backgroundPosition = new Vector2(0, 0);
            backgroundRectangle = new Rectangle((int)backgroundPosition.X, (int)backgroundPosition.Y, backgroundTexture.Width, backgroundTexture.Height);

            // weapon icons
            waterGunIcon = Content.Load<Texture2D>("Sprite/waterGunIcon");
            slingShotIcon = Content.Load<Texture2D>("Sprite/slingShotIcon");
            donutGunIcon = Content.Load<Texture2D>("Sprite/donutGunIcon");

            // Room State Machine
            roomStateMachine = new StateMachine<RoomState>();

            policeTransitionRectangle = new Rectangle(800, 0, 400, 20);
            mainTransitionRectangle = new Rectangle();

            // Transitions between Rooms
            Func<bool> playerExitsToPoliceRoom = () =>
            {
                if (player.weaponType == WeaponType.WaterGun
                    && policeTransitionRectangle.Intersects(player.collisionRectangle))
                {
                    return true;
                }
                else 
                {
                    return false;
                }
            };
            Func<bool> playerExitsToMainRoom = () =>
            {
                if (mainTransitionRectangle.Intersects(player.collisionRectangle))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            };

            // Add Room States
            roomStateMachine.AddState(RoomState.MainRoom).OnEnter += EnterMain;
            roomStateMachine.AddState(RoomState.PoliceRoom).OnEnter += EnterPoliceRoom;

            

            // Add Room Transitions
            roomStateMachine.AddTransition(RoomState.MainRoom, RoomState.PoliceRoom, playerExitsToPoliceRoom);
            roomStateMachine.AddTransition(RoomState.PoliceRoom, RoomState.MainRoom, playerExitsToMainRoom);

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

            //Play Legit Music
            MediaPlayer.Volume = 0.5f;
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(legitMusic);
        }

        void EnterPoliceRoom(State<RoomState> obj)
        {
            player.Position = new Vector2(1000, 1960);
            mainTransitionRectangle = new Rectangle(800, 1980, 400, 20);
        }

        void EnterMain(State<RoomState> obj)
        {
            player.Position = new Vector2(100, 100);
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
            #region MachineGun - Spawn
            //machine gun bullet spawn
            if (player.weaponType == WeaponType.MachineGun)
            {
                if (inputEvent == InputTypes.LeftMouse)
                {
                    // Spawn Bullet
                    PlayerBullet bullet = new PlayerBullet(this, spriteBatch, spriteDictionary["bullet"], player.Position);
                    bullet.weaponType = WeaponType.MachineGun;
                    bullet.SetDirection(player.Rotation, 5);
                    Components.Add(bullet);
                }
            }
            #endregion
            #region SniperRifle - Spawn
            //sniper rifle bullet spawn
            if (player.weaponType == WeaponType.SniperRifle)
            {
                if (inputEvent == InputTypes.LeftMouse)
                {
                    // Spawn Bullet
                    PlayerBullet bullet = new PlayerBullet(this, spriteBatch, spriteDictionary["sniperBullet"], player.Position);
                    bullet.weaponType = WeaponType.SniperRifle;
                    bullet.SetDirection(player.Rotation, 5);
                    Components.Add(bullet);
                }
            }
            #endregion
            #region GernadeLauncher - Spawn
            //gernade launcher bullet spawn
            if (player.weaponType == WeaponType.GernadeLauncher)
            {
                if (inputEvent == InputTypes.LeftMouse)
                {
                    // Spawn Bullet
                    PlayerBullet bullet = new PlayerBullet(this, spriteBatch, spriteDictionary["gernade"], player.Position);
                    bullet.weaponType = WeaponType.GernadeLauncher;
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

            roomStateMachine.Run();

            //Game State Menu
            if (gameState == GameState.Menu)
            {
                gameState = GameState.Play;
            }

            IEnumerable<Pedestrian> pedestrians = Components.OfType<Pedestrian>();
            IEnumerable<BasicEnemy> enemies = Components.OfType<BasicEnemy>();
            IEnumerable<BasicBullet> bullets = Components.OfType<BasicBullet>();
            //Game State Play
            if (gameState == GameState.Play)
            {               
                // health bar
                healthRectangle = new Rectangle((int)this.player.Position.X - 250, (int)this.player.Position.Y - 240, this.player.Health, 20);

                scoreTextLocation = new Vector2(this.player.Position.X + 150, this.player.Position.Y - 250);

                ammoTextLocation = new Vector2(this.player.Position.X + 250, this.player.Position.Y - 250);

                scoreTextLocation2 = new Vector2(this.player.Position.X - 50, this.player.Position.Y + 50);

                drawRectangle = new Rectangle((int)this.player.Position.X - 381, (int)this.player.Position.Y - 300, gameOver.Width, gameOver.Height);

                // weapon icons
                waterGunIconPos = new Vector2(this.player.Position.X + 250, this.player.Position.Y + 100);
                slingShotIconPos = new Vector2(this.player.Position.X + 250, this.player.Position.Y + 100);
                donutGunIconPos = new Vector2(this.player.Position.X + 250, this.player.Position.Y + 100);

                //give player ammo
                
                //Give enemy player position
                foreach (BasicEnemy enemy in enemies)
                {
                    enemy.GetPlayerPosition(player.Position);
                }

                if (this.player.collisionRectangle.Intersects(garabeAmmoRec) || this.player.collisionRectangle.Intersects(hoseAmmoRec)
                    || this.player.collisionRectangle.Intersects(syringeAmmoRec) || this.player.collisionRectangle.Intersects(donutStandAmmoRec))
                {
                    if (canAmmo)
                    {
                        this.player.Ammo += 30;
                        canAmmo = false;
                        ammoTimer.Start(ammoTimeSpan);
                    }
                }

                ammoTimer.Update(gameTime.ElapsedGameTime);

                // health boxes
                if (this.player.collisionRectangle.Intersects(healthDrawRactangle))
                {
                    if (canGetHealth)
                    {
                        if (this.player.Health < 100)
                        {
                            this.player.Health += 30;
                            canGetHealth = false;
                            healthTimer.Start(healthTimeSpan);
                        }
                    }
                }

                healthTimer.Update(gameTime.ElapsedGameTime);

                //check for Collisions
                List<DrawableGameComponent> removals = new List<DrawableGameComponent>();
                List<DrawableGameComponent> additions = new List<DrawableGameComponent>();

                //List<BasicSprite> components = Components.OfType<BasicSprite>().ToList();
                //components.AddRange(RoomManager.Current.Components.AsEnumerable().OfType<BasicSprite>());

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
                                if (bullet.bulletType == BulletType.Bullet)
                                {
                                    player.Health -= 10;
                                }

                                if (bullet.bulletType == BulletType.FireBall)
                                {
                                    player.Health -= 1;
                                }

                                //turns them off components
                                if (player.Health <= 0)
                                {
                                    player.Health = 0;
                                    player.Enabled = false;
                                    player.Remove = true;
                                    //removals.Add(player);
                                    gameState = GameState.Dead;

                                    //removes all enemies
                                    foreach (BasicEnemy enemyRemove in enemies)
                                    {
                                        enemyRemove.Enabled = false;
                                        enemyRemove.Remove = true;
                                        //removals.Add(enemyRemove);
                                    }

                                    foreach (BasicBullet bulletRemove in bullets)
                                    {
                                        bulletRemove.Enabled = false;
                                        bulletRemove.Remove = true;
                                        //removals.Add(bulletRemove);
                                    }
                                }
                                bullet.Enabled = false;
                                bullet.Remove = true;

                                //add to list of things to remove
                                //removals.Add(bullet);
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
                                // bullet collision for the police enemy
                                if (bullet.weaponType == WeaponType.WaterGun && enemy.enemyType == EnemyType.Police)
                                {
                                    //damage, 5 shots kills or one burst
                                    enemy.Health -= 10;
                                }
                                else if (bullet.weaponType == WeaponType.SlingShot && enemy.enemyType == EnemyType.Police)
                                {
                                    // damage, 2 shots kills and slows enemy
                                    enemy.Health -= 100;
                                    enemy.LinearVelocity *= 0.5f;
                                }
                                else if (bullet.weaponType == WeaponType.DonutGun && enemy.enemyType == EnemyType.Police)
                                {
                                    Explosion explosion = new Explosion(this, spriteBatch, spriteDictionary["jellyExplosion"], bullet.Position, 7);
                                    additions.Add(explosion);
                                    // damage, 1 shot kills
                                    enemy.Health -= 200;
                                }
                                else if (bullet.weaponType == WeaponType.MachineGun && enemy.enemyType == EnemyType.Police)
                                {
                                    //damage, 5 shots kills or one burst
                                    enemy.Health -= 10;
                                }
                                else if (bullet.weaponType == WeaponType.SniperRifle && enemy.enemyType == EnemyType.Police)
                                {
                                    // damage, 2 shots kills and slows enemy
                                    enemy.Health -= 100;
                                    enemy.LinearVelocity *= 0.5f;
                                }
                                else if (bullet.weaponType == WeaponType.GernadeLauncher && enemy.enemyType == EnemyType.Police)
                                {
                                    //Explosion explosion = new Explosion(this, spriteBatch, spriteDictionary["jellyExplosion"], bullet.Position, 7);
                                    //additions.Add(explosion);
                                    // damage, 1 shot kills
                                    enemy.Health -= 200;
                                }
                                else
                                {
                                    // Incase something goes wrong, still does damage
                                    enemy.Health -= 10;
                                }

                                // bullet collision for the firefighter enemy
                                if (bullet.weaponType == WeaponType.WaterGun && enemy.enemyType == EnemyType.FireFighter)
                                {
                                    //damage, 5 shots kills or one burst
                                    enemy.Health -= 20;
                                }
                                else if (bullet.weaponType == WeaponType.SlingShot && enemy.enemyType == EnemyType.FireFighter)
                                {
                                    // damage, 2 shots kills and slows enemy
                                    enemy.Health -= 50;
                                    enemy.LinearVelocity *= 0.5f;
                                }
                                else if (bullet.weaponType == WeaponType.DonutGun && enemy.enemyType == EnemyType.FireFighter)
                                {
                                    Explosion explosion = new Explosion(this, spriteBatch, spriteDictionary["jellyExplosion"], bullet.Position, 7);
                                    additions.Add(explosion);
                                    // damage, 1 shot kills
                                    enemy.Health -= 100;
                                }
                                else if (bullet.weaponType == WeaponType.MachineGun && enemy.enemyType == EnemyType.FireFighter)
                                {
                                    //damage, 5 shots kills or one burst
                                    enemy.Health -= 20;
                                }
                                else if (bullet.weaponType == WeaponType.SniperRifle && enemy.enemyType == EnemyType.FireFighter)
                                {
                                    // damage, 2 shots kills and slows enemy
                                    enemy.Health -= 50;
                                    enemy.LinearVelocity *= 0.5f;
                                }
                                else if (bullet.weaponType == WeaponType.GernadeLauncher && enemy.enemyType == EnemyType.FireFighter)
                                {
                                    //Explosion explosion = new Explosion(this, spriteBatch, spriteDictionary["jellyExplosion"], bullet.Position, 7);
                                    //additions.Add(explosion);
                                    // damage, 1 shot kills
                                    enemy.Health -= 100;
                                }
                                else
                                {
                                    // Incase something goes wrong, still does damage
                                    enemy.Health -= 10;
                                }

                                // bullet collision for the paramedic enemy
                                if (bullet.weaponType == WeaponType.WaterGun && enemy.enemyType == EnemyType.Paramedic)
                                {
                                    //damage, 5 shots kills or one burst
                                    enemy.Health -= 10;
                                }
                                else if (bullet.weaponType == WeaponType.SlingShot && enemy.enemyType == EnemyType.Paramedic)
                                {
                                    // damage, 1 shots kills and slows enemy
                                    enemy.Health -= 200;
                                    enemy.LinearVelocity *= 0.5f;
                                }
                                else if (bullet.weaponType == WeaponType.DonutGun && enemy.enemyType == EnemyType.Paramedic)
                                {
                                    Explosion explosion = new Explosion(this, spriteBatch, spriteDictionary["jellyExplosion"], bullet.Position, 7);
                                    additions.Add(explosion);
                                    // damage, 1 shot kills
                                    enemy.Health -= 50;
                                }
                                else if (bullet.weaponType == WeaponType.MachineGun && enemy.enemyType == EnemyType.Paramedic)
                                {
                                    //damage, 5 shots kills or one burst
                                    enemy.Health -= 10;
                                }
                                else if (bullet.weaponType == WeaponType.SniperRifle && enemy.enemyType == EnemyType.Paramedic)
                                {
                                    // damage, 1 shots kills and slows enemy
                                    enemy.Health -= 200;
                                    enemy.LinearVelocity *= 0.5f;
                                }
                                else if (bullet.weaponType == WeaponType.GernadeLauncher && enemy.enemyType == EnemyType.Paramedic)
                                {
                                    //Explosion explosion = new Explosion(this, spriteBatch, spriteDictionary["jellyExplosion"], bullet.Position, 7);
                                    //additions.Add(explosion);
                                    // damage, 1 shot kills
                                    enemy.Health -= 50;
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
                                    enemy.Remove = true;
                                    //removals.Add(enemy);
                                }
                                bullet.Enabled = false;
                                bullet.Remove = true;

                                //add to list of things to remove
                                //removals.Add(bullet);
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
                                player.Remove = true;
                                //removals.Add(player);
                                gameState = GameState.Dead;

                                //removes all enemies
                                foreach (BasicEnemy enemyRemove in enemies)
                                {
                                    enemyRemove.Enabled = false;
                                    enemyRemove.Remove = true;
                                    //removals.Add(enemyRemove);
                                }

                                foreach (BasicBullet bulletRemove in bullets)
                                {
                                    bulletRemove.Enabled = false;
                                    bulletRemove.Remove = true;
                                    //removals.Add(bulletRemove);
                                }
                            }
                        }
                        #endregion
                        #region Pedestrian - Bullet Collision
                        if (collider is Pedestrian && other is PlayerBullet)
                        {
                            Pedestrian pedestrian = collider as Pedestrian;
                            PlayerBullet bullet = other as PlayerBullet;

                            if (pedestrian.Enabled && bullet.Enabled && pedestrian.collisionRectangle.Intersects(bullet.collisionRectangle))
                            {
                                //// score penalty
                                //score -= 100;
                                pedestrian.Enabled = false;
                                pedestrian.Remove = true;

                                bullet.Enabled = false;
                                bullet.Remove = true;
                            }
                        }
                        #endregion

                    }
                }

                

                ////removes bullet outside screen
                //foreach (BasicBullet bullet in bullets)
                //{
                //    if (!Screen.Contains(Tools.Math.Points.FromVector(bullet.Position)))
                //    {
                //        bullet.Enabled = false;
                //        removals.Add(bullet);
                //    }
                //}

                //spawn enemies if spawn timer expires
                if (canSpawn)
                {
                    // spawn paramedics
                    for (int i = 1; i <= 3; i++)
                    {
                        ParamedicEnemy enemy = new ParamedicEnemy(this, spriteBatch, spriteDictionary["paramedic"], new Vector2(Screen.Width - (150 * i), 0));
                        //FireFighterEnemy enemy = new FireFighterEnemy(this, spriteBatch, spriteDictionary["firefighter"], new Vector2(Screen.Width - (150 * i), 0));
                        //PoliceEnemy enemy = new PoliceEnemy(this, spriteBatch, spriteDictionary["police"], new Vector2(Screen.Width - (150 * i), 0));
                        //enemy.EnemyActionTriggeredEvent += SpawnEnemyBullet;
                        Components.Add(enemy);
                    }

                    // spawn firefighters
                    for (int i = 1; i <= 3; i++)
                    {
                        //ParamedicEnemy enemy = new ParamedicEnemy(this, spriteBatch, spriteDictionary["paramedic"], new Vector2(Screen.Width - (150 * i), 0));
                        FireFighterEnemy enemy = new FireFighterEnemy(this, spriteBatch, spriteDictionary["firefighter"], new Vector2(0, Screen.Height - (150 * i)));
                        //PoliceEnemy enemy = new PoliceEnemy(this, spriteBatch, spriteDictionary["police"], new Vector2(Screen.Width - (150 * i), 0));
                        enemy.EnemyActionTriggeredEvent += SpawnEnemyBullet;
                        Components.Add(enemy);
                    }

                    // spawn police
                    for (int i = 1; i <= 3; i++)
                    {
                        //ParamedicEnemy enemy = new ParamedicEnemy(this, spriteBatch, spriteDictionary["paramedic"], new Vector2(Screen.Width - (150 * i), 0));
                        //FireFighterEnemy enemy = new FireFighterEnemy(this, spriteBatch, spriteDictionary["firefighter"], new Vector2(Screen.Width - (150 * i), 0));
                        PoliceEnemy enemy = new PoliceEnemy(this, spriteBatch, spriteDictionary["police"], new Vector2(Screen.Width - (150 * i), Screen.Height + 150 * i));
                        enemy.EnemyActionTriggeredEvent += SpawnEnemyBullet;
                        Components.Add(enemy);
                    }

                    // spawn pedestrians
                    for (int i = 1; i <= 3; i++)
                    {
                        Pedestrian pedestrian = new Pedestrian(this, spriteBatch, spriteDictionary["pedestrian"], new Vector2(Screen.Width - (150 * i), Screen.Height - 50), i, true);
                    }

                    //restart timer
                    canSpawn = false;
                    spawnTimer.Start(spawnTimeSpan);
                }

                foreach (BasicSprite sprite in Components.OfType<BasicSprite>().Where(sprite => sprite.Remove))
                {
                    removals.Add(sprite);
                }

                //remove list of components to be removed
                foreach (DrawableGameComponent removal in removals)
                {
                    Components.Remove(removal);
                }

                // add all additions
                additions.ForEach(addition => Components.Add(addition));

                //update health and score
                //healthText = HEALTH_STRING + this.player.Health;
                scoreText = SCORE_STRING + score;
                ammoText = AMMO_STRING + this.player.Ammo;

                //update timer
                spawnTimer.Update(gameTime.ElapsedGameTime);

                // update camera
                camera.Update(gameTime, this.player);

                this.player.Update(gameTime);
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
                foreach (Pedestrian pedestrian in pedestrians)
                {
                    pedestrian.LayerDepth = -1;
                }
            }

            
            base.Update(gameTime);
        }

        private void SpawnEnemyBullet(EnemyAction enemyAction, BasicEnemy sender)
        {
            if (enemyAction == EnemyAction.FireBullet)
            {
                if (sender.enemyType == EnemyType.Police)
                {
                    // Spawn Bullet
                    EnemyBullet bullet = new EnemyBullet(this, spriteBatch, spriteDictionary["waterBullet"], sender.Position);
                    bullet.bulletType = BulletType.Bullet;
                    bullet.SetDirection(sender.Rotation, 5);
                    Components.Add(bullet);
                }

                if (sender.enemyType == EnemyType.FireFighter)
                {
                    EnemyBullet bullet = new EnemyBullet(this, spriteBatch, spriteDictionary["fireball"], sender.Position);
                    bullet.bulletType = BulletType.FireBall;
                    bullet.SetDirection(sender.Rotation, 5);
                    Components.Add(bullet);
                }
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
            //spriteBatch.Begin(SpriteSortMode.BackToFront, null, null, null, null, null, GetScreenMatrix());
            //spriteBatch.Begin();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.transform);

            //Game State Menu
            if (gameState == GameState.Menu)
            {
 
            }

            //Game State Play
            if (gameState == GameState.Play)
            {
                // draw background
                spriteBatch.Draw(backgroundTexture, backgroundPosition, Color.White);

                //Draw score
                spriteBatch.DrawString(scoreFont, scoreText, scoreTextLocation, Color.Black);
                
                //draw ammo
                spriteBatch.Draw(garabeAmmo, garabeAmmoRec, Color.White);
                spriteBatch.Draw(hoseAmmo, hoseAmmoRec, Color.White);
                spriteBatch.Draw(syringeAmmo, syringeAmmoRec, Color.White);
                spriteBatch.Draw(donutStandAmmo, donutStandAmmoRec, Color.White);

                // draw health boxes
                spriteBatch.Draw(health, healthDrawRactangle, Color.White);

                // weapon icons
                if (Player.wgiFlag == true)
                    spriteBatch.Draw(waterGunIcon, waterGunIconPos, Color.White);
                if (Player.ssiFlag == true)
                    spriteBatch.Draw(slingShotIcon, slingShotIconPos, Color.White);
                if (Player.dgiFlag == true)
                    spriteBatch.Draw(donutGunIcon, donutGunIconPos, Color.White);
                
                //Draw health
                if (this.player.Health <= 25)
                {
                    //spriteBatch.DrawString(scoreFont, healthText, scoreTextLocation - new Vector2(150, 0), Color.Red);
                    spriteBatch.Draw(healthTexture, healthRectangle, Color.Red);
                }
                else if (this.player.Health <= 75)
                {
                    //spriteBatch.DrawString(scoreFont, healthText, scoreTextLocation - new Vector2(150, 0), Color.Yellow);
                    spriteBatch.Draw(healthTexture, healthRectangle, Color.Yellow);
                }
                else
                {
                    //spriteBatch.DrawString(scoreFont, healthText, scoreTextLocation - new Vector2(150, 0), Color.Green);
                    spriteBatch.Draw(healthTexture, healthRectangle, Color.Green);
                }

                // Draw ammo
                if (this.player.Ammo <= 10)
                {
                    spriteBatch.DrawString(scoreFont, ammoText, ammoTextLocation - new Vector2(300, 0), Color.Red);
                }
                else if (this.player.Ammo <= 25)
                {
                    spriteBatch.DrawString(scoreFont, ammoText, ammoTextLocation - new Vector2(300, 0), Color.Yellow);
                }
                else
                {
                    spriteBatch.DrawString(scoreFont, ammoText, ammoTextLocation - new Vector2(300, 0), Color.Green);
                }
            }

            //Game State Dead
            if (gameState == GameState.Dead)
            {
                spriteBatch.Draw(gameOver, drawRectangle, Color.White);
                spriteBatch.DrawString(scoreFont, scoreText, scoreTextLocation2, Color.Black);
                //spriteBatch.Draw(gameOver, drawRectangle, null, Color.White, 0.0f, new Vector2(0, 0), SpriteEffects.None, 10f);
            }
            base.Draw(gameTime);
            spriteBatch.End();
        }

        private Matrix GetScreenMatrix()
        {
            Screen = new Rectangle((int)(player.Position.X - Screen.Width / 2),
                (int)(player.Position.Y - Screen.Height / 2),
                Screen.Width, Screen.Height);
            //return Matrix.CreateOrthographicOffCenter(Screen.Left, Screen.Right, Screen.Bottom, Screen.Top, 0, 1);
            return Matrix.CreateTranslation(new Vector3(Screen.X, Screen.Y, 0));
        }
    }
    public enum GameState { Menu, Play, Dead }
    public enum RoomState { MainRoom, PoliceRoom, FireFighterRoom, ParamedicRoom }
}