using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Laboratorna29
{
    public partial class Form1 : Form
    {
        TextBox txtName;
        TextBox txtMessage;
        TextBox txtAddress;
        NumericUpDown numPort;
        RichTextBox chatBox;
        Button btnLogin;
        Button btnLogout;
        Button btnSend;
        Button btnFont;
        Button btnSaveLog;
        StatusStrip status;
        ToolStripStatusLabel statusLabel;

        UdpClient client;
        IPAddress groupAddress;
        bool alive = false;
        string userName = "";
        int localPort = 8001;
        int remotePort = 8001;
        string host = "235.5.5.1";
        const int ttl = 20;

        public Form1()
        {
            InitializeComponent();
            BuildInterface();
        }

        void BuildInterface()
        {
            Text = "Laboratorna29 - UDP Chat";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(760, 700);
            MinimumSize = new Size(760, 700);
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 10);

            Panel top = new Panel();
            top.Dock = DockStyle.Top;
            top.Height = 125;
            top.BackColor = Color.FromArgb(30, 42, 62);
            Controls.Add(top);

            Label title = new Label();
            title.Text = "Laboratorna29: UDP чат";
            title.ForeColor = Color.White;
            title.Font = new Font("Segoe UI Semibold", 17);
            title.Location = new Point(18, 12);
            title.Size = new Size(360, 35);
            top.Controls.Add(title);

            Label nameLabel = MakeTopLabel("Ім'я:", 20, 65, 45);
            top.Controls.Add(nameLabel);

            txtName = new TextBox();
            txtName.Location = new Point(70, 62);
            txtName.Size = new Size(150, 28);
            txtName.Text = Environment.UserName;
            top.Controls.Add(txtName);

            Label addressLabel = MakeTopLabel("Адреса:", 235, 65, 70);
            top.Controls.Add(addressLabel);

            txtAddress = new TextBox();
            txtAddress.Location = new Point(305, 62);
            txtAddress.Size = new Size(120, 28);
            txtAddress.Text = host;
            top.Controls.Add(txtAddress);

            Label portLabel = MakeTopLabel("Порт:", 440, 65, 55);
            top.Controls.Add(portLabel);

            numPort = new NumericUpDown();
            numPort.Location = new Point(495, 62);
            numPort.Size = new Size(85, 28);
            numPort.Minimum = 1000;
            numPort.Maximum = 65000;
            numPort.Value = 8001;
            top.Controls.Add(numPort);

            btnLogin = MakeButton("Увійти", 595, 58, 130, 34);
            btnLogin.Parent = top;
            btnLogin.Click += BtnLogin_Click;

            chatBox = new RichTextBox();
            chatBox.Location = new Point(15, 140);
            chatBox.Size = new Size(710, 330);
            chatBox.ReadOnly = true;
            chatBox.BackColor = Color.White;
            chatBox.BorderStyle = BorderStyle.FixedSingle;
            chatBox.Font = new Font("Segoe UI", 10);
            Controls.Add(chatBox);

            txtMessage = new TextBox();
            txtMessage.Location = new Point(15, 485);
            txtMessage.Size = new Size(515, 30);
            txtMessage.Enabled = false;
            txtMessage.KeyDown += TxtMessage_KeyDown;
            Controls.Add(txtMessage);

            btnSend = MakeButton("Відправити", 545, 482, 180, 36);
            btnSend.Enabled = false;
            btnSend.Click += BtnSend_Click;
            Controls.Add(btnSend);

            btnLogout = MakeButton("Вийти", 15, 528, 130, 36);
            btnLogout.Enabled = false;
            btnLogout.Click += BtnLogout_Click;
            Controls.Add(btnLogout);

            btnFont = MakeButton("Шрифт", 160, 528, 130, 36);
            btnFont.Click += BtnFont_Click;
            Controls.Add(btnFont);

            btnSaveLog = MakeButton("Зберегти лог", 305, 528, 150, 36);
            btnSaveLog.Click += BtnSaveLog_Click;
            Controls.Add(btnSaveLog);

            status = new StatusStrip();
            statusLabel = new ToolStripStatusLabel("Готово");
            status.Items.Add(statusLabel);
            Controls.Add(status);

            FormClosing += Form1_FormClosing;
        }

        Label MakeTopLabel(string text, int x, int y, int w)
        {
            Label label = new Label();
            label.Text = text;
            label.Location = new Point(x, y + 3);
            label.Size = new Size(w, 25);
            label.ForeColor = Color.White;
            return label;
        }

        Button MakeButton(string text, int x, int y, int w, int h)
        {
            Button button = new Button();
            button.Text = text;
            button.Location = new Point(x, y);
            button.Size = new Size(w, h);
            button.BackColor = Color.FromArgb(48, 97, 180);
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            return button;
        }

        void BtnLogin_Click(object sender, EventArgs e)
        {
            userName = txtName.Text.Trim();

            if (userName == "")
            {
                MessageBox.Show("Введи ім'я.");
                return;
            }

            try
            {
                host = txtAddress.Text.Trim();
                localPort = (int)numPort.Value;
                remotePort = (int)numPort.Value;
                groupAddress = IPAddress.Parse(host);

                client = new UdpClient();
                client.ExclusiveAddressUse = false;
                client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                client.Client.Bind(new IPEndPoint(IPAddress.Any, localPort));
                client.JoinMulticastGroup(groupAddress, ttl);

                alive = true;

                Task.Run(ReceiveMessages);

                txtName.ReadOnly = true;
                txtAddress.ReadOnly = true;
                numPort.Enabled = false;
                btnLogin.Enabled = false;
                btnLogout.Enabled = true;
                btnSend.Enabled = true;
                txtMessage.Enabled = true;
                txtMessage.Focus();

                SendSystemMessage(userName + " увійшов у чат");
                statusLabel.Text = "Підключено до " + host + ":" + localPort;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка");
                ExitChat(false);
            }
        }

        void ReceiveMessages()
        {
            try
            {
                while (alive)
                {
                    IPEndPoint remoteIp = new IPEndPoint(IPAddress.Any, 0);
                    byte[] data = client.Receive(ref remoteIp);
                    string message = Encoding.UTF8.GetString(data);

                    Invoke(new MethodInvoker(delegate
                    {
                        string time = DateTime.Now.ToString("HH:mm:ss");
                        chatBox.AppendText("[" + time + "] " + message + Environment.NewLine);
                        chatBox.SelectionStart = chatBox.Text.Length;
                        chatBox.ScrollToCaret();
                    }));
                }
            }
            catch
            {
                if (alive)
                {
                    Invoke(new MethodInvoker(delegate
                    {
                        statusLabel.Text = "Приймання зупинено";
                    }));
                }
            }
        }

        void BtnSend_Click(object sender, EventArgs e)
        {
            SendUserMessage();
        }

        void TxtMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                SendUserMessage();
            }
        }

        void SendUserMessage()
        {
            string text = txtMessage.Text.Trim();

            if (text == "") return;

            string message = userName + ": " + text;
            SendRaw(message);
            txtMessage.Clear();
            txtMessage.Focus();
        }

        void SendSystemMessage(string text)
        {
            SendRaw(text);
        }

        void SendRaw(string message)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                client.Send(data, data.Length, new IPEndPoint(groupAddress, remotePort));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка");
            }
        }

        void BtnLogout_Click(object sender, EventArgs e)
        {
            ExitChat(true);
        }

        void ExitChat(bool sendExitMessage)
        {
            try
            {
                if (client != null && alive && sendExitMessage)
                    SendSystemMessage(userName + " вийшов із чату");

                alive = false;

                if (client != null)
                {
                    try
                    {
                        client.DropMulticastGroup(groupAddress);
                    }
                    catch
                    {
                    }

                    client.Close();
                    client = null;
                }
            }
            catch
            {
            }

            btnLogin.Enabled = true;
            btnLogout.Enabled = false;
            btnSend.Enabled = false;
            txtMessage.Enabled = false;
            txtName.ReadOnly = false;
            txtAddress.ReadOnly = false;
            numPort.Enabled = true;
            statusLabel.Text = "Відключено";
        }

        void BtnFont_Click(object sender, EventArgs e)
        {
            FontDialog dialog = new FontDialog();
            dialog.Font = chatBox.Font;

            if (dialog.ShowDialog() == DialogResult.OK)
                chatBox.Font = dialog.Font;
        }

        void BtnSaveLog_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Text file (*.txt)|*.txt";
            dialog.FileName = "udp_chat_log.txt";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(dialog.FileName, chatBox.Text, Encoding.UTF8);
                MessageBox.Show("Лог збережено.");
            }
        }

        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (alive)
                ExitChat(true);
        }
    }
}