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
            textBox1.Text = "D:\\Subjects\\4_2\\Диплом\\ВКР.docx";
            docController = new DocumentController();
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
                //this.webBrowser1.DocumentText = docController.getHtml();
                
                this.webView21.NavigateToString(docController.getHtml());
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
            docController.saveHtml();
            //docController.saveFromHtmlToDocx(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\DocxToHtml.html",
            //    Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\testDoc.docx");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            docController.addListInput(docController.getHtml().IndexOf("<div>") + 5);
            this.webView21.NavigateToString(docController.getHtml());
            //webBrowser1.DocumentText = docController.getHtml();
            //webBrowser1.Refresh(WebBrowserRefreshOption.Completely);
            //webBrowser1.Update();
            //webBrowser1.Navigate("about:blank");
            //while (webBrowser1.ReadyState != WebBrowserReadyState.Complete)
            //{
            //    Application.DoEvents();
            //}
            //webBrowser1.Document?.OpenNew(true);
            //webBrowser1.Document.Write(docController.getHtml());
        }
    }
}
