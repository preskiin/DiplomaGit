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
        private long id;
        private String name;
        private byte[] content;

        public long Id { get { return id; } }
        public String Name { get { return name; } }
        public byte[] Content { get { return content; } }

        public Template(Int64 Id, String Name, byte[] Content)
        {
            id = Id;
            name = Name;
            content = Content;
        }

        public static Template forShowFromReader(SqlDataReader reader)
        {
            Template tmp = new Template(
                Id: reader.GetInt64(reader.GetOrdinal("id")),
                Name: reader.GetString(reader.GetOrdinal("Name")), 
                Content: null
            );
            return tmp;
        }
        // Создание объекта из SqlDataReader
        public static Template FromDataReader(SqlDataReader reader)
        {
            Template tmp = new Template(
                Id: reader.GetInt64(reader.GetOrdinal("id")),
                Name: reader.GetString(reader.GetOrdinal("Name")),
                Content: reader.IsDBNull(reader.GetOrdinal("FileContent"))
                       ? null
                       : (byte[])reader["FileContent"]
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
