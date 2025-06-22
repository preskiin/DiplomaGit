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

        public DataTable getAllDataTable()
        {
            var dataTable = new DataTable();
            string sql = @"
                SELECT id, Name
                FROM Templates";

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(sql, connection);
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
                        (
                            Id: reader.GetInt64(reader.GetOrdinal("id")),
                            Name: reader.GetString(reader.GetOrdinal("Name")),
                            Content: reader["FileContent"] as byte[]
                        );
                    }
                }
            }
            return null;
        }

        // Создать шаблон
        public long create(Template template)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"
            INSERT INTO Templates (Name, FileContent)
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
        public void update(Template template)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"
            UPDATE Templates
            SET 
                Name = @Name,
                FileContent = @Content
            WHERE id = @id";

                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", template.Id);
                command.Parameters.AddWithValue("@Name", template.Name);
                command.Parameters.AddWithValue("@Content", template.Content ?? (object)DBNull.Value);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        // Удалить шаблон
        public void delete(long id)
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

        public static List<Template> getAllNamesTemplates(string _connection)
        {
            List<Template> templates = new List<Template>();
            String sql_exp = @"
            SELECT id, Name FROM Templates
            ORDER BY Name ASC";
            SqlConnection con = new SqlConnection(_connection);
            SqlCommand command = new SqlCommand(sql_exp, con);
            con.Open();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                templates.Add(Template.forShowFromReader(reader));
            }
            reader.Close();
            con.Close();
            return templates;
        }
        

    }
}
