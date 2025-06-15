using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diploma.Models;

namespace Diploma.Controllers
{
    public class CRUD_Documents
    {
        private readonly string _connectionString;
        private const int _pageSize = 50;

        public CRUD_Documents(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Получить страницу документов (без FileContent)
        public DataTable getPageAsDataTable(int pageNumber)
        {
            var dataTable = new DataTable();
            string sql = @"
        SELECT id, Name, IdTemplate
        FROM Documents
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

        // Получить документ по ID (с FileContent)
        public Document GetById(long id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM Documents WHERE id = @id";
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Document
                        {
                            id = reader.GetInt64(reader.GetOrdinal("id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            FileContent = reader["FileContent"] as byte[],
                            IdTemplate = reader.GetInt64(reader.GetOrdinal("IdTemplate"))
                        };
                    }
                }
            }
            return null;
        }

        // Создать документ
        public long Create(Document document)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"
            INSERT INTO Documents (Name, FileContent, IdTemplate)
            OUTPUT INSERTED.id
            VALUES (@Name, @FileContent, @IdTemplate)";

                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Name", document.Name);
                command.Parameters.AddWithValue("@FileContent", document.FileContent ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@IdTemplate", document.IdTemplate);

                connection.Open();
                return (long)command.ExecuteScalar();
            }
        }

        // Обновить документ
        public void Update(Document document)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"
            UPDATE Documents
            SET 
                Name = @Name,
                FileContent = @FileContent,
                IdTemplate = @IdTemplate
            WHERE id = @id";

                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", document.id);
                command.Parameters.AddWithValue("@Name", document.Name);
                command.Parameters.AddWithValue("@FileContent", document.FileContent ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@IdTemplate", document.IdTemplate);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        // Удалить документ
        public void Delete(long id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "DELETE FROM Documents WHERE id = @id";
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }

}
