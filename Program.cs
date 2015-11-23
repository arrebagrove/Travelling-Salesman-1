using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace TSP
{
    class Program
    {
        static void Main(string[] args)
        {
            int size = 10;
            int maxDistance = 10;
            int choice = 0;
            /*do
            {
                Console.WriteLine("-----------------------------------------------------------------");
                Console.WriteLine("Ilosc miast: " + size);
                Console.WriteLine("Maksymalny dystans: " + maxDistance);
                Console.WriteLine("1. Podaj nowe dane");
                Console.WriteLine("2. Wygeneruj nowy problem");
                Console.WriteLine("3. Wyjdz");
                choice = Int32.Parse(Console.ReadLine());
                switch (choice)
                {
                    case 1:
                        int s, m;
                        Console.WriteLine("Podaj ilosc miast");
                        s = Int32.Parse(Console.ReadLine());
                        Console.WriteLine("Podaj maksymalny dystans miedzy miastami");
                        m = Int32.Parse(Console.ReadLine());
                        size = s;
                        maxDistance = m;
                        break;
                    case 2:
                        if (size > 0 && maxDistance > 0)
                        {
                            Showcase(size, maxDistance);
                        }
                        else
                        {
                            Console.WriteLine("Ilosc miast i dystans miedzy nimi musza byc wieksze od 0");
                        }
                        break;

                    case 3:
                        return;
                    default:
                        Console.WriteLine("Niepoprawna opcja");
                        break;
                }
            } while (true);*/
            ShowcaseDynamic(size, maxDistance);
        }
        //Prezentacja dzialania algorytmu Branch & Bound
        static void Showcase(int size, int maxDistance)
        {
            Graph graph = new Graph(size, 1, maxDistance);
            graph.ShowGraph();

            TS ts = new TS(graph);
            List<int> optimal = ts.BB();
            Console.Write("Optymalna sciezka: ");
            foreach (int i in optimal)
            {
                Console.Write(i + " ");
            }
            Console.WriteLine("\nDlugosc optymalnej sciezki: " + graph.GetPathLength(optimal));
            Console.ReadKey();
        }
        //Prezentacja działania algorytmu dynamicznego
        static void ShowcaseDynamic(int size, int maxDistance)
        {
            Graph graph = new Graph(size, 1, maxDistance);
            graph.ShowGraph();

            TS ts = new TS(graph);
            List<int> optimal = ts.Dynamic();
            Console.Write("Optymalna sciezka: ");
            foreach (int i in optimal)
            {
                Console.Write(i + " ");
            }
            Console.WriteLine("\nDlugosc optymalnej sciezki: " + graph.GetPathLength(optimal));
            Console.ReadKey();
        }
        //Badania
        static void Diagnostic(int nodes, int instances)
        {
            Console.WriteLine("Badania dla miast: {0}", nodes);
            Stopwatch sw = new Stopwatch();
            List<double> times = new List<double>();
            Console.Write("[");
            for (int i=0; i< instances; i++)
            {
                Graph graph = new Graph(nodes, 1, 1000);
                TS ts = new TS(graph);
                sw.Restart();
                ts.BB();
                sw.Stop();
                times.Add(sw.Elapsed.TotalSeconds);
                if (i % (instances / 10) == 0)
                {
                    Console.Write("*");
                }
            }
            Console.WriteLine("]");
            using (StreamWriter file = new StreamWriter(@"C:\Users\Patryk\Desktop\TSP.txt",true))
            {
                file.WriteLine("Liczba miast: {0}, liczba instancji: {1}", nodes, instances);
                foreach (double time in times)
                {
                    file.WriteLine(time);

                }
            }
            Console.WriteLine("Zakonczono badania dla miast: {0}", nodes);  
        }
    }
}
