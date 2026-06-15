using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Laboratorna16
{
    public partial class Form1 : Form
    {
        private OrganizationAddress currentAddress;

        private TextBox txtOrganization;
        private TextBox txtCountry;
        private TextBox txtCity;
        private TextBox txtStreet;
        private TextBox txtHouse;
        private TextBox txtIndex;

        private RichTextBox output;

        public Form1()
        {
            InitializeComponent();
            BuildInterface();
        }

        private void BuildInterface()
        {
            Text = "Laboratorna16 - Варіант 26";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(950, 670);
            MinimumSize = new Size(900, 620);
            BackColor = Color.White;
            Font = new Font("Segoe UI", 10);

            Label title = new Label();
            title.Text = "Laboratorna16 - Поштова адреса організації";
            title.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            title.Location = new Point(20, 15);
            title.Size = new Size(720, 35);
            Controls.Add(title);

            int labelX = 30;
            int boxX = 190;
            int top = 90;
            int step = 45;

            Controls.Add(CreateLabel("Організація:", labelX, top));
            txtOrganization = CreateBox(boxX, top, "ТОВ Альфа");

            Controls.Add(CreateLabel("Країна:", labelX, top + step));
            txtCountry = CreateBox(boxX, top + step, "Україна");

            Controls.Add(CreateLabel("Місто:", labelX, top + step * 2));
            txtCity = CreateBox(boxX, top + step * 2, "Київ");

            Controls.Add(CreateLabel("Вулиця:", labelX, top + step * 3));
            txtStreet = CreateBox(boxX, top + step * 3, "Хрещатик");

            Controls.Add(CreateLabel("Будинок:", labelX, top + step * 4));
            txtHouse = CreateBox(boxX, top + step * 4, "15");

            Controls.Add(CreateLabel("Індекс:", labelX, top + step * 5));
            txtIndex = CreateBox(boxX, top + step * 5, "01001");
            txtIndex.KeyPress += OnlyDigits;

            Controls.Add(txtOrganization);
            Controls.Add(txtCountry);
            Controls.Add(txtCity);
            Controls.Add(txtStreet);
            Controls.Add(txtHouse);
            Controls.Add(txtIndex);

            Controls.Add(CreateButton("Створити повну адресу", 530, 90, CreateFullAddress));
            Controls.Add(CreateButton("Створити коротку адресу", 530, 140, CreateShortAddress));
            Controls.Add(CreateButton("Створити порожній об'єкт", 530, 190, CreateEmptyAddress));
            Controls.Add(CreateButton("Змінити місто", 530, 240, ChangeCity));
            Controls.Add(CreateButton("Змінити вулицю і будинок", 530, 290, ChangeStreetHouse));
            Controls.Add(CreateButton("Знищити об'єкт", 530, 340, DestroyObject));

            output = new RichTextBox();
            output.Location = new Point(30, 410);
            output.Size = new Size(860, 190);
            output.ReadOnly = true;
            output.Font = new Font("Consolas", 10);
            output.BackColor = Color.FromArgb(248, 248, 248);
            Controls.Add(output);
        }

        private Label CreateLabel(string text, int x, int y)
        {
            Label label = new Label();
            label.Text = text;
            label.Location = new Point(x, y + 3);
            label.Size = new Size(140, 30);
            return label;
        }

        private TextBox CreateBox(int x, int y, string text)
        {
            TextBox box = new TextBox();
            box.Location = new Point(x, y);
            box.Size = new Size(260, 30);
            box.Text = text;
            return box;
        }

        private Button CreateButton(string text, int x, int y, EventHandler click)
        {
            Button button = new Button();
            button.Text = text;
            button.Location = new Point(x, y);
            button.Size = new Size(260, 38);
            button.BackColor = Color.FromArgb(235, 242, 255);
            button.FlatStyle = FlatStyle.Flat;
            button.Click += click;
            return button;
        }

        private void OnlyDigits(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
                return;

            e.Handled = true;
        }

        private bool ObjectExists()
        {
            if (currentAddress == null)
            {
                MessageBox.Show("Спочатку створіть об'єкт.");
                return false;
            }

            return true;
        }

        private void CreateFullAddress(object sender, EventArgs e)
        {
            currentAddress = new OrganizationAddress(
                txtOrganization.Text,
                txtCountry.Text,
                txtCity.Text,
                txtStreet.Text,
                txtHouse.Text,
                txtIndex.Text
            );

            output.Text =
                "Створено повну адресу\n\n" +
                currentAddress.GetFullAddress() +
                "\nКількість активних об'єктів: " +
                OrganizationAddress.ActiveCount;
        }

        private void CreateShortAddress(object sender, EventArgs e)
        {
            currentAddress = new OrganizationAddress(
                txtOrganization.Text,
                txtCountry.Text,
                txtCity.Text,
                txtStreet.Text,
                txtHouse.Text,
                txtIndex.Text,
                true
            );

            output.Text =
                "Створено коротку адресу\n\n" +
                currentAddress.GetShortAddress() +
                "\n\nКількість активних об'єктів: " +
                OrganizationAddress.ActiveCount;
        }

        private void CreateEmptyAddress(object sender, EventArgs e)
        {
            currentAddress = new OrganizationAddress();

            output.Text =
                "Створено порожній об'єкт\n\n" +
                currentAddress.GetFullAddress() +
                "\nКількість активних об'єктів: " +
                OrganizationAddress.ActiveCount;
        }

        private void ChangeCity(object sender, EventArgs e)
        {
            if (!ObjectExists())
                return;

            currentAddress.ChangeCity(txtCity.Text);

            output.Text =
                "Місто змінено\n\n" +
                currentAddress.GetFullAddress();
        }

        private void ChangeStreetHouse(object sender, EventArgs e)
        {
            if (!ObjectExists())
                return;

            currentAddress.ChangeStreetAndHouse(
                txtStreet.Text,
                txtHouse.Text
            );

            output.Text =
                "Вулицю та будинок змінено\n\n" +
                currentAddress.GetFullAddress();
        }

        private void DestroyObject(object sender, EventArgs e)
        {
            if (currentAddress == null)
            {
                output.Text =
                    "Активного об'єкта немає\n\n" +
                    "Кількість активних об'єктів: " +
                    OrganizationAddress.ActiveCount;
                return;
            }

            currentAddress.Destroy();
            currentAddress = null;

            output.Text =
                "Об'єкт знищено\n\n" +
                "Кількість активних об'єктів: " +
                OrganizationAddress.ActiveCount;
        }
    }

    public class OrganizationAddress
    {
        public static int ActiveCount { get; private set; }

        public string Organization { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string House { get; set; }
        public string PostalIndex { get; set; }

        private bool isDestroyed;

        public OrganizationAddress()
        {
            Organization = "Невідома організація";
            Country = "Не вказано";
            City = "Не вказано";
            Street = "Не вказано";
            House = "Не вказано";
            PostalIndex = "00000";

            ActiveCount++;
        }

        public OrganizationAddress(
            string organization,
            string country,
            string city,
            string street,
            string house,
            string postalIndex)
        {
            Organization = Normalize(organization, "Невідома організація");
            Country = Normalize(country, "Не вказано");
            City = Normalize(city, "Не вказано");
            Street = Normalize(street, "Не вказано");
            House = Normalize(house, "Не вказано");
            PostalIndex = Normalize(postalIndex, "00000");

            ActiveCount++;
        }

        public OrganizationAddress(
            string organization,
            string country,
            string city,
            string street,
            string house,
            string postalIndex,
            bool shortMode)
        {
            Organization = Normalize(organization, "Невідома організація");
            Country = Normalize(country, "Не вказано");
            City = Normalize(city, "Не вказано");
            Street = Normalize(street, "");
            House = Normalize(house, "");
            PostalIndex = Normalize(postalIndex, "");

            ActiveCount++;
        }

        private string Normalize(string value, string defaultValue)
        {
            if (string.IsNullOrWhiteSpace(value))
                return defaultValue;

            return value.Trim();
        }

        public void ChangeCity(string newCity)
        {
            City = Normalize(newCity, "Не вказано");
        }

        public void ChangeStreetAndHouse(string newStreet, string newHouse)
        {
            Street = Normalize(newStreet, "Не вказано");
            House = Normalize(newHouse, "Не вказано");
        }

        public void Destroy()
        {
            if (isDestroyed)
                return;

            if (ActiveCount > 0)
                ActiveCount--;

            isDestroyed = true;
        }

        public string GetFullAddress()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Повна адреса:");
            sb.AppendLine("Організація: " + Organization);
            sb.AppendLine("Країна: " + Country);
            sb.AppendLine("Місто: " + City);
            sb.AppendLine("Вулиця: " + Street);
            sb.AppendLine("Будинок: " + House);
            sb.AppendLine("Індекс: " + PostalIndex);

            return sb.ToString();
        }

        public string GetShortAddress()
        {
            return Organization + ", " +
                   Country + ", " +
                   City + ", " +
                   Street + " " +
                   House + ", " +
                   PostalIndex;
        }
    }
}