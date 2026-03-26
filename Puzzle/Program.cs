namespace Puzzle
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var puzzle = new Puzzle();
            puzzle.Shuffle();
            DrawBoard(puzzle);

            while (!puzzle.IsSolved())
            {
                Console.WriteLine("\nВведіть напрямок (w=вгору, s=вниз, a=вліво, d=вправо, r=рестарт):");
                var key = Console.ReadKey().KeyChar;
                Console.WriteLine();

                switch (char.ToLower(key))
                {
                    case 'r':
                        puzzle.Reset();
                        puzzle.Shuffle();
                        break;
                    default:
                        if (!puzzle.Move(key))
                        {
                            Console.WriteLine("Неправильний хід!");
                        }
                        break;
                }

                DrawBoard(puzzle);
            }

            Console.WriteLine("\nВітаю! Ви виграли!");
        }

        // Малювання дошки
        static void DrawBoard(Puzzle puzzle)
        {
            Console.Clear();
            Console.WriteLine("=== П'ЯТНАШКИ 4x4 ===\n");

            var board = puzzle.GetBoard();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (board[i, j] == 0)
                        Console.Write("  □ ");
                    else
                        Console.Write($"{board[i, j],2} ");
                }
                Console.WriteLine();
            }

            Console.WriteLine("\n" + new string('═', 20));
            Console.WriteLine("  1  2  3  4");
            Console.WriteLine("  5  6  7  8");
            Console.WriteLine("  9 10 11 12");
            Console.WriteLine(" 13 14 15  □");
            Console.WriteLine(new string('═', 20));
        }
    }
}
