namespace docMini
{
    partial class SignUp
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SignUp));
            textbox_Username = new TextBox();
            textbox_Email = new TextBox();
            textbox_Password = new TextBox();
            button_SignUp = new Button();
            label1 = new Label();
            label_SwitchSignIn = new Label();
            SuspendLayout();
            // 
            // textbox_Username
            // 
            textbox_Username.BackColor = Color.White;
            textbox_Username.BorderStyle = BorderStyle.None;
            textbox_Username.Font = new Font("Segoe UI", 16.8000011F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textbox_Username.ForeColor = Color.Black;
            textbox_Username.Location = new Point(166, 287);
            textbox_Username.Name = "textbox_Username";
            textbox_Username.Size = new Size(363, 38);
            textbox_Username.TabIndex = 0;
            // 
            // textbox_Email
            // 
            textbox_Email.BackColor = Color.White;
            textbox_Email.BorderStyle = BorderStyle.None;
            textbox_Email.Font = new Font("Segoe UI", 16.8000011F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textbox_Email.ForeColor = Color.Black;
            textbox_Email.Location = new Point(166, 391);
            textbox_Email.Name = "textbox_Email";
            textbox_Email.Size = new Size(363, 38);
            textbox_Email.TabIndex = 1;
            // 
            // textbox_Password
            // 
            textbox_Password.BackColor = Color.White;
            textbox_Password.BorderStyle = BorderStyle.None;
            textbox_Password.Font = new Font("Segoe UI", 16.8000011F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textbox_Password.ForeColor = Color.Black;
            textbox_Password.Location = new Point(166, 495);
            textbox_Password.Name = "textbox_Password";
            textbox_Password.Size = new Size(363, 38);
            textbox_Password.TabIndex = 2;
            // 
            // button_SignUp
            // 
            button_SignUp.AutoSize = true;
            button_SignUp.BackColor = Color.Transparent;
            button_SignUp.FlatAppearance.BorderColor = Color.Black;
            button_SignUp.FlatAppearance.BorderSize = 0;
            button_SignUp.FlatStyle = FlatStyle.Flat;
            button_SignUp.Font = new Font("Sitka Small", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button_SignUp.ForeColor = Color.White;
            button_SignUp.Location = new Point(262, 585);
            button_SignUp.Name = "button_SignUp";
            button_SignUp.Size = new Size(172, 36);
            button_SignUp.TabIndex = 7;
            button_SignUp.Text = "SIGN UP";
            button_SignUp.UseMnemonic = false;
            button_SignUp.UseVisualStyleBackColor = false;
            button_SignUp.Click += button_SignUp_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Sitka Small", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.White;
            label1.Location = new Point(157, 632);
            label1.Name = "label1";
            label1.Size = new Size(252, 26);
            label1.TabIndex = 10;
            label1.Text = "Already have an account ?";
            // 
            // label_SwitchSignIn
            // 
            label_SwitchSignIn.AutoSize = true;
            label_SwitchSignIn.BackColor = Color.Transparent;
            label_SwitchSignIn.Font = new Font("Sitka Small", 10.8F, FontStyle.Bold);
            label_SwitchSignIn.ForeColor = Color.DarkBlue;
            label_SwitchSignIn.Location = new Point(415, 632);
            label_SwitchSignIn.Name = "label_SwitchSignIn";
            label_SwitchSignIn.Size = new Size(80, 26);
            label_SwitchSignIn.TabIndex = 11;
            label_SwitchSignIn.Text = "Sign In";
            label_SwitchSignIn.Click += label_SwitchSignIn_Click;
            // 
            // SignUp
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(682, 753);
            Controls.Add(label_SwitchSignIn);
            Controls.Add(label1);
            Controls.Add(button_SignUp);
            Controls.Add(textbox_Password);
            Controls.Add(textbox_Email);
            Controls.Add(textbox_Username);
            MaximizeBox = false;
            MaximumSize = new Size(700, 800);
            MinimumSize = new Size(700, 800);
            Name = "SignUp";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Sign Up";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textbox_Username;
        private TextBox textbox_Email;
        private TextBox textbox_Password;
        private Button button_SignUp;
        private Label label1;
        private Label label_SwitchSignIn;
    }
}