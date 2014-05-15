using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Defender
{
    public enum Direction
    {
        Up, Down, Left, Right
    };

    class Player
    {
        //FIELDs

        public bool joueurLocal;

        public int numeroEquipe;

        double vitesse;
        public Rectangle Hitbox;
        public Rectangle HitboxPourCollision;
        bool entrainDeSauter;
        bool entrainDeTomber;
        bool entrainDeTirer;

        double dernierGametimePourDeplacementX;
        double firstGametimePourDeplacementX;
        double dernierGametimePourDeplacementY;
        double firstGametimePourDeplacementY;

        double valeurSautActuellement;
        double valeurChuteActuellement;

        int numeroTextureBasMarcheGauche;
        int numeroTextureBasMarcheDroite;
        int numeroTextureBasAfkGauche;
        int numeroTextureBasAfkDroite;
        int numeroTextureHautAfkGauche;
        int numeroTextureHautAfkDroite;
        int numeroTextureHaut;
        int numeroTextureBas;
        int frameTextureHaut;
        int frameTextureBas;
        double firstGametimePourAnimation;
        double dernierGametimePourAnimationBas;
        double dernierGametimePourAnimationHaut;

        MouseState ancienMouse;
        Keys[] ancienKeys;

        public string nom;
        public Titre titre;
        public Arme[] armes;
        public int armeActuelle;

        public int porteePosageCube;
        public float rotationHaut = 0.0f;
        public double angle;

        // CONSTRUCTOR
        public Player(bool joueurLocal, int numeroEquipe, int numeroTextureHautAfkGauche, int numeroTextureHautAfkDroite, int numeroTextureBasMarcheGauche, int numeroTextureBasMarcheDroite, int numeroTextureBasAfkGauche, int numeroTextureBasAfkDroite, string nom, Arme[] armes)
        {
            this.joueurLocal = joueurLocal;

            this.numeroEquipe = numeroEquipe;

            this.vitesse = Ressources.vitesseDefautPlayer;
            this.Hitbox = new Rectangle(100, 100, 53, 38);
            this.HitboxPourCollision = new Rectangle(15, 6, 23, 32);
            Ressources.hitboxPlayerLocal[0] = this.Hitbox.X;
            Ressources.hitboxPlayerLocal[1] = this.Hitbox.Y;
            this.entrainDeSauter = false;
            this.entrainDeTomber = false;
            this.entrainDeTirer = false;

            this.dernierGametimePourDeplacementX = 0;
            this.firstGametimePourDeplacementX = 0;
            this.dernierGametimePourDeplacementY = 0;
            this.firstGametimePourDeplacementY = 0;

            this.valeurSautActuellement = 0;
            this.valeurChuteActuellement = 0;

            this.numeroTextureBasMarcheGauche = numeroTextureBasMarcheGauche;
            this.numeroTextureBasMarcheDroite = numeroTextureBasMarcheDroite;
            this.numeroTextureBasAfkGauche = numeroTextureBasAfkGauche;
            this.numeroTextureBasAfkDroite = numeroTextureBasAfkDroite;
            this.numeroTextureHautAfkGauche = numeroTextureHautAfkGauche;
            this.numeroTextureHautAfkDroite = numeroTextureHautAfkDroite;
            this.numeroTextureHaut = numeroTextureHautAfkDroite;
            this.numeroTextureBas = numeroTextureBasAfkDroite;
            this.frameTextureHaut = 0;
            this.frameTextureBas = 0;
            this.dernierGametimePourAnimationBas = 0;
            this.dernierGametimePourAnimationHaut = 0;
            this.firstGametimePourAnimation = 0;

            this.nom = nom;
            this.titre = new Titre(this.nom, new Color((byte)(94), (byte)(92), (byte)(255)));
            this.armes = armes;
            this.armeActuelle = 0;

            this.porteePosageCube = Ressources.porteePosageCube;
        }

        //METHODS

        public void changerNom(string pseudo)
        {
            this.nom = pseudo;
            this.titre = new Titre(this.nom, new Color((byte)(94), (byte)(92), (byte)(255)));
        }

        public void sauter(double totalMilliseconds)
        {
            if (!entrainDeTomber)
            {
                if (this.valeurSautActuellement >= Ressources.hauteurSaut)
                {
                    this.valeurSautActuellement = 0;
                    entrainDeSauter = false;
                    dernierGametimePourDeplacementY = 0;
                }
                else
                {
                    double fes = (this.valeurSautActuellement / Ressources.hauteurSaut);

                    double gravite = Ressources.propulsionSaut - (Ressources.rationGraviteSaut * fes);


                    double distance = calculerDistancePixelParRapportAuDernierGametimeY(totalMilliseconds, gravite);
                    int distancePixel = (int)Math.Round(distance, 0);

                    bool stop = false;
                    while (!stop && seraEnCollision(Direction.Up, distancePixel, false))
                    {
                        distancePixel -= 1;
                        if (distancePixel <= 0)
                        {
                            distancePixel = 0;
                            stop = true;
                            this.valeurSautActuellement = Ressources.hauteurSaut;
                        }
                    }


                    this.Hitbox.Y -= distancePixel;

                    this.valeurSautActuellement += distancePixel;
                }
            }
        }

        public void tomber(double totalMilliseconds)
        {
            if (!entrainDeSauter)
            {

                double gravite = 1 + (this.valeurChuteActuellement * Ressources.rationGraviteChute);

                if (gravite > Ressources.vitesseDefautMaxChute)
                {
                    gravite = Ressources.vitesseDefautMaxChute;
                }

                double distance = calculerDistancePixelParRapportAuDernierGametimeY(totalMilliseconds, gravite);
                int distancePixel = (int)Math.Round(distance, 0);

                while (seraEnCollision(Direction.Down, distancePixel, false))
                {
                    distancePixel -= 1;
                }

                this.Hitbox.Y += distancePixel;

                this.valeurChuteActuellement += distancePixel;
            }
        }

        public void accroupir()
        {
            if (!entrainDeTomber && !entrainDeSauter)
            {
                //Ressources.texteAffichageVariable += "Accroupi";
            }
        }

        public void deplacementGaucheDroite(bool gauche, double totalMilliseconds, MouseState mouse)
        {
            bool dansLesAir = false; 
            if (entrainDeTomber || entrainDeSauter)
            {
                dansLesAir = true;
            }

            if (this.dernierGametimePourAnimationBas == 0)
            {
                this.dernierGametimePourAnimationBas = this.firstGametimePourAnimation;
            }

            double differenceTemps = totalMilliseconds - this.dernierGametimePourAnimationBas;
            bool animer = false;
            if (differenceTemps >= Ressources.tempsEntreFramePlayer)
            {
                animer = true;
                this.dernierGametimePourAnimationBas = totalMilliseconds;
            }
            

            if (gauche)
            {
                if (mouse.LeftButton != ButtonState.Pressed)
                {
                    this.numeroTextureHaut = this.numeroTextureHautAfkGauche;
                    this.rotationHaut = (float)(Math.PI);
                }

                this.numeroTextureBas = this.numeroTextureBasMarcheGauche;
                if (animer)
                {
                    this.frameTextureBas++;
                    if (this.frameTextureBas >= (Ressources.Textures[this.numeroTextureBas].Height / this.Hitbox.Height))
                    {
                        this.frameTextureBas = 4;
                    }
                }

                double distance = calculerDistancePixelParRapportAuDernierGametimeX(totalMilliseconds, dansLesAir);
                int distancePixel = (int)Math.Round(distance, 0);

                bool stop = false;
                while (!stop && seraEnCollision(Direction.Left, distancePixel, false))
                {
                    distancePixel -= 1;
                    if (distancePixel <= 0)
                    {
                        distancePixel = 0;
                        stop = true;
                    }
                }

                this.Hitbox.X -= distancePixel;
            }
            else
            {
                if (mouse.LeftButton != ButtonState.Pressed)
                {
                    this.numeroTextureHaut = this.numeroTextureHautAfkDroite;
                    this.rotationHaut = 0f;
                }

                this.numeroTextureBas = this.numeroTextureBasMarcheDroite;
                if (animer)
                {
                    this.frameTextureBas++;
                    if (this.frameTextureBas >= (Ressources.Textures[this.numeroTextureBas].Height / this.Hitbox.Height))
                    {
                        this.frameTextureBas = 4;
                    }
                }

                double distance = calculerDistancePixelParRapportAuDernierGametimeX(totalMilliseconds, dansLesAir);
                int distancePixel = (int)Math.Round(distance, 0);

                bool stop = false;
                while (!stop && seraEnCollision(Direction.Right, distancePixel, false))
                {
                    distancePixel -= 1;
                    if (distancePixel <= 0)
                    {
                        distancePixel = 0;
                        stop = true;
                    }
                }

                this.Hitbox.X += distancePixel;
            }
        }

        private int calculerDistancePixelParRapportAuDernierGametimeX(double totalMilliseconds, bool pendantSaut)
        {
            if (dernierGametimePourDeplacementX == 0)
            {
                dernierGametimePourDeplacementX = firstGametimePourDeplacementX;
            }

            double differenceTemps = totalMilliseconds - dernierGametimePourDeplacementX;
            dernierGametimePourDeplacementX = totalMilliseconds;

            double distance = 0;
            if (pendantSaut)
            {
                distance = differenceTemps * ((Ressources.vitesseDefautPlayer / Ressources.rationVitessePendantSaut) / 1000);
            }
            else
            {
                distance = differenceTemps * (Ressources.vitesseDefautPlayer / 1000);
            }
            Ressources.texteAffichageVariable += "[X : " + (Math.Round(distance, 1)).ToString() + "]";
            return (int)Math.Round(distance, 0);
        }

        private int calculerDistancePixelParRapportAuDernierGametimeY(double totalMilliseconds, double PourcentdistanceRestante)
        {
            if (dernierGametimePourDeplacementY == 0)
            {
                dernierGametimePourDeplacementY = firstGametimePourDeplacementY;
            }

            double differenceTemps = totalMilliseconds - dernierGametimePourDeplacementY;
            dernierGametimePourDeplacementY = totalMilliseconds;

            double distance = differenceTemps * ((Ressources.vitesseDefautPlayer * PourcentdistanceRestante) / 1000);
            Ressources.texteAffichageVariable += "[Y : " + (Math.Round(distance, 1)).ToString() + "]";
            return (int)Math.Round(distance, 0);
        }

        private bool seraEnCollision(Direction DirectionPlayerLocal, int distancePixel, bool corpsEntier)
        {
            bool enCol = false;
            int tCube = Ressources.tailleCube;
            int corps = 0;
            switch (DirectionPlayerLocal)
            {
                case Direction.Up:
                    if (corpsEntier)
                    {
                        corps = this.HitboxPourCollision.Height;
                    }

                    foreach (int[] cube in Ressources.mapLocale.CoordsCubes)
                    {
                        if (new Rectangle(this.Hitbox.X + this.HitboxPourCollision.X, this.Hitbox.Y + this.HitboxPourCollision.Y - distancePixel, this.HitboxPourCollision.Width, corps).Intersects(new Rectangle(cube[0] * tCube, cube[1] * tCube, tCube, tCube)))
                        {
                            enCol = true;
                        }
                    }

                    if(!enCol)
                    {
                        if (new Rectangle(this.Hitbox.X + this.HitboxPourCollision.X, this.Hitbox.Y + this.HitboxPourCollision.Y - distancePixel, this.HitboxPourCollision.Width, corps).Intersects(new Rectangle(-1, 0 - (int)Ressources.vitesseDefautPlayer, Ressources.mapLocale.largeur, (int)Ressources.vitesseDefautPlayer)))
                        {
                            enCol = true;
                        }
                    }


                    break;
                case Direction.Down:

                    foreach (int[] cube in Ressources.mapLocale.CoordsCubes)
                    {
                        if (new Rectangle(this.Hitbox.X + this.HitboxPourCollision.X, this.Hitbox.Y + this.HitboxPourCollision.Y + this.HitboxPourCollision.Height + distancePixel, this.HitboxPourCollision.Width, 0).Intersects(new Rectangle(cube[0] * tCube, cube[1] * tCube, tCube, tCube)))
                        {
                            enCol = true;
                        }
                    }

                    if(!enCol)
                    {
                        if (new Rectangle(this.Hitbox.X + this.HitboxPourCollision.X, this.Hitbox.Y + this.HitboxPourCollision.Y + this.HitboxPourCollision.Height + distancePixel, this.HitboxPourCollision.Width, 0).Intersects(new Rectangle(0, Ressources.mapLocale.hauteur, Ressources.mapLocale.largeur, (int)Ressources.vitesseDefautPlayer)))
                        {
                            enCol = true;
                        }
                    }
                    break;
                case Direction.Left:
                    foreach (int[] cube in Ressources.mapLocale.CoordsCubes)
                    {

                        if (new Rectangle(this.Hitbox.X + this.HitboxPourCollision.X - distancePixel, this.Hitbox.Y + this.HitboxPourCollision.Y, 0, this.HitboxPourCollision.Height).Intersects(new Rectangle(cube[0] * tCube, cube[1] * tCube, tCube, tCube)))
                        {
                            //STEPSIZE
                            if (this.Hitbox.Y + this.HitboxPourCollision.Y + this.HitboxPourCollision.Height - ((cube[1] * tCube) + tCube) < Ressources.stepSize)
                            {
                                bool stop = false;
                                int h = 1;
                                while (!stop && h <= this.HitboxPourCollision.Height)
                                {
                                    if (Ressources.mapLocale.returnSiCube((cube[0] * tCube), (cube[1] * tCube) - h))
                                    {
                                        enCol = true;
                                        stop = true;
                                    }
                                    h++;
                                }
                                if (!stop)
                                {
                                    enCol = false;
                                    this.Hitbox.Y -= Ressources.stepSize;
                                }
                            }
                            else
                            {
                                enCol = true;
                            }
                        }

                    }

                    if(!enCol)
                    {
                        if (new Rectangle(this.Hitbox.X + this.HitboxPourCollision.X - distancePixel, this.Hitbox.Y + this.HitboxPourCollision.Y, 0, this.HitboxPourCollision.Height).Intersects(new Rectangle(0 - (int)Ressources.vitesseDefautPlayer, 0, (int)Ressources.vitesseDefautPlayer, Ressources.mapLocale.hauteur)))
                        {
                            enCol = true;
                        }
                    }
                    break;
                case Direction.Right:

                    foreach (int[] cube in Ressources.mapLocale.CoordsCubes)
                    {
                        if (new Rectangle(this.Hitbox.X + this.HitboxPourCollision.X + this.HitboxPourCollision.Width + distancePixel, this.Hitbox.Y + this.HitboxPourCollision.Y, 0, this.HitboxPourCollision.Height).Intersects(new Rectangle(cube[0] * tCube, cube[1] * tCube, tCube, tCube)))
                        {
                            //STEPSIZE
                            if (this.Hitbox.Y + this.HitboxPourCollision.Y + this.HitboxPourCollision.Height - ((cube[1] * tCube) + tCube) < Ressources.stepSize)
                            {
                                bool stop = false;
                                int h = 1;
                                while (!stop && h <= this.HitboxPourCollision.Height)
                                {
                                    if (Ressources.mapLocale.returnSiCube((cube[0] * tCube), (cube[1] * tCube) - h))
                                    {
                                        enCol = true;
                                        stop = true;
                                    }
                                    h++;
                                }
                                if (!stop)
                                {
                                    enCol = false;
                                    this.Hitbox.Y -= Ressources.stepSize;
                                }
                            }
                            else
                            {
                                enCol = true;
                            }
                        }
                    }

                    if(!enCol)
                    {
                        if (new Rectangle(this.Hitbox.X + this.HitboxPourCollision.X + this.HitboxPourCollision.Width + distancePixel, this.Hitbox.Y + this.HitboxPourCollision.Y, 0, this.HitboxPourCollision.Height).Intersects(new Rectangle(Ressources.mapLocale.largeur, 0, (int)Ressources.vitesseDefautPlayer, Ressources.mapLocale.hauteur)))
                        {
                            enCol = true;
                        }
                    }
                    break;
            }
            return enCol;
        }

        // UPDATE
        public void Update(MouseState Mouse, KeyboardState keyboard, GameTime gametime)
        {
            if (this.joueurLocal)
            {
                this.Hitbox.X = Ressources.hitboxPlayerLocal[0];
                this.Hitbox.Y = Ressources.hitboxPlayerLocal[1];

                this.firstGametimePourAnimation = gametime.TotalGameTime.TotalMilliseconds;
                this.firstGametimePourDeplacementX = gametime.TotalGameTime.TotalMilliseconds;
                this.firstGametimePourDeplacementY = gametime.TotalGameTime.TotalMilliseconds;

                //Si il est dans le vide
                if (!seraEnCollision(Direction.Down, 1, false) && !entrainDeSauter)
                {
                    this.entrainDeTomber = true;
                    tomber(gametime.TotalGameTime.TotalMilliseconds);
                }
                else
                {
                    this.entrainDeTomber = false;
                    this.valeurChuteActuellement = 0;
                }

                // touches
                if (keyboard.IsKeyDown(Keys.H))
                {
                    if (!ancienKeys.Contains(Keys.H))
                    {
                        if (Ressources.afficherHitboxCollision)
                        {
                            Ressources.afficherHitboxCollision = false;
                        }
                        else
                        {
                            Ressources.afficherHitboxCollision = true;
                        }
                    }
                }

                if (keyboard.IsKeyDown(Keys.A))
                {
                    if (!ancienKeys.Contains(Keys.A))
                    {
                        this.armeActuelle++;
                        if (this.armeActuelle >= this.armes.Length)
                        {
                            this.armeActuelle = 0;
                        }
                    }
                }

                if (keyboard.IsKeyDown(Keys.Up) || keyboard.IsKeyDown(Keys.E))
                {
                    if (!entrainDeTomber)
                    {
                        this.entrainDeSauter = true;
                    }
                    //Ressources.DirectionPlayerLocal = Direction.Up;
                }

                if (keyboard.IsKeyDown(Keys.Down) || keyboard.IsKeyDown(Keys.D))
                {
                    //Ressources.DirectionPlayerLocal = Direction.Down;
                    accroupir();
                }
                if (keyboard.IsKeyDown(Keys.Left) || keyboard.IsKeyDown(Keys.S))
                {
                    if (Ressources.DirectionPlayerLocal == Direction.Right)
                    {
                        this.frameTextureBas = 0;
                    }
                    Ressources.DirectionPlayerLocal = Direction.Left;
                    this.deplacementGaucheDroite(true, gametime.TotalGameTime.TotalMilliseconds, Mouse);
                }
                if (keyboard.IsKeyDown(Keys.Right) || keyboard.IsKeyDown(Keys.F))
                {
                    if (Ressources.DirectionPlayerLocal == Direction.Left)
                    {
                        this.frameTextureBas = 0;
                    }
                    Ressources.DirectionPlayerLocal = Direction.Right;
                    this.deplacementGaucheDroite(false, gametime.TotalGameTime.TotalMilliseconds, Mouse);
                }

                if ((!keyboard.IsKeyDown(Keys.Left)) && (!keyboard.IsKeyDown(Keys.Right)) && (!keyboard.IsKeyDown(Keys.S)) && (!keyboard.IsKeyDown(Keys.F)))
                {
                    this.dernierGametimePourDeplacementX = 0;
                    if (Ressources.DirectionPlayerLocal == Direction.Left)
                    {
                        this.numeroTextureBas = this.numeroTextureBasAfkGauche;
                    }
                    else
                    {
                        this.numeroTextureBas = this.numeroTextureBasAfkDroite;
                    }
                    this.frameTextureBas = 0;
                }

                if (entrainDeSauter)
                {
                    sauter(gametime.TotalGameTime.TotalMilliseconds);
                }

                if ((!entrainDeSauter) && (!entrainDeTomber))
                {
                    this.dernierGametimePourDeplacementY = 0;
                }

                //SOURIS
                if (Mouse.LeftButton == ButtonState.Pressed && Ressources.WindowActiveElement)
                {
                    Point p1 = new Point((this.Hitbox.X + (this.Hitbox.Width / 2)),(this.Hitbox.Y + (this.Hitbox.Height/2)));
                    Point p2 = new Point(Mouse.X + Ressources.camera.focus.X, Mouse.Y + Ressources.camera.focus.Y);
                    Point p3 = new Point(Ressources.mapLocale.largeur + 1,(this.Hitbox.Y + (this.Hitbox.Height/2)));
                    if (Mouse.Y + Ressources.camera.focus.Y >= (this.Hitbox.Y + (this.Hitbox.Height / 2))) //bas
                    {
                        this.rotationHaut = Ressources.calculerAngle(p1, p2, p3);
                        this.angle = 360 - MathHelper.ToDegrees(Ressources.calculerAngle(p1, p2, p3));
                    }
                    else
                    {
                        this.angle = MathHelper.ToDegrees(Ressources.calculerAngle(p1, p2, p3));
                        this.rotationHaut = 0 - Ressources.calculerAngle(p1, p2, p3);
                    }
                    if (Mouse.X + Ressources.camera.focus.X >= (this.Hitbox.X + (this.Hitbox.Width / 2)))
                    {
                        this.numeroTextureHaut = this.numeroTextureHautAfkDroite;
                    }
                    else
                    {
                        this.numeroTextureHaut = this.numeroTextureHautAfkGauche;
                    }


                    //cube
                    //if ((Mouse.X + Ressources.camera.focus.X > 0) && (Mouse.X + Ressources.camera.focus.X < Ressources.mapLocale.largeur) && (Mouse.Y + Ressources.camera.focus.Y > 0) && (Mouse.Y + Ressources.camera.focus.Y < Ressources.mapLocale.hauteur))
                    //{
                    //    if (Ressources.mapLocale.returnSiCube(Mouse.X + Ressources.camera.focus.X, Mouse.Y + Ressources.camera.focus.Y))
                    //    {
                    //        BoundingSphere sphere = new BoundingSphere(new Vector3(this.Hitbox.X + (this.Hitbox.Width / 2), this.Hitbox.Y + (this.Hitbox.Height / 2), 0), (float)(this.armes[this.armeActuelle].portee / 2));
                    //        bool test = sphere.Intersects(new BoundingBox(new Vector3(Mouse.X + Ressources.camera.focus.X, Mouse.Y + Ressources.camera.focus.Y, 0), new Vector3(Mouse.X + Ressources.camera.focus.X, Mouse.Y + Ressources.camera.focus.Y, 0)));
                    //        if (test || this.armes[this.armeActuelle].porteeInfinie)
                    //        {
                    //            Ressources.mapLocale.retirerCube(Mouse.X + Ressources.camera.focus.X, Mouse.Y + Ressources.camera.focus.Y);
                    //        }
                    //    }
                    //}
                    //if (ancienMouse.LeftButton != ButtonState.Pressed)
                    //{
                        if ((Mouse.X + Ressources.camera.focus.X > 0) && (Mouse.X + Ressources.camera.focus.X < Ressources.mapLocale.largeur) && (Mouse.Y + Ressources.camera.focus.Y > 0) && (Mouse.Y + Ressources.camera.focus.Y < Ressources.mapLocale.hauteur))
                        {
                            int[] mousetir = new int[2];
                            mousetir[0] = Mouse.X + Ressources.camera.focus.X;
                            mousetir[1] = Mouse.Y + Ressources.camera.focus.Y;
                            Ressources.tirs.Add(new Tir(2000, this.nom, 0, this.Hitbox.X + (this.Hitbox.Width / 2), this.Hitbox.Y + (this.Hitbox.Height / 2), this.angle, this.armes[this.armeActuelle].portee, mousetir));
                        }
                    //}
                }
                else
                {
                    if (ancienMouse.LeftButton == ButtonState.Pressed && Ressources.WindowActiveElement)
                    {
                        if (Mouse.X + Ressources.camera.focus.X >= (this.Hitbox.X + (this.Hitbox.Width / 2)))
                        {
                            this.rotationHaut = 0f;
                        }
                        else
                        {
                            this.rotationHaut = (float)(Math.PI);
                        }
                    }
                }

                if (Mouse.RightButton == ButtonState.Pressed && Ressources.WindowActiveElement)
                {
                    //cube
                    if ((Mouse.X + Ressources.camera.focus.X > 0) && (Mouse.X + Ressources.camera.focus.X < Ressources.mapLocale.largeur) && (Mouse.Y + Ressources.camera.focus.Y > 0) && (Mouse.Y + Ressources.camera.focus.Y < Ressources.mapLocale.hauteur))
                    {
                        if (!Ressources.mapLocale.returnSiCube(Mouse.X + Ressources.camera.focus.X, Mouse.Y + Ressources.camera.focus.Y))
                        {
                            BoundingSphere sphere = new BoundingSphere(new Vector3(this.Hitbox.X + (this.Hitbox.Width / 2), this.Hitbox.Y + (this.Hitbox.Height / 2), 0), (float)(Ressources.porteePosageCube / 2));
                            bool test = sphere.Intersects(new BoundingBox(new Vector3(Mouse.X + Ressources.camera.focus.X, Mouse.Y + Ressources.camera.focus.Y, 0), new Vector3(Mouse.X + Ressources.camera.focus.X, Mouse.Y + Ressources.camera.focus.Y, 0)));
                            if (test)
                            {
                                Ressources.mapLocale.ajouterCube(Mouse.X + Ressources.camera.focus.X, Mouse.Y + Ressources.camera.focus.Y, this.numeroEquipe, false);
                            }
                        }
                    }
                }

                if (this.dernierGametimePourAnimationHaut == 0)
                {
                    this.dernierGametimePourAnimationHaut = this.firstGametimePourAnimation;
                }

                double differenceTemps = gametime.TotalGameTime.TotalMilliseconds - this.dernierGametimePourAnimationHaut;
                bool animer = false;
                if (differenceTemps >= Ressources.tempsEntreFramePlayerHaut)
                {
                    animer = true;
                    this.dernierGametimePourAnimationHaut = gametime.TotalGameTime.TotalMilliseconds;
                }
                if (animer)
                {
                    this.frameTextureHaut++;
                    if (this.frameTextureHaut >= (Ressources.Textures[this.numeroTextureHaut].Height / this.Hitbox.Height))
                    {
                        this.frameTextureHaut = 0;
                    }
                }

                ancienKeys = keyboard.GetPressedKeys();
                ancienMouse = Mouse;
                Ressources.hitboxPlayerLocal[0] = this.Hitbox.X;
                Ressources.hitboxPlayerLocal[1] = this.Hitbox.Y;

                Ressources.texteAffichageVariable += "\nArmes : {";
                foreach (Arme arme in this.armes)
                {
                    if (arme == null)
                    {
                        Ressources.texteAffichageVariable += "null ; ";
                    }
                    else
                    {
                        if (this.armes[this.armeActuelle].nom == arme.nom)
                        {
                            Ressources.texteAffichageVariable += "[" + arme.nom + "] ; ";
                        }
                        else
                        {
                            Ressources.texteAffichageVariable += arme.nom + " ; ";
                        }
                    }
                }
                Ressources.texteAffichageVariable += "}\nMouse : [" + (Mouse.X + Ressources.camera.focus.X).ToString() + ";" + (Mouse.Y + Ressources.camera.focus.Y).ToString() + "]\nNombre tirs : " + Ressources.tirs.Count().ToString();
                Ressources.texteAffichageVariable += "\n\nStats packets : \nEnvoyé : " + Ressources.nombrePacketEnvoyer.ToString() + "\nRecu : " + Ressources.nombrePacketRecu.ToString() + "\nTotal : " + (Ressources.nombrePacketRecu + Ressources.nombrePacketEnvoyer).ToString();
                Ressources.envoyerPosition();
            }
        }

        //DRAW
        public void Draw(SpriteBatch spriteBatch, bool affichertitre)
        {
            Rectangle milieuHitbox;
            if (this.joueurLocal)
            {
                milieuHitbox = new Rectangle((Ressources.lesEtats[Ressources.EtatActif].TailleFenetre.Width / 2) - this.Hitbox.Width / 2, (Ressources.lesEtats[Ressources.EtatActif].TailleFenetre.Height / 2) - this.Hitbox.Height / 2, this.Hitbox.Width, this.Hitbox.Height);
            }
            else
            {
                milieuHitbox = new Rectangle(this.Hitbox.X - Ressources.camera.focus.X, this.Hitbox.Y - Ressources.camera.focus.Y, this.Hitbox.Width, this.Hitbox.Height);
            }
            if (affichertitre)
            {
                this.titre.Draw(spriteBatch, milieuHitbox);
            }
            else
            {
                if (Ressources.afficherHitboxCollision)
                {
                    spriteBatch.Draw(Ressources.Textures[0], new Rectangle(milieuHitbox.X + this.HitboxPourCollision.X, milieuHitbox.Y + this.HitboxPourCollision.Y, this.HitboxPourCollision.Width, this.HitboxPourCollision.Height), null, Color.White);
                    Rectangle rectanglePortee = new Rectangle(milieuHitbox.X - (int)(Ressources.porteePosageCube / 2) + (milieuHitbox.Width / 2), milieuHitbox.Y - (int)(Ressources.porteePosageCube / 2) + (milieuHitbox.Height / 2), Ressources.porteePosageCube, Ressources.porteePosageCube);
                    spriteBatch.Draw(Ressources.CreateCircle(spriteBatch.GraphicsDevice, (int)(Ressources.porteePosageCube / 2)), rectanglePortee, Color.Red);
                    Rectangle rectanglePorteeArme = new Rectangle(milieuHitbox.X - (int)(this.armes[this.armeActuelle].portee / 2) + (milieuHitbox.Width / 2), milieuHitbox.Y - (int)(this.armes[this.armeActuelle].portee / 2) + (milieuHitbox.Height / 2), this.armes[this.armeActuelle].portee, this.armes[this.armeActuelle].portee);
                    spriteBatch.Draw(Ressources.CreateCircle(spriteBatch.GraphicsDevice, (int)(this.armes[this.armeActuelle].portee / 2)), rectanglePorteeArme, Color.GreenYellow);
                }
                
                spriteBatch.Draw(Ressources.Textures[this.numeroTextureBas], milieuHitbox, new Rectangle(0, this.frameTextureBas * this.Hitbox.Height, this.Hitbox.Width, this.Hitbox.Height), Color.White);
                //spriteBatch.Draw(Ressources.Textures[this.numeroTextureHaut], milieuHitbox, null, Color.White);
                spriteBatch.Draw(Ressources.Textures[this.numeroTextureHaut], new Rectangle(milieuHitbox.X + (milieuHitbox.Width / 2), milieuHitbox.Y + (milieuHitbox.Height / 2), milieuHitbox.Width, milieuHitbox.Height), new Rectangle(0, this.frameTextureHaut * this.Hitbox.Height, this.Hitbox.Width, this.Hitbox.Height), Color.White, this.rotationHaut, new Vector2(this.Hitbox.Width / 2, this.Hitbox.Height / 2), SpriteEffects.None, 0);
            }
        }
    }
}
