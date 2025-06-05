using Diploma.NewFolder1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diploma.Controllers
{
    internal class CRUD_Users
    {
        private String connection_string;

        public CRUD_Users(string con_str)
        {
            connection_string = con_str;
        }

        //функция проверки логина в пользователях.
        public bool is_in_base(string login)
        {
            using (SqlConnection con = new SqlConnection(connection_string))
            {
                con.Open();
                String sql_exp = "SELECT Login FROM People WHERE Login COLLATE Latin1_General_CS_AS = @login";
                SqlParameter log_par = new SqlParameter("@login", login);
                SqlCommand cmd = new SqlCommand(sql_exp, con);
                cmd.Parameters.Add(log_par);
                SqlDataReader reader = cmd.ExecuteReader();
                bool tmp_res;
                if (reader.Read())
                    tmp_res = true;
                else
                    tmp_res = false;
                return tmp_res;
            }
        }
        //функция чтения одного пользователя из базы по логину
        public User read(String login)
        {
            SqlConnection connection = new SqlConnection(connection_string);
            String sql_exp = @"
            SELECT * 
            FROM People 
            WHERE Login COLLATE Latin1_General_CS_AS = @login";
            SqlCommand command = new SqlCommand(sql_exp, connection);
            command.Parameters.Add(new SqlParameter("@login", login));
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            User tmp_user;
            if (reader.Read())
            {
                //while (reader.Read())
                //{
                //    tmp_user = new User(Convert.ToInt32(reader.GetValue(0)),Convert.ToInt32(reader.GetValue(1)), Convert.ToString(reader.GetValue(2)), Convert.ToString(reader.GetValue(3)),
                //        Convert.ToString(reader.GetValue(4)), Convert.ToInt32(reader.GetValue(5)), Convert.ToString(reader.GetValue(6)), Convert.ToString(reader.GetValue(7)));
                //}
                //return tmp_user;
                tmp_user = User.FromDataReader(reader);
            }
            else
                tmp_user= null;
            reader.Close();
            connection.Close();
            return tmp_user;
        }

        //функция возвращающая пароль из базы по хэшу-логину
        public String get_pas(String login)
        {
            SqlConnection con = new SqlConnection(connection_string);
            String sql_exp = @"
            SELECT Password 
            FROM People 
            WHERE Login COLLATE Latin1_General_CS_AS = @login";
            SqlCommand cmd = new SqlCommand(sql_exp, con);
            cmd.Parameters.AddWithValue("@login", login);
            con.Open();
            String tmp_str;
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                //String tmp_str = "";
                //while (reader.Read())
                //{
                //    tmp_str += reader.GetValue(0) + " " + reader.GetValue(1);
                //}
                //con.Close();
                tmp_str = reader.GetString(reader.GetOrdinal("Password"));
            }
            else
            {
                tmp_str = null;
            }
            reader.Close();
            con.Close();
            return tmp_str;
        }

        public Int32 create(User new_user)
        {
            Int32 res;
            if (!is_in_base(new_user.login))
            {
                SqlConnection con = new SqlConnection(connection_string);
                String sql_exp = @"
                INSERT INTO People (Id_position, Name, Surname, Patronymic, Place, Login, Password) 
                VALUES (@id_pos, @name, @surname, @patro, @place, @log, @pas)";
                SqlCommand command = new SqlCommand(sql_exp, con);
                command.Parameters.Add(new SqlParameter("@id_pos", new_user.id_pos));
                command.Parameters.Add(new SqlParameter("@name", new_user.name));
                command.Parameters.Add(new SqlParameter("@surname", new_user.surname));
                command.Parameters.Add(new SqlParameter("@patro", new_user.patronymic));
                command.Parameters.Add(new SqlParameter("@place", new_user.place_num));
                command.Parameters.Add(new SqlParameter("@log", new_user.login));
                command.Parameters.Add(new SqlParameter("@pas", new_user.password));
                con.Open();
                res = (Int32)command.ExecuteScalar();
                con.Close();
            }
            else
                res = -1;
            return res;
        }
    }
}
