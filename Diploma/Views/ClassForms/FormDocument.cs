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
    public partial class FormDocument : Form
    {
        private readonly string _connectionString;
        public Document documentTmp { get; private set; }
        private byte[] _fileContent;

        public FormDocument(string connectionString)
        {
            InitializeComponent();
            _connectionString = connectionString;
        }

        public FormDocument(string connectionString, Document documentToEdit)
        {
            
            InitializeComponent();
            _connectionString = connectionString;
            documentTmp = documentToEdit;
        }

        private void FormDocument_Load(object sender, EventArgs e)
        {
            // Загрузка списка шаблонов в комбобокс
            LoadTemplates();
            this.Text = "Добавление документа";
            // Если редактируем существующий документ
            if (documentTmp != null)
            {

                this.Text = "Редактирование документа";
                textBoxName.Text = documentTmp.Name;
                comboBoxTemplate.SelectedValue = documentTmp.IdTemplate;

                // Отображаем информацию о загруженном файле
                if (documentTmp.FileContent != null)
                {
                    labelFileInfo.Text = $"Размер файла: {documentTmp.FileContent.Length / 1024} KB";
                    _fileContent = documentTmp.FileContent;
                }
            }
        }

        private void LoadTemplates()
        {
            // Здесь должен быть код загрузки шаблонов из БД
            // Например:
            //comboBoxTemplate.DataSource = null;
            var tmp = CRUD_Templates.getAllNamesTemplates(_connectionString);
            //tmp.Add(new Template(Id: 0, Name: "Нет шаблона", Content: null));
            comboBoxTemplate.DataSource = tmp;
            comboBoxTemplate.DisplayMember = "name";
            comboBoxTemplate.ValueMember = "id";
        }

        private void buttonSelectFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Файлы Word (*.docx)|*.docx";
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
                    var document = new Document
                    (
                        id: documentTmp.Id ,
                        name: textBoxName.Text,
                        fileContent: _fileContent,
                        idTemplate: (long)comboBoxTemplate.SelectedValue
                    );

                    var crud = new CRUD_Documents(_connectionString);

                    if (documentTmp == null)
                    {
                        // Создание нового документа
                        long newId = crud.create(document);
                        if (newId > 0)
                        {
                            MessageBox.Show("Документ успешно сохранен!", "Успех");
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                    }
                    else
                    {
                        // Обновление существующего документа
                        crud.update(document);
                        MessageBox.Show("Документ успешно обновлен!", "Успех");
                        DialogResult = DialogResult.OK;
                        Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении документа: {ex.Message}", "Ошибка");
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
                MessageBox.Show("Название документа не может быть пустым", "Ошибка");
                return false;
            }

            // Проверка что файл выбран (если требуется)
            if (_fileContent == null || _fileContent.Length == 0)
            {
                MessageBox.Show("Необходимо выбрать файл", "Ошибка");
                return false;
            }
            return true;
        }
    }
}
