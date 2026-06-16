using System;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows.Forms;

namespace Laboratorna28
{
    public partial class Form1 : Form
    {
        TreeView tree;
        ListView list;
        RichTextBox previewText;
        PictureBox previewImage;
        TextBox filterFiles;
        TextBox filterFolders;
        TextBox pathBox;
        TextBox propsBox;
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
            Text = "Laboratorna28 - файловий менеджер";
            Width = 1250;
            Height = 760;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 10);

            Panel top = new Panel();
            top.Dock = DockStyle.Top;
            top.Height = 70;
            top.BackColor = Color.FromArgb(35, 45, 65);
            Controls.Add(top);

            Label title = new Label();
            title.Text = "Laboratorna28: Робота з файловою системою";
            title.ForeColor = Color.White;
            title.Font = new Font("Segoe UI Semibold", 16);
            title.Location = new Point(15, 15);
            title.Size = new Size(520, 35);
            top.Controls.Add(title);

            pathBox = new TextBox();
            pathBox.Location = new Point(550, 20);
            pathBox.Size = new Size(500, 30);
            top.Controls.Add(pathBox);

            Button goBtn = ButtonMake("Перейти", 1065, 18, 120, 34);
            goBtn.Parent = top;
            goBtn.Click += GoBtn_Click;

            Panel left = new Panel();
            left.Location = new Point(10, 80);
            left.Size = new Size(270, 610);
            left.BackColor = Color.White;
            left.BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(left);

            tree = new TreeView();
            tree.Location = new Point(10, 10);
            tree.Size = new Size(248, 590);
            tree.AfterSelect += Tree_AfterSelect;
            tree.BeforeExpand += Tree_BeforeExpand;
            left.Controls.Add(tree);

            Panel center = new Panel();
            center.Location = new Point(290, 80);
            center.Size = new Size(570, 610);
            center.BackColor = Color.White;
            center.BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(center);

            Label lf = LabelMake("Фільтр файлів:", 10, 10, 120, 25);
            center.Controls.Add(lf);

            filterFiles = new TextBox();
            filterFiles.Location = new Point(130, 10);
            filterFiles.Size = new Size(140, 28);
            filterFiles.TextChanged += Filter_TextChanged;
            center.Controls.Add(filterFiles);

            Label ld = LabelMake("Фільтр папок:", 285, 10, 120, 25);
            center.Controls.Add(ld);

            filterFolders = new TextBox();
            filterFolders.Location = new Point(405, 10);
            filterFolders.Size = new Size(145, 28);
            filterFolders.TextChanged += Filter_TextChanged;
            center.Controls.Add(filterFolders);

            list = new ListView();
            list.Location = new Point(10, 50);
            list.Size = new Size(540, 370);
            list.View = View.Details;
            list.FullRowSelect = true;
            list.GridLines = true;
            list.HideSelection = false;
            list.Columns.Add("Назва", 250);
            list.Columns.Add("Тип", 100);
            list.Columns.Add("Розмір", 100);
            list.Columns.Add("Змінено", 160);
            list.DoubleClick += List_DoubleClick;
            list.SelectedIndexChanged += List_SelectedIndexChanged;
            center.Controls.Add(list);

            Button createFile = ButtonMake("Створити файл", 10, 435, 130, 36);
            createFile.Parent = center;
            createFile.Click += CreateFile_Click;

            Button createFolder = ButtonMake("Створити папку", 150, 435, 145, 36);
            createFolder.Parent = center;
            createFolder.Click += CreateFolder_Click;

            Button delete = ButtonMake("Видалити", 305, 435, 115, 36);
            delete.Parent = center;
            delete.Click += Delete_Click;

            Button copy = ButtonMake("Копіювати", 430, 435, 120, 36);
            copy.Parent = center;
            copy.Click += Copy_Click;

            Button move = ButtonMake("Перемістити", 10, 480, 130, 36);
            move.Parent = center;
            move.Click += Move_Click;

            Button zip = ButtonMake("ZIP", 150, 480, 80, 36);
            zip.Parent = center;
            zip.Click += Zip_Click;

            Button unzip = ButtonMake("UNZIP", 240, 480, 90, 36);
            unzip.Parent = center;
            unzip.Click += Unzip_Click;

            Button attr = ButtonMake("Атрибути", 340, 480, 100, 36);
            attr.Parent = center;
            attr.Click += Attributes_Click;

