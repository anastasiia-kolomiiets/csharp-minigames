namespace Football
{
    enum PlayerType
    {
        Human = 0,
        Computer = 1
    }

    class Program
    {
        const int Height = 12;
        const int Width = 16;
        const int GoalSize = 6;

        static int ballRow;
        static int ballCol;

        static int humanScore = 0;
        static int computerScore = 0;

        static int goalTopRow = (Height - GoalSize) / 2;
        static int goalBottomRow = (Height - GoalSize) / 2 + GoalSize - 1;

        static HashSet<(int r, int c)> visitedCells = new HashSet<(int r, int c)>();

        static readonly Dictionary<string, (int dr, int dc)> Directions =
            new Dictionary<string, (int dr, int dc)>(StringComparer.OrdinalIgnoreCase)
            {
                { "U",  (-1,  0) },
                { "D",  ( 1,  0) },
                { "L",  ( 0, -1) },
                { "R",  ( 0,  1) },
                { "UL", (-1, -1) },
                { "UR", (-1,  1) },
                { "DL", ( 1, -1) },
                { "DR", ( 1,  1) }
            };

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            ResetBallToCenter();

            PlayerType currentPlayer = PlayerType.Human;

            while (humanScore < 3 && computerScore < 3)
            {
                DrawField();

                bool foul = TakeTurn(currentPlayer);

                if (humanScore >= 3 || computerScore >= 3)
                    break;

                if (!foul)
                    currentPlayer = currentPlayer == PlayerType.Human ? PlayerType.Computer : PlayerType.Human;
            }

            Console.Clear();
            DrawField();
            Console.WriteLine();
            Console.WriteLine($"КІНЕЦЬ ГРИ! Рахунок: Ти {humanScore} : {computerScore} Комп'ютер");
            Console.WriteLine(humanScore > computerScore ? "Ти перемогла! 🎉" : "Комп'ютер переміг!");
        }

        static void ResetBallToCenter()
        {
            ballRow = Height / 2;
            ballCol = Width / 2;
            visitedCells.Clear();
            visitedCells.Add((ballRow, ballCol));
        }

        static void DrawField()
        {
            Console.Clear();
            Console.WriteLine("   0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 (x)");
            for (int r = 0; r < Height; r++)
            {
                Console.Write(r.ToString("D2") + " ");
                for (int c = 0; c < Width; c++)
                {
                    char ch = '.';
                    if (IsGoalCellLeft(r, c) || IsGoalCellRight(r, c))
                        ch = '|';
                    if (visitedCells.Contains((r, c)))
                        ch = '*';
                    if (r == ballRow && c == ballCol)
                        ch = 'O';
                    Console.Write(ch + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine("(y)");
        }

        static bool IsGoalCellLeft(int r, int c) => c == 0 && r >= goalTopRow && r <= goalBottomRow;
        static bool IsGoalCellRight(int r, int c) => c == Width - 1 && r >= goalTopRow && r <= goalBottomRow;
        static bool IsInsideField(int r, int c) => r >= 0 && r < Height && c >= 0 && c < Width;

        static bool IsGoalScored(PlayerType attacker)
        {
            if (attacker == PlayerType.Human && IsGoalCellRight(ballRow, ballCol)) return true;
            if (attacker == PlayerType.Computer && IsGoalCellLeft(ballRow, ballCol)) return true;
            return false;
        }

        static bool TakeTurn(PlayerType player)
        {
            int steps = 0;
            while (steps < 3)
            {
                DrawField();
                Console.WriteLine($"Рахунок: Ти {humanScore} : {computerScore} Комп'ютер");
                Console.WriteLine($"Крок {steps + 1} з 3");
                (int dr, int dc) dir;

                if (player == PlayerType.Human)
                {
                    Console.WriteLine("Керування: U, D, L, R, UL, UR, DL, DR (Enter — завершити хід).\n");
                    string input = Console.ReadLine().Trim();
                    if (input == "") break;
                    if (!Directions.TryGetValue(input, out dir))
                    {
                        Console.WriteLine("Невірний напрямок!");
                        Console.ReadKey();
                        continue;
                    }
                }
                else
                {
                    dir = GetComputerStepDirection();
                    Console.WriteLine($"Комп'ютер обирає: {DirectionName(dir)}");
                    System.Threading.Thread.Sleep(700);
                }

                int newR = ballRow + dir.dr;
                int newC = ballCol + dir.dc;

                if (!IsInsideField(newR, newC))
                {
                    if (player == PlayerType.Human)
                    {
                        Console.WriteLine("Не можна виходити за межі поля!");
                        Console.ReadKey();
                    }
                    break;
                }

                if (visitedCells.Contains((newR, newC)))
                {
                    Console.WriteLine("Перетин траєкторії! Штрафний супернику!");
                    if (player == PlayerType.Human) Console.ReadKey();
                    PlayerType opponent = player == PlayerType.Human ? PlayerType.Computer : PlayerType.Human;
                    ExecutePenaltyShot(opponent);
                    return true;
                }

                ballRow = newR;
                ballCol = newC;
                visitedCells.Add((ballRow, ballCol));
                steps++;

                if (IsGoalScored(player))
                {
                    if (player == PlayerType.Human) humanScore++;
                    else computerScore++;
                    Console.WriteLine(player == PlayerType.Human ? "ГОЛ! ⚽" : "Комп'ютер забив!");
                    Console.ReadKey();
                    ResetBallToCenter();
                    return false;
                }
            }
            return false;
        }

        static void ExecutePenaltyShot(PlayerType player)
        {
            int maxDist = 6;
            (int dr, int dc) dir;
            int dist;

            if (player == PlayerType.Human)
            {
                DrawField();
                Console.WriteLine("ШТРАФНИЙ КИДОК!");
                Console.Write("Напрямок: ");
                string d = Console.ReadLine().Trim();
                if (!Directions.TryGetValue(d, out dir))
                {
                    Console.WriteLine("Невірний напрямок!");
                    return;
                }
                Console.Write($"Відстань (1-{maxDist}): ");
                if (!int.TryParse(Console.ReadLine(), out dist)) dist = 3;
            }
            else
            {
                dir = GetComputerPenaltyDirection();
                dist = maxDist;
                Console.WriteLine($"Комп'ютер виконує штрафний: {DirectionName(dir)}, {dist} клітин.");
                System.Threading.Thread.Sleep(800);
            }

            for (int i = 0; i < dist; i++)
            {
                int newR = ballRow + dir.dr;
                int newC = ballCol + dir.dc;
                if (!IsInsideField(newR, newC)) break;
                ballRow = newR;
                ballCol = newC;
                visitedCells.Add((ballRow, ballCol));
                DrawField();
                System.Threading.Thread.Sleep(150);
                if (IsGoalScored(player))
                {
                    if (player == PlayerType.Human) humanScore++;
                    else computerScore++;
                    Console.WriteLine(player == PlayerType.Human ? "ГОЛ зі штрафного!" : "Комп'ютер забив зі штрафного!");
                    Console.ReadKey();
                    ResetBallToCenter();
                    return;
                }
            }
            Console.WriteLine("Штрафний завершено без голу.");
            Console.ReadKey();
        }

        static (int dr, int dc) GetComputerStepDirection()
        {
            var dirs = new List<(int, int)> { (0, -1), (-1, -1), (1, -1), (0, 1), (-1, 0), (1, 0) };
            foreach (var d in dirs)
            {
                int nr = ballRow + d.Item1, nc = ballCol + d.Item2;
                if (IsInsideField(nr, nc) && !visitedCells.Contains((nr, nc))) return d;
            }
            return (0, 0);
        }

        static (int dr, int dc) GetComputerPenaltyDirection()
        {
            int center = (goalTopRow + goalBottomRow) / 2;
            int dr = ballRow < center ? 1 : ballRow > center ? -1 : 0;
            return (dr, -1);
        }

        static string DirectionName((int dr, int dc) v)
        {
            foreach (var kv in Directions)
                if (kv.Value == v) return kv.Key;
            return $"{v.dr},{v.dc}";
        }
    }
}
