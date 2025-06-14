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
            comboBox2.Items.AddRange(getFieldsOfClass(listClassChoice));

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /*private*/ public String[] getFieldsOfClass(String className)
        {
            List<String> tmpArr = new List<string>();
            switch (className)
            {
                case "user":
                    {
                        tmpArr.AddRange(["Должность", "Номер рабочего места", "Рабочий сектор должности", "Отдел должности"]);
                        break;
                    }
                case "position":
                    {
                        tmpArr.AddRange(["Название должности", "Сектор должности", "Отдел должности"]);
                        break;
                    }
                case "operation":
                    {
                        tmpArr.AddRange(["Название действия", "Описание действия"]);
                        break;
                    }
                case "product":
                    {
                        tmpArr.AddRange(["Описание товара", "Цена товара"]);
                        break;
                    }
                case "counteragent":
                    {
                        tmpArr.AddRange(["Название контрагента"]);
                        break;
                    }
                case "order":
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
    }
}
