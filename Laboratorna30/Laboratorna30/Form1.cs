using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace Laboratorna30
{
    public partial class Form1 : Form
    {
        TextBox tbHost;
        TextBox tbUser;
        TextBox tbPass;
        TextBox tbRemotePath;
        TextBox tbNewName;
        TextBox tbLog;
        ListBox ftpList;
        Button btnConnect;
        Button btnUpload;
        Button btnDownload;
        Button btnDeleteFile;
        Button btnCreateFolder;
        Button btnDeleteFolder;
        Button btnRename;
        Button btnSize;
        Button btnSaveSettings;
        Button btnLoadSettings;
        StatusStrip status;
        ToolStripStatusLabel statusLabel;

        string currentPath = "/";

        public Form1()
        {
            InitializeComponent();
            BuildInterface();
            LoadSettings(false);
        }

        void BuildInterface()
        {
            Text = "Laboratorna30 - FTP клієнт";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(1040, 650);
            MinimumSize = new Size(1040, 650);
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 10);

            Panel top = new Panel();
            top.Dock = DockStyle.Top;
            top.Height = 145;
            top.BackColor = Color.FromArgb(34, 46, 68);
            Controls.Add(top);

            Label title = new Label();
            title.Text = "Laboratorna30: FTP клієнт";
            title.ForeColor = Color.White;
            title.Font = new Font("Segoe UI Semibold", 17);
            title.Location = new Point(18, 15);
            title.Size = new Size(400, 35);
            top.Controls.Add(title);

            Label l1 = MakeTopLabel("Хост:", 20, 68, 60);
            top.Controls.Add(l1);

            tbHost = new TextBox();
            tbHost.Location = new Point(80, 65);
            tbHost.Size = new Size(250, 28);
            tbHost.Text = "ftp://192.168.1.10/";
            top.Controls.Add(tbHost);

            Label l2 = MakeTopLabel("Користувач:", 350, 68, 100);
            top.Controls.Add(l2);

            tbUser = new TextBox();
            tbUser.Location = new Point(455, 65);
            tbUser.Size = new Size(150, 28);
            tbUser.Text = "user";
            top.Controls.Add(tbUser);

            Label l3 = MakeTopLabel("Пароль:", 625, 68, 70);
            top.Controls.Add(l3);

            tbPass = new TextBox();
            tbPass.Location = new Point(695, 65);
            tbPass.Size = new Size(150, 28);
            tbPass.UseSystemPasswordChar = true;
            top.Controls.Add(tbPass);

            btnConnect = MakeButton("Підключитися", 865, 61, 145, 36);
            btnConnect.Parent = top;
            btnConnect.Click += BtnConnect_Click;

            Label l4 = MakeTopLabel("Шлях FTP:", 20, 108, 80);
            top.Controls.Add(l4);

            tbRemotePath = new TextBox();
            tbRemotePath.Location = new Point(105, 105);
            tbRemotePath.Size = new Size(225, 28);
            tbRemotePath.Text = "/";
            top.Controls.Add(tbRemotePath);

            Label l5 = MakeTopLabel("Нова назва:", 350, 108, 100);
            top.Controls.Add(l5);

            tbNewName = new TextBox();
            tbNewName.Location = new Point(455, 105);
            tbNewName.Size = new Size(200, 28);
            top.Controls.Add(tbNewName);

            Panel left = new Panel();
            left.Location = new Point(15, 160);
            left.Size = new Size(645, 420);
            left.BackColor = Color.White;
            left.BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(left);

            Label listTitle = new Label();
            listTitle.Text = "Вміст FTP сервера";
            listTitle.Location = new Point(15, 12);
            listTitle.Size = new Size(250, 25);
            listTitle.Font = new Font("Segoe UI Semibold", 11);
            left.Controls.Add(listTitle);

            ftpList = new ListBox();
            ftpList.Location = new Point(15, 45);
            ftpList.Size = new Size(610, 350);
            ftpList.HorizontalScrollbar = true;
            ftpList.DoubleClick += FtpList_DoubleClick;
            left.Controls.Add(ftpList);

            Panel right = new Panel();
            right.Location = new Point(675, 160);
            right.Size = new Size(335, 420);
            right.BackColor = Color.White;
            right.BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(right);

            btnUpload = MakeButton("Завантажити на FTP", 20, 20, 295, 36);
            btnUpload.Parent = right;
            btnUpload.Click += BtnUpload_Click;

            btnDownload = MakeButton("Скачати з FTP", 20, 65, 295, 36);
            btnDownload.Parent = right;
            btnDownload.Click += BtnDownload_Click;

            btnDeleteFile = MakeButton("Видалити файл", 20, 110, 295, 36);
            btnDeleteFile.Parent = right;
            btnDeleteFile.Click += BtnDeleteFile_Click;

            btnCreateFolder = MakeButton("Створити каталог", 20, 155, 295, 36);
            btnCreateFolder.Parent = right;
            btnCreateFolder.Click += BtnCreateFolder_Click;

            btnDeleteFolder = MakeButton("Видалити каталог", 20, 200, 295, 36);
            btnDeleteFolder.Parent = right;
            btnDeleteFolder.Click += BtnDeleteFolder_Click;

            btnRename = MakeButton("Перейменувати", 20, 245, 295, 36);
            btnRename.Parent = right;
            btnRename.Click += BtnRename_Click;

            btnSize = MakeButton("Розмір файлу", 20, 290, 295, 36);
            btnSize.Parent = right;
            btnSize.Click += BtnSize_Click;

            btnSaveSettings = MakeButton("Зберегти налаштування", 20, 335, 295, 36);
            btnSaveSettings.Parent = right;
            btnSaveSettings.Click += BtnSaveSettings_Click;

            btnLoadSettings = MakeButton("Завантажити налаштування", 20, 375, 295, 32);
            btnLoadSettings.Parent = right;
            btnLoadSettings.Click += BtnLoadSettings_Click;

            tbLog = new TextBox();
            tbLog.Location = new Point(15, 590);
            tbLog.Size = new Size(995, 24);
            tbLog.ReadOnly = true;
            Controls.Add(tbLog);

            status = new StatusStrip();
            statusLabel = new ToolStripStatusLabel("Готово");
            status.Items.Add(statusLabel);
            Controls.Add(status);
        }

        Label MakeTopLabel(string text, int x, int y, int w)
        {
            Label l = new Label();
            l.Text = text;
            l.Location = new Point(x, y + 3);
            l.Size = new Size(w, 25);
            l.ForeColor = Color.White;
            return l;
        }

        Button MakeButton(string text, int x, int y, int w, int h)
        {
            Button b = new Button();
            b.Text = text;
            b.Location = new Point(x, y);
            b.Size = new Size(w, h);
            b.BackColor = Color.FromArgb(48, 97, 180);
            b.ForeColor = Color.White;
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0;
            return b;
        }

        void BtnConnect_Click(object sender, EventArgs e)
        {
            currentPath = FixPath(tbRemotePath.Text);
            LoadFtpList();
        }

        void LoadFtpList()
        {
            try
            {
                ftpList.Items.Clear();

                FtpWebRequest request = CreateRequest(currentPath, WebRequestMethods.Ftp.ListDirectoryDetails);
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    while (!reader.EndOfStream)
                        ftpList.Items.Add(reader.ReadLine());

                    statusLabel.Text = response.StatusDescription.Trim();
                }

                tbRemotePath.Text = currentPath;
                Log("Список завантажено: " + currentPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка");
            }
        }

        FtpWebRequest CreateRequest(string path, string method)
        {
            string url = BuildUrl(path);
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);
            request.Credentials = new NetworkCredential(tbUser.Text, tbPass.Text);
            request.Method = method;
            request.UseBinary = true;
            request.UsePassive = true;
            request.KeepAlive = false;
            return request;
        }

        string BuildUrl(string path)
        {
            string host = tbHost.Text.Trim();

            if (!host.StartsWith("ftp://"))
                host = "ftp://" + host;

            if (!host.EndsWith("/"))
                host += "/";

            path = path.Replace("\\", "/");

            if (path.StartsWith("/"))
                path = path.Substring(1);

            return host + path;
        }

        string FixPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return "/";

            path = path.Replace("\\", "/");

            if (!path.StartsWith("/"))
                path = "/" + path;

            return path;
        }

        string GetSelectedName()
        {
            if (ftpList.SelectedItem == null)
            {
                MessageBox.Show("Вибери елемент у списку.");
                return "";
            }

            string line = ftpList.SelectedItem.ToString();
            string[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
                return "";

            return parts[parts.Length - 1];
        }

        string CombineFtpPath(string folder, string name)
        {
            folder = FixPath(folder);

            if (!folder.EndsWith("/"))
                folder += "/";

            return folder + name;
        }

        void BtnUpload_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Всі файли|*.*";
            dialog.Multiselect = true;

            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                foreach (string filePath in dialog.FileNames)
                {
                    string fileName = Path.GetFileName(filePath);
                    string ftpPath = CombineFtpPath(currentPath, fileName);

                    FtpWebRequest request = CreateRequest(ftpPath, WebRequestMethods.Ftp.UploadFile);

                    byte[] data = File.ReadAllBytes(filePath);

                    using (Stream requestStream = request.GetRequestStream())
                        requestStream.Write(data, 0, data.Length);

                    using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                        Log(fileName + " завантажено: " + response.StatusDescription.Trim());
                }

                LoadFtpList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка");
            }
        }

        void BtnDownload_Click(object sender, EventArgs e)
        {
            string name = GetSelectedName();
            if (name == "") return;

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.FileName = name;
            dialog.Filter = "Всі файли|*.*";

            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                string ftpPath = CombineFtpPath(currentPath, name);
                FtpWebRequest request = CreateRequest(ftpPath, WebRequestMethods.Ftp.DownloadFile);

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (FileStream file = new FileStream(dialog.FileName, FileMode.Create))
                {
                    stream.CopyTo(file);
                    Log(name + " скачано.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка");
            }
        }

        void BtnDeleteFile_Click(object sender, EventArgs e)
        {
            string name = GetSelectedName();
            if (name == "") return;

            if (MessageBox.Show("Видалити файл " + name + "?", "Підтвердження", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;

            try
            {
                string ftpPath = CombineFtpPath(currentPath, name);
                FtpWebRequest request = CreateRequest(ftpPath, WebRequestMethods.Ftp.DeleteFile);

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                    Log(response.StatusDescription.Trim());

                LoadFtpList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка");
            }
        }

        void BtnCreateFolder_Click(object sender, EventArgs e)
        {
            string name = Input("Створити каталог", "Назва каталогу:");
            if (name == "") return;

            try
            {
                string ftpPath = CombineFtpPath(currentPath, name);
                FtpWebRequest request = CreateRequest(ftpPath, WebRequestMethods.Ftp.MakeDirectory);

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                    Log(response.StatusDescription.Trim());

                LoadFtpList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка");
            }
        }

        void BtnDeleteFolder_Click(object sender, EventArgs e)
        {
            string name = GetSelectedName();
            if (name == "") return;

            if (MessageBox.Show("Видалити каталог " + name + "?", "Підтвердження", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;

            try
            {
                string ftpPath = CombineFtpPath(currentPath, name);
                FtpWebRequest request = CreateRequest(ftpPath, WebRequestMethods.Ftp.RemoveDirectory);

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                    Log(response.StatusDescription.Trim());

                LoadFtpList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка");
            }
        }

        void BtnRename_Click(object sender, EventArgs e)
        {
            string oldName = GetSelectedName();
            if (oldName == "") return;

            string newName = tbNewName.Text.Trim();

            if (newName == "")
            {
                MessageBox.Show("Введи нову назву у поле зверху.");
                return;
            }

            try
            {
                string ftpPath = CombineFtpPath(currentPath, oldName);
                FtpWebRequest request = CreateRequest(ftpPath, WebRequestMethods.Ftp.Rename);
                request.RenameTo = newName;

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                    Log(response.StatusDescription.Trim());

                tbNewName.Clear();
                LoadFtpList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка");
            }
        }

        void BtnSize_Click(object sender, EventArgs e)
        {
            string name = GetSelectedName();
            if (name == "") return;

            try
            {
                string ftpPath = CombineFtpPath(currentPath, name);
                FtpWebRequest request = CreateRequest(ftpPath, WebRequestMethods.Ftp.GetFileSize);

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    long size = response.ContentLength;
                    MessageBox.Show("Розмір файлу: " + SizeText(size));
                    Log("Розмір " + name + ": " + SizeText(size));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка");
            }
        }

        void FtpList_DoubleClick(object sender, EventArgs e)
        {
            string name = GetSelectedName();
            if (name == "") return;

            string line = ftpList.SelectedItem.ToString().ToLower();

            if (line.StartsWith("d") || line.Contains("<dir>"))
            {
                currentPath = CombineFtpPath(currentPath, name);
                LoadFtpList();
            }
        }

        void BtnSaveSettings_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }

        void BtnLoadSettings_Click(object sender, EventArgs e)
        {
            LoadSettings(true);
        }

        void SaveSettings()
        {
            try
            {
                string path = Path.Combine(Application.StartupPath, "ftp_settings.txt");

                string[] lines =
                {
                    tbHost.Text,
                    tbUser.Text,
                    tbPass.Text,
                    tbRemotePath.Text
                };

                File.WriteAllLines(path, lines, Encoding.UTF8);
                MessageBox.Show("Налаштування збережено.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка");
            }
        }

        void LoadSettings(bool showMessage)
        {
            try
            {
                string path = Path.Combine(Application.StartupPath, "ftp_settings.txt");

                if (!File.Exists(path))
                    return;

                string[] lines = File.ReadAllLines(path, Encoding.UTF8);

                if (lines.Length > 0) tbHost.Text = lines[0];
                if (lines.Length > 1) tbUser.Text = lines[1];
                if (lines.Length > 2) tbPass.Text = lines[2];
                if (lines.Length > 3) tbRemotePath.Text = lines[3];

                if (showMessage)
                    MessageBox.Show("Налаштування завантажено.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка");
            }
        }

        string Input(string title, string text)
        {
            Form f = new Form();
            f.Text = title;
            f.Size = new Size(400, 165);
            f.StartPosition = FormStartPosition.CenterParent;
            f.FormBorderStyle = FormBorderStyle.FixedDialog;
            f.MaximizeBox = false;
            f.MinimizeBox = false;

            Label label = new Label();
            label.Text = text;
            label.Location = new Point(20, 20);
            label.Size = new Size(330, 25);
            f.Controls.Add(label);

            TextBox box = new TextBox();
            box.Location = new Point(20, 50);
            box.Size = new Size(340, 28);
            f.Controls.Add(box);

            Button ok = MakeButton("OK", 190, 90, 75, 32);
            ok.Parent = f;
            ok.DialogResult = DialogResult.OK;

            Button cancel = MakeButton("Скасувати", 275, 90, 85, 32);
            cancel.Parent = f;
            cancel.DialogResult = DialogResult.Cancel;

            f.AcceptButton = ok;
            f.CancelButton = cancel;

            if (f.ShowDialog() == DialogResult.OK)
                return box.Text.Trim();

            return "";
        }

        void Log(string text)
        {
            tbLog.Text = text;
            statusLabel.Text = text;
        }

        string SizeText(long bytes)
        {
            if (bytes < 1024) return bytes + " B";
            if (bytes < 1024 * 1024) return (bytes / 1024.0).ToString("0.00") + " KB";
            if (bytes < 1024 * 1024 * 1024) return (bytes / 1024.0 / 1024.0).ToString("0.00") + " MB";
            return (bytes / 1024.0 / 1024.0 / 1024.0).ToString("0.00") + " GB";
        }
    }
}