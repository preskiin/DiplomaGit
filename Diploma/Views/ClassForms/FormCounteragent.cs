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

namespace Diploma.Views.ClassForms
{
    public partial class FormCounteragent : Form
    {
        private string _connection;
        public Counteragent counteragentTmp;

        public FormCounteragent()
        {
            InitializeComponent();
        }

        public FormCounteragent(string connection)
        {
            InitializeComponent();
            _connection = connection;
        }

        public FormCounteragent(string connection, Counteragent counteragentToEdit)
        {
            InitializeComponent();
            _connection = connection;
            counteragentTmp = counteragentToEdit;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (CheckAllFields())
            {
                if (counteragentTmp == null)
                {
                    counteragentTmp = new Counteragent(
                        id: 0,
                        name: textBox1.Text
                    );
                }
                else
                {
                    counteragentTmp = new Counteragent(
                        id: counteragentTmp.Id,
                        name: textBox1.Text
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

        private void FormCounteragent_Load(object sender, EventArgs e)
        {
            if (counteragentTmp != null)
            {
                textBox1.Text = counteragentTmp.Name;
            }
        }

        private bool CheckAllFields()
        {
            return !string.IsNullOrWhiteSpace(textBox1.Text);
        }
    }
}
