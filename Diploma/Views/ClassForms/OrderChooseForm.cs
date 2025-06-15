using Diploma.Controllers;
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
    public partial class OrderChooseForm : Form
    {
        public Int64 orderNumber;
        private String _connection;
        public OrderChooseForm()
        {
            InitializeComponent();
        }
        
        public OrderChooseForm(String con)
        {
            InitializeComponent();
            _connection = con;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void OrderChooseForm_Load(object sender, EventArgs e)
        {
            comboBox1.DataSource = CRUD_Orders.getAllNumOrders(_connection);
            comboBox1.DisplayMember = "Number";
            comboBox1.ValueMember = "id";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            orderNumber = (Int64)comboBox1.SelectedValue;
        }
    }
}
