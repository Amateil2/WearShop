using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace WearShop
{
    /// <summary>
    /// Форма для управления справочниками (категории, поставщики, роли, пункты выдачи).
    /// </summary>
    public partial class Reference : Form
    {
        /// <summary>
        /// Строка подключения к базе данных.
        /// </summary>
        string connectionString2 = $"host={Values.DBHost};uid={Values.DBUser};pwd={Values.DBPassword};database=db30;port=3306;";
        /// <summary>
        /// Список для хранения всех строк DataGridView (не используется в текущей реализации).
        /// </summary>
        private List<DataGridViewRow> allRows1 = new List<DataGridViewRow>();
        /// <summary>
        /// Конструктор формы справочников.
        /// </summary>
        public Reference()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Обработчик нажатия кнопки "Назад".  Переходит к форме администратора.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void button2_Click(object sender, EventArgs e)
        {
            Administrator am = new Administrator();
            am.Show();
            this.Hide();
        }
        /// <summary>
        /// Заполняет DataGridView данными из базы данных для таблицы "Категории".
        /// </summary>
        /// <param name="strCmd">SQL-запрос для получения данных о категориях.</param>
        public void FillDataGridCat(string strCmd)
        {
            try
            {
                MySqlConnection con = new MySqlConnection(connectionString2);
                con.Open();
                MySqlCommand command = new MySqlCommand(strCmd, con);
                MySqlDataReader rdr = command.ExecuteReader();
                for (int i = 0; i < dataGridView_Category.Rows.Count; ++i)
                {
                    dataGridView_Category.Rows[i].Visible = true;
                }
                //allRows1.Clear();
                dataGridView_Category.Rows.Clear();
                dataGridView_Category.Columns.Clear();
                dataGridView_Category.AutoResizeColumns();
                dataGridView_Category.AutoResizeRows();
                dataGridView_Category.ReadOnly = true;
                dataGridView_Category.AllowUserToAddRows = false;


                dataGridView_Category.Columns.Add("CategoryID", "ID роли");
                dataGridView_Category.Columns.Add("CategoryName", "Роль");
                dataGridView_Category.Columns["CategoryID"].Visible = false;

                DataGridViewButtonColumn buttonDel = new DataGridViewButtonColumn();
                buttonDel.Name = "Удалить";
                buttonDel.HeaderText = "Удалить";
                buttonDel.Text = "Удалить";
                buttonDel.UseColumnTextForButtonValue = true;
                dataGridView_Category.Columns.Add(buttonDel);

                while (rdr.Read())
                {
                    int rowIndex = dataGridView_Category.Rows.Add();
                    DataGridViewRow row = dataGridView_Category.Rows[rowIndex];
                    row.Cells["CategoryID"].Value = rdr[0];
                    row.Cells["CategoryName"].Value = rdr[1];

                }
                dataGridView_Category.Columns["CategoryID"].Visible = false;
                dataGridView_Category.ReadOnly = true;
                dataGridView_Category.AllowUserToAddRows = false;
                dataGridView_Category.AllowUserToDeleteRows = false;
                dataGridView_Category.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView_Category.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                dataGridView_Category.AutoGenerateColumns = false;
                dataGridView_Category.RowHeadersVisible = false;
                con.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка: {ex}");
            }
        }/// <summary>
         /// Заполняет DataGridView данными из базы данных для таблицы "Поставщики".
         /// </summary>
         /// <param name="strCmd">SQL-запрос для получения данных о поставщиках.</param>
        public void FillDataGridSup(string strCmd)
        {
            try
            {
                MySqlConnection con = new MySqlConnection(connectionString2);
                con.Open();
                MySqlCommand command = new MySqlCommand(strCmd, con);
                MySqlDataReader rdr = command.ExecuteReader();
                for (int i = 0; i < dataGridView_Suppliers.Rows.Count; ++i)
                {
                    dataGridView_Suppliers.Rows[i].Visible = true;
                }
                //allRows1.Clear();
                dataGridView_Suppliers.Rows.Clear();
                dataGridView_Suppliers.Columns.Clear();
                dataGridView_Suppliers.AutoResizeColumns();
                dataGridView_Suppliers.AutoResizeRows();
                dataGridView_Suppliers.ReadOnly = true;
                dataGridView_Suppliers.AllowUserToAddRows = false;


                dataGridView_Suppliers.Columns.Add("SupplierID", "ID поставщика");
                dataGridView_Suppliers.Columns.Add("SupplierName", "Поставщик");

                DataGridViewButtonColumn buttonDel = new DataGridViewButtonColumn();
                buttonDel.Name = "Удалить";
                buttonDel.HeaderText = "Удалить";
                buttonDel.Text = "Удалить";
                buttonDel.UseColumnTextForButtonValue = true;
                dataGridView_Suppliers.Columns.Add(buttonDel);

                while (rdr.Read())
                {
                    int rowIndex = dataGridView_Suppliers.Rows.Add();
                    DataGridViewRow row = dataGridView_Suppliers.Rows[rowIndex];
                    row.Cells["SupplierID"].Value = rdr[0];
                    row.Cells["SupplierName"].Value = rdr[1];

                }
                dataGridView_Suppliers.Columns["SupplierID"].Visible = false;
                dataGridView_Suppliers.ReadOnly = true;
                dataGridView_Suppliers.AllowUserToAddRows = false;
                dataGridView_Suppliers.AllowUserToDeleteRows = false;
                dataGridView_Suppliers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView_Suppliers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                dataGridView_Suppliers.AutoGenerateColumns = false;
                dataGridView_Suppliers.RowHeadersVisible = false;
                con.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка: {ex}");
            }
        }
        /// <summary>
        /// Заполняет DataGridView данными из базы данных для таблицы "Роли".
        /// </summary>
        /// <param name="strCmd">SQL-запрос для получения данных о ролях.</param>
        public void FillDataGridRole(string strCmd)
        {
            try
            {
                MySqlConnection con = new MySqlConnection(connectionString2);
                con.Open();
                MySqlCommand command = new MySqlCommand(strCmd, con);
                MySqlDataReader rdr = command.ExecuteReader();
                for (int i = 0; i < dataGridView_Role.Rows.Count; ++i)
                {
                    dataGridView_Role.Rows[i].Visible = true;
                }
                //allRows1.Clear();
                dataGridView_Role.Rows.Clear();
                dataGridView_Role.Columns.Clear();
                dataGridView_Role.AutoResizeColumns();
                dataGridView_Role.AutoResizeRows();
                dataGridView_Role.ReadOnly = true;
                dataGridView_Role.AllowUserToAddRows = false;


                dataGridView_Role.Columns.Add("RoleID", "ID роли");
                dataGridView_Role.Columns.Add("RoleName", "Роль");

                DataGridViewButtonColumn buttonDel = new DataGridViewButtonColumn();
                buttonDel.Name = "Удалить";
                buttonDel.HeaderText = "Удалить";
                buttonDel.Text = "Удалить";
                buttonDel.UseColumnTextForButtonValue = true;
                dataGridView_Role.Columns.Add(buttonDel);

                while (rdr.Read())
                {
                    int rowIndex = dataGridView_Role.Rows.Add();
                    DataGridViewRow row = dataGridView_Role.Rows[rowIndex];
                    row.Cells["RoleID"].Value = rdr[0];
                    row.Cells["RoleName"].Value = rdr[1];

                }
                dataGridView_Role.Columns["RoleID"].Visible = false;
                dataGridView_Role.ReadOnly = true;
                dataGridView_Role.AllowUserToAddRows = false;
                dataGridView_Role.AllowUserToDeleteRows = false;
                dataGridView_Role.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView_Role.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                dataGridView_Role.AutoGenerateColumns = false;
                dataGridView_Role.RowHeadersVisible = false;
                con.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка: {ex}");
            }
        }
        /// <summary>
        /// Заполняет DataGridView данными из базы данных для таблицы "Пункты выдачи".
        /// </summary>
        /// <param name="strCmd">SQL-запрос для получения данных о пунктах выдачи.</param>
        public void FillDataGridPickUp(string strCmd)
        {
            try
            {
                MySqlConnection con = new MySqlConnection(connectionString2);
                con.Open();
                MySqlCommand command = new MySqlCommand(strCmd, con);
                MySqlDataReader rdr = command.ExecuteReader();
                for (int i = 0; i < dataGridView_PickUp.Rows.Count; ++i)
                {
                    dataGridView_PickUp.Rows[i].Visible = true;
                }
                //allRows1.Clear();
                dataGridView_PickUp.Rows.Clear();
                dataGridView_PickUp.Columns.Clear();
                dataGridView_PickUp.AutoResizeColumns();
                dataGridView_PickUp.AutoResizeRows();
                dataGridView_PickUp.ReadOnly = true;
                dataGridView_PickUp.AllowUserToAddRows = false;


                dataGridView_PickUp.Columns.Add("PickUpPointID", "ID магазина");
                dataGridView_PickUp.Columns.Add("OrderPickUpPointName", "Адресс магазина");

                DataGridViewButtonColumn buttonDel = new DataGridViewButtonColumn();
                buttonDel.Name = "Удалить";
                buttonDel.HeaderText = "Удалить";
                buttonDel.Text = "Удалить";
                buttonDel.UseColumnTextForButtonValue = true;
                dataGridView_PickUp.Columns.Add(buttonDel);

                while (rdr.Read())
                {
                    int rowIndex = dataGridView_PickUp.Rows.Add();
                    DataGridViewRow row = dataGridView_PickUp.Rows[rowIndex];
                    row.Cells["PickUpPointID"].Value = rdr[0];
                    row.Cells["OrderPickUpPointName"].Value = rdr[1];

                }
                dataGridView_PickUp.Columns["PickUpPointID"].Visible = false;
                dataGridView_PickUp.ReadOnly = true;
                dataGridView_PickUp.AllowUserToAddRows = false;
                dataGridView_PickUp.AllowUserToDeleteRows = false;
                dataGridView_PickUp.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView_PickUp.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                dataGridView_PickUp.AutoGenerateColumns = false;
                dataGridView_PickUp.RowHeadersVisible = false;
                con.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка: {ex}");
            }
        }
        /// <summary>
        /// Обработчик события загрузки формы "Справочники".
        /// Заполняет все DataGridView данными из соответствующих таблиц базы данных.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void Reference_Load(object sender, EventArgs e)
        {
            FillDataGridCat("SELECT " +
            "CategoryID AS 'ID категории'," +
            "CategoryName AS 'Категория'" +
            "FROM Category ");

            FillDataGridSup("SELECT " +
            "SupplierID AS 'ID поставщика'," +
            "SupplierName AS 'Поставщик'" +
            "FROM Supplier ");

            FillDataGridRole("SELECT " +
            "RoleID AS 'ID роли'," +
            "RoleName AS 'Роль'" +
            "FROM Role ");

            FillDataGridPickUp("SELECT " +
            "PickUpPointID AS 'ID магазина'," +
            "OrderPickUpPointName AS 'Адресс магазина'" +
            "FROM Pickuppoint ");
        }
        /// <summary>
        /// Обработчик клика по ячейке в DataGridView для таблицы "Категории".
        /// Заполняет текстовое поле именем выбранной категории.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void dataGridView_Category_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView_Category.Rows[e.RowIndex];
                textBoxCategoryName.Text = row.Cells["CategoryName"].Value.ToString();
            }
        }
        /// <summary>
        /// Обработчик нажатия кнопки "Обновить категорию".
        /// Обновляет имя категории в базе данных.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView_Category.CurrentRow != null)
            {
                int categoryId = Convert.ToInt32(dataGridView_Category.CurrentRow.Cells["CategoryID"].Value);
                string newCategoryName = textBoxCategoryName.Text;

                if (!string.IsNullOrWhiteSpace(newCategoryName))
                {
                    UpdateCategory(categoryId, newCategoryName);
                }
                else
                {
                    MessageBox.Show("Введите корректное имя категории.");
                }
            }
        }
        /// <summary>
        /// Обновляет имя категории в базе данных.
        /// </summary>
        /// <param name="categoryId">Идентификатор категории.</param>
        /// <param name="newCategoryName">Новое имя категории.</param>
        private void UpdateCategory(int categoryId, string newCategoryName)
        {
                try
                {
                    MySqlConnection connection = new MySqlConnection(connectionString2);
                    connection.Open();
                    string query = "UPDATE category SET CategoryName = @CategoryName WHERE CategoryID = @CategoryID";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@CategoryName", newCategoryName);
                    command.Parameters.AddWithValue("@CategoryID", categoryId);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Категория успешно обновлена.");
                        FillDataGridCat("SELECT " +
                        "CategoryID AS 'ID категории'," +
                        "CategoryName AS 'Категория'" +
                        "FROM Category");
                    }
                    else
                    {
                        MessageBox.Show("Не удалось обновить категорию.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при обновлении данных: " + ex.Message);
                }
            
        }
        /// <summary>
        /// Обработчик нажатия кнопки "Добавить категорию".
        /// Добавляет новую категорию в базу данных.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void button3_Click(object sender, EventArgs e)
        {
            string categoryName = textBoxCategoryName.Text.Trim();

            if (string.IsNullOrEmpty(categoryName))
            {
                MessageBox.Show("Имя категории не может быть пустым.");
                return;
            }
                try
                {
                    MySqlConnection connection = new MySqlConnection(connectionString2);
                    connection.Open();

                string checkQuery = "SELECT COUNT(*) FROM Category WHERE CategoryName = @CategoryName";
                MySqlCommand command1 = new MySqlCommand(checkQuery, connection);
                command1.Parameters.AddWithValue("@CategoryName", categoryName);

                int count = Convert.ToInt32(command1.ExecuteScalar());
                if (count > 0)
                {
                    MessageBox.Show("Категория с таким именем уже существует.");
                }
                else
                {
                    MySqlCommand command = new MySqlCommand("INSERT INTO Category (CategoryName) VALUES (@categoryName)", connection);
                    command.Parameters.AddWithValue("@categoryName", categoryName);
                    command.ExecuteNonQuery();
                    MessageBox.Show("Категория добавлена успешно.");

                    FillDataGridCat("SELECT " +
                    "CategoryID AS 'ID категории'," +
                    "CategoryName AS 'Категория'" +
                    "FROM Category");

                    textBoxCategoryName.Clear(); // Очищаем текстовое поле после добавления
                }

                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при добавлении категории: " + ex.Message);
                }
            
        }
        /// <summary>
        /// Обработчик нажатия кнопки "Обновить поставщика".
        /// Обновляет имя поставщика в базе данных.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void button5_Click(object sender, EventArgs e)
        {
            if (dataGridView_Suppliers.CurrentRow != null)
            {
                int categoryId = Convert.ToInt32(dataGridView_Suppliers.CurrentRow.Cells["SupplierID"].Value);
                string newCategoryName = textBoxSup.Text;

                if (!string.IsNullOrWhiteSpace(newCategoryName))
                {
                    UpdateSupplier(categoryId, newCategoryName);
                }
                else
                {
                    MessageBox.Show("Введите корректное имя поставщика.");
                }
            }
        }
        /// <summary>
        /// Обновляет имя поставщика в базе данных.
        /// </summary>
        /// <param name="categoryId">Идентификатор поставщика.</param>
        /// <param name="newCategoryName">Новое имя поставщика.</param>
        private void UpdateSupplier(int categoryId, string newCategoryName)
        {
            try
            {
                MySqlConnection connection = new MySqlConnection(connectionString2);
                connection.Open();
                string query = "UPDATE supplier SET SupplierName = @SupplierName WHERE SupplierID = @SupplierID";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@SupplierName", newCategoryName);
                command.Parameters.AddWithValue("@SupplierID", categoryId);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Поставщик успешно обновлен.");
                    FillDataGridSup("SELECT " +
            "SupplierID AS 'ID поставщика'," +
            "SupplierName AS 'Поставщик'" +
            "FROM Supplier ");
                }
                else
                {
                    MessageBox.Show("Не удалось обновить поставщика.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при обновлении данных: " + ex.Message);
            }

        }
        /// <summary>
        /// Обработчик нажатия кнопки "Добавить поставщика".
        /// Добавляет нового поставщика в базу данных.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void button4_Click(object sender, EventArgs e)
        {
            string categoryName = textBoxSup.Text.Trim();

            if (string.IsNullOrEmpty(categoryName))
            {
                MessageBox.Show("Имя категории не может быть пустым.");
                return;
            }
            try
            {
                MySqlConnection connection = new MySqlConnection(connectionString2);
                connection.Open();
                string checkQuery = "SELECT COUNT(*) FROM Supplier WHERE SupplierName = @SupplierName";
                MySqlCommand command1 = new MySqlCommand(checkQuery, connection);
                command1.Parameters.AddWithValue("@SupplierName", categoryName);

                int count = Convert.ToInt32(command1.ExecuteScalar());
                if (count > 0)
                {
                    MessageBox.Show("Поставщик с таким именем уже существует.");
                }
                else
                {
                    MySqlCommand command = new MySqlCommand("INSERT INTO Supplier (SupplierName) VALUES (@SupplierName)", connection);
                    command.Parameters.AddWithValue("@SupplierName", categoryName);
                    command.ExecuteNonQuery();
                    MessageBox.Show("Поставщик добавлена успешно.");

                    FillDataGridSup("SELECT " +
                "SupplierID AS 'ID поставщика'," +
                "SupplierName AS 'Поставщик'" +
                "FROM Supplier ");

                    textBoxSup.Clear(); // Очищаем текстовое поле после добавления
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении поставщика: " + ex.Message);
            }
        }
        /// <summary>
        /// Обработчик клика по ячейке в DataGridView для таблицы "Поставщики".
        /// Заполняет текстовое поле именем выбранного поставщика.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void dataGridView_Suppliers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView_Suppliers.Rows[e.RowIndex];
                textBoxSup.Text = row.Cells["SupplierName"].Value.ToString();
            }
        }
        /// <summary>
        /// Обработчик клика по ячейке в DataGridView для таблицы "Роли".
        /// Заполняет текстовое поле именем выбранной роли.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void dataGridView_Role_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView_Role.Rows[e.RowIndex];
                textBox_Role.Text = row.Cells["RoleName"].Value.ToString();
            }
        }
        /// <summary>
        /// Обработчик нажатия кнопки "Обновить роль".
        /// Обновляет имя роли в базе данных.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void button7_Click(object sender, EventArgs e)
        {
            if (dataGridView_Role.CurrentRow != null)
            {
                int categoryId = Convert.ToInt32(dataGridView_Role.CurrentRow.Cells["RoleID"].Value);
                string newCategoryName = textBox_Role.Text;

                if (!string.IsNullOrWhiteSpace(newCategoryName))
                {
                    UpdateRole(categoryId, newCategoryName);
                }
                else
                {
                    MessageBox.Show("Введите корректное имя Роли.");
                }
            }
        }
        /// <summary>
        /// Обработчик нажатия кнопки "Добавить роль".
        /// Добавляет новую роль в базу данных.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void button6_Click(object sender, EventArgs e)
        {
            string categoryName = textBox_Role.Text.Trim();

            if (string.IsNullOrEmpty(categoryName))
            {
                MessageBox.Show("Роль не может быть пустым.");
                return;
            }
            try
            {
                MySqlConnection connection = new MySqlConnection(connectionString2);
                connection.Open();
                string checkQuery = "SELECT COUNT(*) FROM Role WHERE RoleName = @RoleName";
                MySqlCommand command1 = new MySqlCommand(checkQuery, connection);
                command1.Parameters.AddWithValue("@RoleName", categoryName);

                int count = Convert.ToInt32(command1.ExecuteScalar());
                if (count > 0)
                {
                    MessageBox.Show("Роль с таким именем уже существует.");
                }
                else
                {
                    MySqlCommand command = new MySqlCommand("INSERT INTO Role (RoleName) VALUES (@RoleName)", connection);
                    command.Parameters.AddWithValue("@RoleName", categoryName);
                    command.ExecuteNonQuery();
                    MessageBox.Show("Роль добавлена успешно.");

                    FillDataGridRole("SELECT " +
                "RoleID AS 'ID роли'," +
                "RoleName AS 'Роль'" +
                "FROM Role ");

                    textBox_Role.Clear(); // Очищаем текстовое поле после добавления
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении Роли: " + ex.Message);
            }
        }
        /// <summary>
        /// Обновляет имя роли в базе данных.
        /// </summary>
        /// <param name="categoryId">Идентификатор роли.</param>
        /// <param name="newCategoryName">Новое имя роли.</param>
        private void UpdateRole(int categoryId, string newCategoryName)
        {
            try
            {
                MySqlConnection connection = new MySqlConnection(connectionString2);
                connection.Open();
                string query = "UPDATE role SET RoleName = @RoleName WHERE RoleID = @RoleID";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@RoleName", newCategoryName);
                command.Parameters.AddWithValue("@RoleID", categoryId);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Роль успешно обновлен.");
                    FillDataGridRole("SELECT " +
            "RoleID AS 'ID роли'," +
            "RoleName AS 'Роль'" +
            "FROM Role ");
                }
                else
                {
                    MessageBox.Show("Не удалось обновить роль.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при обновлении данных: " + ex.Message);
            }

        }
        /// <summary>
        /// Обновляет имя пункта выдачи в базе данных.
        /// </summary>
        /// <param name="categoryId">Идентификатор пункта выдачи.</param>
        /// <param name="newCategoryName">Новое имя пункта выдачи.</param>
        private void UpdatePickUp(int categoryId, string newCategoryName)
        {
            try
            {
                MySqlConnection connection = new MySqlConnection(connectionString2);
                connection.Open();
                string query = "UPDATE pickuppoint SET OrderPickUpPointName = @OrderPickUpPointName WHERE PickUpPointID = @PickUpPointID";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@OrderPickUpPointName", newCategoryName);
                command.Parameters.AddWithValue("@PickUpPointID", categoryId);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Пункт выдачи успешно обновлен.");
                    FillDataGridPickUp("SELECT " +
            "PickUpPointID AS 'ID магазина'," +
            "OrderPickUpPointName AS 'Адресс магазина'" +
            "FROM Pickuppoint ");
                }
                else
                {
                    MessageBox.Show("Не удалось обновить пункт выдачи.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при обновлении данных: " + ex.Message);
            }

        }
        /// <summary>
        /// Обработчик клика по ячейке в DataGridView для таблицы "Пункты выдачи".
        /// Заполняет текстовое поле именем выбранного пункта выдачи.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void dataGridView_PickUp_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView_PickUp.Rows[e.RowIndex];
                textBox_pickUp.Text = row.Cells["OrderPickUpPointName"].Value.ToString();
            }
        }
        /// <summary>
        /// Обработчик нажатия кнопки "Обновить пункт выдачи".
        /// Обновляет имя пункта выдачи в базе данных.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void button9_Click(object sender, EventArgs e)
        {
            if (dataGridView_PickUp.CurrentRow != null)
            {
                int categoryId = Convert.ToInt32(dataGridView_PickUp.CurrentRow.Cells["PickUpPointID"].Value);
                string newCategoryName = textBox_pickUp.Text;

                if (!string.IsNullOrWhiteSpace(newCategoryName))
                {
                    UpdatePickUp(categoryId, newCategoryName);
                }
                else
                {
                    MessageBox.Show("Введите корректное имя пункта выдачи.");
                }
            }
        }
        /// <summary>
        /// Обработчик нажатия кнопки "Добавить пункт выдачи".
        /// Добавляет новый пункт выдачи в базу данных.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void button8_Click(object sender, EventArgs e)
        {
            string categoryName = textBox_pickUp.Text.Trim();

            if (string.IsNullOrEmpty(categoryName))
            {
                MessageBox.Show("Пункт выдачи не может быть пустым.");
                return;
            }
            try
            {
                MySqlConnection connection = new MySqlConnection(connectionString2);
                connection.Open();
                string checkQuery = "SELECT COUNT(*) FROM Pickuppoint WHERE OrderPickUpPointName = @OrderPickUpPointName";
                MySqlCommand command1 = new MySqlCommand(checkQuery, connection);
                command1.Parameters.AddWithValue("@OrderPickUpPointName", categoryName);

                int count = Convert.ToInt32(command1.ExecuteScalar());
                if (count > 0)
                {
                    MessageBox.Show("Пункт выдачи с таким именем уже существует.");
                }
                else
                {
                    MySqlCommand command = new MySqlCommand("INSERT INTO Pickuppoint (OrderPickUpPointName) VALUES (@OrderPickUpPointName)", connection);
                    command.Parameters.AddWithValue("@OrderPickUpPointName", categoryName);
                    command.ExecuteNonQuery();
                    MessageBox.Show("Пункт выдачи добавлен успешно.");

                    FillDataGridPickUp("SELECT " +
                "PickUpPointID AS 'ID магазина'," +
                "OrderPickUpPointName AS 'Адресс магазина'" +
                "FROM Pickuppoint ");

                    textBox_pickUp.Clear(); // Очищаем текстовое поле после добавления
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении пункта выдачи: " + ex.Message);
            }
        }
        /// <summary>
        /// Обработчик клика по ячейке DataGridView для таблицы "Категории".
        /// Выполняет удаление категории при нажатии на кнопку "Удалить".
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void dataGridView_Category_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Проверяем, что нажата кнопка "Удалить"
            if (e.ColumnIndex == dataGridView_Category.Columns["Удалить"].Index && e.RowIndex >= 0)
            {
                // Получаем идентификатор записи
                int id = Convert.ToInt32(dataGridView_Category.Rows[e.RowIndex].Cells["CategoryID"].Value);

                // Подтверждение удаления
                DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите удалить эту запись?", "Подтверждение удаления", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    DeleteRecord(id, "Category", "CategoryID"); // Удаляем запись из базы данных
                    dataGridView_Category.Rows.RemoveAt(e.RowIndex); // Удаляем строку из DataGridView
                }
            }
        }
        /// <summary>
        /// Удаляет запись из указанной таблицы базы данных.
        /// </summary>
        /// <param name="id">Идентификатор записи.</param>
        /// <param name="table">Имя таблицы.</param>
        /// <param name="row">Имя столбца с идентификатором.</param>
        private void DeleteRecord(int id, string table, string row)
        {
            string query = $"DELETE FROM {table} WHERE {row} = @UserID";
            MySqlConnection con = new MySqlConnection(connectionString2);
            con.Open();
            MySqlCommand command = new MySqlCommand(query, con);
            command.Parameters.AddWithValue("@UserID", id);
            command.ExecuteNonQuery();
            con.Close();
        }
        /// <summary>
        /// Обработчик клика по ячейке DataGridView для таблицы "Поставщики".
        /// Выполняет удаление поставщика при нажатии на кнопку "Удалить".
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void dataGridView_Suppliers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Проверяем, что нажата кнопка "Удалить"
            if (e.ColumnIndex == dataGridView_Category.Columns["Удалить"].Index && e.RowIndex >= 0)
            {
                // Получаем идентификатор записи
                int id = Convert.ToInt32(dataGridView_Category.Rows[e.RowIndex].Cells["SupplierID"].Value);

                // Подтверждение удаления
                DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите удалить эту запись?", "Подтверждение удаления", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    DeleteRecord(id, "Supplier", "SupplierID"); // Удаляем запись из базы данных
                    dataGridView_Category.Rows.RemoveAt(e.RowIndex); // Удаляем строку из DataGridView
                }
            }
        }
        /// <summary>
        /// Обработчик клика по ячейке DataGridView для таблицы "Роль".
        /// Выполняет удаление роли при нажатии на кнопку "Удалить".
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void dataGridView_Role_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Проверяем, что нажата кнопка "Удалить"
            if (e.ColumnIndex == dataGridView_Category.Columns["Удалить"].Index && e.RowIndex >= 0)
            {
                // Получаем идентификатор записи
                int id = Convert.ToInt32(dataGridView_Category.Rows[e.RowIndex].Cells["RoleID"].Value);

                // Подтверждение удаления
                DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите удалить эту запись?", "Подтверждение удаления", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    DeleteRecord(id, "Role", "RoleID"); // Удаляем запись из базы данных
                    dataGridView_Category.Rows.RemoveAt(e.RowIndex); // Удаляем строку из DataGridView
                }
            }
        }
        /// <summary>
        /// Обработчик клика по ячейке DataGridView для таблицы "Пукнты выдачи".
        /// Выполняет удаление пункта выдачи при нажатии на кнопку "Удалить".
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void dataGridView_PickUp_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Проверяем, что нажата кнопка "Удалить"
            if (e.ColumnIndex == dataGridView_Category.Columns["Удалить"].Index && e.RowIndex >= 0)
            {
                // Получаем идентификатор записи
                int id = Convert.ToInt32(dataGridView_Category.Rows[e.RowIndex].Cells["PickUpPointID"].Value);

                // Подтверждение удаления
                DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите удалить эту запись?", "Подтверждение удаления", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    DeleteRecord(id, "Pickuppoint", "PickUpPointID"); // Удаляем запись из базы данных
                    dataGridView_Category.Rows.RemoveAt(e.RowIndex); // Удаляем строку из DataGridView
                }
            }
        }
    }
}
