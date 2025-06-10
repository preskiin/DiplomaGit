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
        private CRUD_Positions _posCRUD;
        private CRUD_Operations _operationsCRUD;
        private CRUD_Users _usersCRUD;
        
        private DataTable _currentTable;
        private Int32 _currentPage = 1;
        public TestForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //clearTable();
            
        }

        private void операцииToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //clearTable();
            _operationsCRUD = new CRUD_Operations(_connection_string);
            _currentTable = _operationsCRUD.getPageAsDataTable(1);
            bindingSource1.DataSource = _currentTable;
            dataGridView1.DataSource = bindingSource1;
        }

        private void пользователиработникиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //clearTable();
            _usersCRUD = new CRUD_Users(_connection_string);
            _currentTable = _usersCRUD.getPageAsDataTable(1);
            bindingSource1.DataSource = _currentTable;
            dataGridView1.DataSource = bindingSource1;
        }

        private void должностиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //clearTable();
            _posCRUD = new CRUD_Positions(_connection_string);
            _currentTable = _posCRUD.getPageAsDataTable(1);
            bindingSource1.DataSource = _currentTable;
            dataGridView1.DataSource = bindingSource1;
        }

        //функция, которая должна подготовить форму к новым данным
        private void clearTable()
        {
            this._currentPage = 1;
        }

        private void документыToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
