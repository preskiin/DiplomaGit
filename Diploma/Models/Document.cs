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
        public long id { get; set; }          // Первичный ключ (маленькие буквы)
        public string Name { get; set; }
        public byte[] FileContent { get; set; }
        public long IdTemplate { get; set; }  // Внешний ключ (большая буква)
    }

}
