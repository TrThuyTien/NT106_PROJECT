namespace docMini
{
    partial class Login
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            label2 = new Label();
            textBox_TenDangNhap = new TextBox();
            textBox_MatKhau = new TextBox();
            button_QuenMatKhau = new Button();
            button_DangNhap = new Button();
            button_DangKy = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(72, 102);
            label1.Name = "label1";
            label1.Size = new Size(112, 20);
            label1.TabIndex = 0;
            label1.Text = "Tên Đăng Nhập";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(72, 143);
            label2.Name = "label2";
            label2.Size = new Size(72, 20);
            label2.TabIndex = 0;
            label2.Text = "Mật Khẩu";
            // 
            // textBox_TenDangNhap
            // 
            textBox_TenDangNhap.Location = new Point(196, 95);
            textBox_TenDangNhap.Name = "textBox_TenDangNhap";
            textBox_TenDangNhap.Size = new Size(265, 27);
            textBox_TenDangNhap.TabIndex = 1;
            // 
            // textBox_MatKhau
            // 
            textBox_MatKhau.Location = new Point(196, 140);
            textBox_MatKhau.Name = "textBox_MatKhau";
            textBox_MatKhau.Size = new Size(265, 27);
            textBox_MatKhau.TabIndex = 1;
            // 
            // button_QuenMatKhau
            // 
            button_QuenMatKhau.Location = new Point(204, 246);
            button_QuenMatKhau.Name = "button_QuenMatKhau";
            button_QuenMatKhau.Size = new Size(119, 29);
            button_QuenMatKhau.TabIndex = 2;
            button_QuenMatKhau.Text = "Quên Mật Khẩu";
            button_QuenMatKhau.UseVisualStyleBackColor = true;
            // 
            // button_DangNhap
            // 
            button_DangNhap.Location = new Point(204, 198);
            button_DangNhap.Name = "button_DangNhap";
            button_DangNhap.Size = new Size(119, 29);
            button_DangNhap.TabIndex = 2;
            button_DangNhap.Text = "Đăng Nhập";
            button_DangNhap.UseVisualStyleBackColor = true;
            button_DangNhap.Click += button_DangNhap_Click;
            // 
            // button_DangKy
            // 
            button_DangKy.Location = new Point(342, 198);
            button_DangKy.Name = "button_DangKy";
            button_DangKy.Size = new Size(119, 29);
            button_DangKy.TabIndex = 2;
            button_DangKy.Text = "Đăng Ký";
            button_DangKy.UseVisualStyleBackColor = true;
            // 
            // Login
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(button_DangKy);
            Controls.Add(button_DangNhap);
            Controls.Add(button_QuenMatKhau);
            Controls.Add(textBox_MatKhau);
            Controls.Add(textBox_TenDangNhap);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "Login";
            Text = "Login";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private TextBox textBox_TenDangNhap;
        private TextBox textBox_MatKhau;
        private Button button_QuenMatKhau;
        private Button button_DangNhap;
        private Button button_DangKy;
    }
}
