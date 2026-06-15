using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Laboratorna18
{
    public partial class Form1 : Form
    {
        private TextBox txtArray;
        private TextBox txtMatrix;
        private TextBox txtRowM;
        private RichTextBox output;

        public Form1()
        {
            InitializeComponent();
            BuildInterface();
        }

        private void BuildInterface()
        {
            Text = "Laboratorna18 - Варіант 6";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(980, 700);
            MinimumSize = new Size(920, 640);
            BackColor = Color.White;
            Font = new Font("Segoe UI", 10);

            Label title = new Label();
            title.Text = "Laboratorna18 - Масиви в C# - Варіант 6";
            title.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            title.Location = new Point(25, 15);
            title.Size = new Size(850, 35);
            Controls.Add(title);

            Label lblArray = new Label();
            lblArray.Text = "Одномірний масив дійсних чисел:";
            lblArray.Location = new Point(30, 75);
            lblArray.Size = new Size(350, 28);
            Controls.Add(lblArray);

            txtArray = new TextBox();
            txtArray.Location = new Point(30, 105);
            txtArray.Size = new Size(880, 30);
            txtArray.Text = "-3,5; 2; -1; 4; 0,5; -6; 8; 1";
            Controls.Add(txtArray);

            Label lblArrayHelp = new Label();
            lblArrayHelp.Text = "Вводь числа через кому, крапку з комою або пробіл";
            lblArrayHelp.Location = new Point(30, 138);
            lblArrayHelp.Size = new Size(500, 25);
            lblArrayHelp.ForeColor = Color.DimGray;
            Controls.Add(lblArrayHelp);

            Label lblMatrix = new Label();
            lblMatrix.Text = "Двовимірний масив:";
            lblMatrix.Location = new Point(30, 180);
            lblMatrix.Size = new Size(250, 28);
            Controls.Add(lblMatrix);

            txtMatrix = new TextBox();
            txtMatrix.Location = new Point(30, 210);
            txtMatrix.Size = new Size(880, 100);
            txtMatrix.Multiline = true;
            txtMatrix.ScrollBars = ScrollBars.Vertical;
            txtMatrix.Text = "1 2 3 4\r\n5 6 7 8\r\n9 10 11 12\r\n13 14 15 16";
            Controls.Add(txtMatrix);

            Label lblMatrixHelp = new Label();
            lblMatrixHelp.Text = "Кожен рядок матриці вводь з нового рядка, числа через пробіл або ;";
            lblMatrixHelp.Location = new Point(30, 315);
            lblMatrixHelp.Size = new Size(650, 25);
            lblMatrixHelp.ForeColor = Color.DimGray;
            Controls.Add(lblMatrixHelp);

            Label lblM = new Label();
            lblM.Text = "m-й рядок:";
            lblM.Location = new Point(30, 355);
            lblM.Size = new Size(100, 30);
            Controls.Add(lblM);

            txtRowM = new TextBox();
            txtRowM.Location = new Point(130, 352);
            txtRowM.Size = new Size(80, 30);
            txtRowM.Text = "2";
            txtRowM.KeyPress += OnlyDigits;
            Controls.Add(txtRowM);

            Controls.Add(CreateButton("Завдання 1", 240, 350, SolveOneDimensional));
            Controls.Add(CreateButton("Завдання 2", 420, 350, SolveTwoDimensional));
            Controls.Add(CreateButton("Виконати все", 600, 350, SolveAll));

            output = new RichTextBox();
            output.Location = new Point(30, 410);
            output.Size = new Size(880, 220);
            output.ReadOnly = true;
            output.Font = new Font("Consolas", 10);
            output.BackColor = Color.FromArgb(248, 248, 248);
            Controls.Add(output);
        }

        private Button CreateButton(string text, int x, int y, EventHandler click)
        {
            Button button = new Button();
            button.Text = text;
            button.Location = new Point(x, y);
            button.Size = new Size(150, 38);
            button.BackColor = Color.FromArgb(235, 242, 255);
            button.FlatStyle = FlatStyle.Flat;
            button.Click += click;
            return button;
        }

        private void OnlyDigits(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
                return;

            e.Handled = true;
        }

        private bool ReadArray(out double[] array)
        {
            array = null;

            try
            {
                array = txtArray.Text
                    .Split(new[] { ' ', ';', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => double.Parse(x.Trim().Replace('.', ','), CultureInfo.CurrentCulture))
                    .ToArray();

                if (array.Length == 0)
                {
                    MessageBox.Show("Введіть хоча б один елемент масиву.");
                    return false;
                }

                return true;
            }
            catch
            {
                MessageBox.Show("Одномірний масив введено неправильно.");
                txtArray.Focus();
                return false;
            }
        }

        private bool ReadMatrix(out double[,] matrix)
        {
            matrix = null;

            try
            {
                string[] rows = txtMatrix.Text
                    .Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                if (rows.Length == 0)
                {
                    MessageBox.Show("Введіть матрицю.");
                    return false;
                }

                string[][] parts = rows
                    .Select(r => r.Split(new[] { ' ', ';', '\t' }, StringSplitOptions.RemoveEmptyEntries))
                    .ToArray();

                int columns = parts[0].Length;

                if (columns == 0)
                {
                    MessageBox.Show("У матриці немає чисел.");
                    return false;
                }

                for (int i = 0; i < parts.Length; i++)
                {
                    if (parts[i].Length != columns)
                    {
                        MessageBox.Show("Усі рядки матриці повинні мати однакову кількість елементів.");
                        return false;
                    }
                }

                matrix = new double[parts.Length, columns];

                for (int i = 0; i < parts.Length; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        matrix[i, j] = double.Parse(parts[i][j].Trim().Replace('.', ','), CultureInfo.CurrentCulture);
                    }
                }

                return true;
            }
            catch
            {
                MessageBox.Show("Двовимірний масив введено неправильно.");
                txtMatrix.Focus();
                return false;
            }
        }

        private void SolveOneDimensional(object sender, EventArgs e)
        {
            if (!ReadArray(out double[] array))
                return;

            output.Text = GetOneDimensionalResult(array);
        }

        private void SolveTwoDimensional(object sender, EventArgs e)
        {
            if (!ReadMatrix(out double[,] matrix))
                return;

            if (!int.TryParse(txtRowM.Text, out int m))
            {
                MessageBox.Show("Введіть номер рядка m.");
                return;
            }

            output.Text = GetTwoDimensionalResult(matrix, m);
        }

        private void SolveAll(object sender, EventArgs e)
        {
            if (!ReadArray(out double[] array))
                return;

            if (!ReadMatrix(out double[,] matrix))
                return;

            if (!int.TryParse(txtRowM.Text, out int m))
            {
                MessageBox.Show("Введіть номер рядка m.");
                return;
            }

            output.Text =
                GetOneDimensionalResult(array) +
                "\n\n----------------------------------------\n\n" +
                GetTwoDimensionalResult(matrix, m);
        }

        private string GetOneDimensionalResult(double[] source)
        {
            double[] array = (double[])source.Clone();

            int negativeCount = array.Count(x => x < 0);

            int minAbsIndex = 0;
            for (int i = 1; i < array.Length; i++)
            {
                if (Math.Abs(array[i]) < Math.Abs(array[minAbsIndex]))
                    minAbsIndex = i;
            }

            double sumAbsAfterMinAbs = 0;
            for (int i = minAbsIndex + 1; i < array.Length; i++)
                sumAbsAfterMinAbs += Math.Abs(array[i]);

            double[] changedArray = (double[])array.Clone();

            for (int i = 0; i < changedArray.Length; i++)
            {
                if (changedArray[i] < 0)
                    changedArray[i] = changedArray[i] * changedArray[i];
            }

            Array.Sort(changedArray);

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Завдання 1. Одномірний масив");
            sb.AppendLine();
            sb.AppendLine("Початковий масив:");
            sb.AppendLine(ArrayToString(array));
            sb.AppendLine();
            sb.AppendLine("Кількість від'ємних елементів: " + negativeCount);
            sb.AppendLine("Мінімальний за модулем елемент: " + array[minAbsIndex].ToString("0.###"));
            sb.AppendLine("Індекс мінімального за модулем елемента: " + minAbsIndex);
            sb.AppendLine("Сума модулів елементів після нього: " + sumAbsAfterMinAbs.ToString("0.###"));
            sb.AppendLine();
            sb.AppendLine("Після заміни від'ємних елементів квадратами і сортування:");
            sb.AppendLine(ArrayToString(changedArray));

            return sb.ToString();
        }

        private string GetTwoDimensionalResult(double[,] matrix, int m)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            if (cols < 2)
                return "Завдання 2\n\nУ матриці має бути мінімум 2 стовпці.";

            if (m < 1 || m > rows)
                return "Завдання 2\n\nНомер рядка m має бути від 1 до " + rows + ".";

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Завдання 2. Двовимірний масив");
            sb.AppendLine();
            sb.AppendLine("Увесь масив:");
            sb.AppendLine(MatrixToString(matrix));
            sb.AppendLine();
            sb.AppendLine("Елементи другого стовпця:");
            for (int i = 0; i < rows; i++)
                sb.Append(matrix[i, 1].ToString("0.###") + " ");
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("Елементи " + m + "-го рядка:");
            for (int j = 0; j < cols; j++)
                sb.Append(matrix[m - 1, j].ToString("0.###") + " ");

            return sb.ToString();
        }

        private string ArrayToString(double[] array)
        {
            return string.Join("   ", array.Select(x => x.ToString("0.###")));
        }

        private string MatrixToString(double[,] matrix)
        {
            StringBuilder sb = new StringBuilder();

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                    sb.Append(matrix[i, j].ToString("0.###").PadLeft(8));

                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}