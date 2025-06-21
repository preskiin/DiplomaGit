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
        private DocumentController docController;
        private Diploma.Controllers.MyAppContext localContext;
        private Diploma.Models.User enteredUser;
        private String _connection;

        //Исправить по завершению формирования: не должно быть доступа к бд!
        public TemplateForm()
        {
            InitializeComponent();

            _connection = "Data Source=Preskiin-PC;Initial Catalog=Diploma;Integrated Security=True;Encrypt=False;trusted_connection=True";
        }

        public TemplateForm(Diploma.Controllers.MyAppContext context, Diploma.Models.User user, String connection)
        {
            InitializeComponent();
            localContext = context;
            enteredUser = user;
            _connection = connection;
        }

        private void onContextMenuRequested(object sender, Microsoft.Web.WebView2.Core.CoreWebView2ContextMenuRequestedEventArgs e)
        {
            e.Handled = true;
            this.contextMenuStrip1.Show(Cursor.Position);
        }

        private void onAnswerFromWeb(object sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            if (e.TryGetWebMessageAsString().StartsWith("{\"nameElement"))
            {
                var message = JsonConvert.DeserializeObject<dynamic>(e.WebMessageAsJson);
                dynamic data = JsonConvert.DeserializeObject(message);
                var tmpElem = new DocumentController.elemToCreate();
                tmpElem.name_element = data.nameElement;
                tmpElem.name_to_connect_element = data.nameToConnectElement;
                tmpElem.need_field = data.needField;
                tmpElem.need_table = data.needTable;
                tmpElem.current_field = data.currentField;
                tmpElem.current_table = data.currentTable;
                tmpElem.type_element = data.typeElement;
                tmpElem.value = data.value;
                tmpElem.is_filled = Convert.ToBoolean(data.isFilled);
                docController.updateListElements(tmpElem);
            }
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            //textBox1.Text = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..") + "\\2.docx");//адрес файла docx для чтения
            await webView21.EnsureCoreWebView2Async();
            this.webView21.CoreWebView2.Settings.IsScriptEnabled = true;
            string htmlContent = File.ReadAllText("C:/Users/User/Desktop/test (2).html");

            webView21.CoreWebView2.Settings.IsWebMessageEnabled = true;
            this.webView21.CoreWebView2.ContextMenuRequested += onContextMenuRequested; //подписка на событие о нажатии ПКМ внутри webview2
            this.webView21.CoreWebView2.WebMessageReceived += onAnswerFromWeb; //подписка на событие об ответе с webview2 о выборе элемента в списке
            docController = new DocumentController(_connection);
            docController.setHtml(htmlContent);
            docController.createAllTemplateObjects();
            //String htmlCodeFromWV = await getHtmlFromWebView2();
            //docController.setHtml(htmlCodeFromWV);
            webView21.CoreWebView2.NavigateToString(docController.getHtml());

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
        //сохранение текущей строки в вебвью в файлы компьютера
        private async void button2_Click(object sender, EventArgs e)
        {
            String htmlCodeFormWV = await getHtmlFromWebView2();
            docController.saveHtml(htmlCodeFormWV);
        }


        //создает строку html из страницы, которая отображена сейчас в webView2
        public async Task<string> getHtmlFromWebView2()
        {
            try
            {
                // Получаем HTML с помощью JavaScript
                string encodedHtml = await webView21.CoreWebView2.ExecuteScriptAsync(
                    "document.documentElement.outerHTML;"
                );

                // Декодируем JSON-строку (удаляем кавычки и экранированные символы)
                string cleanHtml = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(encodedHtml);
                return cleanHtml;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении HTML: {ex.Message}");
                return null;
            }
        }

        //переход на главную форму, если пользователь зашел через нее сюда
        private void button3_Click(object sender, EventArgs e)
        {
            localContext.SwitchMainForm(new Diploma.Views.MainMenuForm(localContext, enteredUser, _connection));
        }

        private async void webView21_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            await webView21.CoreWebView2.ExecuteScriptAsync("document.body.contentEditable = 'true'; document.designMode = 'on';");
            docController.getElementsFromHtml("template2003");
        }

        //возвращает строку скрипта для выполнения со вставкой html-кода на позицию каретки, который был передан в параметре
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
            String html = docController.createListInput(DocumentController.usingCRUD.people);
            await webView21.CoreWebView2.ExecuteScriptAsync(scriptInsertOnPos(html));
            //docController.getElementsFromHtml("template2003");
        }

        private async void привязанноеПолеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<DocumentController.elemToCreate> tmp_list = new List<DocumentController.elemToCreate>();
            foreach (var element in docController.elements)
            {
                if (element.name_to_connect_element !="null")
                {
                    tmp_list.Add(element);
                }
            }
            FormChooseList formChoice = new FormChooseList(tmp_list);
            if (DialogResult.OK ==formChoice.ShowDialog())
            {
                String html = docController.createBoundField(formChoice.currentElement);
                await webView21.CoreWebView2.ExecuteScriptAsync(scriptInsertOnPos(html));
                docController.getElementsFromHtml("template2003");
                //забираем с формы chooseList объект определенного класса, и кидаем нужные значения в наш объект elements
            }
            
        }

        private void webView21_NavigationStarting(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e)
        {

        }
    }
}
