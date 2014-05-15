using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerDefender
{
    class Player
    {
        private DateTime deconnec;

        public DateTime Deconnec
        {
            get { return deconnec; }
            set { deconnec = value; }
        }

        private DateTime connec;

        public DateTime Connec
        {
            get { return connec; }
            set { connec = value; }
        }

        private int tempconnec;

        public int Tempconnec
        {
            get { return tempconnec; }
            set { tempconnec = value; }
        }

        private string pseudo;

        public string Pseudo
        {
            get { return pseudo; }
            set { pseudo = value; }
        }

        private int numEquipe;

        public int NumEquipe
        {
            get { return numEquipe; }
            set { numEquipe = value; }
        }

        private int hitboxX;

        public int HitboxX
        {
            get { return hitboxX; }
            set { hitboxX = value; }
        }

        private int hitboxY;

        public int HitboxY
        {
            get { return hitboxY; }
            set { hitboxY = value; }
        }

        private double angle;

        public double Angle
        {
            get { return angle; }
            set { angle = value; }
        }

        public Player(string pseudo, int numeroEquipe)
        {
            this.pseudo = pseudo;
            this.numEquipe = numeroEquipe;
            this.tempconnec = 0;
            this.hitboxX = 100;
            this.hitboxY = 100;
            this.Angle = 0;
        }


    }
}
