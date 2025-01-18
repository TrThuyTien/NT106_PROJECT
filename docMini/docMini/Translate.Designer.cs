namespace docMini
{
    partial class Translate
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Translate));
            richTextBox_source = new RichTextBox();
            richTextBox_target = new RichTextBox();
            comboBox_source = new ComboBox();
            comboBox_target = new ComboBox();
            button_translate = new Button();
            SuspendLayout();
            // 
            // richTextBox_source
            // 
            richTextBox_source.Location = new Point(28, 88);
            richTextBox_source.Name = "richTextBox_source";
            richTextBox_source.Size = new Size(334, 301);
            richTextBox_source.TabIndex = 0;
            richTextBox_source.Text = "";
            // 
            // richTextBox_target
            // 
            richTextBox_target.Location = new Point(428, 88);
            richTextBox_target.Name = "richTextBox_target";
            richTextBox_target.Size = new Size(342, 301);
            richTextBox_target.TabIndex = 1;
            richTextBox_target.Text = "";
            // 
            // comboBox_source
            // 
            comboBox_source.FormattingEnabled = true;
            comboBox_source.Location = new Point(120, 24);
            comboBox_source.Name = "comboBox_source";
            comboBox_source.Size = new Size(154, 28);
            comboBox_source.TabIndex = 2;
            // 
            // comboBox_target
            // 
            comboBox_target.FormattingEnabled = true;
            comboBox_target.Location = new Point(527, 24);
            comboBox_target.Name = "comboBox_target";
            comboBox_target.Size = new Size(154, 28);
            comboBox_target.TabIndex = 3;
            // 
            // button_translate
            // 
            button_translate.BackgroundImage = (Image)resources.GetObject("button_translate.BackgroundImage");
            button_translate.BackgroundImageLayout = ImageLayout.Zoom;
            button_translate.Location = new Point(368, 211);
            button_translate.Name = "button_translate";
            button_translate.Size = new Size(54, 47);
            button_translate.TabIndex = 4;
            button_translate.UseVisualStyleBackColor = true;
            button_translate.Click += button_translate_Click;
            // 
            // Translate
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(button_translate);
            Controls.Add(comboBox_target);
            Controls.Add(comboBox_source);
            Controls.Add(richTextBox_target);
            Controls.Add(richTextBox_source);
            Name = "Translate";
            Text = "Translate";
            Load += Translate_Load;
            ResumeLayout(false);
        }

        private void Translate_Load(object sender, EventArgs e)
        {
            // Danh sách mã ngôn ngữ
            var languages = new Dictionary<string,string>
    {
        { "English", "EN" },
        { "Japanese", "JA" },
        { "German", "DE" },
        { "Russia", "RU" },
        { "Korea", "KO" },
        { "Italia", "IT" },
        { "French", "FR" },
        { "Chinese", "ZH" },

        // Thêm các ngôn ngữ khác nếu cần
    };

            // Gán dữ liệu vào ComboBox
            comboBox_source.DataSource = new BindingSource(languages, null);
            comboBox_source.DisplayMember = "Key";
            comboBox_source.ValueMember = "Value";

            comboBox_target.DataSource = new BindingSource(languages, null);
            comboBox_target.DisplayMember = "Key";
            comboBox_target.ValueMember = "Value";

            // Đặt giá trị mặc định
            comboBox_source.SelectedValue = "EN";
            comboBox_target.SelectedValue = "JA";
        }
        #endregion

        private RichTextBox richTextBox_source;
        private RichTextBox richTextBox_target;
        private ComboBox comboBox_source;
        private ComboBox comboBox_target;
        private Button button_translate;
    }
}