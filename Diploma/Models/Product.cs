using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diploma.Models
{
    public class Product
    {
        private Int64 _id;
        private String _name;
        private String _description;
        private decimal _price;

        public Int64 Id { get { return _id; } }
        public String Name { get { return _name; } }
        public String Description { get { return _description; } }
        public decimal Price { get { return _price; } }

        // Конструктор по умолчанию
        public Product()
        {
            this._id = 0;
            this._name = "Новый продукт";
            this._description = "Описание отсутствует";
            this._price = 0;
        }

        // Основной конструктор
        public Product(Int64 id, String name, String description, decimal price)
        {
            this._id = id;
            this._name = name;
            this._description = description;
            this._price = price;
        }

        // Конструктор копирования
        public Product(Product productToCopy)
        {
            this._id = productToCopy._id;
            this._name = productToCopy._name;
            this._description = productToCopy._description;
            this._price = productToCopy._price;
        }

        // Создание объекта из SqlDataReader
        public static Product FromDataReader(SqlDataReader reader)
        {
            Product tmp = new Product(
                id: reader.GetInt64(reader.GetOrdinal("id")),
                name: reader.GetString(reader.GetOrdinal("Name")),
                description: reader.IsDBNull(reader.GetOrdinal("Description")) ?
                           string.Empty : reader.GetString(reader.GetOrdinal("Description")),
                price: reader.GetDecimal(reader.GetOrdinal("Price"))
            );
            return tmp;
        }

        // Получение продукта из ряда DataGridView
        public Product(DataGridViewRow row)
        {
            if (row != null)
            {
                _id = Convert.ToInt64(row.Cells["id"].Value);
                _name = row.Cells["Name"].Value.ToString();
                _description = row.Cells["Description"].Value?.ToString() ?? string.Empty;
                _price = Convert.ToDecimal(row.Cells["Price"].Value);
            }
        }

        // Проверка валидности данных
        public Boolean IsValid()
        {
            return (_id >= 0 &&
                    !String.IsNullOrWhiteSpace(_name) &&
                    _price >= 0);
        }
    }
}
