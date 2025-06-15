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
    public class CRUD_Products
    {
        private string _connectionString;
        private int _pageSize = 50;

        public CRUD_Products(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Создание продукта (возвращает ID созданной записи или -1 при ошибке)
        public long create(Product product)
        {
            if (!product.IsValid())
                return -1;

            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"
                INSERT INTO Products (Name, Description, Price)
                OUTPUT INSERTED.id
                VALUES (@Name, @Description, @Price)";

                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Name", product.Name);
                command.Parameters.AddWithValue("@Description",
                    string.IsNullOrEmpty(product.Description) ? (object)DBNull.Value : product.Description);
                command.Parameters.AddWithValue("@Price", product.Price);

                connection.Open();
                return (long)command.ExecuteScalar();
            }
        }

        // Чтение продукта по ID
        public Product read(long id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM Products WHERE id = @id";
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return Product.FromDataReader(reader);
                    }
                }
            }
            return null;
        }

        // Получение страницы продуктов
        public IEnumerable<Product> GetPage(int pageNumber)
        {
            string sql = @"
            SELECT * FROM Products
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
                        yield return Product.FromDataReader(reader);
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
            SELECT id, Name, Description, Price
            FROM Products
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

        // Обновление продукта
        public int update(Product product)
        {
            if (!product.IsValid())
                return -1;

            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"
                UPDATE Products
                SET 
                    Name = @Name,
                    Description = @Description,
                    Price = @Price
                WHERE id = @id";

                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", product.Id);
                command.Parameters.AddWithValue("@Name", product.Name);
                command.Parameters.AddWithValue("@Description",
                    string.IsNullOrEmpty(product.Description) ? (object)DBNull.Value : product.Description);
                command.Parameters.AddWithValue("@Price", product.Price);

                connection.Open();
                command.ExecuteNonQuery();
            }
            return 1;
        }

        // Удаление продукта
        public void delete(long id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "DELETE FROM Products WHERE id = @id";
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        // Поиск продуктов по названию
        public List<Product> SearchByName(string searchTerm)
        {
            var products = new List<Product>();
            string sql = @"
            SELECT * FROM Products
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
                        products.Add(Product.FromDataReader(reader));
                    }
                }
            }
            return products;
        }

        public static List<Product> getAllProducts(String connection)
        {
            List<Product> products = new();
            String sql_exp = @"
                SELECT * FROM Products
                ORDER BY Name ASC";
            SqlConnection con = new(connection);
            SqlCommand command = new SqlCommand(sql_exp, con);
            con.Open();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                products.Add(Product.FromDataReader(reader));
            }
            reader.Close();
            con.Close();
            return products;
        }
    }

}
