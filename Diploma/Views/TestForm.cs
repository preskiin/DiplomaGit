using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Diploma.Controllers;
using Diploma.Models;
using Diploma.Views.AddForms;

namespace Diploma.Views
{
    public partial class TestForm : Form
    {
        private String _connection_string = "Data Source=PRESKIIN-NOTEBO;Initial Catalog=Diploma;Integrated Security=True;Encrypt=False;trusted_connection=True";
        private CRUD_Positions _posCRUD;
        private CRUD_Operations _operationsCRUD;
        private CRUD_Users _usersCRUD;
        private DataTable _currentTable;
        private Int32 _currentPage = 1;
        private _currentObj _usingObj;

        private enum _currentObj
        {
            users,
            operations,
            positions
        }

        public TestForm()
        {
            InitializeComponent();
        }


        private void операцииToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _usingObj = _currentObj.operations;
            if (_operationsCRUD==null)
                _operationsCRUD = new CRUD_Operations(_connection_string);
            _currentTable = _operationsCRUD.getPageAsDataTable(1);
            bindingSource1.DataSource = _currentTable;
            dataGridView1.DataSource = bindingSource1;
        }

        private void пользователиработникиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _usingObj = _currentObj.users;
            if (_usersCRUD == null)
                _usersCRUD = new CRUD_Users(_connection_string);
            _currentTable = _usersCRUD.getPageAsDataTable(1);
            bindingSource1.DataSource = _currentTable;
            dataGridView1.DataSource = bindingSource1;
        }

        private void должностиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _usingObj = _currentObj.positions;
            if (_posCRUD == null)
                _posCRUD = new CRUD_Positions(_connection_string);
            _currentTable = _posCRUD.getPageAsDataTable(1);
            bindingSource1.DataSource = _currentTable;
            dataGridView1.DataSource = bindingSource1;
        }

        //функция, которая должна подготовить форму к новым данным
        private void clearTable()
        {
            this._currentPage = 1;
            this.bindingNavigatorPositionItem.Text = Convert.ToString(this._currentPage);
        }

        private void документыToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void добавитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.DataSource == null)
            {
                MessageBox.Show("Сперва нужно выбрать таблицу для работы", "Таблица не выбрана", MessageBoxButtons.OK);
            }
            else
            {
                avokeAddForm(_usingObj);
            }
        }

        private void bindingNavigatorMoveNextItem_Click(object sender, EventArgs e)
        {
            _currentPage++;
            this.bindingNavigatorPositionItem.Text = Convert.ToString(this._currentPage);
        }

        private void bindingNavigatorMovePreviousItem_Click(object sender, EventArgs e)
        {
            _currentPage--;
            this.bindingNavigatorPositionItem.Text = Convert.ToString(this._currentPage);
        }

        private void avokeAddForm(_currentObj index)
        {
            switch ((int)index)
            {
                case 0:
                    {
                        AddFormUser addForm = new AddFormUser(_connection_string);
                        if (addForm.ShowDialog()==DialogResult.OK)
                        {

                        }
                        
                        break;
                    }
                case 1:
                    {
                        AddFormPosition addForm = new AddFormPosition();
                        break;
                    }
                case 2:
                    {
                        AddFormOperation addForm = new AddFormOperation();
                        break;
                    }
                default:
                    break;
            }
        }
    }
}
