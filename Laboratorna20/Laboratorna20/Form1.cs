using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Laboratorna20
{
    public partial class Form1 : Form
    {
        private TextBox txtFirst;
        private TextBox txtRatio;
        private TextBox txtCount;

        private RichTextBox output;

        public Form1()
        {
            InitializeComponent();
            BuildInterface();
        }

        private void BuildInterface()
        {
            Text = "Laboratorna20 - Варіант 6";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(900, 620);
            MinimumSize = new Size(840, 560);
            BackColor = Color.White;
            Font = new Font("Segoe UI", 10);

            Label title = new Label();
            title.Text = "Laboratorna20 - Обробка виключень";
            title.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            title.Location = new Point(25, 15);
            title.Size = new Size(700, 35);
            Controls.Add(title);

            Label task = new Label();
            task.Text = "Варіант 6: обчислення суми геометричної прогресії";
            task.Location = new Point(27, 55);
            task.Size = new Size(700, 28);
            Controls.Add(task);

            int labelX = 40;
            int boxX = 250;
            int top = 110;
            int step = 55;

            Controls.Add(CreateLabel("Перший член прогресії:", labelX, top));
            txtFirst = CreateBox(boxX, top, "2");

            Controls.Add(CreateLabel("Знаменник прогресії:", labelX, top + step));
            txtRatio = CreateBox(boxX, top + step, "3");

            Controls.Add(CreateLabel("Кількість елементів:", labelX, top + step * 2));
            txtCount = CreateBox(boxX, top + step * 2, "5");

            Controls.Add(txtFirst);
            Controls.Add(txtRatio);
            Controls.Add(txtCount);

            Controls.Add(CreateButton("Обчислити", 40, 300, CalculateSum));
            Controls.Add(CreateButton("Очистити", 240, 300, ClearFields));
            Controls.Add(CreateButton("Приклад", 440, 300, SetExample));

            output = new RichTextBox();
            output.Location = new Point(40, 370);
            output.Size = new Size(780, 170);
            output.ReadOnly = true;
            output.Font = new Font("Consolas", 10);
            output.BackColor = Color.FromArgb(248, 248, 248);
            Controls.Add(output);

            CalculateSum(null, EventArgs.Empty);
        }

        private Label CreateLabel(string text, int x, int y)
        {
            Label label = new Label();
            label.Text = text;
            label.Location = new Point(x, y + 3);
            label.Size = new Size(200, 30);
            return label;
        }

        private TextBox CreateBox(int x, int y, string text)
        {
            TextBox box = new TextBox();
            box.Location = new Point(x, y);
            box.Size = new Size(220, 30);
            box.Text = text;
            return box;
        }

        private Button CreateButton(string text, int x, int y, EventHandler click)
        {
            Button button = new Button();
            button.Text = text;
            button.Location = new Point(x, y);
            button.Size = new Size(170, 40);
            button.BackColor = Color.FromArgb(235, 242, 255);
            button.FlatStyle = FlatStyle.Flat;
            button.Click += click;
            return button;
        }

        private void CalculateSum(object sender, EventArgs e)
        {
            output.Clear();

            try
            {
                double first = ParseDouble(txtFirst.Text, "перший член");
                double ratio = ParseDouble(txtRatio.Text, "знаменник");
                int count = ParseInt(txtCount.Text, "кількість елементів");

                double sum = CalculateGeometricProgression(first, ratio, count);

                StringBuilder sb = new StringBuilder();

                sb.AppendLine("Laboratorna20 - Варіант 6");
                sb.AppendLine();
                sb.AppendLine("Перший член: " + first);
                sb.AppendLine("Знаменник: " + ratio);
                sb.AppendLine("Кількість елементів: " + count);
                sb.AppendLine();

                sb.AppendLine("Сума геометричної прогресії:");
                sb.AppendLine(sum.ToString("0.######"));

                output.Text = sb.ToString();
            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message, "Помилка формату");
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Помилка аргументу");
            }
            catch (OverflowException)
            {
                MessageBox.Show("Число занадто велике.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Невідома помилка");
            }
            finally
            {
                output.AppendText("\n\nБлок finally виконано.");
            }
        }

        private double ParseDouble(string text, string fieldName)
        {
            if (!double.TryParse(text.Replace('.', ','), out double value))
                throw new FormatException("Поле \"" + fieldName + "\" заповнене неправильно.");

            return value;
        }

        private int ParseInt(string text, string fieldName)
        {
            if (!int.TryParse(text, out int value))
                throw new FormatException("Поле \"" + fieldName + "\" повинно містити ціле число.");

            return value;
        }

        private double CalculateGeometricProgression(double first, double ratio, int count)
        {
            if (count <= 0)
                throw new ArgumentException("Кількість елементів повинна бути більше нуля.");

            if (Math.Abs(ratio - 1) < 0.0000001)
                return first * count;

            double denominator = ratio - 1;

            if (Math.Abs(denominator) < 0.0000001)
                throw new DivideByZeroException("Знаменник формули дорівнює нулю.");

            double numerator = Math.Pow(ratio, count) - 1;

            return first * (numerator / denominator);
        }

        private void ClearFields(object sender, EventArgs e)
        {
            txtFirst.Clear();
            txtRatio.Clear();
            txtCount.Clear();
            output.Clear();

            txtFirst.Focus();
        }

        private void SetExample(object sender, EventArgs e)
        {
            txtFirst.Text = "2";
            txtRatio.Text = "3";
            txtCount.Text = "5";

            CalculateSum(null, EventArgs.Empty);
        }
    }
}