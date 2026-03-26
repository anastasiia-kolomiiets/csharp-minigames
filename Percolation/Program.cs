using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace percolation
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int m = 20; // рядки
            int n = 20; // колонки
            int total = m * n;
            int trials = 100; // кількість спроб (100%)
            double step = 0.01; // крок (1%)
            Random rnd = new Random();

            Console.WriteLine("Percentage Blocked | Probability of Percolation");

            for (double f = 0; f <= 1.0; f += step)
            {
                int k = (int)(f * total); // кількість перекритих вузлів
                int count = 0;
                for (int t = 0; t < trials; t++)
                {
                    if (PercolationSimulator.DoesPercolate(m, n, k, rnd))
                    {
                        count++;
                    }
                }
                double prob = (double)count / trials;
                Console.WriteLine($"{f * 100:F2}% | {prob:F4}");
            }

            Console.WriteLine("X-axis: Percentage Blocked (0% to 100%)");
            Console.WriteLine("Y-axis: Probability of Water Passing (0 to 1)");
        }
    }
}
