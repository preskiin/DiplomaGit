using Diploma.Controllers;
using Diploma.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diploma.Views.ClassForms
{
    public partial class FormOrderItem : Form
    {
        private String _connection;
        public OrderItem orderItemTmp;
        private Int64 _orderId; // ID заказа, к которому принадлежит элемент

        public FormOrderItem()
        {
            InitializeComponent();
        }

        public FormOrderItem(String con, Int64 orderId)
        {
            InitializeComponent();
            _connection = con;
            _orderId = orderId;
        }

        public FormOrderItem(String con, Int64 orderId, OrderItem itemToEdit)
        {
            InitializeComponent();
            _connection = con;
            _orderId = orderId;
            orderItemTmp = itemToEdit;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (checkAllFields())
            {
                if (orderItemTmp == null)
                {
                    orderItemTmp = new OrderItem(
                        id: 0,
                        idProduct: (Int64?)comboBoxProduct.SelectedValue,
                        idOrder: _orderId,
                        price: numericUpDownPrice.Value,
                        amount: (Int32)numericUpDownAmount.Value);

                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    orderItemTmp = new OrderItem(
                        id: orderItemTmp.Id,
                        idProduct: (Int64?)comboBoxProduct.SelectedValue,
                        idOrder: _orderId,
                        price: numericUpDownPrice.Value,
                        amount: (Int32)numericUpDownAmount.Value);

                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
            else
            {
                MessageBox.Show("Введены неверные данные", "Ошибка");
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void FormOrderItem_Load(object sender, EventArgs e)
        {
            // Загрузка списка товаров в комбобокс
            comboBoxProduct.DataSource = CRUD_Products.getAllProducts(_connection);
            comboBoxProduct.DisplayMember = "Name"; // Предполагается, что у Product есть свойство Name
            comboBoxProduct.ValueMember = "id";

            // Если редактируем существующий элемент
            if (orderItemTmp != null)
            {
                comboBoxProduct.SelectedValue = orderItemTmp.IdProduct ?? -1;
                numericUpDownPrice.Value = orderItemTmp.Price;
                numericUpDownAmount.Value = orderItemTmp.Amount;
            }
            else
            {
                // Установка значений по умолчанию для нового элемента
                numericUpDownPrice.Value = 0;
                numericUpDownAmount.Value = 1;
            }
        }

        private bool checkAllFields()
        {
            // Проверка, что выбран товар (если IdProduct не nullable, уберите проверку на -1)
            if (comboBoxProduct.SelectedValue == null ||
               (comboBoxProduct.SelectedValue is Int64 && (Int64)comboBoxProduct.SelectedValue == -1))
            {
                MessageBox.Show("Необходимо выбрать товар", "Ошибка");
                return false;
            }

            // Проверка цены
            if (numericUpDownPrice.Value <= 0)
            {
                MessageBox.Show("Цена должна быть больше нуля", "Ошибка");
                return false;
            }

            // Проверка количества
            if (numericUpDownAmount.Value <= 0)
            {
                MessageBox.Show("Количество должно быть больше нуля", "Ошибка");
                return false;
            }

            return true;
        }
    }
}
