using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WearShop
{
    /// <summary>
    /// Класс, содержащий статические значения, используемые в приложении.
    /// </summary>
    class Values
    {
        /// <summary>
        /// Роль пользователя в системе.
        /// </summary>
        public static int UserRole;

        /// <summary>
        /// ID пользователя в системе.
        /// </summary>
        public static int UserID;

        /// <summary>
        /// ФИО пользователя.
        /// </summary>
        public static string UserFIO;

        /// <summary>
        /// Флаг, указывающий на необходимость очистки заказа.
        /// </summary>
        public static bool clearOrder;

        /// <summary>
        /// Адрес хоста базы данных.
        /// </summary>
        public static string DBHost = "10.207.106.12";

        /// <summary>
        /// Имя пользователя для подключения к базе данных.
        /// </summary>
        public static string DBUser = "user30";

        /// <summary>
        /// Пароль для подключения к базе данных.
        /// </summary>
        public static string DBPassword = "aj98";
    }
}
