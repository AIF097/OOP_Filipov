
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Word = Microsoft.Office.Interop.Word;

namespace Laboratorna26
{
    public partial class Form1 : Form
    {
        private TextBox txtRecipient;
        private TextBox txtSender;
        private TextBox txtAmount;
        private TextBox txtDate;
        private TextBox txtCompany;
        private TextBox txtPhone;
        private TextBox txtEmail;
        private TextBox txtSearch;
        private TextBox txtReplace;
        private ComboBox cmbTemplate;
        private Button btnCreateTemplate;
        private Button btnPreview;
        private Button btnGenerate;
        private Button btnSavePath;
        private Button btnFindReplace;
        private Label lblPath;
        private RichTextBox logBox;

        private string templateFolder;
        private string selectedSavePath = "";

        public Form1()
        {
            InitializeComponent();
            BuildInterface();

            templateFolder = Path.Combine(Application.StartupPath, "Templates");
            Directory.CreateDirectory(templateFolder);
        }

        private void BuildInterface()
        {
            Text = "Laboratorna26 - Варіант 1";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(1050, 690);
            MinimumSize = new Size(1050, 690);
            BackColor = Color.FromArgb(244, 247, 252);
            Font = new Font("Segoe UI", 10F);

            Label title = new Label();
            title.Text = "Генератор подарункового сертифіката";
            title.Font = new Font("Segoe UI Semibold", 18F);
            title.ForeColor = Color.FromArgb(35, 48, 70);
            title.Location = new Point(25, 20);
            title.Size = new Size(700, 40);
            Controls.Add(title);

            Panel left = new Panel();
            left.Location = new Point(25, 80);
            left.Size = new Size(470, 535);
            left.BackColor = Color.White;
            left.BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(left);

            Panel right = new Panel();
            right.Location = new Point(515, 80);
            right.Size = new Size(490, 535);
            right.BackColor = Color.White;
            right.BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(right);

            int y = 25;

            AddLabel(left, "Отримувач:", 25, y);
            txtRecipient = AddTextBox(left, 170, y, "Іван Петренко");
            y += 55;

            AddLabel(left, "Від кого:", 25, y);
            txtSender = AddTextBox(left, 170, y, "Компанія Office Home");
            y += 55;

            AddLabel(left, "Сума / подарунок:", 25, y);
            txtAmount = AddTextBox(left, 170, y, "Microsoft Office для будинку й навчання 2007");
            y += 55;

            AddLabel(left, "Дата:", 25, y);
            txtDate = AddTextBox(left, 170, y, DateTime.Now.ToShortDateString());
            y += 55;

            AddLabel(left, "Компанія:", 25, y);
            txtCompany = AddTextBox(left, 170, y, "Office.com");
            y += 55;

            AddLabel(left, "Телефон:", 25, y);
            txtPhone = AddTextBox(left, 170, y, "+380991234567");
            y += 55;

            AddLabel(left, "E-mail:", 25, y);
            txtEmail = AddTextBox(left, 170, y, "email@email.com");
            y += 65;

            btnCreateTemplate = AddButton(left, "Створити шаблони", 25, y, 195);
            btnCreateTemplate.Click += BtnCreateTemplate_Click;

            btnPreview = AddButton(left, "Перегляд шаблону", 235, y, 195);
            btnPreview.Click += BtnPreview_Click;
            y += 60;

            btnSavePath = AddButton(left, "Куди зберегти документ", 25, y, 405);
            btnSavePath.Click += BtnSavePath_Click;
            y += 50;

            lblPath = new Label();
            lblPath.Text = "Шлях не вибрано";
            lblPath.Location = new Point(25, y);
            lblPath.Size = new Size(405, 45);
            lblPath.ForeColor = Color.FromArgb(80, 80, 80);
            left.Controls.Add(lblPath);

            Label tpl = new Label();
            tpl.Text = "Доступні шаблони:";
            tpl.Location = new Point(25, 25);
            tpl.Size = new Size(180, 25);
            tpl.ForeColor = Color.FromArgb(35, 48, 70);
            right.Controls.Add(tpl);

            cmbTemplate = new ComboBox();
            cmbTemplate.Location = new Point(25, 55);
            cmbTemplate.Size = new Size(430, 30);
            cmbTemplate.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTemplate.Items.Add("Сертифікат класичний");
            cmbTemplate.Items.Add("Сертифікат святковий");
            cmbTemplate.Items.Add("Сертифікат офіційний");
            cmbTemplate.SelectedIndex = 0;
            right.Controls.Add(cmbTemplate);

            btnGenerate = AddButton(right, "Згенерувати Word-документ", 25, 105, 430);
            btnGenerate.Height = 45;
            btnGenerate.Click += BtnGenerate_Click;

            Label findLabel = new Label();
            findLabel.Text = "Пошук і заміна в готовому документі:";
            findLabel.Location = new Point(25, 175);
            findLabel.Size = new Size(360, 25);
            findLabel.ForeColor = Color.FromArgb(35, 48, 70);
            right.Controls.Add(findLabel);

            txtSearch = new TextBox();
            txtSearch.Location = new Point(25, 210);
            txtSearch.Size = new Size(200, 30);
            txtSearch.PlaceholderText = "Що знайти";
            right.Controls.Add(txtSearch);

            txtReplace = new TextBox();
            txtReplace.Location = new Point(240, 210);
            txtReplace.Size = new Size(215, 30);
            txtReplace.PlaceholderText = "На що замінити";
            right.Controls.Add(txtReplace);

            btnFindReplace = AddButton(right, "Знайти і замінити", 25, 255, 430);
            btnFindReplace.Click += BtnFindReplace_Click;

            logBox = new RichTextBox();
            logBox.Location = new Point(25, 320);
            logBox.Size = new Size(430, 185);
            logBox.ReadOnly = true;
            logBox.BackColor = Color.FromArgb(248, 250, 255);
            logBox.BorderStyle = BorderStyle.FixedSingle;
            right.Controls.Add(logBox);
        }

