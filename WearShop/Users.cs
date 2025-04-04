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
    /// Форма для управления пользователями системы.
    /// </summary>
    public partial class Users : Form
    {
        /// <summary>
        /// Строка подключения к базе данных.
        /// </summary>
        string connectionString2 = $"host={Values.DBHost};uid={Values.DBUser};pwd={Values.DBPassword};database=db30;port=3306;";

        /// <summary>
        /// Список для хранения всех строк DataGridView (не используется в текущей реализации).
        /// </summary>
        private List<DataGridViewRow> allRows1 = new List<DataGridViewRow>();

        /// <summary>
        /// Конструктор формы управления пользователями.
        /// </summary>
        public Users()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Назад". Возвращает к форме администратора.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void button2_Click(object sender, EventArgs e)
        {
            Administrator am = new Administrator();
            am.Show();
            this.Hide();
        }

        /// <summary>
        /// Заполняет DataGridView данными о пользователях из базы данных (с кнопками редактирования и удаления).
        /// </summary>
        /// <param name="strCmd">SQL-запрос для получения данных о пользователях.</param>
        public void FillDataGrid(string strCmd)
        {
            try
            {
                MySqlConnection con = new MySqlConnection(connectionString2);
                con.Open();
                MySqlCommand command = new MySqlCommand(strCmd, con);
                MySqlDataReader rdr = command.ExecuteReader();
                for (int i = 0; i < dataGridView1.Rows.Count; ++i)
                {
                    dataGridView1.Rows[i].Visible = true;
                }
                //allRows1.Clear();
                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();
                dataGridView1.AutoResizeColumns();
                dataGridView1.AutoResizeRows();
                dataGridView1.ReadOnly = true;
                dataGridView1.AllowUserToAddRows = false;

                dataGridView1.Columns.Add("UserID", "ID пользователя");
                dataGridView1.Columns.Add("UserSurname", "Фамилия");
                dataGridView1.Columns.Add("UserName", "Имя");
                dataGridView1.Columns.Add("UserPatronymic", "Отчество");
                dataGridView1.Columns.Add("UserRole", "Роль");
                dataGridView1.Columns["UserID"].Visible = false;
                DataGridViewButtonColumn buttonEdit = new DataGridViewButtonColumn();
                buttonEdit.Name = "Редактировать";
                buttonEdit.HeaderText = "Редактировать";
                buttonEdit.Text = "Редактировать";
                buttonEdit.UseColumnTextForButtonValue = true;
                dataGridView1.Columns.Add(buttonEdit);

                DataGridViewButtonColumn buttonDel = new DataGridViewButtonColumn();
                buttonDel.Name = "Удалить";
                buttonDel.HeaderText = "Удалить";
                buttonDel.Text = "Удалить";
                buttonDel.UseColumnTextForButtonValue = true;
                dataGridView1.Columns.Add(buttonDel);

                while (rdr.Read())
                {
                    int rowIndex = dataGridView1.Rows.Add();
                    DataGridViewRow row = dataGridView1.Rows[rowIndex];
                    row.Cells["UserID"].Value = rdr[0];

                    // Используем функцию MaskPersonalData для скрытия данных
                    row.Cells["UserSurname"].Value = MaskPersonalData(rdr[1].ToString());
                    row.Cells["UserName"].Value = MaskPersonalData(rdr[2].ToString());
                    row.Cells["UserPatronymic"].Value = MaskPersonalData(rdr[3].ToString());

                    row.Cells["UserRole"].Value = rdr[4];
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
        /// Функция для скрытия персональных данных (замена половины символов на звёздочки).
        /// </summary>
        /// <param name="data">Строка с персональными данными.</param>
        /// <returns>Скрытая строка с заменёнными символами.</returns>
        private string MaskPersonalData(string data)
        {
            if (string.IsNullOrEmpty(data))
                return data;

            int lengthToMask = data.Length / 2; // Половина длины строки
            string visiblePart = data.Substring(0, data.Length - lengthToMask); // Видимая часть
            string maskedPart = new string('*', lengthToMask); // Невидимая часть
            return visiblePart + maskedPart;
        }

        /// <summary>
        /// Обработчик события загрузки формы "Пользователи".
        /// Заполняет DataGridView данными о пользователях.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void Users_Load(object sender, EventArgs e)
        {
            FillDataGrid("SELECT " +
            "UserID AS 'ID пользователя'," +
            "UserSurname AS 'Фамилия'," +
            "UserName AS 'Имя'," +
            "UserPatronymic AS 'Отчество'," +
            "Role.RoleName AS 'Роль'" +
            "FROM User " +
            "INNER JOIN Role ON User.UserRole = Role.RoleID");
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Добавить пользователя".
        /// Открывает форму для добавления нового пользователя.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void button4_Click(object sender, EventArgs e)
        {
            AddUser am = new AddUser();
            am.Show();
            this.Hide();
        }

        /// <summary>
        /// Обработчик клика по ячейке DataGridView. Выполняет действия в зависимости от нажатой кнопки ("Редактировать" или "Удалить").
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView1.Columns["Редактировать"].Index && e.RowIndex >= 0)
            {
                int id = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["UserID"].Value);
                EditUsers editForm = new EditUsers(id);
                editForm.ShowDialog();
                FillDataGrid("SELECT " +
                    "UserID AS 'ID пользователя'," +
                    "UserSurname AS 'Фамилия'," +
                    "UserName AS 'Имя'," +
                    "UserPatronymic AS 'Отчество'," +
                    "Role.RoleName AS 'Роль'" +
                    "FROM User " +
                    "INNER JOIN Role ON User.UserRole = Role.RoleID");
            }

            if (e.ColumnIndex == dataGridView1.Columns["Удалить"].Index && e.RowIndex >= 0)
            {
                int id = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["UserID"].Value);

                DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите удалить эту запись?", "Подтверждение удаления", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    DeleteRecord(id);
                    dataGridView1.Rows.RemoveAt(e.RowIndex);
                }
            }
        }

        /// <summary>
        /// Удаляет запись о пользователе из базы данных.
        /// </summary>
        /// <param name="id">Идентификатор пользователя.</param>
        private void DeleteRecord(int id)
        {
            string query = "DELETE FROM User WHERE UserID = @UserID";
            MySqlConnection con = new MySqlConnection(connectionString2);
            con.Open();
            MySqlCommand command = new MySqlCommand(query, con);
            command.Parameters.AddWithValue("@UserID", id);
            command.ExecuteNonQuery();
            con.Close();
        }
    }
}
