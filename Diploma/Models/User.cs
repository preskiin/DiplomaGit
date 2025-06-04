using System;
using System.Collections.Generic;
using System.Linq;
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
            this._id_position = 0;
            this._name = "name";
            this._surname = "surname";
            this._patronymic = "patronymic";
            this._place_num = 0;
            this._login = "login";
            this._password = "password";
        }

        public User(String login = "login", String password = "password")
        {
            
            this._login = (String)login;
            this._password = (String)password;
        }

        public User(Int32 id, Int32 id_position, String name, String surname, String patronymic, Int32 place_num, String login, String password)
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

    }
}
