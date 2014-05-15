using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Defender
{
    class Tir
    {
        public double rapidite;

        public double dernierGametimePourDeplacement;

        public string pseudoTireur;

        public int numeroTexture;

        public int sourceX;
        public int sourceY;

        public Rectangle Hitbox;

        public int[] Mouse;

        public double angleDegree;
        public float radius;

        public int porteeActuelle;
        public int porteeMax;

        public float opacite;

        public Tir(double rapidite, string pseudoTireur, int numeroTexture, int sourceX, int sourceY, double angleDegree, int porteeMax, int[] Mouse)
        {
            this.rapidite = rapidite;

            this.dernierGametimePourDeplacement = 0;

            this.pseudoTireur = pseudoTireur;

            this.numeroTexture = numeroTexture;

            this.sourceX = sourceX;
            this.sourceY = sourceY;

            this.Mouse = Mouse;
            this.Mouse[0] += Ressources.camera.focus.X;
            this.Mouse[1] += Ressources.camera.focus.Y;

            this.Hitbox = new Rectangle(0, 0, 4, 4);

            this.angleDegree = angleDegree;
            this.radius = 0 - MathHelper.ToRadians((float)(angleDegree));

            this.porteeActuelle = 0;
            this.porteeMax = porteeMax / 2;

            this.opacite = 1f;
        }

        // UPDATE
        public void Update(MouseState Mouse, KeyboardState keyboard, GameTime gametime)
        {
            if (dernierGametimePourDeplacement == 0)
            {
                dernierGametimePourDeplacement = gametime.TotalGameTime.TotalMilliseconds;
            }

            double differenceTemps = gametime.TotalGameTime.TotalMilliseconds - dernierGametimePourDeplacement;
            if (differenceTemps >= 5) //sec
            {
                dernierGametimePourDeplacement = gametime.TotalGameTime.TotalMilliseconds;

                this.porteeActuelle += (int)(this.rapidite / 200);


                verifierCollision(Mouse);

            }

            
        }

        //DRAW
        public void Draw(SpriteBatch spriteBatch)
        {
            this.opacite = (float)((1 - ((double)(this.porteeActuelle) / (double)(this.porteeMax)))/16);
            spriteBatch.Draw(Ressources.TexturesTirs[this.numeroTexture], new Rectangle(sourceX - Ressources.camera.focus.X, sourceY - Ressources.camera.focus.Y, this.porteeActuelle, 1), new Rectangle(0, 0, 1, 1), Color.White * this.opacite, this.radius, new Vector2(0, 0), SpriteEffects.None, 0);

            spriteBatch.Draw(Ressources.TexturesTirs[this.numeroTexture], new Rectangle(Hitbox.X - Ressources.camera.focus.X, Hitbox.Y - Ressources.camera.focus.Y, Hitbox.Width, Hitbox.Height), new Rectangle(0, 0, 1, 1), Color.White);
        }

        public bool verifierCollision(MouseState Mouse)
        {
            bool collision = false;

            double mouseX = this.Mouse[0];
            double mouseY = Ressources.mapLocale.hauteur - this.Mouse[1];

            double LaSourceX = this.sourceX;
            double LaSourceY = Ressources.mapLocale.hauteur - this.sourceY;

            double longueurSourceMouse = Math.Sqrt(Math.Pow((LaSourceX - mouseX), 2) + Math.Pow((LaSourceY - mouseY), 2));

            Hitbox.X = (int)Math.Round((Math.Cos(MathHelper.ToRadians((float)(this.angleDegree))) * this.porteeActuelle) + LaSourceX, 0);
            Hitbox.Y = Ressources.mapLocale.hauteur - (int)Math.Round((Math.Sin(MathHelper.ToRadians((float)(this.angleDegree))) * this.porteeActuelle) + LaSourceY, 0);

            Hitbox.X = Hitbox.X - (Hitbox.Width / 2);
            Hitbox.Y = Hitbox.Y - (Hitbox.Height / 2);

            if (Hitbox.X >= Ressources.mapLocale.largeur || Hitbox.X <= 0 || Hitbox.Y >= Ressources.mapLocale.hauteur || Hitbox.Y <= 0)
            {
                this.porteeActuelle = this.porteeMax;
            }

            int xHitbot = (int)Math.Floor((double)(this.Hitbox.X / Ressources.tailleCube));
            int yHitbot = (int)Math.Floor((double)(this.Hitbox.Y / Ressources.tailleCube));

            if (xHitbot == Ressources.mapLocale.largeur / Ressources.tailleCube)
            {
                xHitbot--;
            }
            if (yHitbot == Ressources.mapLocale.hauteur / Ressources.tailleCube)
            {
                yHitbot--;
            }

            if (Ressources.mapLocale.Cubes[xHitbot, yHitbot])
            {
                if (this.Hitbox.Intersects(new Rectangle(xHitbot * Ressources.tailleCube, yHitbot * Ressources.tailleCube, Ressources.tailleCube, Ressources.tailleCube)))
                {
                    collision = true;
                    Ressources.mapLocale.retirerCube(xHitbot * Ressources.tailleCube, yHitbot * Ressources.tailleCube, false);
                    this.porteeActuelle = this.porteeMax;
                }
            }


            //int i = 0;
            //while (i < CoordsCubes.Count() && !collision)
            //{

            //    //Ressources.mapLocale.CoordsCubes.Clear();
            //    if (this.Hitbox.Intersects(new Rectangle(CoordsCubes[i][0] * Ressources.tailleCube, CoordsCubes[i][1] * Ressources.tailleCube, Ressources.tailleCube, Ressources.tailleCube)))
            //    {
            //        collision = true;
            //        Ressources.mapLocale.retirerCube(CoordsCubes[i][0] * Ressources.tailleCube, CoordsCubes[i][1] * Ressources.tailleCube, false);
            //        this.porteeActuelle = this.porteeMax;
            //    }
            //    i++;
            //}

            return collision;
        }
    }
}
