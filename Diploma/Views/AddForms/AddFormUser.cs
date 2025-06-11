using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Diploma.Models;
using Diploma.Controllers;
namespace Diploma.Views
{
    public partial class AddFormUser : Form
    {
        private String _connection;
        public Int32 tmpPosition;
        public String tmpName;
        public String tmpSurname;
        public String tmpPatronymic;
        public Int32 tmpPlace;
        public String tmpLogin;
        public String tmpPassword;

        public AddFormUser()
        {
            InitializeComponent();
            
        }
        public AddFormUser(String con)
        {
            InitializeComponent();
            _connection = con;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            tmpName = textBox1.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private bool noDigit(String checkStr)
        {
            return checkStr.Any(char.IsLetter);
        }

        private bool noLetter(String checkStr)
        {
            return checkStr.Any(char.IsDigit);
        }

        private void AddFormUser_Load(object sender, EventArgs e)
        {
            comboBox1.DataSource = CRUD_Positions.getAllPositions(_connection);
            comboBox1.DisplayMember = "Name";
            comboBox1.ValueMember = "id";
        }
    }
}
