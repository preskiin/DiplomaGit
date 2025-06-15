using DocumentFormat.OpenXml.Bibliography;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Diploma.Models
{
    public class Template
    {
        public long id;
        public string name;
        public byte[] content;

        //public Int64 Id { get { return _id; } }
        //public String Name { get { return _name; } }
        //public byte[] Content { get { return _content; } }

        public Template(Int64 Id, String Name, byte[] Content)
        {
            id = Id;
            name = Name;
            content = Content;
        }
        // Создание объекта из SqlDataReader
        public static Template FromDataReader(SqlDataReader reader)
        {
            Template tmp = new Template(
                Id: reader.GetInt64(reader.GetOrdinal("id")),
                Name: reader.GetString(reader.GetOrdinal("Name")),
                Content: reader.IsDBNull(reader.GetOrdinal("Content"))
                       ? null
                       : (byte[])reader["Content"]
            );
            return tmp;
        }
        public static String findNameInList(List<Template> list, Int64 indexToFind)
        {
            foreach (Template template in list)
            {
                if (template.id == indexToFind)
                    return template.name;
            }
            return "Не найдено";
        }
    }
}
