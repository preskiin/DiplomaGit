using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Diploma.Controllers;
using Diploma.Models;
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

            /*_connection = "Data Source=Preskiin-PC;Initial Catalog=Diploma;Integrated Security=True;Encrypt=False;trusted_connection=True"*/;
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
                docController.updateBoundElements(tmpElem, webView21);
            }
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            //textBox1.Text = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..") + "\\2.docx");//адрес файла docx для чтения
            await webView21.EnsureCoreWebView2Async();
            this.webView21.CoreWebView2.Settings.IsScriptEnabled = true;
            string htmlContent = File.ReadAllText("C:/Users/User/Desktop/MyHtml.html");

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
            
        }

        //сохранение текущей строки в вебвью в файлы компьютера
        private async void button2_Click(object sender, EventArgs e)
        {
            //не ясно как и от чего это зависит, но иногда программа крашится и вылетает. В интернете говорили про использование в разных потоках одного элемента, прочитай статью по ошибке/отредактируй код
            //я удмаю, что проблема где-то с вебвью и асинхранным ожиданием чтения в строку
            String htmlCodeFormWV = await getHtmlFromWebView2();
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                FileName = "MyHtml.html",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Filter = "HTML Files (*.html)|*.html|All files (*.*)|*.*",
                Title = "Сохранить HTML-файл"
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;
                docController.setHtml(htmlCodeFormWV);
                docController.saveHtml(filePath);
                MessageBox.Show(
                $"Файл успешно сохранён:\n{filePath}",
                "Успех",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
            }
            else
            {
                MessageBox.Show(
                "Сохранение отменено.",
                "Информация",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
            }
            //docController.saveHtml(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\MyHtml.html");
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
            docController.getAllElementsFromHtml("template2003");
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

        private async void привязанноеПолеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<DocumentController.elemToCreate> tmp_list = new List<DocumentController.elemToCreate>();
            foreach (var element in docController.elements)
            {
                if (element.name_to_connect_element =="base")
                {
                    tmp_list.Add(element);
                }
            }
            FormChooseList formChoice = new FormChooseList(tmp_list);
            if (DialogResult.OK ==formChoice.ShowDialog())
            {
                //метод createBoundFields должен быть универсальным, так как значения, которые можно получить от formChooseList достаточные
                //для создания шаблона
                String html = docController.createTemplateForBoundField(formChoice.currentElement);//строка html-кода шаблона привязанного поля
                await webView21.CoreWebView2.ExecuteScriptAsync(scriptInsertOnPos(html));
                docController.setHtml(await getHtmlFromWebView2());//Сохраняет изменения в htmlCode объекта docController
            }

        }

        private void webView21_NavigationStarting(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e)
        {

        }

        private async void работникиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String html = docController.createListInput(DocumentController.usingCRUD.people);
            await webView21.CoreWebView2.ExecuteScriptAsync(scriptInsertOnPos(html));
            docController.setHtml(await getHtmlFromWebView2());
        }

        private async void должностиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String html = docController.createListInput(DocumentController.usingCRUD.positions);
            await webView21.CoreWebView2.ExecuteScriptAsync(scriptInsertOnPos(html));
            docController.setHtml(await getHtmlFromWebView2());
        }

        private async void действияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String html = docController.createListInput(DocumentController.usingCRUD.operations);
            await webView21.CoreWebView2.ExecuteScriptAsync(scriptInsertOnPos(html));
            docController.setHtml(await getHtmlFromWebView2());
        }

        private async void заказыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String html = docController.createListInput(DocumentController.usingCRUD.orders);
            await webView21.CoreWebView2.ExecuteScriptAsync(scriptInsertOnPos(html));
            docController.setHtml(await getHtmlFromWebView2());
        }

        private async void товарыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String html = docController.createListInput(DocumentController.usingCRUD.products);
            await webView21.CoreWebView2.ExecuteScriptAsync(scriptInsertOnPos(html));
            docController.setHtml(await getHtmlFromWebView2());
        }

        private async void контрагентыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String html = docController.createListInput(DocumentController.usingCRUD.counteragents);
            await webView21.CoreWebView2.ExecuteScriptAsync(scriptInsertOnPos(html));
            docController.setHtml(await getHtmlFromWebView2());
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ChooseFileFromBase frm = new ChooseFileFromBase(_connection);
            if (DialogResult.OK == frm.ShowDialog())
            {
                Template template = frm.chosenTemplate;
                if (template != null)
                {
                    string htmlContent = Encoding.UTF8.GetString(template.Content);
                    docController.setHtml(htmlContent);
                    docController.createAllTemplateObjects();
                    webView21.CoreWebView2.NavigateToString(docController.getHtml());
                    docController.getAllElementsFromHtml("template2003");
                }
            }
            
        }

    }
}
