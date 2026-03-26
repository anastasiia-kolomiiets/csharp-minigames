using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Saper
{
    public partial class Form1 : Form
    {
        private const int CELL_SIZE = 30;
        private const int BOMBS = 40;
        private const int ROWS = 16;
        private const int COLS = 16;

        private Button[,] buttons;
        private int[,] field;        // -1 = бомба, 0-8 = кількість сусідніх бомб
        private bool[,] revealed;
        private bool[,] flagged;
        private bool firstClick = true;
        private int flagsLeft;

        private Label lblFlags;
        private Button btnReset;

        public Form1()
        {
            Text = "Сапер";
            ClientSize = new Size(COLS * CELL_SIZE + 20, ROWS * CELL_SIZE + 80);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;

            InitializeField();
            CreateControls();
        }

        private void InitializeField()
        {
            buttons = new Button[ROWS, COLS];
            field = new int[ROWS, COLS];
            revealed = new bool[ROWS, COLS];
            flagged = new bool[ROWS, COLS];
            flagsLeft = BOMBS;
        }

        private void CreateControls()
        {
            // Панель зверху
            Panel topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.LightGray
            };

            lblFlags = new Label
            {
                Text = $"💣 {BOMBS}",
                Font = new Font("Segoe UI Emoji", 14),
                AutoSize = true,
                Location = new Point(10, 12)
            };

            btnReset = new Button
            {
                Text = "🙂",
                Font = new Font("Segoe UI Emoji", 16),
                Size = new Size(40, 40),
                Location = new Point((COLS * CELL_SIZE - 40) / 2, 5)
            };
            btnReset.Click += (s, e) => ResetGame();

            topPanel.Controls.Add(lblFlags);
            topPanel.Controls.Add(btnReset);
            Controls.Add(topPanel);

            // Сітка кнопок
            for (int r = 0; r < ROWS; r++)
            {
                for (int c = 0; c < COLS; c++)
                {
                    Button btn = new Button
                    {
                        Size = new Size(CELL_SIZE, CELL_SIZE),
                        Location = new Point(c * CELL_SIZE + 10, r * CELL_SIZE + 60),
                        Tag = new Point(r, c),
                        FlatStyle = FlatStyle.Flat,
                        Font = new Font("Consolas", 10, FontStyle.Bold)
                    };

                    btn.MouseDown += Cell_MouseDown;
                    buttons[r, c] = btn;
                    Controls.Add(btn);
                }
            }
        }

        private void ResetGame()
        {
            firstClick = true;
            flagsLeft = BOMBS;
            lblFlags.Text = $"💣 {BOMBS}";

            for (int r = 0; r < ROWS; r++)
            {
                for (int c = 0; c < COLS; c++)
                {
                    field[r, c] = 0;
                    revealed[r, c] = false;
                    flagged[r, c] = false;
                    buttons[r, c].Text = "";
                    buttons[r, c].BackColor = SystemColors.Control;
                    buttons[r, c].ForeColor = Color.Black;
                    buttons[r, c].Enabled = true;
                }
            }
            btnReset.Text = "🙂";
        }

        private void Cell_MouseDown(object sender, MouseEventArgs e)
        {
            Button btn = (Button)sender;
            Point pos = (Point)btn.Tag;
            int r = pos.X, c = pos.Y;

            if (e.Button == MouseButtons.Left)
            {
                if (flagged[r, c]) return;

                if (firstClick)
                {
                    firstClick = false;
                    PlaceBombs(r, c);
                    CalculateNumbers();
                }

                RevealCell(r, c);

                if (field[r, c] == -1)
                {
                    GameOver(false);
                }
                else
                {
                    CheckWin();
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (revealed[r, c]) return;

                flagged[r, c] = !flagged[r, c];
                btn.Text = flagged[r, c] ? "🚩" : "";
                flagsLeft += flagged[r, c] ? -1 : 1;
                lblFlags.Text = $"💣 {flagsLeft}";
            }
        }

        private void PlaceBombs(int safeR, int safeC)
        {
            Random rnd = new Random();
            int placed = 0;

            while (placed < BOMBS)
            {
                int r = rnd.Next(ROWS);
                int c = rnd.Next(COLS);

                // не ставимо бомбу на першу клітинку та навколо неї
                if (field[r, c] != -1 &&
                    Math.Abs(r - safeR) + Math.Abs(c - safeC) > 1)
                {
                    field[r, c] = -1;
                    placed++;
                }
            }
        }

        private void CalculateNumbers()
        {
            for (int r = 0; r < ROWS; r++)
            {
                for (int c = 0; c < COLS; c++)
                {
                    if (field[r, c] == -1) continue;

                    int count = 0;
                    for (int dr = -1; dr <= 1; dr++)
                    {
                        for (int dc = -1; dc <= 1; dc++)
                        {
                            if (dr == 0 && dc == 0) continue;
                            int nr = r + dr, nc = c + dc;
                            if (nr >= 0 && nr < ROWS && nc >= 0 && nc < COLS && field[nr, nc] == -1)
                                count++;
                        }
                    }
                    field[r, c] = count;
                }
            }
        }

        private void RevealCell(int r, int c)
        {
            if (r < 0 || r >= ROWS || c < 0 || c >= COLS || revealed[r, c] || flagged[r, c]) return;

            revealed[r, c] = true;
            Button btn = buttons[r, c];
            btn.Enabled = false;

            if (field[r, c] == -1)
            {
                btn.Text = "💥";
                btn.BackColor = Color.Red;
                return;
            }

            if (field[r, c] > 0)
            {
                btn.Text = field[r, c].ToString();
                btn.BackColor = Color.LightGray;

                // кольори цифр як у класичному сапері
                switch (field[r, c])
                {
                    case 1: btn.ForeColor = Color.Blue; break;
                    case 2: btn.ForeColor = Color.Green; break;
                    case 3: btn.ForeColor = Color.Red; break;
                    case 4: btn.ForeColor = Color.DarkBlue; break;
                    case 5: btn.ForeColor = Color.DarkRed; break;
                    case 6: btn.ForeColor = Color.Teal; break;
                    case 7: btn.ForeColor = Color.Black; break;
                    case 8: btn.ForeColor = Color.Gray; break;
                }
            }
            else
            {
                btn.BackColor = Color.FromArgb(220, 220, 220);
            }

            // автовідкриття порожніх зон
            if (field[r, c] == 0)
            {
                for (int dr = -1; dr <= 1; dr++)
                {
                    for (int dc = -1; dc <= 1; dc++)
                    {
                        RevealCell(r + dr, c + dc);
                    }
                }
            }
        }

        private void GameOver(bool won)
        {
            btnReset.Text = won ? "😎" : "💀";

            for (int r = 0; r < ROWS; r++)
            {
                for (int c = 0; c < COLS; c++)
                {
                    buttons[r, c].Enabled = false;

                    if (field[r, c] == -1)
                    {
                        buttons[r, c].Text = flagged[r, c] ? "🚩" : "💣";
                        if (!flagged[r, c] && !won) buttons[r, c].BackColor = Color.OrangeRed;
                    }
                    else if (flagged[r, c] && field[r, c] != -1)
                    {
                        buttons[r, c].Text = "❌";
                    }
                }
            }

            string message = won ? "Вітаю! Ви виграли! 🎉" : "Бум! Ви підірвались... 💥";
            MessageBox.Show(message, "Кінець гри", MessageBoxButtons.OK,
                won ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
        }

        private void CheckWin()
        {
            int hidden = 0;
            foreach (bool b in revealed) if (!b) hidden++;
            if (hidden == BOMBS)
                GameOver(true);
        }
    }
}
