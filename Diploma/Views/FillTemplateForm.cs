using Diploma.Controllers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Diploma.Models;
using System.Runtime.CompilerServices;
using Aspose.Words;
using Aspose.Words.Saving;
using System.Diagnostics;
using System.Net.Http;

namespace Diploma.Views
{
    public partial class FillTemplateForm : Form
    {
        private Int64 chosenIdTemplate = -1;
        private String _connection;
        private DocumentController docController;
        private Diploma.Controllers.MyAppContext localContext;
        private Diploma.Models.User enteredUser;
        private String htmlCode = "";
        public FillTemplateForm()
        {
            InitializeComponent();
        }

        public FillTemplateForm(Diploma.Controllers.MyAppContext context, Diploma.Models.User user, String connection)
        {
            InitializeComponent();
            localContext = context;
            _connection = connection;
            docController = new DocumentController(_connection);
            enteredUser = user;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ChooseFileFromBase frm = new ChooseFileFromBase(_connection);
            if (DialogResult.OK == frm.ShowDialog())
            {
                Template template = frm.chosenTemplate;
                chosenIdTemplate = template.Id;
                if (template != null)
                {
                    string htmlContent = Encoding.UTF8.GetString(template.Content);
                    docController.setHtml(htmlContent);
                    docController.createAllTemplateObjects();
                    webView21.CoreWebView2.NavigateToString(docController.getHtml());
                }
            }
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

        private async void FillTemplateForm_Load(object sender, EventArgs e)
        {
            try
            {
                if (webView21.CoreWebView2 == null)
                {
                    await webView21.EnsureCoreWebView2Async();
                }
            this.webView21.CoreWebView2.Settings.IsScriptEnabled = true;
            webView21.CoreWebView2.Settings.IsWebMessageEnabled = true;
            this.webView21.CoreWebView2.WebMessageReceived += onAnswerFromWeb; //подписка на событие об ответе с webview2 о выборе элемента в списке
            }
            catch (Exception ex)
            {
                MessageBox.Show($"WebView2 initialization failed: {ex.Message}");
            }
        }
        //создает строку html из страницы, которая отображена сейчас в webView2
        public async Task<string> getHtmlFromWebView2()
        {
            try
            {
                if (webView21?.CoreWebView2 == null)
                    return string.Empty;
                // Скрипт для замены всех полей на их значения (без шаблонных литералов)
                string script = @"
            // Создаем копию документа
            var docClone = document.documentElement.cloneNode(true);
            
            // Заменяем все видимые input-поля на их значения
            var inputs = docClone.querySelectorAll('input:not([type=hidden])');
            for (var i = 0; i < inputs.length; i++) {
                var input = inputs[i];
                
                // Пропускаем кнопки и скрытые поля
                if (input.type === 'hidden' || input.type === 'button') continue;
                
                var value = input.value || '[не заполнено]';
                var span = document.createElement('span');
                span.textContent = value;
                input.parentNode.replaceChild(span, input);
            }
            
            // Удаляем все datalist и скрытые поля
            var elementsToRemove = docClone.querySelectorAll('datalist, input[type=hidden]');
            for (var j = 0; j < elementsToRemove.length; j++) {
                var element = elementsToRemove[j];
                if (element.parentNode) {
                    element.parentNode.removeChild(element);
                }
            }
            
            // Возвращаем модифицированный HTML
            docClone.outerHTML;";

                string encodedHtml = await webView21.CoreWebView2.ExecuteScriptAsync(script);
                string cleanHtml = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(encodedHtml);
                return cleanHtml;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка при получении HTML: {ex.Message}");
                return null;
            }
        }
        private async void webView21_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {

            await webView21.ExecuteScriptAsync("document.body.contentEditable = 'false';");
            this.htmlCode = await getHtmlFromWebView2();
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                button3.Enabled = false;
                string htmlContent = await GetSafeHtmlFromWebView2();

                if (!string.IsNullOrEmpty(htmlContent))
                {
                    using (SaveFileDialog saveDialog = new SaveFileDialog())
                    {
                        saveDialog.Filter = "Word Documents|*.docx";
                        saveDialog.RestoreDirectory = true;
                        saveDialog.OverwritePrompt = true;

                        if (saveDialog.ShowDialog() == DialogResult.OK)
                        {
                            await Task.Run(() =>
                            {
                                var converter = new HtmlToWordConverter(_connection);
                                converter.ConvertHtmlStringToWord(htmlContent, saveDialog.FileName);
                            });

                            MessageBox.Show("Документ сохранен успешно!");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Не удалось получить содержимое страницы");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
                // Логирование ошибки
                Debug.WriteLine($"Save error: {ex}");
            }
            finally
            {
                Cursor = Cursors.Default;
                button3.Enabled = true;
            }
            //string htmlContent = await getHtmlFromWebView2();

            //if (!string.IsNullOrEmpty(htmlContent))
            //{
            //    using (SaveFileDialog saveDialog = new SaveFileDialog())
            //    {
            //        saveDialog.Filter = "Word Documents|*.docx";

            //        if (saveDialog.ShowDialog() == DialogResult.OK)
            //        {
            //            // Используем класс HtmlToWordConverter с передачей подключения
            //            var converter = new HtmlToWordConverter(_connection);
            //            converter.ConvertHtmlStringToWord(htmlContent, saveDialog.FileName);
            //            MessageBox.Show("Документ сохранен успешно!");
            //        }
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("Не удалось получить содержимое страницы");
            //}
        }

        public async Task<string> GetSafeHtmlFromWebView2()
        {
            if (webView21.InvokeRequired)
            {
                return (string)webView21.Invoke(new Func<Task<string>>(async () => await getHtmlFromWebView2()));
            }
            else
            {
                return await getHtmlFromWebView2();
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            localContext.SwitchMainForm(new Diploma.Views.MainMenuForm(localContext, enteredUser, _connection));
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                CRUD_Documents cRUD_docs = new CRUD_Documents(_connection);
                string html = await getHtmlFromWebView2();
                var converter = new HtmlToWordConverter(_connection);
                Diploma.Models.Document document = new Diploma.Models.Document(0, textBox1.Text, converter.CreateByteDocumentForBase(html), chosenIdTemplate);
                if (cRUD_docs.create(document) != -1)
                {
                    MessageBox.Show("Документ успешно сохранен в базу", "Информация", MessageBoxButtons.OK);
                }
                else
                {
                    MessageBox.Show("Документ не удалось сохранить. Проверьте, не пытаетесь ли вы сохранить пустой документ и корректное ли введено имя", "Информация", MessageBoxButtons.OK);
                }
            }
            else { MessageBox.Show("Название документа не должно быть пустым!", "Внимание!", MessageBoxButtons.OK); }
        }
    }
}
