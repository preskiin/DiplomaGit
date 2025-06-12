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
    public partial class EditForm : Form
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

        public EditForm()
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
            correctView(_usingObj);
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
            this.dataGridView1.DataSource = null;
            this.dataGridView1.Rows.Clear();
            this.dataGridView1.Columns.Clear();
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

        //перелистывание страниц коллекции из базы
        private void bindingNavigatorMoveNextItem_Click(object sender, EventArgs e)
        {
            _currentPage++;
            this.bindingNavigatorPositionItem.Text = Convert.ToString(this._currentPage);
            slidePage(_usingObj);
        }

        //перелистывание страниц коллекции из базы
        private void bindingNavigatorMovePreviousItem_Click(object sender, EventArgs e)
        {
            _currentPage--;
            this.bindingNavigatorPositionItem.Text = Convert.ToString(this._currentPage);
            slidePage(_usingObj);
        }

        //вызов формы для добавления записи
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
                            correctView(index);
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
        //вызов формы для редактирования записи
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
        //вызов диалогового окна для удаления записи
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
                default:
                    {
                        break;
                    }
            }
        }

        //обновление данных таблицы после перелистывания
        private void slidePage(_currentObj index)
        {
            switch ((int)index)
            {
                case 0:
                    {
                        _currentTable = _usersCRUD.getPageAsDataTable(_currentPage);
                        dataGridView1.DataSource = _currentTable;
                        break;
                    }
                case 1:
                    {
                        _currentTable = _operationsCRUD.getPageAsDataTable(_currentPage);
                        dataGridView1.DataSource = _currentTable;
                        break;
                    }
                case 2:
                    {
                        _currentTable = _posCRUD.getPageAsDataTable(_currentPage);
                        dataGridView1.DataSource = _currentTable;
                        break;
                    }
                default:
                    break;
            }
        
        }

        private void correctView(_currentObj index)
        {
            switch ((int)index)
            {
                case 0:
                    {
                        dataGridView1.Columns["id"].Visible = false;
                        dataGridView1.Columns["Id_position"].Visible = false;
                        DataGridViewColumn positionColumn = new DataGridViewTextBoxColumn();
                        positionColumn.Name = "PositionName";
                        positionColumn.HeaderText = "Должность";
                        positionColumn.DisplayIndex = 2;
                        dataGridView1.Columns.Add(positionColumn);
                        List<Position> list = CRUD_Positions.getAllPositions(_connection_string);
                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            row.Cells["PositionName"].Value = CRUD_Positions.findNameInList(list, Convert.ToInt32(row.Cells["Id_position"].Value));
                        }
                        break;
                    }
            }
        }
    }
}
