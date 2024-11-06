namespace Server
{
    partial class Server
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
            richTextBox_Editor = new RichTextBox();
            button_Listen = new Button();
            button_Stop = new Button();
            SuspendLayout();
            // 
            // richTextBox_Editor
            // 
            richTextBox_Editor.Location = new Point(55, 44);
            richTextBox_Editor.Name = "richTextBox_Editor";
            richTextBox_Editor.Size = new Size(533, 609);
            richTextBox_Editor.TabIndex = 0;
            richTextBox_Editor.Text = "";
            // 
            // button_Listen
            // 
            button_Listen.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button_Listen.Location = new Point(439, 659);
            button_Listen.Name = "button_Listen";
            button_Listen.Size = new Size(149, 39);
            button_Listen.TabIndex = 1;
            button_Listen.Text = "LISTEN";
            button_Listen.UseVisualStyleBackColor = true;
            button_Listen.Click += button_Listen_Click;
            // 
            // button_Stop
            // 
            button_Stop.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button_Stop.ForeColor = SystemColors.Desktop;
            button_Stop.Location = new Point(269, 659);
            button_Stop.Name = "button_Stop";
            button_Stop.Size = new Size(149, 39);
            button_Stop.TabIndex = 2;
            button_Stop.Text = "STOP";
            button_Stop.UseVisualStyleBackColor = true;
            button_Stop.Click += button_Stop_Click;
            // 
            // Server
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.SteelBlue;
            ClientSize = new Size(667, 730);
            Controls.Add(button_Stop);
            Controls.Add(button_Listen);
            Controls.Add(richTextBox_Editor);
            Name = "Server";
            Text = "SERVER";
            ResumeLayout(false);
        }

        #endregion

        private RichTextBox richTextBox_Editor;
        private Button button_Listen;
        private Button button_Stop;
    }
}
