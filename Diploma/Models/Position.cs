using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diploma.Models
{
    internal class Position
    {
        private Int32 _id;
        private String _name;
        private Int32 _sector;
        private Int32 _department;
        private Int32 _level;

        public Int32 Id => _id;
        public String Name => _name;
        public Int32 Sector => _sector;
        public Int32 Department => _department;
        public Int32 Level => _level;

        // Конструктор для инициализации всех полей
        public Position(Int32 id, String name, Int32 sector, Int32 department, Int32 level)
        {
            _id = id;
            _name = name;
            _sector = sector;
            _department = department;
            _level = level;
        }
        public Position()
        {
            _id = 1003;
            _name = "Никто";
            _sector = 0;
            _department = 0;
            _level = 0;
        }

        // Создание объекта из SqlDataReader
        public static Position FromDataReader(SqlDataReader reader)
        {
            Position tmp = new Position(
                id: reader.GetInt32(reader.GetOrdinal("id")),
                name: reader.GetString(reader.GetOrdinal("Name")),
                sector: reader.GetInt32(reader.GetOrdinal("Sector")),
                department: reader.GetInt32(reader.GetOrdinal("Department")),
                level: reader.GetInt32(reader.GetOrdinal("Leve1"))
            );
            return tmp;
        }

        // Проверка валидности данных
        public Boolean IsValid()
        {
            return (!String.IsNullOrEmpty(_name)
               && _sector > 0
               && _department > 0
               && _level > 0);
        }

    }
}
