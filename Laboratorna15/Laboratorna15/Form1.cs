using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Laboratorna15
{
    public partial class Form1 : Form
    {
        private TabControl tabs;

        public Form1()
        {
            InitializeComponent();
            BuildInterface();
        }

        private void BuildInterface()
        {
            Text = "Laboratorna15 - Варіант 1";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(950, 650);
            MinimumSize = new Size(900, 600);
            Font = new Font("Segoe UI", 10);
            BackColor = Color.White;

            tabs = new TabControl();
            tabs.Dock = DockStyle.Fill;
            tabs.Font = new Font("Segoe UI", 10);

            tabs.TabPages.Add(CreateTask1());
            tabs.TabPages.Add(CreateTask2());
            tabs.TabPages.Add(CreateTask3());
            tabs.TabPages.Add(CreateTask4());
            tabs.TabPages.Add(CreateTask5());
            tabs.TabPages.Add(CreateTask6());
            tabs.TabPages.Add(CreateTask7());

            Controls.Add(tabs);
        }

        private Label Title(string text)
        {
            return new Label
            {
                Text = text,
                AutoSize = false,
                Height = 45,
                Dock = DockStyle.Top,
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(15, 0, 0, 0)
            };
        }

        private TextBox Box(int x, int y, int w = 120)
        {
            TextBox tb = new TextBox();
            tb.Location = new Point(x, y);
            tb.Width = w;
            tb.Height = 30;
            tb.KeyPress += NumberKeyPress;
            return tb;
        }

        private Button Btn(string text, int x, int y, EventHandler click)
        {
            Button b = new Button();
            b.Text = text;
            b.Location = new Point(x, y);
            b.Size = new Size(170, 38);
            b.BackColor = Color.FromArgb(235, 242, 255);
            b.FlatStyle = FlatStyle.Flat;
            b.Click += click;
            return b;
        }

        private Label Lbl(string text, int x, int y, int w = 120)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(w, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };
        }

        private TextBox ResultBox(int x, int y, int w = 520, int h = 190)
        {
            return new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(w, h),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Consolas", 10)
            };
        }

        private bool GetDouble(TextBox tb, string name, out double value)
        {
            value = 0;
            string s = tb.Text.Trim().Replace('.', ',');
            if (!double.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out value))
            {
                MessageBox.Show("Некоректне значення поля: " + name);
                tb.Focus();
                return false;
            }
            return true;
        }

        private bool GetInt(TextBox tb, string name, out int value)
        {
            value = 0;
            if (!int.TryParse(tb.Text.Trim(), out value))
            {
                MessageBox.Show("Некоректне ціле число в полі: " + name);
                tb.Focus();
                return false;
            }
            return true;
        }

        private void NumberKeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox tb = sender as TextBox;

            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
                return;

            if (e.KeyChar == '.' || e.KeyChar == ',')
            {
                if (tb != null && !tb.Text.Contains(",") && !tb.Text.Contains("."))
                {
                    e.KeyChar = ',';
                    return;
                }
            }

            if (e.KeyChar == '-' && tb != null && tb.SelectionStart == 0 && !tb.Text.Contains("-"))
                return;

            e.Handled = true;
        }

        private TabPage CreateTask1()
        {
            TabPage page = new TabPage("Завдання 1");
            page.BackColor = Color.White;

            page.Controls.Add(Title("1. Обчислення виразу: (b + √(b² + 4ac)) / (2a) - a³c + b⁻²"));

            Label formula = Lbl("Введіть a, b, c:", 25, 70, 250);
            TextBox a = Box(70, 115);
            TextBox b = Box(70, 160);
            TextBox c = Box(70, 205);
            TextBox result = ResultBox(25, 315, 520, 150);

            page.Controls.Add(formula);
            page.Controls.Add(Lbl("a:", 25, 115, 40));
            page.Controls.Add(Lbl("b:", 25, 160, 40));
            page.Controls.Add(Lbl("c:", 25, 205, 40));
            page.Controls.Add(a);
            page.Controls.Add(b);
            page.Controls.Add(c);

            PictureBox pic = new PictureBox();
            pic.Location = new Point(590, 85);
            pic.Size = new Size(300, 260);
            pic.BorderStyle = BorderStyle.FixedSingle;
            pic.SizeMode = PictureBoxSizeMode.Zoom;
            page.Controls.Add(pic);
            string photoPath = Path.Combine(Application.StartupPath, "Photo.png");

            if (File.Exists(photoPath))
            {
                pic.Image = Image.FromFile(photoPath);
            }

            page.Controls.Add(Btn("Обчислити", 25, 255, (s, e) =>
            {
                if (!GetDouble(a, "a", out double av)) return;
                if (!GetDouble(b, "b", out double bv)) return;
                if (!GetDouble(c, "c", out double cv)) return;

                if (av == 0)
                {
                    MessageBox.Show("a не може дорівнювати 0");
                    return;
                }

                if (bv == 0)
                {
                    MessageBox.Show("b не може дорівнювати 0, бо є b⁻²");
                    return;
                }

                double underRoot = bv * bv + 4 * av * cv;
                if (underRoot < 0)
                {
                    MessageBox.Show("Підкореневий вираз менше 0");
                    return;
                }

                double value = (bv + Math.Sqrt(underRoot)) / (2 * av) - Math.Pow(av, 3) * cv + Math.Pow(bv, -2);

                result.Text =
                    "Laboratorna15\r\n" +
                    "Варіант 1, завдання 1\r\n\r\n" +
                    $"a = {av}\r\nb = {bv}\r\nc = {cv}\r\n\r\n" +
                    $"Результат = {value:F6}";
            }));

            page.Controls.Add(result);
            return page;
        }

        private TabPage CreateTask2()
        {
            TabPage page = new TabPage("Завдання 2");
            page.BackColor = Color.White;
            page.Controls.Add(Title("2. Час на електронному годиннику через p год q хв r с"));

            TextBox t = Box(90, 90);
            TextBox n = Box(90, 135);
            TextBox k = Box(90, 180);
            TextBox p = Box(330, 90);
            TextBox q = Box(330, 135);
            TextBox r = Box(330, 180);
            TextBox result = ResultBox(25, 290);

            page.Controls.Add(Lbl("t год:", 25, 90, 60));
            page.Controls.Add(Lbl("n хв:", 25, 135, 60));
            page.Controls.Add(Lbl("k сек:", 25, 180, 60));
            page.Controls.Add(Lbl("p год:", 250, 90, 75));
            page.Controls.Add(Lbl("q хв:", 250, 135, 75));
            page.Controls.Add(Lbl("r сек:", 250, 180, 75));

            page.Controls.Add(t); page.Controls.Add(n); page.Controls.Add(k);
            page.Controls.Add(p); page.Controls.Add(q); page.Controls.Add(r);

            page.Controls.Add(Btn("Порахувати час", 25, 235, (s, e) =>
            {
                if (!GetInt(t, "t", out int th)) return;
                if (!GetInt(n, "n", out int nm)) return;
                if (!GetInt(k, "k", out int ks)) return;
                if (!GetInt(p, "p", out int ph)) return;
                if (!GetInt(q, "q", out int qm)) return;
                if (!GetInt(r, "r", out int rs)) return;

                if (th < 0 || th > 23 || nm < 0 || nm > 59 || ks < 0 || ks > 59)
                {
                    MessageBox.Show("Поточний час має бути: 0≤t≤23, 0≤n≤59, 0≤k≤59");
                    return;
                }

                int total = th * 3600 + nm * 60 + ks + ph * 3600 + qm * 60 + rs;
                total %= 24 * 3600;

                int hh = total / 3600;
                int mm = (total % 3600) / 60;
                int ss = total % 60;

                result.Text = $"Початковий час: {th:D2}:{nm:D2}:{ks:D2}\r\n" +
                              $"Додано: {ph} год {qm} хв {rs} с\r\n\r\n" +
                              $"Новий час: {hh:D2}:{mm:D2}:{ss:D2}";
            }));

            page.Controls.Add(result);
            return page;
        }

        private TabPage CreateTask3()
        {
            TabPage page = new TabPage("Завдання 3");
            page.BackColor = Color.White;
            page.Controls.Add(Title("3. Чи дорівнює сума перших двох цифр сумі останніх двох"));

            TextBox number = Box(180, 95, 170);
            TextBox result = ResultBox(25, 210);

            page.Controls.Add(Lbl("Чотиризначне число:", 25, 95, 150));
            page.Controls.Add(number);

            page.Controls.Add(Btn("Перевірити", 25, 150, (s, e) =>
            {
                if (!GetInt(number, "число", out int num)) return;

                if (num < 1000 || num > 9999)
                {
                    MessageBox.Show("Потрібно ввести саме чотиризначне число");
                    return;
                }

                int d1 = num / 1000;
                int d2 = num / 100 % 10;
                int d3 = num / 10 % 10;
                int d4 = num % 10;

                bool ok = d1 + d2 == d3 + d4;

                result.Text = $"Число: {num}\r\n" +
                              $"Перші дві цифри: {d1} + {d2} = {d1 + d2}\r\n" +
                              $"Останні дві цифри: {d3} + {d4} = {d3 + d4}\r\n\r\n" +
                              $"Результат: {ok}";
            }));

            page.Controls.Add(result);
            return page;
        }

        private TabPage CreateTask4()
        {
            TabPage page = new TabPage("Завдання 4");
            page.BackColor = Color.White;
            page.Controls.Add(Title("4. Обчислити число і місяць у невисокосному році за номером дня"));

            TextBox day = Box(160, 95, 160);
            TextBox result = ResultBox(25, 210);

            page.Controls.Add(Lbl("Номер дня:", 25, 95, 120));
            page.Controls.Add(day);

            page.Controls.Add(Btn("Знайти дату", 25, 150, (s, e) =>
            {
                if (!GetInt(day, "номер дня", out int d)) return;

                if (d < 1 || d > 365)
                {
                    MessageBox.Show("Номер дня має бути від 1 до 365");
                    return;
                }

                int[] days = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
                string[] months =
                {
                    "січня", "лютого", "березня", "квітня",
                    "травня", "червня", "липня", "серпня",
                    "вересня", "жовтня", "листопада", "грудня"
                };

                int month = 0;
                while (d > days[month])
                {
                    d -= days[month];
                    month++;
                }

                result.Text = $"Дата у невисокосному році:\r\n\r\n{d} {months[month]}";
            }));

            page.Controls.Add(result);
            return page;
        }

        private TabPage CreateTask5()
        {
            TabPage page = new TabPage("Завдання 5");
            page.BackColor = Color.White;
            page.Controls.Add(Title("5. Перевірити, чи всі цифри натурального числа різні"));

            TextBox number = Box(190, 95, 180);
            TextBox result = ResultBox(25, 210);

            page.Controls.Add(Lbl("Натуральне число:", 25, 95, 150));
            page.Controls.Add(number);

            page.Controls.Add(Btn("Перевірити", 25, 150, (s, e) =>
            {
                if (!GetInt(number, "n", out int n)) return;

                if (n <= 0)
                {
                    MessageBox.Show("Число має бути натуральним");
                    return;
                }

                string sNum = n.ToString();
                bool different = sNum.Distinct().Count() == sNum.Length;

                result.Text = $"Число: {n}\r\n" +
                              $"Цифри: {string.Join(", ", sNum.ToCharArray())}\r\n\r\n" +
                              $"Усі цифри різні: {different}";
            }));

            page.Controls.Add(result);
            return page;
        }

        private TabPage CreateTask6()
        {
            TabPage page = new TabPage("Завдання 6");
            page.BackColor = Color.White;
            page.Controls.Add(Title("6. Переставити елементи масиву X[N] у зворотному порядку без іншого масиву"));

            TextBox array = new TextBox();
            array.Location = new Point(25, 100);
            array.Size = new Size(560, 35);
            array.Font = new Font("Consolas", 10);

            TextBox result = ResultBox(25, 230, 650, 220);

            page.Controls.Add(Lbl("Введіть числа через пробіл або кому:", 25, 70, 350));
            page.Controls.Add(array);

            page.Controls.Add(Btn("Розвернути масив", 25, 155, (s, e) =>
            {
                try
                {
                    int[] arr = array.Text
                        .Split(new[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(int.Parse)
                        .ToArray();

                    if (arr.Length == 0)
                    {
                        MessageBox.Show("Введіть хоча б одне число");
                        return;
                    }

                    string before = string.Join(" ", arr);

                    for (int i = 0, j = arr.Length - 1; i < j; i++, j--)
                    {
                        int temp = arr[i];
                        arr[i] = arr[j];
                        arr[j] = temp;
                    }

                    result.Text = "Початковий масив:\r\n" + before +
                                  "\r\n\r\nМасив після перестановки:\r\n" +
                                  string.Join(" ", arr);
                }
                catch
                {
                    MessageBox.Show("Масив має містити тільки цілі числа");
                }
            }));

            page.Controls.Add(result);
            return page;
        }

        private TabPage CreateTask7()
        {
            TabPage page = new TabPage("Завдання 7");
            page.BackColor = Color.White;
            page.Controls.Add(Title("7. Підрахувати кількість слів у рядку, що закінчується крапкою"));

            TextBox input = new TextBox();
            input.Location = new Point(25, 95);
            input.Size = new Size(650, 90);
            input.Multiline = true;
            input.ScrollBars = ScrollBars.Vertical;

            TextBox result = ResultBox(25, 285, 650, 170);

            page.Controls.Add(Lbl("Введіть рядок:", 25, 65, 150));
            page.Controls.Add(input);

            page.Controls.Add(Btn("Порахувати слова", 25, 205, (s, e) =>
            {
                string text = input.Text.Trim();

                if (!text.EndsWith("."))
                {
                    MessageBox.Show("Рядок повинен закінчуватися крапкою");
                    return;
                }

                text = text.Substring(0, text.Length - 1).Trim();

                if (text.Length == 0)
                {
                    result.Text = "Кількість слів: 0";
                    return;
                }

                char[] separators = { ' ', ',', ';', ':', '!', '?', '\r', '\n', '\t' };
                int count = text.Split(separators, StringSplitOptions.RemoveEmptyEntries).Length;

                result.Text = $"Рядок:\r\n{text}.\r\n\r\nКількість слів: {count}";
            }));

            page.Controls.Add(result);
            return page;
        }
    }
}