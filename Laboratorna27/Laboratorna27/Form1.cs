using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Laboratorna27
{
    public partial class Form1 : Form
    {
        TreeView tree;
        ListView list;
        PictureBox previewImage;
        RichTextBox previewText;
        TextBox txtPath;
        TextBox txtFileFilter;
        TextBox txtFolderFilter;
        TextBox txtProperties;
        StatusStrip status;
        ToolStripStatusLabel statusLabel;

        string currentPath = "";

        public Form1()
        {
            InitializeComponent();
            BuildInterface();
            LoadDrives();
        }

        void BuildInterface()
        {
            Text = "Laboratorna27 - перегляд файлової системи";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(1180, 720);
            MinimumSize = new Size(1180, 720);
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 10);

            Panel top = new Panel();
            top.Dock = DockStyle.Top;
            top.Height = 75;
            top.BackColor = Color.FromArgb(31, 44, 64);
            Controls.Add(top);

            Label title = new Label();
            title.Text = "Laboratorna27: Робота з файловою системою";
            title.ForeColor = Color.White;
            title.Font = new Font("Segoe UI Semibold", 16);
            title.Location = new Point(18, 18);
            title.Size = new Size(520, 35);
            top.Controls.Add(title);

            txtPath = new TextBox();
            txtPath.Location = new Point(545, 22);
            txtPath.Size = new Size(460, 30);
            top.Controls.Add(txtPath);

            Button btnGo = MakeButton("Перейти", 1020, 20, 120, 34);
            btnGo.Parent = top;
            btnGo.Click += BtnGo_Click;

            Panel left = new Panel();
            left.Location = new Point(10, 90);
            left.Size = new Size(260, 555);
            left.BackColor = Color.White;
            left.BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(left);

            tree = new TreeView();
            tree.Location = new Point(10, 10);
            tree.Size = new Size(238, 532);
            tree.BeforeExpand += Tree_BeforeExpand;
            tree.AfterSelect += Tree_AfterSelect;
            left.Controls.Add(tree);

            Panel center = new Panel();
            center.Location = new Point(280, 90);
            center.Size = new Size(540, 555);
            center.BackColor = Color.White;
            center.BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(center);

            Label lblFileFilter = MakeLabel("Фільтр файлів:", 12, 13, 110, 25);
            center.Controls.Add(lblFileFilter);

            txtFileFilter = new TextBox();
            txtFileFilter.Location = new Point(125, 10);
            txtFileFilter.Size = new Size(130, 28);
            txtFileFilter.TextChanged += FilterChanged;
            center.Controls.Add(txtFileFilter);

            Label lblFolderFilter = MakeLabel("Фільтр папок:", 270, 13, 110, 25);
            center.Controls.Add(lblFolderFilter);

            txtFolderFilter = new TextBox();
            txtFolderFilter.Location = new Point(385, 10);
            txtFolderFilter.Size = new Size(130, 28);
            txtFolderFilter.TextChanged += FilterChanged;
            center.Controls.Add(txtFolderFilter);

            list = new ListView();
            list.Location = new Point(12, 50);
            list.Size = new Size(505, 330);
            list.View = View.Details;
            list.FullRowSelect = true;
            list.GridLines = true;
            list.HideSelection = false;
            list.Columns.Add("Назва", 230);
            list.Columns.Add("Тип", 90);
            list.Columns.Add("Розмір", 85);
            list.Columns.Add("Змінено", 150);
            list.SelectedIndexChanged += List_SelectedIndexChanged;
            list.DoubleClick += List_DoubleClick;
            center.Controls.Add(list);

            txtProperties = new TextBox();
            txtProperties.Location = new Point(12, 395);
            txtProperties.Size = new Size(505, 145);
            txtProperties.Multiline = true;
            txtProperties.ReadOnly = true;
            txtProperties.ScrollBars = ScrollBars.Vertical;
            center.Controls.Add(txtProperties);

            Panel right = new Panel();
            right.Location = new Point(830, 90);
            right.Size = new Size(330, 555);
            right.BackColor = Color.White;
            right.BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(right);

            Label lblPreview = MakeLabel("Попередній перегляд", 12, 12, 250, 25);
            lblPreview.Font = new Font("Segoe UI Semibold", 11);
            right.Controls.Add(lblPreview);

            previewImage = new PictureBox();
            previewImage.Location = new Point(12, 50);
            previewImage.Size = new Size(305, 225);
            previewImage.BorderStyle = BorderStyle.FixedSingle;
            previewImage.SizeMode = PictureBoxSizeMode.Zoom;
            right.Controls.Add(previewImage);

            previewText = new RichTextBox();
            previewText.Location = new Point(12, 290);
            previewText.Size = new Size(305, 250);
            previewText.ReadOnly = true;
            previewText.BorderStyle = BorderStyle.FixedSingle;
            right.Controls.Add(previewText);

            status = new StatusStrip();
            statusLabel = new ToolStripStatusLabel("Готово");
            status.Items.Add(statusLabel);
            Controls.Add(status);
        }

        Button MakeButton(string text, int x, int y, int w, int h)
        {
            Button b = new Button();
            b.Text = text;
            b.Location = new Point(x, y);
            b.Size = new Size(w, h);
            b.BackColor = Color.FromArgb(48, 92, 160);
            b.ForeColor = Color.White;
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0;
            return b;
        }

        Label MakeLabel(string text, int x, int y, int w, int h)
        {
            Label l = new Label();
            l.Text = text;
            l.Location = new Point(x, y);
            l.Size = new Size(w, h);
            l.ForeColor = Color.FromArgb(30, 40, 60);
            return l;
        }

        void LoadDrives()
        {
            tree.Nodes.Clear();

            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                TreeNode node = new TreeNode(drive.Name);
                node.Tag = drive.Name;
                node.Nodes.Add("...");
                tree.Nodes.Add(node);
            }

            statusLabel.Text = "Диски завантажено";
        }

        void Tree_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            LoadFolders(e.Node);
        }

        void Tree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            string path = Convert.ToString(e.Node.Tag);

            if (Directory.Exists(path))
            {
                OpenFolder(path);
            }
            else
            {
                ShowDriveProperties(path);
            }
        }

        void LoadFolders(TreeNode node)
        {
            string path = Convert.ToString(node.Tag);
            node.Nodes.Clear();

            try
            {
                foreach (string dir in Directory.GetDirectories(path))
                {
                    DirectoryInfo info = new DirectoryInfo(dir);

                    TreeNode child = new TreeNode(info.Name);
                    child.Tag = info.FullName;
                    child.Nodes.Add("...");
                    node.Nodes.Add(child);
                }
            }
            catch
            {
            }
        }

        void OpenFolder(string path)
        {
            try
            {
                currentPath = path;
                txtPath.Text = path;
                list.Items.Clear();
                previewImage.Image = null;
                previewText.Clear();

                string folderFilter = string.IsNullOrWhiteSpace(txtFolderFilter.Text) ? "*" : txtFolderFilter.Text;
                string fileFilter = string.IsNullOrWhiteSpace(txtFileFilter.Text) ? "*" : txtFileFilter.Text;

                foreach (string dir in Directory.GetDirectories(path, folderFilter))
                {
                    DirectoryInfo d = new DirectoryInfo(dir);

                    ListViewItem item = new ListViewItem(d.Name);
                    item.SubItems.Add("Папка");
                    item.SubItems.Add("");
                    item.SubItems.Add(d.LastWriteTime.ToString());
                    item.Tag = d.FullName;
                    list.Items.Add(item);
                }

                foreach (string file in Directory.GetFiles(path, fileFilter))
                {
                    FileInfo f = new FileInfo(file);

                    ListViewItem item = new ListViewItem(f.Name);
                    item.SubItems.Add(f.Extension);
                    item.SubItems.Add(SizeText(f.Length));
                    item.SubItems.Add(f.LastWriteTime.ToString());
                    item.Tag = f.FullName;
                    list.Items.Add(item);
                }

                ShowFolderProperties(path);
                statusLabel.Text = "Відкрито: " + path;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка");
            }
        }

        void List_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (list.SelectedItems.Count == 0) return;

            string path = Convert.ToString(list.SelectedItems[0].Tag);

            if (File.Exists(path))
            {
                ShowFileProperties(path);
                PreviewFile(path);
            }
            else if (Directory.Exists(path))
            {
                ShowFolderProperties(path);
                previewImage.Image = null;
                previewText.Clear();
            }
        }

        void List_DoubleClick(object sender, EventArgs e)
        {
            if (list.SelectedItems.Count == 0) return;

            string path = Convert.ToString(list.SelectedItems[0].Tag);

            if (Directory.Exists(path))
                OpenFolder(path);
            else if (File.Exists(path))
                PreviewFile(path);
        }

        void PreviewFile(string path)
        {
            previewImage.Image = null;
            previewText.Clear();

            string ext = Path.GetExtension(path).ToLower();

            try
            {
                if (ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".bmp" || ext == ".gif")
                {
                    using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                    {
                        previewImage.Image = Image.FromStream(fs);
                    }
                }
                else if (ext == ".txt" || ext == ".cs" || ext == ".log" || ext == ".xml" || ext == ".json" || ext == ".html" || ext == ".css")
                {
                    previewText.Text = File.ReadAllText(path);
                }
                else
                {
                    previewText.Text = "Перегляд цього типу файлу не підтримується.";
                }
            }
            catch (Exception ex)
            {
                previewText.Text = ex.Message;
            }
        }

        void ShowDriveProperties(string path)
        {
            try
            {
                DriveInfo d = new DriveInfo(path);

                if (!d.IsReady)
                {
                    txtProperties.Text = "Диск не готовий.";
                    return;
                }

                txtProperties.Text =
                    "Диск: " + d.Name + Environment.NewLine +
                    "Тип: " + d.DriveType + Environment.NewLine +
                    "Файлова система: " + d.DriveFormat + Environment.NewLine +
                    "Мітка: " + d.VolumeLabel + Environment.NewLine +
                    "Загальний розмір: " + SizeText(d.TotalSize) + Environment.NewLine +
                    "Вільно: " + SizeText(d.TotalFreeSpace);
            }
            catch
            {
                txtProperties.Text = "Не вдалося прочитати властивості диска.";
            }
        }

        void ShowFolderProperties(string path)
        {
            try
            {
                DirectoryInfo d = new DirectoryInfo(path);

                txtProperties.Text =
                    "Папка: " + d.Name + Environment.NewLine +
                    "Повний шлях: " + d.FullName + Environment.NewLine +
                    "Корінь: " + d.Root + Environment.NewLine +
                    "Створено: " + d.CreationTime + Environment.NewLine +
                    "Останній доступ: " + d.LastAccessTime + Environment.NewLine +
                    "Змінено: " + d.LastWriteTime + Environment.NewLine +
                    "Атрибути: " + d.Attributes;
            }
            catch
            {
                txtProperties.Text = "Не вдалося прочитати властивості папки.";
            }
        }

        void ShowFileProperties(string path)
        {
            try
            {
                FileInfo f = new FileInfo(path);

                txtProperties.Text =
                    "Файл: " + f.Name + Environment.NewLine +
                    "Повний шлях: " + f.FullName + Environment.NewLine +
                    "Розширення: " + f.Extension + Environment.NewLine +
                    "Розмір: " + SizeText(f.Length) + Environment.NewLine +
                    "Створено: " + f.CreationTime + Environment.NewLine +
                    "Останній доступ: " + f.LastAccessTime + Environment.NewLine +
                    "Змінено: " + f.LastWriteTime + Environment.NewLine +
                    "Атрибути: " + f.Attributes;
            }
            catch
            {
                txtProperties.Text = "Не вдалося прочитати властивості файлу.";
            }
        }

        void BtnGo_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(txtPath.Text))
                OpenFolder(txtPath.Text);
            else
                MessageBox.Show("Папку не знайдено.");
        }

        void FilterChanged(object sender, EventArgs e)
        {
            if (Directory.Exists(currentPath))
                OpenFolder(currentPath);
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