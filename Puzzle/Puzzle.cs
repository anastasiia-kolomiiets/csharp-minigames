using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puzzle
{
    internal class Puzzle
    {
        private int[,] board = new int[4, 4];
        private int emptyX = 3, emptyY = 3;
        private Random rand = new Random();
        private bool isSolved = false;

        public Puzzle()
        {
            InitializeBoard();
        }

        // Ініціалізація дошки
        private void InitializeBoard()
        {
            int num = 1;
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                {
                    board[i, j] = num++;
                }
            board[3, 3] = 0;
            emptyX = 3;
            emptyY = 3;
        }

        // Перемішування дошки
        public void Shuffle()
        {
            for (int i = 0; i < 1000; i++)
            {
                int direction = rand.Next(4);
                int dx = 0, dy = 0;
                switch (direction)
                {
                    case 0: dx = -1; break; // вгору
                    case 1: dx = 1; break;  // вниз
                    case 2: dy = -1; break; // вліво
                    case 3: dy = 1; break;  // вправо
                }
                int newX = emptyX + dx;
                int newY = emptyY + dy;
                if (newX >= 0 && newX < 4 && newY >= 0 && newY < 4)
                {
                    board[emptyX, emptyY] = board[newX, newY];
                    board[newX, newY] = 0;
                    emptyX = newX;
                    emptyY = newY;
                }
            }
            isSolved = false;
        }

        // Рух фішки
        public bool Move(char direction)
        {
            int dx = 0, dy = 0;
            switch (char.ToLower(direction))
            {
                case 'w': dx = -1; break; // вгору
                case 's': dx = 1; break;  // вниз
                case 'a': dy = -1; break; // вліво
                case 'd': dy = 1; break;  // вправо
                default: return false;
            }

            int newX = emptyX + dx;
            int newY = emptyY + dy;

            if (newX >= 0 && newX < 4 && newY >= 0 && newY < 4)
            {
                board[emptyX, emptyY] = board[newX, newY];
                board[newX, newY] = 0;
                emptyX = newX;
                emptyY = newY;
                isSolved = false; // скидаємо кеш
                return true;
            }
            return false;
        }

        // Перевірка виграшу (з кешуванням)
        public bool IsSolved()
        {
            if (isSolved) return true;

            int num = 1;
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                {
                    if (i == 3 && j == 3)
                    {
                        isSolved = (board[i, j] == 0);
                        return isSolved;
                    }
                    if (board[i, j] != num++)
                    {
                        isSolved = false;
                        return false;
                    }
                }
            isSolved = true;
            return true;
        }

        // Отримання дошки для малювання
        public int[,] GetBoard()
        {
            return (int[,])board.Clone();
        }

        // Позиція порожньої клітинки (для розширення)
        public (int x, int y) GetEmptyPosition()
        {
            return (emptyX, emptyY);
        }

        // Reset до початкового стану
        public void Reset()
        {
            InitializeBoard();
            isSolved = false;
        }
    }
}
