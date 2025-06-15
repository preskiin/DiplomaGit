using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Diploma.Models;

namespace Diploma.Controllers
{
    public class CRUD_Templates
    {
        private readonly string _connectionString;
        private const int _pageSize = 50;

        public CRUD_Templates(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Получить страницу шаблонов (без Content)
        public DataTable getPageAsDataTable(int pageNumber)
        {
            var dataTable = new DataTable();
            string sql = @"
        SELECT id, Name
        FROM Templates
        ORDER BY id
        OFFSET @Offset ROWS 
        FETCH NEXT @PageSize ROWS ONLY";

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Offset", (pageNumber - 1) * _pageSize);
                command.Parameters.AddWithValue("@PageSize", _pageSize);

                connection.Open();
                new SqlDataAdapter(command).Fill(dataTable);
            }
            return dataTable;
        }

        // Получить шаблон по ID (с Content)
        public Template GetById(long id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM Templates WHERE id = @id";
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Template
                        {
                            id = reader.GetInt64(reader.GetOrdinal("id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Content = reader["Content"] as byte[]
                        };
                    }
                }
            }
            return null;
        }

        // Создать шаблон
        public long Create(Template template)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"
            INSERT INTO Templates (Name, Content)
            OUTPUT INSERTED.id
            VALUES (@Name, @Content)";

                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Name", template.Name);
                command.Parameters.AddWithValue("@Content", template.Content ?? (object)DBNull.Value);

                connection.Open();
                return (long)command.ExecuteScalar();
            }
        }

        // Обновить шаблон
        public void Update(Template template)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"
            UPDATE Templates
            SET 
                Name = @Name,
                Content = @Content
            WHERE id = @id";

                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", template.id);
                command.Parameters.AddWithValue("@Name", template.Name);
                command.Parameters.AddWithValue("@Content", template.Content ?? (object)DBNull.Value);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        // Удалить шаблон
        public void Delete(long id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "DELETE FROM Templates WHERE id = @id";
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}
