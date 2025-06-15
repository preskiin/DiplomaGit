using Diploma.Controllers;
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
using Diploma.Models;
using System.IO;

namespace Diploma.Views.ClassForms
{
    public partial class FormTemplate : Form
    {
        private string _connectionString;
        public Template templateTmp { get; private set; }
        private byte[] _fileContent;

        public FormTemplate(string connectionString)
        {
            InitializeComponent();
            _connectionString = connectionString;
        }

        public FormTemplate(string connectionString, Template templateToEdit)
        {
            InitializeComponent();
            _connectionString = connectionString;
            templateTmp = templateToEdit;
        }

        private void FormTemplate_Load(object sender, EventArgs e)
        {
            this.Text = "Добавление шаблона";
            // Если редактируем существующий шаблон
            if (templateTmp != null)
            {
                this.Text = "Редактирование шаблона";
                textBoxName.Text = templateTmp.name;

                    // Отображаем информацию о загруженном файле
                    if (templateTmp.content != null)
                {
                    labelFileInfo.Text = $"Размер файла: {templateTmp.content.Length / 1024} KB";
                    _fileContent = templateTmp.content;
                }
            }
        }

        private void buttonSelectFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Файлы HTML (*.html)|*.html";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Чтение файла в массив байтов
                        _fileContent = File.ReadAllBytes(openFileDialog.FileName);
                        labelFileInfo.Text = $"Файл: {Path.GetFileName(openFileDialog.FileName)} ({_fileContent.Length / 1024} KB)";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при чтении файла: {ex.Message}", "Ошибка");
                    }
                }
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (ValidateFields())
            {
                try
                {
                    var template = new Template(
                        Id: templateTmp?.id ?? 0,
                        Name: textBoxName.Text,
                        Content: _fileContent
                    );

                    var crud = new CRUD_Templates(_connectionString);

                    if (templateTmp == null)
                    {
                        // Создание нового шаблона
                        long newId = crud.create(template);
                        if (newId > 0)
                        {
                            MessageBox.Show("Шаблон успешно сохранен!", "Успех");
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                    }
                    else
                    {
                        // Обновление существующего шаблона
                        crud.update(template);
                        MessageBox.Show("Шаблон успешно обновлен!", "Успех");
                        DialogResult = DialogResult.OK;
                        Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении шаблона: {ex.Message}", "Ошибка");
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля.", "Ошибка");
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private bool ValidateFields()
        {
            // Проверка названия
            if (string.IsNullOrWhiteSpace(textBoxName.Text))
            {
                MessageBox.Show("Название шаблона не может быть пустым", "Ошибка");
                return false;
            }

            // Проверка что файл выбран
            if (_fileContent == null || _fileContent.Length == 0)
            {
                MessageBox.Show("Необходимо выбрать файл", "Ошибка");
                return false;
            }

            return true;
        }
    }
}
