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
        private String _connection = "Data Source=PRESKIIN-PC;Initial Catalog=Diploma;Integrated Security=True;Encrypt=False;trusted_connection=True";
        public Form1()
        {
            InitializeComponent();
        }

        private void onContextMenuRequested(object sender, Microsoft.Web.WebView2.Core.CoreWebView2ContextMenuRequestedEventArgs e)
        {
            e.Handled = true;

        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..") + "\\1.docx");//адрес файла docx для чтения
            await webView21.EnsureCoreWebView2Async();
            this.webView21.CoreWebView2.ContextMenuRequested += onContextMenuRequested;
            docController = new DocumentController(_connection);
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
            //docController.addListInput(docController.getHtml().IndexOf("<div>") + 5);
            this.webView21.NavigateToString(docController.getHtml());
        }

        private async void webView21_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            await webView21.CoreWebView2.ExecuteScriptAsync("document.body.contentEditable = 'true'; document.designMode = 'on';");
            
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            //await webView21.EnsureCoreWebView2Async();
            String html = docController.createListInput(DocumentController.usingCRUD.positions);
            string script = $@"
        (function() {{
            const selection = window.getSelection();
            if (selection.rangeCount > 0) {{
                const range = selection.getRangeAt(0);
                range.deleteContents();
                
                const tempDiv = document.createElement('div');
                tempDiv.innerHTML = `{html}`;
                
                const fragment = document.createDocumentFragment();
                while (tempDiv.firstChild) {{
                    fragment.appendChild(tempDiv.firstChild);
                }}
                
                range.insertNode(fragment);
                range.setStartAfter(fragment.lastChild);
                range.collapse(true);
                selection.removeAllRanges();
                selection.addRange(range);
            }}
            return true;
        }})()";

            await webView21.CoreWebView2.ExecuteScriptAsync(script);
        }
    }
}
