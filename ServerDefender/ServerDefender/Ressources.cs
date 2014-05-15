using Alta.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace ServerDefender
{
    class Ressources
    {
        public static string[] inscrits;
        public static List<String> tempDeco = new List<String>();

        public static List<int[]> CoordsCubes = new List<int[]>();

        public static ServerHandler server = new ServerHandler();
        public static bool serverstatus = false;
        public static bool serverstatusreprendre = true;

        public static string ipInterne;
        public static string ipExterne;
        public static int port;
        public static int clientMax;
        public static int connexionMax;
        public static DateTime dateLancement;

        public static List<string> nomPseudoConnexion = new List<string>();
        public static bool etat = false;
        public static bool etatConnexion = false;

        public static int[] equilibrageEquipe;

        public static string pseudoKicked = "";

        public static void init()
        {
            Console.WriteLine("Initialisation données reseau ...");

            ipInterne = ClientData.InternalIP;
            ipExterne = ClientData.ExternalIP;
            port = 28223;
            clientMax = 50;
            connexionMax = 50;

            Console.Clear();
        }

        public static void load()
        {
            inscrits = new string[6];
            inscrits[0] = "darkex";
            inscrits[1] = "333666";
            inscrits[2] = "darkexia";
            inscrits[3] = "333666";
            inscrits[4] = "megurine";
            inscrits[5] = "333666";

            equilibrageEquipe = new int[2];

            server.NewConnection += server_NewConnection;
            server.LostConnection += server_LostConnection;
            server.DuplicateFound += server_DuplicateFound;
            server.PreventDuplicate = Dupe.ExceptLocal;
            server.ReceivedTcp += server_ReceivedTcp;
            server.ReceivedUdp += server_ReceivedUDP;
            server.AuthValidation = ValidateUser;
            server.Started += server_Started;
            server.Stopped += server_Stoped;
        }

        public static void Start(int port, int nbClientMax, int nbClientConnexion)
        {
            try
            {
                server.Start(IPAddress.Any, port, nbClientMax, nbClientConnexion);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void Stop()
        {
            server.Stop(true); //Declenche l'event Lost_connexion chez le client !
        }

        public static void server_Started(object sender, EventArgs e)
        {
            etat = true;
            etatConnexion = true;
        }

        public static void server_Stoped(object sender, EventArgs e)
        {
            etat = false;
            etatConnexion = false;
        }

        static bool ValidateUser(object sender, PacketEventArgs e)
        {
            string username = string.Empty;
            string password = string.Empty;

            try
            {
                using (PacketReader reader = new PacketReader(e.Data))
                {
                    username = reader.ReadString();
                    password = reader.ReadString();
                }
            }
            catch
            {
                return false;
            }

            username = username.ToLower();

            if (tempDeco.Find(item => item == username) == username)
            {
                return false;
            }
            bool trouvé = false;

            for (int i = 0; (i < inscrits.Length - 1) && (trouvé == false); i = i + 2)
            {
                if ((inscrits[i] == username) && (inscrits[i + 1] == password))
                {
                    trouvé = true;
                }
            }

            char[] letters = username.ToCharArray();

            letters[0] = char.ToUpper(letters[0]);

            username = new string(letters);

            if (trouvé == true)
            {
                int numCli = ExisteClient(username);
                if (numCli != -1) //si quelqun est déja co sur le meme compte
                {
                    server.KickClient(server.Clients[numCli]);
                    pseudoKicked = username;
                }
                nomPseudoConnexion.Add(username);
                return true;
            }
            else
            {
                int numEquipe = RetourneNumeroEquipe();
                e.Client.ClientState = new Player(username, numEquipe);
                equilibrageEquipe[numEquipe]++;
                return false;
            }
        }

        static public int ExisteClient(string pseudo)
        {
            for (int i = 0; i < server.Clients.Count(); i = i + 1)
            {
                if (((Player)server.Clients[i].ClientState).Pseudo == pseudo)
                {
                    return i;
                }
            }
            return -1;
        }

        static public void kick(string pseudo)
        {
            int numCli = ExisteClient(pseudo);
            if (numCli != -1)
            {
                pseudoKicked = pseudo;
                server.KickClient(server.Clients[numCli]);
            }
        }

        static void server_NewConnection(object sender, NetEventArgs e)
        {
            string pseudo = nomPseudoConnexion[0]; //venant de Authentification
            nomPseudoConnexion.RemoveAt(0);

            int numEquipe = RetourneNumeroEquipe();
            e.Client.ClientState = new Player(pseudo, numEquipe);
            equilibrageEquipe[numEquipe]++;

            Console.WriteLine("→ NCO " + pseudo + " (" + e.Client.IP.ToString() + ")");

            foreach (ClientData client in server.Clients)
            {
                Player player = ((Player)client.ClientState); //envoi au nouveau
                if (player.Pseudo != pseudo)
                {
                    server.SendTcp(e.Client, "N" + player.NumEquipe.ToString() + ";" + player.Pseudo);
                    //server.SendTcp(e.Client, "POS#" + player.Pseudo + "#" + player.HitboxX + "#" + player.HitboxY + "#" + player.Direction + "#" + player.NbFrameColumn + "#" + player.NbFrameLine + "#" + player.NumMap);
                }
            }

            Player newplayer = ((Player)e.Client.ClientState);

            foreach (ClientData client in server.Clients)
            {
                Player player = ((Player)client.ClientState); //envoi aux autres co avant
                if (player.Pseudo != pseudo)
                {
                    server.SendTcp(client, "N" + newplayer.NumEquipe.ToString() + ";" + newplayer.Pseudo);
                    //server.SendTcp(client, "INF#" + pseudo + " vient de se connecter.");
                }
            }
            server.SendTcp(e.Client, "F");
        }

        static void server_LostConnection(object sender, NetEventArgs e)
        {
            Player player = (Player)e.Client.ClientState;

            if (pseudoKicked == player.Pseudo)
            {
                Console.WriteLine("← DCO " + player.Pseudo + " (kicked)");
                pseudoKicked = "";
            }
            else
            {
                Console.WriteLine("← DCO " + player.Pseudo + " (leave)");
            }

            foreach (ClientData client in server.Clients)
            {
                Player playera = ((Player)client.ClientState); //envoi aux autres co avant
                server.SendTcp(client, "D" + player.Pseudo);
                //server.SendTcp(client, "INF#" + pseudo + " vient de se connecter.");
            }

            tempDeco.Add(player.Pseudo.ToLower());
            //((Player)e.Client.ClientState).SauvegardeDeconnexion();
            tempDeco.Remove(player.Pseudo.ToLower());
        }

        static void server_DuplicateFound(object sender, NetEventArgs e)
        {
            //ThreadAfficherValue = TempNomCompte + " - " + e.Client.IP + " - HID : [" + e.Client.HID + "]";
            //ThreadAfficherAction = 4;
        }

        public static bool RemoveCube(int X, int Y)
        {
            int i = 0;
            bool trouver = false;
            while (i < CoordsCubes.Count() && !trouver)
            {
                if (CoordsCubes[i][0] == X && CoordsCubes[i][1] == Y)
                {
                    CoordsCubes.Remove(CoordsCubes[i]);
                }
                i++;
            }
            return trouver;
        }

        static void server_ReceivedTcp(object sender, PacketEventArgs e)
        {
            string pseudo = ((Player)e.Client.ClientState).Pseudo;
            using (PacketReader reader = new PacketReader(e.Data))
            {
                string message = reader.ReadString();
                if (message[0] == 'A') // Ajouter cube
                {
                    message = message.Substring(1);
                    string[] messages = message.Split(';');
                    int[] cube = new int[3];
                    cube[0] = int.Parse(messages[0]);
                    cube[1] = int.Parse(messages[1]);
                    cube[2] = int.Parse(messages[2]);
                    CoordsCubes.Add(cube);
                    foreach (ClientData client in server.Clients)
                    {
                        server.SendTcp(client, "A" + message);
                    }
                }
                else if(message[0] == 'R') // Retirer cube
                {
                    message = message.Substring(1);
                    string[] messages = message.Split(';');
                    RemoveCube(int.Parse(messages[0]), int.Parse(messages[1]));
                    foreach (ClientData client in server.Clients)
                    {
                        server.SendTcp(client, "R" + message);
                    }
                }
                else
                {

                }
            }
        }

        static void server_ReceivedUDP(object sender, NetDataEventArgs e)
        {
            string pseudo = ((Player)e.Client.ClientState).Pseudo;
            using (PacketReader reader = new PacketReader(e.Data))
            {
                string message = reader.ReadString();
                if (message[0] == 'M') //Mouvement Joueur
                {
                    message = message.Substring(1);
                    string[] messages = message.Split(';');
                    ((Player)e.Client.ClientState).HitboxX = int.Parse(messages[1]);
                    ((Player)e.Client.ClientState).HitboxY = int.Parse(messages[2]);
                    ((Player)e.Client.ClientState).Angle = double.Parse(messages[3]);

                    foreach (ClientData client in server.Clients)
                    {
                        Player player = ((Player)client.ClientState); //envoi aux autres co avant
                        if (player.Pseudo != pseudo)
                        {
                            server.SendUdp(client, "M" + pseudo + ";" + ((Player)e.Client.ClientState).HitboxX.ToString() + ";" + ((Player)e.Client.ClientState).HitboxY.ToString() + ";" + ((Player)e.Client.ClientState).Angle.ToString());
                        }
                    }

                }
                else
                {

                }
            }
        }

        static public int RetourneNumeroClient(string nom)
        {
            int i = 0;
            bool test = false;
            while (!test && i < server.Clients.Count)
            {
                if (((Player)server.Clients[i].ClientState).Pseudo == nom)
                {
                    test = true;
                }
                i++;
            }
            if (!test)
            {
                return -1;
            }
            return i - 1;
        }

        static public int RetourneNumeroEquipe()
        {
            int t0 = equilibrageEquipe[0];
            int t1 = equilibrageEquipe[1];

            if (t0 > t1)
            {
                return 1;
            }
            else if (t0 == t1)
            {
                return 0;
            }
            else
            {
                return 0;
            }
        }

    }
}
