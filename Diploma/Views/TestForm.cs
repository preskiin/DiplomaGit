using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Diploma.Controllers;
using Diploma.Models;

namespace Diploma.Views
{
    public partial class TestForm : Form
    {
        private String _connection_string = "Data Source=Preskiin-PC;Initial Catalog=Diploma;Integrated Security=True;Encrypt=False;trusted_connection=True";
        private CRUD_Positions posCRUD;
        private DataTable positions;
        public TestForm()
        {
            InitializeComponent();
            posCRUD = new CRUD_Positions(_connection_string);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            positions = posCRUD.getPageAsDataTable(1);
            bindingSource1.DataSource = positions;
            dataGridView1.DataSource = bindingSource1;
        }
    }
}
