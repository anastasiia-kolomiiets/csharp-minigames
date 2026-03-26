using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sudoku
{
    public partial class Form1 : Form
    {
        private TextBox[,] cells = new TextBox[9, 9];
        private int[,] solution = new int[9, 9];
        private int[,] puzzle = new int[9, 9];
        private Timer timer;
        private int seconds = 0;
        private Label timerLabel;
        private ComboBox difficultyBox;

        public Form1()
        {
            InitializeComponent();
            CreateBoard();
            SetupTimer();
            NewGame("Середньо");
        }

        private void CreateBoard()
        {
            int cellSize = 50;
            int offsetX = 50, offsetY = 80;

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    TextBox tb = new TextBox
                    {
                        Width = cellSize,
                        Height = cellSize,
                        Left = offsetX + j * cellSize,
                        Top = offsetY + i * cellSize,
                        Font = new Font("Arial", 18, FontStyle.Bold),
                        TextAlign = HorizontalAlignment.Center,
                        MaxLength = 1,
                        Tag = new Point(i, j)
                    };

                    // Товсті лінії для блоків 3×3
                    if (i == 3 || i == 6) tb.Top += 3;
                    if (j == 3 || j == 6) tb.Left += 3;

                    tb.TextChanged += Cell_TextChanged;
                    tb.KeyPress += Cell_KeyPress;
                    cells[i, j] = tb;
                    this.Controls.Add(tb);
                }
            }

            // Кнопки
            Button newGameBtn = new Button { Text = "Нова гра", Left = 80, Top = 550, Width = 120, Height = 40 };
            newGameBtn.Click += (s, e) => NewGame(difficultyBox.SelectedItem?.ToString() ?? "Середньо");
            this.Controls.Add(newGameBtn);

            Button checkBtn = new Button { Text = "Перевірити", Left = 220, Top = 550, Width = 100, Height = 40 };
            checkBtn.Click += (s, e) => CheckSolution();
            this.Controls.Add(checkBtn);

            Button hintBtn = new Button { Text = "Підказка", Left = 340, Top = 550, Width = 100, Height = 40 };
            hintBtn.Click += (s, e) => GiveHint();
            this.Controls.Add(hintBtn);

            // Рівень складності
            difficultyBox = new ComboBox
            {
                Left = 460,
                Top = 555,
                Width = 120,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            difficultyBox.Items.AddRange(new[] { "Легко", "Середньо", "Важко", "Експерт" });
            difficultyBox.SelectedIndex = 1;
            this.Controls.Add(difficultyBox);

            // Таймер
            timerLabel = new Label
            {
                Text = "Час: 00:00",
                Left = 80,
                Top = 40,
                Font = new Font("Arial", 14),
                Width = 200
            };
            this.Controls.Add(timerLabel);

            this.ClientSize = new Size(650, 630);
            this.Text = "Судоку — Гра з генератором";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        private void SetupTimer()
        {
            timer = new Timer { Interval = 1000 };
            timer.Tick += (s, e) =>
            {
                seconds++;
                timerLabel.Text = $"Час: {seconds / 60:D2}:{seconds % 60:D2}";
            };
            timer.Start();
        }

        private void NewGame(string difficulty)
        {
            seconds = 0;
            GenerateSudoku();
            ApplyDifficulty(difficulty);
            DisplayPuzzle();
            timer.Start();
        }

        private void GenerateSudoku()
        {
            solution = new int[9, 9];
            puzzle = new int[9, 9];
            Random rand = new Random();

            // Заповнюємо діагональні 3×3 блоки (гарантія розв'язності)
            FillDiagonalBlocks();

            // Заповнюємо решту
            SolveSudoku(solution, 0, 0);

            // Копіюємо у puzzle
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    puzzle[i, j] = solution[i, j];
        }

        private void FillDiagonalBlocks()
        {
            for (int block = 0; block < 9; block += 3)
            {
                int[] nums = Enumerable.Range(1, 9).OrderBy(x => Guid.NewGuid()).ToArray();
                int idx = 0;
                for (int i = 0; i < 3; i++)
                    for (int j = 0; j < 3; j++)
                        solution[block + i, block + j] = nums[idx++];
            }
        }

        private bool SolveSudoku(int[,] board, int row, int col)
        {
            if (row == 9) return true;
            if (col == 9) return SolveSudoku(board, row + 1, 0);

            if (board[row, col] != 0) return SolveSudoku(board, row, col + 1);

            int[] nums = Enumerable.Range(1, 9).OrderBy(x => Guid.NewGuid()).ToArray();
            foreach (int num in nums)
            {
                if (IsSafe(board, row, col, num))
                {
                    board[row, col] = num;
                    if (SolveSudoku(board, row, col + 1)) return true;
                    board[row, col] = 0;
                }
            }
            return false;
        }

        private bool IsSafe(int[,] board, int row, int col, int num)
        {
            // Рядок і стовпець
            for (int x = 0; x < 9; x++)
                if (board[row, x] == num || board[x, col] == num) return false;

            // Блок 3×3
            int boxRow = row / 3 * 3;
            int boxCol = col / 3 * 3;
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (board[boxRow + i, boxCol + j] == num) return false;

            return true;
        }

        private void ApplyDifficulty(string level)
        {
            int cellsToRemove = 45; // Середньо за замовчуванням

            if (level == "Легко") cellsToRemove = 35;
            if (level == "Важко") cellsToRemove = 52;
            if (level == "Експерт") cellsToRemove = 58;

            Random rand = new Random();
            int removed = 0;
            while (removed < cellsToRemove)
            {
                int row = rand.Next(9);
                int col = rand.Next(9);
                if (puzzle[row, col] != 0)
                {
                    puzzle[row, col] = 0;
                    removed++;
                }
            }
        }

        private void DisplayPuzzle()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    var cell = cells[i, j];
                    cell.Text = puzzle[i, j] == 0 ? "" : puzzle[i, j].ToString();
                    cell.ForeColor = puzzle[i, j] == 0 ? Color.Blue : Color.Black;
                    cell.ReadOnly = puzzle[i, j] != 0;
                    cell.BackColor = Color.White;
                }
            }
        }

        private void Cell_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            Point pos = (Point)tb.Tag;

            if (string.IsNullOrEmpty(tb.Text))
            {
                tb.BackColor = Color.White;
                return;
            }

            if (!int.TryParse(tb.Text, out int num) || num < 1 || num > 9)
            {
                tb.Text = "";
                return;
            }

            tb.Text = num.ToString();

            if (num == solution[pos.X, pos.Y])
                tb.BackColor = Color.LightGreen;
            else
                tb.BackColor = Color.LightPink;
        }

        private void Cell_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
                e.Handled = true;
        }

        private void CheckSolution()
        {
            bool correct = true;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (cells[i, j].Text != solution[i, j].ToString())
                    {
                        correct = false;
                        cells[i, j].BackColor = Color.Salmon;
                    }
                }
            }

            if (correct)
            {
                timer.Stop();
                MessageBox.Show($"Вітаю! Ви розв’язали судоку за {timerLabel.Text}!", "Перемога!",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Є помилки! Червоний = неправильно", "Перевірка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void GiveHint()
        {
            var emptyCells = cells.Cast<TextBox>()
                .Where(c => string.IsNullOrEmpty(c.Text))
                .OrderBy(x => Guid.NewGuid())
                .FirstOrDefault();

            if (emptyCells != null)
            {
                Point p = (Point)emptyCells.Tag;
                emptyCells.Text = solution[p.X, p.Y].ToString();
                emptyCells.ForeColor = Color.Green;
                emptyCells.ReadOnly = true;
            }
            else
            {
                MessageBox.Show("Всі клітинки заповнені!");
            }
        }
    }
}
