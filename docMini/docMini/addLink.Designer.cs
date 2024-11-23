namespace docMini
{
    partial class addLink
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
            richTextBox_Display = new RichTextBox();
            richTextBox_Address = new RichTextBox();
            button_ok = new Button();
            button_Cancel = new Button();
            label1 = new Label();
            label2 = new Label();
            SuspendLayout();
            // 
            // richTextBox_Display
            // 
            richTextBox_Display.Font = new Font("Tahoma", 12F);
            richTextBox_Display.Location = new Point(166, 51);
            richTextBox_Display.Name = "richTextBox_Display";
            richTextBox_Display.Size = new Size(523, 31);
            richTextBox_Display.TabIndex = 0;
            richTextBox_Display.Text = "";
            // 
            // richTextBox_Address
            // 
            richTextBox_Address.Font = new Font("Tahoma", 12F);
            richTextBox_Address.Location = new Point(166, 121);
            richTextBox_Address.Name = "richTextBox_Address";
            richTextBox_Address.Size = new Size(523, 32);
            richTextBox_Address.TabIndex = 1;
            richTextBox_Address.Text = "";
            // 
            // button_ok
            // 
            button_ok.Font = new Font("Tahoma", 12F);
            button_ok.Location = new Point(291, 236);
            button_ok.Name = "button_ok";
            button_ok.Size = new Size(115, 36);
            button_ok.TabIndex = 2;
            button_ok.Text = "OK";
            button_ok.UseVisualStyleBackColor = true;
            button_ok.Click += button_ok_Click;
            // 
            // button_Cancel
            // 
            button_Cancel.Font = new Font("Tahoma", 12F);
            button_Cancel.Location = new Point(462, 236);
            button_Cancel.Name = "button_Cancel";
            button_Cancel.Size = new Size(115, 36);
            button_Cancel.TabIndex = 3;
            button_Cancel.Text = "Cancel";
            button_Cancel.UseVisualStyleBackColor = true;
            button_Cancel.Click += button_Cancel_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Tahoma", 12F);
            label1.Location = new Point(11, 58);
            label1.Name = "label1";
            label1.Size = new Size(149, 24);
            label1.TabIndex = 4;
            label1.Text = "Text to display:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Tahoma", 12F);
            label2.Location = new Point(58, 129);
            label2.Name = "label2";
            label2.Size = new Size(87, 24);
            label2.TabIndex = 5;
            label2.Text = "Address:";
            // 
            // addLink
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 332);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(button_Cancel);
            Controls.Add(button_ok);
            Controls.Add(richTextBox_Address);
            Controls.Add(richTextBox_Display);
            Name = "addLink";
            Text = "addLink";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RichTextBox richTextBox_Display;
        private RichTextBox richTextBox_Address;
        private Button button_ok;
        private Button button_Cancel;
        private Label label1;
        private Label label2;
    }
}