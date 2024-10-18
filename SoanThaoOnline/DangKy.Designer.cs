namespace SoanThaoOnline
{
    partial class DangKy
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
            textBox_TaiKhoan = new TextBox();
            textBox_MatKhau = new TextBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            textBox_NhapLaiMatKhau = new TextBox();
            label4 = new Label();
            label5 = new Label();
            button_DangKy = new Button();
            SuspendLayout();
            // 
            // textBox_Email
            // 
            textBox_Email.Location = new Point(52, 103);
            textBox_Email.Name = "textBox_Email";
            textBox_Email.Size = new Size(381, 27);
            textBox_Email.TabIndex = 0;
            // 
            // textBox_TaiKhoan
            // 
            textBox_TaiKhoan.Location = new Point(52, 170);
            textBox_TaiKhoan.Name = "textBox_TaiKhoan";
            textBox_TaiKhoan.Size = new Size(381, 27);
            textBox_TaiKhoan.TabIndex = 0;
            // 
            // textBox_MatKhau
            // 
            textBox_MatKhau.Location = new Point(52, 235);
            textBox_MatKhau.Name = "textBox_MatKhau";
            textBox_MatKhau.Size = new Size(381, 27);
            textBox_MatKhau.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(52, 212);
            label1.Name = "label1";
            label1.Size = new Size(70, 20);
            label1.TabIndex = 1;
            label1.Text = "Mật khẩu";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(52, 80);
            label2.Name = "label2";
            label2.Size = new Size(46, 20);
            label2.TabIndex = 1;
            label2.Text = "Email";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(52, 149);
            label3.Name = "label3";
            label3.Size = new Size(71, 20);
            label3.TabIndex = 1;
            label3.Text = "Tài khoản";
            // 
            // textBox_NhapLaiMatKhau
            // 
            textBox_NhapLaiMatKhau.Location = new Point(52, 303);
            textBox_NhapLaiMatKhau.Name = "textBox_NhapLaiMatKhau";
            textBox_NhapLaiMatKhau.Size = new Size(381, 27);
            textBox_NhapLaiMatKhau.TabIndex = 0;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(52, 280);
            label4.Name = "label4";
            label4.Size = new Size(130, 20);
            label4.TabIndex = 1;
            label4.Text = "Nhập lại mật khẩu";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(190, 9);
            label5.Name = "label5";
            label5.Size = new Size(63, 20);
            label5.TabIndex = 1;
            label5.Text = "Đăng ký";
            // 
            // button_DangKy
            // 
            button_DangKy.Location = new Point(190, 360);
            button_DangKy.Name = "button_DangKy";
            button_DangKy.Size = new Size(94, 29);
            button_DangKy.TabIndex = 2;
            button_DangKy.Text = "Đăng ký";
            button_DangKy.UseVisualStyleBackColor = true;
            // 
            // DangKy
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(button_DangKy);
            Controls.Add(label3);
            Controls.Add(label5);
            Controls.Add(label2);
            Controls.Add(label4);
            Controls.Add(label1);
            Controls.Add(textBox_NhapLaiMatKhau);
            Controls.Add(textBox_MatKhau);
            Controls.Add(textBox_TaiKhoan);
            Controls.Add(textBox_Email);
            Name = "DangKy";
            Text = "DangKy";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBox_Email;
        private TextBox textBox_TaiKhoan;
        private TextBox textBox_MatKhau;
        private Label label1;
        private Label label2;
        private Label label3;
        private TextBox textBox_NhapLaiMatKhau;
        private Label label4;
        private Label label5;
        private Button button_DangKy;
    }
}