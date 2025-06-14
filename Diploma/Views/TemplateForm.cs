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
using Diploma.Views;
using Newtonsoft.Json;

namespace Diploma
{
    public partial class TemplateForm : Form
    {
        int counter = 0;
        List<DocumentController.elemToCreate> elements;
        

        private DocumentController docController;
        private String _connection = "Data Source=PRESKIIN-PC;Initial Catalog=Diploma;Integrated Security=True;Encrypt=False;trusted_connection=True";
        public TemplateForm()
        {
            InitializeComponent();
        } 

        private void onContextMenuRequested(object sender, Microsoft.Web.WebView2.Core.CoreWebView2ContextMenuRequestedEventArgs e)
        {
            e.Handled = true;
            this.contextMenuStrip1.Show(Cursor.Position);
        }

        private void onAnswerFromWeb(object sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            if (e.TryGetWebMessageAsString().StartsWith("{\"listId"))
            {
                var message = JsonConvert.DeserializeObject<string>(e.WebMessageAsJson);
                dynamic data = JsonConvert.DeserializeObject(message);
                MessageBox.Show($"В выпадающем списке с названием: {data.listId} \nБыл выбран элемент с ID: {data.selectedId}");
            }
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..") + "\\2.docx");//адрес файла docx для чтения
            await webView21.EnsureCoreWebView2Async();
            this.webView21.CoreWebView2.Settings.IsScriptEnabled = true;
            string htmlContent = File.ReadAllText("C:/Users/User/Desktop/MyHtml.html");
            webView21.CoreWebView2.NavigateToString(htmlContent);
            //////this.webView21.CoreWebView2.Settings.IsWebMessageEnabled = true;

            //this.webView21.CoreWebView2.Navigate("about:blank");


            this.webView21.CoreWebView2.ContextMenuRequested += onContextMenuRequested; //подписка на событие о нажатии ПКМ внутри webview2
            this.webView21.CoreWebView2.WebMessageReceived += onAnswerFromWeb; //подписка на событие об ответе с webview2 о выборе элемента в списке
            docController = new DocumentController(_connection);
            await docController.getHtmlFromWebView2(this.webView21);
            
        
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
            docController.saveHtml(this.webView21);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //docController.addListInput(docController.getHtml().IndexOf("<div>") + 5);
            //this.webView21.NavigateToString(docController.getHtml());
        }

        private async void webView21_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            await webView21.CoreWebView2.ExecuteScriptAsync("document.body.contentEditable = 'true'; document.designMode = 'on';");
            elements = docController.getElementsFromHtml("template2003");
            counter = elements.Count;
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            //await webView21.EnsureCoreWebView2Async();
            String html = docController.createListInput(DocumentController.usingCRUD.people, counter);
            counter++;
            await webView21.CoreWebView2.ExecuteScriptAsync(scriptInsertOnPos(html));
        }

        private string scriptInsertOnPos(String htmlElem)
        {
            string script = $@"
            (function() {{
                const selection = window.getSelection();
                if (selection.rangeCount > 0) {{
                    const range = selection.getRangeAt(0);
                    range.deleteContents();
                    
                    const tempDiv = document.createElement('div');
                    tempDiv.innerHTML = `{htmlElem}`;
                    
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
            return script;
        }

        private async void работникиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String html = docController.createListInput(DocumentController.usingCRUD.people, counter);
            counter++;
            await webView21.CoreWebView2.ExecuteScriptAsync(scriptInsertOnPos(html));
        }

        private void привязанноеПолеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<DocumentController.elemToCreate> tmp_list = new List<DocumentController.elemToCreate>();
            foreach (var element in elements)
            {
                if (element.name_to_connect_element == "base")
                {
                    tmp_list.Add(element);
                }
            }
            FormChooseList formChoice = new FormChooseList(tmp_list);
            if (DialogResult.OK ==formChoice.ShowDialog())
            {
                //забираем с формы chooseList объект определенного класса, и кидаем нужны значения в наш объект elements
                
            }
        }
 
    }
}
