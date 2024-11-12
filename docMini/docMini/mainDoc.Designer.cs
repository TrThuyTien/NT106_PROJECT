using System.Drawing.Drawing2D;
using static docMini.mainDoc;
namespace docMini
{
    partial class mainDoc
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(mainDoc));
            button_Exit = new Button();
            button_Minimize = new Button();
            label_NameAccount = new Label();
            label_DocumentName = new Label();
            pictureBox_Logo = new PictureBox();
            panel_ToolbarBorder = new Panel();
            panel_Toolbar = new Panel();
            comboBox_Size = new ComboBox();
            comboBox_Font = new ComboBox();
            pictureBox_add = new PictureBox();
            button_NewFile = new RoundedButton();
            button_ShareDoc = new Button();
            button_AddLink = new Button();
            button_AddTable = new Button();
            button_AddPicture = new Button();
            button_Center = new Button();
            button_AlignRight = new Button();
            button_Justify = new Button();
            button_AlignLeft = new Button();
            button_Italic = new Button();
            button_Underline = new Button();
            button_Bold = new Button();
            pictureBox_Avatar = new PictureBox();
            panel_searchDoc = new Panel();
            panel_areaSearch = new Panel();
            button_searchDoc = new Button();
            textBox_searchDoc = new TextBox();
            panel1 = new Panel();
            panel2 = new Panel();
            panel3 = new Panel();
            panel4 = new Panel();
            button_Save = new Button();
            button_Open = new Button();
            richTextBox_Content = new RichTextBox();
            button_Connect = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox_Logo).BeginInit();
            panel_ToolbarBorder.SuspendLayout();
            panel_Toolbar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox_add).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox_Avatar).BeginInit();
            panel_searchDoc.SuspendLayout();
            panel_areaSearch.SuspendLayout();
            panel3.SuspendLayout();
            SuspendLayout();
            // 
            // button_Exit
            // 
            button_Exit.FlatAppearance.BorderSize = 0;
            button_Exit.FlatAppearance.MouseDownBackColor = Color.FromArgb(205, 236, 255);
            button_Exit.FlatAppearance.MouseOverBackColor = Color.FromArgb(205, 236, 255);
            button_Exit.FlatStyle = FlatStyle.Flat;
            button_Exit.Image = Properties.Resources.exit_32px;
            button_Exit.Location = new Point(1209, 11);
            button_Exit.Name = "button_Exit";
            button_Exit.Size = new Size(36, 35);
            button_Exit.TabIndex = 1;
            button_Exit.UseVisualStyleBackColor = true;
            button_Exit.Click += button_Exit_Click;
            // 
            // button_Minimize
            // 
            button_Minimize.FlatAppearance.BorderSize = 0;
            button_Minimize.FlatAppearance.MouseDownBackColor = Color.FromArgb(205, 236, 255);
            button_Minimize.FlatAppearance.MouseOverBackColor = Color.FromArgb(205, 236, 255);
            button_Minimize.FlatStyle = FlatStyle.Flat;
            button_Minimize.Image = Properties.Resources.minimize;
            button_Minimize.Location = new Point(1167, 11);
            button_Minimize.Name = "button_Minimize";
            button_Minimize.Size = new Size(36, 35);
            button_Minimize.TabIndex = 2;
            button_Minimize.UseVisualStyleBackColor = true;
            button_Minimize.Click += button_Minimize_Click;
            // 
            // label_NameAccount
            // 
            label_NameAccount.AutoSize = true;
            label_NameAccount.Font = new Font("Tahoma", 12F, FontStyle.Regular, GraphicsUnit.Point, 163);
            label_NameAccount.Location = new Point(984, 14);
            label_NameAccount.Name = "label_NameAccount";
            label_NameAccount.Size = new Size(133, 24);
            label_NameAccount.TabIndex = 4;
            label_NameAccount.Text = "Tên tài khoản";
            label_NameAccount.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label_DocumentName
            // 
            label_DocumentName.AutoSize = true;
            label_DocumentName.Font = new Font("Tahoma", 12F, FontStyle.Regular, GraphicsUnit.Point, 163);
            label_DocumentName.Location = new Point(56, 25);
            label_DocumentName.Name = "label_DocumentName";
            label_DocumentName.Size = new Size(252, 24);
            label_DocumentName.TabIndex = 6;
            label_DocumentName.Text = "Tên tài liệu đang chỉnh sửa";
            label_DocumentName.TextAlign = ContentAlignment.MiddleRight;
            // 
            // pictureBox_Logo
            // 
            pictureBox_Logo.Image = Properties.Resources.doc;
            pictureBox_Logo.Location = new Point(12, 14);
            pictureBox_Logo.Name = "pictureBox_Logo";
            pictureBox_Logo.Size = new Size(38, 35);
            pictureBox_Logo.SizeMode = PictureBoxSizeMode.CenterImage;
            pictureBox_Logo.TabIndex = 5;
            pictureBox_Logo.TabStop = false;
            // 
            // panel_ToolbarBorder
            // 
            panel_ToolbarBorder.BackColor = Color.RoyalBlue;
            panel_ToolbarBorder.Controls.Add(panel_Toolbar);
            panel_ToolbarBorder.Location = new Point(5, 55);
            panel_ToolbarBorder.Name = "panel_ToolbarBorder";
            panel_ToolbarBorder.Size = new Size(1240, 91);
            panel_ToolbarBorder.TabIndex = 7;
            panel_ToolbarBorder.Paint += panel1_Paint;
            // 
            // panel_Toolbar
            // 
            panel_Toolbar.BackColor = Color.White;
            panel_Toolbar.Controls.Add(comboBox_Size);
            panel_Toolbar.Controls.Add(comboBox_Font);
            panel_Toolbar.Controls.Add(pictureBox_add);
            panel_Toolbar.Controls.Add(button_NewFile);
            panel_Toolbar.Controls.Add(button_ShareDoc);
            panel_Toolbar.Controls.Add(button_AddLink);
            panel_Toolbar.Controls.Add(button_AddTable);
            panel_Toolbar.Controls.Add(button_AddPicture);
            panel_Toolbar.Controls.Add(button_Center);
            panel_Toolbar.Controls.Add(button_AlignRight);
            panel_Toolbar.Controls.Add(button_Justify);
            panel_Toolbar.Controls.Add(button_AlignLeft);
            panel_Toolbar.Controls.Add(button_Italic);
            panel_Toolbar.Controls.Add(button_Underline);
            panel_Toolbar.Controls.Add(button_Bold);
            panel_Toolbar.Location = new Point(3, 3);
            panel_Toolbar.Name = "panel_Toolbar";
            panel_Toolbar.Size = new Size(1234, 85);
            panel_Toolbar.TabIndex = 8;
            panel_Toolbar.Paint += panel2_Paint;
            // 
            // comboBox_Size
            // 
            comboBox_Size.FormattingEnabled = true;
            comboBox_Size.Location = new Point(226, 34);
            comboBox_Size.Name = "comboBox_Size";
            comboBox_Size.Size = new Size(54, 26);
            comboBox_Size.TabIndex = 15;
            // 
            // comboBox_Font
            // 
            comboBox_Font.FormattingEnabled = true;
            comboBox_Font.Location = new Point(48, 34);
            comboBox_Font.Name = "comboBox_Font";
            comboBox_Font.Size = new Size(156, 26);
            comboBox_Font.TabIndex = 14;
            // 
            // pictureBox_add
            // 
            pictureBox_add.BackColor = Color.FromArgb(205, 236, 255);
            pictureBox_add.Image = (Image)resources.GetObject("pictureBox_add.Image");
            pictureBox_add.Location = new Point(937, 30);
            pictureBox_add.Name = "pictureBox_add";
            pictureBox_add.Size = new Size(34, 41);
            pictureBox_add.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox_add.TabIndex = 12;
            pictureBox_add.TabStop = false;
            // 
            // button_NewFile
            // 
            button_NewFile.BackColor = Color.FromArgb(205, 236, 255);
            button_NewFile.Font = new Font("Tahoma", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 163);
            button_NewFile.Location = new Point(925, 25);
            button_NewFile.Name = "button_NewFile";
            button_NewFile.Size = new Size(156, 46);
            button_NewFile.TabIndex = 12;
            button_NewFile.Text = "     New file";
            button_NewFile.UseVisualStyleBackColor = false;
            button_NewFile.Click += button_newFile_Click;
            // 
            // button_ShareDoc
            // 
            button_ShareDoc.BackColor = Color.FromArgb(175, 219, 255);
            button_ShareDoc.FlatAppearance.BorderSize = 0;
            button_ShareDoc.FlatAppearance.MouseDownBackColor = Color.FromArgb(205, 236, 255);
            button_ShareDoc.FlatAppearance.MouseOverBackColor = Color.FromArgb(205, 236, 255);
            button_ShareDoc.FlatStyle = FlatStyle.Flat;
            button_ShareDoc.Font = new Font("Tahoma", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 163);
            button_ShareDoc.Image = Properties.Resources.file;
            button_ShareDoc.ImageAlign = ContentAlignment.MiddleLeft;
            button_ShareDoc.Location = new Point(1108, 13);
            button_ShareDoc.Name = "button_ShareDoc";
            button_ShareDoc.Size = new Size(111, 61);
            button_ShareDoc.TabIndex = 13;
            button_ShareDoc.Text = "      Share";
            button_ShareDoc.UseVisualStyleBackColor = false;
            button_ShareDoc.Paint += button_ShareDoc_Paint;
            // 
            // button_AddLink
            // 
            button_AddLink.FlatAppearance.BorderSize = 0;
            button_AddLink.FlatAppearance.MouseDownBackColor = Color.FromArgb(205, 236, 255);
            button_AddLink.FlatAppearance.MouseOverBackColor = Color.FromArgb(205, 236, 255);
            button_AddLink.FlatStyle = FlatStyle.Flat;
            button_AddLink.Image = Properties.Resources.link_button;
            button_AddLink.Location = new Point(865, 27);
            button_AddLink.Name = "button_AddLink";
            button_AddLink.Size = new Size(36, 35);
            button_AddLink.TabIndex = 12;
            button_AddLink.UseVisualStyleBackColor = true;
            // 
            // button_AddTable
            // 
            button_AddTable.FlatAppearance.BorderSize = 0;
            button_AddTable.FlatAppearance.MouseDownBackColor = Color.FromArgb(205, 236, 255);
            button_AddTable.FlatAppearance.MouseOverBackColor = Color.FromArgb(205, 236, 255);
            button_AddTable.FlatStyle = FlatStyle.Flat;
            button_AddTable.Image = Properties.Resources.table;
            button_AddTable.Location = new Point(810, 27);
            button_AddTable.Name = "button_AddTable";
            button_AddTable.Size = new Size(36, 35);
            button_AddTable.TabIndex = 11;
            button_AddTable.UseVisualStyleBackColor = true;
            // 
            // button_AddPicture
            // 
            button_AddPicture.FlatAppearance.BorderSize = 0;
            button_AddPicture.FlatAppearance.MouseDownBackColor = Color.FromArgb(205, 236, 255);
            button_AddPicture.FlatAppearance.MouseOverBackColor = Color.FromArgb(205, 236, 255);
            button_AddPicture.FlatStyle = FlatStyle.Flat;
            button_AddPicture.Image = Properties.Resources.image;
            button_AddPicture.Location = new Point(749, 27);
            button_AddPicture.Name = "button_AddPicture";
            button_AddPicture.Size = new Size(36, 35);
            button_AddPicture.TabIndex = 10;
            button_AddPicture.UseVisualStyleBackColor = true;
            button_AddPicture.Click += button_AddPicture_Click;
            // 
            // button_Center
            // 
            button_Center.FlatAppearance.BorderSize = 0;
            button_Center.FlatAppearance.MouseDownBackColor = Color.FromArgb(205, 236, 255);
            button_Center.FlatAppearance.MouseOverBackColor = Color.FromArgb(205, 236, 255);
            button_Center.FlatStyle = FlatStyle.Flat;
            button_Center.Image = Properties.Resources.format;
            button_Center.Location = new Point(569, 27);
            button_Center.Name = "button_Center";
            button_Center.Size = new Size(36, 35);
            button_Center.TabIndex = 9;
            button_Center.UseVisualStyleBackColor = true;
            button_Center.Click += button_Center_Click;
            // 
            // button_AlignRight
            // 
            button_AlignRight.FlatAppearance.BorderSize = 0;
            button_AlignRight.FlatAppearance.MouseDownBackColor = Color.FromArgb(205, 236, 255);
            button_AlignRight.FlatAppearance.MouseOverBackColor = Color.FromArgb(205, 236, 255);
            button_AlignRight.FlatStyle = FlatStyle.Flat;
            button_AlignRight.Image = Properties.Resources.align_right;
            button_AlignRight.Location = new Point(627, 27);
            button_AlignRight.Name = "button_AlignRight";
            button_AlignRight.Size = new Size(36, 35);
            button_AlignRight.TabIndex = 8;
            button_AlignRight.UseVisualStyleBackColor = true;
            button_AlignRight.Click += button_AlignRight_Click;
            // 
            // button_Justify
            // 
            button_Justify.FlatAppearance.BorderSize = 0;
            button_Justify.FlatAppearance.MouseDownBackColor = Color.FromArgb(205, 236, 255);
            button_Justify.FlatAppearance.MouseOverBackColor = Color.FromArgb(205, 236, 255);
            button_Justify.FlatStyle = FlatStyle.Flat;
            button_Justify.Image = Properties.Resources.justify;
            button_Justify.Location = new Point(688, 27);
            button_Justify.Name = "button_Justify";
            button_Justify.Size = new Size(36, 35);
            button_Justify.TabIndex = 7;
            button_Justify.UseVisualStyleBackColor = true;
            button_Justify.Click += button_Justify_Click;
            // 
            // button_AlignLeft
            // 
            button_AlignLeft.FlatAppearance.BorderSize = 0;
            button_AlignLeft.FlatAppearance.MouseDownBackColor = Color.FromArgb(205, 236, 255);
            button_AlignLeft.FlatAppearance.MouseOverBackColor = Color.FromArgb(205, 236, 255);
            button_AlignLeft.FlatStyle = FlatStyle.Flat;
            button_AlignLeft.Image = Properties.Resources.align_left1;
            button_AlignLeft.Location = new Point(509, 27);
            button_AlignLeft.Name = "button_AlignLeft";
            button_AlignLeft.Size = new Size(36, 35);
            button_AlignLeft.TabIndex = 6;
            button_AlignLeft.UseVisualStyleBackColor = true;
            button_AlignLeft.Click += button_AlignLeft_Click;
            // 
            // button_Italic
            // 
            button_Italic.FlatAppearance.BorderSize = 0;
            button_Italic.FlatAppearance.MouseDownBackColor = Color.FromArgb(205, 236, 255);
            button_Italic.FlatAppearance.MouseOverBackColor = Color.FromArgb(205, 236, 255);
            button_Italic.FlatStyle = FlatStyle.Flat;
            button_Italic.Image = Properties.Resources.italic_button;
            button_Italic.Location = new Point(375, 25);
            button_Italic.Name = "button_Italic";
            button_Italic.Size = new Size(36, 35);
            button_Italic.TabIndex = 5;
            button_Italic.UseVisualStyleBackColor = true;
            button_Italic.Click += button_Italic_Click;
            // 
            // button_Underline
            // 
            button_Underline.FlatAppearance.BorderSize = 0;
            button_Underline.FlatAppearance.MouseDownBackColor = Color.FromArgb(205, 236, 255);
            button_Underline.FlatAppearance.MouseOverBackColor = Color.FromArgb(205, 236, 255);
            button_Underline.FlatStyle = FlatStyle.Flat;
            button_Underline.Image = Properties.Resources.underline_text;
            button_Underline.Location = new Point(435, 25);
            button_Underline.Name = "button_Underline";
            button_Underline.Size = new Size(36, 35);
            button_Underline.TabIndex = 4;
            button_Underline.UseVisualStyleBackColor = true;
            button_Underline.Click += button_Underline_Click;
            // 
            // button_Bold
            // 
            button_Bold.FlatAppearance.BorderSize = 0;
            button_Bold.FlatAppearance.MouseDownBackColor = Color.FromArgb(205, 236, 255);
            button_Bold.FlatAppearance.MouseOverBackColor = Color.FromArgb(205, 236, 255);
            button_Bold.FlatStyle = FlatStyle.Flat;
            button_Bold.Image = Properties.Resources.bold_button;
            button_Bold.Location = new Point(316, 25);
            button_Bold.Name = "button_Bold";
            button_Bold.Size = new Size(36, 35);
            button_Bold.TabIndex = 3;
            button_Bold.UseVisualStyleBackColor = true;
            button_Bold.Click += button_Bold_Click;
            // 
            // pictureBox_Avatar
            // 
            pictureBox_Avatar.Image = Properties.Resources.user;
            pictureBox_Avatar.Location = new Point(1123, 11);
            pictureBox_Avatar.Name = "pictureBox_Avatar";
            pictureBox_Avatar.Size = new Size(38, 35);
            pictureBox_Avatar.SizeMode = PictureBoxSizeMode.CenterImage;
            pictureBox_Avatar.TabIndex = 9;
            pictureBox_Avatar.TabStop = false;
            // 
            // panel_searchDoc
            // 
            panel_searchDoc.BackColor = Color.FromArgb(205, 236, 255);
            panel_searchDoc.Controls.Add(panel_areaSearch);
            panel_searchDoc.Location = new Point(43, 219);
            panel_searchDoc.Name = "panel_searchDoc";
            panel_searchDoc.Size = new Size(359, 362);
            panel_searchDoc.TabIndex = 11;
            panel_searchDoc.Paint += panel_searchDoc_Paint;
            // 
            // panel_areaSearch
            // 
            panel_areaSearch.BackColor = Color.White;
            panel_areaSearch.Controls.Add(button_searchDoc);
            panel_areaSearch.Controls.Add(textBox_searchDoc);
            panel_areaSearch.Location = new Point(19, 16);
            panel_areaSearch.Name = "panel_areaSearch";
            panel_areaSearch.Size = new Size(322, 69);
            panel_areaSearch.TabIndex = 3;
            panel_areaSearch.Paint += panel3_Paint;
            // 
            // button_searchDoc
            // 
            button_searchDoc.BackColor = Color.White;
            button_searchDoc.BackgroundImage = (Image)resources.GetObject("button_searchDoc.BackgroundImage");
            button_searchDoc.BackgroundImageLayout = ImageLayout.Zoom;
            button_searchDoc.FlatAppearance.BorderSize = 0;
            button_searchDoc.FlatStyle = FlatStyle.Flat;
            button_searchDoc.Location = new Point(15, 20);
            button_searchDoc.Name = "button_searchDoc";
            button_searchDoc.Size = new Size(38, 36);
            button_searchDoc.TabIndex = 2;
            button_searchDoc.UseVisualStyleBackColor = false;
            // 
            // textBox_searchDoc
            // 
            textBox_searchDoc.BackColor = Color.White;
            textBox_searchDoc.BorderStyle = BorderStyle.None;
            textBox_searchDoc.Font = new Font("Tahoma", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 163);
            textBox_searchDoc.Location = new Point(59, 20);
            textBox_searchDoc.Multiline = true;
            textBox_searchDoc.Name = "textBox_searchDoc";
            textBox_searchDoc.PlaceholderText = "Search";
            textBox_searchDoc.Size = new Size(262, 35);
            textBox_searchDoc.TabIndex = 1;
            // 
            // panel1
            // 
            panel1.BackColor = Color.White;
            panel1.Location = new Point(457, 157);
            panel1.Name = "panel1";
            panel1.Size = new Size(74, 671);
            panel1.TabIndex = 15;
            // 
            // panel2
            // 
            panel2.BackColor = Color.White;
            panel2.Location = new Point(539, 3);
            panel2.Name = "panel2";
            panel2.Size = new Size(73, 671);
            panel2.TabIndex = 16;
            // 
            // panel3
            // 
            panel3.BackColor = Color.White;
            panel3.Controls.Add(panel2);
            panel3.Location = new Point(531, 157);
            panel3.Name = "panel3";
            panel3.Size = new Size(533, 63);
            panel3.TabIndex = 17;
            // 
            // panel4
            // 
            panel4.BackColor = Color.White;
            panel4.Location = new Point(1064, 157);
            panel4.Name = "panel4";
            panel4.Size = new Size(76, 671);
            panel4.TabIndex = 18;
            // 
            // button_Save
            // 
            button_Save.Location = new Point(43, 160);
            button_Save.Name = "button_Save";
            button_Save.Size = new Size(116, 44);
            button_Save.TabIndex = 19;
            button_Save.Text = "Save";
            button_Save.UseVisualStyleBackColor = true;
            button_Save.Click += button_Save_Click;
            // 
            // button_Open
            // 
            button_Open.Location = new Point(210, 160);
            button_Open.Name = "button_Open";
            button_Open.Size = new Size(116, 44);
            button_Open.TabIndex = 20;
            button_Open.Text = "Open";
            button_Open.UseVisualStyleBackColor = true;
            button_Open.Click += button_Open_Click;
            // 
            // richTextBox_Content
            // 
            richTextBox_Content.BorderStyle = BorderStyle.None;
            richTextBox_Content.Location = new Point(531, 219);
            richTextBox_Content.Name = "richTextBox_Content";
            richTextBox_Content.Size = new Size(533, 609);
            richTextBox_Content.TabIndex = 10;
            richTextBox_Content.Text = "";
            richTextBox_Content.TextChanged += richTextBox_Content_TextChanged;
            // 
            // button_Connect
            // 
            button_Connect.Location = new Point(77, 643);
            button_Connect.Name = "button_Connect";
            button_Connect.Size = new Size(257, 56);
            button_Connect.TabIndex = 21;
            button_Connect.Text = "Mở tài liệu";
            button_Connect.UseVisualStyleBackColor = true;
            button_Connect.Click += button_Connect_Click;
            // 
            // mainDoc
            // 
            AutoScaleDimensions = new SizeF(8F, 18F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(205, 236, 255);
            ClientSize = new Size(1254, 840);
            Controls.Add(button_Connect);
            Controls.Add(button_Open);
            Controls.Add(button_Save);
            Controls.Add(panel4);
            Controls.Add(panel3);
            Controls.Add(panel1);
            Controls.Add(panel_searchDoc);
            Controls.Add(richTextBox_Content);
            Controls.Add(pictureBox_Avatar);
            Controls.Add(panel_ToolbarBorder);
            Controls.Add(label_DocumentName);
            Controls.Add(pictureBox_Logo);
            Controls.Add(label_NameAccount);
            Controls.Add(button_Minimize);
            Controls.Add(button_Exit);
            Font = new Font("Tahoma", 9F, FontStyle.Regular, GraphicsUnit.Point, 163);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "mainDoc";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "mainDoc";
            Load += mainDoc_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox_Logo).EndInit();
            panel_ToolbarBorder.ResumeLayout(false);
            panel_Toolbar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox_add).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox_Avatar).EndInit();
            panel_searchDoc.ResumeLayout(false);
            panel_areaSearch.ResumeLayout(false);
            panel_areaSearch.PerformLayout();
            panel3.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            SetRoundedPanel(panel_ToolbarBorder, Color.FromArgb(0, 101, 225), 3, 30, Color.FromArgb(0, 101, 225));
        }
        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            SetRoundedPanel(panel_Toolbar, Color.White, 3, 30, Color.White);
        }
        private void panel3_Paint(object sender, PaintEventArgs e)
        {
            SetRoundedPanel(panel_areaSearch, Color.White, 3, 30, Color.White);

        }
        private void panel_searchDoc_Paint(object sender, PaintEventArgs e)
        {
            Panel panel = sender as Panel;
            Color borderColor = Color.Black; // Màu viền bạn muốn

            int borderWidth = 5; // Độ dày của viền
            ControlPaint.DrawBorder(e.Graphics, panel.ClientRectangle, borderColor, borderWidth, ButtonBorderStyle.Solid,
                                    borderColor, borderWidth, ButtonBorderStyle.Solid,
                                    borderColor, borderWidth, ButtonBorderStyle.Solid,
                                    borderColor, borderWidth, ButtonBorderStyle.Solid);
        }
        public class RoundedButton : Button
        {
            protected override void OnPaint(PaintEventArgs e)
            {
                GraphicsPath path = new GraphicsPath();
                int radius = 20; // Bán kính bo góc, bạn có thể thay đổi giá trị này
                path.AddArc(new Rectangle(0, 0, radius, radius), 180, 90);
                path.AddArc(new Rectangle(Width - radius, 0, radius, radius), 270, 90);
                path.AddArc(new Rectangle(Width - radius, Height - radius, radius, radius), 0, 90);
                path.AddArc(new Rectangle(0, Height - radius, radius, radius), 90, 90);
                path.CloseFigure();
                this.Region = new Region(path);

                base.OnPaint(e);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.FillPath(new SolidBrush(this.BackColor), path);

                // Vẽ văn bản của Button
                TextRenderer.DrawText(e.Graphics, this.Text, this.Font, this.ClientRectangle, this.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
        }
        public class PagedRichTextBox : RichTextBox
        {
            private const int MaxLinesPerPage = 28;
            private int currentPage = 1;
            private Form parentForm;

            public PagedRichTextBox(Form parent)
            {
                this.parentForm = parent;
                this.TextChanged += new EventHandler(PagedRichTextBox_TextChanged);
            }

            private void PagedRichTextBox_TextChanged(object sender, EventArgs e)
            {
                if (this.Lines.Length > MaxLinesPerPage)
                {
                    CreateNewPage();
                }
            }

            private void CreateNewPage()
            {
                currentPage++;
                RichTextBox newPage = new PagedRichTextBox(this.parentForm)
                {
                    Location = new Point(this.Location.X, this.Location.Y + this.Height + 10),
                    Size = this.Size
                };
                this.parentForm.Controls.Add(newPage);
                newPage.BringToFront();
                newPage.Focus();
            }
        }



        private void SetRoundedPanel(Panel panel, Color borderColor, int borderWidth, int borderRadius, Color fillColor)
        {
            // Tạo đường dẫn GraphicsPath với bo góc
            GraphicsPath path = new GraphicsPath();
            int width = panel.Width - 1;
            int height = panel.Height - 1;

            path.AddArc(0, 0, borderRadius, borderRadius, 180, 90); // Góc trên bên trái
            path.AddArc(width - borderRadius, 0, borderRadius, borderRadius, 270, 90); // Góc trên bên phải
            path.AddArc(width - borderRadius, height - borderRadius, borderRadius, borderRadius, 0, 90); // Góc dưới bên phải
            path.AddArc(0, height - borderRadius, borderRadius, borderRadius, 90, 90); // Góc dưới bên trái
            path.CloseFigure();

            // Đặt vùng hiển thị cho Panel theo bo góc để ẩn phần thừa
            panel.Region = new Region(path);

            // Vẽ nền và viền cho Panel
            panel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                // Vẽ màu nền
                using (SolidBrush brush = new SolidBrush(fillColor))
                {
                    e.Graphics.FillPath(brush, path);
                }

                // Vẽ viền
                using (Pen pen = new Pen(borderColor, borderWidth))
                {
                    e.Graphics.DrawPath(pen, path);
                }
            };
        }

        private void button_ShareDoc_Paint(object sender, PaintEventArgs e)
        {
            Button button = (Button)sender;
            int borderRadius = 20; // Bán kính bo góc

            // Tạo GraphicsPath để vẽ hình bo góc
            GraphicsPath path = new GraphicsPath();
            path.StartFigure();
            path.AddArc(new Rectangle(0, 0, borderRadius, borderRadius), 180, 90);
            path.AddArc(new Rectangle(button.Width - borderRadius, 0, borderRadius, borderRadius), 270, 90);
            path.AddArc(new Rectangle(button.Width - borderRadius, button.Height - borderRadius, borderRadius, borderRadius), 0, 90);
            path.AddArc(new Rectangle(0, button.Height - borderRadius, borderRadius, borderRadius), 90, 90);
            path.CloseFigure();

            // Thiết lập vùng của nút
            button.Region = new Region(path);

            // Thiết lập chế độ vẽ
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Vẽ nền nút
            using (SolidBrush brush = new SolidBrush(button.BackColor))
            {
                e.Graphics.FillPath(brush, path);
            }

            // Vẽ viền nếu cần (tùy chọn)
            using (Pen pen = new Pen(button.FlatAppearance.BorderColor, button.FlatAppearance.BorderSize))
            {
                e.Graphics.DrawPath(pen, path);
            }

            // Vẽ hình ảnh nếu có
            if (button.Image != null)
            {
                // Xác định vị trí của hình ảnh
                Rectangle imageRect = new Rectangle(8, (button.Height - button.Image.Height) / 2, button.Image.Width, button.Image.Height); // Điều chỉnh tọa độ nếu cần
                e.Graphics.DrawImage(button.Image, imageRect);
            }

            // Vẽ text của nút
            TextRenderer.DrawText(e.Graphics, button.Text, button.Font, button.ClientRectangle, button.ForeColor,
                                  TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }
        #endregion

        private Button button_Exit;
        private Button button_Minimize;
        private Label label_NameAccount;
        private Label label_DocumentName;
        private PictureBox pictureBox_Logo;
        private Panel panel_ToolbarBorder;
        private Panel panel_Toolbar;
        private PictureBox pictureBox_Avatar;
        private Button button_Bold;
        private Button button_AddLink;
        private Button button_AddTable;
        private Button button_AddPicture;
        private Button button_Center;
        private Button button_AlignRight;
        private Button button_Justify;
        private Button button_AlignLeft;
        private Button button_Italic;
        private Button button_Underline;
        private Button button_ShareDoc;
        private Panel panel_searchDoc;
        private TextBox textBox_searchDoc;
        private Panel panel_areaSearch;
        private Button button_searchDoc;
        private PictureBox pictureBox_add;
        private RoundedButton button_NewFile;
        private ComboBox comboBox_Size;
        private ComboBox comboBox_Font;
        private Panel panel1;
        private Panel panel2;
        private Panel panel3;
        private Panel panel4;
        private Button button_Save;
        private RichTextBox richTextBox_Content;
        private Button button_Open;
        private Button button_Connect;
    }
}