            Button refresh = ButtonMake("Оновити", 450, 480, 100, 36);
            refresh.Parent = center;
            refresh.Click += Refresh_Click;

            propsBox = new TextBox();
            propsBox.Location = new Point(10, 530);
            propsBox.Size = new Size(540, 65);
            propsBox.Multiline = true;
            propsBox.ReadOnly = true;
            propsBox.ScrollBars = ScrollBars.Vertical;
            center.Controls.Add(propsBox);

            Panel right = new Panel();
            right.Location = new Point(870, 80);
            right.Size = new Size(350, 610);
            right.BackColor = Color.White;
            right.BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(right);

            Label prevTitle = LabelMake("Попередній перегляд", 10, 10, 250, 25);
            prevTitle.Font = new Font("Segoe UI Semibold", 11);
            right.Controls.Add(prevTitle);

            previewImage = new PictureBox();
            previewImage.Location = new Point(10, 90);
            previewImage.Size = new Size(330, 215);
            previewImage.BorderStyle = BorderStyle.FixedSingle;
            previewImage.SizeMode = PictureBoxSizeMode.Zoom;
            right.Controls.Add(previewImage);

            previewText = new RichTextBox();
            previewText.Location = new Point(10, 305);
            previewText.Size = new Size(330, 290);
            previewText.ReadOnly = false;
            previewText.BorderStyle = BorderStyle.FixedSingle;
            right.Controls.Add(previewText);

            Button saveText = ButtonMake("Зберегти текст", 185, 45, 155, 34);
            saveText.Parent = right;
            saveText.Click += SaveText_Click;

