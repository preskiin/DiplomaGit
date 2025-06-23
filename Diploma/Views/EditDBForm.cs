using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Printing;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Diploma.Controllers;
using Diploma.Models;
using Diploma.Views.AddForms;
using Diploma.Views.ClassForms;
namespace Diploma.Views
{
    public partial class EditDBForm : Form
    {
        private Diploma.Controllers.MyAppContext localContext;
        private Diploma.Models.User enteredUser;
        private String _connection_string;
        private CRUD_Positions _positionsCRUD;
        private CRUD_Operations _operationsCRUD;
        private CRUD_Users _usersCRUD;
        private CRUD_Counteragents _counteragentsCRUD;
        private CRUD_Documents _documentsCRUD;
        private CRUD_Templates _templatesCRUD;
        private CRUD_Orders _ordersCRUD;
        private CRUD_OrderItems _orderItemsCRUD;
        private CRUD_Products _productsCRUD;
        private DataTable _currentTable;
        private Int32 _currentPage = 1;
        private _currentObj _usingObj;

        private enum _currentObj
        {
            users,
            operations,
            positions,
            agents,
            orders,
            orderItems,
            products,
            documents,
            templates,
        }

        public EditDBForm()
        {
            InitializeComponent();
        }
        public EditDBForm(Diploma.Controllers.MyAppContext context, Diploma.Models.User user, String connection)
        {
            InitializeComponent();
            localContext = context;
            enteredUser = user;
            _connection_string = connection;
        }

        private void операцииToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clearTable();
            _usingObj = _currentObj.operations;
            if (_operationsCRUD==null)
                _operationsCRUD = new CRUD_Operations(_connection_string);
            _currentTable = _operationsCRUD.getPageAsDataTable(1);
            dataGridView1.DataSource = _currentTable;
            correctView(_usingObj);
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
            if (_positionsCRUD == null)
                _positionsCRUD = new CRUD_Positions(_connection_string);
            _currentTable = _positionsCRUD.getPageAsDataTable(1);
            dataGridView1.DataSource = _currentTable;
            correctView(_usingObj);
            //bindingSource1.DataSource = _currentTable;
            //dataGridView1.DataSource = bindingSource1;
        }

