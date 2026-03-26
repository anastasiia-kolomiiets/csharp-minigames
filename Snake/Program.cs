namespace Snake
{
    internal class Program
    {
        const int Width = 12;
        const int Height = 12;
        const char Empty = '·';
        const char PlayerTail = '█';
        const char ComputerTail = '▓';
        const char PlayerHead = '☻';
        const char ComputerHead = '☺';

        static char[,] board = new char[Height, Width];
        static (int x, int y) playerPos;
        static (int x, int y) computerPos;
        static Random rnd = new Random();

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            InitializeGame();
            DrawBoard();

            bool playerTurn = true;

            while (true)
            {
                if (playerTurn)
                {
                    Console.WriteLine("\nВаш хід (w/a/s/d): ");
                    var key = Console.ReadKey(true).Key;
                    var dir = KeyToDirection(key);
                    if (dir == (0, 0) || !CanMove(playerPos.x + dir.dx, playerPos.y + dir.dy))
                    {
                        Console.WriteLine("Неприпустимий хід! Ви програли.");
                        break;
                    }
                    MovePlayer(dir.dx, dir.dy);
                }
                else
                {
                    Console.WriteLine("\nХід комп'ютера...");
                    Thread.Sleep(600);
                    var move = GetBestComputerMove();
                    if (move.dx == 0 && move.dy == 0)
                    {
                        Console.WriteLine("Комп'ютер не має ходу! Ви перемогли!");
                        break;
                    }
                    MoveComputer(move.dx, move.dy);
                }

                DrawBoard();
                if (CheckGameOver())
                    break;

                playerTurn = !playerTurn;
            }

            Console.WriteLine("\nГру закінчено. Натисніть будь-яку клавішу...");
            Console.ReadKey();
        }

        static void InitializeGame()
        {
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    board[y, x] = Empty;

            playerPos = (1, Height / 2);
            board[playerPos.y, playerPos.x] = PlayerHead;

            computerPos = (Width - 2, Height / 2);
            board[computerPos.y, computerPos.x] = ComputerHead;
        }

        static void DrawBoard()
        {
            Console.Clear();
            Console.WriteLine("═══ ЗМІЙКА ПРОТИ КОМП'ЮТЕРА ═══");
            Console.WriteLine("Ви: █ (керування w/a/s/d)    Комп'ютер: ▓\n");

            for (int y = 0; y < Height; y++)
            {
                Console.Write("║ ");
                for (int x = 0; x < Width; x++)
                {
                    Console.Write(board[y, x] + " ");
                }
                Console.WriteLine("║");
            }
            Console.WriteLine(new string('═', Width * 2 + 3));
        }

        static (int dx, int dy) KeyToDirection(ConsoleKey key)
        {
            return key switch
            {
                ConsoleKey.W => (0, -1),
                ConsoleKey.S => (0, 1),
                ConsoleKey.A => (-1, 0),
                ConsoleKey.D => (1, 0),
                _ => (0, 0)
            };
        }

        static bool IsValid(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;

        static bool CanMove(int x, int y) => IsValid(x, y) && board[y, x] == Empty;

        static void MovePlayer(int dx, int dy)
        {
            int nx = playerPos.x + dx;
            int ny = playerPos.y + dy;

            board[playerPos.y, playerPos.x] = PlayerTail;
            playerPos = (nx, ny);
            board[ny, nx] = PlayerHead;
        }

        static void MoveComputer(int dx, int dy)
        {
            int nx = computerPos.x + dx;
            int ny = computerPos.y + dy;

            board[computerPos.y, computerPos.x] = ComputerTail;
            computerPos = (nx, ny);
            board[ny, nx] = ComputerHead;
        }

        static (int dx, int dy) GetBestComputerMove()
        {
            var directions = new (int dx, int dy)[] { (0, -1), (0, 1), (-1, 0), (1, 0) };

            // Спробуємо знайти хід, який залишає більше вільного місця для комп'ютера
            (int dx, int dy) best = (0, 0);
            int maxSpace = -1;

            foreach (var d in directions)
            {
                int nx = computerPos.x + d.dx;
                int ny = computerPos.y + d.dy;
                if (CanMove(nx, ny))
                {
                    // скільки вільних клітинок доступно після ходу
                    board[computerPos.y, computerPos.x] = ComputerTail;
                    board[ny, nx] = ComputerHead;
                    int space = FloodFillCount(nx, ny);
                    board[computerPos.y, computerPos.x] = ComputerHead;
                    board[ny, nx] = Empty;

                    if (space > maxSpace)
                    {
                        maxSpace = space;
                        best = d;
                    }
                }
            }

            // Якщо немає розумного ходу — беремо будь-який можливий
            if (best.dx == 0 && best.dy == 0)
            {
                foreach (var d in directions)
                {
                    if (CanMove(computerPos.x + d.dx, computerPos.y + d.dy))
                        return d;
                }
            }

            return best;
        }

        // Підрахунок доступного простору від позиції (для комп'ютера)
        static int FloodFillCount(int x, int y)
        {
            bool[,] visited = new bool[Height, Width];
            Queue<(int x, int y)> q = new Queue<(int, int)>();
            q.Enqueue((x, y));
            visited[y, x] = true;
            int count = 0;

            var dirs = new (int dx, int dy)[] { (0, -1), (0, 1), (-1, 0), (1, 0) };

            while (q.Count > 0)
            {
                var pos = q.Dequeue();
                count++;

                foreach (var d in dirs)
                {
                    int nx = pos.x + d.dx;
                    int ny = pos.y + d.dy;
                    if (IsValid(nx, ny) && !visited[ny, nx] && board[ny, nx] == Empty)
                    {
                        visited[ny, nx] = true;
                        q.Enqueue((nx, ny));
                    }
                }
            }
            return count;
        }

        static bool CheckGameOver()
        {
            // Перевірка, чи може хтось ходити
            var dirs = new (int dx, int dy)[] { (0, -1), (0, 1), (-1, 0), (1, 0) };
            bool playerCanMove = false, compCanMove = false;

            foreach (var d in dirs)
            {
                if (CanMove(playerPos.x + d.dx, playerPos.y + d.dy))
                    playerCanMove = true;
                if (CanMove(computerPos.x + d.dx, computerPos.y + d.dy))
                    compCanMove = true;
            }

            if (!playerCanMove && !compCanMove)
            {
                Console.WriteLine("Ніхто не може ходити! Нічия!");
                return true;
            }
            if (!playerCanMove)
            {
                Console.WriteLine("У вас немає ходу! Ви програли.");
                return true;
            }
            if (!compCanMove)
            {
                Console.WriteLine("У комп'ютера немає ходу! Ви перемогли!");
                return true;
            }

            return false;
        }
    }
}
