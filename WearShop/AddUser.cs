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
    /// Форма добавления пользователя.
    /// </summary>
    public partial class AddUser : Form
    {
        /// <summary>
        /// Строка подключения к базе данных.
        /// </summary>
        string connectionString2 = $"host={Values.DBHost};uid={Values.DBUser};pwd={Values.DBPassword};database=db30;port=3306;";

        /// <summary>
        /// Конструктор формы AddUser.
        /// </summary>
        public AddUser()
        {
            InitializeComponent();
            LoadDataIntoComboBox();
        }

        /// <summary>
        /// Обработчик события нажатия кнопки "Назад".
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие.</param>
        /// <param name="e">Аргументы события.</param>
        private void button2_Click(object sender, EventArgs e)
        {
            Users users = new Users();
            this.Close();
            users.Show();
        }

        /// <summary>
        /// Обработчик события нажатия кнопки "Добавить".
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие.</param>
        /// <param name="e">Аргументы события.</param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBoxSurname.Text == "" || textBoxName.Text == "" || textBoxPatronymic.Text == "" || textBoxLogin.Text == "" || textBoxPwd.Text == "" || comboBox1.Text == "")
            {
                MessageBox.Show("Необходимо заполнить все поля!");
            }
            else
            {
                UpdateProductData();
                MessageBox.Show("Пользователь успешно добавлен");
                Users am = new Users();
                am.Show();
                this.Hide();
            }
        }

        /// <summary>
        /// Обновление данных о пользователе в базе данных.
        /// </summary>
        private void UpdateProductData()
        {
            using (MySqlConnection con = new MySqlConnection(connectionString2))
            {
                con.Open();

                ///

                // 4. Вставка данных о заказе в таблицу `order`
                string insertOrderQuery = @"
                INSERT INTO user (UserSurname, UserName, UserPatronymic, UserLogin, UserPassword, UserRole) 
                VALUES (@UserSurname, @UserName, @UserPatronymic, @UserLogin, @UserPassword, @UserRole);";

                MySqlCommand cmd = new MySqlCommand(insertOrderQuery, con);

                // Параметры для вставки
                cmd.Parameters.AddWithValue("@UserSurname", textBoxSurname.Text); // Фамилия пользователя
                cmd.Parameters.AddWithValue("@UserName", textBoxName.Text); // Имя пользователя
                cmd.Parameters.AddWithValue("@UserPatronymic", textBoxPatronymic.Text); // Отчество пользователя
                cmd.Parameters.AddWithValue("@UserLogin", textBoxLogin.Text); // Логин пользователя
                cmd.Parameters.AddWithValue("@UserPassword", textBoxPwd.Text); // Пароль пользователя
                string selectedRoleName = comboBox1.Text.ToString();
                int roleId = GetRoleIdByName(selectedRoleName, con);
                cmd.Parameters.AddWithValue("@UserRole", roleId); // Роль пользователя
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Получение ID роли по имени роли.
        /// </summary>
        /// <param name="RoleName">Имя роли.</param>
        /// <param name="con">Подключение к базе данных.</param>
        /// <returns>ID роли.</returns>
        private int GetRoleIdByName(string RoleName, MySqlConnection con)
        {
            int supplierId = -1; // Инициализация, если поставщик не найден
            string query = "SELECT RoleID FROM role WHERE RoleName = @RoleName";

            using (MySqlCommand command = new MySqlCommand(query, con))
            {
                command.Parameters.AddWithValue("@RoleName", RoleName);

                object result = command.ExecuteScalar(); // Получаем первый столбец результата
                if (result != null)
                {
                    supplierId = Convert.ToInt32(result);
                }
            }

            return supplierId; // Возвращаем ID поставщика
        }

        /// <summary>
        /// Загрузка данных о ролях в ComboBox.
        /// </summary>
        private void LoadDataIntoComboBox()
        {
            string query2 = "SELECT RoleName FROM role";
            using (MySqlConnection connection = new MySqlConnection(connectionString2))
            {
                connection.Open();
                MySqlCommand command2 = new MySqlCommand(query2, connection);
                using (MySqlDataReader reader2 = command2.ExecuteReader())
                {
                    while (reader2.Read())
                    {
                        comboBox1.Items.Add(reader2["RoleName"].ToString());
                    }
                }
            }

        }

        /// <summary>
        /// Обработчик события изменения текста в поле ввода фамилии.
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие.</param>
        /// <param name="e">Аргументы события.</param>
        private void textBoxSurname_TextChanged(object sender, EventArgs e)
        {
            ValidateNameField(textBoxSurname);
        }

        /// <summary>
        /// Обработчик события изменения текста в поле ввода имени.
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие.</param>
        /// <param name="e">Аргументы события.</param>
        private void textBoxName_TextChanged(object sender, EventArgs e)
        {
            ValidateNameField(textBoxName);
        }

        /// <summary>
        /// Обработчик события изменения текста в поле ввода отчества.
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие.</param>
        /// <param name="e">Аргументы события.</param>
        private void textBoxPatronymic_TextChanged(object sender, EventArgs e)
        {
            ValidateNameField(textBoxPatronymic);
        }

        /// <summary>
        /// Общая функция для валидации полей ФИО.
        /// </summary>
        /// <param name="textBox">TextBox, который нужно проверить.</param>
        private void ValidateNameField(TextBox textBox)
        {
            string text = textBox.Text;
            if (text.Length > 25)
            {
                textBox.Text = text.Substring(0, 25);
                textBox.SelectionStart = 25;
            }

            if (!string.IsNullOrEmpty(text))
            {
                // Удаляем все символы, кроме букв
                string filteredText = new string(text.Where(char.IsLetter).ToArray());

                // Если отфильтрованный текст отличается от исходного, обновляем поле
                if (filteredText.Length != text.Length)
                {
                    textBox.Text = filteredText;
                    textBox.SelectionStart = filteredText.Length; // Возвращаем курсор в конец
                }

                // Делаем первую букву заглавной, а остальные строчными
                if (filteredText.Length > 0)
                {
                    textBox.Text = char.ToUpper(filteredText[0]) + filteredText.Substring(1).ToLower();
                    textBox.SelectionStart = textBox.Text.Length; // Возвращаем курсор в конец
                }
            }


        }

        /// <summary>
        /// Обработчик события изменения текста в поле ввода логина.
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие.</param>
        /// <param name="e">Аргументы события.</param>
        private void textBoxLogin_TextChanged(object sender, EventArgs e)
        {
            string text = textBoxLogin.Text;
            if (text.Length > 20)
            {
                textBoxLogin.Text = text.Substring(0, 20);
                textBoxLogin.SelectionStart = 20;
            }

            // Разрешаем только буквы и цифры
            string filteredText = new string(text.Where(c => char.IsLetterOrDigit(c)).ToArray());
            if (filteredText.Length != text.Length)
            {
                textBoxLogin.Text = filteredText;
                textBoxLogin.SelectionStart = filteredText.Length;
            }
        }

        /// <summary>
        /// Обработчик события изменения текста в поле ввода пароля.
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие.</param>
        /// <param name="e">Аргументы события.</param>
        private void textBoxPwd_TextChanged(object sender, EventArgs e)
        {
            string text = textBoxPwd.Text;
            if (text.Length > 20)
            {
                textBoxPwd.Text = text.Substring(0, 20);
                textBoxPwd.SelectionStart = 20;
            }
        }

        private void textBoxSurname_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (textBoxSurname.Text.Length >= 20 && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }
        }

        private void textBoxName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (textBoxName.Text.Length >= 20 && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }
        }

        private void textBoxPatronymic_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (textBoxPatronymic.Text.Length >= 20 && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }
        }

        private void textBoxLogin_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (textBoxLogin.Text.Length >= 20 && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }
        }

        private void textBoxPwd_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (textBoxPwd.Text.Length >= 20 && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }
        }
    }
}
