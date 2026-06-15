using System;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace Laboratorna17
{
    public partial class Form1 : Form
    {
        private TextBox txtComplexA;
        private TextBox txtComplexB;
        private TextBox txtFuzzyA;
        private TextBox txtFuzzyB;
        private RichTextBox output;

        public Form1()
        {
            InitializeComponent();
            BuildInterface();
        }

        private void BuildInterface()
        {
            Text = "Laboratorna17 - Варіант 6";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(980, 680);
            MinimumSize = new Size(920, 620);
            BackColor = Color.White;
            Font = new Font("Segoe UI", 10);

            Label title = new Label();
            title.Text = "Laboratorna17 - Спадкування та поліморфізм";
            title.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            title.Location = new Point(25, 15);
            title.Size = new Size(850, 35);
            Controls.Add(title);

            Label task = new Label();
            task.Text = "Варіант 6: Pair, Fazzynumber, Complex. Арифметичні операції над числами.";
            task.Location = new Point(27, 55);
            task.Size = new Size(850, 30);
            Controls.Add(task);

            GroupBox complexGroup = new GroupBox();
            complexGroup.Text = "Комплексні числа";
            complexGroup.Location = new Point(30, 100);
            complexGroup.Size = new Size(420, 190);
            Controls.Add(complexGroup);

            complexGroup.Controls.Add(CreateLabel("Перше число:", 20, 40, 120));
            txtComplexA = CreateBox(145, 38, "3;4");
            complexGroup.Controls.Add(txtComplexA);

            complexGroup.Controls.Add(CreateLabel("Друге число:", 20, 85, 120));
            txtComplexB = CreateBox(145, 83, "2;5");
            complexGroup.Controls.Add(txtComplexB);

            complexGroup.Controls.Add(CreateSmallLabel("Формат: дійсна;уявна", 145, 120, 230));

            GroupBox fuzzyGroup = new GroupBox();
            fuzzyGroup.Text = "Нечіткі числа";
            fuzzyGroup.Location = new Point(500, 100);
            fuzzyGroup.Size = new Size(420, 190);
            Controls.Add(fuzzyGroup);

            fuzzyGroup.Controls.Add(CreateLabel("Перше число:", 20, 40, 120));
            txtFuzzyA = CreateBox(145, 38, "4;1");
            fuzzyGroup.Controls.Add(txtFuzzyA);

            fuzzyGroup.Controls.Add(CreateLabel("Друге число:", 20, 85, 120));
            txtFuzzyB = CreateBox(145, 83, "2;0,5");
            fuzzyGroup.Controls.Add(txtFuzzyB);

            fuzzyGroup.Controls.Add(CreateSmallLabel("Формат: значення;похибка", 145, 120, 230));

            Controls.Add(CreateButton("Додавання", 30, 315, AddValues));
            Controls.Add(CreateButton("Віднімання", 210, 315, SubtractValues));
            Controls.Add(CreateButton("Множення", 390, 315, MultiplyValues));
            Controls.Add(CreateButton("Ділення", 570, 315, DivideValues));
            Controls.Add(CreateButton("Показати всі операції", 750, 315, ShowAllOperations));

            output = new RichTextBox();
            output.Location = new Point(30, 380);
            output.Size = new Size(890, 230);
            output.ReadOnly = true;
            output.Font = new Font("Consolas", 10);
            output.BackColor = Color.FromArgb(248, 248, 248);
            Controls.Add(output);

            ShowAllOperations(null, EventArgs.Empty);
        }

        private Label CreateLabel(string text, int x, int y, int w)
        {
            Label label = new Label();
            label.Text = text;
            label.Location = new Point(x, y);
            label.Size = new Size(w, 30);
            label.TextAlign = ContentAlignment.MiddleLeft;
            return label;
        }

        private Label CreateSmallLabel(string text, int x, int y, int w)
        {
            Label label = new Label();
            label.Text = text;
            label.Location = new Point(x, y);
            label.Size = new Size(w, 25);
            label.ForeColor = Color.DimGray;
            return label;
        }

        private TextBox CreateBox(int x, int y, string text)
        {
            TextBox box = new TextBox();
            box.Location = new Point(x, y);
            box.Size = new Size(210, 30);
            box.Text = text;
            return box;
        }

        private Button CreateButton(string text, int x, int y, EventHandler click)
        {
            Button button = new Button();
            button.Text = text;
            button.Location = new Point(x, y);
            button.Size = new Size(155, 40);
            button.BackColor = Color.FromArgb(235, 242, 255);
            button.FlatStyle = FlatStyle.Flat;
            button.Click += click;
            return button;
        }

        private bool TryReadPair(TextBox box, string name, out double a, out double b)
        {
            a = 0;
            b = 0;

            string[] parts = box.Text.Split(';');

            if (parts.Length != 2)
            {
                MessageBox.Show(name + " потрібно вводити у форматі: число;число");
                box.Focus();
                return false;
            }

            if (!double.TryParse(parts[0].Trim().Replace('.', ','), NumberStyles.Any, CultureInfo.CurrentCulture, out a))
            {
                MessageBox.Show("Перша частина поля \"" + name + "\" введена неправильно.");
                box.Focus();
                return false;
            }

            if (!double.TryParse(parts[1].Trim().Replace('.', ','), NumberStyles.Any, CultureInfo.CurrentCulture, out b))
            {
                MessageBox.Show("Друга частина поля \"" + name + "\" введена неправильно.");
                box.Focus();
                return false;
            }

            return true;
        }

        private bool ReadObjects(out Pair complex1, out Pair complex2, out Pair fuzzy1, out Pair fuzzy2)
        {
            complex1 = null;
            complex2 = null;
            fuzzy1 = null;
            fuzzy2 = null;

            if (!TryReadPair(txtComplexA, "перше комплексне число", out double r1, out double i1))
                return false;

            if (!TryReadPair(txtComplexB, "друге комплексне число", out double r2, out double i2))
                return false;

            if (!TryReadPair(txtFuzzyA, "перше нечітке число", out double v1, out double e1))
                return false;

            if (!TryReadPair(txtFuzzyB, "друге нечітке число", out double v2, out double e2))
                return false;

            if (e1 < 0 || e2 < 0)
            {
                MessageBox.Show("Похибка нечіткого числа не може бути від'ємною.");
                return false;
            }

            complex1 = new ComplexNumber(r1, i1);
            complex2 = new ComplexNumber(r2, i2);
            fuzzy1 = new FuzzyNumber(v1, e1);
            fuzzy2 = new FuzzyNumber(v2, e2);

            return true;
        }

        private void AddValues(object sender, EventArgs e)
        {
            if (!ReadObjects(out Pair c1, out Pair c2, out Pair f1, out Pair f2))
                return;

            output.Text =
                "Додавання\n\n" +
                "Complex:\n" + c1 + " + " + c2 + " = " + c1.Add(c2) + "\n\n" +
                "Fazzynumber:\n" + f1 + " + " + f2 + " = " + f1.Add(f2);
        }

        private void SubtractValues(object sender, EventArgs e)
        {
            if (!ReadObjects(out Pair c1, out Pair c2, out Pair f1, out Pair f2))
                return;

            output.Text =
                "Віднімання\n\n" +
                "Complex:\n" + c1 + " - " + c2 + " = " + c1.Subtract(c2) + "\n\n" +
                "Fazzynumber:\n" + f1 + " - " + f2 + " = " + f1.Subtract(f2);
        }

        private void MultiplyValues(object sender, EventArgs e)
        {
            if (!ReadObjects(out Pair c1, out Pair c2, out Pair f1, out Pair f2))
                return;

            output.Text =
                "Множення\n\n" +
                "Complex:\n" + c1 + " * " + c2 + " = " + c1.Multiply(c2) + "\n\n" +
                "Fazzynumber:\n" + f1 + " * " + f2 + " = " + f1.Multiply(f2);
        }

        private void DivideValues(object sender, EventArgs e)
        {
            if (!ReadObjects(out Pair c1, out Pair c2, out Pair f1, out Pair f2))
                return;

            try
            {
                output.Text =
                    "Ділення\n\n" +
                    "Complex:\n" + c1 + " / " + c2 + " = " + c1.Divide(c2) + "\n\n" +
                    "Fazzynumber:\n" + f1 + " / " + f2 + " = " + f1.Divide(f2);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ShowAllOperations(object sender, EventArgs e)
        {
            if (!ReadObjects(out Pair c1, out Pair c2, out Pair f1, out Pair f2))
                return;

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Комплексні числа:");
            sb.AppendLine(c1 + " + " + c2 + " = " + c1.Add(c2));
            sb.AppendLine(c1 + " - " + c2 + " = " + c1.Subtract(c2));
            sb.AppendLine(c1 + " * " + c2 + " = " + c1.Multiply(c2));

            try
            {
                sb.AppendLine(c1 + " / " + c2 + " = " + c1.Divide(c2));
            }
            catch (Exception ex)
            {
                sb.AppendLine("Ділення Complex: " + ex.Message);
            }

            sb.AppendLine();
            sb.AppendLine("Нечіткі числа:");
            sb.AppendLine(f1 + " + " + f2 + " = " + f1.Add(f2));
            sb.AppendLine(f1 + " - " + f2 + " = " + f1.Subtract(f2));
            sb.AppendLine(f1 + " * " + f2 + " = " + f1.Multiply(f2));

            try
            {
                sb.AppendLine(f1 + " / " + f2 + " = " + f1.Divide(f2));
            }
            catch (Exception ex)
            {
                sb.AppendLine("Ділення Fazzynumber: " + ex.Message);
            }

            output.Text = sb.ToString();
        }
    }

    public abstract class Pair
    {
        public abstract Pair Add(Pair other);
        public abstract Pair Subtract(Pair other);
        public abstract Pair Multiply(Pair other);
        public abstract Pair Divide(Pair other);
        public abstract override string ToString();
    }

    public class ComplexNumber : Pair
    {
        public double Real { get; set; }
        public double Imaginary { get; set; }

        public ComplexNumber()
        {
            Real = 0;
            Imaginary = 0;
        }

        public ComplexNumber(double real, double imaginary)
        {
            Real = real;
            Imaginary = imaginary;
        }

        public override Pair Add(Pair other)
        {
            ComplexNumber o = other as ComplexNumber;

            if (o == null)
                throw new ArgumentException("Для операції потрібне комплексне число.");

            return new ComplexNumber(Real + o.Real, Imaginary + o.Imaginary);
        }

        public override Pair Subtract(Pair other)
        {
            ComplexNumber o = other as ComplexNumber;

            if (o == null)
                throw new ArgumentException("Для операції потрібне комплексне число.");

            return new ComplexNumber(Real - o.Real, Imaginary - o.Imaginary);
        }

        public override Pair Multiply(Pair other)
        {
            ComplexNumber o = other as ComplexNumber;

            if (o == null)
                throw new ArgumentException("Для операції потрібне комплексне число.");

            double real = Real * o.Real - Imaginary * o.Imaginary;
            double imaginary = Real * o.Imaginary + Imaginary * o.Real;

            return new ComplexNumber(real, imaginary);
        }

        public override Pair Divide(Pair other)
        {
            ComplexNumber o = other as ComplexNumber;

            if (o == null)
                throw new ArgumentException("Для операції потрібне комплексне число.");

            double denominator = o.Real * o.Real + o.Imaginary * o.Imaginary;

            if (denominator == 0)
                throw new DivideByZeroException("Ділення на нульове комплексне число неможливе.");

            double real = (Real * o.Real + Imaginary * o.Imaginary) / denominator;
            double imaginary = (Imaginary * o.Real - Real * o.Imaginary) / denominator;

            return new ComplexNumber(real, imaginary);
        }

        public override string ToString()
        {
            string sign = Imaginary >= 0 ? "+" : "-";
            return Real.ToString("0.###") + " " + sign + " " + Math.Abs(Imaginary).ToString("0.###") + "i";
        }
    }

    public class FuzzyNumber : Pair
    {
        public double Value { get; set; }
        public double Error { get; set; }

        public FuzzyNumber()
        {
            Value = 0;
            Error = 0;
        }

        public FuzzyNumber(double value, double error)
        {
            Value = value;
            Error = Math.Abs(error);
        }

        public override Pair Add(Pair other)
        {
            FuzzyNumber o = other as FuzzyNumber;

            if (o == null)
                throw new ArgumentException("Для операції потрібне нечітке число.");

            return new FuzzyNumber(Value + o.Value, Error + o.Error);
        }

        public override Pair Subtract(Pair other)
        {
            FuzzyNumber o = other as FuzzyNumber;

            if (o == null)
                throw new ArgumentException("Для операції потрібне нечітке число.");

            return new FuzzyNumber(Value - o.Value, Error + o.Error);
        }

        public override Pair Multiply(Pair other)
        {
            FuzzyNumber o = other as FuzzyNumber;

            if (o == null)
                throw new ArgumentException("Для операції потрібне нечітке число.");

            double value = Value * o.Value;
            double error = Math.Abs(Value) * o.Error + Math.Abs(o.Value) * Error + Error * o.Error;

            return new FuzzyNumber(value, error);
        }

        public override Pair Divide(Pair other)
        {
            FuzzyNumber o = other as FuzzyNumber;

            if (o == null)
                throw new ArgumentException("Для операції потрібне нечітке число.");

            if (Math.Abs(o.Value) <= o.Error)
                throw new DivideByZeroException("Ділення неможливе: інтервал другого нечіткого числа містить нуль.");

            double value = Value / o.Value;
            double error = (Math.Abs(Value) * o.Error + Math.Abs(o.Value) * Error) / (Math.Abs(o.Value) * Math.Abs(o.Value));

            return new FuzzyNumber(value, error);
        }

        public override string ToString()
        {
            double left = Value - Error;
            double right = Value + Error;

            return "<" +
                   left.ToString("0.###") +
                   "; " +
                   Value.ToString("0.###") +
                   "; " +
                   right.ToString("0.###") +
                   ">";
        }
    }
}