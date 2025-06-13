using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diploma.Models
{
    public class Position
    {
        private Int64 _id;
        private String _name;
        private Int64 _sector;
        private Int64 _department;
        private Int64 _level;

        public Int64 Id => _id;
        public String Name => _name;
        public Int64 Sector => _sector;
        public Int64 Department => _department;
        public Int64 Level => _level;

        // Конструктор для инициализации всех полей
        public Position(Int64 id, String name, Int64 sector, Int64 department, Int64 level)
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
                id: reader.GetInt64(reader.GetOrdinal("id")),
                name: reader.GetString(reader.GetOrdinal("Name")),
                sector: reader.GetInt64(reader.GetOrdinal("Sector")),
                department: reader.GetInt64(reader.GetOrdinal("Department")),
                level: reader.GetInt64(reader.GetOrdinal("Leve1"))
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

        //поиск названия в списке по переданному ID
        public static String findNameInList(List<Position> list, Int64 indexToFind)
        {
            foreach (var position in list)
            {
                if (position.Id == indexToFind)
                {
                    return position.Name;
                }
            }
            return "Нет";
        }

    }
}
