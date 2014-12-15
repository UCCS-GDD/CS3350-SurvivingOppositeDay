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

        public GameComponentCollection GUIComponents { get; private set; }

        public Dictionary<RoomState, CollisionCollection> Colliders { get; private set; }

        // player
        Player player;

        // camera
        Camera camera;

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
        Texture2D health2;
        Rectangle healthDrawRactangle2;
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

        //GUI Bar
        Rectangle hudRectangle;

        // background
        Texture2D backgroundTexture;
        Vector2 backgroundPosition;
        Rectangle backgroundRectangle;

        // menu backgrounds
        Texture2D titleScreen;
        Texture2D helpScreen;
        Texture2D aboutScreen;
        Texture2D objectiveScreen;
        Texture2D legendScreen;
        Texture2D creditsScreen;

        Vector2 titleScreenPos;
        Vector2 helpScreenPos;
        Vector2 aboutScreenPos;
        Vector2 objectiveScreenPos;
        Vector2 legendScreenPos;
        Vector2 creditsScreenPos;

        // menu buttons
        Texture2D playButton;
        Texture2D helpButton;
        Texture2D quitButton;
        Texture2D backButton;
        Texture2D continueButton;
        Texture2D aboutButton;

        Rectangle playButtonRec;
        Rectangle helpButtonRec;
        Rectangle quitButtonRec;
        Rectangle backButtonRec;
        Rectangle continueButtonRec;
        Rectangle aboutButtonRec;

        // menu timers
        Timer clickTimer;
        TimeSpan clickTimeSpan;
        public static bool clickable = true;

        // gun selection icons
        Texture2D waterGunIcon;
        Texture2D slingShotIcon;
        Texture2D donutGunIcon;
        Texture2D machineGunIcon;
        Texture2D sniperRifleIcon;
        Texture2D gernadeLauncherIcon;
        Vector2 gunIconPos;

        //pickup items
        Pickup pickup1, pickup2, pickup3;

        // Room State Machine
        public static StateMachine<RoomState> roomStateMachine;
        RoomState previousRoom;

        // transition Rectangles
        Rectangle policeTransitionRectangle;
        Rectangle medicTransitionRectangle;
        Rectangle fireTransitionRectangle;
        Rectangle mainTransitionRectangle;

        // enemy death sounds
        public static SoundEffectInstance soundEffectInstanceDeathSound;
        public static SoundEffectInstance soundEffectInstanceParamedicDeath;

        // random number generator
        Random enemySpawnNum;

        // room signs
        Rectangle mainRoomSignDown;
        Rectangle mainRoomSignLeft;
        Rectangle mainRoomSignRight;
        Rectangle policeRoomSign;
        Rectangle paramedicRoomSign;
        Rectangle fireFighterRoomSign;

        // pickups
        Pickup pickUp1;
        Pickup pickUp2;
        Pickup pickUp3;
        public static bool pickup1Flag = false;
        public static bool pickup2Flag = false;
        public static bool pickup3Flag = false;

        //// invisible collision detection
        //public static Rectangle house1;
        //Rectangle house2;

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
            GUIComponents = new GameComponentCollection();
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

            //// collision detection
            //house1 = new Rectangle(0, 0, 300, 350);

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
            spriteDictionary.Add("police", "Sprite/cop_base");
            spriteDictionary.Add("firefighter", "Sprite/Fireman");
            spriteDictionary.Add("paramedic", "Sprite/medic_base");
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
            spriteDictionary.Add("pedestrian", "Sprite/PedestrianF_base");
            spriteDictionary.Add("mainRoom", "Sprite/MainRoom");
            spriteDictionary.Add("fireRoom", "Sprite/FireRoom2");
            spriteDictionary.Add("policeRoom", "Sprite/PoliceRoom2");
            spriteDictionary.Add("medicRoom", "Sprite/MedicRoom2");
            spriteDictionary.Add("bar", "Sprite/bar");
            TestTexture = Content.Load<Texture2D>("Sprite/CollisionDebugTexture");

            // Menu Items
            spriteDictionary.Add("titleScreen", "Menu Items/sodTitle");
            spriteDictionary.Add("helpScreen", "Menu Items/controlsMenu");
            spriteDictionary.Add("aboutScreen", "Menu Items/aboutMenu");
            spriteDictionary.Add("objectiveScreen", "Menu Items/objectiveMenu");
            spriteDictionary.Add("creditsScreen", "Menu Items/creditsMenu");
            spriteDictionary.Add("legendScreen", "Menu Items/legendMenu");
            spriteDictionary.Add("playButton", "Menu Items/play_button");
            spriteDictionary.Add("helpButton", "Menu Items/help_button");
            spriteDictionary.Add("quitButton", "Menu Items/quit_button");
            spriteDictionary.Add("backButton", "Menu Items/spr_back_button");
            spriteDictionary.Add("continueButton", "Menu Items/nextButton");
            spriteDictionary.Add("aboutButton", "Menu Items/aboutButton");

            // room signs
            spriteDictionary.Add("mainRoomSignDown", "Sprite/mainRoomIconDown");
            spriteDictionary.Add("mainRoomSignLeft", "Sprite/mainRoomIconLeft");
            spriteDictionary.Add("mainRoomSignRight", "Sprite/mainRoomIconRight");
            spriteDictionary.Add("policeRoomSign", "Sprite/policeIcon");
            spriteDictionary.Add("paramedicRoomSign", "Sprite/hospitalIcon");
            spriteDictionary.Add("fireFighterRoomSign", "Sprite/fireStationIcon");

            //Sound
            legitMusic = Content.Load<Song>(@"Sounds/Sound");
            soundDictionary = new SoundDictionary(Content);
            soundDictionary.Add("DonutGun", "Sounds/DonutGun");
            soundDictionary.Add("SlingShot", "Sounds/SlingShot");
            soundDictionary.Add("WaterGun", "Sounds/WaterGun");
            soundDictionary.Add("SlingShotBullet", "Sounds/SlingShotBullet");
            soundDictionary.Add("WaterBullet", "Sounds/Waterbullet");
            soundDictionary.Add("MachineGun", "Sounds/machineGun");
            soundDictionary.Add("SniperRifle", "Sounds/sniperRifle");
            soundDictionary.Add("GrenadeLauncher", "Sounds/grenadeLauncher");

            // enemy death sounds
            soundDictionary.Add("DeathSound", "Sounds/deathSound");
            soundDictionary.Add("ParamedicDeath", "Sounds/paramedicDeath");

            soundEffectInstanceDeathSound = soundDictionary["DeathSound"].CreateInstance();
            soundEffectInstanceParamedicDeath = soundDictionary["ParamedicDeath"].CreateInstance();

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

            garabeAmmoRec = new Rectangle(170, 660, garabeAmmo.Width, garabeAmmo.Height);
            hoseAmmoRec = new Rectangle(1560, 1680, hoseAmmo.Width, hoseAmmo.Height);
            syringeAmmoRec = new Rectangle(1620, 300, syringeAmmo.Width, syringeAmmo.Height);
            donutStandAmmoRec = new Rectangle(620, 1340, donutStandAmmo.Width, donutStandAmmo.Height);

            // health boxes
            health = Content.Load<Texture2D>("Sprite/redCross");
            healthDrawRactangle = new Rectangle(480, 420, health.Width, health.Height);
            health2 = Content.Load<Texture2D>("Sprite/redCross");
            healthDrawRactangle2 = new Rectangle(1400, 1560, health.Width, health.Height);

            //score stuff1
            scoreFont = Content.Load<SpriteFont>("Menu Items/Arial");
            score = 0;
            scoreText = SCORE_STRING + score;
            healthText = HEALTH_STRING + player.Health;
            ammoText = AMMO_STRING + player.Ammo;
            //scoreTextLocation = new Vector2(TEXT_OFFSET, 20);
            //scoreTextLocation2 = new Vector2(TEXT_OFFSET_GAME_OVER, 350);

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
            gameOver = Content.Load<Texture2D>("Menu Items/youDied");
            drawRectangle = new Rectangle(0, 0, gameOver.Width, gameOver.Height);

            // health bar
            healthTexture = Content.Load<Texture2D>("Sprite/health");

            //GUI Hud
            hudRectangle = new Rectangle(0, 0, Screen.Width, 60);
            scoreTextLocation = new Vector2(Screen.Width * ((float)5 / 12), 15);
            ammoTextLocation = new Vector2(Screen.Width * ((float)9 / 12), 15);
            scoreTextLocation2 = new Vector2(Screen.Width * ((float)5 / 12), Screen.Height * ((float)3 / 4));

            // background
            backgroundTexture = spriteDictionary["mainRoom"];
            backgroundPosition = new Vector2(0, 0);
            backgroundRectangle = new Rectangle((int)backgroundPosition.X, (int)backgroundPosition.Y, backgroundTexture.Width, backgroundTexture.Height);

            // menu items
            titleScreen = spriteDictionary["titleScreen"];
            helpScreen = spriteDictionary["helpScreen"];
            legendScreen = spriteDictionary["legendScreen"];
            aboutScreen = spriteDictionary["aboutScreen"];
            objectiveScreen = spriteDictionary["objectiveScreen"];
            creditsScreen = spriteDictionary["creditsScreen"];
            playButton = spriteDictionary["playButton"];
            helpButton = spriteDictionary["helpButton"];
            quitButton = spriteDictionary["quitButton"];
            backButton = spriteDictionary["backButton"];
            continueButton = spriteDictionary["continueButton"];
            aboutButton = spriteDictionary["aboutButton"];

            titleScreenPos = new Vector2(0, 0);
            helpScreenPos = new Vector2(0, 0);
            aboutScreenPos = new Vector2(0, 0);
            objectiveScreenPos = new Vector2(0, 0);
            legendScreenPos = new Vector2(0, 0);
            creditsScreenPos = new Vector2(0, 0);
            playButtonRec = new Rectangle(150, 370, playButton.Width, playButton.Height);
            backButtonRec = new Rectangle(400, 370, quitButton.Width, quitButton.Height);
            helpButtonRec = new Rectangle(300, 370, helpButton.Width, helpButton.Height);
            quitButtonRec = new Rectangle(600, 370, quitButton.Width, quitButton.Height);
            aboutButtonRec = new Rectangle(450, 370, aboutButton.Width, aboutButton.Height);

            // menu timers
            clickTimeSpan = TimeSpan.FromSeconds(.5);
            clickTimer = new Timer();
            clickTimer.OnExpire += () => clickable = true;
            clickTimer.Start(clickTimeSpan);

            // weapon icons
            waterGunIcon = Content.Load<Texture2D>("Sprite/waterGunIcon");
            slingShotIcon = Content.Load<Texture2D>("Sprite/slingShotIcon");
            donutGunIcon = Content.Load<Texture2D>("Sprite/donutGunIcon");
            machineGunIcon = Content.Load<Texture2D>("Sprite/machineGunIcon");
            sniperRifleIcon = Content.Load<Texture2D>("Sprite/sniperRifleIcon");
            gernadeLauncherIcon = Content.Load<Texture2D>("Sprite/gernadeLauncherIcon");

            // weapon icons
            gunIconPos = new Vector2(Screen.Width - waterGunIcon.Width, Screen.Height - waterGunIcon.Height);

            // pickups
            pickUp1 = new Pickup(this, spriteBatch, Content.Load<Texture2D>("Sprite/pickup1"), new Vector2(500, 250), 1, player);
            pickUp2 = new Pickup(this, spriteBatch, Content.Load<Texture2D>("Sprite/pickup2"), new Vector2(1500, 250), 2, player);
            pickUp3 = new Pickup(this, spriteBatch, Content.Load<Texture2D>("Sprite/pickup3"), new Vector2(1500, 500), 3, player);

            // Room State Machine
            roomStateMachine = new StateMachine<RoomState>();

            previousRoom = RoomState.MainRoom;
            policeTransitionRectangle = new Rectangle(800, 0, 400, 20);
            medicTransitionRectangle = new Rectangle(1980, 800, 20, 400);
            fireTransitionRectangle = new Rectangle(0, 800, 20, 400);
            mainTransitionRectangle = new Rectangle();

            // Transitions between Rooms
            Func<bool> playerExitsToPoliceRoom = () =>
            {
                if (policeTransitionRectangle.Intersects(player.collisionRectangle))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            };
            Func<bool> playerExitsToMedicRoom = () =>
            {
                if (medicTransitionRectangle.Intersects(player.collisionRectangle))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            };
            Func<bool> playerExitsToFireRoom = () =>
            {
                if (fireTransitionRectangle.Intersects(player.collisionRectangle))
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
            roomStateMachine.AddState(RoomState.ParamedicRoom).OnEnter += EnterMedicRoom;
            roomStateMachine.AddState(RoomState.FireFighterRoom).OnEnter += EnterFireRoom;

            // Add Room Transitions
            roomStateMachine.AddTransition(RoomState.MainRoom, RoomState.PoliceRoom, playerExitsToPoliceRoom);
            roomStateMachine.AddTransition(RoomState.PoliceRoom, RoomState.MainRoom, playerExitsToMainRoom);
            roomStateMachine.AddTransition(RoomState.MainRoom, RoomState.ParamedicRoom, playerExitsToMedicRoom);
            roomStateMachine.AddTransition(RoomState.ParamedicRoom, RoomState.MainRoom, playerExitsToMainRoom);
            roomStateMachine.AddTransition(RoomState.MainRoom, RoomState.FireFighterRoom, playerExitsToFireRoom);
            roomStateMachine.AddTransition(RoomState.FireFighterRoom, RoomState.MainRoom, playerExitsToMainRoom);

            // random number generator
            enemySpawnNum = new Random();

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

            //// create environmental colliders
            //Colliders = new Dictionary<RoomState, CollisionCollection>();
            //Colliders.Add(RoomState.MainRoom, new CollisionCollection(RoomState.MainRoom));
            //Colliders[RoomState.MainRoom].Colliders.Add(house1);

            //Play Legit Music
            MediaPlayer.Volume = 0.5f;
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(legitMusic);
        }

        void EnterFireRoom(State<RoomState> obj)
        {
            // remove enemy and bullets on enterance
            foreach (BasicEnemy enemy in Components.OfType<BasicEnemy>().ToList())
            {
                enemy.Remove = true;
            }
            foreach (EnemyBullet bullet in Components.OfType<EnemyBullet>().ToList())
            {
                bullet.Remove = true;
            }

            player.Position = new Vector2(1960, 1000);
            mainTransitionRectangle = new Rectangle(1980, 800, 20, 400);
            backgroundTexture = spriteDictionary["fireRoom"];
            previousRoom = RoomState.FireFighterRoom;

            // add pickup
            //Components.Add(new Pickup(this, spriteBatch, Content.Load<Texture2D>("Sprite/pickup1"), new Vector2(500, 250), 1, player));
            Components.Add(pickUp1);
            if (pickup1Flag == true)
            {
                Components.Remove(pickUp1);
            }
        }

        void EnterMedicRoom(State<RoomState> obj)
        {
            // remove enemy and bullets on enterance
            foreach (BasicEnemy enemy in Components.OfType<BasicEnemy>().ToList())
            {
                enemy.Remove = true;
            }
            foreach (EnemyBullet bullet in Components.OfType<EnemyBullet>().ToList())
            {
                bullet.Remove = true;
            }

            player.Position = new Vector2(40, 1000);
            mainTransitionRectangle = new Rectangle(0, 800, 20, 400);
            backgroundTexture = spriteDictionary["medicRoom"];
            previousRoom = RoomState.ParamedicRoom;

            // add pickup
            //Components.Add(new Pickup(this, spriteBatch, Content.Load<Texture2D>("Sprite/pickup2"), new Vector2(1500, 250), 2, player));
            Components.Add(pickUp2);
            if (pickup2Flag == true)
            {
                Components.Remove(pickUp2);
            }
        }

        void EnterPoliceRoom(State<RoomState> obj)
        {
            // remove enemy and bullets on enterance
            foreach (BasicEnemy enemy in Components.OfType<BasicEnemy>().ToList())
            {
                enemy.Remove = true;
            }
            foreach (EnemyBullet bullet in Components.OfType<EnemyBullet>().ToList())
            {
                bullet.Remove = true;
            }

            player.Position = new Vector2(1000, 1960);
            mainTransitionRectangle = new Rectangle(800, 1980, 400, 20);
            backgroundTexture = spriteDictionary["policeRoom"];
            previousRoom = RoomState.PoliceRoom;
            
            // add pickup
            //Components.Add(new Pickup(this, spriteBatch, Content.Load<Texture2D>("Sprite/pickup3"), new Vector2(1500, 500), 3, player));
            Components.Add(pickUp3);
            if (pickup3Flag == true)
            {
                Components.Remove(pickUp3);
            }
        }

        void EnterMain(State<RoomState> obj)
        {
            // remove enemy and bullets on enterance
            foreach (BasicEnemy enemy in Components.OfType<BasicEnemy>().ToList())
            {
                enemy.Remove = true;
            }
            foreach (EnemyBullet bullet in Components.OfType<EnemyBullet>().ToList())
            {
                bullet.Remove = true;
            }

            if (previousRoom == RoomState.PoliceRoom)
            {
                player.Position = new Vector2(1000, 40);
            }
            else if (previousRoom == RoomState.ParamedicRoom)
            {
                player.Position = new Vector2(1960, 1000);
            }
            else if (previousRoom == RoomState.FireFighterRoom)
            {
                player.Position = new Vector2(40, 1000);
            }
            if (previousRoom == RoomState.MainRoom)
            {
                player.Position = new Vector2(1700, 1650);
            }

            foreach (Pickup pickup in Components.OfType<Pickup>().ToList())
            {
                pickup.Remove = true;
            }

            backgroundTexture = spriteDictionary["mainRoom"];
        }

        private void SpawnBullet(InputTypes inputEvent)
        {
            #region WaterGun - Spawn
            //water gun bullet spawn
            if (player.weaponType == WeaponType.WaterGun)
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

            MouseState mouseState = Mouse.GetState();
            var mousePos = new Point(mouseState.X, mouseState.Y);

            //Game State Menu
            if (gameState == GameState.Menu)
            {
                if (playButtonRec.Contains(mousePos))
                {
                    if (mouseState.LeftButton == ButtonState.Pressed && clickable == true)
                    {
                        gameState = GameState.Play;
                        clickable = false;
                        clickTimer.Start(clickTimeSpan);
                    }
                }

                if (quitButtonRec.Contains(mousePos))
                {
                    if (mouseState.LeftButton == ButtonState.Pressed && clickable == true)
                    {
                        Exit();
                    }
                }

                if (helpButtonRec.Contains(mousePos))
                {
                    if (mouseState.LeftButton == ButtonState.Pressed && clickable == true)
                    {
                        gameState = GameState.Help;
                        clickable = false;
                        clickTimer.Start(clickTimeSpan);
                    }
                }

                if (aboutButtonRec.Contains(mousePos))
                {
                    if (mouseState.LeftButton == ButtonState.Pressed && clickable == true)
                    {
                        gameState = GameState.About;
                        clickable = false;
                        clickTimer.Start(clickTimeSpan);
                    }
                }
            }

            if (gameState == GameState.Help)
            {
                if (backButtonRec.Contains(mousePos))
                {
                    if (mouseState.LeftButton == ButtonState.Pressed && clickable == true)
                    {
                        gameState = GameState.Menu;
                        clickable = false;
                        clickTimer.Start(clickTimeSpan);
                    }
                }
            }

            if (gameState == GameState.About)
            {
                if (continueButtonRec.Contains(mousePos))
                {
                    if (mouseState.LeftButton == ButtonState.Pressed && clickable == true)
                    {
                        gameState = GameState.Objective;
                        clickable = false;
                        clickTimer.Start(clickTimeSpan);
                    }
                }
            }

            if (gameState == GameState.Objective)
            {
                if (continueButtonRec.Contains(mousePos))
                {
                    if (mouseState.LeftButton == ButtonState.Pressed && clickable == true)
                    {
                        gameState = GameState.Legend;
                        clickable = false;
                        clickTimer.Start(clickTimeSpan);
                    }
                }
            }

            if (gameState == GameState.Legend)
            {
                if (backButtonRec.Contains(mousePos))
                {
                    if (mouseState.LeftButton == ButtonState.Pressed && clickable == true)
                    {
                        gameState = GameState.Menu;
                        clickable = false;
                        clickTimer.Start(clickTimeSpan);
                    }
                }
            }

            if (gameState == GameState.Dead)
            {
                if (continueButtonRec.Contains(mousePos))
                {
                    if (mouseState.LeftButton == ButtonState.Pressed && clickable == true)
                    {
                        gameState = GameState.Credits;
                        clickable = false;
                        clickTimer.Start(clickTimeSpan);
                    }
                }
            }

            if (gameState == GameState.Credits)
            {
                if (quitButtonRec.Contains(mousePos))
                {
                    if (mouseState.LeftButton == ButtonState.Pressed && clickable == true)
                    {
                        Exit();
                    }
                }
            }

            clickTimer.Update(gameTime.ElapsedGameTime);

            // test what menu player is in, then draw quit button accordingly
            //if (gameState == GameState.Help)
            //    backButtonRec = new Rectangle(400, 350, quitButton.Width, quitButton.Height);
            if (gameState == GameState.Dead)
                continueButtonRec = new Rectangle(550, 350, continueButton.Width, continueButton.Height);
            if (gameState == GameState.Credits)
                quitButtonRec = new Rectangle(550, 350, continueButton.Width, continueButton.Height);
            if (gameState == GameState.About)
                continueButtonRec = new Rectangle(510, 370, continueButton.Width, continueButton.Height);
            if (gameState == GameState.Objective)
                continueButtonRec = new Rectangle(310, 370, continueButton.Width, continueButton.Height);
            if (gameState == GameState.Legend)
                backButtonRec = new Rectangle(310, 370, backButton.Width, backButton.Height);

            IEnumerable<Pedestrian> pedestrians = Components.OfType<Pedestrian>();
            IEnumerable<BasicEnemy> enemies = Components.OfType<BasicEnemy>();
            IEnumerable<BasicBullet> bullets = Components.OfType<BasicBullet>();
            //Game State Play
            if (gameState == GameState.Play)
            {
                //Health Bar
                healthRectangle = new Rectangle(Screen.Width / 12, 20, this.player.Health, 20);
                if (roomStateMachine.Current == RoomState.MainRoom)
                {
                    policeRoomSign = new Rectangle(970, 80, spriteDictionary["policeRoomSign"].Width,
                        spriteDictionary["policeRoomSign"].Height);
                    paramedicRoomSign = new Rectangle(1880, 960, spriteDictionary["paramedicRoomSign"].Width,
                        spriteDictionary["paramedicRoomSign"].Height);
                    fireFighterRoomSign = new Rectangle(80, 930, spriteDictionary["fireFighterRoomSign"].Width,
                       spriteDictionary["fireFighterRoomSign"].Height);
                }

                if (roomStateMachine.Current == RoomState.FireFighterRoom)
                {
                    mainRoomSignRight = new Rectangle(1780, 960, spriteDictionary["mainRoomSignRight"].Width,
                        spriteDictionary["mainRoomSignRight"].Height);
                }

                if (roomStateMachine.Current == RoomState.ParamedicRoom)
                {
                    mainRoomSignLeft = new Rectangle(80, 930, spriteDictionary["mainRoomSignLeft"].Width,
                       spriteDictionary["mainRoomSignLeft"].Height);
                }

                if (roomStateMachine.Current == RoomState.PoliceRoom)
                {
                    mainRoomSignDown = new Rectangle(940, 1860, spriteDictionary["mainRoomSignDown"].Width,
                       spriteDictionary["mainRoomSignDown"].Height);
                }
                //Give enemy player position
                foreach (BasicEnemy enemy in enemies)
                {
                    enemy.GetPlayerPosition(player.Position);
                }

                if (this.player.collisionRectangle.Intersects(garabeAmmoRec) || this.player.collisionRectangle.Intersects(hoseAmmoRec)
                    || this.player.collisionRectangle.Intersects(syringeAmmoRec) || this.player.collisionRectangle.Intersects(donutStandAmmoRec))
                {
                    if (canAmmo && this.player.Ammo <= 300)
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

                else if (this.player.collisionRectangle.Intersects(healthDrawRactangle2))
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

                // update spawns for ammo/health boxes
                // paramedic room
                if (roomStateMachine.Current == RoomState.ParamedicRoom)
                {
                    // ammo boxes
                    garabeAmmoRec = new Rectangle(340, 740, garabeAmmo.Width, garabeAmmo.Height);
                    hoseAmmoRec = new Rectangle(40, 60, hoseAmmo.Width, hoseAmmo.Height);
                    syringeAmmoRec = new Rectangle(1760, 1480, syringeAmmo.Width, syringeAmmo.Height);
                    donutStandAmmoRec = new Rectangle(320, 1260, donutStandAmmo.Width, donutStandAmmo.Height);

                    // health boxes
                    healthDrawRactangle = new Rectangle(960, 80, health.Width, health.Height);
                    healthDrawRactangle2 = new Rectangle(960, 1900, health.Width, health.Height);
                }

                // police room
                if (roomStateMachine.Current == RoomState.PoliceRoom)
                {
                    // ammo boxes
                    garabeAmmoRec = new Rectangle(1340, 1160, garabeAmmo.Width, garabeAmmo.Height);
                    hoseAmmoRec = new Rectangle(40, 1960, hoseAmmo.Width, hoseAmmo.Height);
                    syringeAmmoRec = new Rectangle(1400, 1760, syringeAmmo.Width, syringeAmmo.Height);
                    donutStandAmmoRec = new Rectangle(660, 1160, donutStandAmmo.Width, donutStandAmmo.Height);

                    // health boxes
                    healthDrawRactangle = new Rectangle(80, 1040, health.Width, health.Height);
                    healthDrawRactangle2 = new Rectangle(1870, 1040, health.Width, health.Height);
                }

                // firefighter room
                if (roomStateMachine.Current == RoomState.FireFighterRoom)
                {
                    // ammo boxes
                    garabeAmmoRec = new Rectangle(960, 600, garabeAmmo.Width, garabeAmmo.Height);
                    hoseAmmoRec = new Rectangle(1930, 1940, hoseAmmo.Width, hoseAmmo.Height);
                    syringeAmmoRec = new Rectangle(1740, 620, syringeAmmo.Width, syringeAmmo.Height);
                    donutStandAmmoRec = new Rectangle(1640, 1260, donutStandAmmo.Width, donutStandAmmo.Height);

                    // health boxes
                    healthDrawRactangle = new Rectangle(1000, 1920, health.Width, health.Height);
                    healthDrawRactangle2 = new Rectangle(1000, 80, health.Width, health.Height);
                }

                // main room
                if (roomStateMachine.Current == RoomState.MainRoom)
                {
                    // ammo boxes
                    garabeAmmoRec = new Rectangle(170, 660, garabeAmmo.Width, garabeAmmo.Height);
                    hoseAmmoRec = new Rectangle(1560, 1680, hoseAmmo.Width, hoseAmmo.Height);
                    syringeAmmoRec = new Rectangle(1620, 300, syringeAmmo.Width, syringeAmmo.Height);
                    donutStandAmmoRec = new Rectangle(620, 1340, donutStandAmmo.Width, donutStandAmmo.Height);

                    // health boxes
                    healthDrawRactangle = new Rectangle(480, 420, health.Width, health.Height);
                    healthDrawRactangle2 = new Rectangle(1400, 1560, health.Width, health.Height);
                }
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
                                    enemy.Health -= 50;
                                    enemy.LinearVelocity *= 0.5f;
                                }
                                else if (bullet.weaponType == WeaponType.GernadeLauncher && enemy.enemyType == EnemyType.Police)
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
                                    enemy.Health -= 10;
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
                                    enemy.Health -= 50;
                                    enemy.LinearVelocity *= 0.5f;
                                }
                                else if (bullet.weaponType == WeaponType.GernadeLauncher && enemy.enemyType == EnemyType.Paramedic)
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

                                //turns them off components
                                if (enemy.Health <= 0 && enemy.enemyType != EnemyType.Paramedic)
                                {
                                    //update score
                                    score += 100;
                                    soundEffectInstanceDeathSound.Play();
                                    enemy.Enabled = false;
                                    enemy.Remove = true;
                                    //removals.Add(enemy);
                                }
                                else if (enemy.Health <= 0 && enemy.enemyType == EnemyType.Paramedic)
                                {
                                    //update score
                                    score += 100;
                                    soundEffectInstanceParamedicDeath.Play();
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

                foreach (BasicSprite sprite in Components.OfType<BasicSprite>().Where(sprite => sprite.Remove))
                {
                    removals.Add(sprite);
                }

                //remove list of components to be removed
                foreach (DrawableGameComponent removal in removals)
                {
                    Components.Remove(removal);
                }

                //spawn enemies if spawn timer expires
                while (Components.OfType<BasicEnemy>().ToList().Count <= 15)
                {
                    int spawnNum = enemySpawnNum.Next(1, 100);
                    int spawnX = enemySpawnNum.Next(20, 1980);
                    int spawnY = enemySpawnNum.Next(20, 1980);
                    int spawnSeed = enemySpawnNum.Next(10);

                    if (roomStateMachine.Current == RoomState.MainRoom)
                    {
                        // spawn firefighter
                        if (spawnNum >= 0 && spawnNum <= 33)
                        {
                            FireFighterEnemy enemy = new FireFighterEnemy(this, spriteBatch, spriteDictionary["firefighter"], new Vector2(spawnX, spawnY));
                            enemy.EnemyActionTriggeredEvent += SpawnEnemyBullet;
                            Components.Add(enemy);
                        }

                        else if (spawnNum >= 34 && spawnNum <= 66)
                        {
                            ParamedicEnemy enemy = new ParamedicEnemy(this, spriteBatch, spriteDictionary["paramedic"], new Vector2(spawnX, spawnY));
                            Components.Add(enemy);
                        }

                        else if (spawnSeed >= 67 && spawnSeed <= 98)
                        {
                            PoliceEnemy enemy = new PoliceEnemy(this, spriteBatch, spriteDictionary["police"], new Vector2(spawnX, spawnY), spawnSeed);
                            enemy.EnemyActionTriggeredEvent += SpawnEnemyBullet;
                            Components.Add(enemy);
                        }

                        else
                        {
                            Pedestrian pedrestrian = new Pedestrian(this, spriteBatch, spriteDictionary["pedestrian"], new Vector2(spawnX, spawnY), spawnSeed);
                            Components.Add(pedrestrian);
                        }
                    }

                    // police area spawns
                    if (roomStateMachine.Current == RoomState.PoliceRoom)
                    {
                        // spawn firefighter
                        if (spawnNum >= 0 && spawnNum <= 22)
                        {
                            FireFighterEnemy enemy = new FireFighterEnemy(this, spriteBatch, spriteDictionary["firefighter"], new Vector2(spawnX, spawnY));
                            enemy.EnemyActionTriggeredEvent += SpawnEnemyBullet;
                            Components.Add(enemy);
                        }

                        // spawn paramedic
                        else if (spawnNum >= 23 && spawnNum <= 45)
                        {
                            ParamedicEnemy enemy = new ParamedicEnemy(this, spriteBatch, spriteDictionary["paramedic"], new Vector2(spawnX, spawnY));
                            Components.Add(enemy);
                        }

                        // spawn police
                        else if (spawnNum >= 46 && spawnNum <= 98)
                        {
                            PoliceEnemy enemy = new PoliceEnemy(this, spriteBatch, spriteDictionary["police"], new Vector2(spawnX, spawnY), spawnSeed);
                            enemy.EnemyActionTriggeredEvent += SpawnEnemyBullet;
                            Components.Add(enemy);
                        }

                        else
                        {
                            Pedestrian pedrestrian = new Pedestrian(this, spriteBatch, spriteDictionary["pedestrian"], new Vector2(spawnX, spawnY), spawnSeed);
                            Components.Add(pedrestrian);
                        }
                    }

                    // firefighter area spawns
                    if (roomStateMachine.Current == RoomState.FireFighterRoom)
                    {
                        // spawn firefighter
                        if (spawnNum >= 0 && spawnNum <= 66)
                        {
                            FireFighterEnemy enemy = new FireFighterEnemy(this, spriteBatch, spriteDictionary["firefighter"], new Vector2(spawnX, spawnY));
                            enemy.EnemyActionTriggeredEvent += SpawnEnemyBullet;
                            Components.Add(enemy);
                        }

                        // spawn paramedic
                        else if (spawnNum >= 68 && spawnNum <= 80)
                        {
                            ParamedicEnemy enemy = new ParamedicEnemy(this, spriteBatch, spriteDictionary["paramedic"], new Vector2(spawnX, spawnY));
                            Components.Add(enemy);
                        }

                        // spawn police
                        else if (spawnNum >= 81 && spawnNum <= 98)
                        {
                            PoliceEnemy enemy = new PoliceEnemy(this, spriteBatch, spriteDictionary["police"], new Vector2(spawnX, spawnY), spawnSeed);
                            enemy.EnemyActionTriggeredEvent += SpawnEnemyBullet;
                            Components.Add(enemy);
                        }

                        else
                        {
                            Pedestrian pedrestrian = new Pedestrian(this, spriteBatch, spriteDictionary["pedestrian"], new Vector2(spawnX, spawnY), spawnSeed);
                            Components.Add(pedrestrian);
                        }
                    }

                    // paramedic area spawns
                    if (roomStateMachine.Current == RoomState.ParamedicRoom)
                    {
                        // spawn firefighter
                        if (spawnNum >= 0 && spawnNum <= 45)
                        {
                            FireFighterEnemy enemy = new FireFighterEnemy(this, spriteBatch, spriteDictionary["firefighter"], new Vector2(spawnX, spawnY));
                            enemy.EnemyActionTriggeredEvent += SpawnEnemyBullet;
                            Components.Add(enemy);
                        }

                        // spawn paramedic
                        else if (spawnNum >= 45 && spawnNum <= 75)
                        {
                            ParamedicEnemy enemy = new ParamedicEnemy(this, spriteBatch, spriteDictionary["paramedic"], new Vector2(spawnX, spawnY));
                            Components.Add(enemy);
                        }

                        // spawn police
                        else if (spawnNum >= 75 && spawnNum <= 98)
                        {
                            PoliceEnemy enemy = new PoliceEnemy(this, spriteBatch, spriteDictionary["police"], new Vector2(spawnX, spawnY), spawnSeed);
                            enemy.EnemyActionTriggeredEvent += SpawnEnemyBullet;
                            Components.Add(enemy);
                        }

                        else
                        {
                            Pedestrian pedrestrian = new Pedestrian(this, spriteBatch, spriteDictionary["pedestrian"], new Vector2(spawnX, spawnY), spawnSeed);
                            Components.Add(pedrestrian);
                        }
                    }
                }

                // add all additions
                additions.ForEach(addition => Components.Add(addition));

                //update health and score
                //healthText = HEALTH_STRING + this.player.Health;
                scoreText = SCORE_STRING + score;
                ammoText = AMMO_STRING + this.player.Ammo;

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

                //// temp back to main menu button
                //if (quitButtonRec.Contains(mousePos))
                //{
                //    if (mouseState.LeftButton == ButtonState.Pressed && clickable == true)
                //    {
                //        //gameState = GameState.Menu;
                //        //clickable = false;
                //        //clickTimer.Start(clickTimeSpan);

                //        Exit();
                //    }
                //}
            }


            base.Update(gameTime);

            // update gui
            foreach (GameComponent guiComponent in GUIComponents)
            {
                if (guiComponent.Enabled)
                    guiComponent.Update(gameTime);
            }
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
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            //spriteBatch.Begin(SpriteSortMode.BackToFront, null, null, null, null, null, GetScreenMatrix());
            //spriteBatch.Begin();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.transform);

            ////Game State Menu
            //if (gameState == GameState.Menu)
            //{
            //    spriteBatch.Draw(titleScreen, titleScreenPos, Color.White);
            //    spriteBatch.Draw(playButton, playButtonPos, Color.White);
            //    spriteBatch.Draw(helpButton, helpButtonPos, Color.White);
            //    spriteBatch.Draw(quitButton, quitButtonPos, Color.White);
            //}

            //Game State Play
            if (gameState == GameState.Play)
            {
                // draw background
                spriteBatch.Draw(backgroundTexture, backgroundPosition, Color.White);

                //draw ammo
                spriteBatch.Draw(garabeAmmo, garabeAmmoRec, Color.White);
                spriteBatch.Draw(hoseAmmo, hoseAmmoRec, Color.White);
                spriteBatch.Draw(syringeAmmo, syringeAmmoRec, Color.White);
                spriteBatch.Draw(donutStandAmmo, donutStandAmmoRec, Color.White);

                // draw health boxes
                spriteBatch.Draw(health, healthDrawRactangle, Color.White);
                spriteBatch.Draw(health2, healthDrawRactangle2, Color.White);

                // draw signs
                if (roomStateMachine.Current == RoomState.PoliceRoom)
                    spriteBatch.Draw(spriteDictionary["mainRoomSignDown"], mainRoomSignDown, Color.White);
                if (roomStateMachine.Current == RoomState.ParamedicRoom)
                    spriteBatch.Draw(spriteDictionary["mainRoomSignLeft"], mainRoomSignLeft, Color.White);
                if (roomStateMachine.Current == RoomState.FireFighterRoom)
                    spriteBatch.Draw(spriteDictionary["mainRoomSignRight"], mainRoomSignRight, Color.White);
                if (roomStateMachine.Current == RoomState.MainRoom)
                    spriteBatch.Draw(spriteDictionary["policeRoomSign"], policeRoomSign, Color.White);
                if (roomStateMachine.Current == RoomState.MainRoom)
                    spriteBatch.Draw(spriteDictionary["paramedicRoomSign"], paramedicRoomSign, Color.White);
                if (roomStateMachine.Current == RoomState.MainRoom)
                    spriteBatch.Draw(spriteDictionary["fireFighterRoomSign"], fireFighterRoomSign, Color.White);
            }

            base.Draw(gameTime);
            spriteBatch.End();

            //GUI Draw
            spriteBatch.Begin();

            //Game State Menu
            if (gameState == GameState.Menu)
            {
                spriteBatch.Draw(titleScreen, titleScreenPos, Color.White);
                spriteBatch.Draw(playButton, playButtonRec, Color.White);
                spriteBatch.Draw(helpButton, helpButtonRec, Color.White);
                spriteBatch.Draw(quitButton, quitButtonRec, Color.White);
                spriteBatch.Draw(aboutButton, aboutButtonRec, Color.White);
            }

            if (gameState == GameState.Help)
            {
                spriteBatch.Draw(helpScreen, helpScreenPos, Color.White);
                spriteBatch.Draw(backButton, backButtonRec, Color.White);
            }

            if (gameState == GameState.About)
            {
                spriteBatch.Draw(aboutScreen, aboutScreenPos, Color.White);
                spriteBatch.Draw(continueButton, continueButtonRec, Color.White);
            }

            if (gameState == GameState.Objective)
            {
                spriteBatch.Draw(objectiveScreen, objectiveScreenPos, Color.White);
                spriteBatch.Draw(continueButton, continueButtonRec, Color.White);
            }

            if (gameState == GameState.Legend)
            {
                spriteBatch.Draw(legendScreen, legendScreenPos, Color.White);
                spriteBatch.Draw(backButton, backButtonRec, Color.White);
            }

            //Game State Play
            if (gameState == GameState.Play)
            {
                //GUI hud
                spriteBatch.Draw(spriteDictionary["bar"], hudRectangle, Color.Goldenrod);

                //Draw score
                spriteBatch.DrawString(scoreFont, scoreText, scoreTextLocation, Color.Black);

                // weapon icons
                if (Player.weaponIconFlag == 1)
                    spriteBatch.Draw(waterGunIcon, gunIconPos, Color.White);
                if (Player.weaponIconFlag == 2)
                    spriteBatch.Draw(slingShotIcon, gunIconPos, Color.White);
                if (Player.weaponIconFlag == 3)
                    spriteBatch.Draw(donutGunIcon, gunIconPos, Color.White);
                if (Player.weaponIconFlag == 4)
                    spriteBatch.Draw(machineGunIcon, gunIconPos, Color.White);
                if (Player.weaponIconFlag == 5)
                    spriteBatch.Draw(sniperRifleIcon, gunIconPos, Color.White);
                if (Player.weaponIconFlag == 6)
                    spriteBatch.Draw(gernadeLauncherIcon, gunIconPos, Color.White);

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
                    spriteBatch.DrawString(scoreFont, ammoText, ammoTextLocation, Color.Red);
                }
                else if (this.player.Ammo <= 25)
                {
                    spriteBatch.DrawString(scoreFont, ammoText, ammoTextLocation, Color.Yellow);
                }
                else
                {
                    spriteBatch.DrawString(scoreFont, ammoText, ammoTextLocation, Color.Green);
                }
            }
            //draw gui
            foreach (DrawableGameComponent guiComponent in GUIComponents.OfType<DrawableGameComponent>())
            {
                guiComponent.Draw(gameTime);
            }

            //Game State Dead
            if (gameState == GameState.Dead)
            {
                spriteBatch.Draw(gameOver, drawRectangle, Color.White);
                spriteBatch.DrawString(scoreFont, scoreText, scoreTextLocation2, Color.Red);
                spriteBatch.Draw(continueButton, continueButtonRec, Color.White);
            }

            if (gameState == GameState.Credits)
            {
                spriteBatch.Draw(creditsScreen, creditsScreenPos, Color.White);
                spriteBatch.Draw(quitButton, quitButtonRec, Color.White);
            }
            spriteBatch.End();
        }
    }
    public enum GameState { Menu, Help, Legend, About, Objective, Play, Dead, Credits }
    public enum RoomState { MainRoom, PoliceRoom, FireFighterRoom, ParamedicRoom }
}