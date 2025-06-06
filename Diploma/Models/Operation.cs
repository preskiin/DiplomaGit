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
        private readonly Int32 _id;
        private readonly Int32 _idPosition;
        private readonly String _name;
        private readonly String _description;

        public Int32 Id => _id;
        public Int32 IdPosition => _idPosition;
        public String Name => _name;
        public String Description => _description;

        // Конструктор со всеми полями
        public Operation(Int32 id, Int32 idPosition, String name, String description)
        {
            _id = id;
            _idPosition = idPosition;
            _name = name;
            _description = description;
        }

        public Operation()
        {
            _id = (Int32)0;
            _idPosition = (Int32)1003;
            _name = "Бездельничать";
            _description = "Просто ничего не делать";
        }
        // Создает объект Operation из SqlDataReader
        public static Operation FromDataReader(SqlDataReader reader)
        {
            Int32 id = reader.GetInt32(reader.GetOrdinal("id"));
            Int32 idPosition = reader.GetInt32(reader.GetOrdinal("Id_position"));
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
