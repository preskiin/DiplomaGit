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
            textBox1.Text = "D:\\Subjects\\4_2\\Диплом\\Diploma\\TestDocx.docx";
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
            if (docController.docxToHtml(textBox1.Text) /*&& docController.setHtml(this.webBrowser1)*/)
            {
                webBrowser1.DocumentText = "\"<html>\r\n<head>\r\n<meta http-equiv=\\\"Content-Type\\\" content=\\\"text/html; charset=utf-8\\\" />\r\n<meta http-equiv=\\\"Content-Style-Type\\\" content=\\\"text/css\\\" />\r\n<meta name=\\\"generator\\\" content=\\\"Aspose.Words for .NET 25.5.0\\\" />\r\n<title></title>\r\n</head>\r\n<body style=\\\"line-height:108%; font-family:Calibri; font-size:11pt\\\">\r\n<div>\r\n<div style=\\\"-aw-headerfooter-type:header-primary; clear:both\\\">\r\n<p style=\\\"margin-top:0pt; margin-bottom:8pt\\\">\r\n<span style=\\\"height:0pt; display:block; position:absolute; z-index:-65537\\\">\r\n<img src=\\\"Aspose.Words.56caa6cc-a2c8-4163-85e6-d0f5a52a718f.001.png\\\" width=\\\"624\\\" height=\\\"339\\\" alt=\\\"\\\" style=\\\"margin-top:257.79pt; -aw-left-pos:0pt; -aw-rel-hpos:margin; -aw-rel-vpos:margin; -aw-top-pos:0pt; -aw-wrap-type:none; position:absolute\\\" />\r\n</span>\r\n<span style=\\\"-aw-import:ignore\\\">&#xa0;</span>\r\n</p>\r\n</div>\r\n<p style=\\\"margin-top:0pt; margin-bottom:8pt; line-height:108%; font-size:12pt\\\">\r\n<span style=\\\"font-weight:bold; color:#ff0000\\\">Created with an evaluation copy of Aspose.Words. To remove all limitations, you can use Free Temporary Licens...\"";
            }
            else
            {
                MessageBox.Show("Текст не был присвоен элементу", "Ошибка", MessageBoxButtons.OK);
            }    
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //docController.saveFromHtmlToDocx(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\DocxToHtml.html",
            //    Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\testDoc.docx");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
                textBox2.Text = System.IO.Directory.GetCurrentDirectory() + "\\tmp_data\\DocxToHtml.html";
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
