using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KochCurve
{
    public partial class Form1 : Form
    {
        private PictureBox pictureBox;
        private NumericUpDown nudIterations;
        private Button btnDraw;
        private Label lblIterations;
        public Form1()
        {
            Text = "Крива Коха";
            Width = 900;
            Height = 600;
            StartPosition = FormStartPosition.CenterScreen;

            pictureBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            nudIterations = new NumericUpDown
            {
                Minimum = 0,
                Maximum = 8,
                Value = 4,
                Width = 60,
                Location = new Point(120, 10)
            };

            lblIterations = new Label
            {
                Text = "Кількість ітерацій:",
                AutoSize = true,
                Location = new Point(10, 12)
            };

            btnDraw = new Button
            {
                Text = "Намалювати",
                AutoSize = true,
                Location = new Point(200, 8)
            };
            btnDraw.Click += BtnDraw_Click;

            var topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40
            };

            topPanel.Controls.Add(lblIterations);
            topPanel.Controls.Add(nudIterations);
            topPanel.Controls.Add(btnDraw);

            Controls.Add(pictureBox);
            Controls.Add(topPanel);
        }

        private void BtnDraw_Click(object sender, EventArgs e)
        {
            int iterations = (int)nudIterations.Value;

            if (pictureBox.Width <= 0 || pictureBox.Height <= 0)
                return;

            var bmp = new Bitmap(pictureBox.Width, pictureBox.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                float margin = 40f;
                PointF a = new PointF(margin, pictureBox.Height / 2f);
                PointF b = new PointF(pictureBox.Width - margin, pictureBox.Height / 2f);

                using (Pen pen = new Pen(Color.Black, 1))
                {
                    DrawKoch(g, pen, a, b, iterations);
                }
            }

            pictureBox.Image = bmp;
        }

        private void DrawKoch(Graphics g, Pen pen, PointF a, PointF b, int depth)
        {
            if (depth == 0)
            {
                g.DrawLine(pen, a, b);
                return;
            }

            float dx = b.X - a.X;
            float dy = b.Y - a.Y;

            PointF p1 = a;
            PointF p2 = new PointF(a.X + dx / 3f, a.Y + dy / 3f);
            PointF p4 = new PointF(a.X + 2f * dx / 3f, a.Y + 2f * dy / 3f);
            PointF p5 = b;

            float vx = p4.X - p2.X;
            float vy = p4.Y - p2.Y;

            double angle = Math.PI / 3.0; // 60°
            float rx = (float)(vx * Math.Cos(angle) - vy * Math.Sin(angle));
            float ry = (float)(vx * Math.Sin(angle) + vy * Math.Cos(angle));

            PointF p3 = new PointF(p2.X + rx, p2.Y + ry);

            DrawKoch(g, pen, p1, p2, depth - 1);
            DrawKoch(g, pen, p2, p3, depth - 1);
            DrawKoch(g, pen, p3, p4, depth - 1);
            DrawKoch(g, pen, p4, p5, depth - 1);
        }
    }
}
