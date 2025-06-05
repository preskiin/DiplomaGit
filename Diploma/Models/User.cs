using Diploma.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Diploma.NewFolder1
{
    internal class User
    {
        Int32 _id;
        Int32 _id_position;
        String _name;
        String _surname;
        String _patronymic;
        Int32 _place_num;
        String _login;
        String _password;
        public Int32 id_pos { get { return _id_position; } }
        public String name { get { return _name; } }
        public String surname { get { return _surname; } }
        public String patronymic { get {return _patronymic; } }
        public Int32 place_num { get { return _place_num; } }
        public String login { get { return _login; } }
        public String password { get { return _password; } }

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

        public User(Int32 id = 0, Int32 id_position = 1003, String name = "nobody", String surname="nobody", String patronymic ="nobody", Int32 place_num = 0, String login = "C6C094BC0054F9CBE34102FF49F86B3928B5AC09F3D2AC87E170D0500675921F", String password = "C6C094BC0054F9CBE34102FF49F86B3928B5AC09F3D2AC87E170D0500675921F")
        {
            this._id = (Int32)id;
            this._id_position = (Int32)id_position;
            this._name = (String)name;
            this._surname = (String)surname;
            this._patronymic = (String)patronymic;
            this._place_num = (Int32)place_num;
            this._login = (String)login;
            this._password = (String)password;
        }

        // Создание объекта из SqlDataReader
        public static User FromDataReader(SqlDataReader reader)
        {
            User tmp = new User(
                id: reader.GetInt32(reader.GetOrdinal("id")),
                id_position: reader.GetInt32(reader.GetOrdinal("Id_position")),
                name: reader.GetString(reader.GetOrdinal("Name")),
                surname: reader.GetString(reader.GetOrdinal("Surname")),
                patronymic: reader.GetString(reader.GetOrdinal("Patronymic")),
                place_num: reader.GetInt32(reader.GetOrdinal("Place")),
                login: reader.GetString(reader.GetOrdinal("Login")),
                password: reader.GetString(reader.GetOrdinal("Password"))
            );
            return tmp;
        }
        // Проверка валидности данных
        //public Boolean IsValid()
        //{
        //    return (!String.IsNullOrEmpty(_name)
        //       && _sector > 0
        //       && _department > 0
        //       && _level > 0);
        //}
    }
}
