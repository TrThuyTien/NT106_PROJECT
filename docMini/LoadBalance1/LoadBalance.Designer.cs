namespace LoadBalance1
{
    partial class LoadBalance
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
            richTextBox1 = new RichTextBox();
            button_Listen = new Button();
            SuspendLayout();
            // 
            // richTextBox1
            // 
            richTextBox1.Location = new Point(12, 12);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(776, 367);
            richTextBox1.TabIndex = 0;
            richTextBox1.Text = "";
            // 
            // button_Listen
            // 
            button_Listen.Location = new Point(694, 396);
            button_Listen.Name = "button_Listen";
            button_Listen.Size = new Size(94, 29);
            button_Listen.TabIndex = 1;
            button_Listen.Text = "Listen";
            button_Listen.UseVisualStyleBackColor = true;
            button_Listen.Click += button1_Click;
            // 
            // LoadBalance
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.SteelBlue;
            ClientSize = new Size(800, 450);
            Controls.Add(button_Listen);
            Controls.Add(richTextBox1);
            ForeColor = SystemColors.ControlText;
            Name = "LoadBalance";
            Text = "Load Balance";
            ResumeLayout(false);
        }

        #endregion

        private RichTextBox richTextBox1;
        private Button button_Listen;
    }
}
