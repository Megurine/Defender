using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Defender
{
    class Arme
    {
        public string nom;
        public int typeArme; //0 cac , 1 pistolet , 2 lourde

        public int portee;

        public double dommage;

        public int munitionActuelle;
        public int munitionEnStock;
        public int tailleChargeurMunition;
        public bool munitionInfinie;

        public int numeroTexture;
        
        public double rapiditeBalle;

        public Arme(string nom, int typeArme, int portee, double dommage, int munitionEnStock, int tailleChargeurMunition, bool munitionInfinie, int numeroTexture, double rapiditeBalle)
        {
            this.nom = nom;
            this.typeArme = typeArme;
            this.portee = portee;
            this.dommage = dommage;
            this.tailleChargeurMunition = tailleChargeurMunition;
            if (munitionEnStock < tailleChargeurMunition)
            {
                this.munitionEnStock = 0;
                this.munitionActuelle = munitionEnStock;
            }
            else
            {
                this.munitionEnStock = munitionEnStock - tailleChargeurMunition;
                this.munitionActuelle = tailleChargeurMunition;
            }
            this.munitionInfinie = munitionInfinie;
            this.numeroTexture = numeroTexture;
            this.rapiditeBalle = rapiditeBalle;
        }
    }
}
