using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace Diploma.Models
{
    public class Document
    {
        private long id;
        private string name;
        private byte[] fileContent;
        private long? idTemplate; 

        public long Id { get { return id; } }
        public string Name { get { return name; } }
        public byte[] FileContent { get { return fileContent; } }
        public long? IdTemplate { get { return idTemplate; } } 

        public Document(long id, string name, byte[] fileContent, long? idTemplate)
        {
            this.id = id;
            this.name = name;
            this.fileContent = fileContent;
            this.idTemplate = idTemplate;
        }

        // Упрощенный вариант для отображения (без содержимого файла)
        public static Document forShowFromReader(SqlDataReader reader)
        {
            return new Document(
                id: reader.GetInt64(reader.GetOrdinal("id")),
                name: reader.GetString(reader.GetOrdinal("Name")),
                fileContent: null,
                idTemplate: reader.IsDBNull(reader.GetOrdinal("IdTemplate"))
                          ? (long?)null
                          : reader.GetInt64(reader.GetOrdinal("IdTemplate"))
            );
        }

        // Полная загрузка из SqlDataReader
        public static Document FromDataReader(SqlDataReader reader)
        {
            return new Document(
                id: reader.GetInt64(reader.GetOrdinal("id")),
                name: reader.GetString(reader.GetOrdinal("Name")),
                fileContent: reader.IsDBNull(reader.GetOrdinal("FileContent"))
                           ? null
                           : (byte[])reader["FileContent"],
                idTemplate: reader.IsDBNull(reader.GetOrdinal("IdTemplate"))
                          ? (long?)null
                          : reader.GetInt64(reader.GetOrdinal("IdTemplate"))
            );
        }

        // Поиск имени документа в списке по ID
        public static string FindNameInList(List<Document> list, long indexToFind)
        {
            foreach (Document document in list)
            {
                if (document.id == indexToFind)
                    return document.name;
            }
            return "Не найдено";
        }
    }
}
