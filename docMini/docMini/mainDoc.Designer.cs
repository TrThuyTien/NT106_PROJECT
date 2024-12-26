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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(mainDoc));
            button_Exit = new Button();
            button_Minimize = new Button();
            label_NameAccount = new Label();
            label_DocumentName = new Label();
            pictureBox_Logo = new PictureBox();
            panel_ToolbarBorder = new Panel();
            panel_Toolbar = new Panel();
            button_HighLight = new Button();
            button_textColor = new Button();
            button_Download = new Button();
            button_LineCounter = new Button();
            button_LineSpace = new Button();
            comboBox_Size = new ComboBox();
            button_NewFile = new Button();
            comboBox_Font = new ComboBox();
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
            listBox_Docs = new ListBox();
            richTextBox_Content = new RichTextBox();
            button_Connect = new Button();
            contextMenu_Table = new ContextMenuStrip(components);
            contextMenuStrip_Table = new ContextMenuStrip(components);
            toolStripTextBox1 = new ToolStripTextBox();
            button_LoadFile = new Button();
            button_DeleteFile = new Button();
            contextMenuStrip_headIcon = new ContextMenuStrip(components);
            ((System.ComponentModel.ISupportInitialize)pictureBox_Logo).BeginInit();
            panel_ToolbarBorder.SuspendLayout();
            panel_Toolbar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox_Avatar).BeginInit();
            contextMenuStrip_Table.SuspendLayout();
            SuspendLayout();
            // 
            // button_Exit
            // 
            button_Exit.FlatAppearance.BorderSize = 0;
            button_Exit.FlatAppearance.MouseDownBackColor = Color.FromArgb(205, 236, 255);
            button_Exit.FlatAppearance.MouseOverBackColor = Color.FromArgb(205, 236, 255);
            button_Exit.FlatStyle = FlatStyle.Flat;
            button_Exit.Image = Properties.Resources.exit_32px;
            button_Exit.Location = new Point(1365, 4);
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
            button_Minimize.Location = new Point(1323, 4);
            button_Minimize.Name = "button_Minimize";
            button_Minimize.Size = new Size(36, 35);
            button_Minimize.TabIndex = 2;
            button_Minimize.UseVisualStyleBackColor = true;
            button_Minimize.Click += button_Minimize_Click;
            // 
            // label_NameAccount
            // 
            label_NameAccount.Font = new Font("Tahoma", 12F, FontStyle.Regular, GraphicsUnit.Point, 163);
            label_NameAccount.Location = new Point(1092, 10);
            label_NameAccount.Name = "label_NameAccount";
            label_NameAccount.Size = new Size(184, 24);
            label_NameAccount.TabIndex = 4;
            label_NameAccount.Text = "Tên tài khoản";
            label_NameAccount.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label_DocumentName
            // 
            label_DocumentName.AutoSize = true;
            label_DocumentName.Font = new Font("Tahoma", 12F, FontStyle.Regular, GraphicsUnit.Point, 163);
            label_DocumentName.Location = new Point(38, 9);
            label_DocumentName.Name = "label_DocumentName";
            label_DocumentName.Size = new Size(133, 24);
            label_DocumentName.TabIndex = 6;
            label_DocumentName.Text = "* Tạo file mới";
            label_DocumentName.TextAlign = ContentAlignment.MiddleRight;
            // 
            // pictureBox_Logo
            // 
            pictureBox_Logo.Image = Properties.Resources.doc;
            pictureBox_Logo.Location = new Point(-1, 2);
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
            panel_ToolbarBorder.Location = new Point(6, 43);
            panel_ToolbarBorder.Name = "panel_ToolbarBorder";
            panel_ToolbarBorder.Size = new Size(1395, 91);
            panel_ToolbarBorder.TabIndex = 7;
            panel_ToolbarBorder.Paint += panel1_Paint;
            // 
            // panel_Toolbar
            // 
            panel_Toolbar.BackColor = Color.White;
            panel_Toolbar.Controls.Add(button_HighLight);
            panel_Toolbar.Controls.Add(button_textColor);
            panel_Toolbar.Controls.Add(button_Download);
            panel_Toolbar.Controls.Add(button_LineCounter);
            panel_Toolbar.Controls.Add(button_LineSpace);
            panel_Toolbar.Controls.Add(comboBox_Size);
            panel_Toolbar.Controls.Add(button_NewFile);
            panel_Toolbar.Controls.Add(comboBox_Font);
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
            panel_Toolbar.Size = new Size(1389, 85);
            panel_Toolbar.TabIndex = 8;
            panel_Toolbar.Paint += panel2_Paint;
            // 
            // button_HighLight
            // 
            button_HighLight.BackgroundImage = (Image)resources.GetObject("button_HighLight.BackgroundImage");
            button_HighLight.BackgroundImageLayout = ImageLayout.Stretch;
            button_HighLight.FlatAppearance.BorderSize = 0;
            button_HighLight.FlatStyle = FlatStyle.Flat;
            button_HighLight.Image = (Image)resources.GetObject("button_HighLight.Image");
            button_HighLight.Location = new Point(893, 27);
            button_HighLight.Name = "button_HighLight";
            button_HighLight.Size = new Size(35, 35);
            button_HighLight.TabIndex = 23;
            button_HighLight.UseVisualStyleBackColor = true;
            button_HighLight.Click += button_HighLight_Click;
            // 
            // button_textColor
            // 
            button_textColor.BackgroundImage = (Image)resources.GetObject("button_textColor.BackgroundImage");
            button_textColor.BackgroundImageLayout = ImageLayout.Stretch;
            button_textColor.FlatAppearance.BorderSize = 0;
            button_textColor.FlatStyle = FlatStyle.Flat;
            button_textColor.Location = new Point(843, 28);
            button_textColor.Name = "button_textColor";
            button_textColor.Size = new Size(35, 34);
            button_textColor.TabIndex = 24;
            button_textColor.UseVisualStyleBackColor = true;
            button_textColor.Click += button_textColor_Click;
            // 
            // button_Download
            // 
            button_Download.BackColor = Color.FromArgb(175, 219, 255);
            button_Download.FlatAppearance.BorderSize = 0;
            button_Download.FlatAppearance.MouseDownBackColor = Color.FromArgb(205, 236, 255);
            button_Download.FlatAppearance.MouseOverBackColor = Color.FromArgb(205, 236, 255);
            button_Download.FlatStyle = FlatStyle.Flat;
            button_Download.Font = new Font("Tahoma", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 163);
            button_Download.Image = (Image)resources.GetObject("button_Download.Image");
            button_Download.ImageAlign = ContentAlignment.MiddleLeft;
            button_Download.Location = new Point(1228, 23);
            button_Download.Name = "button_Download";
            button_Download.Size = new Size(145, 40);
            button_Download.TabIndex = 22;
            button_Download.Text = "      Download";
            button_Download.UseVisualStyleBackColor = false;
            button_Download.Click += button_Save_Click;
            button_Download.Paint += button_Download_Paint;
            // 
            // button_LineCounter
            // 
            button_LineCounter.FlatAppearance.BorderSize = 0;
            button_LineCounter.FlatStyle = FlatStyle.Flat;
            button_LineCounter.Image = (Image)resources.GetObject("button_LineCounter.Image");
            button_LineCounter.Location = new Point(784, 26);
            button_LineCounter.Name = "button_LineCounter";
            button_LineCounter.Size = new Size(37, 36);
            button_LineCounter.TabIndex = 16;
            button_LineCounter.UseVisualStyleBackColor = true;
            button_LineCounter.Click += button_LineCounter_Click;
            // 
            // button_LineSpace
            // 
            button_LineSpace.FlatAppearance.BorderSize = 0;
            button_LineSpace.FlatStyle = FlatStyle.Flat;
            button_LineSpace.Image = (Image)resources.GetObject("button_LineSpace.Image");
            button_LineSpace.Location = new Point(742, 23);
            button_LineSpace.Name = "button_LineSpace";
            button_LineSpace.Size = new Size(36, 36);
            button_LineSpace.TabIndex = 16;
            button_LineSpace.UseVisualStyleBackColor = true;
            button_LineSpace.Click += button_LineSpace_Click;
            // 
            // comboBox_Size
            // 
            comboBox_Size.Font = new Font("Tahoma", 12F);
            comboBox_Size.FormattingEnabled = true;
            comboBox_Size.Location = new Point(330, 26);
            comboBox_Size.Name = "comboBox_Size";
            comboBox_Size.Size = new Size(54, 32);
            comboBox_Size.TabIndex = 15;
            comboBox_Size.SelectedIndexChanged += comboBox_Size_SelectedIndexChanged;
            // 
            // button_NewFile
            // 
            button_NewFile.BackColor = Color.FromArgb(175, 219, 255);
            button_NewFile.FlatAppearance.BorderSize = 0;
            button_NewFile.FlatAppearance.MouseDownBackColor = Color.FromArgb(205, 236, 255);
            button_NewFile.FlatAppearance.MouseOverBackColor = Color.FromArgb(205, 236, 255);
            button_NewFile.FlatStyle = FlatStyle.Flat;
            button_NewFile.Font = new Font("Tahoma", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 163);
            button_NewFile.Image = (Image)resources.GetObject("button_NewFile.Image");
            button_NewFile.ImageAlign = ContentAlignment.MiddleLeft;
            button_NewFile.Location = new Point(14, 22);
            button_NewFile.Name = "button_NewFile";
            button_NewFile.Size = new Size(135, 40);
            button_NewFile.TabIndex = 21;
            button_NewFile.Text = "      New File";
            button_NewFile.UseVisualStyleBackColor = false;
            button_NewFile.Click += button_newFile_Click;
            button_NewFile.Paint += button_NewFile_Paint;
            // 
            // comboBox_Font
            // 
            comboBox_Font.Font = new Font("Tahoma", 12F);
            comboBox_Font.FormattingEnabled = true;
            comboBox_Font.Location = new Point(166, 26);
            comboBox_Font.Name = "comboBox_Font";
            comboBox_Font.Size = new Size(156, 32);
            comboBox_Font.TabIndex = 14;
            comboBox_Font.SelectedIndexChanged += comboBox_Font_SelectedIndexChanged;
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
            button_ShareDoc.Location = new Point(1118, 23);
            button_ShareDoc.Name = "button_ShareDoc";
            button_ShareDoc.Size = new Size(104, 40);
            button_ShareDoc.TabIndex = 13;
            button_ShareDoc.Text = "      Share";
            button_ShareDoc.UseVisualStyleBackColor = false;
            button_ShareDoc.Click += button_ShareDoc_Click;
            button_ShareDoc.Paint += button_ShareDoc_Paint;
            // 
            // button_AddLink
            // 
            button_AddLink.FlatAppearance.BorderSize = 0;
            button_AddLink.FlatAppearance.MouseDownBackColor = Color.FromArgb(205, 236, 255);
            button_AddLink.FlatAppearance.MouseOverBackColor = Color.FromArgb(205, 236, 255);
            button_AddLink.FlatStyle = FlatStyle.Flat;
            button_AddLink.Image = Properties.Resources.link_button;
            button_AddLink.Location = new Point(1050, 25);
            button_AddLink.Name = "button_AddLink";
            button_AddLink.Size = new Size(36, 36);
            button_AddLink.TabIndex = 12;
            button_AddLink.UseVisualStyleBackColor = true;
            button_AddLink.Click += button_AddLink_Click;
            // 
            // button_AddTable
            // 
            button_AddTable.FlatAppearance.BorderSize = 0;
            button_AddTable.FlatAppearance.MouseDownBackColor = Color.FromArgb(205, 236, 255);
            button_AddTable.FlatAppearance.MouseOverBackColor = Color.FromArgb(205, 236, 255);
            button_AddTable.FlatStyle = FlatStyle.Flat;
            button_AddTable.Image = Properties.Resources.table;
            button_AddTable.Location = new Point(1007, 27);
            button_AddTable.Name = "button_AddTable";
            button_AddTable.Size = new Size(36, 36);
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
            button_AddPicture.Location = new Point(965, 25);
            button_AddPicture.Name = "button_AddPicture";
            button_AddPicture.Size = new Size(36, 36);
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
            button_Center.Location = new Point(594, 25);
            button_Center.Name = "button_Center";
            button_Center.Size = new Size(36, 36);
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
            button_AlignRight.Location = new Point(636, 25);
            button_AlignRight.Name = "button_AlignRight";
            button_AlignRight.Size = new Size(36, 36);
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
            button_Justify.Location = new Point(679, 25);
            button_Justify.Name = "button_Justify";
            button_Justify.Size = new Size(36, 36);
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
            button_AlignLeft.Location = new Point(552, 25);
            button_AlignLeft.Name = "button_AlignLeft";
            button_AlignLeft.Size = new Size(36, 36);
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
            button_Italic.Location = new Point(452, 25);
            button_Italic.Name = "button_Italic";
            button_Italic.Size = new Size(36, 36);
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
            button_Underline.Location = new Point(494, 25);
            button_Underline.Name = "button_Underline";
            button_Underline.Size = new Size(36, 36);
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
            button_Bold.Location = new Point(410, 25);
            button_Bold.Name = "button_Bold";
            button_Bold.Size = new Size(36, 36);
            button_Bold.TabIndex = 3;
            button_Bold.UseVisualStyleBackColor = true;
            button_Bold.Click += button_Bold_Click;
            // 
            // pictureBox_Avatar
            // 
            pictureBox_Avatar.Image = Properties.Resources.user;
            pictureBox_Avatar.Location = new Point(1279, 5);
            pictureBox_Avatar.Name = "pictureBox_Avatar";
            pictureBox_Avatar.Size = new Size(38, 35);
            pictureBox_Avatar.SizeMode = PictureBoxSizeMode.CenterImage;
            pictureBox_Avatar.TabIndex = 9;
            pictureBox_Avatar.TabStop = false;
            // 
            // listBox_Docs
            // 
            listBox_Docs.BorderStyle = BorderStyle.FixedSingle;
            listBox_Docs.Font = new Font("Tahoma", 12F, FontStyle.Regular, GraphicsUnit.Point, 163);
            listBox_Docs.FormattingEnabled = true;
            listBox_Docs.IntegralHeight = false;
            listBox_Docs.ItemHeight = 24;
            listBox_Docs.Location = new Point(38, 276);
            listBox_Docs.Margin = new Padding(8);
            listBox_Docs.Name = "listBox_Docs";
            listBox_Docs.Size = new Size(321, 547);
            listBox_Docs.TabIndex = 21;
            listBox_Docs.DoubleClick += listBox_Docs_DoubleClick;
            // 
            // richTextBox_Content
            // 
            richTextBox_Content.AcceptsTab = true;
            richTextBox_Content.BackColor = Color.White;
            richTextBox_Content.BorderStyle = BorderStyle.None;
            richTextBox_Content.Location = new Point(480, 166);
            richTextBox_Content.Name = "richTextBox_Content";
            richTextBox_Content.Size = new Size(781, 737);
            richTextBox_Content.TabIndex = 10;
            richTextBox_Content.Text = "";
            richTextBox_Content.TextChanged += richTextBox_Content_TextChanged;
            richTextBox_Content.SelectionChanged += richTextBox_Content_SelectionChanged;
           // richTextBox_Content.KeyDown += richTextBox_Content_KeyDown;
         
            // 
            // button_Connect
            // 
            button_Connect.Location = new Point(0, 0);
            button_Connect.Name = "button_Connect";
            button_Connect.Size = new Size(75, 23);
            button_Connect.TabIndex = 0;
            // 
            // contextMenu_Table
            // 
            contextMenu_Table.ImageScalingSize = new Size(20, 20);
            contextMenu_Table.Name = "contextMenuStrip1";
            contextMenu_Table.Size = new Size(61, 4);
            // 
            // contextMenuStrip_Table
            // 
            contextMenuStrip_Table.ImageScalingSize = new Size(20, 20);
            contextMenuStrip_Table.Items.AddRange(new ToolStripItem[] { toolStripTextBox1 });
            contextMenuStrip_Table.Name = "contextMenuStrip1";
            contextMenuStrip_Table.Size = new Size(161, 33);
            // 
            // toolStripTextBox1
            // 
            toolStripTextBox1.Name = "toolStripTextBox1";
            toolStripTextBox1.Size = new Size(100, 27);
            // 
            // button_LoadFile
            // 
            button_LoadFile.BackColor = Color.FromArgb(255, 255, 192);
            button_LoadFile.FlatAppearance.BorderColor = Color.Black;
            button_LoadFile.FlatAppearance.MouseDownBackColor = Color.White;
            button_LoadFile.FlatAppearance.MouseOverBackColor = Color.White;
            button_LoadFile.FlatStyle = FlatStyle.Flat;
            button_LoadFile.Font = new Font("Cambria", 13.8F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 163);
            button_LoadFile.Image = (Image)resources.GetObject("button_LoadFile.Image");
            button_LoadFile.ImageAlign = ContentAlignment.MiddleRight;
            button_LoadFile.Location = new Point(38, 242);
            button_LoadFile.Name = "button_LoadFile";
            button_LoadFile.Size = new Size(321, 34);
            button_LoadFile.TabIndex = 23;
            button_LoadFile.Text = "Danh sách các file";
            button_LoadFile.UseVisualStyleBackColor = false;
            button_LoadFile.Click += button_LoadFile_Click;
            // 
            // button_DeleteFile
            // 
            button_DeleteFile.BackColor = Color.FromArgb(255, 128, 128);
            button_DeleteFile.FlatStyle = FlatStyle.Flat;
            button_DeleteFile.Font = new Font("Tahoma", 12F, FontStyle.Bold, GraphicsUnit.Point, 163);
            button_DeleteFile.Image = (Image)resources.GetObject("button_DeleteFile.Image");
            button_DeleteFile.ImageAlign = ContentAlignment.MiddleLeft;
            button_DeleteFile.Location = new Point(226, 824);
            button_DeleteFile.Name = "button_DeleteFile";
            button_DeleteFile.Size = new Size(132, 44);
            button_DeleteFile.TabIndex = 24;
            button_DeleteFile.Text = "Xóa File";
            button_DeleteFile.TextAlign = ContentAlignment.MiddleRight;
            button_DeleteFile.UseVisualStyleBackColor = false;
            button_DeleteFile.Click += button_DeleteFile_Click;
            // 
            // contextMenuStrip_headIcon
            // 
            contextMenuStrip_headIcon.ImageScalingSize = new Size(20, 20);
            contextMenuStrip_headIcon.Name = "contextMenuStrip_headIcon";
            contextMenuStrip_headIcon.Size = new Size(211, 32);
            // 
            // mainDoc
            // 
            AutoScaleDimensions = new SizeF(8F, 18F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(205, 236, 255);
            ClientSize = new Size(1404, 935);
            Controls.Add(button_DeleteFile);
            Controls.Add(button_LoadFile);
            Controls.Add(listBox_Docs);
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
            ((System.ComponentModel.ISupportInitialize)pictureBox_Avatar).EndInit();
            contextMenuStrip_Table.ResumeLayout(false);
            contextMenuStrip_Table.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private void ButtonDownload_Paint(object sender, PaintEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            SetRoundedPanel(panel_ToolbarBorder, Color.FromArgb(0, 101, 225), 3, 30, Color.FromArgb(0, 101, 225));
        }
        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            SetRoundedPanel(panel_Toolbar, Color.White, 3, 30, Color.White);
        }
        private void richTextBox_Content_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = e.LinkText,
                UseShellExecute = true
            });
        }
        private void InsertRtfAtCursor(RichTextBox richTextBox, string rtf)
        {
            // Lưu vị trí con trỏ hiện tại
            int selectionStart = richTextBox.SelectionStart;

            // Chèn RTF tại vị trí con trỏ
            richTextBox.SelectedRtf = rtf;

            // Đặt lại vị trí con trỏ sau khi chèn
            richTextBox.SelectionStart = selectionStart + rtf.Length;
        }
        private void InitializeTableContextMenu()
        {
            contextMenu_Table = new ContextMenuStrip();
            contextMenu_Table.RenderMode = ToolStripRenderMode.System;

            // Thêm sự kiện xử lý cho menu
            button_AddTable.Click += (s, e) =>
            {
                // Kiểm soát trạng thái định dạng
                if (isFormatting) return;
                isFormatting = true;
                try
                {
                    contextMenu_Table.Items.Clear();

                    // Thêm các mục chọn số cột
                    for (int cols = 1; cols <= 8; cols++) // Giới hạn số cột
                    {
                        ToolStripMenuItem colItem = new ToolStripMenuItem($"{cols} Columns");

                        // Thêm menu con cho số hàng
                        for (int rows = 1; rows <= 8; rows++) // Giới hạn số hàng
                        {
                            ToolStripMenuItem rowItem = new ToolStripMenuItem($"{cols} x {rows}");
                            rowItem.Tag = new Tuple<int, int>(rows, cols);

                            // Sự kiện khi mục hàng được chọn
                            rowItem.Click += (sender, args) =>
                            {
                                if (rowItem.Tag is Tuple<int, int> size)
                                {
                                    int selectedRows = size.Item1;
                                    int selectedCols = size.Item2;

                                    // Chèn bảng vào vị trí con trỏ
                                    string newTableRtf = InsertTableInRichTextBox(selectedRows, selectedCols, 500);
                                    InsertRtfAtCursor(this.richTextBox_Content, newTableRtf);
                                }
                            };

                            colItem.DropDownItems.Add(rowItem);
                        }
                        contextMenu_Table.Items.Add(colItem);
                    }

                    // Hiển thị menu dưới nút
                    contextMenu_Table.Show(button_AddTable, 0, button_AddTable.Height);
                }
                finally
                {
                    // Kết thúc trạng thái định dạng
                    isFormatting = false;
                }

            };

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
        private void button_NewFile_Paint(object sender, PaintEventArgs e)
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
        private void button_Download_Paint(object sender, PaintEventArgs e)
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
        private void UpdateFormattingButtons()
        {
            // Lấy font của ký tự tại vị trí con trỏ
            Font currentFont = richTextBox_Content.SelectionFont;
            // Kiểm tra trạng thái căn lề
            switch (richTextBox_Content.SelectionAlignment)
            {
                case HorizontalAlignment.Left:
                    button_AlignLeft.BackColor = Color.LightBlue;
                    button_Center.BackColor = SystemColors.Control;
                    button_AlignRight.BackColor = SystemColors.Control;
                    button_Justify.BackColor = SystemColors.Control;
                    break;
                case HorizontalAlignment.Center:
                    button_AlignLeft.BackColor = SystemColors.Control;
                    button_Center.BackColor = Color.LightBlue;
                    button_AlignRight.BackColor = SystemColors.Control;
                    button_Justify.BackColor = SystemColors.Control;
                    break;
                case HorizontalAlignment.Right:
                    button_AlignLeft.BackColor = SystemColors.Control;
                    button_Center.BackColor = SystemColors.Control;
                    button_AlignRight.BackColor = Color.LightBlue;
                    button_Justify.BackColor = SystemColors.Control;
                    break;
                default: // Justify không được hỗ trợ trực tiếp trong HorizontalAlignment
                    button_AlignLeft.BackColor = SystemColors.Control;
                    button_Center.BackColor = SystemColors.Control;
                    button_AlignRight.BackColor = SystemColors.Control;
                    button_Justify.BackColor = Color.LightBlue;
                    break;
            }
        }
        private void InitializeBulletStyleMenu()
        {
            contextMenuStrip_headIcon = new ContextMenuStrip();

            // Thêm các tùy chọn kiểu đánh dấu
            contextMenuStrip_headIcon.Items.Add("Chấm trắng", null, (s, e) => ApplyBulletPoints(richTextBox_Content, "○"));
            contextMenuStrip_headIcon.Items.Add("Chấm đen", null, (s, e) => ApplyBulletPoints(richTextBox_Content, "●"));
            contextMenuStrip_headIcon.Items.Add("Gạch ngang", null, (s, e) => ApplyBulletPoints(richTextBox_Content, "—"));
            contextMenuStrip_headIcon.Items.Add("Số thứ tự", null, (s, e) => ApplyNumberedList(richTextBox_Content));
        }
        public static void ApplyNumberedList(RichTextBox richTextBox)
        {
            // Lưu vị trí con trỏ hiện tại
            int selectionStart = richTextBox.SelectionStart;
            int selectionLength = richTextBox.SelectionLength;

            if (selectionLength == 0)
            {
                MessageBox.Show("Vui lòng chọn văn bản cần đánh số thứ tự.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Tách văn bản đã chọn thành các dòng
            string selectedText = richTextBox.SelectedText;
            string[] lines = selectedText.Split(new[] { '\n' }, StringSplitOptions.None);

            // Duyệt qua từng dòng
            int currentIndex = selectionStart;
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                // Chèn số thứ tự trước dòng
                richTextBox.Select(currentIndex, line.Length);
                string numberedLine = $"{i + 1}. {line}";

                // Lưu định dạng hiện tại
                Font currentFont = richTextBox.SelectionFont;

                // Thay thế nội dung dòng với số thứ tự
                richTextBox.SelectedText = numberedLine;

                // Đặt lại định dạng
                richTextBox.Select(currentIndex, numberedLine.Length);
                richTextBox.SelectionFont = currentFont;

                // Cập nhật chỉ số cho dòng tiếp theo
                currentIndex += numberedLine.Length + 1; // +1 cho ký tự xuống dòng
            }

            // Đặt lại vùng chọn sau khi hoàn thành
            richTextBox.Select(selectionStart, currentIndex - selectionStart - 1);
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
        private ComboBox comboBox_Size;
        private ComboBox comboBox_Font;
        private RichTextBox richTextBox_Content;
        private ListBox listBox_Docs;
        private Button button_Connect;
        private ContextMenuStrip contextMenu_Table;
        private Button button_LineCounter;
        private Button button_LineSpace;
        private ContextMenuStrip contextMenuStrip_Table;
        private ToolStripTextBox toolStripTextBox1;
        private Button button_NewFile;
        private Button button_Download;
        private Button button_HighLight;
        private Button button_textColor;
        private Button button_LoadFile;
        private Button button_DeleteFile;
        private ContextMenuStrip contextMenuStrip_headIcon;
    }
}
