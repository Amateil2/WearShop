using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient; // Для подключения к базе данных MySQL
using System.Configuration; // Для работы с ConfigurationManager (файл App.config)
using System.IO; // Для работы с файловой системой (если вам нужно работать с файлами, включая изображения)
using Microsoft.Office.Interop.Word;

namespace WearShop
{
    /// <summary>
    /// Форма добавления и редактирования заказа.
    /// </summary>
    public partial class AddOrder : Form
    {
        /// <summary>
        /// Строка подключения к базе данных.
        /// </summary>
        string connectionString2 = $"host={Values.DBHost};uid={Values.DBUser};pwd={Values.DBPassword};database=db30;port=3306;";

        /// <summary>
        /// Обновленный словарь заказа (товар - количество).
        /// </summary>
        public Dictionary<string, int> UpdatedOrder { get; private set; }

        /// <summary>
        /// Выбранный пункт выдачи заказа.
        /// </summary>
        public string SelectedPickUpPoint { get; private set; }

        /// <summary>
        ///  Словарь заказа (товар - количество).
        /// </summary>
        private Dictionary<string, int> order;

        /// <summary>
        /// Конструктор формы AddOrder.
        /// </summary>
        /// <param name="order">Словарь, содержащий информацию о заказе (товар - количество).</param>
        /// <param name="pickUpPoint">Выбранный пункт выдачи.</param>
        public AddOrder(Dictionary<string, int> order, string pickUpPoint)
        {
            InitializeComponent();
            this.order = new Dictionary<string, int>(order);
            this.UpdatedOrder = order;
            this.SelectedPickUpPoint = pickUpPoint;
            Values.clearOrder = false;
            LoadDataIntoComboBox();
            InitializeOrderFormUI(); // Инициализация всех UI-элементов
            PopulateOrderDetails(); // Отображение данных заказа
            UpdateConfirmButtonState(); // Первоначальная проверка состояния заказа
        }

        /// <summary>
        /// Инициализация UI-элементов формы заказа, добавление столбцов в DataGridView.
        /// </summary>
        private void InitializeOrderFormUI()
        {

            // Добавляем столбцы таблицы
            dataGridOrder.Columns.Add("ProductName", "Товар");
            dataGridOrder.Columns.Add("Quantity", "Количество");
            dataGridOrder.Columns.Add("UnitPrice", "Цена на ед.");
            dataGridOrder.Columns.Add("Discount", "Скидка (%)");
            dataGridOrder.Columns.Add("Total", "Итоговая стоимость");
            dataGridOrder.Dock = DockStyle.Top;
            dataGridOrder.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridOrder.AutoResizeColumns();
            dataGridOrder.AutoResizeRows();
            dataGridOrder.ReadOnly = true;
            dataGridOrder.AllowUserToAddRows = false;

            DataGridViewButtonColumn buttonDel = new DataGridViewButtonColumn();
            buttonDel.Name = "Удалить";
            buttonDel.HeaderText = "Удалить";
            buttonDel.Text = "Удалить";
            buttonDel.UseColumnTextForButtonValue = true;
            dataGridOrder.Columns.Add(buttonDel);

            // Кнопка увеличения количества
            DataGridViewButtonColumn buttonIncrease = new DataGridViewButtonColumn();
            buttonIncrease.Name = "IncreaseButton";
            buttonIncrease.HeaderText = "Прибавить";
            buttonIncrease.Text = "+";
            buttonIncrease.UseColumnTextForButtonValue = true;
            dataGridOrder.Columns.Add(buttonIncrease);

            // Кнопка уменьшения количества
            DataGridViewButtonColumn buttonDecrease = new DataGridViewButtonColumn();
            buttonDecrease.Name = "DecreaseButton";
            buttonDecrease.HeaderText = "Уменьшить";
            buttonDecrease.Text = "-";
            buttonDecrease.UseColumnTextForButtonValue = true;
            dataGridOrder.Columns.Add(buttonDecrease);

            dataGridOrder.ReadOnly = true;
            dataGridOrder.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridOrder.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridOrder.AutoGenerateColumns = false;
            dataGridOrder.RowHeadersVisible = false;

        }

