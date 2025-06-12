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
    public partial class FormUser : Form
    {
        private String _connection;
        public User userTmp;

        public FormUser()
        {
            InitializeComponent();
            
        }

        public FormUser(String con)
        {
            InitializeComponent();
            _connection = con;
        }

        public FormUser(String con, User userToEdit)
        {
            InitializeComponent();
            _connection = con;
            userTmp = userToEdit;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (checkAllFields())
            {
                if (userTmp == null)
                {
                    userTmp = new User(id: 0, id_position: (Int32)comboBox1.SelectedValue, name: textBox1.Text,
                        surname: textBox2.Text, patronymic: textBox3.Text, place_num: Convert.ToInt32(textBox4.Text),
                        login: MyAuthorization.get_sha256(textBox5.Text), password: MyAuthorization.get_sha256(textBox6.Text));
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    userTmp = new User(id: userTmp.Id, id_position: (Int32)comboBox1.SelectedValue, name: textBox1.Text,
                        surname: textBox2.Text, patronymic: textBox3.Text, place_num: Convert.ToInt32(textBox4.Text),
                        login: textBox5.Text, password: textBox6.Text);
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
            else
            {
                MessageBox.Show("Введены неверные данные", "Ошибка");
            }
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
            if (userTmp != null)
            {
                textBox1.Text = userTmp.Name;
                textBox2.Text = userTmp.Surname;
                textBox3.Text = userTmp.Patronymic;
                comboBox1.SelectedValue = userTmp.IdPosition;
                textBox4.Text = Convert.ToString(userTmp.Place);
                textBox5.Text = userTmp.Login;
                textBox5.Enabled = false;
                textBox6.Text = userTmp.Password;
                textBox6.Enabled = false;
            }
        }

        private bool checkAllFields()
        {
            if (textBox1.Text != null && noDigit(textBox1.Text) &&
                textBox2.Text != null && noDigit(textBox2.Text) &&
                textBox3.Text != null && noDigit(textBox3.Text) &&
                comboBox1.SelectedIndex != -1 &&
                textBox4.Text != null && noLetter(textBox4.Text) &&
                textBox5.Text != null && textBox6.Text != null)
                return true;
            else return false;

        }
    }
}
