using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics.Eventing.Reader;
using Aspose.Words;
using Aspose.Words.Saving;
using Diploma.Controllers;
using Diploma.Models;
using System.Data.SqlClient;
using HtmlAgilityPack;

namespace Diploma.Controllers
{
    public class DocumentController
    {
        public enum usingCRUD
        {
            people,
            positions,
            operations,
            counteragents,
            products
        }

        public struct elemToCreate
        {
            public string name_element;
            public string show_field;
            public string name_to_connect_element;
            public bool is_filled;
            public string value;
            public string className;
        }

        private String _connection;
        private String htmlCode;
        //private String text;

        public DocumentController(String con)
        {
            _connection = con;
        }


        //public void loadDocument()
        //{


        //}

        //с помощью Aspose.Words формирует html-код страницу
        public bool docxToHtml(string docxPath)
        {
            if (System.IO.File.Exists(docxPath))
            {
                var doc = new Aspose.Words.Document(docxPath);
                MemoryStream stream = new MemoryStream();
                var options = new HtmlSaveOptions()
                {
                    Encoding = Encoding.UTF8,
                    ExportImagesAsBase64 = true,
                };
                doc.Save(stream, options);
                stream.Position = 0;

                using (StreamReader reader = new StreamReader(stream))
                {
                    this.htmlCode = reader.ReadToEnd();
                }
                stream.Close();
                cleanFromWatermarks();
                if (putStringAfter("<meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">", "<head>") != -1)
                    return true;
                else return false;
            }
            else
                return false;
        }
        
        //удаляет упоминания библиотеки в документе html
        private void cleanFromWatermarks()
        {
            String tmpStr = this.htmlCode;
            int start_rem;
            for (int i = 0; i < 10; i++)
            {
                if (tmpStr.IndexOf("<div style=") != -1)
                {
                    start_rem = tmpStr.IndexOf("<div style=");
                    tmpStr = tmpStr.Remove(start_rem, getDivClosePosition(start_rem, tmpStr)-start_rem);
                }
                else
                    break;
            }
            tmpStr = removeFirstLastP(tmpStr);
            this.htmlCode = tmpStr;
        }

        //Определяет положение </div>, который закроет строку, перед которой начали форматирование
        private Int32 getDivClosePosition(Int32 start_pos, String allStr)
        {
            int result = 1;
            int tmp_position = start_pos+4;
            for (int i=0; i<10; i++)
            {
                if (result == 0)
                    break;
                if (allStr.IndexOf("<div", tmp_position)<allStr.IndexOf("</div>", tmp_position)&&allStr.IndexOf("<div", tmp_position)!=-1)
                {
                    result++;
                    tmp_position = allStr.IndexOf("<div", tmp_position) + 4;
                }
                else
                {
                    result--;
                    tmp_position = allStr.IndexOf("</div>", tmp_position) + 6;
                }
            }
            return tmp_position;
        }

        //Удаляет упоминания библиотеки сверху с снизу документа.
        private String removeFirstLastP(String strToClean)
        {
            String tmpStr = strToClean;
            tmpStr = tmpStr.Remove(tmpStr.IndexOf("<p"), tmpStr.IndexOf("</p>")+4-tmpStr.IndexOf("<p"));
            tmpStr = tmpStr.Remove(tmpStr.LastIndexOf("<p"), tmpStr.LastIndexOf("</p>") + 4- tmpStr.LastIndexOf("<p"));
            return tmpStr;
        }

        //Возвращает строку html-кода
        public String getHtml()
        {
            if (htmlCode != null)
            {
                return this.htmlCode;
            }
            else
            {
                return null;
            }
        }

        //засовывает строку после другой строки в html-код страницы
        private int putStringAfter(String putString, String afterString)
        {
            if (this.htmlCode.IndexOf(afterString) != -1)
            {
                Int32 stringPosition = this.htmlCode.IndexOf(afterString) + afterString.Length;
                String tmpString = this.htmlCode.Insert(stringPosition, putString);
                this.htmlCode = tmpString;
                return 1;
            }
            else return -1;
            
        }

        //сохраняет html-код на рабочий стол
        public async void saveHtml(Microsoft.Web.WebView2.WinForms.WebView2 webView)
        {
            if (this.htmlCode != null)
            {
                this.htmlCode =  await getHtmlFromWebView2(webView);
                File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)+"\\MyHtml.html", this.htmlCode);
            }
        }

        //создает строку html из страницы, которая отображена сейчас в webView2
        public async Task<string> getHtmlFromWebView2(Microsoft.Web.WebView2.WinForms.WebView2 webView)
        {
            try
            {
                // Получаем HTML с помощью JavaScript
                string encodedHtml = await webView.CoreWebView2.ExecuteScriptAsync(
                    "document.documentElement.outerHTML;"
                );

                // Декодируем JSON-строку (удаляем кавычки и экранированные символы)
                string cleanHtml = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(encodedHtml);
                this.htmlCode = cleanHtml;
                return cleanHtml;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении HTML: {ex.Message}");
                return null;
            }
        }

        //public void addListInput(Int32 textPos)
        //{
        //    this.htmlCode = this.htmlCode.Insert(textPos, CRUD_Positions.generatePositionsDropdown(this._connection));
        //}
        public List<elemToCreate> getElementsFromHtml(String classToFind)
        {
            List<elemToCreate> elements = new List<elemToCreate>();
            var htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(this.htmlCode);
            // Ищем все элементы с классом template2003
            var nodes = htmlDoc.DocumentNode.SelectNodes($"//*[contains(@class, '{classToFind}')]");
            if (nodes!=null)
            {
                foreach (var node in nodes)
                {
                    elemToCreate elem = new elemToCreate();
                    elem.name_element = node.Attributes["data-name-element"].Value;
                    elem.show_field = node.Attributes["data-show-field"].Value;
                    elem.name_to_connect_element = node.Attributes["data-name-to-connect"].Value;
                    elem.is_filled = Convert.ToBoolean(node.Attributes["data-is-filled"].Value);
                    elem.value = node.Attributes["data-value"].Value;
                    elem.className = node.Attributes["data-class-name"].Value;
                    elements.Add(elem);
                }
            }
            return elements;
        }

        public String createListInput(usingCRUD dataNeeded, int amountOfEls)
        {
            String htmlString = "";
            switch (dataNeeded)
            {
                case usingCRUD.positions:
                    {
                        htmlString = CRUD_Positions.generatePositionsDropdown(this._connection);
                        break;
                    }
                case usingCRUD.operations:
                    {
                        htmlString = CRUD_Operations.generateOperationsDropdown(this._connection);
                        break;
                    }
                case usingCRUD.people:
                    {
                        htmlString = CRUD_Users.generateUsersDropdown(connectionString:this._connection, amountOfEls+1);
                        break;
                    }
                case usingCRUD.counteragents:
                    {
                        //htmlString = CRUD_Counteragents.generateCounteragentsDropdown(this._connection);
                        break;
                    }
                case usingCRUD.products:
                    {
                        //htmlString = CRUD_Products.generateProductsDropdown(this._connection);
                        break;
                    }
                default:
                    {
                        htmlString = "";
                        break;
                    }
            }
            return htmlString;
        }
    }

}
