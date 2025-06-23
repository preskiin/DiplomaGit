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
using Diploma.Controllers;

namespace Diploma.Views
{
    public partial class FormChooseList : Form
    {
        List<DocumentController.elemToCreate> listFromAbove;//коллекция элементов, к которым можно подключиться
        //String connectedTable;//значение названия таблицы, к которой привязан элемент, к которому привязывается создаваемый нами элемент
        public DocumentController.elemToCreate currentElement;//объект создаваемого шаблона
        public FormChooseList()
        {
            InitializeComponent();
        }

        public FormChooseList(List<DocumentController.elemToCreate> list)
        {
            InitializeComponent();
            listFromAbove = list;
            foreach (var elem in listFromAbove)
            {
                comboBox1.Items.Add(elem.name_element);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentElement= new DocumentController.elemToCreate();
            currentElement.type_element = "bound-field";
            currentElement.is_filled = false;
            currentElement.value = "null";
            //connectedTable = "";
            //очищаем второй список от прошлых значений
            comboBox2.Items.Clear();
            comboBox2.SelectedIndex = -1;
            comboBox2.Enabled = true;

            var listNameChoice = Convert.ToString(comboBox1.Items[comboBox1.SelectedIndex]);
            foreach (var element in listFromAbove)
            {
                if (element.name_element == listNameChoice)
                {
                    currentElement.need_table = element.current_table;
                    break;
                }
            }
            if (currentElement.need_table != "" && currentElement.need_table!=null)
            {
                //проверить на работоспособность при отсутствии выбранного значения
                this.currentElement.name_to_connect_element = listNameChoice;
                comboBox2.Items.AddRange(getFieldsForChoose(currentElement.need_table));
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private String[] getFieldsForChoose(String _connectedTable)
        {
            List<String> tmpArr = new List<string>();
            switch (_connectedTable)
            {
                case "People":
                    {
                        tmpArr.AddRange(["Должность человека", "Номер рабочего места", "Сектор человека", "Отдел человека"]);
                        break;
                    }
                case "Positions":
                    {
                        tmpArr.AddRange(["Сектор должности", "Отдел должности"]);
                        break;
                    }
                case "Operations":
                    {
                        tmpArr.AddRange(["Должность, которая это может выполнить", "Название действия", "Описание действия"]);
                        break;
                    }
                case "Products":
                    {
                        tmpArr.AddRange(["Описание товара", "Цена товара"]);
                        break;
                    }
                case "Counteragents":
                    {
                        tmpArr.AddRange(["Контрагент"]);
                        break;
                    }
                case "Orders":
                    {
                        tmpArr.AddRange(["Номер заказа", "Контрагент заказа", "Дата заказа", "Дата доставки", "Комментарий к заказу"]);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            if (tmpArr.Count != 0)
                return tmpArr.ToArray();
            else return null;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            var showNameChoice = Convert.ToString(comboBox2.Items[comboBox2.SelectedIndex]);
            switch (showNameChoice)
            {
                //варианты от списка людей
                case "Должность человека":
                    {
                        currentElement.need_field = "IdPosition";
                        currentElement.current_field = "Name";
                        currentElement.current_table = "Positions";
                        break;
                    }
                case "Номер рабочего места":
                    {
                        currentElement.need_field = "Place";
                        currentElement.current_field = "Place";
                        currentElement.current_table = "People";
                        break;
                    }
                case "Сектор человека":
                    {
                        currentElement.need_field = "IdPosition";
                        currentElement.current_field = "Sector";
                        currentElement.current_table = "Positions";
                        break;
                    }
                case "Отдел человека":
                    {
                        currentElement.need_field = "IdPosition";
                        currentElement.current_field = "Department";
                        currentElement.current_table = "Positions";
                        break;
                    }
                //варианты списка должностей
                case "Сектор должности":
                    {
                        currentElement.need_field = "Sector";
                        currentElement.current_field = "Sector";
                        currentElement.current_table = "Positions";
                        break;
                    }
                case "Отдел должности":
                    {
                        currentElement.need_field = "Department";
                        currentElement.current_field = "Department";
                        currentElement.current_table = "Positions";
                        break;
                    }
                    //варианты операций
                case "Должность, которая это может выполнить":
                    {
                        currentElement.need_field = "IdPosition";
                        currentElement.current_field = "Name";
                        currentElement.current_table = "Positions";
                        break;
                    }
                case "Название действия":
                    {
                        currentElement.need_field = "Name";
                        currentElement.current_field = "Name";
                        currentElement.current_table = "Operations";
                        break;
                    }
                case "Описание действия":
                    {
                        currentElement.need_field = "Description";
                        currentElement.current_field = "Description";
                        currentElement.current_table = "Operations";
                        break;
                    }
                    //варианты товара
                case "Описание товара":
                    {
                        currentElement.need_field = "Description";
                        currentElement.current_field = "Description";
                        currentElement.current_table = "Products";
                        break;
                    }
                case "Цена товара":
                    {

                        currentElement.need_field = "Price";
                        currentElement.current_field = "Price";
                        currentElement.current_table = "Products";
                        break;
                    }
                    //варианты контрагента???
                case "Контрагент":
                    {
                        currentElement.need_field = "Name";
                        currentElement.current_field = "Name";
                        currentElement.current_table = "Counteragents";
                        break;
                    }
                    //варианты заказа
                case "Номер заказа":
                    {
                        currentElement.need_field = "Number";
                        currentElement.current_field = "Number";
                        currentElement.current_table = "Orders";
                        break;
                    }
                case "Контрагент заказа":
                    {
                        currentElement.need_field = "IdCounteragent";
                        currentElement.current_field = "Name";
                        currentElement.current_table = "Counteragents";
                        break;
                    }
                case "Дата заказа":
                    {
                        currentElement.need_field = "OrderDate";
                        currentElement.current_field = "OrderDate";
                        currentElement.current_table = "Orders";
                        break;
                    }
                case "Дата доставки":
                    {
                        currentElement.need_field = "DeliveryDate";
                        currentElement.current_field = "DeliveryDate";
                        currentElement.current_table = "Orders";
                        break;
                    }
                case "Комментарий к заказу":
                    {
                        currentElement.need_field = "Comment";
                        currentElement.current_field = "Comment";
                        currentElement.current_table = "Orders";
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        private void FormChooseList_Load(object sender, EventArgs e)
        {

        }
    }
}
