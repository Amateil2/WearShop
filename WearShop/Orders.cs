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
using Microsoft.Office.Interop.Excel;
using Excel = Microsoft.Office.Interop.Excel;

namespace WearShop
{
    /// <summary>
    /// Форма для просмотра и управления заказами.
    /// </summary>
    public partial class Orders : Form
    {
        /// <summary>
        /// Строка подключения к базе данных.
        /// </summary>
        string connectionString2 = $"host={Values.DBHost};uid={Values.DBUser};pwd={Values.DBPassword};database=db30;port=3306;";

        /// <summary>
        /// Конструктор формы "Заказы".
        /// </summary>
        /// <param name="Role">Роль пользователя (например, для определения прав доступа).</param>
        public Orders(int Role)
        {
            InitializeComponent();
        }

        /// <summary>
        /// Вспомогательный класс для хранения данных о заказе.
        /// </summary>
        private class OrderData
        {
            /// <summary>
            /// Идентификатор заказа.
            /// </summary>
            public int OrderID { get; set; }

            /// <summary>
            /// Статус заказа.
            /// </summary>
            public string OrderStatus { get; set; }

            /// <summary>
            /// Словарь для хранения количества товаров в заказе (ключ - ProductArticleNumber).
            /// </summary>
            public Dictionary<string, int> OrderItems { get; set; } = new Dictionary<string, int>();
        }

        /// <summary>
        /// Список для хранения данных о заказах.
        /// </summary>
        private List<OrderData> orderDataList = new List<OrderData>();

