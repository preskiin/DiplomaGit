using Diploma.Controllers;
using DocumentFormat.OpenXml.Wordprocessing;
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
namespace Diploma.Views.ClassForms
{
    public partial class FormOrder : Form
    {
        private String _connection;
        public Order orderTmp;

        public FormOrder()
        {
            InitializeComponent();
        }

        public FormOrder(String con)
        {
            InitializeComponent();
            _connection = con;
        }

        public FormOrder(String con, Order orderToEdit)
        {
            InitializeComponent();
            _connection = con;
            orderTmp = orderToEdit;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (checkAllFields())
            {
                if (orderTmp == null)
                {
                    orderTmp = new Order(
                        id: 0,
                        number: Convert.ToInt64(textBoxNumber.Text),
                        idCounteragent: (Int64?)comboBoxCounteragent.SelectedValue,
                        orderDate: dateTimePickerOrderDate.Value,
                        deliveryDate: checkBoxDeliveryDate.Checked ? dateTimePickerDeliveryDate.Value : (DateTime?)null,
                        comment: textBoxComment.Text);
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    orderTmp = new Order(
                        id: orderTmp.Id,
                        number: Convert.ToInt64(textBoxNumber.Text),
                        idCounteragent: (Int64?)comboBoxCounteragent.SelectedValue,
                        orderDate: dateTimePickerOrderDate.Value,
                        deliveryDate: checkBoxDeliveryDate.Checked ? dateTimePickerDeliveryDate.Value : (DateTime?)null,
                        comment: textBoxComment.Text);
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
            else
            {
                MessageBox.Show("Введены неверные данные", "Ошибка");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void FormOrder_Load(object sender, EventArgs e)
        {
            // Загрузка контрагентов в комбобокс
            this.Text = "Добавление заказа";
            comboBoxCounteragent.DataSource = CRUD_Counteragents.getAllCounteragents(_connection);
            comboBoxCounteragent.DisplayMember = "Name";
            comboBoxCounteragent.ValueMember = "id";

            // Установка текущей даты по умолчанию
            dateTimePickerOrderDate.Value = DateTime.Now;
            dateTimePickerDeliveryDate.Value = DateTime.Now;

            // Если редактируем существующий заказ
            if (orderTmp != null)
            {
                this.Text = "Редактирование заказа";
                textBoxNumber.Text = orderTmp.Number.ToString();
                comboBoxCounteragent.SelectedValue = orderTmp.IdCounteragent ?? -1;
                dateTimePickerOrderDate.Value = orderTmp.OrderDate;

                if (orderTmp.DeliveryDate.HasValue)
                {
                    checkBoxDeliveryDate.Checked = true;
                    dateTimePickerDeliveryDate.Value = orderTmp.DeliveryDate.Value;
                }

                textBoxComment.Text = orderTmp.Comment ?? "";
            }
        }

        private bool checkAllFields()
        {
            // Проверка, что дата доставки не раньше даты заказа (если указана)
            if (checkBoxDeliveryDate.Checked && dateTimePickerDeliveryDate.Value < dateTimePickerOrderDate.Value)
            {
                MessageBox.Show("Дата доставки не может быть раньше даты заказа", "Ошибка");
                return false;
            }

            // Проверка номера заказа (если указан)
            if (!String.IsNullOrEmpty(textBoxNumber.Text) && !textBoxNumber.Text.All(char.IsDigit))
            {
                MessageBox.Show("Номер заказа должен содержать только цифры", "Ошибка");
                return false;
            }

            // Основные проверки
            return comboBoxCounteragent.SelectedValue != null &&
                   dateTimePickerOrderDate.Value != null;
        }

        private void checkBoxDeliveryDate_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePickerDeliveryDate.Enabled = checkBoxDeliveryDate.Checked;
        }
    }
}
