using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Laboratorna19
{
    public partial class Form1 : Form
    {
        private TextBox txtInput;
        private RichTextBox output;
        private CheckBox chkIgnoreSpaces;
        private CheckBox chkIgnoreCase;

        public Form1()
        {
            InitializeComponent();
            BuildInterface();
        }

        private void BuildInterface()
        {
            Text = "Laboratorna19 - Варіант 6";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(900, 620);
            MinimumSize = new Size(840, 560);
            BackColor = Color.White;
            Font = new Font("Segoe UI", 10);

            Label title = new Label();
            title.Text = "Laboratorna19 - Рядки та символи";
            title.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            title.Location = new Point(25, 15);
            title.Size = new Size(760, 35);
            Controls.Add(title);

            Label task = new Label();
            task.Text = "Варіант 6: підрахувати кількість однакових символів у заданому рядку";
            task.Location = new Point(27, 55);
            task.Size = new Size(760, 28);
            Controls.Add(task);

            Label lblInput = new Label();
            lblInput.Text = "Введіть рядок:";
            lblInput.Location = new Point(30, 105);
            lblInput.Size = new Size(200, 28);
            Controls.Add(lblInput);

            txtInput = new TextBox();
            txtInput.Location = new Point(30, 135);
            txtInput.Size = new Size(800, 90);
            txtInput.Multiline = true;
            txtInput.ScrollBars = ScrollBars.Vertical;
            txtInput.Text = "мама мила раму, а рама була мала";
            Controls.Add(txtInput);

            chkIgnoreSpaces = new CheckBox();
            chkIgnoreSpaces.Text = "Не рахувати пробіли";
            chkIgnoreSpaces.Location = new Point(30, 245);
            chkIgnoreSpaces.Size = new Size(220, 30);
            chkIgnoreSpaces.Checked = true;
            Controls.Add(chkIgnoreSpaces);

            chkIgnoreCase = new CheckBox();
            chkIgnoreCase.Text = "Не враховувати регістр";
            chkIgnoreCase.Location = new Point(270, 245);
            chkIgnoreCase.Size = new Size(240, 30);
            chkIgnoreCase.Checked = true;
            Controls.Add(chkIgnoreCase);

            Controls.Add(CreateButton("Порахувати", 30, 295, CountSymbols));
            Controls.Add(CreateButton("Очистити", 210, 295, ClearText));
            Controls.Add(CreateButton("Приклад", 390, 295, SetExample));

            output = new RichTextBox();
            output.Location = new Point(30, 355);
            output.Size = new Size(800, 190);
            output.ReadOnly = true;
            output.Font = new Font("Consolas", 10);
            output.BackColor = Color.FromArgb(248, 248, 248);
            Controls.Add(output);

            CountSymbols(null, EventArgs.Empty);
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

        private void CountSymbols(object sender, EventArgs e)
        {
            string text = txtInput.Text;

            if (text.Length == 0)
            {
                MessageBox.Show("Введіть рядок.");
                txtInput.Focus();
                return;
            }

            if (chkIgnoreCase.Checked)
                text = text.ToLower();

            Dictionary<char, int> counts = new Dictionary<char, int>();

            foreach (char ch in text)
            {
                if (chkIgnoreSpaces.Checked && char.IsWhiteSpace(ch))
                    continue;

                if (counts.ContainsKey(ch))
                    counts[ch]++;
                else
                    counts[ch] = 1;
            }

            var repeated = counts
                .Where(x => x.Value > 1)
                .OrderByDescending(x => x.Value)
                .ThenBy(x => x.Key)
                .ToList();

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Laboratorna19 - Варіант 6");
            sb.AppendLine();
            sb.AppendLine("Початковий рядок:");
            sb.AppendLine(txtInput.Text);
            sb.AppendLine();

            if (repeated.Count == 0)
            {
                sb.AppendLine("Однакових символів немає.");
            }
            else
            {
                sb.AppendLine("Однакові символи:");
                sb.AppendLine();

                foreach (var item in repeated)
                {
                    string symbol = item.Key == ' ' ? "[пробіл]" : item.Key.ToString();
                    sb.AppendLine("'" + symbol + "' -> " + item.Value + " рази");
                }

                sb.AppendLine();
                sb.AppendLine("Кількість різних символів, які повторюються: " + repeated.Count);
                sb.AppendLine("Загальна кількість повторних входжень: " + repeated.Sum(x => x.Value - 1));
            }

            output.Text = sb.ToString();
        }

        private void ClearText(object sender, EventArgs e)
        {
            txtInput.Clear();
            output.Clear();
            txtInput.Focus();
        }

        private void SetExample(object sender, EventArgs e)
        {
            txtInput.Text = "мама мила раму, а рама була мала";
            CountSymbols(null, EventArgs.Empty);
        }
    }
}