using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diploma.Models
{
    public class OrderItem
    {
        private Int64 _id;
        private Int64 _idProduct;
        private Int64 _idOrder;
        private decimal _price;
        private Int64 _amount;

        public Int64 Id { get { return _id; } }
        public Int64 IdProduct { get { return _idProduct; } }
        public Int64 IdOrder { get { return _idOrder; } }
        public decimal Price { get { return _price; } }
        public Int64 Amount { get { return _amount; } }

        // Вычисляемое свойство для общей стоимости позиции
        public decimal TotalPrice { get { return _price * _amount; } }

        // Конструктор по умолчанию
        public OrderItem()
        {
            this._id = 0;
            this._idProduct = 0;
            this._idOrder = 0;
            this._price = 0;
            this._amount = 1;
        }

        // Основной конструктор
        public OrderItem(Int64 id, Int64 idProduct, Int64 idOrder, decimal price, Int64 amount)
        {
            this._id = id;
            this._idProduct = idProduct;
            this._idOrder = idOrder;
            this._price = price;
            this._amount = amount;
        }

        // Конструктор копирования
        public OrderItem(OrderItem itemToCopy)
        {
            this._id = itemToCopy._id;
            this._idProduct = itemToCopy._idProduct;
            this._idOrder = itemToCopy._idOrder;
            this._price = itemToCopy._price;
            this._amount = itemToCopy._amount;
        }

        // Создание объекта из SqlDataReader
        public static OrderItem FromDataReader(SqlDataReader reader)
        {
            OrderItem tmp = new OrderItem(
                id: reader.GetInt64(reader.GetOrdinal("id")),
                idProduct: reader.GetInt64(reader.GetOrdinal("IdProduct")),
                idOrder: reader.GetInt64(reader.GetOrdinal("IdOrder")),
                price: reader.GetDecimal(reader.GetOrdinal("Price")),
                amount: reader.GetInt64(reader.GetOrdinal("Amount"))
            );
            return tmp;
        }

        // Получение элемента заказа из ряда DataGridView
        public OrderItem(DataGridViewRow row)
        {
            if (row != null)
            {
                _id = Convert.ToInt64(row.Cells["id"].Value);
                _idProduct = Convert.ToInt64(row.Cells["IdProduct"].Value);
                _idOrder = Convert.ToInt64(row.Cells["IdOrder"].Value);
                _price = Convert.ToDecimal(row.Cells["Price"].Value);
                _amount = Convert.ToInt64(row.Cells["Amount"].Value);
            }
        }

        // Проверка валидности данных
        public Boolean IsValid()
        {
            return (_id >= 0 &&
                    _idProduct > 0 &&
                    _idOrder > 0 &&
                    _price >= 0 &&
                    _amount > 0);
        }

    }
}
