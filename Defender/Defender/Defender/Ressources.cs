using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Alta.Net;
using System.Net;
using System.Net.Sockets;

namespace Defender
{
    class Ressources
    {
        // STATIC FIELDS
        public static Etat[] lesEtats;
        public static int EtatActif;

        public static List<SpriteFont> Fonts;
        public static List<Texture2D> Textures;
        public static List<Texture2D> TexturesArmes;
        public static List<Texture2D> TexturesTirs;

        public static int tempsEntreFramePlayer;
        public static int tempsEntreFramePlayerHaut;

        public static int[] hitboxPlayerLocal;
        public static Direction DirectionPlayerLocal;
        public static bool afficherHitboxCollision;

        public static List<Player> Players;

        public static Equipe[] Equipes;

        public static Map mapLocale; //dans Gamemain
        public static Camera camera;

        public static bool WindowActiveElement;

        public static List<Arme> armes;
        public static List<Tir> tirs;

        public static double rationGraviteChute;
        public static double rationGraviteSaut;
        public static int tailleCube;
        public static int stepSize;
        public static int porteePosageCube;
        public static double vitesseDefautPlayer;
        public static double vitesseDefautMaxChute;
        public static double vitesseDefautMinChute;
        public static int hauteurSaut;
        public static double rationVitessePendantSaut;
        public static double propulsionSaut;

        public static string nomClient;
        public static string mdpClient;
        public static ClientHandler client;

        public static bool EnConnexion = false;
        public static string erreur;
        public static bool finInitialisation = false;
        public static double nombrePacketEnvoyer = 0;
        public static double nombrePacketRecu = 0;

        public static string texteAffichageVariable;

        // LOAD CONTENT
        public static void LoadContent(ContentManager content)
        {
            //AUTRES
            texteAffichageVariable = "";
            rationGraviteChute = 0.05;
            rationGraviteSaut = 1.2;
            tailleCube = 10;
            stepSize = 10;
            porteePosageCube = 275;
            vitesseDefautPlayer = 100; // 100px/s
            vitesseDefautMaxChute = 4; // x 4
            vitesseDefautMinChute = 0.5; // x 0.5
            propulsionSaut = 2;
            hauteurSaut = 55;
            rationVitessePendantSaut = 1.5;
            tempsEntreFramePlayer = 83; //0083ms
            tempsEntreFramePlayerHaut = 166; //0166ms
            afficherHitboxCollision = true;

            //FONTS
            Fonts = new List<SpriteFont>();
            Fonts.Add(content.Load<SpriteFont>("data/gametime")); //0
            Fonts.Add(content.Load<SpriteFont>("graphic/font")); //1

            //TEXTURES
            Textures = new List<Texture2D>();
            Textures.Add(content.Load<Texture2D>("graphic/player")); //0
            Textures.Add(content.Load<Texture2D>("graphic/noir1x1")); //1
            Textures.Add(content.Load<Texture2D>("graphic/blanc1x1")); //2
            Textures.Add(content.Load<Texture2D>("graphic/eriBasAfkGauche53x38")); //3
            Textures.Add(content.Load<Texture2D>("graphic/eriBasAfkDroite53x38")); //4
            Textures.Add(content.Load<Texture2D>("graphic/eriBasMarcheGauche53x38")); //5
            Textures.Add(content.Load<Texture2D>("graphic/eriBasMarcheDroite53x38")); //6
            Textures.Add(content.Load<Texture2D>("graphic/eriHautAfkGauche53x38")); //7
            Textures.Add(content.Load<Texture2D>("graphic/eriHautAfkDroite53x38")); //8

            TexturesArmes = new List<Texture2D>();
            Textures.Add(content.Load<Texture2D>("graphic/player")); //0

            TexturesTirs = new List<Texture2D>();
            TexturesTirs.Add(content.Load<Texture2D>("graphic/player")); //0


            //ETATS
            lesEtats = new Etat[2];
            lesEtats[0] = new Etat("game", "Defender - game", new System.Drawing.Size(1280, 720));
            lesEtats[1] = new Etat("login", "Defender - Login", new System.Drawing.Size(1280, 720));
            EtatActif = 1;

            //CAMERA
            camera = new Camera(0, 0, lesEtats[EtatActif].TailleFenetre.Width, lesEtats[EtatActif].TailleFenetre.Height);

            //ARMES
            armes = new List<Arme>(); //string nom, int typeArme, int portee, bool porteeInfinie, double dommage, int munitionEnStock, int tailleChargeurMunition, bool munitionInfinie, int numeroTexture
            armes.Add(new Arme("Couteau", 0, 100, 40, 0, 0, true, 0, 0));
            armes.Add(new Arme("Pistolet", 1, 800, 15, 0, 12, true, 1, 100));
            armes.Add(new Arme("Fusil", 2, 2000, 20, 120, 30, false, 2, 200));

            //TIRS
            tirs = new List<Tir>();

            //HITBOXPLAYER
            hitboxPlayerLocal = new int[2];

            //PLAYERS
            Players = new List<Player>();

            //EQUIPES
            Equipes = new Equipe[2];
            Equipes[0] = new Equipe(1, "rouge", Color.Firebrick);
            Equipes[1] = new Equipe(2, "vert", Color.GreenYellow);

            //CONNEXION !
            client = new ClientHandler();
            //client.SynchronizingObject = this; //Pour des WindowsForm !?
            client.AuthRequested += client_AuthRequested;
            client.Disconnected += client_Disconnected;
            client.ReceivedTcp += client_ReceivedTCP;
            client.ReceivedUdp += client_ReceivedUDP;
            nomClient = "megurine";
            mdpClient = "333666";
        }

