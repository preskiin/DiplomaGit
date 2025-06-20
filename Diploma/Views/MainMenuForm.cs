using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diploma.Views
{
    public partial class MainMenuForm : Form
    {
        private Diploma.Controllers.MyAppContext localContext;
        private Diploma.Models.User enteredUser;
        private String _connectionString;

        public MainMenuForm()
        {
            InitializeComponent();
        }

        public MainMenuForm(Diploma.Controllers.MyAppContext context, Diploma.Models.User user, String connection)
        {
            InitializeComponent();
            localContext = context;
            enteredUser = user;
            _connectionString = connection;
        }

        private void MainMenuForm_Load(object sender, EventArgs e)
        {
            label1.Text = "Здравствуйте, " + enteredUser.Name + " " + enteredUser.Patronymic + "!";
            if (Diploma.Controllers.CRUD_Positions.getLevel(enteredUser.IdPosition, _connectionString) != 8919409)
            {
                button4.Enabled = false;
            }
            switch (label1.Text.Length)
            {
                case int n when n <=21:
                    {
                        label1.Location = new Point(110, 10);
                        break;
                    }
                case int n when n > 20 && n <= 25:
                    {
                        label1.Location = new Point(85, 10);
                        break;
                    }
                case int n when n > 25 && n <= 30:
                    {
                        label1.Location = new Point(60, 10);
                        break;
                    }
                case int n when n > 30 && n <= 35:
                    {
                        label1.Location = new Point(35, 10);
                        break;
                    }
                case int n when n > 35 && n <= 40:
                    {
                        label1.Location = new Point(10, 10);
                        break;
                    }
                case int n when n > 40 && n <= 45:
                    {
                        label1.Location = new Point(0, 10);
                        break;
                    }
                default:
                    break;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            localContext.SwitchMainForm(new Diploma.Views.EditDBForm(localContext, enteredUser, _connectionString));
        }
    }
}
