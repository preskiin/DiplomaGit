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
        private Int64? _idCounteragent;
        private DateTime _orderDate;
        private DateTime? _deliveryDate;
        private String _comment;

        public Int64 Id { get { return _id; } }
        public Int64 Number { get { return _number; } } 
        public Int64? IdCounteragent { get { return _idCounteragent; } }
        public DateTime OrderDate { get { return _orderDate; } }
        public DateTime? DeliveryDate { get { return _deliveryDate; } }
        public String Comment { get { return _comment; } }

        // Конструктор по умолчанию
        public Order()
        {
            this._id = 0;
            this._number = 0;  
            this._idCounteragent = null;
            this._orderDate = DateTime.Now;
            this._deliveryDate = null;
            this._comment = null;
        }

        // Основной конструктор
        public Order(Int64 id, Int64 number, Int64? idCounteragent,  
                    DateTime orderDate, DateTime? deliveryDate, String comment)
        {
            this._id = id;
            this._number = number;       
            this._idCounteragent = idCounteragent;
            this._orderDate = orderDate;
            this._deliveryDate = deliveryDate;
            this._comment = comment;
        }

        // Конструктор копирования
        public Order(Order orderToCopy)
        {
            this._id = orderToCopy._id;
            this._number = orderToCopy._number;
            this._idCounteragent = orderToCopy._idCounteragent;
            this._orderDate = orderToCopy._orderDate;
            this._deliveryDate = orderToCopy._deliveryDate;
            this._comment = orderToCopy._comment;
        }

        // Создание из SqlDataReader
        public static Order FromDataReader(SqlDataReader reader)
        {
            return new Order(
                id: reader.GetInt64(reader.GetOrdinal("id")),
                number: reader.IsDBNull(reader.GetOrdinal("Number"))
                       ? 0 
                       : reader.GetInt64(reader.GetOrdinal("Number")),
                idCounteragent: reader.IsDBNull(reader.GetOrdinal("IdCounteragent"))
                              ? null
                              : reader.GetInt64(reader.GetOrdinal("IdCounteragent")),
                orderDate: reader.GetDateTime(reader.GetOrdinal("OrderDate")),
                deliveryDate: reader.IsDBNull(reader.GetOrdinal("DeliveryDate"))
                            ? null
                            : reader.GetDateTime(reader.GetOrdinal("DeliveryDate")),
                comment: reader.IsDBNull(reader.GetOrdinal("Comment"))
                       ? null
                       : reader.GetString(reader.GetOrdinal("Comment"))
            );
        }

        // Создание из DataGridViewRow
        public Order(DataGridViewRow row)
        {
            if (row != null)
            {
                _id = Convert.ToInt64(row.Cells["id"].Value);
                _number = row.Cells["Number"].Value == DBNull.Value
                         ? 0 
                         : Convert.ToInt64(row.Cells["Number"].Value);
                _idCounteragent = row.Cells["IdCounteragent"].Value == DBNull.Value
                                 ? null
                                 : Convert.ToInt64(row.Cells["IdCounteragent"].Value);
                _orderDate = Convert.ToDateTime(row.Cells["OrderDate"].Value);
                _deliveryDate = row.Cells["DeliveryDate"].Value == DBNull.Value
                               ? null
                               : Convert.ToDateTime(row.Cells["DeliveryDate"].Value);
                _comment = row.Cells["Comment"].Value == DBNull.Value
                         ? null
                         : row.Cells["Comment"].Value.ToString();
            }
        }

        // Остальные методы без изменений
        public static String findNumberInList(List<Order> list, Int64 indexToFind)
        {
            foreach (Order order in list)
            {
                if (order.Id == indexToFind)
                {
                    return Convert.ToString(order.Number);
                }
            }
            return "Не найдено";
        }

        public Boolean IsValid()
        {
            return (_id >= 0 &&
                   (_deliveryDate == null || _orderDate <= _deliveryDate) && _number>0);
        }
    }
}
