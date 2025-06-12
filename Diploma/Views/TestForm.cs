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
//using DocumentFormat.OpenXml.Drawing.Charts;

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
            clearTable();
            _usingObj = _currentObj.operations;
            if (_operationsCRUD==null)
                _operationsCRUD = new CRUD_Operations(_connection_string);
            _currentTable = _operationsCRUD.getPageAsDataTable(1);
            dataGridView1.DataSource = _currentTable;
            //bindingSource1.DataSource = _currentTable;
            //dataGridView1.DataSource = bindingSource1;
        }

        private void пользователиработникиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clearTable();
            _usingObj = _currentObj.users;
            if (_usersCRUD == null)
                _usersCRUD = new CRUD_Users(_connection_string);
            _currentTable = _usersCRUD.getPageAsDataTable(1);
            dataGridView1.DataSource = _currentTable;
            //bindingSource1.DataSource = _currentTable;
            //dataGridView1.DataSource = bindingSource1;
        }

        private void должностиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clearTable();
            _usingObj = _currentObj.positions;
            if (_posCRUD == null)
                _posCRUD = new CRUD_Positions(_connection_string);
            _currentTable = _posCRUD.getPageAsDataTable(1);
            dataGridView1.DataSource = _currentTable;
            //bindingSource1.DataSource = _currentTable;
            //dataGridView1.DataSource = bindingSource1;
        }

        //функция, которая должна подготовить форму к новым данным
        private void clearTable()
        {
            this._currentPage = 1;
            this.bindingNavigatorPositionItem.Text = Convert.ToString(this._currentPage);
        }

        private void документыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clearTable();

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


        private void изменитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.DataSource == null)
            {
                MessageBox.Show("Сперва нужно выбрать таблицу для работы", "Таблица не выбрана", MessageBoxButtons.OK);
            }
            else
            {
                avokeEditForm(_usingObj);
            }
        }

        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.DataSource == null)
            {
                MessageBox.Show("Сперва нужно выбрать таблицу для работы", "Таблица не выбрана", MessageBoxButtons.OK);
            }
            else
            {
                avokeDeleteForm(_usingObj);
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            //chosenIdTable = dataGridView1.Rows.
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
            Int32 result = 0;
            switch ((int)index)
            {
                case 0:
                    {
                        User user;
                        FormUser addForm = new FormUser(_connection_string);
                        if (addForm.ShowDialog()==DialogResult.OK)
                        {
                            user = addForm.userTmp;
                            result = _usersCRUD.create(user);
                            _currentTable = _usersCRUD.getPageAsDataTable(_currentPage);
                            dataGridView1.DataSource = _currentTable;
                        }
                        break;
                    }
                case 1:
                    {
                        FormPosition addForm = new FormPosition();
                        break;
                    }
                case 2:
                    {
                        FormOperation addForm = new FormOperation();
                        break;
                    }
                default:
                    break;
            }
        }

        private void avokeEditForm(_currentObj index)
        {
            Int32 result = 0;
            switch ((int)index)
            {
                case 0:
                    {
                        User user = new User(dataGridView1.Rows[dataGridView1.SelectedRows[0].Index]);
                        FormUser editForm = new FormUser(_connection_string, user);
                        if (editForm.ShowDialog() == DialogResult.OK)
                        {
                            user = editForm.userTmp;
                            result = _usersCRUD.update(user);
                            _currentTable = _usersCRUD.getPageAsDataTable(_currentPage);
                            dataGridView1.DataSource = _currentTable;
                        }
                        break;
                    }
                case 1:
                    {
                        FormPosition editForm = new FormPosition();
                        break;
                    }
                case 2:
                    {
                        FormOperation editForm = new FormOperation();
                        break;
                    }
                default:
                    break;
            }
        }
        private void avokeDeleteForm(_currentObj index)
        {
            switch ((int)index)
            {
                case 0:
                    {
                        DialogResult result = MessageBox.Show("Вы уверены, что хотите удалить выбранного пользователя?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            _usersCRUD.delete(new User(dataGridView1.Rows[dataGridView1.SelectedRows[0].Index]));
                            _currentTable = _usersCRUD.getPageAsDataTable(_currentPage);
                            dataGridView1.DataSource = _currentTable;
                        }
                        break;
                    }
            }
        }
    }
}
