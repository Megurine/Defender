using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Defender
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        GameMain Main;
        LoginMain Login;

        //private SpriteFont _font;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            this.IsMouseVisible = true;
            base.Initialize();
        }

        protected override void LoadContent()
        {

            spriteBatch = new SpriteBatch(GraphicsDevice);
            Ressources.LoadContent(Content);
            Login = new LoginMain(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
            Content.Unload();
        }

        protected override void Update(GameTime gameTime)
        {
            Ressources.WindowActiveElement = IsActive;
            if (Ressources.EtatActif == 0 && Ressources.finInitialisation)
            {
                if (Main == null)
                {
                    Main = new GameMain(GraphicsDevice);
                }
                Main.Update(Mouse.GetState(), Keyboard.GetState(), gameTime);
            }
            else if (Ressources.EtatActif == 1)
            {
                Login.Update(Mouse.GetState(), Keyboard.GetState(), gameTime);
            }
            double FPS = 1000.0d / gameTime.ElapsedGameTime.TotalMilliseconds;
            string texte = string.Format("{0:00.00} Fps", FPS);
            this.Window.Title = Ressources.lesEtats[Ressources.EtatActif].TitreFenetre + " - " + texte;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            this.graphics.PreferredBackBufferWidth = Ressources.lesEtats[Ressources.EtatActif].TailleFenetre.Width;
            this.graphics.PreferredBackBufferHeight = Ressources.lesEtats[Ressources.EtatActif].TailleFenetre.Height;
            this.graphics.ApplyChanges();
            GraphicsDevice.Clear(Color.Black);

            if (Ressources.EtatActif == 0 && Ressources.finInitialisation)
            {
                if (Main == null)
                {
                    Main = new GameMain(GraphicsDevice);
                }
                Main.Draw(spriteBatch, gameTime);
            }
            else if (Ressources.EtatActif == 1)
            {
                Main = null;
                Login.Draw(spriteBatch, gameTime);
            }

            base.Draw(gameTime);
        }
    }
}
