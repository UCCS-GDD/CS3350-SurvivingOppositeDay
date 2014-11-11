using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SurvivingOppositeDay
{
    public class Room
    {
        private string name;

        public static Dictionary<string, Func<bool>> transitions;

        public GameComponentCollection Components { get; private set; }

        public void Update(GameTime gameTime)
        {
            foreach (GameComponent component in Components)
            {
                if (component.Enabled)
                {
                    component.Update(gameTime); 
                }
            }
           
        }

        public void Draw(GameTime gameTime)
        {
           
        }
    }
}
