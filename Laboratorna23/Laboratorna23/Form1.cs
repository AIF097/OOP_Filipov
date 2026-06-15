using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;

namespace Laboratorna23
{
    public partial class Form1 : Form
    {
        private Panel drawPanel;

        private TextBox txtX0;
        private TextBox txtY0;
        private TextBox txtR;

        private Button btnDraw;
        private Button btnClear;
        private Button btnExample;

        private bool needDraw = false;

        public Form1()
        {
            InitializeComponent();
            BuildInterface();
        }

        private void BuildInterface()
        {
            Text = "Laboratorna23 - Варіант 1";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(1100, 760);
            MinimumSize = new Size(1000, 700);
            BackColor = Color.White;
            Font = new Font("Segoe UI", 10);

            Label title = new Label();
            title.Text = "Laboratorna23 - Побудова графіка";
            title.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            title.Location = new Point(20, 15);
            title.Size = new Size(500, 35);
            Controls.Add(title);

            Label formula = new Label();
            formula.Text = "x = x₀ + R cos(t)      y = y₀ + R sin(t)";
            formula.Font = new Font("Segoe UI", 12, FontStyle.Italic);
            formula.Location = new Point(22, 55);
            formula.Size = new Size(500, 30);
            Controls.Add(formula);

            GroupBox group = new GroupBox();
            group.Text = "Параметри";
            group.Location = new Point(20, 100);
            group.Size = new Size(260, 230);
            Controls.Add(group);

            Label lblX0 = new Label();
            lblX0.Text = "x₀:";
            lblX0.Location = new Point(20, 40);
            lblX0.Size = new Size(40, 30);
            group.Controls.Add(lblX0);

            txtX0 = new TextBox();
            txtX0.Location = new Point(70, 38);
            txtX0.Size = new Size(150, 30);
            txtX0.Text = "0";
            group.Controls.Add(txtX0);

            Label lblY0 = new Label();
            lblY0.Text = "y₀:";
            lblY0.Location = new Point(20, 85);
            lblY0.Size = new Size(40, 30);
            group.Controls.Add(lblY0);

            txtY0 = new TextBox();
            txtY0.Location = new Point(70, 83);
            txtY0.Size = new Size(150, 30);
            txtY0.Text = "0";
            group.Controls.Add(txtY0);

            Label lblR = new Label();
            lblR.Text = "R:";
            lblR.Location = new Point(20, 130);
            lblR.Size = new Size(40, 30);
            group.Controls.Add(lblR);

            txtR = new TextBox();
            txtR.Location = new Point(70, 128);
            txtR.Size = new Size(150, 30);
            txtR.Text = "5";
            group.Controls.Add(txtR);

            btnDraw = new Button();
            btnDraw.Text = "Побудувати";
            btnDraw.Location = new Point(20, 175);
            btnDraw.Size = new Size(110, 35);
            btnDraw.BackColor = Color.FromArgb(220, 235, 255);
            btnDraw.FlatStyle = FlatStyle.Flat;
            btnDraw.Click += DrawGraph;
            group.Controls.Add(btnDraw);

            btnClear = new Button();
            btnClear.Text = "Очистити";
            btnClear.Location = new Point(140, 175);
            btnClear.Size = new Size(100, 35);
            btnClear.BackColor = Color.FromArgb(240, 240, 240);
            btnClear.FlatStyle = FlatStyle.Flat;
            btnClear.Click += ClearGraph;
            group.Controls.Add(btnClear);

            btnExample = new Button();
            btnExample.Text = "Приклад";
            btnExample.Location = new Point(20, 370);
            btnExample.Size = new Size(260, 38);
            btnExample.BackColor = Color.FromArgb(235, 242, 255);
            btnExample.FlatStyle = FlatStyle.Flat;
            btnExample.Click += SetExample;
            Controls.Add(btnExample);

            drawPanel = new Panel();
            drawPanel.Location = new Point(310, 25);
            drawPanel.Size = new Size(750, 650);
            drawPanel.BackColor = Color.White;
            drawPanel.BorderStyle = BorderStyle.FixedSingle;
            drawPanel.Paint += DrawPanel_Paint;
            Controls.Add(drawPanel);
        }

        private void DrawGraph(object sender, EventArgs e)
        {
            if (!ReadValues(out double x0, out double y0, out double r))
                return;

            if (r <= 0)
            {
                MessageBox.Show("Радіус повинен бути більше нуля.");
                return;
            }

            needDraw = true;
            drawPanel.Tag = new double[] { x0, y0, r };
            drawPanel.Invalidate();
        }

