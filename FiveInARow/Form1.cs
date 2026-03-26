using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Five
{
    public partial class Form1 : Form
    {
        const int SIZE = 10;
        const int CELL_SIZE = 35;
        const int WIN_COUNT = 5;
        const char EMPTY = '.';
        const char PLAYER = 'X';
        const char COMPUTER = 'O';

        char[,] board = new char[SIZE, SIZE];
        bool isPlayerTurn = true;
        Button[,] buttons = new Button[SIZE, SIZE];
        Label statusLabel;

        public Form1()
        {
            InitializeComponent();
            InitializeBoard();
            CreateBoard();
            UpdateStatus();
        }

        void InitializeBoard()
        {
            for (int i = 0; i < SIZE; i++)
                for (int j = 0; j < SIZE; j++)
                    board[i, j] = EMPTY;
        }

        void CreateBoard()
        {
            int offsetX = 20, offsetY = 60;

            for (int i = 0; i < SIZE; i++)
            {
                for (int j = 0; j < SIZE; j++)
                {
                    Button btn = new Button
                    {
                        Width = CELL_SIZE,
                        Height = CELL_SIZE,
                        Left = offsetX + j * CELL_SIZE,
                        Top = offsetY + i * CELL_SIZE,
                        Font = new Font("Arial", 16, FontStyle.Bold),
                        Tag = new Point(i, j),
                        BackColor = Color.LightGray,
                        FlatStyle = FlatStyle.Flat
                    };
                    btn.FlatAppearance.BorderSize = 1;
                    btn.FlatAppearance.BorderColor = Color.DarkGray;
                    btn.Click += Cell_Click;
                    buttons[i, j] = btn;
                    this.Controls.Add(btn);
                }
            }

            // Кнопка "Нова гра"
            Button newGameBtn = new Button
            {
                Text = "Нова гра",
                Left = offsetX,
                Top = offsetY + SIZE * CELL_SIZE + 20,
                Width = 120,
                Height = 40,
                Font = new Font("Arial", 12, FontStyle.Bold)
            };
            newGameBtn.Click += (s, e) => RestartGame();
            this.Controls.Add(newGameBtn);

            // Статус
            statusLabel = new Label
            {
                Text = "",
                Left = offsetX + 140,
                Top = offsetY + SIZE * CELL_SIZE + 30,
                Width = 300,
                Height = 25,
                Font = new Font("Arial", 11)
            };
            this.Controls.Add(statusLabel);

            this.ClientSize = new Size(
                offsetX * 2 + SIZE * CELL_SIZE,
                offsetY + SIZE * CELL_SIZE + 100
            );
            this.Text = "5 в ряд — Гравець vs Комп'ютер";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
        }

        void Cell_Click(object sender, EventArgs e)
        {
            if (!isPlayerTurn) return;

            Button btn = (Button)sender;
            Point pos = (Point)btn.Tag;
            int row = pos.X, col = pos.Y;

            if (board[row, col] != EMPTY) return;

            MakeMove(row, col, PLAYER);
            btn.Text = "X";
            btn.ForeColor = Color.Blue;
            btn.Enabled = false;

            if (CheckWin(PLAYER))
            {
                MessageBox.Show("Ви виграли!", "Перемога!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DisableBoard();
                return;
            }

            if (IsBoardFull())
            {
                MessageBox.Show("Нічия!", "Гра закінчена", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            isPlayerTurn = false;
            UpdateStatus();
            this.Refresh();
            System.Threading.Thread.Sleep(300); // Пауза перед ходом комп'ютера

            ComputerMove();
        }

        void ComputerMove()
        {
            statusLabel.Text = "Комп'ютер думає...";
            this.Refresh();

            int bestScore = int.MinValue;
            int bestRow = -1, bestCol = -1;

            // Спроба центру
            int center = SIZE / 2;
            if (board[center, center] == EMPTY)
            {
                bestRow = center; bestCol = center;
            }
            else
            {
                // Мінімакс
                for (int i = 0; i < SIZE; i++)
                {
                    for (int j = 0; j < SIZE; j++)
                    {
                        if (board[i, j] == EMPTY)
                        {
                            board[i, j] = COMPUTER;
                            int score = Minimax(board, 0, false, int.MinValue, int.MaxValue);
                            board[i, j] = EMPTY;
                            if (score > bestScore)
                            {
                                bestScore = score;
                                bestRow = i;
                                bestCol = j;
                            }
                        }
                    }
                }
            }

            if (bestRow != -1 && bestCol != -1)
            {
                MakeMove(bestRow, bestCol, COMPUTER);
                Button btn = buttons[bestRow, bestCol];
                btn.Text = "O";
                btn.ForeColor = Color.Red;
                btn.Enabled = false;

                if (CheckWin(COMPUTER))
                {
                    MessageBox.Show("Комп'ютер виграв!", "Поразка", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    DisableBoard();
                }
                else if (IsBoardFull())
                {
                    MessageBox.Show("Нічия!", "Гра закінчена", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            isPlayerTurn = true;
            UpdateStatus();
        }

        void MakeMove(int row, int col, char player)
        {
            board[row, col] = player;
        }

        int Minimax(char[,] state, int depth, bool isMaximizing, int alpha, int beta)
        {
            if (CheckWin(COMPUTER)) return 1000 - depth;
            if (CheckWin(PLAYER)) return -1000 + depth;
            if (IsBoardFull() || depth >= 3) return 0;

            if (isMaximizing)
            {
                int maxEval = int.MinValue;
                for (int i = 0; i < SIZE; i++)
                    for (int j = 0; j < SIZE; j++)
                    {
                        if (state[i, j] == EMPTY)
                        {
                            state[i, j] = COMPUTER;
                            int eval = Minimax(state, depth + 1, false, alpha, beta);
                            state[i, j] = EMPTY;
                            maxEval = Math.Max(maxEval, eval);
                            alpha = Math.Max(alpha, eval);
                            if (beta <= alpha) return maxEval;
                        }
                    }
                return maxEval;
            }
            else
            {
                int minEval = int.MaxValue;
                for (int i = 0; i < SIZE; i++)
                    for (int j = 0; j < SIZE; j++)
                    {
                        if (state[i, j] == EMPTY)
                        {
                            state[i, j] = PLAYER;
                            int eval = Minimax(state, depth + 1, true, alpha, beta);
                            state[i, j] = EMPTY;
                            minEval = Math.Min(minEval, eval);
                            beta = Math.Min(beta, eval);
                            if (beta <= alpha) return minEval;
                        }
                    }
                return minEval;
            }
        }

        bool CheckWin(char player)
        {
            // Горизонталі та вертикалі
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = 0; j <= SIZE - WIN_COUNT; j++)
                {
                    bool h = true, v = true;
                    for (int k = 0; k < WIN_COUNT; k++)
                    {
                        if (board[i, j + k] != player) h = false;
                        if (board[j + k, i] != player) v = false;
                    }
                    if (h || v) return true;
                }
            }

            // Діагоналі
            for (int i = 0; i <= SIZE - WIN_COUNT; i++)
            {
                for (int j = 0; j <= SIZE - WIN_COUNT; j++)
                {
                    bool d1 = true, d2 = true;
                    for (int k = 0; k < WIN_COUNT; k++)
                    {
                        if (board[i + k, j + k] != player) d1 = false;
                        if (board[i + k, j + WIN_COUNT - 1 - k] != player) d2 = false;
                    }
                    if (d1 || d2) return true;
                }
            }
            return false;
        }

        bool IsBoardFull()
        {
            for (int i = 0; i < SIZE; i++)
                for (int j = 0; j < SIZE; j++)
                    if (board[i, j] == EMPTY) return false;
            return true;
        }

        void UpdateStatus()
        {
            statusLabel.Text = isPlayerTurn ? "Ваш хід (X)" : "Хід комп'ютера (O)";
        }

        void DisableBoard()
        {
            foreach (var btn in buttons)
                btn.Enabled = false;
        }

        void RestartGame()
        {
            InitializeBoard();
            foreach (var btn in buttons)
            {
                btn.Text = "";
                btn.Enabled = true;
                btn.BackColor = Color.LightGray;
            }
            isPlayerTurn = true;
            UpdateStatus();
        }
    }
}
