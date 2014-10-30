using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace SurvivingOppositeDay
{
    public class Sprite :  DrawableGameComponent
    {
        public String textureFile;
        public Texture2D texture;
        public Vector2 position;
        public Vector2 center;
        public Color color;
        Random random;
        Game game;

        public Sprite(Game game, String textureFile, Vector2 position) 
            : base(game)
        {
            this.textureFile = textureFile;
            this.position = position;
            color = Color.White;
            random = new Random();
            this.game = game;
        }
        
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization logic here
            texture = game.Content.Load<Texture2D>(textureFile);
            center = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            

            base.LoadContent();
            // TODO: use this.Content to load your game content here
        }


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            
            // TODO: Add your drawing code here
            SpriteBatch sb = ((Game1)this.Game).spriteBatch;
            sb.Begin();
            sb.Draw(texture, position, null, color, 0f, center, 1.0f, SpriteEffects.None, 0f);
            sb.End();

            base.Draw(gameTime);
        }

        public SpriteSave SaveSprite()
        {
            SpriteSave spriteSave = new SpriteSave(textureFile, position, color);
            return spriteSave;
        }

        public void Loadsprite(SpriteSave spriteSave)
        {
            textureFile = spriteSave.textureFile;
            position = spriteSave.position;
            color = spriteSave.color;
        }

        [Serializable()]
        public class SpriteSave
        {
            public String textureFile;
            public Vector2 position;
            public Color color;

            public SpriteSave(String textureFile, Vector2 position, Color color)
            {
                this.textureFile = textureFile;
                this.position = position;
                this.color = color;
            }
        }
    }
}
