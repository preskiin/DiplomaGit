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
        private Diploma.Controllers.MyAppContext localContext;
        private String _connection;
        private String origPassword;
        public Authorize()
        {
            InitializeComponent();
        }
        public Authorize(Diploma.Controllers.MyAppContext context, String connection)
        {
            InitializeComponent();
            _connection = connection;
            localContext = context;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var auth = new Diploma.Controllers.MyAuthorization(_connection);
            Diploma.Models.User tmpUser = auth.check_auth(textBox1.Text, textBox2.Text);
            if (tmpUser!=null)
            {
                localContext.SwitchMainForm(new Diploma.Views.MainMenuForm(localContext, tmpUser, _connection));
            }
            else
                label3.Text = "Неверное имя пользователя или пароль";
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
        }
    }
}
