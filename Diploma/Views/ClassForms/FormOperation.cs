using Diploma.Controllers;
using Diploma.Models;
using DocumentFormat.OpenXml.Office2010.Word.DrawingShape;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diploma.Views.AddForms
{
    public partial class FormOperation : Form
    {
        private string _connection;
        public Operation operationTmp;

        public FormOperation()
        {
            InitializeComponent();
        }

        public FormOperation(string connection)
        {
            InitializeComponent();
            _connection = connection;
        }

        public FormOperation(string connection, Operation operationToEdit)
        {
            InitializeComponent();
            _connection = connection;
            operationTmp = operationToEdit;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (CheckAllFields())
            {
                if (operationTmp == null)
                {
                    operationTmp = new Operation(
                        id: 0,
                        idPosition: (Int64)comboBox1.SelectedValue,
                        name: textBox1.Text,
                        description: textBox2.Text
                    );
                }
                else
                {
                    operationTmp = new Operation(
                        id: operationTmp.Id,
                        idPosition: (Int64)comboBox1.SelectedValue,
                        name: textBox1.Text,
                        description: textBox2.Text
                    );
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("Введены неверные данные", "Ошибка");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void FormOperation_Load(object sender, EventArgs e)
        {

            comboBox1.DataSource = CRUD_Positions.getAllPositions(_connection);
            comboBox1.DisplayMember = "Name";
            comboBox1.ValueMember = "id";
            this.Text = "Добавление операции";
            if (operationTmp != null)
            {
                this.Text = "Редактирование операции";
                textBox1.Text = operationTmp.Name;
                textBox2.Text = operationTmp.Description;
                comboBox1.SelectedValue = operationTmp.IdPosition;
            }

        }

        private bool CheckAllFields()
        {
            return !string.IsNullOrWhiteSpace(textBox1.Text) &&
                   comboBox1.SelectedIndex != -1;
        }
    }
}
