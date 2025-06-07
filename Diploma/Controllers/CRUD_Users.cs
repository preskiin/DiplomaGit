using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diploma.Models;

namespace Diploma.Controllers
{
    internal class CRUD_Users
    {
        private String _connectionString;
        private Int32 _pageSize=50;
        public CRUD_Users(string con_str)
        {
            _connectionString = con_str;
        }

        //Возвращает либо индекс созданной записи, либо индекс ошибки
        public Int32 create(User new_user)
        {
            Int32 res;
            if (is_in_base(new_user.Login))
                res = -2; //Индекс, указывающий, что такой логин в базе уже есть
            else if (!new_user.IsValid())
                res = -1; //Индекс, указывающий на неверные данные при вводе
            else
            {
                SqlConnection con = new SqlConnection(_connectionString);
                String sql_exp = @"
                INSERT INTO People (Id_position, Name, Surname, Patronymic, Place, Login, Password) 
                VALUES (@id_pos, @name, @surname, @patro, @place, @log, @pas)";
                SqlCommand command = new SqlCommand(sql_exp, con);
                command.Parameters.Add(new SqlParameter("@id_pos", new_user.IdPosition));
                command.Parameters.Add(new SqlParameter("@name", new_user.Name));
                command.Parameters.Add(new SqlParameter("@surname", new_user.Surname));
                command.Parameters.Add(new SqlParameter("@patro", new_user.Patronymic));
                command.Parameters.Add(new SqlParameter("@place", new_user.Place));
                command.Parameters.Add(new SqlParameter("@log", new_user.Login));
                command.Parameters.Add(new SqlParameter("@pas", new_user.Password));
                con.Open();
                res = (Int32)command.ExecuteScalar();
                con.Close();
            }
            return res;
        }

        //функция чтения одного пользователя из базы по логину-хэшу
        public User read(String login)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
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
                tmp_user = User.FromDataReader(reader);
            }
            else
                tmp_user= null;
            reader.Close();
            connection.Close();
            return tmp_user;
        }

        //Возвращает страницу из пользователей (заданное число записей)
        public IEnumerable<User> getPage(Int32 pageNumber)
        {
            String sql = @"
            SELECT * FROM People
            ORDER BY id
            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY";
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Offset", (pageNumber - 1) * _pageSize);
            command.Parameters.AddWithValue("@PageSize", _pageSize);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                yield return User.FromDataReader(reader);
            }
        }

        //Получает всех пользователей, с указнной должностью
        public List<User> readByPositionId(Int32 positionId, Int32 pageNum)
        {
            var users = new List<User>();
            String sql_exp = "SELECT * FROM Users WHERE Id_position=@IdPosition";
            //String sql_exp = @"
            //SELECT * FROM Users
            //WHERE Id_position = @IdPosition
            //ORDER BY id
            //OFFSET @Offset ROWS
            //FETCH NEXT @PageSize ROWS ONLY";
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand(@sql_exp, connection);
            Int32 offset = (pageNum-1) * _pageSize;
            //command.Parameters.AddWithValue("Offset", offset);
           // command.Parameters.AddWithValue("@PageSize", _pageSize);
            command.Parameters.AddWithValue("@IdPosition", positionId);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                users.Add(User.FromDataReader(reader));
            }
            reader.Close();
            connection.Close();
            return users;
        }

        //Отправляет запрос на обновление указанного экземпляра по ID
        public Int32 update(User user)
        {
            Int32 result;
            if (!user.IsValid())
                result = -1; //индекс неверных данных
            else
            {
                String sql_exp = @"
                UPDATE Users
                SET
                    Id_position=@IdPosition,
                    Name = @Name,
                    Surname=@Surname,
                    Patronymic=@Patronymic,
                    Place=@Place,
                    Login= @Login,
                    Password=@Password
                WHERE id=@Id";
                SqlConnection connecton = new SqlConnection(_connectionString);
                SqlCommand command = new SqlCommand(sql_exp, connecton);
                command.Parameters.AddWithValue("@Id", user.Id);
                command.Parameters.AddWithValue("@IdPosition", user.IdPosition);
                command.Parameters.AddWithValue("@Name", user.Name);
                command.Parameters.AddWithValue("@Surname", user.Surname);
                command.Parameters.AddWithValue("@Patronymic", user.Patronymic);
                command.Parameters.AddWithValue("@Place", user.Place);
                command.Parameters.AddWithValue("@Login", user.Login);
                command.Parameters.AddWithValue("@Password", user.Password);
                connecton.Open();
                command.ExecuteNonQuery();
                connecton.Close();
                result = 1;//Индекс успешного ввода значений
            }
            return result;
        }

        //Удаляет пользователя по ID. 
        public void delete(User userToDel)
        {
            String sql_exp = "DELETE FROM Users WHERE id = @Id";
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand(sql_exp, connection);
            command.Parameters.AddWithValue("@Id", userToDel.Id);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }

        //функция возвращающая пароль из базы по хэшу-логину
        public String get_pas(String login)
        {
            SqlConnection con = new SqlConnection(_connectionString);
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

        //функция проверки логина в пользователях.
        public bool is_in_base(string login)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
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
                reader.Close();
                return tmp_res;
            }
        }

    }
}
