using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diploma.Models
{
    public class Order
    {
        private Int64 _id;
        private Int64 _number;
        private Int64 _idCounteragent;
        private DateTime _orderDate;
        private DateTime _deliveryDate;
        private String _comment;

        public Int64 Id { get { return _id; } }
        public Int64 Number { get { return _number; } }
        public Int64 IdCounteragent { get { return _idCounteragent; } }
        public DateTime OrderDate { get { return _orderDate; } }
        public DateTime DeliveryDate { get { return _deliveryDate; } }
        public String Comment { get { return _comment; } }

        // Конструктор по умолчанию
        public Order()
        {
            this._id = 0;
            this._number = 0;
            this._idCounteragent = 0;
            this._orderDate = DateTime.Now;
            this._deliveryDate = DateTime.Now.AddDays(1);
            this._comment = "Нет комментария";
        }

        // Основной конструктор (обновлён)
        public Order(Int64 id, Int64 number, Int64 idCounteragent,
                    DateTime orderDate, DateTime deliveryDate, String comment)
        {
            this._id = id;
            this._number = number;
            this._idCounteragent = idCounteragent;
            this._orderDate = orderDate;
            this._deliveryDate = deliveryDate;
            this._comment = comment;
        }

        // Конструктор копирования (обновлён)
        public Order(Order orderToCopy)
        {
            this._id = orderToCopy._id;
            this._number = orderToCopy._number;
            this._idCounteragent = orderToCopy._idCounteragent;
            this._orderDate = orderToCopy._orderDate;
            this._deliveryDate = orderToCopy._deliveryDate;
            this._comment = orderToCopy._comment;
        }

        // Создание объекта из SqlDataReader (обновлён)
        public static Order FromDataReader(SqlDataReader reader)
        {
            Order tmp = new Order(
                id: reader.GetInt64(reader.GetOrdinal("id")),
                number: reader.GetInt64(reader.GetOrdinal("Number")),
                idCounteragent: reader.GetInt64(reader.GetOrdinal("IdCounteragent")),
                orderDate: reader.GetDateTime(reader.GetOrdinal("OrderDate")),
                deliveryDate: reader.GetDateTime(reader.GetOrdinal("DeliveryDate")),
                comment: reader.IsDBNull(reader.GetOrdinal("Comment")) ?
                        string.Empty : reader.GetString(reader.GetOrdinal("Comment"))
            );
            return tmp;
        }

        // Получение заказа из ряда DataGridView (обновлён)
        public Order(DataGridViewRow row)
        {
            if (row != null)
            {
                _id = Convert.ToInt64(row.Cells["id"].Value);
                _number = Convert.ToInt64(row.Cells["Number"].Value);
                _idCounteragent = Convert.ToInt64(row.Cells["IdCounteragent"].Value);
                _orderDate = Convert.ToDateTime(row.Cells["OrderDate"].Value);
                _deliveryDate = Convert.ToDateTime(row.Cells["DeliveryDate"].Value);
                _comment = row.Cells["Comment"].Value?.ToString() ?? string.Empty;
            }
        }

        // Проверка валидности данных (обновлён)
        public Boolean IsValid()
        {
            return (_id >= 0 &&
                    _number > 0 &&  // Номер заказа должен быть положительным
                    _idCounteragent > 0 &&
                    _orderDate <= _deliveryDate);

        }
    }
}
