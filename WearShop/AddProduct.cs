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
using System.IO;


namespace WearShop
{
    /// <summary>
    /// Форма добавления товара.
    /// </summary>
    public partial class AddProduct : Form
    {
        /// <summary>
        /// Строка подключения к базе данных.
        /// </summary>
        string connectionString2 = $"host={Values.DBHost};uid={Values.DBUser};pwd={Values.DBPassword};database=db30;port=3306;";

        /// <summary>
        /// Путь к фотографии товара.
        /// </summary>
        private string _photoPath;

        /// <summary>
        /// Роль пользователя.
        /// </summary>
        private int role_user;

        /// <summary>
        /// Конструктор формы AddProduct.
        /// </summary>
        /// <param name="Role">Роль пользователя.</param>
        public AddProduct(int Role)
        {
            InitializeComponent();
            role_user = Role;
            LoadDataIntoComboBox();
        }

        /// <summary>
        /// Загрузка данных в ComboBox (поставщики и категории).
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
        /// Обработчик события нажатия кнопки "Назад".
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие.</param>
        /// <param name="e">Аргументы события.</param>
        private void button2_Click(object sender, EventArgs e)
        {
            ViewProducts viewProducts = new ViewProducts(role_user);
            this.Close();
            viewProducts.Show();
        }


        /// <summary>
        /// Обработчик события нажатия клавиши в поле ввода артикула товара.
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие.</param>
        /// <param name="e">Аргументы события.</param>
        private void textBoxArticle_KeyPress(object sender, KeyPressEventArgs e)
        {
            char symbol = e.KeyChar;

            // Разрешаем только буквы и цифры и контролируем длину
            if (!(Char.IsLetterOrDigit(symbol) || symbol == 8) || textBoxArticle.Text.Length >= 6 && symbol != 8)
            {
                e.Handled = true;
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

        /// <summary>
        /// Обработчик события нажатия клавиши в поле ввода цены товара.
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие.</param>
        /// <param name="e">Аргументы события.</param>
        private void textBox7_KeyPress(object sender, KeyPressEventArgs e)
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
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
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

        /// <summary>
        /// Обработчик события нажатия клавиши в поле ввода скидки на товар.
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие.</param>
        /// <param name="e">Аргументы события.</param>
        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Проверяем, является ли введенный символ цифрой или буквой
            if (!char.IsDigit(e.KeyChar) || textBoxDiscount.Text.Length >= 2)
            {
                e.Handled = true; // Отменяем ввод, если символ не допустим
            }
        }

        /// <summary>
        /// Обработчик события нажатия клавиши в поле ввода описания товара.
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие.</param>
        /// <param name="e">Аргументы события.</param>
        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            char symbol = e.KeyChar;

            // Разрешаем буквы, цифры, пробелы и знаки препинания
            if (!(Char.IsLetterOrDigit(symbol) || Char.IsWhiteSpace(symbol) || symbol == '.' || symbol == ',' || symbol == '-' || symbol == 8) || textBoxDescription.Text.Length >= 100 && symbol != 8)
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Обработчик события нажатия клавиши в поле ввода единицы измерения товара.
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие.</param>
        /// <param name="e">Аргументы события.</param>
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            char symbol = e.KeyChar;

            // Разрешаем буквы, цифры, пробелы и знаки препинания
            if (!(Char.IsLetterOrDigit(symbol) || Char.IsWhiteSpace(symbol) || symbol == '.' || symbol == ',' || symbol == '-' || symbol == 8) || textBoxUnit.Text.Length >= 50 && symbol != 8)
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Обработчик события нажатия кнопки "Добавить".
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие.</param>
        /// <param name="e">Аргументы события.</param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBoxName.Text == "" || textBoxUnit.Text == "" || textBoxCost.Text == "" || comboBoxSupplier.Text == "" || comboBoxCategory.Text == "" || textBoxDiscount.Text == "" || textBoxCount.Text == "" || textBoxDescription.Text == "")
            {
                MessageBox.Show("Необходимо заполнить все поля!");
            }
            else
            {
                var result = MessageBox.Show("Вы действительно хотите добавить новую запись?", "Подтверждение добавленя", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    using (MySqlConnection connection = new MySqlConnection(connectionString2))
                    {
                        connection.Open();
                        string query = "INSERT INTO product (ProductArticleNumber, ProductName, ProductUnit, ProductCost, ProductSupplier, ProductCategory, ProductDiscountAmount, ProductCount, ProductDescription, ProductPhoto) " +
                                       "VALUES (@ArticleNumber, @Name, @Unit, @Cost, @Supplier, @Category, @Discount, @Count, @Description, @Photo)";

                        MySqlCommand command = new MySqlCommand(query, connection);
                        command.Parameters.AddWithValue("@ArticleNumber", textBoxArticle.Text);
                        command.Parameters.AddWithValue("@Name", textBoxName.Text);
                        command.Parameters.AddWithValue("@Unit", textBoxUnit.Text);
                        command.Parameters.AddWithValue("@Cost", int.Parse(textBoxCost.Text));
                        // Получаем ID поставщика на основе выбранного имени
                        string selectedSupplierName = comboBoxSupplier.Text.ToString();
                        int supplierId = GetSupplierIdByName(selectedSupplierName, connection);  // Метод для получения ID поставщика
                        command.Parameters.AddWithValue("@Supplier", supplierId);
                        // Категория
                        string selectedCategoryName = comboBoxCategory.Text.ToString();
                        int categoryId = GetCategoryIdByName(selectedCategoryName, connection);
                        command.Parameters.AddWithValue("@Category", categoryId);
                        if (!string.IsNullOrEmpty(textBoxDiscount.Text))
                        {
                            command.Parameters.AddWithValue("@Discount", byte.Parse(textBoxDiscount.Text));
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@Discount", DBNull.Value);
                        }

                        command.Parameters.AddWithValue("@Count", int.Parse(textBoxCount.Text));
                        command.Parameters.AddWithValue("@Description", textBoxDescription.Text);

                        if (!string.IsNullOrEmpty(_photoPath))
                        {
                            command.Parameters.AddWithValue("@Photo", Path.GetFileName(_photoPath)); // сохраняем только имя файла
                                                                                                     // При необходимости можно также скопировать файл по этому имени в указанную папку на сервере
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@Photo", DBNull.Value);
                        }

                        command.ExecuteNonQuery();
                        MessageBox.Show("Товар добавлен успешно.");

                    }
                }
                ViewProducts viewProducts = new ViewProducts(role_user);
                this.Close();
                viewProducts.Show();
            }

        }

