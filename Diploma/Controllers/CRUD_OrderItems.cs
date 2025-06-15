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
    public class CRUD_OrderItems
    {
        private readonly string _connectionString;
        private const int _pageSize = 50;

        public CRUD_OrderItems(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Создание элемента заказа
        public long Create(OrderItem item)
        {
            if (!item.IsValid())
                return -1;

            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"
                INSERT INTO OrderItem (IdProduct, IdOrder, Price, Amount)
                OUTPUT INSERTED.id
                VALUES (@IdProduct, @IdOrder, @Price, @Amount)";

                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@IdProduct",
                    item.IdProduct.HasValue ? (object)item.IdProduct.Value : DBNull.Value);
                command.Parameters.AddWithValue("@IdOrder", item.IdOrder);
                command.Parameters.AddWithValue("@Price", item.Price);
                command.Parameters.AddWithValue("@Amount", item.Amount);

                connection.Open();
                return (long)command.ExecuteScalar();
            }
        }

        // Получение элемента заказа по ID
        public OrderItem Read(long id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM OrderItem WHERE id = @id";
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return OrderItem.FromDataReader(reader);
                    }
                }
            }
            return null;
        }

        // Получение всех элементов для конкретного заказа
        public List<OrderItem> GetItemsForOrder(long orderId)
        {
            var items = new List<OrderItem>();
            string sql = "SELECT * FROM OrderItem WHERE IdOrder = @IdOrder ORDER BY id";

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@IdOrder", orderId);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(OrderItem.FromDataReader(reader));
                    }
                }
            }
            return items;
        }

        // Получение страницы элементов
        public DataTable GetPageAsDataTable(int pageNumber)
        {
            var dataTable = new DataTable();
            string sql = @"
            SELECT id, IdProduct, IdOrder, Price, Amount
            FROM OrderItem
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

        // Обновление элемента заказа
        public int Update(OrderItem item)
        {
            if (!item.IsValid())
                return -1;

            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"
                UPDATE OrderItem
                SET 
                    IdProduct = @IdProduct,
                    IdOrder = @IdOrder,
                    Price = @Price,
                    Amount = @Amount
                WHERE id = @id";

                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", item.Id);
                command.Parameters.AddWithValue("@IdProduct",
                    item.IdProduct.HasValue ? (object)item.IdProduct.Value : DBNull.Value);
                command.Parameters.AddWithValue("@IdOrder", item.IdOrder);
                command.Parameters.AddWithValue("@Price", item.Price);
                command.Parameters.AddWithValue("@Amount", item.Amount);

                connection.Open();
                command.ExecuteNonQuery();
            }
            return 1;
        }

        // Удаление элемента заказа
        public void Delete(long id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "DELETE FROM OrderItem WHERE id = @id";
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        // Удаление всех элементов для конкретного заказа
        public void DeleteAllForOrder(long orderId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "DELETE FROM OrderItem WHERE IdOrder = @IdOrder";
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@IdOrder", orderId);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        // Получение общей суммы заказа
        public decimal GetOrderTotal(long orderId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT SUM(Price * Amount) FROM OrderItem WHERE IdOrder = @IdOrder";
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@IdOrder", orderId);

                connection.Open();
                var result = command.ExecuteScalar();
                return result == DBNull.Value ? 0 : Convert.ToDecimal(result);
            }
        }
    }
}