        private void ClearGraph(object sender, EventArgs e)
        {
            needDraw = false;
            drawPanel.Invalidate();
        }

        private void SetExample(object sender, EventArgs e)
        {
            txtX0.Text = "0";
            txtY0.Text = "0";
            txtR.Text = "5";

            DrawGraph(null, EventArgs.Empty);
        }

        private bool ReadValues(out double x0, out double y0, out double r)
        {
            x0 = 0;
            y0 = 0;
            r = 0;

            try
            {
                x0 = double.Parse(txtX0.Text.Replace('.', ','), CultureInfo.CurrentCulture);
                y0 = double.Parse(txtY0.Text.Replace('.', ','), CultureInfo.CurrentCulture);
                r = double.Parse(txtR.Text.Replace('.', ','), CultureInfo.CurrentCulture);

                return true;
            }
            catch
            {
                MessageBox.Show("Неправильно введені дані.");
                return false;
            }
        }

        private void DrawPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.White);

            int width = drawPanel.Width;
            int height = drawPanel.Height;

            int centerX = width / 2;
            int centerY = height / 2;

            DrawGrid(g, width, height);
            DrawAxes(g, width, height, centerX, centerY);

            if (!needDraw || drawPanel.Tag == null)
                return;

            double[] values = (double[])drawPanel.Tag;

            double x0 = values[0];
            double y0 = values[1];
            double r = values[2];

            DrawCircle(g, centerX, centerY, x0, y0, r);
        }

        private void DrawGrid(Graphics g, int width, int height)
        {
            using (Pen gridPen = new Pen(Color.FromArgb(230, 230, 230)))
            {
                for (int x = 0; x < width; x += 25)
                    g.DrawLine(gridPen, x, 0, x, height);

                for (int y = 0; y < height; y += 25)
                    g.DrawLine(gridPen, 0, y, width, y);
            }
        }

        private void DrawAxes(Graphics g, int width, int height, int centerX, int centerY)
        {
            using (Pen axisPen = new Pen(Color.Black, 2))
            {
                g.DrawLine(axisPen, 0, centerY, width, centerY);
                g.DrawLine(axisPen, centerX, 0, centerX, height);
            }

            Font font = new Font("Segoe UI", 9);

            for (int x = centerX; x < width; x += 50)
            {
                g.DrawLine(Pens.Black, x, centerY - 4, x, centerY + 4);

                int value = (x - centerX) / 25;

                if (value != 0)
                    g.DrawString(value.ToString(), font, Brushes.Black, x - 5, centerY + 8);
            }

            for (int x = centerX; x > 0; x -= 50)
            {
                g.DrawLine(Pens.Black, x, centerY - 4, x, centerY + 4);

                int value = (x - centerX) / 25;

                if (value != 0)
                    g.DrawString(value.ToString(), font, Brushes.Black, x - 10, centerY + 8);
            }

            for (int y = centerY; y < height; y += 50)
            {
                g.DrawLine(Pens.Black, centerX - 4, y, centerX + 4, y);

                int value = -(y - centerY) / 25;

                if (value != 0)
                    g.DrawString(value.ToString(), font, Brushes.Black, centerX + 8, y - 8);
            }

            for (int y = centerY; y > 0; y -= 50)
            {
                g.DrawLine(Pens.Black, centerX - 4, y, centerX + 4, y);

                int value = -(y - centerY) / 25;

                if (value != 0)
                    g.DrawString(value.ToString(), font, Brushes.Black, centerX + 8, y - 8);
            }

            g.DrawString("X", new Font("Segoe UI", 11, FontStyle.Bold), Brushes.Black, width - 30, centerY + 10);
            g.DrawString("Y", new Font("Segoe UI", 11, FontStyle.Bold), Brushes.Black, centerX + 10, 10);
        }

        private void DrawCircle(Graphics g, int centerX, int centerY, double x0, double y0, double r)
        {
            PointF[] points = new PointF[1000];

            double scale = 25;

            for (int i = 0; i < points.Length; i++)
            {
                double t = 2 * Math.PI * i / (points.Length - 1);

                double x = x0 + r * Math.Cos(t);
                double y = y0 + r * Math.Sin(t);

                float screenX = (float)(centerX + x * scale);
                float screenY = (float)(centerY - y * scale);

                points[i] = new PointF(screenX, screenY);
            }

            using (Pen graphPen = new Pen(Color.Red, 3))
            {
                g.DrawLines(graphPen, points);
            }
        }
    }
}