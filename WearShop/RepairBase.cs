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

namespace WearShop
{
    public partial class RepairBase : Form
    {
        string connectionString2 = $"host={Values.DBHost};uid={Values.DBUser};pwd={Values.DBPassword};port=3306;";
        private bool IsAdmin;
        public RepairBase(bool IsAdmin)
        {
            InitializeComponent();
            this.IsAdmin = IsAdmin;
        }

        private void button2_Click(object sender, EventArgs e)
        {            
            RepairAndImport am = new RepairAndImport(IsAdmin);
            am.Show();
            this.Hide();            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            RestoreDatabase();
        }
        private void RestoreDatabase()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString2))
            {
                try
                {
                    connection.Open();
                    MySqlCommand cmd = connection.CreateCommand();

                    // Удаляем базу данных, если она существует
                    cmd.CommandText = "DROP DATABASE IF EXISTS `db30`";
                    cmd.ExecuteNonQuery();

                    // Создаём новую базу данных
                    cmd.CommandText = "CREATE DATABASE `db30`";
                    cmd.ExecuteNonQuery();

                    // Используем новую базу данных
                    cmd.CommandText = "USE `db30`";
                    cmd.ExecuteNonQuery();

                    // Создаем таблицы
                    cmd.CommandText = @"
                    CREATE TABLE `category` (
                        `CategoryID` int NOT NULL AUTO_INCREMENT,
                        `CategoryName` varchar(100) NOT NULL,
                        PRIMARY KEY (`CategoryID`)
                    );
                    CREATE TABLE `pickuppoint` (
                        `PickUpPointID` int NOT NULL AUTO_INCREMENT,
                        `OrderPickUpPointName` varchar(100) NOT NULL,
                        PRIMARY KEY (`PickUpPointID`)
                    );
                    CREATE TABLE `role` (
                        `RoleID` int NOT NULL AUTO_INCREMENT,
                        `RoleName` varchar(100) NOT NULL,
                        PRIMARY KEY (`RoleID`)
                    );
                    CREATE TABLE `supplier` (
                        `SupplierID` int NOT NULL AUTO_INCREMENT,
                        `SupplierName` varchar(100) NOT NULL,
                        PRIMARY KEY (`SupplierID`)
                    );
                    CREATE TABLE `user` (
                        `UserID` int NOT NULL AUTO_INCREMENT,
                        `UserSurname` varchar(100) NOT NULL,
                        `UserName` varchar(100) NOT NULL,
                        `UserPatronymic` varchar(100) NOT NULL,
                        `UserLogin` varchar(100) NOT NULL,
                        `UserPassword` varchar(100) NOT NULL,
                        `UserRole` int NOT NULL,
                        PRIMARY KEY (`UserID`),
                        KEY `user_ibfk_1` (`UserRole`),
                        CONSTRAINT `user_ibfk_1` FOREIGN KEY (`UserRole`) REFERENCES `role` (`RoleID`) ON DELETE CASCADE ON UPDATE CASCADE
                    );
                    CREATE TABLE `product` (
                        `ProductArticleNumber` varchar(6) NOT NULL,
                        `ProductName` varchar(100) NOT NULL,
                        `ProductUnit` varchar(100) NOT NULL,
                        `ProductCost` int NOT NULL,
                        `ProductSupplier` int NOT NULL,
                        `ProductCategory` int NOT NULL,
                        `ProductDiscountAmount` tinyint DEFAULT NULL,
                        `ProductCount` int NOT NULL,
                        `ProductDescription` varchar(100) NOT NULL,
                        `ProductPhoto` varchar(45) DEFAULT NULL,
                        PRIMARY KEY (`ProductArticleNumber`),
                        KEY `product_ibfk_2` (`ProductSupplier`),
                        KEY `product_ibfk_3` (`ProductCategory`),
                        CONSTRAINT `product_ibfk_2` FOREIGN KEY (`ProductSupplier`) REFERENCES `supplier` (`SupplierID`) ON DELETE CASCADE ON UPDATE CASCADE,
                        CONSTRAINT `product_ibfk_3` FOREIGN KEY (`ProductCategory`) REFERENCES `category` (`CategoryID`) ON DELETE CASCADE ON UPDATE CASCADE
                    );
                    CREATE TABLE `order` (
                        `OrderID` int NOT NULL AUTO_INCREMENT,
                        `OrderDate` date NOT NULL,
                        `OrderPickUpPoint` int NOT NULL,
                        `OrderUser` int NOT NULL,
                        `OrderPrice` int NOT NULL,
                        `OrderStatus` varchar(100) NOT NULL,
                        PRIMARY KEY (`OrderID`),
                        KEY `OrderPickUpPoint` (`OrderPickUpPoint`),
                        KEY `OrderUser` (`OrderUser`),
                        CONSTRAINT `order_ibfk_1` FOREIGN KEY (`OrderPickUpPoint`) REFERENCES `pickuppoint` (`PickUpPointID`),
                        CONSTRAINT `order_ibfk_2` FOREIGN KEY (`OrderUser`) REFERENCES `user` (`UserID`)
                    );
                    CREATE TABLE `orderproduct` (
                        `OrderID` int NOT NULL,
                        `ProductArticleNumber` varchar(100) NOT NULL,
                        `OrderProductCount` int NOT NULL,
                        PRIMARY KEY (`OrderID`,`ProductArticleNumber`),
                        KEY `orderproduct_ibfk_2` (`ProductArticleNumber`),
                        CONSTRAINT `orderproduct_ibfk_1` FOREIGN KEY (`OrderID`) REFERENCES `order` (`OrderID`) ON DELETE CASCADE ON UPDATE CASCADE,
                        CONSTRAINT `orderproduct_ibfk_2` FOREIGN KEY (`ProductArticleNumber`) REFERENCES `product` (`ProductArticleNumber`) ON DELETE CASCADE ON UPDATE CASCADE
                    );";

                    // Выполняем создание таблиц
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("База данных восстановлена успешно!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            }
        }
    }
}
