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
        List<DocumentController.elemToCreate> listFromAbove;
        String listClassChoice;
        public DocumentController.elemToCreate currentElement;
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
            currentElement.is_filled = false;
            currentElement.value = "null";
            comboBox2.Items.Clear();
            comboBox2.SelectedIndex = -1;
            comboBox2.Enabled = true;

            var listNameChoice = Convert.ToString(comboBox1.Items[comboBox1.SelectedIndex]);
            foreach (var element in listFromAbove)
            {
                if (element.name_element == listNameChoice)
                {
                    listClassChoice = element.className;
                    break;
                }
            }
            this.currentElement.name_to_connect_element = listNameChoice;
            comboBox2.Items.AddRange(getFieldsOfClass(listClassChoice));

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

        private String[] getFieldsOfClass(String className)
        {
            List<String> tmpArr = new List<string>();
            switch (className)
            {
                case "User":
                    {
                        tmpArr.AddRange(["Должность человека", "Номер рабочего места"]);
                        break;
                    }
                case "Position":
                    {
                        tmpArr.AddRange(["Сектор должности", "Отдел должности"]);
                        break;
                    }
                case "Operation":
                    {
                        tmpArr.AddRange(["Должность, которая это может выполнить", "Название действия", "Описание действия"]);
                        break;
                    }
                case "Product":
                    {
                        tmpArr.AddRange(["Описание товара", "Цена товара"]);
                        break;
                    }
                case "Counteragent":
                    {
                        tmpArr.AddRange(["Контрагент"]);
                        break;
                    }
                case "Order":
                    {
                        tmpArr.AddRange(["Номер заказа", "Контрагент", "Дата заказа", "Дата доставки", "Комментарий к заказу"]);
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
                case "Должность человека":
                    {
                        this.currentElement.className = "User";
                        this.currentElement.show_field = "IdPosition";
                        break;
                    }
                case "Номер рабочего места":
                    {
                        this.currentElement.className = "User";
                        this.currentElement.show_field = "Place";
                        break;
                    }
                case "Сектор должности":
                    {
                        this.currentElement.className = "Position";
                        this.currentElement.show_field = "Sector";
                        break;
                    }
                case "Отдел должности":
                    {
                        this.currentElement.className = "Position";
                        this.currentElement.show_field = "Department";
                        break;
                    }
                case "Должность, которая это может выполнить":
                    {
                        this.currentElement.className = "Operation";
                        this.currentElement.show_field = "IdPosition";
                        break;
                    }
                case "Название действия":
                    {
                        this.currentElement.className = "Operation";
                        this.currentElement.show_field = "Name";
                        break;
                    }
                case "Описание действия":
                    {
                        this.currentElement.className = "Operation";
                        this.currentElement.show_field = "Description";
                        break;
                    }
                case "Описание товара":
                    {
                        this.currentElement.className = "Product";
                        this.currentElement.show_field = "Description";
                        break;
                    }
                case "Цена товара":
                    {
                        this.currentElement.className = "Product";
                        this.currentElement.show_field = "Price";
                        break;
                    }
                case "Контрагент":
                    {
                        this.currentElement.className = "Order";
                        this.currentElement.show_field = "IdCounteragent";
                        break;
                    }
                case "Номер заказа":
                    {
                        this.currentElement.className = "Order";
                        this.currentElement.show_field = "Number";
                        break;
                    }
                case "Дата заказа":
                    {
                        this.currentElement.className = "Order";
                        this.currentElement.show_field = "OrderDate";
                        break;
                    }
                case "Дата доставки":
                    {
                        this.currentElement.className = "Order";
                        this.currentElement.show_field = "DeliveryDate";
                        break;
                    }
                case "Комментарий к заказу":
                    {
                        this.currentElement.className = "Order";
                        this.currentElement.show_field = "Comment";
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }
    }
}
