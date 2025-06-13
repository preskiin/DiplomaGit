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

namespace Diploma.Views.AddForms
{
    public partial class FormPosition : Form
    {
        private String _connection;
        public Position positionTmp;

        public FormPosition()
        {
            InitializeComponent();
        }

        public FormPosition(String con)
        {
            InitializeComponent();
            _connection = con;
        }

        public FormPosition(String con, Position positionToEdit)
        {
            InitializeComponent();
            _connection = con;
            positionTmp = positionToEdit;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (checkAllFields())
            {
                if (positionTmp == null)
                {
                    positionTmp = new Position(
                        id: 0,
                        name: textBox1.Text,
                        sector: Convert.ToInt32(textBox2.Text),
                        department: Convert.ToInt32(textBox3.Text),
                        level: Convert.ToInt32(textBox4.Text));
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    positionTmp = new Position(
                        id: positionTmp.Id,
                        name: textBox1.Text,
                        sector: Convert.ToInt32(textBox2.Text),
                        department: Convert.ToInt32(textBox3.Text),
                        level: Convert.ToInt32(textBox4.Text));
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

        private void FormPosition_Load(object sender, EventArgs e)
        {
            if (positionTmp != null)
            {
                textBox1.Text = positionTmp.Name;
                textBox2.Text = Convert.ToString(positionTmp.Sector);
                textBox3.Text = Convert.ToString(positionTmp.Department);
                textBox4.Text = Convert.ToString(positionTmp.Level);
            }
        }

        private bool checkAllFields()
        {
            if (textBox1.Text != null && noDigit(textBox1.Text) &&
                textBox2.Text != null && noLetter(textBox2.Text) &&
                textBox3.Text != null && noLetter(textBox3.Text) &&
                textBox4.Text != null && noLetter(textBox4.Text))
                return true;
            else return false;
        }
    }
}
