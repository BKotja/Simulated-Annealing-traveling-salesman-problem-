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
    /// 5. Zmniejszenie temperatury
    /// 6. Jesli temperatura > epsilon idz do kroku 2, w przeciwnym przypadku idz do kroku 7
    /// 7. Koniec programu
    /// </summary>
    public class TravellingSalesmanProblemSimulatedAnnealing
    {
        private List<int> currentOrder = new List<int>();
        private List<int> nextOrder = new List<int>();
        private double[,] distances;
        private Random random = new Random();
        private double bestDistance = 0;
        private string cities;

        public TravellingSalesmanProblemSimulatedAnnealing(string cities)
        {
            this.cities = cities;
        }

        private void InitCities()   //inicjalizacja miast z pliku
        {
            string[] rows = cities.Split('\n');
            distances = new double[rows.Length, rows.Length];

            for (int i = 0; i < rows.Length; i++)
            {
                string[] distance = rows[i].Split(' ');
                for (int j = 0; j < distance.Length; j++)
                     distances[i, j] = double.Parse(distance[j]);   //PILNE: uwazac na znaki w pliku oraz puste linii i ewentualnie zamenic "," na "." (moze spowodowac blad)

                currentOrder.Add(i);    //ilosci miast odpowiada ilosc wierszow w naszym plikie, dodajemy ich po kolei do kolejki
            }
        }

        private double RouteDistance(List<int> order)   //liczymy wage (odleglosc) trasy wedlug przekazanej kolejki
        {
            double distance = 0;

            for (int i = 0; i < order.Count - 1; i++)
                distance += distances[order[i], order[i + 1]];

            if (order.Count > 0)
                distance += distances[order[order.Count - 1], 0];

            return distance;
        }

        private List<int> SwapCities(List<int> order)   //bierzemy 2 losowych miasta i zamieniamy ich miejscami w kolejce
        {
            List<int> newOrder = new List<int>();
            int firstCity;
            int secondCity;
            int tmp;

            foreach (var city in order)
                newOrder.Add(city);

            firstCity = random.Next(1, newOrder.Count);  //zaczynamy od 0, wiec losujemy w zakresie od 1 miasta do miasta N (ostatniego indeksu)
            secondCity = random.Next(1, newOrder.Count);

            tmp = newOrder[firstCity];
            newOrder[firstCity] = newOrder[secondCity];
            newOrder[secondCity] = tmp;

            return newOrder;
        }

        public void AnnealingProcess()    //wyzarzanie 
        {
            double coolingRate = 0.99999;
            double Epsilon = 0.000000001;
            double delta;
            double tmpDistance;

            InitCities();
            tmpDistance = RouteDistance(currentOrder);  //liczymy dystans trasy w przypadku bez zadnych zmian

            for (double temperature = 100000.0; temperature > Epsilon; temperature *= coolingRate)
            {
                nextOrder = SwapCities(currentOrder);   //zamieniamy 2 miasta
                delta = RouteDistance(nextOrder) - tmpDistance;    //porownujemy nowy dystans trasy (po zamianie tych 2 miast miejscami) ze starym
                
                if ((delta < 0) || (tmpDistance > 0 && Math.Exp(-delta / temperature) > random.NextDouble())) //zastapiamy stara trase na nowa w przypadku gdy nowa trasa ma mniejszy dystans albo spelnia ona warunek Boltzmana
                {
                    for (int i = 0; i < nextOrder.Count; i++)
                        currentOrder[i] = nextOrder[i];

                    tmpDistance = delta + tmpDistance;
                }              
            }

            bestDistance = tmpDistance;
        }

        public string OutputScheme()
        {
            string result = "";

            for (int i = 0; i < currentOrder.Count; i++)
                if (!i.Equals(currentOrder.Count-1))
                    result += currentOrder[i] + " -> ";
                else
                    result += currentOrder[i];
            
            return result;
        }

        public override string ToString()               //wypisujemy wynik
        {
            string route = "Shortest route (city indexes): " + OutputScheme();
            string totalCities = "Total cities: " + currentOrder.Count.ToString();
            string shortestDistance = "The shortest distance is: " + this.bestDistance.ToString();

            string outputString = string.Format("{0}{1}{2}{3}{4}", route, Environment.NewLine, totalCities, Environment.NewLine, shortestDistance);
            return outputString;
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            string cities = File.ReadAllText("Miasta.txt");
            TravellingSalesmanProblemSimulatedAnnealing travellingSalesmanProblemSimulatedAnnealing = new TravellingSalesmanProblemSimulatedAnnealing(cities);
            travellingSalesmanProblemSimulatedAnnealing.AnnealingProcess();

            Console.WriteLine(travellingSalesmanProblemSimulatedAnnealing.ToString());
        }
    }
}
