using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurvivingOppositeDay
{
    public static class RoomManager
    {
        private static Dictionary<string, Room> rooms;
        public static Room Current { get; private set; }

        //public RoomManager()
        //{

        //}

        public static void AddRoom (string name)
        {
            Room room = new Room();
            rooms.Add(name, room);
        }

        public static void AddTransition (string name, Func<bool> transition)
        {
            
        }

        public static void Update(GameTime gameTime)
        {
            Current.Update(gameTime);
        }

        public static void Draw(GameTime gameTime)
        {

        }
    }
}
