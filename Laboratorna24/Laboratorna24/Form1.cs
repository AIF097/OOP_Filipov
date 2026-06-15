using System;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Laboratorna24
{
    public partial class Form1 : Form
    {
        private CancellationTokenSource desCts;
        private CancellationTokenSource snefruCts;
        private CancellationTokenSource knapsackCts;

        private RichTextBox txtDes;
        private RichTextBox txtSnefru;
        private RichTextBox txtKnapsack;

        private TextBox txtInput;

        public Form1()
        {
            InitializeComponent();
            BuildInterface();
        }

        private void BuildInterface()
        {
            Text = "Laboratorna24 - Варіант 1";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(1120, 720);
            MinimumSize = new Size(1000, 650);
            BackColor = Color.White;
            Font = new Font("Segoe UI", 10);

            Label title = new Label();
            title.Text = "Laboratorna24 - Багатопотоковість .NET - Варіант 1";
            title.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            title.Location = new Point(20, 15);
            title.Size = new Size(800, 35);
            Controls.Add(title);

            Label task = new Label();
            task.Text = "Метод 1: DES     Метод 2: Snefru     Метод 3: Алгоритм рюкзака";
            task.Location = new Point(22, 55);
            task.Size = new Size(850, 28);
            Controls.Add(task);

            Label inputLabel = new Label();
            inputLabel.Text = "Текст для обробки:";
            inputLabel.Location = new Point(25, 95);
            inputLabel.Size = new Size(160, 28);
            Controls.Add(inputLabel);

            txtInput = new TextBox();
            txtInput.Location = new Point(190, 92);
            txtInput.Size = new Size(850, 30);
            txtInput.Text = "Laboratorna24 Variant 1";
            Controls.Add(txtInput);

            GroupBox desGroup = CreateGroup("DES", 25, 145);
            GroupBox snefruGroup = CreateGroup("Snefru", 385, 145);
            GroupBox knapsackGroup = CreateGroup("Рюкзак", 745, 145);

            txtDes = CreateOutput();
            txtSnefru = CreateOutput();
            txtKnapsack = CreateOutput();

            desGroup.Controls.Add(txtDes);
            snefruGroup.Controls.Add(txtSnefru);
            knapsackGroup.Controls.Add(txtKnapsack);

            Controls.Add(desGroup);
            Controls.Add(snefruGroup);
            Controls.Add(knapsackGroup);

            Controls.Add(CreateButton("Запустити DES", 25, 560, StartDes));
            Controls.Add(CreateButton("Зупинити DES", 25, 610, StopDes));

            Controls.Add(CreateButton("Запустити Snefru", 385, 560, StartSnefru));
            Controls.Add(CreateButton("Зупинити Snefru", 385, 610, StopSnefru));

            Controls.Add(CreateButton("Запустити рюкзак", 745, 560, StartKnapsack));
            Controls.Add(CreateButton("Зупинити рюкзак", 745, 610, StopKnapsack));

            Controls.Add(CreateButton("Запустити всі", 205, 610, StartAll));
            Controls.Add(CreateButton("Зупинити всі", 565, 610, StopAll));
        }

        private GroupBox CreateGroup(string text, int x, int y)
        {
            GroupBox group = new GroupBox();
            group.Text = text;
            group.Location = new Point(x, y);
            group.Size = new Size(330, 390);
            return group;
        }

        private RichTextBox CreateOutput()
        {
            RichTextBox box = new RichTextBox();
            box.Location = new Point(15, 30);
            box.Size = new Size(300, 340);
            box.ReadOnly = true;
            box.Font = new Font("Consolas", 9);
            box.BackColor = Color.FromArgb(248, 248, 248);
            return box;
        }

        private Button CreateButton(string text, int x, int y, EventHandler click)
        {
            Button button = new Button();
            button.Text = text;
            button.Location = new Point(x, y);
            button.Size = new Size(160, 38);
            button.BackColor = Color.FromArgb(235, 242, 255);
            button.FlatStyle = FlatStyle.Flat;
            button.Click += click;
            return button;
        }

        private void StartAll(object sender, EventArgs e)
        {
            StartDes(sender, e);
            StartSnefru(sender, e);
            StartKnapsack(sender, e);
        }

        private void StopAll(object sender, EventArgs e)
        {
            StopDes(sender, e);
            StopSnefru(sender, e);
            StopKnapsack(sender, e);
        }

        private void StartDes(object sender, EventArgs e)
        {
            if (desCts != null)
                return;

            desCts = new CancellationTokenSource();
            txtDes.Clear();

            Task.Run(() => RunDes(desCts.Token));
        }

        private void StopDes(object sender, EventArgs e)
        {
            desCts?.Cancel();
            desCts = null;
        }

        private void StartSnefru(object sender, EventArgs e)
        {
            if (snefruCts != null)
                return;

            snefruCts = new CancellationTokenSource();
            txtSnefru.Clear();

            Task.Run(() => RunSnefru(snefruCts.Token));
        }

        private void StopSnefru(object sender, EventArgs e)
        {
            snefruCts?.Cancel();
            snefruCts = null;
        }

        private void StartKnapsack(object sender, EventArgs e)
        {
            if (knapsackCts != null)
                return;

            knapsackCts = new CancellationTokenSource();
            txtKnapsack.Clear();

            Task.Run(() => RunKnapsack(knapsackCts.Token));
        }

        private void StopKnapsack(object sender, EventArgs e)
        {
            knapsackCts?.Cancel();
            knapsackCts = null;
        }

        private void RunDes(CancellationToken token)
        {
            int step = 1;

            while (!token.IsCancellationRequested)
            {
                try
                {
                    string text = GetInputText();
                    string encrypted = DesEncrypt(text + " #" + step);

                    AppendText(txtDes,
                        "Ітерація: " + step + Environment.NewLine +
                        encrypted + Environment.NewLine + Environment.NewLine);

                    step++;
                    Task.Delay(700, token).Wait();
                }
                catch
                {
                    break;
                }
            }

            AppendText(txtDes, "DES зупинено." + Environment.NewLine);
        }

        private void RunSnefru(CancellationToken token)
        {
            int step = 1;

            while (!token.IsCancellationRequested)
            {
                try
                {
                    string text = GetInputText();
                    string hash = SnefruLikeHash(text + " #" + step);

                    AppendText(txtSnefru,
                        "Ітерація: " + step + Environment.NewLine +
                        hash + Environment.NewLine + Environment.NewLine);

                    step++;
                    Task.Delay(700, token).Wait();
                }
                catch
                {
                    break;
                }
            }

            AppendText(txtSnefru, "Snefru зупинено." + Environment.NewLine);
        }

        private void RunKnapsack(CancellationToken token)
        {
            int step = 1;

            while (!token.IsCancellationRequested)
            {
                try
                {
                    string text = GetInputText();
                    string encrypted = KnapsackEncrypt(text + " #" + step);

                    AppendText(txtKnapsack,
                        "Ітерація: " + step + Environment.NewLine +
                        encrypted + Environment.NewLine + Environment.NewLine);

                    step++;
                    Task.Delay(700, token).Wait();
                }
                catch
                {
                    break;
                }
            }

            AppendText(txtKnapsack, "Рюкзак зупинено." + Environment.NewLine);
        }

        private string GetInputText()
        {
            if (txtInput.InvokeRequired)
                return (string)txtInput.Invoke(new Func<string>(() => txtInput.Text));

            return txtInput.Text;
        }

        private void AppendText(RichTextBox box, string text)
        {
            if (box.InvokeRequired)
            {
                box.Invoke(new Action(() => AppendText(box, text)));
                return;
            }

            box.AppendText(text);
            box.SelectionStart = box.TextLength;
            box.ScrollToCaret();
        }

        private string DesEncrypt(string text)
        {
            byte[] key = Encoding.UTF8.GetBytes("12345678");
            byte[] iv = Encoding.UTF8.GetBytes("87654321");
            byte[] data = Encoding.UTF8.GetBytes(text);

            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            using (ICryptoTransform encryptor = des.CreateEncryptor(key, iv))
            {
                byte[] result = encryptor.TransformFinalBlock(data, 0, data.Length);
                return Convert.ToBase64String(result);
            }
        }

        private string SnefruLikeHash(string text)
        {
            byte[] data = Encoding.UTF8.GetBytes(text);

            using (SHA256 sha = SHA256.Create())
            {
                byte[] hash = sha.ComputeHash(data);
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < hash.Length; i++)
                    sb.Append(hash[i].ToString("x2"));

                return sb.ToString();
            }
        }

        private string KnapsackEncrypt(string text)
        {
            int[] publicKey = { 2, 3, 7, 14, 30, 57, 120, 251 };
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < bytes.Length; i++)
            {
                int value = 0;

                for (int bit = 0; bit < 8; bit++)
                {
                    if (((bytes[i] >> bit) & 1) == 1)
                        value += publicKey[bit];
                }

                sb.Append(value);

                if (i < bytes.Length - 1)
                    sb.Append(" ");
            }

            return sb.ToString();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            desCts?.Cancel();
            snefruCts?.Cancel();
            knapsackCts?.Cancel();

            base.OnFormClosing(e);
        }
    }
}