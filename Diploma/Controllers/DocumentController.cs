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
using DocumentFormat.OpenXml.Drawing.Diagrams;
using System.Data;
using System.Windows.Markup.Localizer;
using System.Runtime.CompilerServices;

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
            orders,
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
                        //htmlString = CRUD_Positions.generatePositionsDropdown(this._connection);
                        htmlString = createTemplateListPositions();
                        break;
                    }
                case usingCRUD.operations:
                    {
                        //htmlString = CRUD_Operations.generateOperationsDropdown(this._connection);
                        htmlString = createTemplateListOperations();
                        break;
                    }
                case usingCRUD.people:
                    {
                        //htmlString = CRUD_Users.generateUsersDropdown(connectionString: this._connection, counter+1);
                        htmlString = this.createTemplateListUsers();
                        break;
                    }
                case usingCRUD.counteragents:
                    {

                        //this.counter++;
                        //htmlString = CRUD_Counteragents.generateCounteragentsDropdown(this._connection);
                        htmlString = this.createTemplateListCounteragents();
                        break;
                    }
                case usingCRUD.products:
                    {
                        //this.counter++;
                        //htmlString = CRUD_Products.generateProductsDropdown(this._connection);
                        htmlString = this.createTemplateListProducts();
                        break;
                    }
                case usingCRUD.orders:
                    {
                        htmlString = this.createTemplateListOrders();
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
        ////не метод, а бред, не следует его использовать: ищет в строке элементы с указанным классом и возвращает ПЕРВЫЙ элемент из коллекции
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
                    break;//костыль!
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

        //Создает код шаблона списка пользователей
        public String createTemplateListUsers()
        {
            string new_name = "element"+Convert.ToString(this.counter + 1);
            string show_name = "СписокЛюдей"+ Convert.ToString(this.counter + 1);
            string htmlStr = @$"<div class='template2003' 
                data-name-element='{new_name}'
                data-name-to-connect=null
                data-need-field=null
                data-need-table=null
                data-current-field=Surname_Name_Patronymic
                data-current-table=People
                data-type-element=list
                data-value=null
                data-is-filled=false>{show_name}</div>";
            return htmlStr;
        }

        public String createTemplateListCounteragents()
        {
            string new_name = "element" + Convert.ToString(this.counter + 1);
            string show_name = "СписокКонтрагентов" + Convert.ToString(this.counter + 1);
            string htmlStr = @$"<div class='template2003' 
                data-name-element='{new_name}'
                data-name-to-connect=null
                data-need-field=null
                data-need-table=null
                data-current-field=Name
                data-current-table=Counteragents
                data-type-element=list
                data-value=null
                data-is-filled=false>{show_name}</div>";
            return htmlStr;
        }

        public String createTemplateListOperations()
        {
            string new_name = "element" + Convert.ToString(this.counter + 1);
            string show_name = "СписокДействий" + Convert.ToString(this.counter + 1);
            string htmlStr = @$"<div class='template2003' 
                data-name-element='{new_name}'
                data-name-to-connect=null
                data-need-field=null
                data-need-table=null
                data-current-field=Name
                data-current-table=Operations
                data-type-element=list
                data-value=null
                data-is-filled=false>{show_name}</div>";
            return htmlStr;
        }

        public String createTemplateListProducts()
        {
            string new_name = "element" + Convert.ToString(this.counter + 1);
            string show_name = "СписокТоваров" + Convert.ToString(this.counter + 1);
            string htmlStr = @$"<div class='template2003' 
                data-name-element='{new_name}'
                data-name-to-connect=null
                data-need-field=null
                data-need-table=null
                data-current-field=Name
                data-current-table=Products
                data-type-element=list
                data-value=null
                data-is-filled=false>{show_name}</div>";
            return htmlStr;
        }

        public String createTemplateListPositions()
        {
            string new_name = "element" + Convert.ToString(this.counter + 1);
            string show_name = "СписокДолжностей" + Convert.ToString(this.counter + 1);
            string htmlStr = @$"<div class='template2003' 
                data-name-element='{new_name}'
                data-name-to-connect=null
                data-need-field=null
                data-need-table=null
                data-current-field=Name
                data-current-table=Positions
                data-type-element=list
                data-value=null
                data-is-filled=false>{show_name}</div>";
            return htmlStr;
        }

        public String createTemplateListOrders()
        {
            string new_name = "element" + Convert.ToString(this.counter + 1);
            string show_name = "СписокЗаказов" + Convert.ToString(this.counter + 1);
            string htmlStr = @$"<div class='template2003' 
                data-name-element='{new_name}'
                data-name-to-connect=null
                data-need-field=null
                data-need-table=null
                data-current-field=Number
                data-current-table=Orders
                data-type-element=list
                data-value=null
                data-is-filled=false>{show_name}</div>";
            return htmlStr;
        }

        public String createHtmlForListUsers(elemToCreate instructElement)
        {
            var html = new StringBuilder();//создает строку кода html
            var users = CRUD_Users.readAllUsers(_connection);//получаем массив пользователей.
            html.AppendLine($"<input list='{instructElement.name_element}-list' name='{instructElement.name_element}' id='{instructElement.name_element}' value='' class='form-control' placeholder='-- {instructElement.name_element} --'>");
            html.AppendLine($"<datalist id='{instructElement.name_element}-list'>");
            foreach (DataRow user in users.Rows)
            {
                html.AppendLine($"<option value='{user["Surname"]+" " + user["Name"]+" " + user["Patronymic"]}' data-id='{user["id"]}'>");
            }

            html.AppendLine("</datalist>");
            html.AppendLine($"<input type='hidden' name='{instructElement.name_element}-id' id='{instructElement.name_element}-id' value=''>");
            return html.ToString();
        }

        public String createHtmlForListCounteragents(elemToCreate instructElement)
        {
            var html = new StringBuilder();//создает строку кода html
            var agents = CRUD_Counteragents.readAllCounteragents(_connection);//получаем массив агентов
            html.AppendLine($"<input list='{instructElement.name_element}-list' name='{instructElement.name_element}' id='{instructElement.name_element}' value='' class='form-control' placeholder='-- {instructElement.name_element} --'>");
            html.AppendLine($"<datalist id='{instructElement.name_element}-list'>");
            foreach (DataRow agent in agents.Rows)
            {
                html.AppendLine($"<option value='{agent["Name"]}' data-id='{agent["id"]}'>");
            }

            html.AppendLine("</datalist>");
            html.AppendLine($"<input type='hidden' name='{instructElement.name_element}-id' id='{instructElement.name_element}-id' value=''>");
            return html.ToString();
        }

        public String createHtmlForListOperations(elemToCreate instructElement)
        {
            var html = new StringBuilder();//создает строку кода html
            var operations = CRUD_Operations.readAllOperations(_connection);//получаем массив действий
            html.AppendLine($"<input list='{instructElement.name_element}-list' name='{instructElement.name_element}' id='{instructElement.name_element}' value='' class='form-control' placeholder='-- {instructElement.name_element} --'>");
            html.AppendLine($"<datalist id='{instructElement.name_element}-list'>");
            foreach (DataRow operation in operations.Rows)
            {
                html.AppendLine($"<option value='{operation["Name"]}' data-id='{operation["id"]}'>");
            }

            html.AppendLine("</datalist>");
            html.AppendLine($"<input type='hidden' name='{instructElement.name_element}-id' id='{instructElement.name_element}-id' value=''>");
            return html.ToString();
        }

        public String createHtmlForListOrders(elemToCreate instructElement)
        {
            var html = new StringBuilder();//создает строку кода html
            var orders = CRUD_Orders.readAllOrders(_connection);//получаем массив заказов
            html.AppendLine($"<input list='{instructElement.name_element}-list' name='{instructElement.name_element}' id='{instructElement.name_element}' value='' class='form-control' placeholder='-- {instructElement.name_element} --'>");
            html.AppendLine($"<datalist id='{instructElement.name_element}-list'>");
            foreach (DataRow order in orders.Rows)
            {
                html.AppendLine($"<option value='{order["Number"]}' data-id='{order["id"]}'>");
            }

            html.AppendLine("</datalist>");
            html.AppendLine($"<input type='hidden' name='{instructElement.name_element}-id' id='{instructElement.name_element}-id' value=''>");
            return html.ToString();
        }

        public String createHtmlForListPositions(elemToCreate instructElement)
        {
            var html = new StringBuilder();//создает строку кода html
            var positions = CRUD_Positions.readAllPositions(_connection);//получаем массив должностей
            html.AppendLine($"<input list='{instructElement.name_element}-list' name='{instructElement.name_element}' id='{instructElement.name_element}' value='' class='form-control' placeholder='-- {instructElement.name_element} --'>");
            html.AppendLine($"<datalist id='{instructElement.name_element}-list'>");
            foreach (DataRow position in positions.Rows)
            {
                html.AppendLine($"<option value='{position["Name"]}' data-id='{position["id"]}'>");
            }

            html.AppendLine("</datalist>");
            html.AppendLine($"<input type='hidden' name='{instructElement.name_element}-id' id='{instructElement.name_element}-id' value=''>");
            return html.ToString();
        }

        public String createHtmlForListProducts(elemToCreate instructElement)
        {
            var html = new StringBuilder();//создает строку кода html
            var products = CRUD_Products.readAllProducts(_connection);//получаем массив должностей
            html.AppendLine($"<input list='{instructElement.name_element}-list' name='{instructElement.name_element}' id='{instructElement.name_element}' value='' class='form-control' placeholder='-- {instructElement.name_element} --'>");
            html.AppendLine($"<datalist id='{instructElement.name_element}-list'>");
            foreach (DataRow product in products.Rows)
            {
                html.AppendLine($"<option value='{product["Name"]}' data-id='{product["id"]}'>");
            }

            html.AppendLine("</datalist>");
            html.AppendLine($"<input type='hidden' name='{instructElement.name_element}-id' id='{instructElement.name_element}-id' value=''>");
            return html.ToString();
        }

        //возвращает elemToCreate с указанным именем из переданной коллекции
        private elemToCreate findElementByName(String name, List<elemToCreate> elements)
        {
            elemToCreate tmp_elem= new();
            foreach (elemToCreate elem in elements)
            {
                if (elem.name_element == name)
                {
                    tmp_elem = elem;
                    break;
                }
            }
            return tmp_elem;
        }

        //генерирует код выпадающего списка (созданного), в зависимости от current_table в объекте tmpElem
        private String baseTemplateList(elemToCreate tmpElem)
        {
            String tmpStr = "";
            switch (tmpElem.current_table)
            {
                case "People":
                    {
                        tmpStr = createHtmlForListUsers(tmpElem);
                        break;
                    }
                case "Counteragents":
                    {
                        tmpStr = createHtmlForListCounteragents(tmpElem);
                        break;
                    }
                case "Operations":
                    {
                        tmpStr = createHtmlForListOperations(tmpElem);
                        break; 
                    }
                case "Orders":
                    {
                        tmpStr = createHtmlForListOrders(tmpElem);
                        break;
                    }
                case "Positions":
                    {
                        tmpStr = createHtmlForListPositions(tmpElem);
                        break;
                    }
                case "Products":
                    {
                        tmpStr = createHtmlForListProducts(tmpElem);
                        break;
                    }
                default:
                    break;
            }
            return tmpStr;
        }

        //Возвращает коллекцию объектов elemToCreate, содержащую в себе данные(атрибуты) всех встреченных объектов с классом templateToFind в строке html
        private List<elemToCreate> findAllTemplatesInHtml(String html, String templateToFind)
        {
            List<elemToCreate> elements = new List<elemToCreate>();
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
                    tmpElem.current_table = node.Attributes["data-current-table"].Value;
                    tmpElem.type_element = node.Attributes["data-type-element"].Value;
                    tmpElem.value = node.Attributes["data-value"].Value;
                    tmpElem.is_filled = Convert.ToBoolean(node.Attributes["data-is-filled"].Value);
                    elements.Add(tmpElem);
                }
            }
            return elements;
        }

        //Это функция загрузки шаблона. При открытии шаблона сперва данные из базы выгружаются в this.htmlCode, затем,
        //с помощью функции findAllTemplatesInHtml, собираются данные о коллекции шаблонных элементов существующем коде и создается коллекция elemToCreate, хранящая в себе атрибуты
        //всех объектов. Финальным этапом является прохождение всего this.htmlCode, удаление текста внутри <div></div> и вставка туда с помощью соответствующих createTemplateList. 
        //После этого строка this.htmlCode может быть присвоена в webView2.
        public void createAllTemplateObjects()
        {
            List<elemToCreate> elements = findAllTemplatesInHtml(this.htmlCode, "template2003");
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(this.htmlCode);
            var divs = doc.DocumentNode.SelectNodes("//div[contains(@class, 'template2003')]");
            if (divs != null)
            {
                foreach (var div in divs)
                {
                    //здесь нужно будет, скорее всего, вставить свитч, выбирающий, какой тип элемента находится в div (list или привязанный list или привязанной поле)
                    string nameElement = div.GetAttributeValue("data-name-element", "");
                    elemToCreate tmp = findElementByName(nameElement, elements);
                    switch (tmp.type_element)
                    {
                        case "list":
                            {
                                div.InnerHtml = baseTemplateList(tmp);
                                break;
                            }
                        case "bound-list":
                            {
                                break;
                            }
                        case "bound-field":
                            {
                                break;
                            }
                        default:
                            break;
                    }
                }
                this.htmlCode = doc.DocumentNode.OuterHtml;//присваиваем полученный текст в htmlCode
            }

        }
    }

}