        private Label AddLabel(Control parent, string text, int x, int y)
        {
            Label label = new Label();
            label.Text = text;
            label.Location = new Point(x, y + 4);
            label.Size = new Size(135, 28);
            label.ForeColor = Color.FromArgb(45, 55, 75);
            parent.Controls.Add(label);
            return label;
        }

        private TextBox AddTextBox(Control parent, int x, int y, string text)
        {
            TextBox box = new TextBox();
            box.Location = new Point(x, y);
            box.Size = new Size(260, 30);
            box.Text = text;
            parent.Controls.Add(box);
            return box;
        }

        private Button AddButton(Control parent, string text, int x, int y, int w)
        {
            Button button = new Button();
            button.Text = text;
            button.Location = new Point(x, y);
            button.Size = new Size(w, 38);
            button.BackColor = Color.FromArgb(58, 102, 180);
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            parent.Controls.Add(button);
            return button;
        }

        private void BtnCreateTemplate_Click(object sender, EventArgs e)
        {
            try
            {
                CreateAllTemplates();
                Log("Шаблони створено у папці Templates.");
                MessageBox.Show("Шаблони створено.", "Готово");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка");
            }
        }

        private void BtnPreview_Click(object sender, EventArgs e)
        {
            string templatePath = GetTemplatePath();

            if (!File.Exists(templatePath))
            {
                MessageBox.Show("Спочатку натисни «Створити шаблони».", "Немає шаблону");
                return;
            }

            Word.Application word = null;

            try
            {
                word = new Word.Application();
                word.Visible = true;
                word.Documents.Open(templatePath, ReadOnly: true);
                Log("Відкрито перегляд шаблону.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка");
                if (word != null) word.Quit();
            }
        }

