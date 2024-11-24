namespace docMini
{
    partial class SignIn
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SignIn));
            textBox2 = new TextBox();
            textbox_Username = new TextBox();
            pictureBox1 = new PictureBox();
            textbox_Password = new TextBox();
            pictureBox2 = new PictureBox();
            button_SignIn = new Button();
            checkbox_SaveAccount = new CheckBox();
            label_ForgotPass = new Label();
            label1 = new Label();
            label_SwitchSignUp = new Label();
            button1 = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            SuspendLayout();
            // 
            // textBox2
            // 
            textBox2.BackColor = SystemColors.HighlightText;
            textBox2.BorderStyle = BorderStyle.FixedSingle;
            textBox2.Font = new Font("Segoe UI", 25.8000011F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBox2.Location = new Point(183, 553);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(445, 65);
            textBox2.TabIndex = 1;
            // 
            // textbox_Username
            // 
            textbox_Username.BackColor = SystemColors.Desktop;
            textbox_Username.BorderStyle = BorderStyle.FixedSingle;
            textbox_Username.Font = new Font("Segoe UI", 25.8000011F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textbox_Username.ForeColor = Color.White;
            textbox_Username.Location = new Point(248, 411);
            textbox_Username.Name = "textbox_Username";
            textbox_Username.Size = new Size(380, 65);
            textbox_Username.TabIndex = 2;
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = SystemColors.Desktop;
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(183, 414);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(68, 62);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 3;
            pictureBox1.TabStop = false;
            // 
            // textbox_Password
            // 
            textbox_Password.BackColor = SystemColors.Desktop;
            textbox_Password.BorderStyle = BorderStyle.FixedSingle;
            textbox_Password.Font = new Font("Segoe UI", 25.8000011F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textbox_Password.ForeColor = Color.White;
            textbox_Password.Location = new Point(248, 553);
            textbox_Password.Name = "textbox_Password";
            textbox_Password.Size = new Size(380, 65);
            textbox_Password.TabIndex = 4;
            // 
            // pictureBox2
            // 
            pictureBox2.BackColor = SystemColors.Desktop;
            pictureBox2.Image = (Image)resources.GetObject("pictureBox2.Image");
            pictureBox2.Location = new Point(183, 553);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(68, 65);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.TabIndex = 5;
            pictureBox2.TabStop = false;
            // 
            // button_SignIn
            // 
            button_SignIn.AutoSize = true;
            button_SignIn.BackColor = Color.Black;
            button_SignIn.FlatAppearance.BorderColor = Color.Black;
            button_SignIn.FlatStyle = FlatStyle.Flat;
            button_SignIn.Font = new Font("Sitka Small", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button_SignIn.ForeColor = Color.White;
            button_SignIn.Location = new Point(313, 694);
            button_SignIn.Name = "button_SignIn";
            button_SignIn.Size = new Size(201, 46);
            button_SignIn.TabIndex = 6;
            button_SignIn.Text = "SIGN IN";
            button_SignIn.UseMnemonic = false;
            button_SignIn.UseVisualStyleBackColor = false;
            button_SignIn.Click += button_SignIn_Click;
            // 
            // checkbox_SaveAccount
            // 
            checkbox_SaveAccount.AutoSize = true;
            checkbox_SaveAccount.BackColor = Color.Transparent;
            checkbox_SaveAccount.Font = new Font("Sitka Small", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            checkbox_SaveAccount.ForeColor = Color.White;
            checkbox_SaveAccount.Location = new Point(183, 636);
            checkbox_SaveAccount.Name = "checkbox_SaveAccount";
            checkbox_SaveAccount.Size = new Size(161, 28);
            checkbox_SaveAccount.TabIndex = 7;
            checkbox_SaveAccount.Text = "Remember me";
            checkbox_SaveAccount.UseVisualStyleBackColor = false;
            // 
            // label_ForgotPass
            // 
            label_ForgotPass.AutoSize = true;
            label_ForgotPass.BackColor = Color.Transparent;
            label_ForgotPass.Font = new Font("Sitka Small", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_ForgotPass.ForeColor = Color.FromArgb(255, 128, 128);
            label_ForgotPass.Location = new Point(446, 636);
            label_ForgotPass.Name = "label_ForgotPass";
            label_ForgotPass.Size = new Size(180, 26);
            label_ForgotPass.TabIndex = 8;
            label_ForgotPass.Text = "Forgot Password?";
            label_ForgotPass.Click += label_ForgotPass_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Sitka Small", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.White;
            label1.Location = new Point(183, 766);
            label1.Name = "label1";
            label1.Size = new Size(226, 26);
            label1.TabIndex = 9;
            label1.Text = "Don't have an account?";
            // 
            // label_SwitchSignUp
            // 
            label_SwitchSignUp.AutoSize = true;
            label_SwitchSignUp.BackColor = Color.Transparent;
            label_SwitchSignUp.Font = new Font("Sitka Small", 10.8F, FontStyle.Bold);
            label_SwitchSignUp.ForeColor = Color.DeepSkyBlue;
            label_SwitchSignUp.Location = new Point(415, 766);
            label_SwitchSignUp.Name = "label_SwitchSignUp";
            label_SwitchSignUp.Size = new Size(87, 26);
            label_SwitchSignUp.TabIndex = 10;
            label_SwitchSignUp.Text = "Sign Up";
            label_SwitchSignUp.Click += label_SwitchSignUp_Click;
            // 
            // button1
            // 
            button1.Location = new Point(534, 711);
            button1.Name = "button1";
            button1.Size = new Size(94, 29);
            button1.TabIndex = 13;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // SignIn
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            ClientSize = new Size(794, 961);
            Controls.Add(button1);
            Controls.Add(label_SwitchSignUp);
            Controls.Add(label1);
            Controls.Add(label_ForgotPass);
            Controls.Add(checkbox_SaveAccount);
            Controls.Add(button_SignIn);
            Controls.Add(pictureBox2);
            Controls.Add(textbox_Password);
            Controls.Add(pictureBox1);
            Controls.Add(textbox_Username);
            Controls.Add(textBox2);
            MaximizeBox = false;
            Name = "SignIn";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Sign In";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBox2;
        private TextBox textbox_Username;
        private PictureBox pictureBox1;
        private TextBox textbox_Password;
        private PictureBox pictureBox2;
        private Button button_SignIn;
        private CheckBox checkbox_SaveAccount;
        private Label label_ForgotPass;
        private Label label1;
        private Label label_SwitchSignUp;
        private Button button1;
    }
}
