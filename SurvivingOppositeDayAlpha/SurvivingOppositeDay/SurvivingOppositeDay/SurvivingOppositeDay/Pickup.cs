using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurvivingOppositeDay
{
    class Pickup : BasicSprite
    {
        int type;
        Player player;
        public Pickup(Game game, SpriteBatch spriteBatch, Texture2D texture, Vector2 position, bool add, int type, Player player)
            :base(game,spriteBatch,texture,position,add)
        {
            this.type = type;
            this.player = player;
        }

        public override void Update(GameTime gameTime)
        {
            if (collisionRectangle.Intersects(player.collisionRectangle))
            {
                player.pickups[this.type] = 1;
                if (type == 1)
                {
                    player.weaponType = WeaponType.WaterGun;
                    Player.weaponIconFlag = 1;
                }
                    
                if (type == 2)
                {
                    player.weaponType = WeaponType.SlingShot;
                    Player.weaponIconFlag = 2;
                }
                    
                if (type == 3)
                {
                    player.weaponType = WeaponType.DonutGun;
                    Player.weaponIconFlag = 3;
                }
                    
                this.Remove = true;
            }
            base.Update(gameTime);
        }
    }
}
