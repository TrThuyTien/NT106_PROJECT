namespace SoanThaoOnline
{
    partial class QuenMatKhau
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
            textBox_Email = new TextBox();
            label1 = new Label();
            label2 = new Label();
            button_DatLaiMatKhau = new Button();
            SuspendLayout();
            // 
            // textBox_Email
            // 
            textBox_Email.Location = new Point(51, 87);
            textBox_Email.Name = "textBox_Email";
            textBox_Email.Size = new Size(360, 27);
            textBox_Email.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(180, 9);
            label1.Name = "label1";
            label1.Size = new Size(109, 20);
            label1.TabIndex = 1;
            label1.Text = "Quên mật khẩu";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(51, 64);
            label2.Name = "label2";
            label2.Size = new Size(46, 20);
            label2.TabIndex = 1;
            label2.Text = "Email";
            // 
            // button_DatLaiMatKhau
            // 
            button_DatLaiMatKhau.Location = new Point(160, 151);
            button_DatLaiMatKhau.Name = "button_DatLaiMatKhau";
            button_DatLaiMatKhau.Size = new Size(151, 29);
            button_DatLaiMatKhau.TabIndex = 2;
            button_DatLaiMatKhau.Text = "Đặt lại mật khẩu";
            button_DatLaiMatKhau.UseVisualStyleBackColor = true;
            // 
            // QuenMatKhau
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(button_DatLaiMatKhau);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(textBox_Email);
            Name = "QuenMatKhau";
            Text = "QuenMatKhau";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBox_Email;
        private Label label1;
        private Label label2;
        private Button button_DatLaiMatKhau;
    }
}