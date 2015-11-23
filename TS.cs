using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP
{
    class TS
    {
        Graph graph;
        List<int> path;
        /*Do algorytmu dynamicznego - słownik przechowujący zestawy argumentów i odpowiadające im wartości
        Argumenty - miasto początkowe, lista miast do przejścia
        Wartości zwracane - następne miasto, odległość do tego miasta*/
        Dictionary<Tuple<int,List<int>>, Tuple<int,int>> memory;

        public TS(Graph graph)
        {
            this.graph = graph;
            path = new List<int>();
            memory = new Dictionary<Tuple<int,List<int>>, Tuple<int,int>>(new ListComparer());
        }
        //Przesunięcie do następnego miasta
        public int AddNode(int destination, List<int> currentPath)
        {
            int upperBound;
            int distance = 0;
            //Jeśli wcześniej odwiedzono inne miasta to dodaj przebyty dystans
            if (currentPath.Count > 0)
            {
                distance = graph.GetPathLength(currentPath) + graph.Distance(currentPath[currentPath.Count - 1],destination);
            }
            //Dodanie celu do obecnej ścieżki
            currentPath.Add(destination);
            //Wyliczenie górnej granicy dla nowej ścieżki
            upperBound = graph.Bound(distance, currentPath);
            return upperBound;
        }
        //Rozwiązanie problemu komiwojażera metodą Branch & Bound
        public List<int> BB()
        {
            int visitedNodes = 0;
            List<int> optimalPath = null;
            //Deklaracja kolejki priorytetowej
            PriorityQueue<int, List<int>> queue = new PriorityQueue<int, List<int>>();
            //Dodanie ścieżki składającej się z węzła początkowego do kolejki
            queue.Enqueue(AddNode(0, path), path);

            while(!queue.IsEmpty)
            {
                List<int> currentPath = queue.DequeueValue();
                //Sprawdzamy jesli nie ma jeszcze wyliczonej ścieżki, lub jest ona gorsza niż upper bound dla ścieżki obecnie rozważanej
                if (optimalPath == null || graph.Bound(graph.GetPathLength(currentPath),currentPath) < graph.GetPathLength(optimalPath))
                {
                    visitedNodes++;
                    //Sprawdzamy czy obecna ścieżka może być uznana za rozwiązanie (Wszystkie miasta odwiedzone)
                    if(currentPath.Count() == graph.GetSize())
                    {
                        //Sprawdzamy czy z ostatniego punktu na naszej ścieżce można wrócić do miasta początkowego
                        if(graph.Distance(currentPath[currentPath.Count-1],0) > 0)
                        {
                            //Wracamy do miasta początkowego
                            AddNode(0, currentPath);
                            //Jeśli nie ma jeszcze wyznaczonej ścieżki, lub obecna jest lepsza to ją zapisujemy
                            if(optimalPath == null || graph.GetPathLength(currentPath) < graph.GetPathLength(optimalPath))
                            {
                                optimalPath = new List<int>(currentPath);                                
                            }
                        }
                    }
                    //Przypadek, gdy obecna ścieżka nie może być uznana za rozwiązanie (nie odwiedzono wszystkich miast)
                    else
                    {
                        for (int i = 0; i < graph.GetSize(); i++)
                        {
                            //Sprawdzamy czy miasto, które rozważamy nie było już odwiedzone i czy istnieje prowadząca do niego droga
                            if (!graph.CheckVisited(i, currentPath) && graph.Distance(currentPath[currentPath.Count - 1], i) > 0)
                            {
                                List<int> newPath = new List<int>(currentPath);
                                //Dodajemy węzeł do ścieżki, przy okazji wyliczamy górną granicę
                                int bound = AddNode(i, newPath);
                                //Daną ścieżkę rozważać będziemy dalej tylko wtedy, kiedy nie ma jeszcze proponowanej ścieżki, lub jest ona bardziej obiecująca od obecnej wartości
                                if (optimalPath == null || bound < graph.GetPathLength(optimalPath))
                                {
                                    //Dodajemy nową ścieżkę do kolejki, wyliczona górna granica będzie kluczem, a nowa ścieżka wartością
                                    queue.Enqueue(bound, newPath);
                                }
                            }
                        }
                    }
                }
               
            }
            return optimalPath;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //Programowanie Dynamiczne

        public Tuple<int,int> DynamicCheck(int begin, List<int> toVisit)
        {
            Tuple<int,List<int>> key = new Tuple<int,List<int>>(begin,toVisit);
            //Item1 - miasto, Item2 - odległość
            if(memory.ContainsKey(key))
            {
                return memory[key];
            }
            else
            {
                return null;
            }
        }

        //Algorytm Dynamiczny
        public List<int> Dynamic()
        {
            List<int> optimalPath = new List<int>();
            //Ustawiamy miasto początkowe na 0
            int begin = 0;
            int lastVisited = -1;
            path = new List<int>();
            //Dodajemy pozostałe miasta do zestawu miast do odwiedzenia
            for(int i =1; i< graph.GetSize(); i++)
            {
                path.Add(i);
            }
            //Wykonujemy tak długo aż nie wrócimy do początku
            while(lastVisited != 0)
            {
                //Wybieramy następne miasto algorytmem rekurencyjnym i dodajemy je do optymalnej ścieżki
                lastVisited = recTSP(begin, path).Key;
                optimalPath.Add(lastVisited);
                //Ustawiamy ostatnio odwiedzone miasto na obecnie rozważane
                begin = lastVisited;
                //Usuwamy odwiedzone miasto z zestawu do odwiedzenia
                path.Remove(lastVisited);
            }
            optimalPath.Reverse();
            return optimalPath;
        }

        //Rekurencyjne wykonywanie podproblemów
        //Funkcja zwraca parę Klucz - miasto, Wartość - odległość
        public KeyValuePair<int,int> recTSP(int begin, List<int> toVisit)
        {
            Tuple<int, int> check = DynamicCheck(begin, toVisit);
            if (check != null)
            {
                return new KeyValuePair<int, int>(check.Item1, check.Item2);
            }
            else
            {
                //Sprawdzamy, czy miasto docelowe nie znajduje się w liście miast do odwiedzenia
                foreach (int city in toVisit)
                {
                    if (begin == city)
                    {
                        return new KeyValuePair<int, int>(-1, -1);
                    }
                }
                //Wykonujemy tylko jeśli lista miast do odwiedzenia jest niepusta
                if (toVisit.Count > 0)
                {
                    //Tworzymy słownik podproblemów, klucz - miasto, wartość - odległość
                    Dictionary<int, int> subProblems = new Dictionary<int, int>();
                    //Dla każdego miasta na zadanej trasie tworzymy podproblemy
                    foreach (int lastVisited in toVisit)
                    {
                        List<int> newProblem = new List<int>(toVisit);
                        //Usuwamy rozważane miasto z listy miast do przejścia
                        newProblem.Remove(lastVisited);
                        //Wywołanie rekurencji
                        try
                        {                           
                            subProblems.Add(lastVisited, graph.Distance(lastVisited, begin) + recTSP(lastVisited, newProblem).Value);
                        }
                        catch (ArgumentException exc)
                        {
                            continue;
                        }
                    }
                    //Sortowanie listy za pomocą LINQ
                    var min = subProblems.OrderBy(kvp => kvp.Value).First();
                    Tuple<int, List<int>> key = new Tuple<int, List<int>>(begin, toVisit);
                    Tuple<int, int> value = new Tuple<int, int>(min.Key, min.Value);
                    memory.Add(key, value);
                    //Zwracamy następne miasto i odległość do niego
                    return new KeyValuePair<int, int>(min.Key, min.Value);
                }
                //Sprawdzono już wszystkie miasta, koniec rozważania
                else
                {
                    return new KeyValuePair<int, int>(0, graph.Distance(0, begin));
                }
            }
        }
    }
}
