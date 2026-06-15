using System;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Laboratorna25
{
    public partial class Form1 : Form
    {
        private string dbPath;
        private string connectionString;

        private OleDbConnection connection;
        private OleDbDataAdapter flightsAdapter;
        private OleDbDataAdapter passengersAdapter;
        private OleDbDataAdapter ticketsAdapter;

        private DataSet dataSet;

        private TabControl tabs;
        private DataGridView dgvFlights;
        private DataGridView dgvPassengers;
        private DataGridView dgvTickets;
        private DataGridView dgvReports;

        private TextBox txtSearch;
        private ComboBox cmbReport;
        private Label status;

        public Form1()
        {
            InitializeComponent();
            BuildInterface();
            PrepareDatabase();
            LoadData();
        }

        private void BuildInterface()
        {
            Text = "Laboratorna25 - Авіарейси й продаж авіаквитків";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(1180, 720);
            MinimumSize = new Size(1100, 650);
            BackColor = Color.White;
            Font = new Font("Segoe UI", 10);

            Label title = new Label();
            title.Text = "Laboratorna25 - Підключення до бази даних Access";
            title.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            title.Location = new Point(20, 15);
            title.Size = new Size(900, 35);
            Controls.Add(title);

            Label topic = new Label();
            topic.Text = "Варіант 1: авіарейси й продаж авіаквитків";
            topic.Location = new Point(22, 55);
            topic.Size = new Size(700, 28);
            Controls.Add(topic);

            txtSearch = new TextBox();
            txtSearch.Location = new Point(25, 95);
            txtSearch.Size = new Size(300, 30);
            txtSearch.PlaceholderText = "Пошук рейсу або пасажира";
            Controls.Add(txtSearch);

            Controls.Add(CreateButton("Пошук", 340, 93, SearchData));
            Controls.Add(CreateButton("Скинути", 520, 93, ResetSearch));
            Controls.Add(CreateButton("Зберегти зміни", 700, 93, SaveChanges));
            Controls.Add(CreateButton("Додати запис", 910, 93, AddRecord));
            Controls.Add(CreateButton("Видалити запис", 910, 138, DeleteSelectedRecord));

            cmbReport = new ComboBox();
            cmbReport.Location = new Point(25, 140);
            cmbReport.Size = new Size(300, 30);
            cmbReport.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbReport.Items.Add("Звіт 1: квитки по рейсах");
            cmbReport.Items.Add("Звіт 2: прибуток по напрямках");
            cmbReport.SelectedIndex = 0;
            Controls.Add(cmbReport);

            Controls.Add(CreateButton("Побудувати", 340, 138, BuildReport));

            tabs = new TabControl();
            tabs.Location = new Point(25, 185);
            tabs.Size = new Size(1100, 430);
            Controls.Add(tabs);

            dgvFlights = CreateGrid();
            dgvPassengers = CreateGrid();
            dgvTickets = CreateGrid();
            dgvReports = CreateGrid();
            dgvReports.ReadOnly = true;
            dgvReports.AllowUserToAddRows = false;
            dgvReports.AllowUserToDeleteRows = false;

            tabs.TabPages.Add(CreatePage("Рейси", dgvFlights));
            tabs.TabPages.Add(CreatePage("Пасажири", dgvPassengers));
            tabs.TabPages.Add(CreatePage("Квитки", dgvTickets));
            tabs.TabPages.Add(CreatePage("Звіти", dgvReports));

            status = new Label();
            status.Location = new Point(25, 630);
            status.Size = new Size(1000, 30);
            status.Text = "Готово";
            Controls.Add(status);
        }

        private Button CreateButton(string text, int x, int y, EventHandler click)
        {
            Button button = new Button();
            button.Text = text;
            button.Location = new Point(x, y);
            button.Size = new Size(170, 38);
            button.BackColor = Color.FromArgb(235, 242, 255);
            button.FlatStyle = FlatStyle.Flat;
            button.Click += click;
            return button;
        }

        private DataGridView CreateGrid()
        {
            DataGridView grid = new DataGridView();
            grid.Dock = DockStyle.Fill;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.None;
            grid.AllowUserToAddRows = true;
            grid.AllowUserToDeleteRows = true;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            return grid;
        }

        private TabPage CreatePage(string text, DataGridView grid)
        {
            TabPage page = new TabPage(text);
            page.BackColor = Color.White;
            page.Controls.Add(grid);
            return page;
        }

        private void PrepareDatabase()
        {
            dbPath = Path.Combine(Application.StartupPath, "AirTickets.mdb");
            connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + dbPath;

            if (!File.Exists(dbPath))
                CreateAccessDatabase();

            connection = new OleDbConnection(connectionString);
        }

        private void CreateAccessDatabase()
        {
            Type catalogType = Type.GetTypeFromProgID("ADOX.Catalog");

            if (catalogType == null)
            {
                MessageBox.Show("Не знайдено ADOX.Catalog. Встанови Microsoft Access Database Engine або створи .mdb вручну.");
                return;
            }

            dynamic catalog = Activator.CreateInstance(catalogType);
            catalog.Create(connectionString);

            using (OleDbConnection cn = new OleDbConnection(connectionString))
            {
                cn.Open();

                Execute(cn, "CREATE TABLE Flights (FlightID AUTOINCREMENT PRIMARY KEY, FlightNumber TEXT(20), FromCity TEXT(50), ToCity TEXT(50), FlightDate DATETIME, Price DOUBLE)");
                Execute(cn, "CREATE TABLE Passengers (PassengerID AUTOINCREMENT PRIMARY KEY, FullName TEXT(80), Passport TEXT(30), Phone TEXT(30), Email TEXT(80))");
                Execute(cn, "CREATE TABLE Tickets (TicketID AUTOINCREMENT PRIMARY KEY, FlightID INTEGER, PassengerID INTEGER, SeatNumber TEXT(10), SaleDate DATETIME, TicketPrice DOUBLE)");

                Execute(cn, "INSERT INTO Flights (FlightNumber, FromCity, ToCity, FlightDate, Price) VALUES ('PS101', 'Київ', 'Львів', #2026-05-20#, 1800)");
                Execute(cn, "INSERT INTO Flights (FlightNumber, FromCity, ToCity, FlightDate, Price) VALUES ('PS202', 'Київ', 'Варшава', #2026-05-22#, 4200)");
                Execute(cn, "INSERT INTO Flights (FlightNumber, FromCity, ToCity, FlightDate, Price) VALUES ('PS303', 'Львів', 'Прага', #2026-05-25#, 3900)");

                Execute(cn, "INSERT INTO Passengers (FullName, Passport, Phone, Email) VALUES ('Іван Петренко', 'AA123456', '+380501112233', 'ivan@mail.com')");
                Execute(cn, "INSERT INTO Passengers (FullName, Passport, Phone, Email) VALUES ('Олена Коваль', 'BB654321', '+380671234567', 'olena@mail.com')");
                Execute(cn, "INSERT INTO Passengers (FullName, Passport, Phone, Email) VALUES ('Денис Мельник', 'CC777888', '+380931234567', 'denys@mail.com')");

                Execute(cn, "INSERT INTO Tickets (FlightID, PassengerID, SeatNumber, SaleDate, TicketPrice) VALUES (1, 1, '12A', #2026-05-10#, 1800)");
                Execute(cn, "INSERT INTO Tickets (FlightID, PassengerID, SeatNumber, SaleDate, TicketPrice) VALUES (2, 2, '07B', #2026-05-11#, 4200)");
                Execute(cn, "INSERT INTO Tickets (FlightID, PassengerID, SeatNumber, SaleDate, TicketPrice) VALUES (3, 3, '03C', #2026-05-12#, 3900)");
            }
        }

        private void Execute(OleDbConnection cn, string sql)
        {
            using (OleDbCommand cmd = new OleDbCommand(sql, cn))
                cmd.ExecuteNonQuery();
        }

        private void LoadData()
        {
            dataSet = new DataSet();

            flightsAdapter = CreateAdapter("SELECT * FROM Flights");
            passengersAdapter = CreateAdapter("SELECT * FROM Passengers");
            ticketsAdapter = CreateAdapter("SELECT * FROM Tickets");

            flightsAdapter.Fill(dataSet, "Flights");
            passengersAdapter.Fill(dataSet, "Passengers");
            ticketsAdapter.Fill(dataSet, "Tickets");

            dgvFlights.DataSource = dataSet.Tables["Flights"];
            dgvPassengers.DataSource = dataSet.Tables["Passengers"];
            dgvTickets.DataSource = dataSet.Tables["Tickets"];

            RenameColumns();

            status.Text = "База підключена: " + dbPath;
        }

        private OleDbDataAdapter CreateAdapter(string selectSql)
        {
            OleDbDataAdapter adapter = new OleDbDataAdapter(selectSql, connection);
            OleDbCommandBuilder builder = new OleDbCommandBuilder(adapter);
            adapter.InsertCommand = builder.GetInsertCommand();
            adapter.UpdateCommand = builder.GetUpdateCommand();
            adapter.DeleteCommand = builder.GetDeleteCommand();
            return adapter;
        }

        private void RenameColumns()
        {
            dgvFlights.Columns["FlightID"].HeaderText = "ID";
            dgvFlights.Columns["FlightNumber"].HeaderText = "Номер рейсу";
            dgvFlights.Columns["FromCity"].HeaderText = "Звідки";
            dgvFlights.Columns["ToCity"].HeaderText = "Куди";
            dgvFlights.Columns["FlightDate"].HeaderText = "Дата рейсу";
            dgvFlights.Columns["Price"].HeaderText = "Ціна";

            dgvPassengers.Columns["PassengerID"].HeaderText = "ID";
            dgvPassengers.Columns["FullName"].HeaderText = "ПІБ";
            dgvPassengers.Columns["Passport"].HeaderText = "Паспорт";
            dgvPassengers.Columns["Phone"].HeaderText = "Телефон";
            dgvPassengers.Columns["Email"].HeaderText = "Email";

            dgvTickets.Columns["TicketID"].HeaderText = "ID";
            dgvTickets.Columns["FlightID"].HeaderText = "ID рейсу";
            dgvTickets.Columns["PassengerID"].HeaderText = "ID пасажира";
            dgvTickets.Columns["SeatNumber"].HeaderText = "Місце";
            dgvTickets.Columns["SaleDate"].HeaderText = "Дата продажу";
            dgvTickets.Columns["TicketPrice"].HeaderText = "Ціна квитка";
        }

        private void SaveChanges(object sender, EventArgs e)
        {
            try
            {
                Validate();
                dgvFlights.EndEdit();
                dgvPassengers.EndEdit();
                dgvTickets.EndEdit();

                flightsAdapter.Update(dataSet, "Flights");
                passengersAdapter.Update(dataSet, "Passengers");
                ticketsAdapter.Update(dataSet, "Tickets");

                RefreshTables();

                status.Text = "Зміни збережено.";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка збереження:\n" + ex.Message);
            }
        }

        private void SearchData(object sender, EventArgs e)
        {
            string q = txtSearch.Text.Trim().Replace("'", "''");

            if (q.Length == 0)
            {
                ResetSearch(sender, e);
                return;
            }

            dataSet.Tables["Flights"].DefaultView.RowFilter =
                $"FlightNumber LIKE '%{q}%' OR FromCity LIKE '%{q}%' OR ToCity LIKE '%{q}%'";

            dataSet.Tables["Passengers"].DefaultView.RowFilter =
                $"FullName LIKE '%{q}%' OR Passport LIKE '%{q}%' OR Phone LIKE '%{q}%' OR Email LIKE '%{q}%'";

            tabs.SelectedIndex = 0;
            status.Text = "Пошук виконано: " + q;
        }

        private void ResetSearch(object sender, EventArgs e)
        {
            txtSearch.Clear();
            dataSet.Tables["Flights"].DefaultView.RowFilter = "";
            dataSet.Tables["Passengers"].DefaultView.RowFilter = "";
            status.Text = "Фільтр скинуто.";
        }

        private void BuildReport(object sender, EventArgs e)
        {
            try
            {
                string sql;

                if (cmbReport.SelectedIndex == 0)
                {
                    sql =
                        "SELECT Flights.FlightNumber AS [Рейс], Flights.FromCity AS [Звідки], Flights.ToCity AS [Куди], " +
                        "Count(Tickets.TicketID) AS [Продано квитків], Sum(Tickets.TicketPrice) AS [Сума продажів] " +
                        "FROM Flights LEFT JOIN Tickets ON Flights.FlightID = Tickets.FlightID " +
                        "GROUP BY Flights.FlightNumber, Flights.FromCity, Flights.ToCity";
                }
                else
                {
                    sql =
                        "SELECT Flights.FromCity AS [Звідки], Flights.ToCity AS [Куди], " +
                        "Count(Tickets.TicketID) AS [Кількість продажів], Sum(Tickets.TicketPrice) AS [Загальний прибуток] " +
                        "FROM Flights LEFT JOIN Tickets ON Flights.FlightID = Tickets.FlightID " +
                        "GROUP BY Flights.FromCity, Flights.ToCity";
                }

                DataTable table = new DataTable();

                using (OleDbDataAdapter adapter = new OleDbDataAdapter(sql, connection))
                    adapter.Fill(table);

                dgvReports.DataSource = table;
                tabs.SelectedIndex = 3;
                status.Text = "Звіт побудовано.";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка звіту:\n" + ex.Message);
            }
        }

        private void AddRecord(object sender, EventArgs e)
        {
            AddTicketForm form = new AddTicketForm();

            if (form.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                double price;

                if (!double.TryParse(form.PriceText.Replace('.', ','), out price))
                {
                    MessageBox.Show("Ціна введена неправильно.");
                    return;
                }

                using (OleDbConnection cn = new OleDbConnection(connectionString))
                {
                    cn.Open();

                    using (OleDbCommand cmd = new OleDbCommand(
                        "INSERT INTO Flights (FlightNumber, FromCity, ToCity, FlightDate, Price) VALUES (?, ?, ?, ?, ?)", cn))
                    {
                        cmd.Parameters.AddWithValue("@p1", form.FlightNumber);
                        cmd.Parameters.AddWithValue("@p2", form.FromCity);
                        cmd.Parameters.AddWithValue("@p3", form.ToCity);
                        cmd.Parameters.AddWithValue("@p4", DateTime.Now.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@p5", price);
                        cmd.ExecuteNonQuery();
                    }

                    using (OleDbCommand cmd = new OleDbCommand(
                        "INSERT INTO Passengers (FullName, Passport, Phone, Email) VALUES (?, ?, ?, ?)", cn))
                    {
                        cmd.Parameters.AddWithValue("@p1", form.PassengerName);
                        cmd.Parameters.AddWithValue("@p2", form.Passport);
                        cmd.Parameters.AddWithValue("@p3", form.Phone);
                        cmd.Parameters.AddWithValue("@p4", form.Email);
                        cmd.ExecuteNonQuery();
                    }

                    int flightId;
                    int passengerId;

                    using (OleDbCommand cmd = new OleDbCommand("SELECT MAX(FlightID) FROM Flights", cn))
                        flightId = Convert.ToInt32(cmd.ExecuteScalar());

                    using (OleDbCommand cmd = new OleDbCommand("SELECT MAX(PassengerID) FROM Passengers", cn))
                        passengerId = Convert.ToInt32(cmd.ExecuteScalar());

                    using (OleDbCommand cmd = new OleDbCommand(
                        "INSERT INTO Tickets (FlightID, PassengerID, SeatNumber, SaleDate, TicketPrice) VALUES (?, ?, ?, ?, ?)", cn))
                    {
                        cmd.Parameters.AddWithValue("@p1", flightId);
                        cmd.Parameters.AddWithValue("@p2", passengerId);
                        cmd.Parameters.AddWithValue("@p3", form.SeatNumber);
                        cmd.Parameters.AddWithValue("@p4", DateTime.Now.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@p5", price);
                        cmd.ExecuteNonQuery();
                    }
                }

                RefreshTables();
                status.Text = "Новий запис додано.";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка додавання:\n" + ex.Message);
            }
        }
        private void DeleteSelectedRecord(object sender, EventArgs e)
        {
            try
            {
                if (tabs.SelectedTab.Text == "Рейси")
                {
                    if (dgvFlights.CurrentRow == null)
                        return;

                    int flightId = Convert.ToInt32(dgvFlights.CurrentRow.Cells["FlightID"].Value);

                    DialogResult result = MessageBox.Show(
                        "Видалити рейс і всі квитки, пов'язані з ним?",
                        "Видалення",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (result != DialogResult.Yes)
                        return;

                    using (OleDbConnection cn = new OleDbConnection(connectionString))
                    {
                        cn.Open();

                        using (OleDbCommand cmd = new OleDbCommand("DELETE FROM Tickets WHERE FlightID = ?", cn))
                        {
                            cmd.Parameters.AddWithValue("@p1", flightId);
                            cmd.ExecuteNonQuery();
                        }

                        using (OleDbCommand cmd = new OleDbCommand("DELETE FROM Flights WHERE FlightID = ?", cn))
                        {
                            cmd.Parameters.AddWithValue("@p1", flightId);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    RefreshTables();
                    status.Text = "Рейс і пов'язані квитки видалено.";
                }
                else if (tabs.SelectedTab.Text == "Пасажири")
                {
                    if (dgvPassengers.CurrentRow == null)
                        return;

                    int passengerId = Convert.ToInt32(dgvPassengers.CurrentRow.Cells["PassengerID"].Value);

                    DialogResult result = MessageBox.Show(
                        "Видалити пасажира і всі його квитки?",
                        "Видалення",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (result != DialogResult.Yes)
                        return;

                    using (OleDbConnection cn = new OleDbConnection(connectionString))
                    {
                        cn.Open();

                        using (OleDbCommand cmd = new OleDbCommand("DELETE FROM Tickets WHERE PassengerID = ?", cn))
                        {
                            cmd.Parameters.AddWithValue("@p1", passengerId);
                            cmd.ExecuteNonQuery();
                        }

                        using (OleDbCommand cmd = new OleDbCommand("DELETE FROM Passengers WHERE PassengerID = ?", cn))
                        {
                            cmd.Parameters.AddWithValue("@p1", passengerId);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    RefreshTables();
                    status.Text = "Пасажира і пов'язані квитки видалено.";
                }
                else if (tabs.SelectedTab.Text == "Квитки")
                {
                    if (dgvTickets.CurrentRow == null)
                        return;

                    int ticketId = Convert.ToInt32(dgvTickets.CurrentRow.Cells["TicketID"].Value);

                    DialogResult result = MessageBox.Show(
                        "Видалити вибраний квиток?",
                        "Видалення",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (result != DialogResult.Yes)
                        return;

                    using (OleDbConnection cn = new OleDbConnection(connectionString))
                    {
                        cn.Open();

                        using (OleDbCommand cmd = new OleDbCommand("DELETE FROM Tickets WHERE TicketID = ?", cn))
                        {
                            cmd.Parameters.AddWithValue("@p1", ticketId);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    RefreshTables();
                    status.Text = "Квиток видалено.";
                }
                else
                {
                    MessageBox.Show("Видалення доступне тільки у вкладках Рейси, Пасажири або Квитки.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка видалення:\n" + ex.Message);
            }
        }
        private void RefreshTables()
        {
            dataSet.Tables["Flights"].Clear();
            dataSet.Tables["Passengers"].Clear();
            dataSet.Tables["Tickets"].Clear();

            flightsAdapter.Fill(dataSet, "Flights");
            passengersAdapter.Fill(dataSet, "Passengers");
            ticketsAdapter.Fill(dataSet, "Tickets");
        }
    }

    public class AddTicketForm : Form
    {
        private TextBox txtFlight;
        private TextBox txtFrom;
        private TextBox txtTo;
        private TextBox txtPrice;
        private TextBox txtPassenger;
        private TextBox txtPassport;
        private TextBox txtPhone;
        private TextBox txtEmail;
        private TextBox txtSeat;

        public string FlightNumber => txtFlight.Text.Trim();
        public string FromCity => txtFrom.Text.Trim();
        public string ToCity => txtTo.Text.Trim();
        public string PriceText => txtPrice.Text.Trim();
        public string PassengerName => txtPassenger.Text.Trim();
        public string Passport => txtPassport.Text.Trim();
        public string Phone => txtPhone.Text.Trim();
        public string Email => txtEmail.Text.Trim();
        public string SeatNumber => txtSeat.Text.Trim();

        public AddTicketForm()
        {
            BuildInterface();
        }

        private void BuildInterface()
        {
            Text = "Додавання авіаквитка";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(500, 620);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.White;
            Font = new Font("Segoe UI", 10);

            int y = 20;

            txtFlight = AddField("Номер рейсу:", "PS500", y);
            y += 55;

            txtFrom = AddField("Звідки:", "Київ", y);
            y += 55;

            txtTo = AddField("Куди:", "Одеса", y);
            y += 55;

            txtPrice = AddField("Ціна квитка:", "2100", y);
            y += 55;

            txtPassenger = AddField("ПІБ пасажира:", "Новий Пасажир", y);
            y += 55;

            txtPassport = AddField("Паспорт:", "EE123456", y);
            y += 55;

            txtPhone = AddField("Телефон:", "+380991112233", y);
            y += 55;

            txtEmail = AddField("Email:", "user@mail.com", y);
            y += 55;

            txtSeat = AddField("Місце:", "10A", y);
            y += 70;

            Button btnOk = new Button();
            btnOk.Text = "Додати";
            btnOk.Location = new Point(95, y);
            btnOk.Size = new Size(140, 40);
            btnOk.BackColor = Color.FromArgb(235, 242, 255);
            btnOk.FlatStyle = FlatStyle.Flat;
            btnOk.Click += ConfirmAdd;
            Controls.Add(btnOk);

            Button btnCancel = new Button();
            btnCancel.Text = "Скасувати";
            btnCancel.Location = new Point(255, y);
            btnCancel.Size = new Size(140, 40);
            btnCancel.BackColor = Color.FromArgb(240, 240, 240);
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;
            Controls.Add(btnCancel);
        }

        private TextBox AddField(string labelText, string defaultText, int y)
        {
            Label label = new Label();
            label.Text = labelText;
            label.Location = new Point(30, y);
            label.Size = new Size(160, 25);
            Controls.Add(label);

            TextBox box = new TextBox();
            box.Location = new Point(190, y - 2);
            box.Size = new Size(250, 30);
            box.Text = defaultText;
            Controls.Add(box);

            return box;
        }

        private void ConfirmAdd(object sender, EventArgs e)
        {
            if (FlightNumber.Length == 0 || FromCity.Length == 0 || ToCity.Length == 0 ||
                PriceText.Length == 0 || PassengerName.Length == 0 || SeatNumber.Length == 0)
            {
                MessageBox.Show("Заповніть основні поля.");
                return;
            }

            DialogResult = DialogResult.OK;
        }
    }
}