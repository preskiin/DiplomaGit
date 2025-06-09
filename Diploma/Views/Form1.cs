using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Diploma.Controllers;

namespace Diploma
{
    public partial class Form1 : Form
    {
        private DocumentController docController;
        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            await webView21.EnsureCoreWebView2Async();
            textBox1.Text = Directory.GetCurrentDirectory()+"\\tmp_data\\1.docx";//адрес файла docx для чтения
            docController = new DocumentController();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (docController.docxToHtml(textBox1.Text))
            {
                this.webView21.NavigateToString(docController.getHtml());
            }
            else
            {
                MessageBox.Show("Текст не был присвоен элементу", "Ошибка", MessageBoxButtons.OK);
            }    
        }

        private void button2_Click(object sender, EventArgs e)
        {
            docController.saveHtml();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            docController.addListInput(docController.getHtml().IndexOf("<div>") + 5);
            this.webView21.NavigateToString(docController.getHtml());
        }
    }
}
