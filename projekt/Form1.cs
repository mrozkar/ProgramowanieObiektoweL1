using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;

namespace CRUD2
{
    public partial class Form1 : Form
    {
        private StoreContext _db = new StoreContext();
        private TabControl tabControl;
        private DataGridView gridProducts, gridUsers;
        private TextBox txtSearch;

        public Form1()
        {
            InitializeComponent();
            InitializeUI();

            try
            {
                _db.Database.EnsureCreated(); // Tworzy bazê, jeœli nie istnieje
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("B³¹d bazy danych przy starcie: " + ex.Message);
            }
        }

        private void InitializeUI()
        {
            this.Text = "Sklep Spo¿ywczy - System Zarz¹dzania (CRUD)";
            this.Size = new System.Drawing.Size(1000, 750);
            this.StartPosition = FormStartPosition.CenterScreen;

            TableLayoutPanel mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                ColumnCount = 1
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 65F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 75F));

            // wyszukiwarka
            Panel pnlSearch = new Panel { Dock = DockStyle.Fill, BackColor = System.Drawing.Color.FromArgb(230, 230, 230) };
            Label lblSearch = new Label { Text = "Wyszukaj produkt:", Top = 22, Left = 20, Width = 120 };
            txtSearch = new TextBox { Left = 145, Top = 20, Width = 300 };
            txtSearch.TextChanged += (s, e) => SearchData();
            pnlSearch.Controls.Add(lblSearch);
            pnlSearch.Controls.Add(txtSearch);

            // zak³adki
            tabControl = new TabControl { Dock = DockStyle.Fill, Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold) };

            TabPage tpProducts = new TabPage("    PRODUKTY    ");
            gridProducts = CreateStyledGridView();
            gridProducts.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.Navy;
            gridProducts.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            gridProducts.DataError += (s, e) => { e.ThrowException = false; };
            SetupCategoryComboBox();
            tpProducts.Controls.Add(gridProducts);

            TabPage tpUsers = new TabPage("    KLIENCI    ");
            gridUsers = CreateStyledGridView();
            gridUsers.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.DarkOliveGreen;
            gridUsers.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            tpUsers.Controls.Add(gridUsers);

            tabControl.TabPages.Add(tpProducts);
            tabControl.TabPages.Add(tpUsers);

            // przyciski
            Panel pnlButtons = new Panel { Dock = DockStyle.Fill, BackColor = System.Drawing.Color.WhiteSmoke };

            Button btnSave = new Button { Text = "ZAPISZ ZMIANY", Width = 180, Height = 45, Top = 12, Left = 20, BackColor = System.Drawing.Color.PaleGreen, FlatStyle = FlatStyle.Flat };
            btnSave.Click += (s, e) => SaveToDatabase();

            Button btnAdd = new Button { Text = "DODAJ REKORD", Width = 150, Height = 45, Top = 12, Left = 210 };
            btnAdd.Click += (s, e) => AddNewRecord();

            // Przycisk USUÑ REKORD
            Button btnDelete = new Button { Text = "USUÑ REKORD", Width = 150, Height = 45, Top = 12, Left = 370, BackColor = System.Drawing.Color.LightCoral, FlatStyle = FlatStyle.Flat };
            btnDelete.Click += (s, e) => DeleteSelectedRecord();

            pnlButtons.Controls.Add(btnSave);
            pnlButtons.Controls.Add(btnAdd);
            pnlButtons.Controls.Add(btnDelete);

            mainLayout.Controls.Add(pnlSearch, 0, 0);
            mainLayout.Controls.Add(tabControl, 0, 1);
            mainLayout.Controls.Add(pnlButtons, 0, 2);
            this.Controls.Add(mainLayout);
        }

        private void DeleteSelectedRecord()
        {
            DataGridView activeGrid = tabControl.SelectedIndex == 0 ? gridProducts : gridUsers;

            if (activeGrid.CurrentRow != null && activeGrid.CurrentRow.DataBoundItem != null)
            {
                var item = activeGrid.CurrentRow.DataBoundItem;
                var res = MessageBox.Show("Czy na pewno chcesz usun¹æ zaznaczony rekord?", "Potwierdzenie", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (res == DialogResult.Yes)
                {
                    if (tabControl.SelectedIndex == 0) _db.Products.Remove((Product)item);
                    else _db.Users.Remove((User)item);

                    MessageBox.Show("Rekord zosta³ usuniêty z listy. Kliknij 'Zapisz zmiany', aby potwierdziæ w bazie danych.");
                }
            }
            else
            {
                MessageBox.Show("Proszê najpierw zaznaczyæ rekord do usuniêcia.");
            }
        }

        private void SetupCategoryComboBox()
        {
            _db.Categories.Load();
            DataGridViewComboBoxColumn combo = new DataGridViewComboBoxColumn
            {
                HeaderText = "Kategoria",
                DataPropertyName = "CategoryID",
                DataSource = _db.Categories.Local.ToBindingList(),
                DisplayMember = "CategoryName",
                ValueMember = "CategoryID",
                Name = "ColCategory",
                FlatStyle = FlatStyle.Flat,
                DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox
            };
            gridProducts.Columns.Add(combo);
        }

        private DataGridView CreateStyledGridView()
        {
            return new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = System.Drawing.Color.White,
                RowHeadersVisible = false,
                ColumnHeadersHeight = 45, 
                EnableHeadersVisualStyles = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
        }

        private void LoadData()
        {
            _db.Products.Load();
            _db.Users.Load();
            gridProducts.DataSource = _db.Products.Local.ToBindingList();
            gridUsers.DataSource = _db.Users.Local.ToBindingList();

            
            if (gridProducts.Columns["ProductID"] != null) gridProducts.Columns["ProductID"].ReadOnly = true;
            if (gridUsers.Columns["UserID"] != null) gridUsers.Columns["UserID"].ReadOnly = true;

            // ukrywanie kolumn technicznych
            if (gridProducts.Columns["Category"] != null) gridProducts.Columns["Category"].Visible = false;
            if (gridProducts.Columns["CategoryID"] != null) gridProducts.Columns["CategoryID"].Visible = false;
        }

        private void SaveToDatabase()
        {
            try
            {
                this.Validate();
                _db.SaveChanges();
                MessageBox.Show("Dane zapisane pomyœlnie!", "Sukces");
            }
            catch (Exception ex)
            {
                string msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                MessageBox.Show("B³¹d zapisu: " + msg);
            }
        }

        private void AddNewRecord()
        {
            if (tabControl.SelectedIndex == 0) _db.Products.Add(new Product { ProductName = "Nowy Produkt", Price = 0 });
            else _db.Users.Add(new User { FirstName = "Imiê", LastName = "Nazwisko" });
        }

        private void SearchData()
        {
            if (txtSearch == null || gridProducts == null || _db?.Products == null) return;
            string term = txtSearch.Text.Trim().ToLower();
            gridProducts.DataSource = string.IsNullOrEmpty(term)
                ? _db.Products.Local.ToBindingList()
                : _db.Products.Local.Where(x => x.ProductName != null && x.ProductName.ToLower().Contains(term)).ToList();
        }
    }
}