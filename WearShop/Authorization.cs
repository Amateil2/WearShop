using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Security.Cryptography;
using System.Timers;
using System.IO;

namespace WearShop
{
    /// <summary>
    /// Форма авторизации пользователя.
    /// </summary>
    public partial class Authorization : Form
    {
        /// <summary>
        /// Строка подключения к базе данных.
        /// </summary>
        string connectionString = $"host={Values.DBHost};uid={Values.DBUser};pwd={Values.DBPassword};database=db30;port=3306;";
        /// <summary>
        /// Флаг, определяющий, виден ли пароль.
        /// </summary>
        private bool isPasswordVisible = false;
        private string generatedCaptcha;
        private Random random;
        private int failedAttempts;
        private bool isLocked = false;
        private bool isLocked2 = false;
        private bool captcha;
        private System.Timers.Timer lockTimer;
        /// <summary>
        /// Конструктор формы авторизации.
        /// </summary>
        public Authorization()
        {
            InitializeComponent();
            button1.Enabled = false;
            // Подписка на изменения текстовых полей

            textBoxPwd.UseSystemPasswordChar = true;
            textBoxLogin.TextChanged += Field_TextChanged;
            textBoxPwd.TextChanged += Field_TextChanged;
            random = new Random();
            failedAttempts = 0;
            // Настройка таймера для разблокировки
            lockTimer = new System.Timers.Timer(10000);
            lockTimer.Elapsed += OnLockTimerElapsed;
            lockTimer.AutoReset = false; // задаем один раз
            GenerateCaptcha();
            GenerateCaptchaImage();
        }

        /// <summary>
        /// Обработчик события изменения текста в полях логина и пароля.
        /// Активирует кнопку авторизации, если оба поля заполнены.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void Field_TextChanged(object sender, EventArgs e)
        {
            if (!isLocked) // Если система не заблокирована
            {
                // Проверяем, что оба текстовых поля заполнены
                bool isUsernameFilled = !string.IsNullOrWhiteSpace(textBoxLogin.Text);
                bool isPasswordFilled = !string.IsNullOrWhiteSpace(textBoxPwd.Text);
                button1.Enabled = isUsernameFilled && isPasswordFilled;
            }
        }

