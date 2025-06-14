using Diploma.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diploma.Controllers
{
    internal class CRUD_Positions
    {
        private String _connectionString;
        private int _pageSize = 50;
        public CRUD_Positions(String connectionString)
        {
            _connectionString = connectionString /*?? throw new ArgumentNullException(nameof(connectionString))*/;
        }

        // Создает новую запись в таблице Positions.
        public Int64 create(Position position)
        {
            Int64 tmp;
            if (!position.IsValid())
                tmp = -1; //возврат неверного ввода данных
            else if (checkPosInBase(position))
                tmp = -2; //возврат существования записи в таблице
            else
            {
                String sql_exp = @"
                INSERT INTO Positions (Name, Sector, Department, Leve1)
                OUTPUT INSERTED.id
                VALUES (@Name, @Sector, @Department, @Level)";
                SqlConnection connection = new SqlConnection(_connectionString);
                var command = new SqlCommand(sql_exp, connection);
                command.Parameters.AddWithValue("@Name", position.Name);
                command.Parameters.AddWithValue("@Sector", position.Sector);
                command.Parameters.AddWithValue("@Department", position.Department);
                command.Parameters.AddWithValue("@Level", position.Level);
                connection.Open();
                tmp = (Int64)command.ExecuteScalar();// Возвращает ID новой записи
                connection.Close();
            }
            return tmp;
        }

        //Получает записи с пагинацией
        public List<Position> getPage(int pageNumber)
        {
            //if (pageNumber < 1)
            //    throw new ArgumentException("Номер страницы должен быть >= 1");

            List<Position> positions = new();
            String sql_exp = @"
            SELECT * FROM Positions
            ORDER BY id
            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY";
            SqlConnection connection = new(_connectionString);
            SqlCommand command = new SqlCommand(sql_exp, connection);
            int offset = (pageNumber - 1) * _pageSize;
            command.Parameters.AddWithValue("@Offset", offset);
            command.Parameters.AddWithValue("@PageSize", _pageSize);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                positions.Add(Position.FromDataReader(reader));
            }
            reader.Close();
            connection.Close();
            return positions;
        }

        //Получает записи с заданной пагинацией для вывода в dataGridView
        public System.Data.DataTable getPageAsDataTable(int pageNumber)
        {
            if (pageNumber < 1)
                throw new ArgumentException("Номер страницы должен быть >= 1");
            var dataTable = new System.Data.DataTable();
            String sql_exp = @"
            SELECT id, Name, Sector, Department, Leve1 
            FROM Positions
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

        // Получает позицию по ID.
        public Position read(Int64 id)
        {
            String sql_exp = "SELECT * FROM Positions WHERE id = @Id";
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand cmd= new SqlCommand(sql_exp, connection);
            cmd.Parameters.AddWithValue("@Id", id);
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            Position tmp_pos;
            if (reader.Read())
            {
                tmp_pos = Position.FromDataReader(reader);
            }
            else
            {
                tmp_pos = null;
            }
            reader.Close();
            connection.Close();
            return tmp_pos;
        }

        // Обновляет существующую позицию.
        public Int64 update(Position position)
        {
            Int64 result;
            if (!position.IsValid())
                result = -1; //Индекс неверных данных
            else
            {
                String sql_exp = @"
                UPDATE Positions 
                SET 
                    Name = @Name,
                    Sector = @Sector,
                    Department = @Department,
                    Leve1 = @Level
                WHERE id = @Id";
                SqlConnection connection = new SqlConnection(_connectionString);
                SqlCommand command = new SqlCommand(sql_exp, connection);
                command.Parameters.AddWithValue("@Id", position.Id);
                command.Parameters.AddWithValue("@Name", position.Name);
                command.Parameters.AddWithValue("@Sector", position.Sector);
                command.Parameters.AddWithValue("@Department", position.Department);
                command.Parameters.AddWithValue("@Level", position.Level);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
                result = 1;//индекс успешного ввода новых данных
            }
            return result;
        }

        //Удаляет позицию по ID. 
        public void delete(Position posToDel)
        {
            String sql_exp = "DELETE FROM Positions WHERE id = @Id";
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand(sql_exp, connection);
            command.Parameters.AddWithValue("@Id", posToDel.Id);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }

        //Проверяет, есть ли должность с таким же названием в базе
        private bool checkPosInBase(Position position)
        {
            bool result;
            String sql_exp = @"
            SELECT * 
            FROM Positions 
            WHERE Name COLLATE Latin1_General_CS_AS = @name";
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand cmd = new SqlCommand (sql_exp, connection);
            cmd.Parameters.AddWithValue("@name", position.Name);
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
                result = true;
            else
                result = false;
            reader.Close();
            connection.Close();
            return result;
        }
        
        public static List<Position> getAllPositions(String connection)
        {
            List<Position> positions = new();
            String sql_exp = @"
            SELECT * FROM Positions
            ORDER BY Name ASC";
            SqlConnection con = new(connection);
            SqlCommand command = new SqlCommand(sql_exp, con);
            con.Open();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                positions.Add(Position.FromDataReader(reader));
            }
            reader.Close();
            con.Close();
            return positions;
        }

        //создает html-код выпадающего списка со значениями должностей из базы
        //public static string generatePositionsDropdown(string connectionString, string currentValue = "", string dropdownName = "positionId")
        //{
        //    var html = new StringBuilder();
        //    string sql = "SELECT id, Name FROM Positions ORDER BY Name";
        //    var connection = new SqlConnection(connectionString);
        //    var command = new SqlCommand(sql, connection);
        //    connection.Open();
        //    var reader = command.ExecuteReader();
        //    html.AppendLine($"<select name='{dropdownName}' id='{dropdownName}' class='form-control'>");
        //    html.AppendLine("<option value=''>-- Выберите должность --</option>");
        //    while (reader.Read())
        //    {
        //        Int64 id = reader.GetInt64(0);
        //        string name = reader.GetString(1);
        //        bool isSelected = id.ToString() == currentValue;
        //        html.AppendLine(
        //            $"<option value='{id}' {(isSelected ? "selected" : "")}>{name}</option>"
        //        );
        //    }
        //    reader.Close();
        //    connection.Close();
        //    html.AppendLine("</select>");
        //    return html.ToString();
        //}

        public static string generatePositionsDropdown(string connectionString, string currentValue = "", string dropdownName = "positionId")
        {
            var html = new StringBuilder();
            string sql = "SELECT id, Name FROM Positions ORDER BY Name";
            var connection = new SqlConnection(connectionString);
            var command = new SqlCommand(sql, connection);

            // Получаем список всех должностей для проверки введенного значения
            var positions = new Dictionary<string, long>();
            connection.Open();
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                long id = reader.GetInt64(0);
                string name = reader.GetString(1);
                positions[name] = id;
            }
            reader.Close();
            connection.Close();

            // Определяем текущее название должности по ID (если currentValue - это ID)
            string currentName = "";
            if (!string.IsNullOrEmpty(currentValue))
            {
                if (long.TryParse(currentValue, out long currentId))
                {
                    currentName = positions.FirstOrDefault(x => x.Value == currentId).Key ?? "";
                }
                else
                {
                    // Если currentValue - это название, проверяем его наличие в списке
                    if (positions.ContainsKey(currentValue))
                    {
                        currentName = currentValue;
                    }
                }
            }

            // Создаем input с datalist
            html.AppendLine($"<input list='{dropdownName}-list' name='{dropdownName}' id='{dropdownName}' value='{currentName}' class='form-control' placeholder='-- Выберите должность --'>");
            html.AppendLine($"<datalist id='{dropdownName}-list'>");

            // Добавляем варианты в datalist
            foreach (var position in positions)
            {
                html.AppendLine($"<option value='{position.Key}' data-id='{position.Value}'>");
            }

            html.AppendLine("</datalist>");

            // Добавляем скрытое поле для хранения ID
            html.AppendLine($"<input type='hidden' name='{dropdownName}-id' id='{dropdownName}-id' value='{currentValue}'>");

            // Добавляем JavaScript для валидации введенного значения
            html.AppendLine($@"
            <script>
            document.addEventListener('DOMContentLoaded', function() {{
                const input = document.getElementById('{dropdownName}');
                const options = document.getElementById('{dropdownName}-list').options;
                const hiddenField = document.getElementById('{dropdownName}-id');
                
                // Основная функция валидации
                function validateInput() 
                {{
                    let isValid = false;
                    for(let i = 0; i < options.length; i++) 
                    {{
                        if(options[i].value === input.value) 
                        {{
                            isValid = true;
                            hiddenField.value = options[i].getAttribute('data-id');
                            break;
                        }}
                    }}
                    if(!isValid) 
                    {{
                        input.value = '';
                        hiddenField.value = '';
                    }}
                }}
                
                // Обработчики событий
                input.addEventListener('blur', validateInput); // При потере фокуса
                input.addEventListener('keydown', function(e) 
                {{
                    if(e.key === 'Enter') validateInput(); // При нажатии Enter
                }});
            }});
            </script>");
            return html.ToString();
        }

    }
}
