using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WearShop
{
    /// <summary>
    /// Форма администратора.
    /// </summary>
    public partial class Administrator : Form
    {
        /// <summary>
        /// Конструктор формы администратора.
        /// </summary>
        public Administrator()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Выход".  Переходит к форме авторизации.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void button2_Click(object sender, EventArgs e)
        {
            Authorization authorization = new Authorization();
            this.Close();
            authorization.Show();
        }

        /// <summary>
        /// Обработчик события загрузки формы.  Отображает ФИО пользователя.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void Administrator_Load(object sender, EventArgs e)
        {
            Login.Text = Values.UserFIO;
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Поставщики". Переходит к форме "Поставщики".
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void button4_Click(object sender, EventArgs e)
        {
            Reference Suppliers = new Reference(); // Исправить название переменной.  Должна ссылаться на форму поставщиков, если это предполагается.
            this.Close();
            Suppliers.Show();
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Пользователи". Переходит к форме "Пользователи".
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void button1_Click(object sender, EventArgs e)
        {
            Users users = new Users();
            this.Close();
            users.Show();
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Товары". Переходит к форме "Просмотр товаров" в режиме администратора.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void button3_Click(object sender, EventArgs e)
        {
            ViewProducts viewProducts = new ViewProducts(3); //Режим 3, вероятно, администраторский
            this.Close();
            viewProducts.Show();
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Справочники". Переходит к форме "Справочники".  Внимание:  Этот метод дублирует button4_Click.  Следует проверить логику и, возможно, удалить один из них.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void button4_Click_1(object sender, EventArgs e)
        {
            Reference Reference = new Reference();
            this.Close();
            Reference.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            RepairBase RepairBase = new RepairBase(true);
            this.Close();
            RepairBase.Show();
        }
    }
}
