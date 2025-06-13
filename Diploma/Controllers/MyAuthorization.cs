using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Windows.Forms.VisualStyles;
using System.Windows.Forms;


namespace Diploma.Controllers
{
    internal class MyAuthorization
    {
        private String connection_string;
        
        public MyAuthorization()
        {
            connection_string= "Data Source=Preskiin-PC;Initial Catalog=Diploma;Integrated Security=True;Encrypt=False;trusted_connection=True";
        }

        //возвращает хэш, созданный из переданной строки
        public static String get_sha256(String text_to_sha256)
        {
            using SHA256 sha256 = SHA256.Create();
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(text_to_sha256));
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashBytes)
                sb.Append(b.ToString("X2"));
            return sb.ToString();
        }

        //Сверяет хэши логина и пароля из базы, с хэшем переданных логина и пароля
        public bool check_auth(String log, String pas)
        {
            CRUD_Users users = new CRUD_Users(connection_string);
            String data_pas = users.get_pas(get_sha256(log));//вытаскивает по хэшу
            if (data_pas != null)
            {
                
                 if (data_pas == get_sha256(pas))
                 {
                     return true;
                 }
                 else
                 {
                     return false;
                 }
            }
            else
                return false;
        }
    }
}
