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
        private Int64 _id_position;
        private String _name;
        private String _surname;
        private String _patronymic;
        private Int64 _place_num;
        private String _login;
        private String _password;
        public Int64 Id { get { return _id; } }
        public Int64 IdPosition { get { return _id_position; } }
        public String Name { get { return _name; } }
        public String Surname { get { return _surname; } }
        public String Patronymic { get { return _patronymic; } }
        public Int64 Place { get { return _place_num; } }
        public String Login { get { return _login; } }
        public String Password { get { return _password; } }

        public User()
        {
            this._id = 0;
            this._id_position = 1003;
            this._name = "nobody";
            this._surname = "nobody";
            this._patronymic = "nobody";
            this._place_num = 0;
            this._login = "C6C094BC0054F9CBE34102FF49F86B3928B5AC09F3D2AC87E170D0500675921F";
            this._password = "C6C094BC0054F9CBE34102FF49F86B3928B5AC09F3D2AC87E170D0500675921F";
        }

        public User(Int64 id, Int64 id_position, String name, String surname, String patronymic, Int64 place_num, String login, String password)
        {
            this._id = (Int64)id;
            this._id_position = (Int64)id_position;
            this._name = (String)name;
            this._surname = (String)surname;
            this._patronymic = (String)patronymic;
            this._place_num = (Int64)place_num;
            this._login = (String)login;
            this._password = (String)password;
        }

        //конструктор копирования
        public User(User userToCopy)
        {
            this._id = userToCopy._id;
            this._name = userToCopy._name;
            this._surname= userToCopy._surname;
            this._id_position= userToCopy._id_position;
            this._place_num= userToCopy._place_num;
            this._login = userToCopy._login;
            this._password = userToCopy._password;
        }

        // Создание объекта из SqlDataReader
        public static User FromDataReader(SqlDataReader reader)
        {
            User tmp = new User(
                id: reader.GetInt64(reader.GetOrdinal("id")),
                id_position: reader.GetInt64(reader.GetOrdinal("Id_position")),
                name: reader.GetString(reader.GetOrdinal("Name")),
                surname: reader.GetString(reader.GetOrdinal("Surname")),
                patronymic: reader.GetString(reader.GetOrdinal("Patronymic")),
                place_num: reader.GetInt64(reader.GetOrdinal("Place")),
                login: reader.GetString(reader.GetOrdinal("Login")),
                password: reader.GetString(reader.GetOrdinal("Password"))
            );
            return tmp;
        }

        //Получение пользователя из ряда датагрида
        public User(DataGridViewRow row)
        {
            if (row != null)
            {
                _id = Convert.ToInt64(row.Cells["id"].Value);
                _id_position = Convert.ToInt64(row.Cells["Id_position"].Value);
                _name = row.Cells["Name"].Value.ToString();
                _surname = row.Cells["Surname"].Value.ToString();
                _patronymic = row.Cells["Patronymic"].Value.ToString();
                _place_num = Convert.ToInt64(row.Cells["Place"].Value);
                _login = row.Cells["Login"].Value.ToString();
                _password = row.Cells["Password"].Value.ToString();
            }
        }

        //Проверка валидности данных
        public Boolean IsValid()
        {
            return (!String.IsNullOrEmpty(_name)
               && _id_position > 0
               && !String.IsNullOrEmpty(_surname)
               && !String.IsNullOrEmpty(_login)
               && !String.IsNullOrEmpty(_password));
        }
    }
}
