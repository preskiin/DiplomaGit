using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diploma.Models;
using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json.Schema;

namespace Diploma.Controllers
{
    internal class CRUD_Users
    {
        private String _connectionString;
        private int _pageSize=50;
        public CRUD_Users(string con_str)
        {
            _connectionString = con_str;
        }

        //Возвращает либо индекс созданной записи, либо индекс ошибки
        public Int64 create(User new_user)
        {
            Int64 res;
            if (is_in_base(new_user.Login))
                res = -2; //Индекс, указывающий, что такой логин в базе уже есть
            else if (!new_user.IsValid())
                res = -1; //Индекс, указывающий на неверные данные при вводе
            else
            {
                SqlConnection con = new SqlConnection(_connectionString);
                String sql_exp = @"
                INSERT INTO People (Id_position, Name, Surname, Patronymic, Place, Login, Password) 
                OUTPUT INSERTED.id
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
                res = (Int64)command.ExecuteScalar();
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

        //возвращает user с указанным ID
        public User read(Int64 id)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            String sql_exp = @"
            SELECT * 
            FROM People 
            WHERE id = @id";
            SqlCommand command = new SqlCommand(sql_exp, connection);
            command.Parameters.Add(new SqlParameter("@id", id));
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            User tmp_user;
            if (reader.Read())
            {
                tmp_user = User.FromDataReader(reader);
            }
            else
                tmp_user = null;
            reader.Close();
            connection.Close();
            return tmp_user;
        }

        //Возвращает страницу из пользователей (заданное число записей)
        public IEnumerable<User> getPage(int pageNumber)
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

        public System.Data.DataTable getPageAsDataTable(int pageNumber)
        {
            if (pageNumber < 1)
                throw new ArgumentException("Номер страницы должен быть >= 1");
            var dataTable = new System.Data.DataTable();
            String sql_exp = @"
            SELECT * 
            FROM People
            ORDER BY id
            OFFSET @Offset ROWS 
            FETCH NEXT @PageSize ROWS ONLY";

            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand(sql_exp, connection);
            int offset = (pageNumber - 1) * _pageSize;
            command.Parameters.AddWithValue("@Offset", offset);
            command.Parameters.AddWithValue("@PageSize", _pageSize);
            connection.Open();
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(dataTable);
            connection.Close();
            return dataTable;
        }

        //Получает всех пользователей, с указнной должностью
        public List<User> readByPositionId(Int64 positionId, int pageNum)
        {
            var users = new List<User>();
            String sql_exp = "SELECT * FROM Users WHERE Id_position=@IdPosition";
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand(@sql_exp, connection);
            int offset = (pageNum-1) * _pageSize;
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
        public Int64 update(User user)
        {
            Int64 result;
            if (!user.IsValid())
                result = -1; //индекс неверных данных
            else
            {
                String sql_exp = @"
                UPDATE People
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
            String sql_exp = "DELETE FROM People WHERE id = @Id";
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

        //создает html-код выпадающего списка со значениями ФИО людей из базы
        public static string generateUsersDropdown(string connectionString, int counter, string currentValue = "")
        {
            String dropdownName = "element" + Convert.ToString(counter);
            var html = new StringBuilder();
            string sql = "SELECT id, Name, Surname, Patronymic FROM People ORDER BY Name";
            var connection = new SqlConnection(connectionString);
            var command = new SqlCommand(sql, connection);
            connection.Open();
            var reader = command.ExecuteReader();
            var users = new Dictionary<string, long>();
            while (reader.Read()) 
            {
                users[reader.GetString(2) + " " + reader.GetString(1) + " " + reader.GetString(3)] = reader.GetInt64(0);
            }
            // Определяем текущее название должности по ID (если currentValue - это ID)
            string currentName = "";
            if (!string.IsNullOrEmpty(currentValue))
            {
                if (long.TryParse(currentValue, out long currentId))
                {
                    currentName = users.FirstOrDefault(x => x.Value == currentId).Key ?? "";
                }
                else
                {
                    // Если currentValue - это название, проверяем его наличие в списке
                    if (users.ContainsKey(currentValue))
                    {
                        currentName = currentValue;
                    }
                }
            }
            html.AppendLine(@$"
                <div class='template2003' 
                data-name-element='{dropdownName}'
                data-show-field='null'
                data-name-to-connect='base'
                data-is-filled='false'
                data-value='null'
                data-class-name='user'>
                ");

            // Создаем input с datalist
            html.AppendLine($"<input list='{dropdownName}-list' name='{dropdownName}' id='{dropdownName}' value='{currentName}' class='form-control' placeholder='-- {dropdownName} --'>");
            html.AppendLine($"<datalist id='{dropdownName}-list'>");

            // Добавляем варианты в datalist
            foreach (var user in users)
            {
                html.AppendLine($"<option value='{user.Key}' data-id='{user.Value}'>");
            }

            html.AppendLine("</datalist>");

            // Добавляем скрытое поле для хранения ID
            html.AppendLine($"<input type='hidden' name='{dropdownName}-id' id='{dropdownName}-id' value='{currentValue}'>");

            // Добавляем JavaScript для валидации введенного значения
            html.AppendLine($@"
            <script>
            document.addEventListener('DOMContentLoaded', function() {{
                const input{counter} = document.getElementById('{dropdownName}');
                const options{counter} = document.getElementById('{dropdownName}-list').options;
                const hiddenField{counter} = document.getElementById('{dropdownName}-id');
                let isSending=false;
                // Основная функция валидации
                function validateInput() 
                {{
                    if (isSending)return;
                    isSending=true;
                    let isValid = false;
                    for(let i = 0; i < options{counter}.length; i++) 
                    {{
                        if(options{counter}[i].value === input{counter}.value) 
                        {{
                            isValid = true;
                            const selectedId = options{counter}[i].getAttribute('data-id');
                            hiddenField{counter}.value = selectedId;
                            // Формируем сообщение для отправки
                            const message = JSON.stringify({{
                            listId: '{dropdownName}',
                            selectedId: selectedId
                            }});
                            // Отправляем ID в C# код
                            if(window.chrome && chrome.webview) 
                            {{
                                chrome.webview.postMessage(message);
                            }}
                            break;
                        }}
                    }}
                    if(!isValid) 
                    {{
                        input{counter}.value = '';
                        hiddenField{counter}.value = '';
                    }}
                    setTimeout(() => {{ isSending = false; }}, 100);
                }}
                
                // Обработчики событий
                input{counter}.addEventListener('blur', validateInput); // При потере фокуса
                
                input{counter}.addEventListener('keydown', function(e) 
                {{
                    if (e.key === 'Enter' && e.currentTarget === input{counter}) 
                    {{
                        validateInput();
                    }}
                }});
            }});
            </script>
            </div>");
            return html.ToString();
        }
    }
}