        private void контрагентыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clearTable();
            _usingObj = _currentObj.agents;
            if (_counteragentsCRUD == null)
                _counteragentsCRUD = new CRUD_Counteragents(_connection_string);
            _currentTable = _counteragentsCRUD.getPageAsDataTable(1);
            dataGridView1.DataSource = _currentTable;
            correctView(_usingObj);
        }

        private void шаблоныToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clearTable();
            _usingObj = _currentObj.templates;
            if (_templatesCRUD == null)
                _templatesCRUD = new CRUD_Templates(_connection_string);
            _currentTable = _templatesCRUD.getPageAsDataTable(1);
            dataGridView1.DataSource = _currentTable;
            correctView(_usingObj);
        }

        private void заказыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clearTable();
            _usingObj = _currentObj.orders;
            if (_ordersCRUD == null)
                _ordersCRUD = new CRUD_Orders(_connection_string);
            _currentTable = _ordersCRUD.getPageAsDataTable(1);
            dataGridView1.DataSource = _currentTable;
            correctView(_usingObj);
        }

        private void деталиЗаказовToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clearTable();
            _usingObj = _currentObj.orderItems;
            if (_orderItemsCRUD == null)
                _orderItemsCRUD = new CRUD_OrderItems(_connection_string);
            _currentTable = _orderItemsCRUD.getPageAsDataTable(1);
            dataGridView1.DataSource = _currentTable;
            correctView(_usingObj);
        }

        private void товарыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clearTable();
            _usingObj = _currentObj.products;
            if (_productsCRUD == null)
                _productsCRUD = new CRUD_Products(_connection_string);
            _currentTable = _productsCRUD.getPageAsDataTable(1);
            dataGridView1.DataSource = _currentTable;
            correctView(_usingObj);
        }

        private void документыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clearTable();
            _usingObj = _currentObj.documents;
            if (_documentsCRUD == null)
                _documentsCRUD = new CRUD_Documents(_connection_string);
            _currentTable = _documentsCRUD.getPageAsDataTable(1);
            dataGridView1.DataSource = _currentTable;
            correctView(_usingObj);
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
            Int64 result = 0;
            
            switch (index)
            {
                case _currentObj.users:
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
                case _currentObj.positions:
                    {
                        Position position;
                        FormPosition addForm = new FormPosition(_connection_string);
                        if (addForm.ShowDialog() == DialogResult.OK)
                        {
                            position = addForm.positionTmp;
                            result = _positionsCRUD.create(position);
                            _currentTable = _positionsCRUD.getPageAsDataTable(_currentPage);
                            dataGridView1.DataSource = _currentTable;
                            correctView(index);
                        }
                        break;
                    }
                case _currentObj.operations:
                    {
                        Operation operation;
                        FormOperation addForm = new FormOperation(_connection_string);
                        if (addForm.ShowDialog() == DialogResult.OK)
                        {
                            operation = addForm.operationTmp;
                            result = _operationsCRUD.create(operation);
                            _currentTable = _operationsCRUD.getPageAsDataTable(_currentPage);
                            dataGridView1.DataSource = _currentTable;
                            correctView(index);
                        }
                        break;
                    }
                case _currentObj.agents:
                    {
                        Counteragent agent;
                        FormCounteragent addForm = new FormCounteragent(_connection_string);
                        if (addForm.ShowDialog() == DialogResult.OK)
                        {
                            agent = addForm.counteragentTmp;
                            result = _counteragentsCRUD.create(agent);
                            _currentTable = _counteragentsCRUD.getPageAsDataTable(_currentPage);
                            dataGridView1.DataSource = _currentTable;
                            correctView(index);
                        }
                        break;

                    }
            case _currentObj.orders:
                {
                    Order order;
                    FormOrder addForm = new FormOrder(_connection_string);
                    if (addForm.ShowDialog() == DialogResult.OK)
                    {
                        order = addForm.orderTmp;
                        result = _ordersCRUD.create(order);
                        _currentTable = _ordersCRUD.getPageAsDataTable(_currentPage);
                        dataGridView1.DataSource = _currentTable;
                        correctView(index);
                    }
                    break;
                }
            case _currentObj.orderItems:
                {
                    OrderChooseForm orderForm = new OrderChooseForm(_connection_string);
                    if (orderForm.ShowDialog() == DialogResult.OK)
                    {
                        Int64 tmpOrder = orderForm.orderNumber;
                        OrderItem orderItem;
                        FormOrderItem addForm = new FormOrderItem(_connection_string, tmpOrder);
                        if (addForm.ShowDialog() == DialogResult.OK)
                        {
                            orderItem = addForm.orderItemTmp;
                            result = _orderItemsCRUD.create(orderItem);
                            _currentTable = _orderItemsCRUD.getPageAsDataTable(_currentPage);
                            dataGridView1.DataSource = _currentTable;
                            correctView(index);
                        }
                    }
                    break;
                }
            case _currentObj.products:
                {
                    Product product;
                    FormProduct addForm = new FormProduct(_connection_string);
                    if (addForm.ShowDialog() == DialogResult.OK)
                    {
                        product = addForm.productTmp;
                        result = _productsCRUD.create(product);
                        _currentTable = _productsCRUD.getPageAsDataTable(_currentPage);
                        dataGridView1.DataSource = _currentTable;
                        correctView(index);
                    }
                    break;
                }
            case _currentObj.documents:
                {
                    Document document;
                    FormDocument addForm = new FormDocument(_connection_string);
                    if (addForm.ShowDialog() == DialogResult.OK)
                    {
                        document = addForm.documentTmp;
                        //result = _documentsCRUD.create(document);
                        _currentTable = _documentsCRUD.getPageAsDataTable(_currentPage);
                        dataGridView1.DataSource = _currentTable;
                        correctView(index);
                    }
                    break;
                }
            case _currentObj.templates:
                {
                    Template template;
                    FormTemplate addForm = new FormTemplate(_connection_string);
                    if (addForm.ShowDialog() == DialogResult.OK)
                    {
                        template = addForm.templateTmp;
                        //result = _templatesCRUD.create(template);
                        _currentTable = _templatesCRUD.getPageAsDataTable(_currentPage);
                        dataGridView1.DataSource = _currentTable;
                        correctView(index);
                    }
                    break;
                }

            default:
                break;
            }
            
        }
        //вызов формы для редактирования записи
        private void avokeEditForm(_currentObj index)
        {
            Int64 result = 0;
            switch (index)
            {
                case _currentObj.users:
                    {
                        User user = new User(dataGridView1.Rows[dataGridView1.SelectedRows[0].Index]);
                        FormUser editForm = new FormUser(_connection_string, user);
                        if (editForm.ShowDialog() == DialogResult.OK)
                        {
                            user = editForm.userTmp;
                            result = _usersCRUD.update(user);
                            _currentTable = _usersCRUD.getPageAsDataTable(_currentPage);
                            dataGridView1.DataSource = _currentTable;
                            correctView(index);
                        }
                        break;
                    }
                case _currentObj.positions:
                    {
                        Position position = new Position(dataGridView1.Rows[dataGridView1.SelectedRows[0].Index]);
                        FormPosition editForm = new FormPosition(_connection_string, position);
                        if (editForm.ShowDialog() == DialogResult.OK)
                        {
                            position = editForm.positionTmp;
                            result = _positionsCRUD.update(position);
                            _currentTable = _positionsCRUD.getPageAsDataTable(_currentPage);
                            dataGridView1.DataSource = _currentTable;
                            correctView(index);
                        }
                        break;
                    }
                case _currentObj.operations:
                    {
                        Operation operation = new Operation(dataGridView1.Rows[dataGridView1.SelectedRows[0].Index]);
                        FormOperation editForm = new FormOperation(_connection_string, operation);
                        if (editForm.ShowDialog() == DialogResult.OK)
                        {
                            operation = editForm.operationTmp;
                            result = _operationsCRUD.update(operation);
                            _currentTable = _operationsCRUD.getPageAsDataTable(_currentPage);
                            dataGridView1.DataSource = _currentTable;
                            correctView(index);
                        }
                        break;
                    }
                case _currentObj.agents:
                    {
                        Counteragent agent = new Counteragent(dataGridView1.Rows[dataGridView1.SelectedRows[0].Index]);
                        FormCounteragent editForm = new FormCounteragent(_connection_string, agent);
                        if (editForm.ShowDialog() == DialogResult.OK)
                        {
                            agent = editForm.counteragentTmp;
                            result = _counteragentsCRUD.update(agent);
                            _currentTable = _operationsCRUD.getPageAsDataTable(_currentPage);
                            dataGridView1.DataSource = _currentTable;
                            correctView(index);
                        }
                        break;
                    }
                default:
                    break;
            }
        }
        //вызов диалогового окна для удаления записи
        private void avokeDeleteForm(_currentObj index)
        {
            switch (index)
            {
                case _currentObj.users:
                    {
                        DialogResult result = MessageBox.Show("Вы уверены, что хотите удалить выбранного пользователя?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            _usersCRUD.delete(new User(dataGridView1.Rows[dataGridView1.SelectedRows[0].Index]));
                            _currentTable = _usersCRUD.getPageAsDataTable(_currentPage);
                            dataGridView1.DataSource = _currentTable;
                            correctView(index);
                        }
                        break;
                    }
                case _currentObj.positions:
                    {
                        DialogResult result = MessageBox.Show("Вы уверены, что хотите удалить выбранную должность?",
                                   "Подтверждение удаления",
                                   MessageBoxButtons.YesNo,
                                   MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            _positionsCRUD.delete(new Position(dataGridView1.Rows[dataGridView1.SelectedRows[0].Index]));
                            _currentTable = _positionsCRUD.getPageAsDataTable(_currentPage);
                            dataGridView1.DataSource = _currentTable;
                            correctView(index);
                        }
                        break;
                    }
                case _currentObj.operations:
                    {
                        DialogResult result = MessageBox.Show("Вы уверены, что хотите удалить выбранную операцию?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            _operationsCRUD.delete(new Operation(dataGridView1.Rows[dataGridView1.SelectedRows[0].Index]));
                            _currentTable = _operationsCRUD.getPageAsDataTable(_currentPage);
                            dataGridView1.DataSource = _currentTable;
                            correctView(index);
                        }
                        break;
                    }
                case _currentObj.agents:
                    {
                        DialogResult result = MessageBox.Show("Вы уверены, что хотите удалить выбранного контрагента?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            _counteragentsCRUD.delete(Convert.ToInt64(dataGridView1.Rows[dataGridView1.SelectedRows[0].Index].Cells["id"].Value));
                            _currentTable = _counteragentsCRUD.getPageAsDataTable(_currentPage);
                            dataGridView1.DataSource = _currentTable;
                            correctView(index);
                        }
                        break;
                    }
                case _currentObj.orders:
                    {
                        DialogResult result = MessageBox.Show("Вы уверены, что хотите удалить выбранный заказ?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            _ordersCRUD.delete(Convert.ToInt64(dataGridView1.Rows[dataGridView1.SelectedRows[0].Index].Cells["id"].Value));
                            _currentTable = _ordersCRUD.getPageAsDataTable(_currentPage);
                            dataGridView1.DataSource = _currentTable;
                            correctView(index);
                        }
                        break;
                    }
                case _currentObj.orderItems:
                    {
                        DialogResult result = MessageBox.Show("Вы уверены, что хотите удалить выбранную часть заказа?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            _orderItemsCRUD.Delete(Convert.ToInt64(dataGridView1.Rows[dataGridView1.SelectedRows[0].Index].Cells["id"].Value));
                            _currentTable = _orderItemsCRUD.getPageAsDataTable(_currentPage);
                            dataGridView1.DataSource = _currentTable;
                            correctView(index);
                        }
                        break; 
                    }
                case _currentObj.products:
                    {
                        DialogResult result = MessageBox.Show("Вы уверены, что хотите удалить выбранный товар?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            _productsCRUD.delete(Convert.ToInt64(dataGridView1.Rows[dataGridView1.SelectedRows[0].Index].Cells["id"].Value));
                            _currentTable = _productsCRUD.getPageAsDataTable(_currentPage);
                            dataGridView1.DataSource = _currentTable;
                            correctView(index);
                        }
                        break;
                    }
                case _currentObj.documents:
                    {
                        DialogResult result = MessageBox.Show("Вы уверены, что хотите удалить выбранный документ?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            _documentsCRUD.delete(Convert.ToInt64(dataGridView1.Rows[dataGridView1.SelectedRows[0].Index].Cells["id"].Value));
                            _currentTable = _documentsCRUD.getPageAsDataTable(_currentPage);
                            dataGridView1.DataSource = _currentTable;
                            correctView(index);
                        }
                        break;
                    }
                case _currentObj.templates:
                    {
                        DialogResult result = MessageBox.Show("Вы уверены, что хотите удалить выбранный шаблон?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            _templatesCRUD.delete(Convert.ToInt64(dataGridView1.Rows[dataGridView1.SelectedRows[0].Index].Cells["id"].Value));
                            _currentTable = _templatesCRUD.getPageAsDataTable(_currentPage);
                            dataGridView1.DataSource = _currentTable;
                            correctView(index);
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
                        _currentTable = _positionsCRUD.getPageAsDataTable(_currentPage);
                        dataGridView1.DataSource = _currentTable;
                        break;
                    }
                default:
                    break;
            }
        
        }

        private void correctView(_currentObj index)
        {
            switch (index)
            {
                case _currentObj.users:
                    {
                        dataGridView1.Columns["id"].Visible = false;
                        dataGridView1.Columns["IdPosition"].Visible = false;
                        dataGridView1.Columns["Name"].HeaderText = "Имя";
                        dataGridView1.Columns["Surname"].HeaderText = "Фамилия";
                        dataGridView1.Columns["Patronymic"].HeaderText = "Отчество";
                        dataGridView1.Columns["Place"].HeaderText = "№ рабочего места";
                        dataGridView1.Columns["Login"].Visible = false;
                        dataGridView1.Columns["Password"].Visible = false;
                        if (!dataGridView1.Columns.Contains("PositionName"))
                        {
                            DataGridViewColumn positionColumn = new DataGridViewTextBoxColumn();
                            positionColumn.Name = "PositionName";
                            positionColumn.HeaderText = "Должность";
                            positionColumn.DisplayIndex = 2;
                            dataGridView1.Columns.Add(positionColumn);
                        }
                        List<Position> list = CRUD_Positions.getAllPositions(_connection_string);
                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            if (row.Cells["IdPosition"].Value != null && row.Cells["IdPosition"].Value != DBNull.Value)
                            {
                                row.Cells["PositionName"].Value = Position.findNameInList(list, Convert.ToInt64(row.Cells["IdPosition"].Value));
                            }
                            else
                            {
                                row.Cells["PositionName"].Value = "null";
                            }
                        }
                        break;
                    }
                case _currentObj.operations:
                    {
                        dataGridView1.Columns["id"].Visible = false;
                        dataGridView1.Columns["IdPosition"].Visible=false;
                        dataGridView1.Columns["Name"].HeaderText = "Деятельности";
                        dataGridView1.Columns["Description"].HeaderText = "Описание";
                        List<Position> list = CRUD_Positions.getAllPositions(_connection_string);
                        if (!dataGridView1.Columns.Contains("PositionName"))
                        {
                            DataGridViewColumn positionColumn = new DataGridViewTextBoxColumn();
                            positionColumn.Name = "PositionName";
                            positionColumn.HeaderText = "Должность";
                            positionColumn.DisplayIndex = 2;
                            dataGridView1.Columns.Add(positionColumn);
                        }
                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            if (row.Cells["IdPosition"].Value != null && row.Cells["IdPosition"].Value != DBNull.Value)
                            {
                                row.Cells["PositionName"].Value = Position.findNameInList(list, Convert.ToInt64(row.Cells["IdPosition"].Value));
                            }
                            else
                            {
                                row.Cells["PositionName"].Value = "null";
                            }
                        }
                        break;
                    }
                case _currentObj.positions:
                    {
                        dataGridView1.Columns["id"].Visible = false;
                        dataGridView1.Columns["Name"].HeaderText = "Должность";
                        dataGridView1.Columns["Sector"].HeaderText = "Сектор";
                        dataGridView1.Columns["Department"].HeaderText = "Отдел";
                        dataGridView1.Columns["Leve1"].HeaderText = "Уровень допуска";
                        break;
                    }
                case _currentObj.agents:
                    {
                        dataGridView1.Columns["id"].Visible = false;
                        dataGridView1.Columns["Name"].HeaderText = "Контрагент";
                        break;
                    }
                case _currentObj.products:
                    {
                        dataGridView1.Columns["id"].Visible = false;
                        dataGridView1.Columns["Name"].HeaderText = "Название";
                        dataGridView1.Columns["Description"].HeaderText = "Описание";
                        dataGridView1.Columns["Price"].HeaderText = "Цена";
                        break;
                    }
                case _currentObj.orders:
                    {
                        dataGridView1.Columns["id"].Visible = false;
                        dataGridView1.Columns["Number"].HeaderText = "Номер заказа";
                        dataGridView1.Columns["IdCounteragent"].Visible = false;
                        dataGridView1.Columns["OrderDate"].HeaderText = "Дата заказа";
                        dataGridView1.Columns["DeliveryDate"].HeaderText = "Дата доставки";
                        dataGridView1.Columns["Comment"].HeaderText = "Комментарий";
                        if (!dataGridView1.Columns.Contains("AgentName"))
                        {
                            DataGridViewColumn agentColumn = new DataGridViewTextBoxColumn();
                            agentColumn.Name = "AgentName";
                            agentColumn.HeaderText = "Контрагент";
                            agentColumn.DisplayIndex = 3;
                            dataGridView1.Columns.Add(agentColumn);
                        }
                        List<Counteragent> agents = CRUD_Counteragents.getAllCounteragents(_connection_string);
                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            if (row.Cells["IdCounteragent"].Value != null && row.Cells["IdCounteragent"].Value != DBNull.Value)
                            {
                                row.Cells["AgentName"].Value = Counteragent.findNameInList(agents, Convert.ToInt64(row.Cells["IdCounteragent"].Value));
                            }
                            else
                            {
                                row.Cells["AgentName"].Value = "null";
                            }
                        }
                        break;
                    }
                case _currentObj.orderItems:
                    {
                        dataGridView1.Columns["id"].Visible = false;
                        dataGridView1.Columns["IdProduct"].Visible = false;
                        dataGridView1.Columns["IdOrder"].Visible = false;
                        dataGridView1.Columns["Price"].HeaderText = "Цена";
                        dataGridView1.Columns["Amount"].HeaderText = "Количество";
                        if (!dataGridView1.Columns.Contains("NumberOrder"))
                        {
                            DataGridViewColumn numberColumn = new DataGridViewTextBoxColumn();
                            numberColumn.Name = "NumberOrder";
                            numberColumn.HeaderText = "Номер заказа";
                            numberColumn.DisplayIndex = 3;
                            dataGridView1.Columns.Add(numberColumn);
                            DataGridViewColumn productColumn = new DataGridViewTextBoxColumn();
                            productColumn.Name = "ProductName";
                            productColumn.HeaderText = "Название товара";
                            productColumn.DisplayIndex = 4;
                            dataGridView1.Columns.Add(productColumn);
                        }
                        List<Product> products = CRUD_Products.getAllProducts(_connection_string);
                        List<Order> orders = CRUD_Orders.getAllOrders(_connection_string);
                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            if (row.Cells["IdProduct"].Value != null && row.Cells["IdProduct"].Value != DBNull.Value)
                            {
                                row.Cells["ProductName"].Value = Product.findNameInList(products, Convert.ToInt64(row.Cells["IdProduct"].Value));
                            }
                            else
                            {
                                row.Cells["ProductName"].Value = "null";
                            }
                            if (row.Cells["IdOrder"].Value != null && row.Cells["IdOrder"].Value != DBNull.Value)
                            {
                                row.Cells["NumberOrder"].Value = Order.findNumberInList(orders, Convert.ToInt64(row.Cells["IdOrder"].Value));
                            }
                            else
                            {
                                row.Cells["NumberOrder"].Value="null";
                            }
                        }
                        break;
                    }
                case _currentObj.documents:
                    {
                        dataGridView1.Columns["id"].Visible = false;
                        dataGridView1.Columns["Name"].HeaderText = "Название документа";
                        dataGridView1.Columns["IdTemplate"].Visible = false;
                        if (!dataGridView1.Columns.Contains("TemplateName"))
                        {
                            DataGridViewColumn templateName = new DataGridViewTextBoxColumn();
                            templateName.Name = "TemplateName";
                            templateName.HeaderText = "Название шаблона";
                            templateName.DisplayIndex = 3;
                            dataGridView1.Columns.Add(templateName);
                        }
                        List<Template> templates = CRUD_Templates.getAllNamesTemplates(_connection_string);
                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            if (row.Cells["IdTemplate"].Value != null && row.Cells["IdTemplate"].Value != DBNull.Value)
                            {
                                row.Cells["TemplateName"].Value = Template.findNameInList(templates, Convert.ToInt64(row.Cells["IdTemplate"].Value));
                            }
                            else
                            {
                                row.Cells["TemplateName"].Value = "null";
                            }
                        }
                        break;
                    }
                case _currentObj.templates:
                    {
                        dataGridView1.Columns["id"].Visible = false;
                        dataGridView1.Columns["Name"].HeaderText = "Название шаблона";
                        break;
                    }
            }
        }

        private void EditDBForm_Load(object sender, EventArgs e)
        {

        }

        private void вГлавноеМенюToolStripMenuItem_Click(object sender, EventArgs e)
        {
            localContext.SwitchMainForm(new Diploma.Views.MainMenuForm(localContext, enteredUser, _connection_string));
        }
    }
}
