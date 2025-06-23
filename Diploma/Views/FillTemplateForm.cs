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

namespace Diploma.Views
{
    public partial class FillTemplateForm : Form
    {
        private String _connection;
        private DocumentController docController;
        private Diploma.Controllers.MyAppContext localContext;
        private Diploma.Models.User enteredUser;

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
            await webView21.EnsureCoreWebView2Async();
            this.webView21.CoreWebView2.Settings.IsScriptEnabled = true;
            webView21.CoreWebView2.Settings.IsWebMessageEnabled = true;
            this.webView21.CoreWebView2.WebMessageReceived += onAnswerFromWeb; //подписка на событие об ответе с webview2 о выборе элемента в списке

        }

        private async void webView21_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {

            await webView21.ExecuteScriptAsync("document.body.contentEditable = 'false';");
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }
    }
}