        /// <summary>
        /// Получение ID категории по имени.
        /// </summary>
        /// <param name="categoryName">Имя категории.</param>
        /// <param name="con">Подключение к базе данных.</param>
        /// <returns>ID категории.</returns>
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
        /// Получение ID поставщика по имени.
        /// </summary>
        /// <param name="supplierName">Имя поставщика.</param>
        /// <param name="con">Подключение к базе данных.</param>
        /// <returns>ID поставщика.</returns>
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
        /// Обработчик события нажатия кнопки "Фото".
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие.</param>
        /// <param name="e">Аргументы события.</param>
        private void buttonPhoto_Click(object sender, EventArgs e)
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

                string newFileName = $"{textBoxArticle.Text}_{DateTime.Now:yyyyMMdd_HHmmss}";
                string newFileNameWithExtension = $"{newFileName}{fileInfo.Extension}";
                string newFilePath = Path.Combine(folderPath, newFileNameWithExtension);

                File.Copy(selectedFilePath, newFilePath);

                // Для отображения изображения
                pictureBox1.Image = Image.FromFile(newFilePath);
                _photoPath = Path.GetFileName(newFilePath);
            }
        }

        /// <summary>
        /// Обработчик события изменения текста в поле ввода артикула товара.
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие.</param>
        /// <param name="e">Аргументы события.</param>
        private void textBoxArticle_TextChanged(object sender, EventArgs e)
        {
            // Проверяем, что оба поля заполнены
            bool isIdFilled = !string.IsNullOrWhiteSpace(textBoxArticle.Text);

            // Активируем или деактивируем кнопку в зависимости от состояния полей
            buttonPhoto.Enabled = isIdFilled;
        }
    }
}