        private void BtnSavePath_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Word Document (*.docx)|*.docx";
            dialog.FileName = "Подарунковий_сертифікат.docx";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                selectedSavePath = dialog.FileName;
                lblPath.Text = selectedSavePath;
                Log("Вибрано шлях збереження.");
            }
        }

        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(selectedSavePath))
            {
                MessageBox.Show("Спочатку вибери шлях збереження.", "Немає шляху");
                return;
            }

            string templatePath = GetTemplatePath();

            if (!File.Exists(templatePath))
            {
                MessageBox.Show("Спочатку створи шаблони.", "Немає шаблону");
                return;
            }

            Word.Application word = null;
            Word.Document doc = null;

            try
            {
                word = new Word.Application();
                doc = word.Documents.Add(templatePath);

                ReplaceText(doc, "{RECIPIENT}", txtRecipient.Text);
                ReplaceText(doc, "{SENDER}", txtSender.Text);
                ReplaceText(doc, "{AMOUNT}", txtAmount.Text);
                ReplaceText(doc, "{DATE}", txtDate.Text);
                ReplaceText(doc, "{COMPANY}", txtCompany.Text);
                ReplaceText(doc, "{PHONE}", txtPhone.Text);
                ReplaceText(doc, "{EMAIL}", txtEmail.Text);

                doc.SaveAs2(selectedSavePath);
                word.Visible = true;
                doc.Activate();

                Log("Документ згенеровано і відкрито у Word.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка");
                if (doc != null) doc.Close(false);
                if (word != null) word.Quit();
            }
            finally
            {
                if (doc != null) Marshal.ReleaseComObject(doc);
                if (word != null) Marshal.ReleaseComObject(word);
            }
        }

        private void BtnFindReplace_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(selectedSavePath) || !File.Exists(selectedSavePath))
            {
                MessageBox.Show("Спочатку згенеруй документ.", "Немає документа");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                MessageBox.Show("Введи текст для пошуку.", "Порожній пошук");
                return;
            }

            Word.Application word = null;
            Word.Document doc = null;

            try
            {
                word = new Word.Application();
                doc = word.Documents.Open(selectedSavePath);

                ReplaceText(doc, txtSearch.Text, txtReplace.Text);

                doc.Save();
                word.Visible = true;
                doc.Activate();

                Log("Пошук і заміну виконано.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка");
                if (doc != null) doc.Close(false);
                if (word != null) word.Quit();
            }
            finally
            {
                if (doc != null) Marshal.ReleaseComObject(doc);
                if (word != null) Marshal.ReleaseComObject(word);
            }
        }

        private string GetTemplatePath()
        {
            string name = "TemplateClassic.dotx";

            if (cmbTemplate.SelectedIndex == 1)
                name = "TemplateHoliday.dotx";

            if (cmbTemplate.SelectedIndex == 2)
                name = "TemplateOfficial.dotx";

            return Path.Combine(templateFolder, name);
        }

        private void CreateAllTemplates()
        {
            CreateTemplate(Path.Combine(templateFolder, "TemplateClassic.dotx"), 1);
            CreateTemplate(Path.Combine(templateFolder, "TemplateHoliday.dotx"), 2);
            CreateTemplate(Path.Combine(templateFolder, "TemplateOfficial.dotx"), 3);
        }

        private void CreateTemplate(string path, int style)
        {
            Word.Application word = null;
            Word.Document doc = null;

            try
            {
                word = new Word.Application();
                doc = word.Documents.Add();

                Word.Section section = doc.Sections[1];
                section.PageSetup.TopMargin = word.CentimetersToPoints(1.5f);
                section.PageSetup.BottomMargin = word.CentimetersToPoints(1.5f);
                section.PageSetup.LeftMargin = word.CentimetersToPoints(1.5f);
                section.PageSetup.RightMargin = word.CentimetersToPoints(1.5f);

                Word.Range range = doc.Range(0, 0);
                Word.Table table = doc.Tables.Add(range, 5, 2);
                table.Borders.Enable = 1;
                table.Rows.HeightRule = Word.WdRowHeightRule.wdRowHeightExactly;
                table.Rows.Height = word.CentimetersToPoints(5.2f);

                for (int i = 1; i <= 5; i++)
                {
                    for (int j = 1; j <= 2; j++)
                    {
                        Word.Cell cell = table.Cell(i, j);
                        cell.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                        cell.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;

                        string title = "ПОДАРУНКОВИЙ СЕРТИФІКАТ";
                        string line = "===================";

                        if (style == 2)
                        {
                            title = "СВЯТКОВИЙ СЕРТИФІКАТ";
                            line = "**************";
                        }

                        if (style == 3)
                        {
                            title = "ОФІЦІЙНИЙ ПОДАРУНКОВИЙ СЕРТИФІКАТ";
                            line = "-------------------";
                        }

                        cell.Range.Text =
                            title + "\r\n" +
                            line + "\r\n" +
                            "Отримувач: {RECIPIENT}\r\n" +
                            "Подарунок: {AMOUNT}\r\n" +
                            "Від: {SENDER}\r\n" +
                            "Компанія: {COMPANY}\r\n" +
                            "Телефон: {PHONE} | E-mail: {EMAIL}\r\n" +
                            "Дата: {DATE}";

                        cell.Range.Font.Name = "Segoe UI";
                        cell.Range.Font.Size = 9;

                        Word.Range firstLine = cell.Range;
                        firstLine.Start = cell.Range.Start;
                        firstLine.End = cell.Range.Start + title.Length;
                        firstLine.Font.Bold = 1;
                        firstLine.Font.Size = 12;

                        if (style == 1)
                            firstLine.Font.Color = Word.WdColor.wdColorDarkBlue;

                        if (style == 2)
                            firstLine.Font.Color = Word.WdColor.wdColorDarkRed;

                        if (style == 3)
                            firstLine.Font.Color = Word.WdColor.wdColorBlack;
                    }
                }

                doc.SaveAs2(path, Word.WdSaveFormat.wdFormatXMLTemplate);
                doc.Close();
                word.Quit();
            }
            finally
            {
                if (doc != null) Marshal.ReleaseComObject(doc);
                if (word != null) Marshal.ReleaseComObject(word);
            }
        }

        private void ReplaceText(Word.Document doc, string findText, string replaceText)
        {
            Word.Range range = doc.Content;
            Word.Find find = range.Find;

            find.ClearFormatting();
            find.Replacement.ClearFormatting();

            find.Text = findText;
            find.Replacement.Text = replaceText;
            find.Forward = true;
            find.Wrap = Word.WdFindWrap.wdFindContinue;
            find.Format = false;
            find.MatchCase = false;
            find.MatchWholeWord = false;
            find.MatchWildcards = false;

            find.Execute(
                Replace: Word.WdReplace.wdReplaceAll
            );
        }

        private void Log(string text)
        {
            logBox.AppendText(DateTime.Now.ToString("HH:mm:ss") + " - " + text + Environment.NewLine);
        }
    }
}