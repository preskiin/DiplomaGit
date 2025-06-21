using Diploma.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace Diploma
{
    
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            String connectionToDb = "Data Source=Preskiin-PC;Initial Catalog=Diploma;Integrated Security=True;Encrypt=False;trusted_connection=True";
            Diploma.Controllers.MyAppContext context = new Diploma.Controllers.MyAppContext();
            context.MainForm = new Authorize(context, connectionToDb); // Первая главная форма

            //Application.Run(new TemplateForm());
            Application.Run(new TemplateForm());
        }
    }
}
