using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP
{
    class Graph
    {
        //Zawiera długości krawędzi między węzłami
        int[,] matrix;
        //Ilość miast
        int size;
        double density;

        public Graph(int size, double density, int maxDistance)
        {
            this.size = size;
            Random rand = new Random();
            matrix = new int[size,size];
            if (density < 0)
            {
                density = 0;
            }
            else if (density > 1.0)
            {
                density = 1;
            }
            else
                this.density = density;
            for(int i=0; i< size;i++)
            {
                for(int j=0; j< size;j++)
                {                  
                    //Wyzeruj przekątną macierzy
                    if (i == j)
                    {
                        matrix[i, j] = 0;
                        continue;
                    }
                    double chance = rand.NextDouble();
                    //Dostosowanie ilości połączeń między miastami w zależności od zadanej gęstości                  
                    if (chance < density)
                    {
                        matrix[i, j] = rand.Next(1, maxDistance);
                        //Symetryczność macierzy
                        matrix[j, i] = matrix[i, j];
                    }
                    else if(chance >= density)
                    {
                        matrix[i, j] = -1;
                        matrix[j, i] = matrix[i, j];
                    }
                }
            }
        }
        //Wylicza dystans między wskazanymi miastami
        public int Distance(int begin, int destination)
        {
            if(begin >= size || destination >= size || begin < 0 || destination < 0)
            {
                Console.WriteLine("Niepoprawne lokacje");
                return -1;
            }
            return matrix[begin,destination];
        }
        //Wylicza dlugosc podanej sciezki
        public int GetPathLength(List<int> path)
        {
            int distance=0;
            for(int i=0; i+1<path.Count;i++)
            {
                distance += Distance(path[i], path[i + 1]);
            }
            return distance;
        }

        //Upper Bound - Algorytm Najbliższego Sąsiada (Suma najkrótszych krawędzi wychodzących z każdego wierzchołka
        public int Bound(int distanceWalked, List<int> visited)
        {
            //Zapamiętaj dystans, który został pokonany do tej pory
            int distance = distanceWalked;
            
            for (int i = 0; i < size; i++)
            {
                int shortest = -1;
                for (int j = 0; j < size; j++)
                {
                    //Pomiń dla połączeń miast z samymi sobą
                    if (i == j)
                        continue;
                    //Pomiń dla miast już odwiedzonych
                    if (!CheckVisited(j, visited))
                    {
                        int dist = Distance(i,j);
                        //Jeśli nie ma jeszcze ścieżki możliwie najkrótszej, lub rozważana krawędź jest krótsza - podstaw
                        if (shortest == -1 || (dist < shortest && dist > -1))
                        {
                            shortest = dist;
                        }
                    }
                }
                distance += shortest;
            }

            return distance;
        }
        //Sprawdza czy zadane miasto było już odwiedzone
        public bool CheckVisited(int toCheck, List<int> visited)
        {
            foreach(int node in visited)
            {
                if (node == toCheck)
                    return true;
            }
            return false;
        }
        public void ShowGraph()
        {
            Console.Write("   ");       
            for(int i=0;i< size;i++)
            {
                Console.Write(i + " ");
            }
            Console.WriteLine();
            Console.Write("   ");
            for (int i = 0; i < size; i++)
            {
                Console.Write("_ ");
            }
            Console.WriteLine();
            for (int i=0; i< size;i++)
            {
                Console.Write(i + "| ");
                for(int j=0;j < size; j++)
                {
                    Console.Write(matrix[i, j] + " ");
                }
                Console.WriteLine("\n");
            }
        }

        public int GetSize()
        {
            return size;
        }

    }
}
