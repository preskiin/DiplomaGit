using Diploma.NewFolder1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Net;
using Diploma.Controllers;

namespace Diploma
{
    public partial class Authorize : Form
    {
        public Authorize()
        {
            InitializeComponent();
            
            
        }

        

        private void button1_Click(object sender, EventArgs e)
        {
            MyAuthorization auth = new MyAuthorization();
            if (auth.check_auth(textBox1.Text, textBox2.Text))
            {
                button1.BackColor = Color.Green;
            }
            else
                label3.Text = "Неверное имя пользователя или пароль";


            //if (!users.check_login(textBox1.Text))
            //    button1.BackColor = Color.Red;
            //else
            //    
        }
    }
}
