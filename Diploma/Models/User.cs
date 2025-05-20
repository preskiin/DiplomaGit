using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diploma.NewFolder1
{
    internal class User
    {
        Int32 id;
        Int32 id_position;
        String name;
        String surname;
        String patronymic;
        Int32 place_num;
        String login;
        String password;

        public User()
        {
            this.id = 0;
            this.id_position = 0;
            this.name = "name";
            this.surname = "surname";
            this.patronymic = "patronymic";
            this.place_num = 0;
            this.login = "login";
            this.password = "password";
        }

        public User(string login = "login", string password = "password")
        {
            
            this.login = login;
            this.password = password;
        }

        public User(int id=0, int id_position = 0, string name = "name", string surname = "surname", string patronymic = "patronymic", int place_num = 0, string login= "login", string password = "password")
        {
            this.id = id;
            this.id_position = id_position;
            this.name = name;
            this.surname = surname;
            this.patronymic = patronymic;
            this.place_num = place_num;
            this.login = login;
            this.password = password;
        }

    }
}
