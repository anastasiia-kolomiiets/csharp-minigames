using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumbersGame
{
    internal class Helpers
    {
        public static bool IsValid(string input)
        {
            return input.Length == 4 &&
                   int.TryParse(input, out _) &&
                   input.Distinct().Count() == 4;
        }

        public static (int bulls, int cows) GetScore(string guess, string secret)
        {
            int bulls = 0, cows = 0;
            for (int i = 0; i < 4; i++)
            {
                if (guess[i] == secret[i]) bulls++;
                else if (secret.Contains(guess[i])) cows++;
            }
            return (bulls, cows);
        }

        public static string FormatResult(int bulls, int cows)
        {
            string b = bulls switch { 1 => "бик", 2 or 3 or 4 => "бики", _ => "биків" };
            string c = cows switch { 1 => "корова", 2 or 3 or 4 => "корови", _ => "коров" };
            return $"{bulls} {b}, {cows} {c}";
        }
    }
}
