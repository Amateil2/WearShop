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
using System.IO;

namespace WearShop
{
    /// <summary>
    /// Форма редактирования информации о продукте.
    /// </summary>
    public partial class EditProduct : Form
    {
        /// <summary>
        /// Строка подключения к базе данных.
        /// </summary>
        string connectionString2 = $"host={Values.DBHost};uid={Values.DBUser};pwd={Values.DBPassword};database=db30;port=3306;";
        /// <summary>
        /// Артикул продукта, который необходимо отредактировать.
        /// </summary>
        private string productId;
        /// <summary>
        /// Путь к фотографии продукта.
        /// </summary>
        private string _photoPath;

        /// <summary>
        /// Конструктор формы редактирования продукта.
        /// </summary>
        /// <param name="productId">Артикул продукта для редактирования.</param>
        public EditProduct(string productId)
        {
            InitializeComponent();
            this.productId = productId;
            LoadDataIntoComboBox();
            LoadProductData();
        }

        /// <summary>
        /// Загружает данные о продукте из базы данных и отображает их в соответствующих элементах управления формы.
        /// </summary>
        private void LoadProductData()
        {
            using (MySqlConnection con = new MySqlConnection(connectionString2))
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT " +
            "ProductName," +
            "ProductUnit," +
            "ProductCost," +
            "Supplier.SupplierName," +
            "Category.CategoryName," +
            "ProductDiscountAmount," +
            "ProductCount," +
            "ProductDescription," +
            "ProductPhoto " +
            "FROM Product " +
            "INNER JOIN Supplier ON ProductSupplier = SupplierID " +
            "INNER JOIN Category ON ProductCategory = CategoryID " +
            "WHERE ProductArticleNumber = @ProductArticleNumber", con);
                ; // начальная часть запроса
                cmd.Parameters.AddWithValue("ProductArticleNumber", productId);
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {

                        textBoxName.Text = rdr[0].ToString();
                        textBoxUnit.Text = rdr[1].ToString();
                        textBoxCost.Text = rdr[2].ToString();
                        comboBoxSupplier.SelectedItem = rdr[3].ToString();
                        comboBoxCategory.SelectedItem = rdr[4].ToString();
                        textBoxDiscount.Text = rdr[5].ToString();
                        textBoxCount.Text = rdr[6].ToString();
                        textBoxDescription.Text = rdr[7].ToString();
                        _photoPath = rdr[8].ToString();
                        string folderPath = @"./photo/";
                        if (string.IsNullOrEmpty(_photoPath))
                        {
                            pictureBox1.ImageLocation = Path.Combine(folderPath, Path.GetFileName("zaglushka.jpg"));
                        }
                        else
                        {
                            pictureBox1.ImageLocation = Path.Combine(folderPath, Path.GetFileName(rdr[8].ToString()));

                        }

                    }
                }
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Отмена". Закрывает форму редактирования.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Сохранить".
        /// Проверяет заполненность полей, запрашивает подтверждение изменения и обновляет данные продукта в базе данных.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBoxName.Text == "" || textBoxUnit.Text == "" || textBoxCost.Text == "" || comboBoxSupplier.Text == "" || comboBoxCategory.Text == "" || textBoxDiscount.Text == "" || textBoxCount.Text == "" || textBoxDescription.Text == "")
            {
                MessageBox.Show("Необходимо заполнить все поля!");
            }
            else
            {
                var result = MessageBox.Show("Вы действительно хотите изменить запись?", "Подтверждение редактирования", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    UpdateProductData();
                    MessageBox.Show("Данные успешно изменены!");
                }
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        /// <summary>
        /// Обновляет данные продукта в базе данных.
        /// </summary>
        private void UpdateProductData()
        {
            using (MySqlConnection con = new MySqlConnection(connectionString2))
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand(@"UPDATE Product 
        SET ProductName = @ProductName,
            ProductUnit = @ProductUnit,
            ProductCost = @ProductCost,
            ProductSupplier = @ProductSupplier,
            ProductCategory = @ProductCategory,
            ProductDiscountAmount = @ProductDiscountAmount,
            ProductCount = @ProductCount,
            ProductDescription = @ProductDescription,
            ProductPhoto = @ProductPhoto
        WHERE ProductArticleNumber = @ProductArticleNumber", con);


                cmd.Parameters.AddWithValue("@ProductArticleNumber", productId);
                cmd.Parameters.AddWithValue("@ProductName", textBoxName.Text);
                cmd.Parameters.AddWithValue("@ProductUnit", textBoxUnit.Text);
                cmd.Parameters.AddWithValue("@ProductCost", textBoxCost.Text);
                // Получаем ID поставщика на основе выбранного имени
                string selectedSupplierName = comboBoxSupplier.Text.ToString();
                int supplierId = GetSupplierIdByName(selectedSupplierName, con);  // Метод для получения ID поставщика
                cmd.Parameters.AddWithValue("@ProductSupplier", supplierId);
                // Категория
                string selectedCategoryName = comboBoxCategory.Text.ToString();
                int categoryId = GetCategoryIdByName(selectedCategoryName, con);
                cmd.Parameters.AddWithValue("@ProductCategory", categoryId);

                cmd.Parameters.AddWithValue("@ProductDiscountAmount", textBoxDiscount.Text);
                cmd.Parameters.AddWithValue("@ProductCount", textBoxCount.Text);
                cmd.Parameters.AddWithValue("@ProductDescription", textBoxDescription.Text);
                cmd.Parameters.AddWithValue("@ProductPhoto", _photoPath);


                // Выполнение обновления
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Получает ID категории по ее имени из базы данных.
        /// </summary>
        /// <param name="categoryName">Имя категории.</param>
        /// <param name="con">Подключение к базе данных.</param>
        /// <returns>ID категории или -1, если категория не найдена.</returns>
        private int GetCategoryIdByName(string categoryName, MySqlConnection con)
        {
            int categoryId = -1; // Инициализация, если поставщик не найден
            string query = "SELECT CategoryID FROM category WHERE CategoryName = @CategoryName";

            using (MySqlCommand command = new MySqlCommand(query, con))
            {
                command.Parameters.AddWithValue("@CategoryName", categoryName);

                object result = command.ExecuteScalar(); // Получаем первый столбец результата
                if (result != null)
                {
                    categoryId = Convert.ToInt32(result);
                }
            }

            return categoryId; // Возвращаем ID поставщика
        }

        /// <summary>
        /// Получает ID поставщика по его имени из базы данных.
        /// </summary>
        /// <param name="supplierName">Имя поставщика.</param>
        /// <param name="con">Подключение к базе данных.</param>
        /// <returns>ID поставщика или -1, если поставщик не найден.</returns>
        private int GetSupplierIdByName(string supplierName, MySqlConnection con)
        {
            int supplierId = -1; // Инициализация, если поставщик не найден
            string query = "SELECT SupplierID FROM supplier WHERE SupplierName = @SupplierName";

            using (MySqlCommand command = new MySqlCommand(query, con))
            {
                command.Parameters.AddWithValue("@SupplierName", supplierName);

                object result = command.ExecuteScalar(); // Получаем первый столбец результата
                if (result != null)
                {
                    supplierId = Convert.ToInt32(result);
                }
            }

            return supplierId; // Возвращаем ID поставщика
        }

        /// <summary>
        /// Загружает данные о поставщиках и категориях из базы данных и заполняет ими ComboBox'ы на форме.
        /// </summary>
        private void LoadDataIntoComboBox()
        {
            string query2 = "SELECT SupplierName FROM supplier";
            string query3 = "SELECT CategoryName FROM category";
            using (MySqlConnection connection = new MySqlConnection(connectionString2))
            {
                connection.Open();
                MySqlCommand command2 = new MySqlCommand(query2, connection);
                using (MySqlDataReader reader2 = command2.ExecuteReader())
                {
                    while (reader2.Read())
                    {
                        comboBoxSupplier.Items.Add(reader2["SupplierName"].ToString());
                    }
                }

                MySqlCommand command3 = new MySqlCommand(query3, connection);
                using (MySqlDataReader reader3 = command3.ExecuteReader())
                {
                    while (reader3.Read())
                    {
                        comboBoxCategory.Items.Add(reader3["CategoryName"].ToString());
                    }
                }
            }

        }

        /// <summary>
        /// Обработчик нажатия кнопки "Изменить фотографию".
        /// Открывает диалоговое окно выбора файла, позволяет выбрать изображение, проверяет его расширение и размер,
        /// копирует файл в папку photo и отображает его в PictureBox.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png";
            openFileDialog.Title = "Выберите фотографию";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedFilePath = openFileDialog.FileName;
                FileInfo fileInfo = new FileInfo(selectedFilePath);

                if (fileInfo.Extension.ToLower() != ".jpg" && fileInfo.Extension.ToLower() != ".png")
                {
                    MessageBox.Show("Ошибка: Выберите файл с расширением .jpg или .png.", "Ошибка выбора файла", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (fileInfo.Length > 2 * 1024 * 1024)
                {
                    MessageBox.Show("Ошибка: Размер файла должен быть не более 2 Мб.", "Ошибка размера файла", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string folderPath = @"./photo/";

                string newFileName = $"{productId}_{DateTime.Now:yyyyMMdd_HHmmss}";
                string newFileNameWithExtension = $"{newFileName}{fileInfo.Extension}";
                string newFilePath = Path.Combine(folderPath, newFileNameWithExtension);

                File.Copy(selectedFilePath, newFilePath);

                // Для отображения изображения
                pictureBox1.Image = Image.FromFile(newFilePath);
                _photoPath = Path.GetFileName(newFilePath);
            }
        }
        /// <summary>
        /// Обработчик события нажатия клавиши в поле ввода названия товара.
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие.</param>
        /// <param name="e">Аргументы события.</param>
        private void textBoxName_KeyPress(object sender, KeyPressEventArgs e)
        {
            char symbol = e.KeyChar;

            // Разрешаем буквы, цифры, пробелы и знаки препинания (например, ., -)
            if (!(Char.IsLetterOrDigit(symbol) || Char.IsWhiteSpace(symbol) || symbol == '.' || symbol == ',' || symbol == '-' || symbol == 8) || textBoxName.Text.Length >= 100 && symbol != 8)
            {
                e.Handled = true;
            }
        }

        private void textBoxCost_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Проверяем, является ли введенный символ цифрой или буквой
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Отменяем ввод, если символ не допустим
            }
            // Проверяем длину строки
            if (textBoxCost.Text.Length >= 6 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Отменяем ввод, если длина превышает 100000 символов
            }
        }
        /// <summary>
        /// Обработчик события нажатия клавиши в поле ввода количества товара.
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие.</param>
        /// <param name="e">Аргументы события.</param>
        private void textBoxCount_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Проверяем, является ли введенный символ цифрой или буквой
            if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Отменяем ввод, если символ не допустим
            }
            // Проверяем длину строки
            if (textBoxCount.Text.Length >= 4 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Отменяем ввод, если длина превышает 1000 символов
            }
        }

        private void textBoxDiscount_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Проверяем, является ли введенный символ цифрой или буквой
            if (!char.IsDigit(e.KeyChar) || textBoxDiscount.Text.Length >= 2)
            {
                e.Handled = true; // Отменяем ввод, если символ не допустим
            }
        }

        private void textBoxDescription_KeyPress(object sender, KeyPressEventArgs e)
        {
            char symbol = e.KeyChar;

            // Разрешаем буквы, цифры, пробелы и знаки препинания
            if (!(Char.IsLetterOrDigit(symbol) || Char.IsWhiteSpace(symbol) || symbol == '.' || symbol == ',' || symbol == '-' || symbol == 8) || textBoxDescription.Text.Length >= 100 && symbol != 8)
            {
                e.Handled = true;
            }
        }

        private void textBoxUnit_KeyPress(object sender, KeyPressEventArgs e)
        {
            char symbol = e.KeyChar;

            // Разрешаем буквы, цифры, пробелы и знаки препинания
            if (!(Char.IsLetterOrDigit(symbol) || Char.IsWhiteSpace(symbol) || symbol == '.' || symbol == ',' || symbol == '-' || symbol == 8) || textBoxUnit.Text.Length >= 50 && symbol != 8)
            {
                e.Handled = true;
            }
        }

        private void EditProduct_Load(object sender, EventArgs e)
        {

        }
    }
}
