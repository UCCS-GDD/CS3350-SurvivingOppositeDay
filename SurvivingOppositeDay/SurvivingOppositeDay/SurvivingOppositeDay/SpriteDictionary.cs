using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurvivingOppositeDay
{
    public class SpriteDictionary
    {
        Dictionary<string, Texture2D> dictionary = new Dictionary<string, Texture2D>();
        ContentManager contentManager;

        public SpriteDictionary(ContentManager contentManager)
        {
            this.contentManager = contentManager;
        }

        public void Add(string name, string filePath)
        {
            dictionary.Add(name, contentManager.Load<Texture2D>(filePath));
        }

        public Texture2D this[string name]
        {
            get { return dictionary[name]; }
        }
    }
}
