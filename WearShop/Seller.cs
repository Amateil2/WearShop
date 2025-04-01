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
    /// Форма для интерфейса продавца.
    /// </summary>
    public partial class Seller : Form
    {
        /// <summary>
        /// Конструктор формы продавца.
        /// </summary>
        public Seller()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Обработчик события нажатия на метку (не используется).
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void label1_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Обработчик события нажатия кнопки "Просмотр товаров".
        /// Открывает форму просмотра товаров.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void button1_Click(object sender, EventArgs e)
        {
            ViewProducts viewProducts = new ViewProducts(1);
            this.Close();
            viewProducts.Show();
        }

        /// <summary>
        /// Обработчик события нажатия кнопки "Выход".
        /// Открывает форму авторизации.
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
        /// Обработчик события загрузки формы продавца.
        /// Заполняет метку с именем пользователя.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void Seller_Load(object sender, EventArgs e)
        {
            Login.Text = Values.UserFIO;
        }
    }
}
