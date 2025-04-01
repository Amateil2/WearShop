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
    /// Форма менеджера.
    /// </summary>
    public partial class Manager : Form
    {
        /// <summary>
        /// Конструктор формы менеджера.
        /// </summary>
        public Manager()
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
        private void Manager_Load(object sender, EventArgs e)
        {
            Login.Text = Values.UserFIO;
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Заказы". Переходит к форме "Заказы" в режиме менеджера.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void button4_Click(object sender, EventArgs e)
        {
            Orders orders = new Orders(2); //Режим 2, вероятно, менеджерский
            this.Close();
            orders.Show();
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Товары". Переходит к форме "Просмотр товаров" в режиме менеджера.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void button3_Click(object sender, EventArgs e)
        {
            ViewProducts viewProducts = new ViewProducts(2); //Режим 2, вероятно, менеджерский
            this.Close();
            viewProducts.Show();
        }
    }
}
