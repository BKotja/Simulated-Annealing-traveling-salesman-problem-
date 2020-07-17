using System;
using System.Collections.Generic;
using System.IO;

namespace symulowanie_wyżarzania_komiwojażera
{

    /// <summary>
    /// Glownie algorytm polega na kilku dzialaniach:
    /// 1. Inicjalizacja danych
    /// 2. Losowa zamiana 2 miast w kolejce
    /// 3. Porownanie czy calkowity dystans nowej trasy jest lepszy od starego lub spelnia ona warunek Boltzmana
    /// 4. Jesli 3 punkt jest prawda to zastapiamy stara kolejku na nowa
    /// 5. Jesli temperatura > epsilon idz do kroku 2
    /// </summary>
    public class TravellingSalesmantravellingsalesmanproblem
    {
        public List<int> currentOrder = new List<int>();
        private List<int> nextOrder = new List<int>();
        private double[,] distances;
        private Random random = new Random();
        public double shortestDistance = 0;

      
        private void LoadCities()   //inicjalizacja miast z pliku
        {
            string cities = File.ReadAllText("Miasta.txt");
            string[] rows = cities.Split('\n');
            distances = new double[rows.Length, rows.Length];

            for (int i = 0; i < rows.Length; i++)
            {
                string[] distance = rows[i].Split(' ');
                for (int j = 0; j < distance.Length; j++)
                {
                     distances[i, j] = double.Parse(distance[j]);   //PILNE: uwazac na znaki w pliku oraz puste linii i ewentualnie zamenic "," na "." (moze spowodowac blad)
                }

                currentOrder.Add(i);    //ilosci miast odpowiada ilosc wierszow w naszym plikie, dodajemy ich po kolei do kolejki
            }
        }

        private double CountDistance(List<int> order)   //liczymy wage (odleglosc) trasy wedlug przekazanej kolejki
        {
            double distance = 0;

            for (int i = 0; i < order.Count - 1; i++)
                distance += distances[order[i], order[i + 1]];

            if (order.Count > 0)
                distance += distances[order[order.Count - 1], 0];

            return distance;
        }

        private List<int> Swap2Cities(List<int> order)   //bierzemy 2 losowych miasta i zamieniamy ich miejscami w kolejce
        {
            List<int> newOrder = new List<int>();

            for (int i = 0; i < order.Count; i++)
                newOrder.Add(order[i]);

            int firstRandomCityIndex = random.Next(1, newOrder.Count);  //zaczynamy od 0, wiec losujemy w zakresie od 1 miasta do miasta N (ostatniego indeksu)
            int secondRandomCityIndex = random.Next(1, newOrder.Count);

            int tmp = newOrder[firstRandomCityIndex];
            newOrder[firstRandomCityIndex] = newOrder[secondRandomCityIndex];
            newOrder[secondRandomCityIndex] = tmp;

            return newOrder;
        }

        public void Anneal()    //wyzarzanie 
        {
            double temperature = 10000.0;
            double deltaDistance = 0;
            double cooling = 0.9999;
            double eps = 0.00001;

            LoadCities();

            double distance = CountDistance(currentOrder);  //liczymy dystans trasy w przypadku bez zadnych zmian

            while (temperature > eps)
            {
                nextOrder = Swap2Cities(currentOrder);   //zamieniamy 2 miasta

                deltaDistance = CountDistance(nextOrder) - distance;    //porownujemy nowy dystans trasy (po zamianie tych 2 miast miejscami) ze starym
                
                if ((deltaDistance < 0) || (distance > 0 && Math.Exp(-deltaDistance / temperature) > random.NextDouble())) //zastapiamy stara trase na nowa w przypadku gdy nowa trasa ma mniejszy dystans albo spelnia ona warunek Boltzmana
                {
                    for (int i = 0; i < nextOrder.Count; i++)
                        currentOrder[i] = nextOrder[i];

                    distance = deltaDistance + distance;
                }
              
                temperature *= cooling; //zmniejszamy temperature
            }

            shortestDistance = distance;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            TravellingSalesmantravellingsalesmanproblem travellingsalesmanproblem = new TravellingSalesmantravellingsalesmanproblem();
            travellingsalesmanproblem.Anneal();

            string result = "";

            foreach (var tmp in travellingsalesmanproblem.currentOrder)
                result += tmp + " ";

            Console.WriteLine("Shortest Route: " + result);
            Console.WriteLine("Total cities: " + travellingsalesmanproblem.currentOrder.Count.ToString());
            Console.WriteLine("The shortest distance is: " + travellingsalesmanproblem.shortestDistance.ToString());
        }
    }
}
