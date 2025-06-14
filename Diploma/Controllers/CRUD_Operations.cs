using Diploma.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diploma.Controllers
{
    public class CRUD_Operations
    {
        private String _connectionString;
        private int _pageSize = 50;
        public CRUD_Operations(String connectionString)
        {
            _connectionString = connectionString;
        }

        // Создает новую операцию (возвращает ID созданной записи)
        public Int64 create(Operation operation)
        {
            Int64 result;
            if (!operation.IsValid())
                result = -1; //Индекс неверного ввода данных
            else if (checkOperInBase(operation))
                result = -2; //Индекс существования в базе записи с таким именем и должностью
            else
            {
                String sql_exp = @"
                INSERT INTO Operations (Id_position, Name, Description)
                OUTPUT INSERTED.id
                VALUES (@IdPosition, @Name, @Description)";
                SqlConnection connection = new SqlConnection(_connectionString);
                SqlCommand command = new SqlCommand(sql_exp, connection);
                command.Parameters.AddWithValue("@IdPosition", operation.IdPosition);
                command.Parameters.AddWithValue("@Name", operation.Name);
                command.Parameters.AddWithValue("@Description", operation.Description);
                connection.Open();
                result = (Int64)command.ExecuteScalar();
                connection.Close();
            }
            return result;
        }

        // Получает операцию по ID
        public Operation read(Int64 id)
        {
            Operation tmp;
            String sql_exp = "SELECT * FROM Operations WHERE id = @Id";
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand(sql_exp, connection);
            command.Parameters.AddWithValue("@Id", id);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
                tmp = Operation.FromDataReader(reader);
            tmp = null;
            reader.Close();
            connection.Close();
            return tmp;
        }

        // Получает все операции для указанной должности (Id_position)
        public List<Operation> readByPositionId(Int64 positionId)
        {
            List<Operation> operations = new List<Operation>();
            String sql_exp = "SELECT * FROM Operations WHERE Id_position = @PositionId";
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand(sql_exp, connection);
            command.Parameters.AddWithValue("@PositionId", positionId);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                operations.Add(Operation.FromDataReader(reader));
            }
            reader.Close();
            connection.Close();
            return operations;
        }

        //Получает записи операций с пагинацией
        public System.Data.DataTable getPageAsDataTable(int pageNumber)
        {
            if (pageNumber < 1)
                throw new ArgumentException("Номер страницы должен быть >= 1");
            var dataTable = new System.Data.DataTable();
            String sql_exp = @"
            SELECT id, Id_position, Name, Description 
            FROM Operations
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

        //Обновляет сущетсвующую в базе запись, которая передана в экземпляре (передавать надо уже измененную)
        public Int64 update(Operation operation)
        {
            Int64 result;
            if (!operation.IsValid())
            {
                result = -1;//индекс неверных данных в обновлении
            }
            else
            {
                String sql_exp = @"
                UPDATE Operations
                SET 
                    Id_position = @IdPosition,
                    Name = @Name,
                    Description = @Description
                WHERE id = @Id";
                SqlConnection connection = new SqlConnection(_connectionString);
                SqlCommand command = new SqlCommand(sql_exp,connection);
                command.Parameters.AddWithValue("@Id", operation.Id);
                command.Parameters.AddWithValue("@IdPosition", operation.IdPosition);
                command.Parameters.AddWithValue("@Name", operation.Name);
                command.Parameters.AddWithValue("@Description", operation.Description);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
                result = 1;
            }
            return result;
        }
        
        //Выполняет запрос на удаление указанной записи по ID
        public void delete(Operation operToDel)
        {
            String sql_exp = "DELETE FROM Operations WHERE id = @Id";
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand(sql_exp, connection);
            command.Parameters.AddWithValue("@Id", operToDel.Id);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }

        //Проверяет, есть ли в базе операция, присущая определенной должности с таким же названием
        private bool checkOperInBase(Operation operation)
        {
            bool result;
            String sql_exp = @"
            SELECT * 
            FROM Positions 
            WHERE Name COLLATE Latin1_General_CS_AS =@Name AND Id_position=@IdPosition";
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand cmd = new SqlCommand(sql_exp, connection);
            cmd.Parameters.AddWithValue("@Name", operation.Name);
            cmd.Parameters.AddWithValue("@IdPosition", operation.IdPosition);
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

        //создает html-код выпадающего списка со значениями людей из базы
        public static string generateOperationsDropdown(string connectionString, string currentValue = "", string dropdownName = "positionId")
        {
            var html = new StringBuilder();
            string sql = "SELECT id, Name FROM Operations ORDER BY Name";
            var connection = new SqlConnection(connectionString);
            var command = new SqlCommand(sql, connection);
            connection.Open();
            var reader = command.ExecuteReader();
            html.AppendLine($"<select name='{dropdownName}' id='{dropdownName}' class='form-control'>");
            html.AppendLine("<option value=''>-- Выберите действие --</option>");
            while (reader.Read())
            {
                Int64 id = reader.GetInt64(0);
                string operation = reader.GetString(1);
                bool isSelected = id.ToString() == currentValue;
                html.AppendLine(
                    $"<option value='{id}' {(isSelected ? "selected" : "")}>{operation}</option>"
                );
            }
            reader.Close();
            connection.Close();
            html.AppendLine("</select>");
            return html.ToString();
        }
    }
}