            status = new StatusStrip();
            statusLabel = new ToolStripStatusLabel("Готово");
            status.Items.Add(statusLabel);
            Controls.Add(status);
        }

        Button ButtonMake(string text, int x, int y, int w, int h)
        {
            Button b = new Button();
            b.Text = text;
            b.Location = new Point(x, y);
            b.Size = new Size(w, h);
            b.BackColor = Color.FromArgb(45, 95, 170);
            b.ForeColor = Color.White;
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0;
            return b;
        }

        Label LabelMake(string text, int x, int y, int w, int h)
        {
            Label l = new Label();
            l.Text = text;
            l.Location = new Point(x, y);
            l.Size = new Size(w, h);
            l.ForeColor = Color.FromArgb(35, 45, 65);
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
            LoadSubFolders(e.Node);
        }

        void Tree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            string path = Convert.ToString(e.Node.Tag);
            if (Directory.Exists(path))
                OpenFolder(path);
        }

        void LoadSubFolders(TreeNode node)
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
                pathBox.Text = path;
                list.Items.Clear();
                previewText.Clear();
                previewImage.Image = null;

                string folderFilter = string.IsNullOrWhiteSpace(filterFolders.Text) ? "*" : filterFolders.Text;
                string fileFilter = string.IsNullOrWhiteSpace(filterFiles.Text) ? "*" : filterFiles.Text;

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

                ShowProperties(path);
                statusLabel.Text = "Відкрито: " + path;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка");
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

        void List_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (list.SelectedItems.Count == 0) return;

            string path = Convert.ToString(list.SelectedItems[0].Tag);
            ShowProperties(path);

            if (File.Exists(path))
                PreviewFile(path);
        }

        void PreviewFile(string path)
        {
            previewText.Clear();
            previewImage.Image = null;

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

        void ShowProperties(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    FileInfo f = new FileInfo(path);
                    propsBox.Text =
                        "Файл: " + f.Name + Environment.NewLine +
                        "Повний шлях: " + f.FullName + Environment.NewLine +
                        "Розмір: " + SizeText(f.Length) + Environment.NewLine +
                        "Створено: " + f.CreationTime + Environment.NewLine +
                        "Змінено: " + f.LastWriteTime + Environment.NewLine +
                        "Атрибути: " + f.Attributes;
                }
                else if (Directory.Exists(path))
                {
                    DirectoryInfo d = new DirectoryInfo(path);
                    propsBox.Text =
                        "Папка: " + d.Name + Environment.NewLine +
                        "Повний шлях: " + d.FullName + Environment.NewLine +
                        "Створено: " + d.CreationTime + Environment.NewLine +
                        "Змінено: " + d.LastWriteTime + Environment.NewLine +
                        "Атрибути: " + d.Attributes;
                }
                else
                {
                    DriveInfo drive = new DriveInfo(path);
                    if (drive.IsReady)
                    {
                        propsBox.Text =
                            "Диск: " + drive.Name + Environment.NewLine +
                            "Тип: " + drive.DriveType + Environment.NewLine +
                            "Файлова система: " + drive.DriveFormat + Environment.NewLine +
                            "Всього: " + SizeText(drive.TotalSize) + Environment.NewLine +
                            "Вільно: " + SizeText(drive.TotalFreeSpace);
                    }
                }
            }
            catch
            {
            }
        }

        void GoBtn_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(pathBox.Text))
                OpenFolder(pathBox.Text);
            else
                MessageBox.Show("Папку не знайдено.");
        }

        void Filter_TextChanged(object sender, EventArgs e)
        {
            if (Directory.Exists(currentPath))
                OpenFolder(currentPath);
        }

        void CreateFile_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(currentPath)) return;

            string name = Input("Назва файлу", "Введи назву файлу:");
            if (string.IsNullOrWhiteSpace(name)) return;

            string path = Path.Combine(currentPath, name);

            if (!Path.HasExtension(path))
                path += ".txt";

            File.WriteAllText(path, "");
            OpenFolder(currentPath);
        }

        void CreateFolder_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(currentPath)) return;

            string name = Input("Назва папки", "Введи назву папки:");
            if (string.IsNullOrWhiteSpace(name)) return;

            Directory.CreateDirectory(Path.Combine(currentPath, name));
            OpenFolder(currentPath);
        }

        void Delete_Click(object sender, EventArgs e)
        {
            string path = SelectedPath();
            if (path == "") return;

            if (MessageBox.Show("Видалити вибраний елемент?", "Підтвердження", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;

            if (File.Exists(path))
                File.Delete(path);
            else if (Directory.Exists(path))
                Directory.Delete(path, true);

            OpenFolder(currentPath);
        }

        void Copy_Click(object sender, EventArgs e)
        {
            string path = SelectedPath();
            if (path == "") return;

            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() != DialogResult.OK) return;

                if (File.Exists(path))
                {
                    string dest = Path.Combine(dialog.SelectedPath, Path.GetFileName(path));
                    File.Copy(path, dest, true);
                }
                else if (Directory.Exists(path))
                {
                    string dest = Path.Combine(dialog.SelectedPath, Path.GetFileName(path));
                    CopyDirectory(path, dest);
                }
            }

            OpenFolder(currentPath);
        }

        void Move_Click(object sender, EventArgs e)
        {
            string path = SelectedPath();
            if (path == "") return;

            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() != DialogResult.OK) return;

                if (File.Exists(path))
                {
                    string dest = Path.Combine(dialog.SelectedPath, Path.GetFileName(path));
                    File.Move(path, dest);
                }
                else if (Directory.Exists(path))
                {
                    string dest = Path.Combine(dialog.SelectedPath, Path.GetFileName(path));
                    Directory.Move(path, dest);
                }
            }

            OpenFolder(currentPath);
        }

        void Zip_Click(object sender, EventArgs e)
        {
            string path = SelectedPath();
            if (path == "") return;

            if (Directory.Exists(path))
            {
                string zipPath = path.TrimEnd('\\') + ".zip";
                if (File.Exists(zipPath)) File.Delete(zipPath);
                ZipFile.CreateFromDirectory(path, zipPath);
                MessageBox.Show("ZIP створено.");
            }
            else
            {
                MessageBox.Show("ZIP можна створити тільки з папки.");
            }

            OpenFolder(currentPath);
        }

        void Unzip_Click(object sender, EventArgs e)
        {
            string path = SelectedPath();
            if (path == "") return;

            if (File.Exists(path) && Path.GetExtension(path).ToLower() == ".zip")
            {
                string folder = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                ZipFile.ExtractToDirectory(path, folder);
                MessageBox.Show("Архів розпаковано.");
            }
            else
            {
                MessageBox.Show("Вибери ZIP файл.");
            }

            OpenFolder(currentPath);
        }

        void Attributes_Click(object sender, EventArgs e)
        {
            string path = SelectedPath();
            if (path == "") return;

            FileAttributes attrs = File.GetAttributes(path);

            Form f = new Form();
            f.Text = "Атрибути";
            f.Size = new Size(300, 260);
            f.StartPosition = FormStartPosition.CenterParent;
            f.FormBorderStyle = FormBorderStyle.FixedDialog;
            f.MaximizeBox = false;
            f.MinimizeBox = false;

            CheckBox readOnly = new CheckBox();
            readOnly.Text = "ReadOnly";
            readOnly.Location = new Point(25, 25);
            readOnly.Size = new Size(200, 25);
            readOnly.Checked = attrs.HasFlag(FileAttributes.ReadOnly);
            f.Controls.Add(readOnly);

            CheckBox hidden = new CheckBox();
            hidden.Text = "Hidden";
            hidden.Location = new Point(25, 60);
            hidden.Size = new Size(200, 25);
            hidden.Checked = attrs.HasFlag(FileAttributes.Hidden);
            f.Controls.Add(hidden);

            CheckBox archive = new CheckBox();
            archive.Text = "Archive";
            archive.Location = new Point(25, 95);
            archive.Size = new Size(200, 25);
            archive.Checked = attrs.HasFlag(FileAttributes.Archive);
            f.Controls.Add(archive);

            Button ok = ButtonMake("Зберегти", 25, 145, 220, 35);
            ok.Parent = f;
            ok.Click += delegate
            {
                FileAttributes newAttrs = 0;

                if (readOnly.Checked) newAttrs |= FileAttributes.ReadOnly;
                if (hidden.Checked) newAttrs |= FileAttributes.Hidden;
                if (archive.Checked) newAttrs |= FileAttributes.Archive;
                if (Directory.Exists(path)) newAttrs |= FileAttributes.Directory;

                File.SetAttributes(path, newAttrs);
                f.Close();
                OpenFolder(currentPath);
            };

            f.ShowDialog();
        }

        void Refresh_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(currentPath))
                OpenFolder(currentPath);
            else
                LoadDrives();
        }

        void SaveText_Click(object sender, EventArgs e)
        {
            string path = SelectedPath();
            if (path == "" || !File.Exists(path)) return;

            string ext = Path.GetExtension(path).ToLower();

            if (ext == ".txt" || ext == ".cs" || ext == ".log" || ext == ".xml" || ext == ".json" || ext == ".html" || ext == ".css")
            {
                File.WriteAllText(path, previewText.Text);
                MessageBox.Show("Текст збережено.");
            }
            else
            {
                MessageBox.Show("Цей файл не можна редагувати як текст.");
            }
        }

        string SelectedPath()
        {
            if (list.SelectedItems.Count == 0)
            {
                MessageBox.Show("Вибери файл або папку.");
                return "";
            }

            return Convert.ToString(list.SelectedItems[0].Tag);
        }

        string Input(string title, string text)
        {
            Form f = new Form();
            f.Text = title;
            f.Size = new Size(420, 170);
            f.StartPosition = FormStartPosition.CenterParent;
            f.FormBorderStyle = FormBorderStyle.FixedDialog;
            f.MaximizeBox = false;
            f.MinimizeBox = false;

            Label l = new Label();
            l.Text = text;
            l.Location = new Point(20, 20);
            l.Size = new Size(360, 25);
            f.Controls.Add(l);

            TextBox t = new TextBox();
            t.Location = new Point(20, 50);
            t.Size = new Size(360, 30);
            f.Controls.Add(t);

            Button ok = ButtonMake("OK", 220, 90, 75, 32);
            ok.Parent = f;
            ok.DialogResult = DialogResult.OK;

            Button cancel = ButtonMake("Скасувати", 305, 90, 75, 32);
            cancel.Parent = f;
            cancel.DialogResult = DialogResult.Cancel;

            f.AcceptButton = ok;
            f.CancelButton = cancel;

            if (f.ShowDialog() == DialogResult.OK)
                return t.Text;

            return "";
        }

        void CopyDirectory(string source, string destination)
        {
            Directory.CreateDirectory(destination);

            foreach (string file in Directory.GetFiles(source))
            {
                string dest = Path.Combine(destination, Path.GetFileName(file));
                File.Copy(file, dest, true);
            }

            foreach (string dir in Directory.GetDirectories(source))
            {
                string dest = Path.Combine(destination, Path.GetFileName(dir));
                CopyDirectory(dir, dest);
            }
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