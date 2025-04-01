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
    /// Форма для просмотра списка товаров.
    /// </summary>
    public partial class ViewProducts : Form
    {
        /// <summary>
        /// Строка подключения к базе данных.
        /// </summary>
        string connectionString2 = $"host={Values.DBHost};uid={Values.DBUser};pwd={Values.DBPassword};database=db30;port=3306;";

        /// <summary>
        /// Роль пользователя.
        /// </summary>
        private int role_user;

        /// <summary>
        /// Текущий заказ (словарь: артикул товара - количество).
        /// </summary>
        private Dictionary<string, int> currentOrder = new Dictionary<string, int>();

        /// <summary>
        /// Выбранный пункт выдачи.
        /// </summary>
        private string selectedPickUpPoint = "";

        //Пагинация
        /// <summary>
        /// Текущая страница.
        /// </summary>
        private int currentPage1 = 1;

        /// <summary>
        /// Количество строк на странице.
        /// </summary>
        private int rowsPerPage1 = 20;

        /// <summary>
        /// Общее количество строк на текущей странице.
        /// </summary>
        private int totalRows1 = 0;

        /// <summary>
        /// Общее количество записей в таблице.
        /// </summary>
        private int totalRecords;

        /// <summary>
        /// Список всех строк в DataGridView.
        /// </summary>
        private List<DataGridViewRow> allRows1 = new List<DataGridViewRow>();

        /// <summary>
        /// Конструктор класса ViewProducts.
        /// </summary>
        /// <param name="Role">Роль пользователя.</param>
        public ViewProducts(int Role)
        {
            InitializeComponent();
            role_user = Role;
            PopulateCategoryComboBox();
            if (Values.clearOrder == true)
            {
                currentOrder.Clear();
                Values.clearOrder = false;
            }
        }

        /// <summary>
        /// Заполняет DataGridView данными из базы данных.
        /// </summary>
        /// <param name="strCmd">SQL команда для выполнения.</param>
        /// <param name="categoryId">ID категории для фильтрации.</param>
        public void FillDataGrid(string strCmd, int categoryId)
        {
            try
            {
                MySqlConnection con = new MySqlConnection(connectionString2);
                con.Open();
                MySqlCommand command = new MySqlCommand(strCmd, con);
                if (categoryId != -1) // Если categoryId найден
                {
                    command.Parameters.AddWithValue("@categoryId", categoryId);
                }
                MySqlDataReader rdr = command.ExecuteReader();

                allRows1.Clear();
                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();

                DataGridViewImageColumn imageColumn = new DataGridViewImageColumn();
                imageColumn.Name = "ProductPhoto";
                imageColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;
                imageColumn.HeaderText = "Фото";




                dataGridView1.Columns.Add(imageColumn);
                dataGridView1.Columns["ProductPhoto"].Visible = true;

                dataGridView1.Columns["ProductPhoto"].Width = 100;
                dataGridView1.Columns.Add("ProductArticleNumber", "Артикул");
                dataGridView1.Columns["ProductArticleNumber"].Visible = false;
                dataGridView1.Columns.Add("ProductName", "Наименование");
                dataGridView1.Columns.Add("ProductUnit", "Единица измерения");
                dataGridView1.Columns["ProductUnit"].Visible = false;
                dataGridView1.Columns.Add("ProductCost", "Стоимость");
                dataGridView1.Columns["ProductCost"].Width = 200;
                dataGridView1.Columns.Add("ProductSupplier", "Поставщик");
                dataGridView1.Columns["ProductSupplier"].Visible = false;
                dataGridView1.Columns.Add("ProductCategory", "Категория");
                dataGridView1.Columns["ProductCategory"].Width = 250;
                dataGridView1.Columns.Add("ProductDiscountAmount", "Скидка");
                dataGridView1.Columns["ProductDiscountAmount"].Width = 100;
                dataGridView1.Columns.Add("ProductCount", "Количество на складе");
                dataGridView1.Columns["ProductCount"].Visible = false;
                dataGridView1.Columns.Add("ProductDescription", "Описание");
                dataGridView1.Columns["ProductDescription"].Width = 300;

                if (Values.UserRole != 1)
                {
                    DataGridViewButtonColumn buttonEdit = new DataGridViewButtonColumn();
                    buttonEdit.Name = "Редактировать";
                    buttonEdit.HeaderText = "Редактировать";
                    buttonEdit.Text = "Редактировать";
                    buttonEdit.UseColumnTextForButtonValue = true;
                    dataGridView1.Columns.Add(buttonEdit);
                }

                if (Values.UserRole != 1)
                {
                    DataGridViewButtonColumn buttonDel = new DataGridViewButtonColumn();
                    buttonDel.Name = "Удалить";
                    buttonDel.HeaderText = "Удалить";
                    buttonDel.Text = "Удалить";
                    buttonDel.UseColumnTextForButtonValue = true;
                    dataGridView1.Columns.Add(buttonDel);
                }
                if (Values.UserRole == 1)
                {
                    buttonAdd.Visible = false;
                }


                while (rdr.Read())
                {
                    string imsName = rdr[9].ToString(); // Replace with the actual column name for the image.
                    if (string.IsNullOrEmpty(imsName))
                    {
                        imsName = "zaglushka.jpg";
                    }
                    Image img = Image.FromFile(@"./photo/" + imsName);


                    int rowIndex = dataGridView1.Rows.Add();
                    DataGridViewRow row = dataGridView1.Rows[rowIndex];

                    row.Cells["ProductArticleNumber"].Value = rdr[0];
                    row.Cells["ProductName"].Value = rdr[1];
                    row.Cells["ProductUnit"].Value = rdr[2];
                    row.Cells["ProductCost"].Value = rdr[3];
                    row.Cells["ProductSupplier"].Value = rdr[4];
                    row.Cells["ProductCategory"].Value = rdr[5];
                    row.Cells["ProductDiscountAmount"].Value = rdr[6];
                    row.Cells["ProductCount"].Value = rdr[7];
                    row.Cells["ProductDescription"].Value = rdr[8];
                    row.Cells["ProductPhoto"].Value = img;

                    allRows1.Add(row);
                }
                dataGridView1.ReadOnly = true;
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.AllowUserToDeleteRows = false;
                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                dataGridView1.AutoGenerateColumns = false;
                dataGridView1.RowHeadersVisible = false;

                con.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка: {ex}");
            }
        }

        /// <summary>
        /// Обновляет данные в DataGridView, выполняя поиск, сортировку и фильтрацию.
        /// </summary>
        private void UpdateDataGrid()//поиск сортировка фильтр
        {
            string searchStr = textBoxSearch.Text;

            // Получение выбранной категории
            string selectedCategory = comboBox2.SelectedItem?.ToString(); // выбранная категория
            int categoryId = -1;



            string strCmd = "SELECT " +
            "ProductArticleNumber AS 'Артикул'," +
            "ProductName AS 'Наименование'," +
            "ProductUnit AS 'Единица измерения'," +
            "ProductCost AS 'Стоимость'," +
            "Supplier.SupplierName AS 'Поставщик'," +
            "Category.CategoryName AS 'Категория'," +
            "ProductDiscountAmount AS 'Скидка'," +
            "ProductCount AS 'Количество на складе'," +
            "ProductDescription AS 'Описание'," +
            "ProductPhoto " +
            "FROM Product " +
            "INNER JOIN Supplier ON Product.ProductSupplier = Supplier.SupplierID " +
            "INNER JOIN Category ON Product.ProductCategory = Category.CategoryID"; // начальная часть запроса

            // Поиск

            if (textBoxSearch.Text == "")
            {
                FillDataGrid(strCmd, categoryId);
            }
            if (!string.IsNullOrWhiteSpace(searchStr) && searchStr.Length >= 3)
            {
                strCmd += $" AND ProductName LIKE '%{searchStr}%'";
                FillDataGrid(strCmd, categoryId);
            }



            // фильтрация по категориям
            if (!string.IsNullOrWhiteSpace(selectedCategory))
            {
                if (comboBox2.SelectedIndex == 0)
                {
                    FillDataGrid(strCmd, categoryId);
                }
                else
                {
                    strCmd += $" AND ProductCategory = (SELECT CategoryID FROM Category WHERE CategoryName = '{selectedCategory}')";
                    FillDataGrid(strCmd, categoryId);
                }
            }
            //сортировка
            if (comboBox1.Text != "")
            {
                if (comboBox1.Text == "Без сортировки")
                {
                    FillDataGrid(strCmd, categoryId);
                }
                if (comboBox1.Text == "По убыванию")
                {
                    strCmd += $" ORDER BY ProductCost DESC";
                    FillDataGrid(strCmd, categoryId);
                }
                if (comboBox1.Text == "По возрастанию")
                {
                    strCmd += $" ORDER BY ProductCost ASC";
                    FillDataGrid(strCmd, categoryId);
                }
            }
            // Сортировка
            //strCmd += $" ORDER BY ProductCost {orderBy}";

            // Заполнение DataGrid

        }

        /// <summary>
        /// Заполняет ComboBox категориями из базы данных.
        /// </summary>
        private void PopulateCategoryComboBox()
        {
            comboBox2.Items.Add("Без фильтрации");
            using (MySqlConnection conn = new MySqlConnection(connectionString2))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT CategoryName FROM Category", conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    comboBox2.Items.Add(reader["CategoryName"].ToString());
                }
            }
        }

        /// <summary>
        /// Обработчик события изменения выбранного индекса в ComboBox для сортировки.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDataGrid();
        }

        /// <summary>
        /// Обработчик события изменения выбранного индекса в ComboBox для фильтрации по категориям.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDataGrid();
        }

        /// <summary>
        /// Обработчик нажатия на кнопку "Назад".
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void button2_Click(object sender, EventArgs e)
        {
            switch (role_user)
            {
                case 1:
                    Seller cl = new Seller();
                    cl.Show();
                    this.Hide();
                    break;
                case 3:
                    Administrator am = new Administrator();
                    am.Show();
                    this.Hide();
                    break;
                case 2:
                    Manager mm = new Manager();
                    mm.Show();
                    this.Hide();
                    break;
                default:
                    return;
            }
        }

        /// <summary>
        /// Показывает или скрывает кнопку "Просмотр заказа" в зависимости от содержимого текущего заказа.
        /// </summary>
        private void ShowOrderButton()
        {
            if (currentOrder.Count > 0)
            {
                if (!Controls.ContainsKey("viewOrderButton"))
                {
                    checkorder.Visible = true;
                }
            }
            else
            {
                checkorder.Visible = false;
            }
        }

        /// <summary>
        /// Обработчик нажатия на кнопку "Оформить заказ".
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (Values.clearOrder == true)
            {
                currentOrder.Clear();
                Values.clearOrder = false;
            }
            AddOrder orderForm = new AddOrder(currentOrder, selectedPickUpPoint);
            if (orderForm.ShowDialog() == DialogResult.OK)
            {
                currentOrder = orderForm.UpdatedOrder;
                selectedPickUpPoint = orderForm.SelectedPickUpPoint;
                ShowOrderButton();
            }
        }

        /// <summary>
        /// Обработчик события загрузки формы.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void ViewProducts_Load(object sender, EventArgs e)
        {
            if (Values.UserRole != 3 && Values.UserRole != 2)
            {
                ContextMenuStrip menu = new ContextMenuStrip();
                ToolStripMenuItem addToOrderMenuItem = new ToolStripMenuItem("Добавить к заказу");
                addToOrderMenuItem.Click += AddToOrderMenuItem_Click;
                menu.Items.Add(addToOrderMenuItem);
                dataGridView1.ContextMenuStrip = menu;
            }
            //Сортировка
            comboBox1.Items.Add("Без сортировки");
            comboBox1.Items.Add("По убыванию");
            comboBox1.Items.Add("По возрастанию");
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;

            FillDataGrid("SELECT " +
            "ProductArticleNumber AS 'Артикул'," +
            "ProductName AS 'Наименование'," +
            "ProductUnit AS 'Единица измерения'," +
            "ProductCost AS 'Стоимость'," +
            "Supplier.SupplierName AS 'Поставщик'," +
            "Category.CategoryName AS 'Категория'," +
            "ProductDiscountAmount AS 'Скидка'," +
            "ProductCount AS 'Количество на складе'," +
            "ProductDescription AS 'Описание'," +
            "ProductPhoto " +
            "FROM Product " +
            "INNER JOIN Supplier ON Product.ProductSupplier = Supplier.SupplierID " +
            "INNER JOIN Category ON Product.ProductCategory = Category.CategoryID", 1);

        }

        /// <summary>
        /// Обработчик события нажатия на пункт "Добавить к заказу" в контекстном меню.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void AddToOrderMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                string productArticleNumber = dataGridView1.CurrentRow.Cells["ProductArticleNumber"].Value.ToString();
                int productCount = Convert.ToInt32(dataGridView1.CurrentRow.Cells["ProductCount"].Value); // Получаем количество на складе

                if (currentOrder.ContainsKey(productArticleNumber))
                {
                    // Проверяем, не превысит ли добавление товара количество на складе
                    if (currentOrder[productArticleNumber] + 1 > productCount)
                    {
                        MessageBox.Show("Невозможно добавить больше товара, чем есть на складе.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return; // Прекращаем добавление товара
                    }
                    currentOrder[productArticleNumber]++;
                }
                else
                {
                    // Если товара еще нет в заказе, проверяем, есть ли он вообще на складе
                    if (productCount <= 0)
                    {
                        MessageBox.Show("Товара нет на складе.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return; // Прекращаем добавление товара
                    }
                    currentOrder[productArticleNumber] = 1;
                }

                // Отображаем кнопку "Просмотр заказа"
                ShowOrderButton();
            }
        }

        /// <summary>
        /// Обработчик события изменения текста в поле поиска.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void textBoxSearch_TextChanged(object sender, EventArgs e)
        {
            UpdateDataGrid();
        }

        /// <summary>
        /// Обработчик нажатия на кнопку "Добавить товар".
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void button1_Click_1(object sender, EventArgs e)
        {
            AddProduct AddProduct = new AddProduct(role_user);
            AddProduct.Show();
            this.Hide();
        }

        /// <summary>
        /// Обработчик события нажатия на ячейку в DataGridView.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)

        {
            if (Values.UserRole != 1)
            {
                // Проверяем, нажата ли кнопка в определенной колонке
                if (e.ColumnIndex == dataGridView1.Columns["Редактировать"].Index && e.RowIndex >= 0)
                {
                    // Получите данные из выбранной строки
                    string productId = Convert.ToString(dataGridView1.Rows[e.RowIndex].Cells["ProductArticleNumber"].Value); //  ID

                    // Создаем и открываем новую форму
                    EditProduct editForm = new EditProduct(productId);
                    editForm.ShowDialog(); // Показываем форму как модальное окно

                    FillDataGrid("SELECT " +
                    "ProductArticleNumber AS 'Артикул'," +
                    "ProductName AS 'Наименование товара'," +
                    "ProductUnit AS 'Единица измерения'," +
                    "ProductCost AS 'Стоимость'," +
                    "Supplier.SupplierName AS 'Поставщик'," +
                    "Category.CategoryName AS 'Категория'," +
                    "ProductDiscountAmount AS 'Скидка'," +
                    "ProductCount AS 'Количество на складе'," +
                    "ProductDescription AS 'Описание'," +
                    "ProductPhoto " +
                "FROM Product " +
                "INNER JOIN Supplier ON Product.ProductSupplier = Supplier.SupplierID " +
                "INNER JOIN Category ON Product.ProductCategory = Category.CategoryID", 1);
                }
                if (Values.UserRole != 1)
                {
                    // Проверяем, что нажата кнопка "Удалить"
                    if (e.ColumnIndex == dataGridView1.Columns["Удалить"].Index && e.RowIndex >= 0)
                    {
                        // Получаем идентификатор записи
                        string id = Convert.ToString(dataGridView1.Rows[e.RowIndex].Cells["ProductArticleNumber"].Value);

                        // Подтверждение удаления
                        DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите удалить эту запись?", "Подтверждение удаления", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            DeleteRecord(id); // Удаляем запись из базы данных
                            dataGridView1.Rows.RemoveAt(e.RowIndex); // Удаляем строку из DataGridView
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Удаляет запись из базы данных по заданному ID.
        /// </summary>
        /// <param name="id">Идентификатор записи для удаления.</param>
        private void DeleteRecord(string id)
        {
            string query = "DELETE FROM Product WHERE ProductArticleNumber = @ProductArticleNumber";
            MySqlConnection con = new MySqlConnection(connectionString2);
            con.Open();
            MySqlCommand command = new MySqlCommand(query, con);
            command.Parameters.AddWithValue("@ProductArticleNumber", id);
            command.ExecuteNonQuery();
            con.Close();
        }
    }
}
