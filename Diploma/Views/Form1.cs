using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
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

        private void Form1_Load(object sender, EventArgs e)
        {
            //textBox1.Text = System.IO.Directory.GetCurrentDirectory()+"\\tmp_data\\TestDocx.docx";
            textBox1.Text = "D:\\Subjects\\4_2\\Диплом\\ВКР.docx";
            docController = new DocumentController();
            //docController.openDocument()
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (webBrowser1.Document != null)
            {
                //textBox1.Text = webBrowser1.Url.ToString();
                webBrowser1.Document.ExecCommand("EditMode", false, null);
                //webBrowser1.Document.Body.SetAttribute("contentEditable", "true");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (docController.docxToHtml(textBox1.Text))
            {
                this.webBrowser1.DocumentText = docController.getHtml();
            }
            else
            {
                MessageBox.Show("Текст не был присвоен элементу", "Ошибка", MessageBoxButtons.OK);
            }    
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //webBrowser1.Navigate(System.IO.Directory.GetCurrentDirectory() + "\\tmp_data\\AsposeWordsDocxToHtml.html");
            //webBrowser1.Update();
            //webBrowser2.DocumentText = webBrowser1.DocumentText;
            
            //docController.saveFromHtmlToDocx(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\DocxToHtml.html",
            //    Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\testDoc.docx");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
                textBox2.Text = System.IO.Directory.GetCurrentDirectory() + "\\tmp_data\\AsposeWordsDocxToHtml.html";
            if (docController.saveFromDocxToHtml(textBox1.Text, textBox2.Text))
            {
                docController.openDocument(webBrowser1);
                webBrowser1.Update();
            }
            else
            {
                MessageBox.Show("Такого файла не существует", "Ошибка!", MessageBoxButtons.OK);
            }
        }
    }
}
