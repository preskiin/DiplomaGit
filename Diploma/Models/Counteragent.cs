using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diploma.Models
{
    public class Counteragent
    {
        private Int64 _id;
        private String _name;

        public Int64 Id { get { return _id; } }
        public String Name { get { return _name; } }

        // Конструктор по умолчанию
        public Counteragent()
        {
            this._id = 0;
            this._name = "Не указано";
        }

        // Основной конструктор
        public Counteragent(Int64 id, String name)
        {
            this._id = id;
            this._name = name;
        }

        // Конструктор копирования
        public Counteragent(Counteragent agentToCopy)
        {
            this._id = agentToCopy._id;
            this._name = agentToCopy._name;
        }

        // Создание объекта из SqlDataReader
        public static Counteragent FromDataReader(SqlDataReader reader)
        {
            Counteragent tmp = new Counteragent(
                id: reader.GetInt64(reader.GetOrdinal("id")),
                name: reader.GetString(reader.GetOrdinal("Name"))
            );
            return tmp;
        }

        // Получение контрагента из ряда DataGridView
        public Counteragent(DataGridViewRow row)
        {
            if (row != null)
            {
                _id = Convert.ToInt64(row.Cells["id"].Value);
                _name = row.Cells["Name"].Value.ToString();
            }
        }

        // Проверка валидности данных
        public Boolean IsValid()
        {
            return (!String.IsNullOrEmpty(_name) && _id > 0);
        }
    }
}