        /// <summary>
        /// Заполняет DataGridView данными из базы данных.
        /// </summary>
        /// <param name="strCmd">SQL-запрос для получения данных о заказах.</param>
        public void FillDataGrid(string strCmd)
        {
            MySqlConnection con = new MySqlConnection(connectionString2);
            try
            {
                con.Open();
                MySqlCommand command = new MySqlCommand(strCmd, con);
                MySqlDataReader rdr = command.ExecuteReader();

                // Очистка DataGridView и списка данных
                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();
                orderDataList.Clear();

                dataGridView1.AutoResizeColumns();
                dataGridView1.AutoResizeRows();
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.ReadOnly = false;  // Разрешаем редактирование

                // Создаем столбцы
                dataGridView1.Columns.Add("OrderID", "Номер заказа");
                dataGridView1.Columns.Add("OrderDate", "Дата заказа");
                dataGridView1.Columns.Add("OrderPickUpPoint", "Магазин");
                dataGridView1.Columns.Add("OrderUser", "Ползователь");
                dataGridView1.Columns.Add("OrderPrice", "Цена заказа");

                // Создаем ComboBoxColumn для статуса заказа
                DataGridViewComboBoxColumn statusColumn = new DataGridViewComboBoxColumn();
                statusColumn.Name = "OrderStatus";
                statusColumn.HeaderText = "Статус заказа";
                statusColumn.Items.AddRange(new string[] { "Выполнен", "Отменён" });
                dataGridView1.Columns.Add(statusColumn);

                dataGridView1.Columns["OrderID"].ReadOnly = true;
                dataGridView1.Columns["OrderDate"].ReadOnly = true;
                dataGridView1.Columns["OrderPickUpPoint"].ReadOnly = true;
                dataGridView1.Columns["OrderUser"].ReadOnly = true;
                dataGridView1.Columns["OrderPrice"].ReadOnly = true;


                while (rdr.Read())
                {
                    int orderID = Convert.ToInt32(rdr["Номер заказа"]); // ИЗМЕНЕНО ЗДЕСЬ

                    // Создаем объект OrderData и заполняем его данными
                    OrderData orderData = new OrderData
                    {
                        OrderID = orderID,
                        OrderStatus = rdr["Статус заказа"].ToString()
                    };

                    // Загружаем информацию о товарах в заказе
                    LoadOrderItems(orderID, orderData);

                    // Добавляем данные в DataGridView
                    int rowIndex = dataGridView1.Rows.Add();
                    DataGridViewRow row = dataGridView1.Rows[rowIndex];
                    row.Cells["OrderID"].Value = orderID;
                    row.Cells["OrderDate"].Value = rdr["Дата заказа"];
                    row.Cells["OrderPickUpPoint"].Value = rdr["Магазин"];
                    row.Cells["OrderUser"].Value = rdr["Пользователь"];
                    row.Cells["OrderPrice"].Value = rdr["Цена заказа"];
                    row.Cells["OrderStatus"].Value = rdr["Статус заказа"]; // Заполняем ComboBox

                    // Добавляем OrderData в список
                    orderDataList.Add(orderData);
                }


            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка: {ex}");
            }
            finally // Добавляем блок finally
            {
                if (con != null && con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }

        /// <summary>
        /// Загружает информацию о товарах в заказе из базы данных.
        /// </summary>
        /// <param name="orderID">Идентификатор заказа.</param>
        /// <param name="orderData">Объект OrderData для хранения информации о товарах.</param>
        private void LoadOrderItems(int orderID, OrderData orderData)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString2)) // Создаем новое соединение
                {
                    connection.Open();
                    string query = "SELECT ProductArticleNumber, OrderProductCount FROM orderproduct WHERE OrderID = @OrderID";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@OrderID", orderID);
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string productArticleNumber = reader["ProductArticleNumber"].ToString();
                                int quantity = Convert.ToInt32(reader["OrderProductCount"]);
                                orderData.OrderItems.Add(productArticleNumber, quantity); // Используем ProductArticleNumber
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке товаров для заказа {orderID}: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Обработчик события загрузки формы "Заказы".
        /// Инициализирует ComboBox'ы для сортировки и фильтрации, а также загружает данные о заказах в DataGridView.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void Orders_Load(object sender, EventArgs e)
        {

            //Сортировка
            comboBox1.Items.Add("Без сортировки");
            comboBox1.Items.Add("По убыванию");
            comboBox1.Items.Add("По возрастанию");
            comboBox2.Items.Add("Без фильтрации");
            comboBox2.Items.Add("Статус Выполнен");
            comboBox2.Items.Add("Статус Отменён");

            //Фильтрация
            //comboBox2.Items.Add("Все диапазоны");
            //comboBox2.Items.Add("0-9,99%");
            //comboBox2.Items.Add("10-14,99%");
            //comboBox2.Items.Add("15% и более");
            //comboBox2.SelectedIndexChanged += comboBox2_SelectedIndexChanged;

            FillDataGrid("SELECT " +
            "OrderID AS 'Номер заказа'," +
            "OrderDate AS 'Дата заказа'," +
            "pickuppoint.OrderPickUpPointName AS 'Магазин'," +
            "CONCAT(COALESCE(UserSurname, ''), ' ', COALESCE(UserName, ''), ' ', COALESCE(UserPatronymic, '')) AS 'Пользователь'," +
            "OrderPrice AS 'Цена заказа'," +
            "OrderStatus AS 'Статус заказа' " +
            "FROM `Order` " + //  Order заключено в обратные кавычки
            "INNER JOIN pickuppoint ON `Order`.OrderPickUpPoint = pickuppoint.PickUpPointID " + // Order заключено в обратные кавычки
            "INNER JOIN User ON `Order`.OrderUser = User.UserID"); // Order заключено в обратные кавычки

            dateTimePickerStart.MaxDate = DateTime.Now;
            dateTimePickerEnd.MaxDate = DateTime.Now;

            // Подписываемся на событие CellEndEdit
            dataGridView1.CellEndEdit += dataGridView1_CellEndEdit;
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Назад". Переходит на форму менеджера.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void button2_Click(object sender, EventArgs e)
        {
            Manager mm = new Manager();
            mm.Show();
            this.Hide();
        }

        /// <summary>
        /// Обработчик изменения выбранного элемента в ComboBox для сортировки.
        /// Обновляет DataGridView с учетом выбранной сортировки.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDataGrid();
        }

        /// <summary>
        /// Обновляет DataGridView данными из базы данных с учетом фильтрации и сортировки.
        /// </summary>
        private void UpdateDataGrid()//поиск сортировка фильтр
        {
            string searchStr = textBoxLogin.Text;

            // Получение выбранной категории
            string selectedCategory = comboBox2.SelectedItem?.ToString(); // выбранная категория
            int categoryId = -1;



            string strCmd = "SELECT " +
            "OrderID AS 'Номер заказа'," +
            "OrderDate AS 'Дата заказа'," +
            "pickuppoint.OrderPickUpPointName AS 'Магазин'," +
            "CONCAT(COALESCE(UserSurname, ''), ' ', COALESCE(UserName, ''), ' ', COALESCE(UserPatronymic, '')) AS 'Пользователь'," +
            "OrderPrice AS 'Цена заказа'," +
            "OrderStatus AS 'Статус заказа' " +
            "FROM `Order` " + //  Order заключено в обратные кавычки
            "INNER JOIN pickuppoint ON `Order`.OrderPickUpPoint = pickuppoint.PickUpPointID " + // Order заключено в обратные кавычки
            "INNER JOIN User ON `Order`.OrderUser = User.UserID"; // Order заключено в обратные кавычки
            // Поиск

            if (textBoxLogin.Text == "")
            {
                FillDataGrid(strCmd);
            }
            if (!string.IsNullOrWhiteSpace(searchStr))
            {
                strCmd += $" AND OrderID = '{searchStr}'";
                FillDataGrid(strCmd);
            }



            // фильтрация по категориям
            if (!string.IsNullOrWhiteSpace(selectedCategory))
            {
                if (comboBox2.Text != "")
                {
                    if (comboBox2.Text == "Статус Выполнен")
                    {
                        strCmd += $" AND OrderStatus = 'Выполнен'";
                        FillDataGrid(strCmd);
                    }
                    if (comboBox2.Text == "Статус Отменён")
                    {
                        strCmd += $" AND OrderStatus = 'Отменён'";
                        FillDataGrid(strCmd);
                    }
                }
            }
            //сортировка
            if (comboBox1.Text != "")
            {
                if (comboBox1.Text == "Без сортировки")
                {
                    FillDataGrid(strCmd);
                }
                if (comboBox1.Text == "По убыванию")
                {
                    strCmd += $" ORDER BY OrderPrice DESC";
                    FillDataGrid(strCmd);
                }
                if (comboBox1.Text == "По возрастанию")
                {
                    strCmd += $" ORDER BY OrderPrice ASC";
                    FillDataGrid(strCmd);
                }
            }
            // Сортировка
            //strCmd += $" ORDER BY ProductCost {orderBy}";

            // Заполнение DataGrid

        }

        /// <summary>
        /// Обработчик изменения текста в поле для поиска заказа по ID.
        /// Обновляет DataGridView с учетом введенного значения.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void textBoxLogin_TextChanged(object sender, EventArgs e)
        {
            UpdateDataGrid();
        }

        /// <summary>
        /// Обработчик изменения выбранного элемента в ComboBox для фильтрации по статусу заказа.
        /// Обновляет DataGridView с учетом выбранного статуса.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDataGrid();
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Сформировать отчет".
        /// Генерирует отчет в формате Excel о заказах за выбранный период.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void button1_Click(object sender, EventArgs e)
        {
            DateTime startDate = dateTimePickerStart.Value.Date;
            DateTime endDate = dateTimePickerEnd.Value.Date;

            // Проверка дат
            if (startDate > endDate)
            {
                MessageBox.Show("Дата начала периода не может быть больше даты окончания периода.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Подключение к БД
                using (MySqlConnection connection = new MySqlConnection(connectionString2))
                {
                    connection.Open();

                    // SQL-запрос для получения данных о заказах за указанный период
                    string query = @"SELECT 
                                OrderID AS 'Номер заказа',
                                OrderDate AS 'Дата заказа',
                                pickuppoint.OrderPickUpPointName AS 'Магазин',
                                CONCAT(COALESCE(UserSurname, ''), ' ', COALESCE(UserName, ''), ' ', COALESCE(UserPatronymic, '')) AS 'Пользователь',
                                OrderPrice AS 'Цена заказа',
                                OrderStatus AS 'Статус заказа'
                            FROM `Order`
                            INNER JOIN pickuppoint ON `Order`.OrderPickUpPoint = pickuppoint.PickUpPointID
                            INNER JOIN User ON `Order`.OrderUser = User.UserID
                            WHERE OrderDate BETWEEN @startDate AND @endDate AND OrderStatus = 'Выполнен'";

                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@startDate", startDate);
                    command.Parameters.AddWithValue("@endDate", endDate);

                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    System.Data.DataTable dataTable = new System.Data.DataTable();
                    adapter.Fill(dataTable);

                    // Подсчет общей суммы и других метрик
                    int totalRevenue = dataTable.AsEnumerable().Sum(row => row.Field<int>("Цена заказа"));
                    int totalOrders = dataTable.Rows.Count;
                    double averagePrice = totalOrders > 0 ? totalRevenue / (double)totalOrders : 0;

                    // Создание Excel-приложения и книги
                    Excel.Application excelApp = new Excel.Application();
                    Excel.Workbook workbook = excelApp.Workbooks.Add();
                    Excel.Worksheet worksheet = workbook.Sheets[1];

                    // Заголовки столбцов
                    for (int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        worksheet.Cells[1, i + 1] = dataTable.Columns[i].ColumnName;
                    }

                    // Данные
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        for (int j = 0; j < dataTable.Columns.Count; j++)
                        {
                            worksheet.Cells[i + 2, j + 1] = dataTable.Rows[i][j].ToString();
                        }
                    }

                    // Итоговая информация
                    int lastRow = dataTable.Rows.Count + 2;
                    worksheet.Cells[lastRow, 1] = "Общий доход:";
                    worksheet.Cells[lastRow, 2] = totalRevenue.ToString("F2");
                    worksheet.Cells[lastRow + 1, 1] = "Количество заказов:";
                    worksheet.Cells[lastRow + 1, 2] = totalOrders.ToString();
                    worksheet.Cells[lastRow + 2, 1] = "Средняя цена заказа:";
                    worksheet.Cells[lastRow + 2, 2] = averagePrice.ToString("F2");

                    // Распределение по пользователям
                    var userDistribution = dataTable.AsEnumerable()
                        .GroupBy(row => row.Field<string>("Пользователь"))
                        .Select(g => new { User = g.Key, Count = g.Count(), TotalPrice = g.Sum(row => row.Field<int>("Цена заказа")) });

                    worksheet.Cells[lastRow + 4, 1] = "Распределение по пользователям:";
                    worksheet.Cells[lastRow + 5, 1] = "Пользователь";
                    worksheet.Cells[lastRow + 5, 2] = "Количество заказов";
                    worksheet.Cells[lastRow + 5, 3] = "Сумма";

                    int currentRow = lastRow + 6;
                    foreach (var user in userDistribution)
                    {
                        worksheet.Cells[currentRow, 1] = user.User;
                        worksheet.Cells[currentRow, 2] = user.Count.ToString();
                        worksheet.Cells[currentRow, 3] = user.TotalPrice.ToString("F2");
                        currentRow++;
                    }

                    // Авторазмер столбцов
                    worksheet.Columns.AutoFit();

                    // Сохранение файла
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.FileName = $"Отчет_по_заказам_{startDate.ToString("yyyy-MM-dd")}_{endDate.ToString("yyyy-MM-dd")}.xlsx";
                    saveFileDialog.DefaultExt = "xlsx";
                    saveFileDialog.Filter = "Excel Files|*.xlsx";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        workbook.SaveAs(saveFileDialog.FileName);
                        MessageBox.Show("Отчет успешно сформирован и сохранен.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    // Закрытие Excel
                    workbook.Close();
                    excelApp.Quit();

                    // Очистка COM-объектов
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
                    excelApp = null;
                    workbook = null;
                    worksheet = null;
                    GC.Collect();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при формировании отчета: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Обработчик изменения даты начала периода.
        /// Устанавливает минимальную дату для DateTimePicker с датой окончания периода.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void dateTimePickerStart_ValueChanged(object sender, EventArgs e)
        {
            dateTimePickerEnd.MinDate = dateTimePickerStart.Value;
        }

        /// <summary>
        /// Обработчик изменения даты окончания периода.
        /// Устанавливает максимальную дату для DateTimePicker с датой начала периода.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void dateTimePickerEnd_ValueChanged(object sender, EventArgs e)
        {
            dateTimePickerStart.MaxDate = dateTimePickerEnd.Value;
        }

        /// <summary>
        /// Обработчик события окончания редактирования ячейки в DataGridView.
        /// Обновляет статус заказа в базе данных и изменяет количество товаров на складе.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // Проверяем, что редактировали столбец "OrderStatus"
            if (dataGridView1.Columns[e.ColumnIndex].Name == "OrderStatus")
            {
                int orderID = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["OrderID"].Value);
                string newStatus = dataGridView1.Rows[e.RowIndex].Cells["OrderStatus"].Value.ToString();

                // Получаем старый статус заказа из списка данных
                OrderData orderData = orderDataList.FirstOrDefault(o => o.OrderID == orderID);

                //Если заказ отменен, то запрещаем менять статус
                if (orderData != null && orderData.OrderStatus == "Отменён")
                {
                    MessageBox.Show("Нельзя изменить статус отмененного заказа", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dataGridView1.Rows[e.RowIndex].Cells["OrderStatus"].Value = orderData.OrderStatus;
                    return;
                }

                // Проверяем, что статус изменился
                if (orderData != null && orderData.OrderStatus != newStatus)
                {
                    // Проверяем, достаточно ли товара на складе, если меняем статус с "Отменён" на "Выполнен"
                    if (orderData.OrderStatus == "Отменён" && newStatus == "Выполнен")
                    {
                        if (!CheckStockAvailability(orderID))
                        {
                            MessageBox.Show("Недостаточно товара на складе для выполнения заказа.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            dataGridView1.Rows[e.RowIndex].Cells["OrderStatus"].Value = orderData.OrderStatus; // Возвращаем старый статус
                            return; // Прекращаем выполнение метода
                        }
                    }

                    // Обновляем статус заказа в базе данных
                    UpdateOrderStatus(orderID, newStatus);

                    // Если статус изменился с "Выполнен" на "Отменён", возвращаем товар на склад
                    if (orderData.OrderStatus == "Выполнен" && newStatus == "Отменён")
                    {
                        ReturnProductsToStock(orderID);
                    }
                    // Если статус изменился с "Отменён" на "Выполнен", списываем товар со склада
                    else if (orderData.OrderStatus == "Отменён" && newStatus == "Выполнен")
                    {
                        DeductProductsFromStock(orderID);
                    }

                    // Обновляем статус заказа в списке данных
                    if (orderData != null)
                    {
                        orderData.OrderStatus = newStatus;
                    }
                }
            }
        }

        /// <summary>
        /// Проверяет наличие товара на складе перед выполнением заказа.
        /// </summary>
        /// <param name="orderID">Идентификатор заказа.</param>
        /// <returns>True, если достаточно товара на складе, иначе - False.</returns>
        private bool CheckStockAvailability(int orderID)
        {
            OrderData orderData = orderDataList.FirstOrDefault(o => o.OrderID == orderID);

            if (orderData == null)
            {
                MessageBox.Show($"Не найден заказ с ID {orderID}.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString2))
                {
                    connection.Open();

                    foreach (var item in orderData.OrderItems)
                    {
                        string productArticleNumber = item.Key;
                        int quantity = item.Value;

                        // Проверяем, достаточно ли товара на складе
                        string query = "SELECT ProductCount FROM Product WHERE ProductArticleNumber = @ProductArticleNumber";
                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@ProductArticleNumber", productArticleNumber);

                            object result = command.ExecuteScalar();
                            if (result != null && result != DBNull.Value)
                            {
                                int stockQuantity = Convert.ToInt32(result);
                                if (stockQuantity < quantity)
                                {
                                    return false; // Недостаточно товара на складе
                                }
                            }
                            else
                            {
                                MessageBox.Show($"Товар с артикулом {productArticleNumber} не найден.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return false; // Товар не найден
                            }
                        }
                    }

                    return true; // Достаточно товара на складе для всех позиций заказа
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при проверке наличия товара на складе: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Обновляет статус заказа в базе данных.
        /// </summary>
        /// <param name="orderID">Идентификатор заказа.</param>
        /// <param name="newStatus">Новый статус заказа.</param>
        private void UpdateOrderStatus(int orderID, string newStatus)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString2))
                {
                    connection.Open();
                    string query = "UPDATE `Order` SET OrderStatus = @OrderStatus WHERE OrderID = @OrderID";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@OrderID", orderID);
                        command.Parameters.AddWithValue("@OrderStatus", newStatus);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении статуса заказа: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Возвращает товар на склад при отмене заказа.
        /// </summary>
        /// <param name="orderID">Идентификатор заказа.</param>
        private void ReturnProductsToStock(int orderID)
        {
            try
            {
                // Находим OrderData для данного orderID
                OrderData orderData = orderDataList.FirstOrDefault(o => o.OrderID == orderID);

                if (orderData != null)
                {
                    using (MySqlConnection connection = new MySqlConnection(connectionString2))
                    {
                        connection.Open();
                        // Перебираем товары в заказе и возвращаем их на склад
                        foreach (var item in orderData.OrderItems)
                        {
                            string productArticleNumber = item.Key; // Используем ProductArticleNumber
                            int quantity = item.Value;
                            // Увеличиваем количество товара на складе
                            string query = "UPDATE Product SET ProductCount = ProductCount + @Quantity WHERE ProductArticleNumber = @ProductArticleNumber"; // Используем ProductArticleNumber
                            using (MySqlCommand command = new MySqlCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@ProductArticleNumber", productArticleNumber); // Используем ProductArticleNumber
                                command.Parameters.AddWithValue("@Quantity", quantity);
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при возврате товара на склад: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Списывает товар со склада при выполнении заказа.
        /// </summary>
        /// <param name="orderID">Идентификатор заказа.</param>
        private void DeductProductsFromStock(int orderID)
        {
            try
            {
                // Находим OrderData для данного orderID
                OrderData orderData = orderDataList.FirstOrDefault(o => o.OrderID == orderID);

                if (orderData != null)
                {
                    using (MySqlConnection connection = new MySqlConnection(connectionString2))
                    {
                        connection.Open();
                        // Перебираем товары в заказе и списываем их со склада
                        foreach (var item in orderData.OrderItems)
                        {
                            string productArticleNumber = item.Key; // Используем ProductArticleNumber
                            int quantity = item.Value;
                            // Уменьшаем количество товара на складе
                            string query = "UPDATE Product SET ProductCount = ProductCount - @Quantity WHERE ProductArticleNumber = @ProductArticleNumber"; // Используем ProductArticleNumber
                            using (MySqlCommand command = new MySqlCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@ProductArticleNumber", productArticleNumber); // Используем ProductArticleNumber
                                command.Parameters.AddWithValue("@Quantity", quantity);
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при списании товара со склада: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
