using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumbersGame
{
    internal class GameEngine
    {
        private string playerSecret = "";
        private string computerSecret = "";
        private readonly ComputerPlayer computer = new();

        public void Play()
        {
            Console.Clear();

            // Крок 1: гравець вводить своє число
            while (true)
            {
                Console.Write("Загадай своє 4-цифрове число (різні цифри): ");
                playerSecret = Console.ReadLine()!.Trim();

                if (Helpers.IsValid(playerSecret))
                    break;

                Console.WriteLine("Помилка! Введіть 4 різні цифри.\n");
            }

            // Крок 2: комп’ютер загадує
            computerSecret = string.Concat(NumberGenerator.Generate());

            Console.WriteLine("\nЯ теж загадав число. Починаємо гру — ходи по черзі!");
            Console.WriteLine("Ти ходиш першою.\n");
            Thread.Sleep(1500);

            bool playerTurn = true;
            int move = 0;

            while (true)
            {
                if (playerTurn)
                {
                    move++;
                    // === Твоя черга ===
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write($"Твій хід #{move}: ");
                    Console.ResetColor();

                    string guess = Console.ReadLine()!.Trim();

                    if (!Helpers.IsValid(guess))
                    {
                        Console.WriteLine("Невірне число — спробуй ще раз.\n");
                        move--;
                        continue;
                    }

                    var (bulls, cows) = Helpers.GetScore(guess, computerSecret);
                    Console.WriteLine($"    → {Helpers.FormatResult(bulls, cows)}\n");

                    if (bulls == 4)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Вітаю! Ти виграла за {move} ходів!");
                        Console.ResetColor();
                        return;
                    }
                }
                else
                {
                    // === Черга комп’ютера ===
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"Мій хід #{move}...");
                    Console.ResetColor();

                    string guess = computer.MakeGuess();
                    var (bulls, cows) = Helpers.GetScore(guess, playerSecret);

                    Console.WriteLine($"Я кажу: {guess}");
                    Console.WriteLine($"    → {Helpers.FormatResult(bulls, cows)}\n");

                    if (bulls == 4)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Я виграв! Вгадав твоє число за {move} ходів");
                        Console.WriteLine($"Твоє число було: {playerSecret}");
                        Console.ResetColor();
                        return;
                    }

                    computer.ReceiveFeedback(guess, bulls, cows);
                    Thread.Sleep(1300);
                }

                playerTurn = !playerTurn;
            }
        }
    }
}
