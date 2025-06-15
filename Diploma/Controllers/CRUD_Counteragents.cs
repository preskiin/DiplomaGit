using Diploma.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
namespace Diploma.Controllers
{
    public class CRUD_Counteragents
    {
        private readonly string _connectionString;
        private const int _pageSize = 50;

        public CRUD_Counteragents(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Создание контрагента (возвращает ID созданной записи или -1 при ошибке)
        public long Create(Counteragent agent)
        {
            if (!agent.IsValid())
                return -1;

            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"
                INSERT INTO Counteragents (Name)
                OUTPUT INSERTED.id
                VALUES (@Name)";

                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Name", agent.Name);

                connection.Open();
                return (long)command.ExecuteScalar();
            }
        }

        // Чтение контрагента по ID
        public Counteragent Read(long id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM Counteragents WHERE id = @id";
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return Counteragent.FromDataReader(reader);
                    }
                }
            }
            return null;
        }

        // Получение страницы контрагентов
        public IEnumerable<Counteragent> GetPage(int pageNumber)
        {
            string sql = @"
            SELECT * FROM Counteragents
            ORDER BY Name
            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY";

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Offset", (pageNumber - 1) * _pageSize);
                command.Parameters.AddWithValue("@PageSize", _pageSize);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return Counteragent.FromDataReader(reader);
                    }
                }
            }
        }

        // Получение страницы в виде DataTable
        public DataTable getPageAsDataTable(int pageNumber)
        {
            if (pageNumber < 1)
                throw new ArgumentException("Номер страницы должен быть >= 1");

            var dataTable = new DataTable();
            string sql = @"
            SELECT id, Name
            FROM Counteragents
            ORDER BY Name
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

        // Обновление контрагента
        public int Update(Counteragent agent)
        {
            if (!agent.IsValid())
                return -1;

            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"
                UPDATE Counteragents
                SET 
                    Name = @Name
                WHERE id = @id";

                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", agent.Id);
                command.Parameters.AddWithValue("@Name", agent.Name);

                connection.Open();
                command.ExecuteNonQuery();
            }
            return 1;
        }

        // Удаление контрагента
        public void Delete(long id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "DELETE FROM Counteragents WHERE id = @id";
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        // Поиск контрагентов по имени
        public List<Counteragent> SearchByName(string searchTerm)
        {
            var agents = new List<Counteragent>();
            string sql = @"
            SELECT * FROM Counteragents
            WHERE Name LIKE @SearchTerm
            ORDER BY Name";

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        agents.Add(Counteragent.FromDataReader(reader));
                    }
                }
            }
            return agents;
        }

        // Получение всех контрагентов
        public List<Counteragent> GetAll()
        {
            var agents = new List<Counteragent>();
            string sql = "SELECT * FROM Counteragents ORDER BY Name";

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(sql, connection);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        agents.Add(Counteragent.FromDataReader(reader));
                    }
                }
            }
            return agents;
        }
    }
}
