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
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        return true;
                    }
                    else
                        return false;
                }
            }
        }

        public User read(String login)
        {

            using (SqlConnection connection = new SqlConnection(connection_string))
            {
                User tmp_user = null;
                connection.Open();
                String sql_expression = "SELECT * FROM People WHERE Login COLLATE Latin1_General_CS_AS = @login";
                SqlParameter login_param = new SqlParameter("@login", login);
                SqlCommand command = new SqlCommand(sql_expression, connection);
                command.Parameters.Add(login_param);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            tmp_user = new User(Convert.ToInt32(reader.GetValue(0)),Convert.ToInt32(reader.GetValue(1)), Convert.ToString(reader.GetValue(2)), Convert.ToString(reader.GetValue(3)),
                                Convert.ToString(reader.GetValue(4)), Convert.ToInt32(reader.GetValue(5)), Convert.ToString(reader.GetValue(6)), Convert.ToString(reader.GetValue(7)));
                        }
                        return tmp_user;
                    }
                    else
                        return null;

                }
            }
        }

        public String get_log_pas(String login)
        {
            using (SqlConnection con = new SqlConnection(connection_string))
            {
                con.Open();
                String sql_exp = "SELECT Login, Password FROM People WHERE Login COLLATE Latin1_General_CS_AS = @login";
                SqlParameter log_par = new SqlParameter("@login", login);
                SqlCommand cmd = new SqlCommand(sql_exp, con);
                cmd.Parameters.Add(log_par);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        String tmp_str = "";
                        while (reader.Read())
                        {
                            tmp_str += reader.GetValue(0) + " " + reader.GetValue(1);
                        }
                        return tmp_str;
                    }
                    else
                        return "None";
                }
            }
        }


    }
}
