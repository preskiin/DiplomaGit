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
            Int32 tmp;
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
                tmp = (Int32)command.ExecuteScalar();// Возвращает ID новой записи
                connection.Close();
            }
            return tmp;
        }

        //Получает записи с пагинацией
        public List<Position> getPage(Int32 pageNumber)
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
        public Int32 update(Position position)
        {
            Int32 result;
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
        public static string generatePositionsDropdown(string connectionString, string currentValue = "", string dropdownName = "positionId")
        {
            var html = new StringBuilder();
            string sql = "SELECT id, Name FROM Positions ORDER BY Name";
            var connection = new SqlConnection(connectionString);
            var command = new SqlCommand(sql, connection);
            connection.Open();
            var reader = command.ExecuteReader();
            html.AppendLine($"<select name='{dropdownName}' id='{dropdownName}' class='form-control'>");
            html.AppendLine("<option value=''>-- Выберите должность --</option>");
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                string name = reader.GetString(1);
                bool isSelected = id.ToString() == currentValue;
                html.AppendLine(
                    $"<option value='{id}' {(isSelected ? "selected" : "")}>{name}</option>"
                );
            }
            reader.Close();
            connection.Close();
            html.AppendLine("</select>");
            return html.ToString();
        }

        public static String findNameInList(List<Position> list, Int32 indexToFind)
        {
            foreach (var position in list)
            {
                if (position.Id == indexToFind)
                {
                    return position.Name;
                }
            }
            return "Нет";
        }
    }
}
