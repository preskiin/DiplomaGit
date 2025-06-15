using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diploma.Controllers
{
    using global::Diploma.Models;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Windows.Forms;

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
            public long Create(Product product)
            {
                if (!product.IsValid())
                    return -1;

                using (var connection = new SqlConnection(_connectionString))
                {
                    string sql = @"
                INSERT INTO Product (Name, Description, Price)
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
            public Product Read(long id)
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string sql = "SELECT * FROM Product WHERE id = @id";
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
            SELECT * FROM Product
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
            public DataTable GetPageAsDataTable(int pageNumber)
            {
                if (pageNumber < 1)
                    throw new ArgumentException("Номер страницы должен быть >= 1");

                var dataTable = new DataTable();
                string sql = @"
            SELECT id, Name, Description, Price
            FROM Product
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
            public int Update(Product product)
            {
                if (!product.IsValid())
                    return -1;

                using (var connection = new SqlConnection(_connectionString))
                {
                    string sql = @"
                UPDATE Product
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
            public void Delete(long id)
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string sql = "DELETE FROM Product WHERE id = @id";
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
            SELECT * FROM Product
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
        }
    }
}
