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
    public partial class ImportData : Form
    {
        string connectionString2 = $"host={Values.DBHost};uid={Values.DBUser};pwd={Values.DBPassword};database=db30;port=3306;";
        private bool IsAdmin;
        private string filePath;
        public ImportData(bool IsAdmin)
        {
            InitializeComponent();
            this.IsAdmin = IsAdmin;
            LoadTableNames();
        }
        private void LoadTableNames()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString2))
            {
                try
                {
                    connection.Open();
                    DataTable schemaTable = connection.GetSchema("Tables");
                    foreach (DataRow row in schemaTable.Rows)
                    {
                        string tableName = row[2].ToString();
                        comboBoxTables.Items.Add(tableName);
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"Ошибка подключения: {ex.Message}");
                }
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            RepairAndImport am = new RepairAndImport(IsAdmin);
            am.Show();
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                    filePath = openFileDialog.FileName; 
            }
            string selectedTable = comboBoxTables.SelectedItem?.ToString();
            

            if (string.IsNullOrEmpty(selectedTable))
            {
                MessageBox.Show("Выберите таблицу для импорта.");
                return;
            }
            if (string.IsNullOrEmpty(filePath))
            {
                MessageBox.Show("Выберите CSV файл для импорта.");
                return;
            }

            ImportData2(selectedTable, filePath);
        }
        private void ImportData2(string tableName, string filePath)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString2))
                {
                    connection.Open();
                    int insertedRowsCount = 0;

                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        string line;
                        bool isFirstLine = true; // Флаг для пропуска первой строки

                        while ((line = reader.ReadLine()) != null)
                        {
                            if (isFirstLine)
                            {
                                // Пропустить первую строку
                                isFirstLine = false;
                                continue;
                            }

                            string[] values = line.Split(';');

                            // Получаем количество столбцов в таблице
                            using (MySqlCommand countCmd = new MySqlCommand($"SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}'", connection))
                            {
                                int columnsCount = Convert.ToInt32(countCmd.ExecuteScalar());
                                if (values.Length != columnsCount)
                                {
                                    MessageBox.Show($"Количество столбцов в файле ({values.Length}) не соответствует таблице {tableName} ({columnsCount}).");
                                    return;
                                }
                            }

                            // Создаем запрос для вставки данных
                            string insertCommand = GenerateInsertCommand(tableName, values);
                            
                            using (MySqlCommand cmd = new MySqlCommand(insertCommand, connection))
                            {
                                cmd.ExecuteNonQuery();
                                insertedRowsCount++;
                            }
                        }
                    }

                    MessageBox.Show($"Успешно импортировано {insertedRowsCount} записей в таблицу {tableName}.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка импорта данных: {ex.Message}");
            }
        }
        private string GenerateInsertCommand(string tableName, string[] values)
        {
            // Оборачиваем каждое значение в одинарные кавычки и экранируем одиночные кавычки внутри значений
            for (int i = 0; i < values.Length; i++)
            {
                // Убираем лишние пробелы
                values[i] = values[i].Trim();

                // Экранируем одинарные кавычки
                values[i] = values[i].Replace("'", "''");

                // Оборачиваем значение в одинарные кавычки
                values[i] = $"'{values[i]}'";
            }

            string columns = string.Join(", ", values);
            return $"INSERT INTO {tableName} VALUES ({columns})";
        }
    }
}
