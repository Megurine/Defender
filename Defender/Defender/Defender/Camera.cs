using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Defender
{
    class Camera
    {
        //FIELDS
        public Rectangle focus;

        //CONSTRUCTOR
        public Camera(int x, int y, int largeur, int hauteur)
        {
            this.focus = new Rectangle(x, y, largeur, hauteur);
        }

        //UPDATE & DRAW
        public void Update(GameTime gametime, Player player)
        {
            this.focus.X = (player.Hitbox.X + (player.Hitbox.Width / 2)) - (this.focus.Width / 2);
            this.focus.Y = (player.Hitbox.Y + (player.Hitbox.Height / 2)) - (this.focus.Height / 2);
        }
    }
}
