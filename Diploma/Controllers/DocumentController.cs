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
            products,
            order,
            orderItems,
        }

        public struct elemToCreate
        {
            public string name_element;
            public string name_to_connect_element;
            public string need_field;
            public string need_table;
            public string current_field;
            public string current_table;
            public string type_element;
            public string value;
            public bool is_filled;
        }

        private String _connection;
        private String htmlCode;
        private int counter = 0;

        public List<elemToCreate> elements;
       

        public DocumentController(String con)
        {
            _connection = con;
        }

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
        public void saveHtml(string htmlString)
        {
            if (this.htmlCode != null)
            {
                this.htmlCode =  htmlString;
                File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)+"\\MyHtml.html", this.htmlCode);
            }
        }

        public void setHtml(String htmlString)
        {
            this.htmlCode=htmlString;
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
                    elem.name_to_connect_element = node.Attributes["data-name-to-connect"].Value;
                    elem.need_field = node.Attributes["data-need-field"].Value;
                    elem.need_table = node.Attributes["data-need-table"].Value;
                    elem.current_field = node.Attributes["data-current-field"].Value;
                    elem.current_table = node.Attributes["data-current-table"].Value;
                    elem.value = node.Attributes["data-value"].Value;
                    elem.is_filled = Convert.ToBoolean(node.Attributes["data-is-filled"].Value);
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
                        htmlString = CRUD_Users.generateUsersDropdown(connectionString: this._connection, counter+1);
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
                    tmpElem.name_to_connect_element = node.Attributes["data-name-to-connect"].Value;
                    tmpElem.need_field = node.Attributes["data-need-field"].Value;
                    tmpElem.need_table = node.Attributes["data-need-table"].Value;
                    tmpElem.current_field = node.Attributes["data-current-field"].Value;
                    tmpElem.current_table = node.Attributes["data-current-field"].Value;
                    tmpElem.type_element = node.Attributes["data-type-element"].Value;
                    tmpElem.value = node.Attributes["data-value"].Value;
                    tmpElem.is_filled = Convert.ToBoolean(node.Attributes["data-is-filled"].Value);
                }
            }
            return tmpElem;
           
        }

        public String createBoundField(elemToCreate element)
        {
            //counter++;
            element.name_element = "element"+Convert.ToString(this.counter);
            StringBuilder html = new StringBuilder();
            html.AppendLine(@$"<input type='text' class='template2003' 
                data-name-element='{element.name_element}'
                data-name-to-connect={element.name_to_connect_element}
                data-need-field={element.need_field}
                data-need-table={element.need_table}
                data-current-field={element.current_field}
                data-current-table={element.current_table}
                data-type-element={element.type_element}
                data-value={element.value}
                data-is-filled={element.is_filled}
                'placeholder='--{element.name_element}--' readonly>
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
