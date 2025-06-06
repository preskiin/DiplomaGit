using Diploma.Models;
using System;
using System.Collections.Generic;
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
        private Int32 _pageSize = 50;
        public CRUD_Positions(String connectionString)
        {
            _connectionString = connectionString /*?? throw new ArgumentNullException(nameof(connectionString))*/;
        }

        // Создает новую запись в таблице Positions.
        public Int32 create(Position position)
        {
            if (!position.IsValid())
                throw new ArgumentException("Некорректные данные позиции.");

            String sql_exp = @"
            INSERT INTO Positions (Name, Sector, Department, Leve1)
            OUTPUT INSERTED.id
            VALUES (@Name, @Sector, @Department, @Level)";

            SqlConnection connection = new SqlConnection(_connectionString);
            using (var command = new SqlCommand(sql_exp, connection))
            {
                command.Parameters.AddWithValue("@Name", position.Name);
                command.Parameters.AddWithValue("@Sector", position.Sector);
                command.Parameters.AddWithValue("@Department", position.Department);
                command.Parameters.AddWithValue("@Level", position.Level);
                connection.Open();
                Int32 tmp = (Int32)command.ExecuteScalar();
                connection.Close();
                return tmp; // Возвращает ID новой записи
            }
        }

        //Получает записи с пагинацией
        public IEnumerable<Position> getPage(Int32 pageNumber)
        {
            if (pageNumber < 1)
                throw new ArgumentException("Номер страницы должен быть >= 1");

            List<Position> positions = new List<Position>();
            String sql_exp = @"
            SELECT * FROM Positions
            ORDER BY id
            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY";
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand(sql_exp, connection);
            Int32 offset = (pageNumber - 1) * _pageSize;
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
        public System.Data.DataTable getPageAsDataTable(Int32 pageNumber)
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
        public Position read(Int32 id)
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
        public void update(Position position)
        {
            if (!position.IsValid())
                throw new ArgumentException("Некорректные данные позиции.");

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
        }

        //Удаляет позицию по ID.
        public void delete(Int32 id)
        {
            const String sql = "DELETE FROM Positions WHERE id = @Id";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}