        static public void Connexion()
        {
            EnConnexion = true;
            client.Connect(IPAddress.Parse("127.0.0.1"), 28223, ConnectResult);
        }

        static public void client_AuthRequested(object sender, EventArgs e) //METHODE DE CONNEXION
        {
            using (PacketWriter Pwriter = new PacketWriter())
            {
                Pwriter.Write(nomClient, mdpClient);
                client.SendTcp(Pwriter.Data);
            }
        }

        static public void client_Disconnected(object sender, NetEventArgs e) //METHODE A LA FERMETURE DE CONNEXION (ET DU JEU)
        {
            EtatActif = 1;
            finInitialisation = false;
            erreur = "Vous avez été deconnecté !";
        }

        static public void ConnectResult(object sender, Exception e) //TENTATIVE DE CONNEXION
        {
            if (e != null) //L'AUTHENTIFICATION EST MAUVAISE !
            {
                bool newErreur = true;
                if (e.Message.StartsWith("Une tentative de connexion a") || e.Message.StartsWith("Aucune connexion n'a pu être établie car l'ordinateur cible l'a expressément refusée") || e.Message.StartsWith("Aucune connexion n’a pu être établie car l’ordinateur cible l’a expressément refusée"))
                {
                    erreur = "Le serveur distant n'a pas répondu.\n\nPlusieurs raisons possibles :\n-Vous n'êtes pas connecté au réseau (ou internet).\n-Le serveur distant n'est pas disponible.\n-Le serveur distant refuse les connexions.\n\nCode erreur : " + ((SocketException)e).ErrorCode;
                    newErreur = false;
                }
                if (e.Message.StartsWith("Authentication"))
                {
                    erreur = "Identifiant ou mot de passe incorrect !\n\n-Votre tentative est enregistrée.";
                    newErreur = false;
                }
                if (e.Message.StartsWith("A similar client"))
                {
                    erreur = "Une autre instance du jeu est déjà lancée, celle ci va ce fermer !";
                    newErreur = false;
                }
                if (newErreur)
                    erreur = "Cette erreur n'est pas connus de Megu.fr,\nmerci de contacter l'équipe pour l'aider dans ce projet\nen copiant le code d'erreur, Merci !\n\nCode erreur : " + ((SocketException)e).ErrorCode;
            }
            else //L'AUTHENTIFICATION EST BONNE !
            {
                EtatActif = 0;
            }
            EnConnexion = false;
        }

        static public void client_ReceivedTCP(object sender, PacketEventArgs e) //Reception d'un paquet
        {
            nombrePacketRecu++;
            using (PacketReader reader = new PacketReader(e.Data))
            {
                string message = reader.ReadString();
                if (message[0] == 'F') //FIN du chargement, on peux déssiner !
                {
                    finInitialisation = true;
                }
                else if (message[0] == 'A') //AjoutCube
                {
                    message = message.Substring(1);
                    string[] messages = message.Split(';');
                    mapLocale.ajouterCube(int.Parse(messages[0]), int.Parse(messages[1]), int.Parse(messages[2]), true);
                }
                else if (message[0] == 'R') //RetirerCube
                {
                    message = message.Substring(1);
                    string[] messages = message.Split(';');
                    mapLocale.retirerCube(int.Parse(messages[0]), int.Parse(messages[1]), true);
                }
                else if (message[0] == 'N') //Nouvelle connexion joueur
                {
                    message = message.Substring(1);
                    string[] messages = message.Split(';');
                    connecterJoueur(int.Parse(messages[0]), messages[1]);
                }
                else if (message[0] == 'D') //Deconnexion joueur
                {
                    message = message.Substring(1);
                    string[] messages = message.Split(';');
                    deconnecterJoueur(messages[0]);
                }
                else
                {

                }
            }
        }

