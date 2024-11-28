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
            textbox_Username = new TextBox();
            textbox_Password = new TextBox();
            button_SignIn = new Button();
            checkbox_Showpass = new CheckBox();
            label_ForgotPass = new Label();
            label1 = new Label();
            label_SwitchSignUp = new Label();
            SuspendLayout();
            // 
            // textbox_Username
            // 
            textbox_Username.BackColor = Color.White;
            textbox_Username.BorderStyle = BorderStyle.None;
            textbox_Username.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            textbox_Username.ForeColor = Color.Black;
            textbox_Username.Location = new Point(220, 319);
            textbox_Username.Name = "textbox_Username";
            textbox_Username.Size = new Size(297, 27);
            textbox_Username.TabIndex = 2;
            // 
            // textbox_Password
            // 
            textbox_Password.BackColor = Color.White;
            textbox_Password.BorderStyle = BorderStyle.None;
            textbox_Password.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            textbox_Password.ForeColor = Color.Black;
            textbox_Password.Location = new Point(220, 424);
            textbox_Password.Name = "textbox_Password";
            textbox_Password.Size = new Size(297, 27);
            textbox_Password.TabIndex = 4;
            textbox_Password.UseSystemPasswordChar = true;
            // 
            // button_SignIn
            // 
            button_SignIn.BackColor = Color.Transparent;
            button_SignIn.FlatAppearance.BorderColor = Color.Black;
            button_SignIn.FlatAppearance.BorderSize = 0;
            button_SignIn.FlatStyle = FlatStyle.Flat;
            button_SignIn.Font = new Font("Sitka Small", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button_SignIn.ForeColor = Color.White;
            button_SignIn.Location = new Point(261, 518);
            button_SignIn.Name = "button_SignIn";
            button_SignIn.Size = new Size(172, 36);
            button_SignIn.TabIndex = 6;
            button_SignIn.Text = "SIGN IN";
            button_SignIn.UseMnemonic = false;
            button_SignIn.UseVisualStyleBackColor = false;
            button_SignIn.Click += button_SignIn_Click;
            // 
            // checkbox_Showpass
            // 
            checkbox_Showpass.AutoSize = true;
            checkbox_Showpass.BackColor = Color.Transparent;
            checkbox_Showpass.Font = new Font("Sitka Small", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            checkbox_Showpass.ForeColor = Color.White;
            checkbox_Showpass.Location = new Point(152, 475);
            checkbox_Showpass.Name = "checkbox_Showpass";
            checkbox_Showpass.Size = new Size(176, 29);
            checkbox_Showpass.TabIndex = 7;
            checkbox_Showpass.Text = "Show password";
            checkbox_Showpass.UseVisualStyleBackColor = false;
            checkbox_Showpass.CheckedChanged += checkbox_Showpass_CheckedChanged;
            // 
            // label_ForgotPass
            // 
            label_ForgotPass.AutoSize = true;
            label_ForgotPass.BackColor = Color.Transparent;
            label_ForgotPass.Font = new Font("Sitka Small", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_ForgotPass.ForeColor = Color.Coral;
            label_ForgotPass.Location = new Point(345, 475);
            label_ForgotPass.Name = "label_ForgotPass";
            label_ForgotPass.Size = new Size(182, 26);
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
            label1.Location = new Point(180, 571);
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
            label_SwitchSignUp.ForeColor = Color.DarkBlue;
            label_SwitchSignUp.Location = new Point(400, 571);
            label_SwitchSignUp.Name = "label_SwitchSignUp";
            label_SwitchSignUp.Size = new Size(87, 26);
            label_SwitchSignUp.TabIndex = 10;
            label_SwitchSignUp.Text = "Sign Up";
            label_SwitchSignUp.Click += label_SwitchSignUp_Click;
            // 
            // SignIn
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(682, 753);
            Controls.Add(label_SwitchSignUp);
            Controls.Add(label1);
            Controls.Add(label_ForgotPass);
            Controls.Add(checkbox_Showpass);
            Controls.Add(button_SignIn);
            Controls.Add(textbox_Password);
            Controls.Add(textbox_Username);
            MaximizeBox = false;
            MaximumSize = new Size(700, 800);
            MinimumSize = new Size(700, 800);
            Name = "SignIn";
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Sign In";
            TopMost = true;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private TextBox textbox_Username;
        private TextBox textbox_Password;
        private Button button_SignIn;
        private CheckBox checkbox_Showpass;
        private Label label_ForgotPass;
        private Label label1;
        private Label label_SwitchSignUp;
    }
}
