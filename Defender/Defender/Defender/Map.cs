using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Defender
{
    class Map
    {
        // FIELDS
        public string nom;

        public List<int[]> CoordsCubes;
        public Queue<int[]> CoordsCubesModifier;
        public bool[,] Cubes;

        public int hauteur;
        public int largeur;

        // CONSTRUCTOR
        public Map(string nom, int hauteur, int largeur)
        {
            this.nom = nom;

            this.CoordsCubes = new List<int[]>();
            this.CoordsCubesModifier = new Queue<int[]>();

            this.Cubes = new bool[largeur / Ressources.tailleCube, hauteur / Ressources.tailleCube];

            this.hauteur = hauteur;
            this.largeur = largeur;
        }

        //METHODS
        public bool existCube(int X, int Y)
        {
            int i = 0;
            bool trouver = false;
            while (i < CoordsCubes.Count() && !trouver)
            {
                if (CoordsCubes[i][0] == X && CoordsCubes[i][1] == Y)
                {
                    trouver = true;
                }
                i++;
            }
            return trouver;
        }

        public bool RemoveCube(int X, int Y)
        {
            int i = 0;
            bool trouver = false;
            while (i < CoordsCubes.Count() && !trouver)
            {
                if (CoordsCubes[i][0] == X && CoordsCubes[i][1] == Y && CoordsCubes[i][2] != 0)
                {
                    CoordsCubes.Remove(CoordsCubes[i]);
                }
                i++;
            }
            return trouver;
        }

        public bool returnSiCube(int X, int Y)
        {
            double posX = X / Ressources.tailleCube;
            double posY = Y / Ressources.tailleCube;
            if (posX >= this.largeur / Ressources.tailleCube)
            {
                posX = (this.largeur / Ressources.tailleCube) - 1;
            }
            if (posY >= this.hauteur / Ressources.tailleCube)
            {
                posY = (this.hauteur / Ressources.tailleCube) - 1;
            }

            if (posX < 0 || posY < 0) { return true; }

            if (existCube((int)Math.Round(posX, 0), (int)Math.Round(posY, 0)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ajouterCube(int X, int Y, int numeroEquipe, bool byServer)
        {
            double posX = X;
            double posY = Y;

            if (!byServer)
            {
                posX = X / Ressources.tailleCube;
                posY = Y / Ressources.tailleCube;
            }

            if (posX >= this.largeur / Ressources.tailleCube)
            {
                posX = (this.largeur / Ressources.tailleCube) - 1;
            }
            if (posY >= this.hauteur / Ressources.tailleCube)
            {
                posY = (this.hauteur / Ressources.tailleCube) - 1;
            }

            if (posX < 0 || posY < 0) {}
            else
            {
                if (!existCube((int)Math.Round(posX, 0), (int)Math.Round(posY, 0)))
                {
                    int lol = 0;
                    if (byServer) { lol = 1; }
                    this.CoordsCubesModifier.Enqueue(new int[] {1, (int)Math.Round(posX, 0), (int)Math.Round(posY, 0), numeroEquipe, lol});
                }
            }
            
        }

        public void retirerCube(int X, int Y, bool byServer)
        {
            double posX = X;
            double posY = Y;

            if (!byServer)
            {
                posX = X / Ressources.tailleCube;
                posY = Y / Ressources.tailleCube;
            }

            if (posX >= this.largeur / Ressources.tailleCube)
            {
                posX = (this.largeur / Ressources.tailleCube) - 1;
            }
            if (posY >= this.hauteur / Ressources.tailleCube)
            {
                posY = (this.hauteur / Ressources.tailleCube) - 1;
            }

            if (posX < 0 || posY < 0) { }
            else
            {
                if (existCube((int)Math.Round(posX, 0), (int)Math.Round(posY, 0)))
                {
                    int lol = 0;
                    if (byServer) { lol = 1; }
                    this.CoordsCubesModifier.Enqueue(new int[] {0, (int)Math.Round(posX, 0), (int)Math.Round(posY, 0), lol });
                }
            }
        }

        public void Update()
        {
            int i;
            int count;

            i = 0;
            count = CoordsCubesModifier.Count();
            
            while (i < count)
            {
                int[] cube = CoordsCubesModifier.Dequeue();

                if (cube[0] == 1) //Ajouter
                {
                    if (!existCube(cube[1], cube[2]))
                    {
                        if (cube[4] == 0)
                        {
                            Ressources.ajouterCube(cube[1], cube[2], cube[3]);
                        }
                        else
                        {
                            CoordsCubes.Add(new int[] { cube[1], cube[2], cube[3] });
                        }
                    }
                }
                else //Retirer
                {
                    if (existCube(cube[1], cube[2]))
                    {
                        if (cube[3] == 0)
                        {
                            Ressources.retirerCube(cube[1], cube[2]);
                        }
                        else
                        {
                            RemoveCube(cube[1], cube[2]);
                        }
                    }
                }
                i++;
            }

            //foreach (int[] cube in CoordsCubesAjouter)
            //{
            //    if (!existCube(cube[0], cube[1]))
            //    {
            //        CoordsCubes.Add(cube);
            //        if (cube[3] == 0)
            //        {
            //            Ressources.ajouterCube(cube[0], cube[1], cube[2]);
            //        }
            //    }
            //}

            //CoordsCubesAjouter.Clear();

            //foreach (int[] cube in CoordsCubesSupprimer)
            //{
            //    if (existCube(cube[0], cube[1]))
            //    {
            //        RemoveCube(cube[0], cube[1]);
            //        if (cube[2] == 0)
            //        {
            //            Ressources.retirerCube(cube[0], cube[1]);
            //        }
            //    }
            //}

            //CoordsCubesSupprimer.Clear();

            for (i = 0; i < (this.largeur / Ressources.tailleCube); i++)
            {
                for (int j = 0; j < (this.hauteur / Ressources.tailleCube); j++)
                {
                    this.Cubes[i, j] = false;
                }
            }

            foreach (int[] cube in CoordsCubes)
            {
                this.Cubes[cube[0], cube[1]] = true;
            }
        }

        //UPDATE & DRAW
        public void Draw(SpriteBatch spriteBatch)
        {
            //BORDURE
            spriteBatch.Draw(Ressources.Textures[2], new Rectangle(0 - Ressources.camera.focus.X, 0 - Ressources.camera.focus.Y, this.largeur, this.hauteur), new Rectangle(0, 0, 1, 1), Color.White);

            //CUBES
            int tCube = Ressources.tailleCube;

            foreach (int[] cube in this.CoordsCubes)
            {
                Color color;
                if (cube[2] == 0)
                {
                    color = Color.Black;
                }
                else
                {
                    color = Ressources.Equipes[cube[2]].color;
                }
                spriteBatch.Draw(Ressources.Textures[2], new Rectangle((cube[0] * tCube) - Ressources.camera.focus.X, (cube[1] * tCube) - Ressources.camera.focus.Y, tCube, tCube), new Rectangle(0, 0, 1, 1), color);
            }
        }
    }
}