        /// <summary>
        /// Загрузка данных пунктов выдачи в ComboBox.
        /// </summary>
        private void LoadDataIntoComboBox()
        {
            string query2 = "SELECT OrderPickUpPointName FROM pickuppoint";
            using (MySqlConnection connection = new MySqlConnection(connectionString2))
            {
                connection.Open();
                MySqlCommand command2 = new MySqlCommand(query2, connection);
                using (MySqlDataReader reader2 = command2.ExecuteReader())
                {
                    while (reader2.Read())
                    {
                        comboBoxPickUpPoint.Items.Add(reader2["OrderPickUpPointName"].ToString());
                    }
                }

            }

        }

        /// <summary>
        /// Заполнение DataGridView данными о заказе из словаря order.
        /// </summary>
        private void PopulateOrderDetails()
        {
            // Получаем ссылку на DataGridView и очищаем его
            DataGridView dataGridOrder = Controls["dataGridOrder"] as DataGridView;
            if (dataGridOrder == null) return;

            dataGridOrder.Rows.Clear();
            decimal totalOrderPrice = 0;

            // Заполняем таблицу данными о заказе
            foreach (var product in order)
            {
                string productName = GetProductNameFromDB(product.Key);
                decimal productCost = GetProductCostFromDB(product.Key);
                decimal productDiscount = GetProductDiscountFromDB(product.Key);
                decimal discountAmount = productCost * productDiscount / 100;
                decimal finalPrice = (productCost - discountAmount) * product.Value;

                totalOrderPrice += finalPrice;

                dataGridOrder.Rows.Add(productName, product.Value, productCost, productDiscount, finalPrice);
            }

            // Обновляем итоговую сумму в лейбле
            Label lblTotal = Controls["lblTotal"] as Label;
            if (lblTotal != null)
            {
                lblTotal.Text = $"Итоговая сумма: {totalOrderPrice:F2} руб.";
            }

            UpdateConfirmButtonState(); // Обновляем состояние кнопки "Оформить заказ"
        }

