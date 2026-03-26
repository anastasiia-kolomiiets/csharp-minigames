using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumbersGame
{
    internal class ComputerPlayer
    {
        private readonly List<string> possible = new();
        private readonly List<(string guess, int bulls, int cows)> history = new();

        public ComputerPlayer()
        {
            // Заповнюємо всі можливі 4-значні числа без повторів
            for (int i = 0; i < 10000; i++)
            {
                string s = i.ToString("D4");
                if (s.Distinct().Count() == 4)
                    possible.Add(s);
            }
        }

        public string MakeGuess()
        {
            // Перший хід — завжди 0123 (або можна рандом)
            if (history.Count == 0)
                return "0123";

            // Фільтруємо можливі числа за всіма попередніми відповідями
            var currentPossible = new List<string>(possible);

            foreach (var (guess, bulls, cows) in history)
            {
                currentPossible.RemoveAll(cand => !MatchesScore(guess, cand, bulls, cows));
            }

            // Повертаємо перше (найпростіший, але дуже ефективний вибір)
            return currentPossible.FirstOrDefault() ?? "0123";
        }

        public void ReceiveFeedback(string guess, int bulls, int cows)
        {
            history.Add((guess, bulls, cows));
        }

        private static bool MatchesScore(string guess, string candidate, int targetBulls, int targetCows)
        {
            int b = 0, c = 0;
            for (int i = 0; i < 4; i++)
            {
                if (guess[i] == candidate[i]) b++;
                else if (candidate.Contains(guess[i])) c++;
            }
            return b == targetBulls && c == targetCows;
        }
    }
}
