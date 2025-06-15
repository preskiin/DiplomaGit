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
            public string name_element;//название элемента
            public string show_field;//Отображаемое значение (первое слово - таблица, в которой лежит искомое значение; второе слово - поле в таблице с искомым значением)
            public string name_to_connect_element; // название элемента, к которому привязан (если не привязан - null, если является начальным списком - base)
            public bool is_filled; //готов этот элемент к формированию документ или не готов
            public string value;//значение, которые присвоено этому элементу. У List это ID, у привязанных полей - значение, которое берется из запроса в базу, у простых текстовых - значения в тексте 
            public string className;
            public string anotherTableField;
        }

        private String _connection;
        private String htmlCode;
        private int counter = 0;

        public List<elemToCreate> elements;
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
                //getElementsFromHtml("template2003");
                //this.counter = this.elements.Count;
                //if (putStringAfter("<meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">", "<head>") != -1)
                return true;
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

        public void getElementsFromHtml(String classToFind)
        {
            this.elements = new List<elemToCreate>();
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
                    elem.anotherTableField = node.Attributes["data-another-table-field"].Value;
                    this.elements.Add(elem);
                }
                this.counter = this.elements.Count;
            }
        }

        public String createListInput(usingCRUD dataNeeded)
        {
            String htmlString = "";
            switch (dataNeeded)
            {
                case usingCRUD.positions:
                    {

                        //this.counter++;
                        //htmlString = CRUD_Positions.generatePositionsDropdown(this._connection);
                        break;
                    }
                case usingCRUD.operations:
                    {

                        //this.counter++;
                        //htmlString = CRUD_Operations.generateOperationsDropdown(this._connection);
                        break;
                    }
                case usingCRUD.people:
                    {

                        //this.counter++;
                        //htmlString = CRUD_Users.generateUsersDropdown(connectionString: this._connection, counter+1);
                        break;
                    }
                case usingCRUD.counteragents:
                    {

                        //this.counter++;
                        //htmlString = CRUD_Counteragents.generateCounteragentsDropdown(this._connection);
                        break;
                    }
                case usingCRUD.products:
                    {
                        //this.counter++;
                        //htmlString = CRUD_Products.generateProductsDropdown(this._connection);
                        break;
                    }
                default:
                    {
                        htmlString = "";
                        break;
                    }
            }
            elemToCreate tmpToAdd = findTemplateInHtml(htmlString, "template2003");
            if (tmpToAdd.name_element != null)
            {
                counter++;
                elements.Add(tmpToAdd);
            }
            return htmlString;
        }

        private elemToCreate findTemplateInHtml(String html, String templateToFind)
        {
            elemToCreate tmpElem = new elemToCreate();
            var htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(html);
            // Ищем все элементы с классом template2003
            var nodes = htmlDoc.DocumentNode.SelectNodes($"//*[contains(@class, '{templateToFind}')]");
            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    tmpElem.name_element = node.Attributes["data-name-element"].Value;
                    tmpElem.show_field = node.Attributes["data-show-field"].Value;
                    tmpElem.name_to_connect_element = node.Attributes["data-name-to-connect"].Value;
                    tmpElem.is_filled = Convert.ToBoolean(node.Attributes["data-is-filled"].Value);
                    tmpElem.value = node.Attributes["data-value"].Value;
                    tmpElem.className = node.Attributes["data-class-name"].Value;
                    tmpElem.anotherTableField = node.Attributes["data-another-table-field"].Value;
                }
            }
            return tmpElem;
           
        }

        public String createBoundField(elemToCreate element)
        {
            counter++;
            element.name_element = "element"+Convert.ToString(this.counter);
            StringBuilder html = new StringBuilder();
            html.AppendLine(@$"<input type='text' class='template2003' 
                data-name-element='{element.name_element}'
                data-show-field={element.show_field}
                data-name-to-connect={element.name_to_connect_element}
                data-is-filled={element.is_filled}
                data-value={element.value}
                data-class-name='{element.className}'
                data-another-table-field='{element.anotherTableField}'placeholder='--{element.name_element}--' readonly>
                ");
            return html.ToString();
        }

        public void updateListElements(elemToCreate updated_element)
        {
            int count = 0;
            foreach (var elem in elements)
            {
                if (elem.name_element == updated_element.name_element)
                {
                    break;
                }
                count++;
            }
            elements[count] = updated_element;
            updateBoundFieldElements(elements[count].name_element);
        }

        public void updateBoundFieldElements(String listName)
        {
            List<elemToCreate> elementsToUpdate = new List<elemToCreate>();
            foreach (var elem in elements)
            {
                if (elem.name_to_connect_element == listName)
                {
                    elementsToUpdate.Add(elem);
                }
            }
            foreach (var element in elementsToUpdate)
            {
                
            }
        }
    }

}
