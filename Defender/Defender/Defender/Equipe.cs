using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Defender
{
    class Equipe
    {
        public int numero;

        public string nom;

        public Color color;

        public Equipe(int numero, string nom, Color color)
        {
            this.numero = numero;
            this.nom = nom;
            this.color = color;
        }
    }
}
