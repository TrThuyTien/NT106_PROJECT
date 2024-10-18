namespace SoanThaoOnline
{
    partial class DangNhap
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
            textBox_TaiKhoan = new TextBox();
            textBox_MatKhau = new TextBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            button_DangNhap = new Button();
            button_DangKy = new Button();
            button_QuenMatKhau = new Button();
            SuspendLayout();
            // 
            // textBox_TaiKhoan
            // 
            textBox_TaiKhoan.Location = new Point(39, 123);
            textBox_TaiKhoan.Name = "textBox_TaiKhoan";
            textBox_TaiKhoan.Size = new Size(362, 27);
            textBox_TaiKhoan.TabIndex = 0;
            // 
            // textBox_MatKhau
            // 
            textBox_MatKhau.Location = new Point(39, 211);
            textBox_MatKhau.Name = "textBox_MatKhau";
            textBox_MatKhau.Size = new Size(362, 27);
            textBox_MatKhau.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(184, 9);
            label1.Name = "label1";
            label1.Size = new Size(82, 20);
            label1.TabIndex = 1;
            label1.Text = "Đăng nhập";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(39, 100);
            label2.Name = "label2";
            label2.Size = new Size(71, 20);
            label2.TabIndex = 1;
            label2.Text = "Tài khoản";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(39, 188);
            label3.Name = "label3";
            label3.Size = new Size(70, 20);
            label3.TabIndex = 1;
            label3.Text = "Mật khẩu";
            // 
            // button_DangNhap
            // 
            button_DangNhap.Location = new Point(39, 265);
            button_DangNhap.Name = "button_DangNhap";
            button_DangNhap.Size = new Size(94, 29);
            button_DangNhap.TabIndex = 2;
            button_DangNhap.Text = "Đăng nhập";
            button_DangNhap.UseVisualStyleBackColor = true;
            button_DangNhap.Click += button_DangNhap_Click;
            // 
            // button_DangKy
            // 
            button_DangKy.Location = new Point(160, 265);
            button_DangKy.Name = "button_DangKy";
            button_DangKy.Size = new Size(94, 29);
            button_DangKy.TabIndex = 2;
            button_DangKy.Text = "Đăng ký";
            button_DangKy.UseVisualStyleBackColor = true;
            button_DangKy.Click += button_DangKy_Click;
            // 
            // button_QuenMatKhau
            // 
            button_QuenMatKhau.Location = new Point(278, 265);
            button_QuenMatKhau.Name = "button_QuenMatKhau";
            button_QuenMatKhau.Size = new Size(123, 29);
            button_QuenMatKhau.TabIndex = 2;
            button_QuenMatKhau.Text = "Quên mật khẩu";
            button_QuenMatKhau.UseVisualStyleBackColor = true;
            button_QuenMatKhau.Click += button_QuenMatKhau_Click;
            // 
            // DangNhap
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(button_QuenMatKhau);
            Controls.Add(button_DangKy);
            Controls.Add(button_DangNhap);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(textBox_MatKhau);
            Controls.Add(textBox_TaiKhoan);
            Name = "DangNhap";
            Text = "DangNhap";
            Load += DangNhap_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBox_TaiKhoan;
        private TextBox textBox_MatKhau;
        private Label label1;
        private Label label2;
        private Label label3;
        private Button button_DangNhap;
        private Button button_DangKy;
        private Button button_QuenMatKhau;
    }
}