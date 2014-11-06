using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurvivingOppositeDay
{
    public class SoundDictionary
    {
        Dictionary<string, SoundEffect> dictionary = new Dictionary<string, SoundEffect>();
        ContentManager contentManager;

        public SoundDictionary(ContentManager contentManager)
        {
            this.contentManager = contentManager;
        }

        public void Add(string name, string filePath)
        {
            dictionary.Add(name, contentManager.Load<SoundEffect>(filePath));
        }

        public SoundEffect this[string name]
        {
            get { return dictionary[name]; }
        }
    }
}