        static public void client_ReceivedUDP(object sender, NetDataEventArgs e) //Reception d'un paquet UDP
        {
            nombrePacketRecu++;
            using (PacketReader reader = new PacketReader(e.Data))
            {
                string message = reader.ReadString();
                if (message[0] == 'M') //Mouvement joueur
                {
                    message = message.Substring(1);
                    string[] messages = message.Split(';');
                    mouvementJoueur(messages[0], int.Parse(messages[1]), int.Parse(messages[2]), float.Parse(messages[3]), int.Parse(messages[4]), int.Parse(messages[5]), int.Parse(messages[6]), int.Parse(messages[7]));
                }
                else
                {

                }
            }
        }

        static public void mouvementJoueur(String pseudo, int newX, int newY, float newRotationHaut, int newFrameTextureHaut, int newFrameTextureBas, int newNumeroTextureHaut, int newNumeroTextureBas)
        {
            Player player = Players.Find(p => p.nom == pseudo);
            player.Hitbox.X = newX;
            player.Hitbox.Y = newY;
            player.rotationHaut = newRotationHaut;
            player.frameTextureHaut = newFrameTextureHaut;
            player.frameTextureBas = newFrameTextureBas;
            player.numeroTextureHaut = newNumeroTextureHaut;
            player.numeroTextureBas = newNumeroTextureBas;
        }

        static public void connecterJoueur(int numeroEquipe, string pseudo)
        {
            Arme[] armes = new Arme[3];
            armes[0] = Ressources.armes[0];
            armes[1] = Ressources.armes[1];
            armes[2] = Ressources.armes[2];

            Players.Add(new Player(false, numeroEquipe, 7, 8, 5, 6, 3, 4, pseudo, armes));
        }

        static public void deconnecterJoueur(string pseudo)
        {
            Player player = Players.Find(p => p.nom == pseudo);
            Players.Remove(player);
        }

        static public void ajouterCube(int X, int Y, int numeroEquipe)
        {
            nombrePacketEnvoyer++;
            client.SendTcp("A" + X.ToString() + ";" + Y.ToString() + ";" + numeroEquipe.ToString());
        }
        
        static public void retirerCube(int X, int Y)
        {
            nombrePacketEnvoyer++;
            client.SendTcp("R" + X.ToString() + ";" + Y.ToString());
        }

        static public void envoyerPosition()
        {
            Player player = Players.Find(p => p.joueurLocal == true);
            nombrePacketEnvoyer++;
            client.SendUdp("M" + player.nom + ";" + player.Hitbox.X.ToString() + ";" + player.Hitbox.Y.ToString() + ";" + player.rotationHaut.ToString() + ";" + player.frameTextureHaut.ToString() + ";" + player.frameTextureBas.ToString() + ";" + player.numeroTextureHaut.ToString() + ";" + player.numeroTextureBas.ToString());
        }

        public static float calculerAngle(Point p1, Point p2, Point p3)
        {
            if (p1 == p2)
            {
                return 0;
            }
            double p12 = Math.Sqrt(Math.Pow((p1.X - p2.X), 2) + Math.Pow((p1.Y - p2.Y), 2));
            double p13 = Math.Sqrt(Math.Pow((p1.X - p3.X), 2) + Math.Pow((p1.Y - p3.Y), 2));
            double p23 = Math.Sqrt(Math.Pow((p2.X - p3.X), 2) + Math.Pow((p2.Y - p3.Y), 2));
            double cos = ((p12 * p12) + (p13 * p13) - (p23 * p23)) / (2 * p12 * p13);
            double lol = Math.Acos(cos);
            return (float)lol;
        }

        public static Texture2D CreateCircle(GraphicsDevice importedGraphicsDevice, int radius)
        {
            int outerRadius = radius * 2 + 2;
            Texture2D texture = new Texture2D(importedGraphicsDevice, outerRadius, outerRadius);

            Color[] data = new Color[outerRadius * outerRadius];

            for (int i = 0; i < data.Length; i++)
                data[i] = Color.Transparent;

            double angleStep = 1f / radius;

            for (double angle = 0; angle < Math.PI * 2; angle += angleStep)
            {
                int x = (int)Math.Round(radius + radius * Math.Cos(angle));
                int y = (int)Math.Round(radius + radius * Math.Sin(angle));

                data[y * outerRadius + x + 1] = Color.White;
            }

            texture.SetData(data);
            return texture;
        }
    }
}