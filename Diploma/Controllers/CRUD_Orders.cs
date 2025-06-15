using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diploma.Models;
using System.Data;

namespace Diploma.Controllers
{
    public class CRUD_Order
    {
        private readonly string _connectionString;
        private const int _pageSize = 50;

        public CRUD_Order(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Создание заказа (возвращает ID созданной записи или -1 при ошибке)
        public long Create(Order order)
        {
            if (!order.IsValid())
                return -1;

            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"
                INSERT INTO Orders (Number, IdCounteragent, OrderDate, DeliveryDate, Comment)
                OUTPUT INSERTED.id
                VALUES (@Number, @IdCounteragent, @OrderDate, @DeliveryDate, @Comment)";

                var command = new SqlCommand(sql, connection);

                command.Parameters.AddWithValue("@Number",
                    order.Number.HasValue ? (object)order.Number.Value : DBNull.Value);
                command.Parameters.AddWithValue("@IdCounteragent",
                    order.IdCounteragent.HasValue ? (object)order.IdCounteragent.Value : DBNull.Value);
                command.Parameters.AddWithValue("@OrderDate", order.OrderDate);
                command.Parameters.AddWithValue("@DeliveryDate",
                    order.DeliveryDate.HasValue ? (object)order.DeliveryDate.Value : DBNull.Value);
                command.Parameters.AddWithValue("@Comment",
                    string.IsNullOrEmpty(order.Comment) ? (object)DBNull.Value : order.Comment);

                connection.Open();
                return (long)command.ExecuteScalar();
            }
        }

        // Чтение заказа по ID
        public Order Read(long id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM Orders WHERE id = @id";
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return Order.FromDataReader(reader);
                    }
                }
            }
            return null;
        }

        // Получение страницы заказов
        public IEnumerable<Order> GetPage(int pageNumber)
        {
            string sql = @"
            SELECT * FROM Orders
            ORDER BY id
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
                        yield return Order.FromDataReader(reader);
                    }
                }
            }
        }

        // Получение страницы в виде DataTable
        public DataTable GetPageAsDataTable(int pageNumber)
        {
            if (pageNumber < 1)
                throw new ArgumentException("Номер страницы должен быть >= 1");

            var dataTable = new DataTable();
            string sql = @"
            SELECT 
                id, 
                Number, 
                IdCounteragent, 
                OrderDate, 
                DeliveryDate, 
                Comment
            FROM Orders
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

        // Обновление заказа
        public int Update(Order order)
        {
            if (!order.IsValid())
                return -1;

            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"
                UPDATE Orders
                SET 
                    Number = @Number,
                    IdCounteragent = @IdCounteragent,
                    OrderDate = @OrderDate,
                    DeliveryDate = @DeliveryDate,
                    Comment = @Comment
                WHERE id = @id";

                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", order.Id);
                command.Parameters.AddWithValue("@Number",
                    order.Number.HasValue ? (object)order.Number.Value : DBNull.Value);
                command.Parameters.AddWithValue("@IdCounteragent",
                    order.IdCounteragent.HasValue ? (object)order.IdCounteragent.Value : DBNull.Value);
                command.Parameters.AddWithValue("@OrderDate", order.OrderDate);
                command.Parameters.AddWithValue("@DeliveryDate",
                    order.DeliveryDate.HasValue ? (object)order.DeliveryDate.Value : DBNull.Value);
                command.Parameters.AddWithValue("@Comment",
                    string.IsNullOrEmpty(order.Comment) ? (object)DBNull.Value : order.Comment);

                connection.Open();
                command.ExecuteNonQuery();
            }
            return 1;
        }

        // Удаление заказа
        public void Delete(long id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "DELETE FROM Orders WHERE id = @id";
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        // Поиск заказов по дате (диапазон)
        public List<Order> GetOrdersByDateRange(DateTime startDate, DateTime endDate)
        {
            var orders = new List<Order>();
            string sql = @"
            SELECT * FROM Orders
            WHERE OrderDate BETWEEN @StartDate AND @EndDate
            ORDER BY OrderDate";

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@StartDate", startDate);
                command.Parameters.AddWithValue("@EndDate", endDate);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        orders.Add(Order.FromDataReader(reader));
                    }
                }
            }
            return orders;
        }

        // Получение заказов для конкретного контрагента
        public List<Order> GetOrdersByCounteragent(long counteragentId)
        {
            var orders = new List<Order>();
            string sql = @"
            SELECT * FROM Orders
            WHERE IdCounteragent = @IdCounteragent
            ORDER BY OrderDate DESC";

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@IdCounteragent", counteragentId);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        orders.Add(Order.FromDataReader(reader));
                    }
                }
            }
            return orders;
        }
    }
}
