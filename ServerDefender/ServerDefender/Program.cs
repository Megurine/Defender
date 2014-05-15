using Alta.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ServerDefender
{
    class Program
    {


        static void Main(string[] args)
        {
            Ressources.init();

            EcrireInfoDebut();

            Console.WriteLine("Appuyez sur ESPACE pour lancer le serveur.");
            ConsoleKeyInfo readkey = Console.ReadKey();
            while (readkey.KeyChar != ' ')
            {
                readkey = Console.ReadKey();
            }

            Lancement();

            bool quitter = false;
            while (!quitter)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey();
                    if (keyInfo.Key == ConsoleKey.Escape)
                    {
                        quitter = true;
                    }
                }
            }

            Fermeture();

            Console.WriteLine("\nAppuyez sur une touche pour fermet la fenetre.");
            Console.ReadKey();
        }

        static void EcrireInfoDebut()
        {
            Console.WriteLine("---------------------ServerDefender 0.1-----------------------");
            Console.WriteLine("\nIP interne/externe : " + Ressources.ipInterne + " / " + Ressources.ipExterne);
            Console.WriteLine("Port : " + Ressources.port);
            if(Ressources.etat)
            {
                Console.WriteLine("Etat serveur : En ligne depuis " + Ressources.dateLancement.ToString());
            }
            else
            {
                Console.WriteLine("Etat serveur : Hors-ligne");
            }
            Console.WriteLine("Clients : " + Ressources.server.Clients.Count().ToString());
            Console.WriteLine("\n--------------------------------------------------------------\n");
        }

        static void Lancement()
        {
            Ressources.load();

            Console.Clear();

            EcrireInfoDebut();

            Console.WriteLine("Lancement du serveur...");

            while (!Ressources.etat)
            {
                ChangerStatutServeur();
                Console.Write(".");
            }

            Console.Clear();

            EcrireInfoDebut();

            Application.Run(new Form1());
        }

        static void Fermeture()
        {
            Console.Clear();

            EcrireInfoDebut();

            Console.WriteLine("Fermeture du serveur...");

            while (!Ressources.etat)
            {
                ChangerStatutServeur();
                Console.Write(".");
            }

            Console.WriteLine("\nServeur hors-ligne !");
        }

        static void ChangerStatutServeur()
        {
            if (Ressources.etat)
            {
                Ressources.Stop();
            }
            else
            {
                Ressources.Start(Ressources.port, Ressources.clientMax, Ressources.connexionMax);
                Ressources.dateLancement = DateTime.Now;
            }
        }
    }
}
