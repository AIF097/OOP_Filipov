using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Laboratorna21
{
    public partial class Form1 : Form
    {
        private MenuStrip menu;
        private ToolStrip tool;
        private StatusStrip status;
        private ToolStripStatusLabel statusInfo;
        private bool ukrainian = true;

        private ToolStripMenuItem fileMenu;
        private ToolStripMenuItem editMenu;
        private ToolStripMenuItem formatMenu;
        private ToolStripMenuItem languageMenu;
        private ToolStripMenuItem windowMenu;

        private ToolStripMenuItem newItem;
        private ToolStripMenuItem openItem;
        private ToolStripMenuItem saveItem;
        private ToolStripMenuItem saveAsItem;
        private ToolStripMenuItem closeItem;
        private ToolStripMenuItem exitItem;

        private ToolStripMenuItem copyItem;
        private ToolStripMenuItem pasteItem;
        private ToolStripMenuItem cutItem;
        private ToolStripMenuItem selectAllItem;

        private ToolStripMenuItem fontItem;
        private ToolStripMenuItem leftItem;
        private ToolStripMenuItem centerItem;
        private ToolStripMenuItem rightItem;

        private ToolStripMenuItem uaItem;
        private ToolStripMenuItem enItem;

        private ToolStripButton btnNew;
        private ToolStripButton btnOpen;
        private ToolStripButton btnSave;
        private ToolStripButton btnFont;
        private ToolStripButton btnLeft;
        private ToolStripButton btnCenter;
        private ToolStripButton btnRight;

        public Form1()
        {
            InitializeComponent();
            BuildInterface();
            ApplyLanguage();
        }

        private void BuildInterface()
        {
            Text = "Laboratorna21 - MDI RTF Editor";
            IsMdiContainer = true;
            WindowState = FormWindowState.Maximized;
            BackColor = Color.FromArgb(25, 25, 25);
            Font = new Font("Segoe UI", 10);

            menu = new MenuStrip();
            menu.BackColor = Color.FromArgb(35, 35, 35);
            menu.ForeColor = Color.White;

            fileMenu = new ToolStripMenuItem();
            editMenu = new ToolStripMenuItem();
            formatMenu = new ToolStripMenuItem();
            languageMenu = new ToolStripMenuItem();
            windowMenu = new ToolStripMenuItem();

            newItem = new ToolStripMenuItem("", null, NewDocument, Keys.Control | Keys.N);
            openItem = new ToolStripMenuItem("", null, OpenDocument, Keys.Control | Keys.O);
            saveItem = new ToolStripMenuItem("", null, SaveDocument, Keys.Control | Keys.S);
            saveAsItem = new ToolStripMenuItem("", null, SaveAsDocument);
            closeItem = new ToolStripMenuItem("", null, CloseDocument);
            exitItem = new ToolStripMenuItem("", null, ExitProgram);

            copyItem = new ToolStripMenuItem("", null, CopyText, Keys.Control | Keys.C);
            pasteItem = new ToolStripMenuItem("", null, PasteText, Keys.Control | Keys.V);
            cutItem = new ToolStripMenuItem("", null, CutText, Keys.Control | Keys.X);
            selectAllItem = new ToolStripMenuItem("", null, SelectAllText, Keys.Control | Keys.A);

            fontItem = new ToolStripMenuItem("", null, ChangeFont);
            leftItem = new ToolStripMenuItem("", null, AlignLeft);
            centerItem = new ToolStripMenuItem("", null, AlignCenter);
            rightItem = new ToolStripMenuItem("", null, AlignRight);

            uaItem = new ToolStripMenuItem("Українська", null, SetUkrainian);
            enItem = new ToolStripMenuItem("English", null, SetEnglish);

            fileMenu.DropDownItems.AddRange(new ToolStripItem[]
            {
                newItem, openItem, saveItem, saveAsItem,
                new ToolStripSeparator(), closeItem, exitItem
            });

            editMenu.DropDownItems.AddRange(new ToolStripItem[]
            {
                copyItem, pasteItem, cutItem, selectAllItem
            });

            formatMenu.DropDownItems.AddRange(new ToolStripItem[]
            {
                fontItem, new ToolStripSeparator(), leftItem, centerItem, rightItem
            });

            languageMenu.DropDownItems.AddRange(new ToolStripItem[]
            {
                uaItem, enItem
            });

            menu.Items.AddRange(new ToolStripItem[]
            {
                fileMenu, editMenu, formatMenu, languageMenu, windowMenu
            });

            MainMenuStrip = menu;
            Controls.Add(menu);

            tool = new ToolStrip();
            tool.BackColor = Color.FromArgb(45, 45, 45);
            tool.ForeColor = Color.White;
            tool.GripStyle = ToolStripGripStyle.Hidden;

            btnNew = CreateToolButton("New", NewDocument);
            btnOpen = CreateToolButton("Open", OpenDocument);
            btnSave = CreateToolButton("Save", SaveDocument);
            btnFont = CreateToolButton("Font", ChangeFont);
            btnLeft = CreateToolButton("Left", AlignLeft);
            btnCenter = CreateToolButton("Center", AlignCenter);
            btnRight = CreateToolButton("Right", AlignRight);

            tool.Items.AddRange(new ToolStripItem[]
            {
                btnNew, btnOpen, btnSave,
                new ToolStripSeparator(),
                btnFont,
                new ToolStripSeparator(),
                btnLeft, btnCenter, btnRight
            });

            tool.Dock = DockStyle.Top;
            Controls.Add(tool);

            status = new StatusStrip();
            status.BackColor = Color.FromArgb(35, 35, 35);
            status.ForeColor = Color.White;

            statusInfo = new ToolStripStatusLabel();
            statusInfo.Text = "Ready";
            status.Items.Add(statusInfo);
            Controls.Add(status);

            MdiChildActivate += (s, e) => UpdateStatus();
            NewDocument(null, EventArgs.Empty);
        }

        private ToolStripButton CreateToolButton(string text, EventHandler click)
        {
            ToolStripButton button = new ToolStripButton(text);
            button.ForeColor = Color.White;
            button.DisplayStyle = ToolStripItemDisplayStyle.Text;
            button.Click += click;
            return button;
        }

        private EditorChild ActiveEditor()
        {
            return ActiveMdiChild as EditorChild;
        }

        private void NewDocument(object sender, EventArgs e)
        {
            EditorChild child = new EditorChild();
            child.MdiParent = this;
            child.TextChangedInEditor += (s, ev) => UpdateStatus();
            child.Show();
            UpdateStatus();
        }

        private void OpenDocument(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "RTF файли (*.rtf)|*.rtf|Текстові файли (*.txt)|*.txt|Усі файли (*.*)|*.*";

            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            EditorChild child = new EditorChild();
            child.MdiParent = this;
            child.OpenFile(dialog.FileName);
            child.TextChangedInEditor += (s, ev) => UpdateStatus();
            child.Show();
            UpdateStatus();
        }

        private void SaveDocument(object sender, EventArgs e)
        {
            EditorChild child = ActiveEditor();

            if (child == null)
                return;

            child.Save();
            UpdateStatus();
        }

        private void SaveAsDocument(object sender, EventArgs e)
        {
            EditorChild child = ActiveEditor();

            if (child == null)
                return;

            child.SaveAs();
            UpdateStatus();
        }

        private void CloseDocument(object sender, EventArgs e)
        {
            EditorChild child = ActiveEditor();

            if (child != null)
                child.Close();
        }

        private void ExitProgram(object sender, EventArgs e)
        {
            Close();
        }

        private void CopyText(object sender, EventArgs e)
        {
            ActiveEditor()?.Editor.Copy();
        }

        private void PasteText(object sender, EventArgs e)
        {
            ActiveEditor()?.Editor.Paste();
        }

        private void CutText(object sender, EventArgs e)
        {
            ActiveEditor()?.Editor.Cut();
        }

        private void SelectAllText(object sender, EventArgs e)
        {
            ActiveEditor()?.Editor.SelectAll();
        }

        private void ChangeFont(object sender, EventArgs e)
        {
            EditorChild child = ActiveEditor();

            if (child == null)
                return;

            FontDialog dialog = new FontDialog();
            dialog.Font = child.Editor.SelectionFont ?? child.Editor.Font;

            if (dialog.ShowDialog() == DialogResult.OK)
                child.Editor.SelectionFont = dialog.Font;
        }

        private void AlignLeft(object sender, EventArgs e)
        {
            SetAlignment(HorizontalAlignment.Left);
        }

        private void AlignCenter(object sender, EventArgs e)
        {
            SetAlignment(HorizontalAlignment.Center);
        }

        private void AlignRight(object sender, EventArgs e)
        {
            SetAlignment(HorizontalAlignment.Right);
        }

        private void SetAlignment(HorizontalAlignment alignment)
        {
            EditorChild child = ActiveEditor();

            if (child != null)
                child.Editor.SelectionAlignment = alignment;
        }

        private void SetUkrainian(object sender, EventArgs e)
        {
            ukrainian = true;
            ApplyLanguage();
        }

        private void SetEnglish(object sender, EventArgs e)
        {
            ukrainian = false;
            ApplyLanguage();
        }

        private void ApplyLanguage()
        {
            if (ukrainian)
            {
                fileMenu.Text = "Файл";
                editMenu.Text = "Редагування";
                formatMenu.Text = "Формат";
                languageMenu.Text = "Мова";
                windowMenu.Text = "Вікно";

                newItem.Text = "Новий";
                openItem.Text = "Відкрити";
                saveItem.Text = "Зберегти";
                saveAsItem.Text = "Зберегти як";
                closeItem.Text = "Закрити документ";
                exitItem.Text = "Вихід";

                copyItem.Text = "Копіювати";
                pasteItem.Text = "Вставити";
                cutItem.Text = "Вирізати";
                selectAllItem.Text = "Виділити все";

                fontItem.Text = "Шрифт";
                leftItem.Text = "Ліворуч";
                centerItem.Text = "По центру";
                rightItem.Text = "Праворуч";

                btnNew.Text = "Новий";
                btnOpen.Text = "Відкрити";
                btnSave.Text = "Зберегти";
                btnFont.Text = "Шрифт";
                btnLeft.Text = "Ліворуч";
                btnCenter.Text = "Центр";
                btnRight.Text = "Праворуч";
            }
            else
            {
                fileMenu.Text = "File";
                editMenu.Text = "Edit";
                formatMenu.Text = "Format";
                languageMenu.Text = "Language";
                windowMenu.Text = "Window";

                newItem.Text = "New";
                openItem.Text = "Open";
                saveItem.Text = "Save";
                saveAsItem.Text = "Save As";
                closeItem.Text = "Close Document";
                exitItem.Text = "Exit";

                copyItem.Text = "Copy";
                pasteItem.Text = "Paste";
                cutItem.Text = "Cut";
                selectAllItem.Text = "Select All";

                fontItem.Text = "Font";
                leftItem.Text = "Left";
                centerItem.Text = "Center";
                rightItem.Text = "Right";

                btnNew.Text = "New";
                btnOpen.Text = "Open";
                btnSave.Text = "Save";
                btnFont.Text = "Font";
                btnLeft.Text = "Left";
                btnCenter.Text = "Center";
                btnRight.Text = "Right";
            }

            UpdateStatus();
        }

        private void UpdateStatus()
        {
            EditorChild child = ActiveEditor();

            if (child == null)
            {
                statusInfo.Text = ukrainian ? "Документ не відкрито" : "No document";
                return;
            }

            int chars = child.Editor.TextLength;
            int words = child.Editor.Text.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;

            if (ukrainian)
                statusInfo.Text = "Символів: " + chars + " | Слів: " + words + " | Документ: " + child.Text;
            else
                statusInfo.Text = "Characters: " + chars + " | Words: " + words + " | Document: " + child.Text;
        }
    }

    public class EditorChild : Form
    {
        public RichTextBox Editor { get; private set; }
        public string CurrentFile { get; private set; }
        public event EventHandler TextChangedInEditor;

        private static int counter = 1;

        public EditorChild()
        {
            Text = "Document " + counter;
            counter++;

            Width = 650;
            Height = 450;
            BackColor = Color.FromArgb(30, 30, 30);

            Editor = new RichTextBox();
            Editor.Dock = DockStyle.Fill;
            Editor.Font = new Font("Consolas", 11);
            Editor.BackColor = Color.FromArgb(22, 22, 22);
            Editor.ForeColor = Color.White;
            Editor.BorderStyle = BorderStyle.None;
            Editor.AcceptsTab = true;
            Editor.TextChanged += (s, e) => TextChangedInEditor?.Invoke(this, EventArgs.Empty);

            Controls.Add(Editor);
        }

        public void OpenFile(string path)
        {
            CurrentFile = path;
            Text = Path.GetFileName(path);

            if (Path.GetExtension(path).ToLower() == ".rtf")
                Editor.LoadFile(path, RichTextBoxStreamType.RichText);
            else
                Editor.Text = File.ReadAllText(path);
        }

        public void Save()
        {
            if (string.IsNullOrWhiteSpace(CurrentFile))
            {
                SaveAs();
                return;
            }

            SaveToFile(CurrentFile);
        }

        public void SaveAs()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "RTF файл (*.rtf)|*.rtf|Текстовий файл (*.txt)|*.txt";
            dialog.FileName = Text.Replace("*", "") + ".rtf";

            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            CurrentFile = dialog.FileName;
            Text = Path.GetFileName(CurrentFile);
            SaveToFile(CurrentFile);
        }

        private void SaveToFile(string path)
        {
            if (Path.GetExtension(path).ToLower() == ".rtf")
                Editor.SaveFile(path, RichTextBoxStreamType.RichText);
            else
                File.WriteAllText(path, Editor.Text);
        }
    }
}