using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerDefender
{
    class Cube
    {
        public int Etat;
        public int Team;

        public Cube()
        {
            Etat = 0;
            Team = 0;
        }

        public Cube(int etat, int team)
        {
            Etat = etat;
            Team = team;
        }
    }
}
