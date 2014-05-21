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

        private float rotationHaut;

        public float RotationHaut
        {
            get { return rotationHaut; }
            set { rotationHaut = value; }
        }

        private int frameTextureHaut;

        public int FrameTextureHaut
        {
            get { return frameTextureHaut; }
            set { frameTextureHaut = value; }
        }

        private int frameTextureBas;

        public int FrameTextureBas
        {
            get { return frameTextureBas; }
            set { frameTextureBas = value; }
        }

        private int numeroTextureHaut;

        public int NumeroTextureHaut
        {
            get { return numeroTextureHaut; }
            set { numeroTextureHaut = value; }
        }

        private int numeroTextureBas;

        public int NumeroTextureBas
        {
            get { return numeroTextureBas; }
            set { numeroTextureBas = value; }
        }

        public Player(string pseudo, int numeroEquipe)
        {
            this.pseudo = pseudo;
            this.numEquipe = numeroEquipe;
            this.tempconnec = 0;
            this.hitboxX = 100;
            this.hitboxY = 100;
            this.RotationHaut = 0.0f;
            this.frameTextureHaut = 0;
            this.frameTextureBas = 0;
            this.numeroTextureHaut = 0;
            this.numeroTextureBas = 0;
        }
    }
}
