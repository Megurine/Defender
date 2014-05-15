using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Defender
{
    class LoginMain
    {
        // FIELDS

        // CONSTRUCTOR
        public LoginMain(GraphicsDevice GraphicsDevice)
        {
            Ressources.Players.Clear();
        }

        // METHODS

        // UPDATE
        public void Update(MouseState mouse, KeyboardState keyboard, GameTime gametime)
        {
            Ressources.texteAffichageVariable = "Appuyez sur Espace pour se connecter\n\n" + Ressources.erreur;

            if (keyboard.IsKeyDown(Keys.Space) && !Ressources.EnConnexion)
            {
                Ressources.Connexion();
            }

        }

        public void Draw(SpriteBatch spriteBatch, GameTime gametime)
        {
            spriteBatch.GraphicsDevice.SetRenderTarget(null);
            spriteBatch.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            //------------------------------------------START-------------------------------------------------



            //------------------------------------------VAR--------------------------------------------------

            spriteBatch.DrawString(Ressources.Fonts[0], Ressources.texteAffichageVariable, new Vector2(40, 40), new Color((byte)(106), (byte)(201), (byte)(242))); //Coordonnées

            //-------------------------------------END-------------------------------------------------------
            spriteBatch.End();
        }
    }
}
