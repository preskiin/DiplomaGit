using Diploma.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diploma.Models
{
    public class User
    {
        private Int64 _id;
        private Int64? _id_position;  // Изменено на nullable тип
        private String _name;
        private String _surname;
        private String _patronymic;
        private Int32 _place_num;
        private String _login;
        private String _password;

        public Int64 Id { get { return _id; } }
        public Int64? IdPosition { get { return _id_position; } }  // Изменено на nullable
        public String Name { get { return _name; } }
        public String Surname { get { return _surname; } }
        public String Patronymic { get { return _patronymic; } }
        public Int32 Place { get { return _place_num; } }
        public String Login { get { return _login; } }
        public String Password { get { return _password; } }

        public User()
        {
            this._id = 0;
            this._id_position = null;  // Значение по умолчанию - null
            this._name = "nobody";
            this._surname = "nobody";
            this._patronymic = "nobody";
            this._place_num = 0;
            this._login = "C6C094BC0054F9CBE34102FF49F86B3928B5AC09F3D2AC87E170D0500675921F";
            this._password = "C6C094BC0054F9CBE34102FF49F86B3928B5AC09F3D2AC87E170D0500675921F";
        }

        public User(Int64 id, Int64? id_position, String name, String surname,
                   String patronymic, Int32 place_num, String login, String password)
        {
            this._id = id;
            this._id_position = id_position;  // Может быть null
            this._name = name;
            this._surname = surname;
            this._patronymic = patronymic;
            this._place_num = place_num;
            this._login = login;
            this._password = password;
        }

        // Конструктор копирования
        public User(User userToCopy)
        {
            this._id = userToCopy._id;
            this._id_position = userToCopy._id_position;
            this._name = userToCopy._name;
            this._surname = userToCopy._surname;
            this._patronymic = userToCopy._patronymic;
            this._place_num = userToCopy._place_num;
            this._login = userToCopy._login;
            this._password = userToCopy._password;
        }

        // Создание объекта из SqlDataReader
        public static User FromDataReader(SqlDataReader reader)
        {
            return new User(
                id: reader.GetInt64(reader.GetOrdinal("id")),
                id_position: reader.IsDBNull(reader.GetOrdinal("Id_position")) ?
                    (Int64?)null : reader.GetInt64(reader.GetOrdinal("Id_position")),
                name: reader.GetString(reader.GetOrdinal("Name")),
                surname: reader.GetString(reader.GetOrdinal("Surname")),
                patronymic: reader.GetString(reader.GetOrdinal("Patronymic")),
                place_num: reader.GetInt32(reader.GetOrdinal("Place")),
                login: reader.GetString(reader.GetOrdinal("Login")),
                password: reader.GetString(reader.GetOrdinal("Password"))
            );
        }

        // Получение пользователя из ряда DataGridView
        public User(DataGridViewRow row)
        {
            if (row != null)
            {
                _id = Convert.ToInt64(row.Cells["id"].Value);
                _id_position = row.Cells["IdPosition"].Value == DBNull.Value ?
                    null : (Int64?)Convert.ToInt64(row.Cells["IdPosition"].Value);
                _name = row.Cells["Name"].Value.ToString();
                _surname = row.Cells["Surname"].Value.ToString();
                _patronymic = row.Cells["Patronymic"].Value.ToString();
                _place_num = Convert.ToInt32(row.Cells["Place"].Value);
                _login = row.Cells["Login"].Value.ToString();
                _password = row.Cells["Password"].Value.ToString();
            }
        }

        // Проверка валидности данных (убрана проверка _id_position > 0)
        public Boolean IsValid()
        {
            return (!String.IsNullOrEmpty(_name)
               && !String.IsNullOrEmpty(_surname)
               && !String.IsNullOrEmpty(_login)
               && !String.IsNullOrEmpty(_password));
        }
    }
}
