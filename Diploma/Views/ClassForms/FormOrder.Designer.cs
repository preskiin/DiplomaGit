namespace Diploma.Views.ClassForms
{
    partial class FormOrder
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBoxNumber = new System.Windows.Forms.TextBox();
            this.textBoxComment = new System.Windows.Forms.RichTextBox();
            this.comboBoxCounteragent = new System.Windows.Forms.ComboBox();
            this.dateTimePickerOrderDate = new System.Windows.Forms.DateTimePicker();
            this.dateTimePickerDeliveryDate = new System.Windows.Forms.DateTimePicker();
            this.checkBoxDeliveryDate = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxNumber
            // 
            this.textBoxNumber.Location = new System.Drawing.Point(95, 12);
            this.textBoxNumber.Name = "textBoxNumber";
            this.textBoxNumber.Size = new System.Drawing.Size(204, 20);
            this.textBoxNumber.TabIndex = 0;
            // 
            // textBoxComment
            // 
            this.textBoxComment.Location = new System.Drawing.Point(95, 145);
            this.textBoxComment.Name = "textBoxComment";
            this.textBoxComment.Size = new System.Drawing.Size(204, 96);
            this.textBoxComment.TabIndex = 1;
            this.textBoxComment.Text = "";
            // 
            // comboBoxCounteragent
            // 
            this.comboBoxCounteragent.FormattingEnabled = true;
            this.comboBoxCounteragent.Location = new System.Drawing.Point(95, 38);
            this.comboBoxCounteragent.Name = "comboBoxCounteragent";
            this.comboBoxCounteragent.Size = new System.Drawing.Size(204, 21);
            this.comboBoxCounteragent.TabIndex = 2;
            // 
            // dateTimePickerOrderDate
            // 
            this.dateTimePickerOrderDate.Location = new System.Drawing.Point(95, 65);
            this.dateTimePickerOrderDate.Name = "dateTimePickerOrderDate";
            this.dateTimePickerOrderDate.Size = new System.Drawing.Size(204, 20);
            this.dateTimePickerOrderDate.TabIndex = 3;
            // 
            // dateTimePickerDeliveryDate
            // 
            this.dateTimePickerDeliveryDate.Location = new System.Drawing.Point(95, 119);
            this.dateTimePickerDeliveryDate.Name = "dateTimePickerDeliveryDate";
            this.dateTimePickerDeliveryDate.Size = new System.Drawing.Size(204, 20);
            this.dateTimePickerDeliveryDate.TabIndex = 4;
            // 
            // checkBoxDeliveryDate
            // 
            this.checkBoxDeliveryDate.AutoSize = true;
            this.checkBoxDeliveryDate.Location = new System.Drawing.Point(95, 96);
            this.checkBoxDeliveryDate.Name = "checkBoxDeliveryDate";
            this.checkBoxDeliveryDate.Size = new System.Drawing.Size(165, 17);
            this.checkBoxDeliveryDate.TabIndex = 5;
            this.checkBoxDeliveryDate.Text = "Дата доставки определена";
            this.checkBoxDeliveryDate.UseVisualStyleBackColor = true;
            this.checkBoxDeliveryDate.Click += new System.EventHandler(this.checkBoxDeliveryDate_CheckedChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 261);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(224, 261);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 7;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // FormOrder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(311, 296);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.checkBoxDeliveryDate);
            this.Controls.Add(this.dateTimePickerDeliveryDate);
            this.Controls.Add(this.dateTimePickerOrderDate);
            this.Controls.Add(this.comboBoxCounteragent);
            this.Controls.Add(this.textBoxComment);
            this.Controls.Add(this.textBoxNumber);
            this.Name = "FormOrder";
            this.Text = "FormOrder";
            this.Load += new System.EventHandler(this.FormOrder_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxNumber;
        private System.Windows.Forms.RichTextBox textBoxComment;
        private System.Windows.Forms.ComboBox comboBoxCounteragent;
        private System.Windows.Forms.DateTimePicker dateTimePickerOrderDate;
        private System.Windows.Forms.DateTimePicker dateTimePickerDeliveryDate;
        private System.Windows.Forms.CheckBox checkBoxDeliveryDate;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}