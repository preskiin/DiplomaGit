using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diploma.Controllers
{
    public class MyAppContext : ApplicationContext
    {
        public void SwitchMainForm(Form newForm)
        {
            Form oldMain = this.MainForm;
            this.MainForm = newForm;
            newForm.Show();
            oldMain?.Close();
        }
    }
}
