using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurvivingOppositeDay
{
    public class CollisionCollection
    {
        public RoomState Room { get; private set; }
        public List<Rectangle> Colliders { get; private set; }

        public CollisionCollection(RoomState room)
        {
            Room = room;
            Colliders = new List<Rectangle>();
        }
    }
}
