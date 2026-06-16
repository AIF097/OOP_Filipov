    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;

    namespace Laboratorna31
    {
        public partial class Form1 : Form
        {
            DataGridView grid;
            RichTextBox detailsBox;
            ContextMenuStrip menu;
            StatusStrip status;
            ToolStripStatusLabel statusLabel;

            public Form1()
            {
                InitializeComponent();
                BuildInterface();
                LoadProcesses();
            }

            void BuildInterface()
            {
                Text = "Laboratorna31 - Process Monitor";
                StartPosition = FormStartPosition.CenterScreen;
                Size = new Size(1220, 760);
                MinimumSize = new Size(1220, 760);
                BackColor = Color.FromArgb(18, 24, 34);
                Font = new Font("Segoe UI", 10);

                Panel side = new Panel();
                side.Dock = DockStyle.Left;
                side.Width = 230;
                side.BackColor = Color.FromArgb(24, 33, 47);
                Controls.Add(side);

                Label title = new Label();
                title.Text = "PROCESS\nMONITOR";
                title.ForeColor = Color.White;
                title.Font = new Font("Segoe UI Semibold", 22);
                title.Location = new Point(22, 25);
                title.Size = new Size(190, 80);
                side.Controls.Add(title);

                Button refresh = MakeSideButton("Оновити список", 20, 140);
                refresh.Click += Refresh_Click;
                side.Controls.Add(refresh);

                Button export = MakeSideButton("Експорт TXT", 20, 195);
                export.Click += Export_Click;
                side.Controls.Add(export);

                Button openTaskmgr = MakeSideButton("Диспетчер задач", 20, 250);
                openTaskmgr.Click += OpenTaskmgr_Click;
                side.Controls.Add(openTaskmgr);

                Label hint = new Label();
                hint.Text = "ПКМ по процесу:\nінформація, потоки,\nмодулі, завершення";
                hint.ForeColor = Color.FromArgb(165, 175, 190);
                hint.Font = new Font("Segoe UI", 10);
                hint.Location = new Point(22, 330);
                hint.Size = new Size(180, 90);
                side.Controls.Add(hint);

                Panel header = new Panel();
                header.Location = new Point(250, 20);
                header.Size = new Size(940, 75);
                header.BackColor = Color.FromArgb(28, 38, 54);
                Controls.Add(header);

                Label headerTitle = new Label();
                headerTitle.Text = "Laboratorna31: запущені процеси Windows";
                headerTitle.ForeColor = Color.White;
                headerTitle.Font = new Font("Segoe UI Semibold", 18);
                headerTitle.Location = new Point(25, 18);
                headerTitle.Size = new Size(600, 35);
                header.Controls.Add(headerTitle);

                Label small = new Label();
                small.Text = "ID, назва, пам'ять, потоки, модулі";
                small.ForeColor = Color.FromArgb(165, 175, 190);
                small.Location = new Point(650, 26);
                small.Size = new Size(260, 25);
                header.Controls.Add(small);

                Panel tablePanel = new Panel();
                tablePanel.Location = new Point(250, 115);
                tablePanel.Size = new Size(610, 575);
                tablePanel.BackColor = Color.FromArgb(28, 38, 54);
                Controls.Add(tablePanel);

                Label tableTitle = new Label();
                tableTitle.Text = "Список процесів";
                tableTitle.ForeColor = Color.White;
                tableTitle.Font = new Font("Segoe UI Semibold", 12);
                tableTitle.Location = new Point(18, 15);
                tableTitle.Size = new Size(250, 25);
                tablePanel.Controls.Add(tableTitle);

                grid = new DataGridView();
                grid.Location = new Point(18, 55);
                grid.Size = new Size(575, 500);
                grid.BackgroundColor = Color.FromArgb(22, 30, 43);
                grid.BorderStyle = BorderStyle.None;
                grid.AllowUserToAddRows = false;
                grid.AllowUserToDeleteRows = false;
                grid.AllowUserToResizeRows = false;
                grid.AllowUserToResizeColumns = false;
                grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                grid.MultiSelect = false;
                grid.ReadOnly = true;
                grid.RowHeadersVisible = false;
                grid.GridColor = Color.FromArgb(45, 58, 78);
                grid.EnableHeadersVisualStyles = false;
                grid.ColumnHeadersHeight = 36;
                grid.RowTemplate.Height = 31;
                grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(35, 48, 68);
                grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(35, 48, 68);
                grid.DefaultCellStyle.BackColor = Color.FromArgb(22, 30, 43);
                grid.DefaultCellStyle.ForeColor = Color.White;
                grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(57, 100, 180);
                grid.DefaultCellStyle.SelectionForeColor = Color.White;
                grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(26, 36, 52);

                grid.Columns.Add("Id", "ID");
                grid.Columns.Add("Name", "Назва");
                grid.Columns.Add("Memory", "RAM");
                grid.Columns.Add("Threads", "Потоки");

                grid.Columns[0].Width = 75;
                grid.Columns[1].Width = 250;
                grid.Columns[2].Width = 120;
                grid.Columns[3].Width = 95;

                grid.SelectionChanged += Grid_SelectionChanged;
                tablePanel.Controls.Add(grid);

                Panel detailsPanel = new Panel();
                detailsPanel.Location = new Point(880, 115);
                detailsPanel.Size = new Size(310, 575);
                detailsPanel.BackColor = Color.FromArgb(28, 38, 54);
                Controls.Add(detailsPanel);

                Label detailsTitle = new Label();
                detailsTitle.Text = "Деталі процесу";
                detailsTitle.ForeColor = Color.White;
                detailsTitle.Font = new Font("Segoe UI Semibold", 12);
                detailsTitle.Location = new Point(18, 15);
                detailsTitle.Size = new Size(250, 25);
                detailsPanel.Controls.Add(detailsTitle);

                detailsBox = new RichTextBox();
                detailsBox.Location = new Point(18, 55);
                detailsBox.Size = new Size(275, 500);
                detailsBox.ReadOnly = true;
                detailsBox.BackColor = Color.FromArgb(22, 30, 43);
                detailsBox.ForeColor = Color.White;
                detailsBox.BorderStyle = BorderStyle.None;
                detailsBox.Font = new Font("Consolas", 10);
                detailsPanel.Controls.Add(detailsBox);

                menu = new ContextMenuStrip();

                ToolStripMenuItem info = new ToolStripMenuItem("Інформація");
                info.Click += Info_Click;
                menu.Items.Add(info);

                ToolStripMenuItem kill = new ToolStripMenuItem("Завершити процес");
                kill.Click += Kill_Click;
                menu.Items.Add(kill);

                ToolStripMenuItem threads = new ToolStripMenuItem("Потоки");
                threads.Click += Threads_Click;
                menu.Items.Add(threads);

                ToolStripMenuItem modules = new ToolStripMenuItem("Модулі");
                modules.Click += Modules_Click;
                menu.Items.Add(modules);

                grid.ContextMenuStrip = menu;

                status = new StatusStrip();
                status.BackColor = Color.FromArgb(24, 33, 47);
                status.ForeColor = Color.White;
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
                b.BackColor = Color.FromArgb(48, 95, 175);
                b.ForeColor = Color.White;
                b.FlatStyle = FlatStyle.Flat;
                b.FlatAppearance.BorderSize = 0;
                return b;

            }
            Button MakeSideButton(string text, int x, int y)
            {
                Button b = new Button();
                b.Text = text;
                b.Location = new Point(x, y);
                b.Size = new Size(185, 40);
                b.BackColor = Color.FromArgb(57, 100, 180);
                b.ForeColor = Color.White;
                b.FlatStyle = FlatStyle.Flat;
                b.FlatAppearance.BorderSize = 0;
                return b;
            }

            void LoadProcesses()
            {
                grid.Rows.Clear();

                Process[] processes = Process.GetProcesses();

                foreach (Process p in processes)
                {
                    try
                    {
                        long memory = p.WorkingSet64 / 1024 / 1024;

                        grid.Rows.Add(
                            p.Id,
                            p.ProcessName,
                            memory + " MB",
                            p.Threads.Count
                        );
                    }
                    catch
                    {
                    }
                }

                statusLabel.Text = "Процесів: " + grid.Rows.Count;
            }

            Process GetSelectedProcess()
            {
                if (grid.SelectedRows.Count == 0)
                    return null;

                try
                {
                    int id = Convert.ToInt32(grid.SelectedRows[0].Cells[0].Value);
                    return Process.GetProcessById(id);
                }
                catch
                {
                    return null;
                }
            }

            void Grid_SelectionChanged(object sender, EventArgs e)
            {
                ShowInfo();
            }

            void ShowInfo()
            {
                Process p = GetSelectedProcess();

                if (p == null)
                {
                    detailsBox.Clear();
                    return;
                }

                try
                {
                    detailsBox.Text =
                        "ID: " + p.Id + Environment.NewLine +
                        "Назва: " + p.ProcessName + Environment.NewLine +
                        "Машина: " + p.MachineName + Environment.NewLine +
                        "Пам'ять: " + (p.WorkingSet64 / 1024 / 1024) + " MB" + Environment.NewLine +
                        "Віртуальна пам'ять: " + (p.VirtualMemorySize64 / 1024 / 1024) + " MB" + Environment.NewLine +
                        "Потоки: " + p.Threads.Count + Environment.NewLine +
                        "Час запуску: " + p.StartTime + Environment.NewLine +
                        "Файл: " + p.MainModule.FileName;
                }
                catch
                {
                    detailsBox.Text = "Немає доступу до процесу.";
                }
            }

            void Info_Click(object sender, EventArgs e)
            {
                ShowInfo();
            }

            void Kill_Click(object sender, EventArgs e)
            {
                Process p = GetSelectedProcess();

                if (p == null) return;

                if (MessageBox.Show(
                    "Завершити процес " + p.ProcessName + "?",
                    "Підтвердження",
                    MessageBoxButtons.YesNo) != DialogResult.Yes)
                    return;

                try
                {
                    p.Kill();
                    LoadProcesses();
                    detailsBox.Clear();
                    statusLabel.Text = "Процес завершено";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Помилка");
                }
            }

            void Threads_Click(object sender, EventArgs e)
            {
                Process p = GetSelectedProcess();

                if (p == null) return;

                detailsBox.Clear();

                try
                {
                    detailsBox.AppendText("ПОТОКИ ПРОЦЕСУ" + Environment.NewLine);
                    detailsBox.AppendText("==================================" + Environment.NewLine);

                    foreach (ProcessThread t in p.Threads)
                    {
                        detailsBox.AppendText(
                            "ID: " + t.Id +
                            " | Пріоритет: " + t.PriorityLevel +
                            " | Час старту: " + t.StartTime +
                            Environment.NewLine
                        );
                    }
                }
                catch
                {
                    detailsBox.Text = "Немає доступу до потоків.";
                }
            }

            void Modules_Click(object sender, EventArgs e)
            {
                Process p = GetSelectedProcess();

                if (p == null) return;

                detailsBox.Clear();

                try
                {
                    detailsBox.AppendText("МОДУЛІ ПРОЦЕСУ" + Environment.NewLine);
                    detailsBox.AppendText("==================================" + Environment.NewLine);

                    foreach (ProcessModule m in p.Modules)
                    {
                        detailsBox.AppendText(
                            m.ModuleName +
                            Environment.NewLine +
                            m.FileName +
                            Environment.NewLine +
                            Environment.NewLine
                        );
                    }
                }
                catch
                {
                    detailsBox.Text = "Немає доступу до модулів.";
                }
            }

            void Refresh_Click(object sender, EventArgs e)
            {
                LoadProcesses();
            }

            void Export_Click(object sender, EventArgs e)
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = "Text file (*.txt)|*.txt";
                dialog.FileName = "processes.txt";

                if (dialog.ShowDialog() != DialogResult.OK)
                    return;

                try
                {
                    StringBuilder sb = new StringBuilder();

                    foreach (DataGridViewRow row in grid.Rows)
                    {
                        sb.AppendLine(
                            "ID: " + row.Cells[0].Value +
                            " | Процес: " + row.Cells[1].Value +
                            " | Пам'ять: " + row.Cells[2].Value +
                            " | Потоки: " + row.Cells[3].Value
                        );
                    }

                    File.WriteAllText(dialog.FileName, sb.ToString(), Encoding.UTF8);

                    statusLabel.Text = "Експорт виконано";
                    MessageBox.Show("Файл збережено.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Помилка");
                }
            }

            void OpenTaskmgr_Click(object sender, EventArgs e)
            {
                try
                {
                    Process.Start("taskmgr");
                }
                catch
                {
                }
            }
        }
    }