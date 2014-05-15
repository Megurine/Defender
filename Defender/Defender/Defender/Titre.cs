using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Defender
{
    class Titre
    {
        // FIELDS
        public string nom;
        public Color couleur;

        // CONSTRUCTOR
        public Titre(string nom, Color clr)
        {
            this.nom = nom;
            this.couleur = clr;
        }

        // METHOD


        // UPDATE & DRAW

        public void Draw(SpriteBatch spriteBatch, Rectangle hitboxevent)
        {
            int x = 0;
            int y = 0;
            Vector2 taille = Ressources.Fonts[0].MeasureString(this.nom);
            x = (hitboxevent.X + (hitboxevent.Width / 2)) - (((int)taille.X) / 2);
            y = hitboxevent.Y - (int)taille.Y;
            taille = new Vector2(x, y);
            string nom = this.nom[0].ToString().ToUpper() + this.nom.Substring(1).ToLower();
            spriteBatch.DrawString(Ressources.Fonts[1], nom, taille, new Color(this.couleur.R - 50, this.couleur.G - 50, this.couleur.B - 50));
            //taille = new Vector2(x - 1, y - 1);
            //spriteBatch.DrawString(Ressources.Fonts[1], this.nom, taille, this.couleur); //OMBRE PORTEE
        }
    }
}
