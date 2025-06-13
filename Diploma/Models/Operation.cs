using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diploma.Models
{
    internal class Operation
    {
        private readonly Int64 _id;
        private readonly Int64 _idPosition;
        private readonly String _name;
        private readonly String _description;

        public Int64 Id => _id;
        public Int64 IdPosition => _idPosition;
        public String Name => _name;
        public String Description => _description;

        // Конструктор со всеми полями
        public Operation(Int64 id, Int64 idPosition, String name, String description)
        {
            _id = id;
            _idPosition = idPosition;
            _name = name;
            _description = description;
        }

        public Operation()
        {
            _id = (Int64)0;
            _idPosition = (Int64)1003;
            _name = "Бездельничать";
            _description = "Просто ничего не делать";
        }
        // Создает объект Operation из SqlDataReader
        public static Operation FromDataReader(SqlDataReader reader)
        {
            Int64 id = reader.GetInt64(reader.GetOrdinal("id"));
            Int64 idPosition = reader.GetInt64(reader.GetOrdinal("Id_position"));
            String name = reader.GetString(reader.GetOrdinal("Name"));
            String description = reader.GetString(reader.GetOrdinal("Description"));
            return new Operation(id, idPosition, name, description);
        }

        // Проверяет валидность данных
        public Boolean IsValid()
        {
            return (!String.IsNullOrWhiteSpace(_name) && _idPosition > 0); // Id_position должен ссылаться на существующую запись
        }
    }
}
