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
        public Int32 id_pos { get; }
        public String name { get; }
        public String surname { get; }
        public String patronymic { get; }
        public Int32 place_num { get; }
        public String login { get; }
        public String password { get; }

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
            
            this._login = login;
            this._password = password;
        }

        public User(Int32 id=0, Int32 id_position = 0, String name = "name", String surname = "surname", String patronymic = "patronymic", Int32 place_num = 0, String login= "login", String password = "password")
        {
            this._id = id;
            this._id_position = id_position;
            this._name = name;
            this._surname = surname;
            this._patronymic = patronymic;
            this._place_num = place_num;
            this._login = login;
            this._password = password;
        }

    }
}
