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
        private MyAuthorization auth;
        public Authorize()
        {
            InitializeComponent();
            auth = new MyAuthorization();
            
        }

        

        private void button1_Click(object sender, EventArgs e)
        {
            //private MyAuthorization auth = new MyAuthorization();
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

        private void button2_Click(object sender, EventArgs e)
        {
            User tmp = new User(1, 2, "Анна", "Полушкина", "Петровна", 2, auth.get_sha256("AnnaPol"), auth.get_sha256("AnnaPol2000"));
            CRUD_Users tmp_CRUD = new CRUD_Users("Data Source=PRESKIIN-PC;Initial Catalog=Diploma;Integrated Security=True;Encrypt=False;trusted_connection=True");
            tmp_CRUD.create(tmp);
        }
    }
}
