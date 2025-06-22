using Diploma.Controllers;
using Diploma.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diploma.Views
{
    public partial class ChooseFileFromBase : Form
    {

        private DataTable _currentTable;
        private String _connection_string;
        public Template chosenTemplate;
        private CRUD_Templates _templatesCRUD;
        private Int64 selectedId;
        public ChooseFileFromBase()
        {
            InitializeComponent();
        }
        public ChooseFileFromBase(String conStr)
        {
            InitializeComponent();
            _connection_string = conStr;
        }

        private void ChooseFileFromBase_Load(object sender, EventArgs e)
        {
            _templatesCRUD = new CRUD_Templates(_connection_string);
            _currentTable = _templatesCRUD.getAllDataTable();
            dataGridView1.DataSource = _currentTable;
            dataGridView1.Columns["id"].Visible = false;
            dataGridView1.Columns["Name"].HeaderText = "Название шаблона";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            selectedId = Convert.ToInt64(dataGridView1.Rows[dataGridView1.SelectedRows[0].Index].Cells["id"].Value);
            chosenTemplate = _templatesCRUD.GetById(selectedId);
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