        /// <summary>
        /// Выполняет авторизацию пользователя в базе данных.
        /// </summary>
        /// <param name="login">Логин пользователя.</param>
        /// <param name="password">Хэшированный пароль пользователя.</param>
        /// <returns>True, если авторизация успешна, иначе False.</returns>
        private bool Authorize(string login, string password)
        {
            try
            {
                MySqlConnection conn = new MySqlConnection(connectionString);
                conn.Open();
                MySqlCommand cmd = new MySqlCommand($"SELECT * FROM user WHERE UserLogin='{login}' and UserPassword='{password}'", conn);
                MySqlDataAdapter ad = new MySqlDataAdapter(cmd);
                DataTable tb = new DataTable();

                ad.Fill(tb);

                if (tb.Rows.Count != 1)
                {
                    return false;
                }

                Values.UserRole = Convert.ToInt32(tb.Rows[0].ItemArray.GetValue(6).ToString());
                Values.UserFIO = tb.Rows[0].ItemArray.GetValue(1).ToString() + " " +
                    tb.Rows[0].ItemArray.GetValue(2).ToString() + " " +
                    tb.Rows[0].ItemArray.GetValue(3).ToString();
                Values.UserID = Convert.ToInt32(tb.Rows[0].ItemArray.GetValue(0).ToString());

                conn.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Переключает пользователя на соответствующую форму в зависимости от его роли.
        /// </summary>
        public void switchRole()
        {
            switch (Values.UserRole)
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
                    MessageBox.Show("Роль пользователя не распознана.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxLogin.Text = "";
                    textBoxPwd.Text = "";
                    return;
            }
        }
        private void ShowCaptcha()
        {
            // Показываем CAPTCHA
            textBoxCaptcha.Visible = true;
            picCaptcha.Visible = true;
            labelCaptcha.Visible = true;
            button4.Visible = true;
        }
        /// <summary>
        /// Обработчик нажатия кнопки "Войти".
        /// Получает хеш пароля, выполняет авторизацию и переключает пользователя на соответствующую форму.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void button1_Click(object sender, EventArgs e)
        {
            string hashPassword = GetHashPass(textBoxPwd.Text);

            if (failedAttempts < 1) // Если попытки меньше 1, проверяем логин/пароль
            {
                if (ConfigurationManager.AppSettings["Admin_login"] == textBoxLogin.Text &&
                    ConfigurationManager.AppSettings["Admin_pwd"] == textBoxPwd.Text)
                {
                    RepairAndImport RepairBase = new RepairAndImport(false);
                    RepairBase.Show();
                    this.Hide();
                }
                else if (Authorize(textBoxLogin.Text, hashPassword))
                {
                    switchRole();
                }
                else
                {
                    failedAttempts++;
                    MessageBox.Show("Логин или пароль неправильный. Попробуйте еще раз.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxLogin.Text = "";
                    textBoxPwd.Text = "";

                    // Показываем CAPTCHA после первой ошибки
                    if (failedAttempts == 1)
                    {
                        ShowCaptcha();
                    }
                }
            }
            else // Если была хотя бы одна неудачная попытка
            {
                // Проверка CAPTCHA
                if (textBoxCaptcha.Text.Equals(generatedCaptcha, StringComparison.OrdinalIgnoreCase))
                {
                    // Если CAPTCHA верна, проверяем логин/пароль
                    if (ConfigurationManager.AppSettings["Admin_login"] == textBoxLogin.Text &&
                        ConfigurationManager.AppSettings["Admin_pwd"] == textBoxPwd.Text)
                    {
                        ResetState();
                        RepairAndImport RepairBase = new RepairAndImport(false);
                        RepairBase.Show();
                        this.Hide();
                    }
                    else if (Authorize(textBoxLogin.Text, hashPassword))
                    {
                        ResetState();
                        switchRole();
                    }
                    else
                    {
                        LockSystem("Логин или пароль неправильный. Попробуйте еще раз.");
                    }
                }
                else
                {
                    LockSystem("Неверная CAPTCHA. Попробуйте снова.");
                }
            }
        }
        private void LockSystem(string message)
        {
            MessageBox.Show(message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            MessageBox.Show("Система будет заблокирована на 10 секунд", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Information);

            button1.Enabled = false;
            isLocked = true; // Устанавливаем флаг блокировки
            lockTimer.Start();

            textBoxLogin.Clear();
            textBoxPwd.Clear();
            textBoxCaptcha.Clear();
            GenerateCaptcha(); // Генерируем новую CAPTCHA для следующей попытки
            GenerateCaptchaImage();
        }
        private void ResetState()
        {
            // Сбрасываем все состояния блокировки
            failedAttempts = 0;
            isLocked = false;
            lockTimer.Stop();

            textBoxCaptcha.Clear();
            HideCaptcha();
        }
        private void OnLockTimerElapsed(object sender, ElapsedEventArgs e)
        {
            // Сбрасываем блокировку после 10 секунд
            isLocked = false;

            // Используем Invoke, чтобы работать с элементами управления из другого потока
            Invoke((MethodInvoker)delegate
            {
                UpdateButtonState();
                MessageBox.Show("Система разблокирована. Вы можете попытаться войти снова.", "Уведомление", MessageBoxButtons.OK, MessageBoxIcon.Information);
            });
        }
        private void UpdateButtonState()
        {
            // Активируем кнопку, если поля логина и пароля заполнены
            bool isUsernameFilled = !string.IsNullOrWhiteSpace(textBoxLogin.Text);
            bool isPasswordFilled = !string.IsNullOrWhiteSpace(textBoxPwd.Text);
            button1.Enabled = isUsernameFilled && isPasswordFilled;
        }
        private void HideCaptcha()
        {
            // Скрываем CAPTCHA
            textBoxCaptcha.Visible = false;
            picCaptcha.Visible = false;
            labelCaptcha.Visible = false;
            button4.Visible = false;
        }
        private void GenerateCaptcha()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            generatedCaptcha = new string(Enumerable.Repeat(chars, 4)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private void GenerateCaptchaImage()
        {
            Bitmap bmp = new Bitmap(120, 40);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.White);
            Font font = new Font("Arial", 20, FontStyle.Bold | FontStyle.Italic);
            for (int i = 0; i < generatedCaptcha.Length; i++)
            {
                g.DrawString(generatedCaptcha[i].ToString(), font, Brushes.Black, 10 + 25 * i, random.Next(5, 15));
            }

            // Добавляем графический шум
            for (int i = 0; i < 30; i++)
            {
                int x = random.Next(0, 120);
                int y = random.Next(0, 40);
                bmp.SetPixel(x, y, Color.Red);
            }

            picCaptcha.Image = bmp;
        }
        /// <summary>
        /// Генерирует хеш пароля с использованием алгоритма SHA256.
        /// </summary>
        /// <param name="password">Пароль для хеширования.</param>
        /// <returns>Хешированный пароль в виде строки.</returns>
        string GetHashPass(string password)
        {
            //получаем байтовое представление строки
            //byte[] bytesPass = Encoding.Unicode.GetBytes(password);
            byte[] bytesPass = Encoding.UTF8.GetBytes(password);

            //экземпляр класса для работы с алгоритмом SHA256
            SHA256Managed hashstring = new SHA256Managed();

            //получаем хеш из строки в виде массива байтов
            byte[] hash = hashstring.ComputeHash(bytesPass);

            //очищаем строку
            string hashPasswd = string.Empty;

            //собираем полученный хеш воедино
            foreach (byte x in hash)
            {
                hashPasswd += String.Format("{0:x2}", x);
            }

            //освобождаем все ресурсы связанные с экземпляром объекта SHA256Managed
            hashstring.Dispose();

            return hashPasswd;
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Выход".
        /// Подтверждает выход из приложения и закрывает его.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void button2_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Вы действительно хотите выйти?", "Подтверждение выхода", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки показа/скрытия пароля.
        /// Переключает видимость пароля и изменяет текст кнопки.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void button3_Click(object sender, EventArgs e)
        {
            isPasswordVisible = !isPasswordVisible;
            textBoxPwd.UseSystemPasswordChar = !isPasswordVisible;
            button3.Text = isPasswordVisible ? "*" : "*";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBoxCaptcha.Text = "";
            GenerateCaptcha(); // Генерируем новую CAPTCHA
            GenerateCaptchaImage();
        }
    }
}
