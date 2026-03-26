using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumbersGame
{
    internal class NumberGenerator
    {
        private static readonly Random rnd = new Random();

        public static int[] Generate()
        {
            var digits = Enumerable.Range(0, 10).ToList();
            var result = new int[4];

            for (int i = 0; i < 4; i++)
            {
                int index = rnd.Next(digits.Count);
                result[i] = digits[index];
                digits.RemoveAt(index);
            }

            return result;
        }

        public static string ToString(int[] number) => string.Concat(number);
    }
}
