using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diploma.Models
{
    public class Position
    {
        private Int64 _id;
        private String _name;
        private Int32 _sector;
        private Int32 _department;
        private Int32 _level;

        public Int64 Id { get { return _id; } }
        public String Name { get { return _name; } }
        public Int32 Sector { get { return _sector; } }
        public Int32 Department {  get { return _department; } }
        public Int32 Level {  get { return _level; } }

        // Конструктор для инициализации всех полей
        public Position(Int64 id, String name, Int32 sector, Int32 department, Int32 level)
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
                sector: reader.GetInt32(reader.GetOrdinal("Sector")),
                department: reader.GetInt32(reader.GetOrdinal("Department")),
                level: reader.GetInt32(reader.GetOrdinal("Leve1"))
            );
            return tmp;
        }

        public Position(DataGridViewRow row)
        {
            if (row != null)
            {
                _id = Convert.ToInt64(row.Cells["id"].Value);
                _name = row.Cells["Name"].Value.ToString();
                _sector = Convert.ToInt32(row.Cells["Sector"].Value);
                _department = Convert.ToInt32(row.Cells["Department"].Value);
                _level = Convert.ToInt32(row.Cells["Leve1"].Value);
            }
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