        /// <summary>
        /// Получение названия товара из базы данных по его артикулу.
        /// </summary>
        /// <param name="productId">Артикул товара.</param>
        /// <returns>Название товара.</returns>
        private string GetProductNameFromDB(string productId)
        {
            string productName = string.Empty;

            using (MySqlConnection connection = new MySqlConnection(connectionString2))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT ProductName FROM product WHERE ProductArticleNumber = @ProductArticleNumber";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@ProductArticleNumber", productId);

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        productName = result.ToString();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при получении названия товара: {ex.Message}");
                }
            }

            return productName;
        }

        /// <summary>
        /// Получение стоимости товара из базы данных по его артикулу.
        /// </summary>
        /// <param name="productId">Артикул товара.</param>
        /// <returns>Стоимость товара.</returns>
        private decimal GetProductCostFromDB(string productId)
        {
            decimal productCost = 0;

            using (MySqlConnection connection = new MySqlConnection(connectionString2))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT ProductCost FROM product WHERE ProductArticleNumber = @ProductArticleNumber";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@ProductArticleNumber", productId);

                    object result = cmd.ExecuteScalar();
                    if (result != null && decimal.TryParse(result.ToString(), out decimal cost))
                    {
                        productCost = cost;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при получении стоимости товара: {ex.Message}");
                }
            }

            return productCost;
        }

        /// <summary>
        /// Получение скидки на товар из базы данных по его артикулу.
        /// </summary>
        /// <param name="productId">Артикул товара.</param>
        /// <returns>Скидка на товар (в процентах).</returns>
        private decimal GetProductDiscountFromDB(string productId)
        {
            decimal productDiscount = 0;

            using (MySqlConnection connection = new MySqlConnection(connectionString2))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT ProductDiscountAmount FROM product WHERE ProductArticleNumber = @ProductArticleNumber";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@ProductArticleNumber", productId);

                    object result = cmd.ExecuteScalar();
                    if (result != null && decimal.TryParse(result.ToString(), out decimal discount))
                    {
                        productDiscount = discount;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при получении скидки на товар: {ex.Message}");
                }
            }

            return productDiscount;
        }

        /// <summary>
        /// Получение количества товара на складе по его артикулу.
        /// </summary>
        /// <param name="productId">Артикул товара.</param>
        /// <returns>Количество товара на складе.</returns>
        private int GetProductQuantityInStock(string productId)
        {
            int quantityInStock = 0;

            using (MySqlConnection connection = new MySqlConnection(connectionString2))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT ProductCount FROM product WHERE ProductArticleNumber = @ProductArticleNumber";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@ProductArticleNumber", productId);

                    object result = cmd.ExecuteScalar();
                    if (result != null && int.TryParse(result.ToString(), out int quantity))
                    {
                        quantityInStock = quantity;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при получении количества товара на складе: {ex.Message}");
                }
            }

            return quantityInStock;
        }

        /// <summary>
        /// Генерация документа заказа в формате Word.
        /// </summary>
        /// <param name="orderId">ID заказа.</param>
        private void GenerateOrderDocument(int orderId)
        {
            // Генерация талона в Word
            var wordApp = new Microsoft.Office.Interop.Word.Application();
            var doc = wordApp.Documents.Add();

            // Установка междустрочного интервала 1.5
            var paragraphFormat = doc.Content.ParagraphFormat;
            paragraphFormat.LineSpacingRule = Microsoft.Office.Interop.Word.WdLineSpacing.wdLineSpace1pt5;

            paragraphFormat.SpaceBefore = 0;  // Установить интервал перед абзацем в 0
            paragraphFormat.SpaceAfter = 0;   // Установить интервал после абзаца в 0

            // Заголовок документа
            doc.Content.Text += $"=========================================";
            doc.Content.Text += $"                ЧЕК";
            doc.Content.Text += $"=========================================";
            doc.Content.Text += $"Дата заказа: {DateTime.Now.ToShortDateString()}";
            doc.Content.Text += $"Номер заказа: {orderId}";
            doc.Content.Text += $"Адресс: {comboBoxPickUpPoint.Text}";
            doc.Content.Text += $"-----------------------------------------";
            doc.Content.Text += $"Состав заказа:\n";

            decimal total = 0;
            decimal totalDiscount = 0;
            foreach (var product in UpdatedOrder)
            {
                string productName = GetProductNameFromDB(product.Key);
                decimal productCost = GetProductCostFromDB(product.Key);
                decimal discount = GetProductDiscountFromDB(product.Key);
                decimal subtotal = product.Value * productCost * (1 - discount / 100);

                var discountAmount = product.Value * productCost * discount / 100;
                doc.Content.Text += $"{productName}: {product.Value} шт. x {productCost} руб. " +
                                    $"(-{discount}% = {discountAmount} руб.) -> {subtotal} руб.";

                total += subtotal;
                totalDiscount += discountAmount;
            }

            doc.Content.Text += $"-----------------------------------------";
            doc.Content.Text += $"Итоговая сумма: {total} руб.";
            doc.Content.Text += $"Сумма скидки: {totalDiscount} руб.";
            doc.Content.Text += $"=========================================";
            doc.Content.Text += $"Спасибо за покупку!";
            doc.Content.Text += $"=========================================";

            // Сохранение документа
            doc.SaveAs($"OrderTicket_{orderId}.docx");
            doc.Close();
            wordApp.Quit();
        }

        /// <summary>
        /// Обработчик события нажатия на кнопку "Оформить заказ".
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие.</param>
        /// <param name="e">Аргументы события.</param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(comboBoxPickUpPoint.Text))
            {
                MessageBox.Show("Выберите пункт выдачи.");
                return;
            }

            if (order.Count == 0)
            {
                MessageBox.Show("Ваш заказ пуст. Нельзя оформить заказ без выбранных товаров.");
                return;
            }

            try
            {
                int orderId = SaveOrderToDB(); // Сохраняем заказ в базе данных и получаем его ID

                if (orderId > 0)
                {
                    GenerateOrderDocument(orderId); // Генерируем талон
                    MessageBox.Show($"Заказ успешно оформлен! Чек сохранён как OrderTicket_{orderId}.docx.");
                    ClearOrder(); // Метод очистки заказа
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении заказа: {ex.Message}");
            }
        }

        /// <summary>
        /// Очистка заказа (словаря order, ComboBox и DataGridView).
        /// </summary>
        private void ClearOrder()
        {
            order.Clear(); // Очищаем внутреннюю коллекцию заказа
            UpdatedOrder.Clear(); // Очищаем обновленный заказ
            comboBoxPickUpPoint.Text = null; // Сбрасываем выбранный пункт выдачи
            PopulateOrderDetails(); // Обновляем интерфейс, чтобы отобразить изменения
            UpdateConfirmButtonState(); // Обновляем состояние кнопки "Оформить заказ"
        }

        /// <summary>
        /// Получение ID пункта выдачи по его названию.
        /// </summary>
        /// <param name="pickUpPointName">Название пункта выдачи.</param>
        /// <returns>ID пункта выдачи.</returns>
        private int GetPickUpPointIdByName(string pickUpPointName)
        {
            int pickUpPointId = 0;

            using (MySqlConnection connection = new MySqlConnection(connectionString2))
            {
                connection.Open();
                string query = "SELECT PickUpPointID FROM pickuppoint WHERE OrderPickUpPointName = @PickUpPointName";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@PickUpPointName", pickUpPointName);

                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    pickUpPointId = Convert.ToInt32(result);
                }
                else
                {
                    throw new Exception($"Пункт выдачи '{pickUpPointName}' не найден.");
                }
            }

            return pickUpPointId;
        }

        /// <summary>
        /// Получение ID текущего пользователя.
        /// </summary>
        /// <returns>ID текущего пользователя.</returns>
        private int GetCurrentUserId()
        {
            if (Values.UserID == 0)
            {
                throw new Exception("Пользователь не авторизован.");
            }

            return Values.UserID; // Возвращаем ID текущего пользователя
        }

        /// <summary>
        /// Сохранение заказа в базу данных.
        /// </summary>
        /// <returns>ID созданного заказа.</returns>
        private int SaveOrderToDB()
        {
            int orderId = 0; // ID созданного заказа

            using (MySqlConnection connection = new MySqlConnection(connectionString2))
            {
                connection.Open();
                MySqlTransaction transaction = connection.BeginTransaction(); // Начинаем транзакцию

                try
                {
                    // 1. Получение ID пункта выдачи из таблицы pickuppoint
                    int pickUpPointId = GetPickUpPointIdByName(comboBoxPickUpPoint.Text);

                    // 2. Установка текущего пользователя, связанного с заказом (здесь замените "currentUserId" на реальное значение)
                    int currentUserId = GetCurrentUserId(); // Метод для получения текущего пользователя из сессии или контекста

                    // 3. Рассчитываем итоговые суммы заказа
                    decimal totalAmount = 0;
                    decimal totalDiscount = 0;

                    foreach (var product in order)
                    {
                        decimal unitPrice = GetProductCostFromDB(product.Key);
                        decimal discount = GetProductDiscountFromDB(product.Key);
                        decimal discountedPrice = unitPrice - (unitPrice * discount / 100);

                        totalAmount += discountedPrice * product.Value;
                        totalDiscount += unitPrice * product.Value - discountedPrice * product.Value;
                    }

                    // 4. Вставка данных о заказе в таблицу `order`
                    string insertOrderQuery = @"
                INSERT INTO `order` (OrderDate, OrderPickUpPoint, OrderUser, OrderPrice, OrderStatus) 
                VALUES (@OrderDate, @OrderPickUpPoint, @OrderUser, @OrderPrice, @OrderStatus);
                SELECT LAST_INSERT_ID();";

                    MySqlCommand cmd = new MySqlCommand(insertOrderQuery, connection, transaction);

                    // Параметры для вставки
                    cmd.Parameters.AddWithValue("@OrderDate", DateTime.Now.Date); // Текущая дата
                    cmd.Parameters.AddWithValue("@OrderPickUpPoint", pickUpPointId); // ID пункта выдачи
                    cmd.Parameters.AddWithValue("@OrderUser", currentUserId); // ID текущего пользователя
                    cmd.Parameters.AddWithValue("@OrderPrice", totalAmount);
                    cmd.Parameters.AddWithValue("@OrderStatus", "Отменён"); // Статус заказа (по умолчанию "Принят")

                    // Выполняем запрос и получаем ID нового заказа
                    orderId = Convert.ToInt32(cmd.ExecuteScalar());

                    // 5. Сохранение товаров в заказе (таблица orderproduct)
                    string insertProductQuery = @"
                INSERT INTO `orderproduct` (OrderID, ProductArticleNumber, OrderProductCount)
                VALUES (@OrderID, @ProductArticleNumber, @OrderProductCount);";

                    foreach (var product in order)
                    {
                        MySqlCommand productCmd = new MySqlCommand(insertProductQuery, connection, transaction);

                        // Параметры для добавления товаров в заказ
                        productCmd.Parameters.AddWithValue("@OrderID", orderId); // ID созданного заказа
                        productCmd.Parameters.AddWithValue("@ProductArticleNumber", product.Key); // Артикул товара
                        productCmd.Parameters.AddWithValue("@OrderProductCount", product.Value); // Количество товара

                        // Выполняем запрос
                        productCmd.ExecuteNonQuery();

                        //Уменьшаем кол-во товаров на складе
                        string updateStockQuery = "UPDATE product SET ProductCount = ProductCount - @OrderProductCount WHERE ProductArticleNumber = @ProductArticleNumber";
                        MySqlCommand updateStockCmd = new MySqlCommand(updateStockQuery, connection, transaction);
                        updateStockCmd.Parameters.AddWithValue("@OrderProductCount", product.Value);
                        updateStockCmd.Parameters.AddWithValue("@ProductArticleNumber", product.Key);
                        updateStockCmd.ExecuteNonQuery();

                    }

                    transaction.Commit(); // Завершаем транзакцию
                }
                catch (Exception ex)
                {
                    transaction.Rollback(); // В случае ошибки откатываем изменения
                    throw new Exception($"Ошибка при сохранении заказа: {ex.Message}");
                }
            }

            return orderId; // Возвращаем ID созданного заказа
        }

        /// <summary>
        /// Обработчик события окончания редактирования ячейки DataGridView.
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие.</param>
        /// <param name="e">Аргументы события.</param>
        private void dataGridOrder_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dataGridOrder = sender as DataGridView;

            // При изменении количества пересчитываем значение в таблице
            if (e.ColumnIndex == dataGridOrder.Columns["Quantity"].Index && e.RowIndex >= 0)
            {
                string productName = dataGridOrder.Rows[e.RowIndex].Cells["ProductName"].Value.ToString();
                string productId = GetProductIdByName(productName);
                if (!int.TryParse(dataGridOrder.Rows[e.RowIndex].Cells["Quantity"].Value?.ToString(), out int newQuantity))
                {
                    MessageBox.Show("Введите целое число!");
                    PopulateOrderDetails(); // Перезаполняем таблицу
                    return;
                }

                int maxQuantity = GetProductQuantityInStock(productId);

                if (newQuantity > 0 && newQuantity <= maxQuantity && order.ContainsKey(productId))
                {
                    order[productId] = newQuantity;
                    UpdatedOrder[productId] = newQuantity;
                }
                else
                {
                    if (newQuantity <= 0)
                    {
                        MessageBox.Show("Количество должно быть больше нуля!");
                        dataGridOrder.Rows[e.RowIndex].Cells["Quantity"].Value = order[productId]; // Возвращаем старое значение
                    }
                    else
                    {
                        MessageBox.Show($"Количество не может превышать {maxQuantity}!");
                        dataGridOrder.Rows[e.RowIndex].Cells["Quantity"].Value = order[productId]; // Возвращаем старое значение
                    }

                }

                PopulateOrderDetails(); // Перезаполняем таблицу
            }
        }

        /// <summary>
        /// Получение артикула товара по его названию.
        /// </summary>
        /// <param name="productName">Название товара.</param>
        /// <returns>Артикул товара.</returns>
        private string GetProductIdByName(string productName)
        {
            string productId = "";

            using (MySqlConnection connection = new MySqlConnection(connectionString2))
            {
                connection.Open();
                string query = "SELECT ProductArticleNumber FROM product WHERE ProductName = @ProductName";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@ProductName", productName);

                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    productId = result.ToString();
                }
            }

            return productId;
        }

        /// <summary>
        /// Обработчик события нажатия на ячейку DataGridView.
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие.</param>
        /// <param name="e">Аргументы события.</param>
        private void dataGridOrder_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dataGridOrder = sender as DataGridView;

            if (e.RowIndex < 0)
                return; // Предотвращаем обработку кликов по заголовкам столбцов.

            string productName = dataGridOrder.Rows[e.RowIndex].Cells["ProductName"].Value.ToString();
            string productId = GetProductIdByName(productName);
            int quantityInStock = GetProductQuantityInStock(productId); // Получаем количество на складе

            // Кнопка "Удалить"
            if (e.ColumnIndex == dataGridOrder.Columns["Удалить"].Index)
            {
                // Подтверждение удаления
                DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите удалить эту запись?", "Подтверждение удаления", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    if (order.ContainsKey(productId))
                    {
                        order.Remove(productId);
                        UpdatedOrder.Remove(productId);
                    }
                    PopulateOrderDetails(); // Перезаполняем таблицу
                }
            }
            // Кнопка "+"
            else if (e.ColumnIndex == dataGridOrder.Columns["IncreaseButton"].Index)
            {
                if (order.ContainsKey(productId))
                {
                    if (order[productId] < quantityInStock)
                    {
                        order[productId]++;
                        UpdatedOrder[productId]++;
                        PopulateOrderDetails();
                    }
                    else
                    {
                        MessageBox.Show("На складе недостаточно товара.");
                    }

                }
            }
            // Кнопка "-"
            else if (e.ColumnIndex == dataGridOrder.Columns["DecreaseButton"].Index)
            {
                if (order.ContainsKey(productId))
                {
                    if (order[productId] > 1) // Чтобы не уйти в ноль.
                    {
                        order[productId]--;
                        UpdatedOrder[productId]--;
                    }
                    else
                    {
                        //Если количество товара 1, то при нажатии на - товар удаляется
                        DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите удалить эту запись?", "Подтверждение удаления", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            if (order.ContainsKey(productId))
                            {
                                order.Remove(productId);
                                UpdatedOrder.Remove(productId);
                            }
                        }
                    }
                    PopulateOrderDetails();
                }
            }
        }

        /// <summary>
        /// Обработчик события нажатия на кнопку "Отмена".
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие.</param>
        /// <param name="e">Аргументы события.</param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridOrder.Rows.Count == 0 || (dataGridOrder.Rows.Count == 1 && dataGridOrder.Rows[0].IsNewRow))
            {
                order.Clear();
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// Обработчик события загрузки формы AddOrder.
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие.</param>
        /// <param name="e">Аргументы события.</param>
        private void AddOrder_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Обработчик события нажатия на кнопку "Очистить заказ".
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие.</param>
        /// <param name="e">Аргументы события.</param>
        private void button3_Click(object sender, EventArgs e)
        {
            Values.clearOrder = true;
            order.Clear();
            PopulateOrderDetails();
        }

        /// <summary>
        /// Обновление состояния кнопки "Оформить заказ" в зависимости от наличия товаров в заказе.
        /// </summary>
        private void UpdateConfirmButtonState()
        {
            button1.Enabled = order.Count > 0; // Кнопка активна, только если в заказе есть товары.
        }
    }
}
