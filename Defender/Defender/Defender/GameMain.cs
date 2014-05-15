using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Defender
{
    class GameMain
    {
        // FIELDS

        // CONSTRUCTOR
        public GameMain(GraphicsDevice GraphicsDevice)
        {
            Ressources.mapLocale = new Map("local", 100 * Ressources.tailleCube, 2000 * Ressources.tailleCube);

            Arme[] armes = new Arme[3];
            armes[0] = Ressources.armes[0];
            armes[1] = Ressources.armes[1];
            armes[2] = Ressources.armes[2];

            Ressources.tirs.Clear();

            Ressources.Players.Add(new Player(true, 1, 7, 8, 5, 6, 3, 4, Ressources.nomClient, armes));
            //Ressources.Players.Add(new Player(false, 2, 7, 8, 5, 6, 3, 4, "Joueur2", armes));
        }

        // METHODS

        // UPDATE
        public void Update(MouseState mouse, KeyboardState keyboard, GameTime gametime)
        {
            Ressources.mapLocale.Update();

            Ressources.texteAffichageVariable = "Touches : \n{ESDF.Fleches} : Se deplacer\n{MouseLeft/Right} : Poser/Enlever des blocs\n{H} : Afficher/Cacher Hitbox collision\n{A} : Changer d'arme\n\nStats : \n";
            foreach (Player player in Ressources.Players)
            {
                if (player.joueurLocal)
                {
                    Ressources.camera.Update(gametime, player);
                }
                player.Update(mouse, keyboard, gametime);
            }

            //TIRS
            List<Tir> tirsTemp = new List<Tir>();
            foreach (Tir tir in Ressources.tirs)
            {
                tir.Update(mouse, keyboard, gametime);
                if (tir.porteeActuelle >= tir.porteeMax)
                {
                    tirsTemp.Add(tir);
                }
            }
            foreach (Tir tir in tirsTemp)
            {
                Ressources.tirs.Remove(tir);
            }

            Ressources.mapLocale.Update();
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gametime)
        {
            spriteBatch.GraphicsDevice.SetRenderTarget(null);
            spriteBatch.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            //------------------------------------------START-------------------------------------------------

            //MAP
            Ressources.mapLocale.Draw(spriteBatch);

            //PLAYER
            foreach (Player player in Ressources.Players)
            {
                player.Draw(spriteBatch, false);
            }

            //TIRS
            foreach (Tir tir in Ressources.tirs)
            {
                tir.Draw(spriteBatch);
            }

            //TITRE
            foreach (Player player in Ressources.Players)
            {
                player.Draw(spriteBatch, true);
            }

            //------------------------------------------VAR--------------------------------------------------

            spriteBatch.DrawString(Ressources.Fonts[0], Ressources.texteAffichageVariable, new Vector2(40, 40), new Color((byte)(106), (byte)(201), (byte)(242))); //Coordonnées

            //-------------------------------------END-------------------------------------------------------
            spriteBatch.End();
        }
    }
}
