using Diploma.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;

namespace Diploma.Views.ClassForms
{
    public partial class FormProduct : Form
    {
        private String _connection;
        public Product productTmp;

        public FormProduct()
        {
            InitializeComponent();
        }

        public FormProduct(String con)
        {
            InitializeComponent();
            _connection = con;
        }

        public FormProduct(String con, Product productToEdit)
        {
            InitializeComponent();
            _connection = con;
            productTmp = productToEdit;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (checkAllFields())
            {
                if (productTmp == null)
                {
                    productTmp = new Product(
                        id: 0,
                        name: textBoxName.Text,
                        description: string.IsNullOrWhiteSpace(textBoxDescription.Text) ? null : textBoxDescription.Text,
                        price: numericUpDownPrice.Value);

                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    productTmp = new Product(
                        id: productTmp.Id,
                        name: textBoxName.Text,
                        description: string.IsNullOrWhiteSpace(textBoxDescription.Text) ? null : textBoxDescription.Text,
                        price: numericUpDownPrice.Value);

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

        private void FormProduct_Load(object sender, EventArgs e)
        {
            // Если редактируем существующий продукт
            if (productTmp != null)
            {
                textBoxName.Text = productTmp.Name;
                textBoxDescription.Text = productTmp.Description ?? string.Empty;
                numericUpDownPrice.Value = productTmp.Price;
            }
            else
            {
                // Установка значений по умолчанию для нового продукта
                numericUpDownPrice.Value = 0;
            }
        }

        private bool checkAllFields()
        {
            // Проверка названия
            if (string.IsNullOrWhiteSpace(textBoxName.Text))
            {
                MessageBox.Show("Название продукта не может быть пустым", "Ошибка");
                return false;
            }

            // Проверка цены
            if (numericUpDownPrice.Value <= 0)
            {
                MessageBox.Show("Цена должна быть больше нуля", "Ошибка");
                return false;
            }

            return true;
        }
    }
}
