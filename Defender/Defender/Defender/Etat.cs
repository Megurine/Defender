using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Defender
{
    class Etat
    {
        private string nom;
        public string Nom
        {
            get { return nom; }
            set { nom = value; }
        }

        private string titreFenetre;
        public string TitreFenetre
        {
            get { return titreFenetre; }
            set { titreFenetre = value; }
        }

        private Size tailleFenetre;
        public Size TailleFenetre
        {
            get { return tailleFenetre; }
            set { tailleFenetre = value; }
        }

        public Etat(string nom, string titreFenetre, Size tailleFenetre)
        {
            this.nom = nom;
            this.titreFenetre = titreFenetre;
            this.tailleFenetre = tailleFenetre;
        }
    }
}
