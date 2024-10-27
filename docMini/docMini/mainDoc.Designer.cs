using System.Drawing.Drawing2D;

namespace miniDoc
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
            button_Exit = new Button();
            button_Minimize = new Button();
            pictureBox_Avatar = new PictureBox();
            label_AccountName = new Label();
            label_DocumentName = new Label();
            pictureBox_Logo = new PictureBox();
            panel_Toolbar = new Panel();
            button_ShareDoc = new Button();
            button_AddLink = new Button();
            button_AddTable = new Button();
            button_AddPicture = new Button();
            button_AlignRight = new Button();
            button_Justify = new Button();
            button_Center = new Button();
            button_AlignLeft = new Button();
            button_UnderlineText = new Button();
            button_ItalicText = new Button();
            button_BoldText = new Button();
            numericUpDown_FontSize = new NumericUpDown();
            listBox_Font = new ListBox();
            panel_ToolbarBorder = new Panel();
            richTextBox_Content = new RichTextBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox_Avatar).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox_Logo).BeginInit();
            panel_Toolbar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_FontSize).BeginInit();
            panel_ToolbarBorder.SuspendLayout();
            SuspendLayout();
            // 
            // button_Exit
            // 
            button_Exit.FlatAppearance.BorderColor = SystemColors.Window;
            button_Exit.FlatAppearance.BorderSize = 0;
            button_Exit.FlatAppearance.MouseDownBackColor = SystemColors.Window;
            button_Exit.FlatAppearance.MouseOverBackColor = Color.FromArgb(205, 236, 255);
            button_Exit.FlatStyle = FlatStyle.Flat;
            button_Exit.Image = docMini.Properties.Resources.exit_32px;
            button_Exit.Location = new Point(1878, 0);
            button_Exit.Name = "button_Exit";
            button_Exit.Size = new Size(34, 37);
            button_Exit.TabIndex = 0;
            button_Exit.UseVisualStyleBackColor = true;
            button_Exit.Click += button_Exit_Click;
            // 
            // button_Minimize
            // 
            button_Minimize.FlatAppearance.BorderColor = Color.White;
            button_Minimize.FlatAppearance.BorderSize = 0;
            button_Minimize.FlatAppearance.MouseDownBackColor = SystemColors.Window;
            button_Minimize.FlatAppearance.MouseOverBackColor = Color.FromArgb(205, 236, 255);
            button_Minimize.FlatStyle = FlatStyle.Flat;
            button_Minimize.Image = docMini.Properties.Resources.link_button;
            button_Minimize.Location = new Point(1838, 0);
            button_Minimize.Name = "button_Minimize";
            button_Minimize.Size = new Size(34, 37);
            button_Minimize.TabIndex = 1;
            button_Minimize.UseVisualStyleBackColor = true;
            button_Minimize.Click += button_Minimize_Click;
            // 
            // pictureBox_Avatar
            // 
            pictureBox_Avatar.Location = new Point(1779, 1);
            pictureBox_Avatar.Name = "pictureBox_Avatar";
            pictureBox_Avatar.Size = new Size(36, 36);
            pictureBox_Avatar.SizeMode = PictureBoxSizeMode.CenterImage;
            pictureBox_Avatar.TabIndex = 2;
            pictureBox_Avatar.TabStop = false;
            // 
            // label_AccountName
            // 
            label_AccountName.AutoSize = true;
            label_AccountName.Font = new Font("Tahoma", 12F, FontStyle.Regular, GraphicsUnit.Point, 163);
            label_AccountName.Location = new Point(1640, 9);
            label_AccountName.Name = "label_AccountName";
            label_AccountName.Size = new Size(133, 24);
            label_AccountName.TabIndex = 3;
            label_AccountName.Text = "Tên tài khoản";
            label_AccountName.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label_DocumentName
            // 
            label_DocumentName.AutoSize = true;
            label_DocumentName.Font = new Font("Tahoma", 12F, FontStyle.Regular, GraphicsUnit.Point, 163);
            label_DocumentName.Location = new Point(42, 9);
            label_DocumentName.Name = "label_DocumentName";
            label_DocumentName.Size = new Size(252, 24);
            label_DocumentName.TabIndex = 5;
            label_DocumentName.Text = "Tên tài liệu đang chỉnh sửa";
            label_DocumentName.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // pictureBox_Logo
            // 
            pictureBox_Logo.Image = docMini.Properties.Resources.doc;
            pictureBox_Logo.Location = new Point(0, 1);
            pictureBox_Logo.Name = "pictureBox_Logo";
            pictureBox_Logo.Size = new Size(36, 36);
            pictureBox_Logo.SizeMode = PictureBoxSizeMode.CenterImage;
            pictureBox_Logo.TabIndex = 4;
            pictureBox_Logo.TabStop = false;
            // 
            // panel_Toolbar
            // 
            panel_Toolbar.BackColor = Color.White;
            panel_Toolbar.Controls.Add(button_ShareDoc);
            panel_Toolbar.Controls.Add(button_AddLink);
            panel_Toolbar.Controls.Add(button_AddTable);
            panel_Toolbar.Controls.Add(button_AddPicture);
            panel_Toolbar.Controls.Add(button_AlignRight);
            panel_Toolbar.Controls.Add(button_Justify);
            panel_Toolbar.Controls.Add(button_Center);
            panel_Toolbar.Controls.Add(button_AlignLeft);
            panel_Toolbar.Controls.Add(button_UnderlineText);
            panel_Toolbar.Controls.Add(button_ItalicText);
            panel_Toolbar.Controls.Add(button_BoldText);
            panel_Toolbar.Controls.Add(numericUpDown_FontSize);
            panel_Toolbar.Controls.Add(listBox_Font);
            panel_Toolbar.Location = new Point(3, 3);
            panel_Toolbar.Name = "panel_Toolbar";
            panel_Toolbar.Size = new Size(1906, 72);
            panel_Toolbar.TabIndex = 7;
            panel_Toolbar.Paint += panel_Toolbar_Paint;
            // 
            // button_ShareDoc
            // 
            button_ShareDoc.BackColor = Color.FromArgb(175, 219, 255);
            button_ShareDoc.FlatAppearance.BorderColor = SystemColors.Window;
            button_ShareDoc.FlatAppearance.BorderSize = 0;
            button_ShareDoc.FlatAppearance.MouseDownBackColor = SystemColors.Window;
            button_ShareDoc.FlatAppearance.MouseOverBackColor = Color.FromArgb(205, 236, 255);
            button_ShareDoc.FlatStyle = FlatStyle.Flat;
            button_ShareDoc.Font = new Font("Tahoma", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 163);
            button_ShareDoc.ImageAlign = ContentAlignment.MiddleLeft;
            button_ShareDoc.Location = new Point(1740, 9);
            button_ShareDoc.Name = "button_ShareDoc";
            button_ShareDoc.Padding = new Padding(16, 6, 8, 6);
            button_ShareDoc.Size = new Size(129, 53);
            button_ShareDoc.TabIndex = 15;
            button_ShareDoc.Text = "      Share";
            button_ShareDoc.TextAlign = ContentAlignment.MiddleRight;
            button_ShareDoc.UseVisualStyleBackColor = false;
            button_ShareDoc.Paint += button_ShareDoc_Paint;
            // 
            // button_AddLink
            // 
            button_AddLink.FlatAppearance.BorderColor = SystemColors.Window;
            button_AddLink.FlatAppearance.BorderSize = 0;
            button_AddLink.FlatAppearance.MouseDownBackColor = SystemColors.Window;
            button_AddLink.FlatAppearance.MouseOverBackColor = Color.FromArgb(205, 236, 255);
            button_AddLink.FlatStyle = FlatStyle.Flat;
            button_AddLink.Location = new Point(1074, 18);
            button_AddLink.Name = "button_AddLink";
            button_AddLink.Size = new Size(34, 37);
            button_AddLink.TabIndex = 14;
            button_AddLink.UseVisualStyleBackColor = true;
            // 
            // button_AddTable
            // 
            button_AddTable.FlatAppearance.BorderColor = Color.White;
            button_AddTable.FlatAppearance.BorderSize = 0;
            button_AddTable.FlatAppearance.MouseDownBackColor = SystemColors.Window;
            button_AddTable.FlatAppearance.MouseOverBackColor = Color.FromArgb(205, 236, 255);
            button_AddTable.FlatStyle = FlatStyle.Flat;
            button_AddTable.Location = new Point(1014, 18);
            button_AddTable.Name = "button_AddTable";
            button_AddTable.Size = new Size(34, 37);
            button_AddTable.TabIndex = 13;
            button_AddTable.UseVisualStyleBackColor = true;
            // 
            // button_AddPicture
            // 
            button_AddPicture.FlatAppearance.BorderColor = Color.White;
            button_AddPicture.FlatAppearance.BorderSize = 0;
            button_AddPicture.FlatAppearance.MouseDownBackColor = SystemColors.Window;
            button_AddPicture.FlatAppearance.MouseOverBackColor = Color.FromArgb(205, 236, 255);
            button_AddPicture.FlatStyle = FlatStyle.Flat;
            button_AddPicture.Location = new Point(956, 18);
            button_AddPicture.Name = "button_AddPicture";
            button_AddPicture.Size = new Size(34, 37);
            button_AddPicture.TabIndex = 12;
            button_AddPicture.UseVisualStyleBackColor = true;
            // 
            // button_AlignRight
            // 
            button_AlignRight.FlatAppearance.BorderColor = Color.White;
            button_AlignRight.FlatAppearance.BorderSize = 0;
            button_AlignRight.FlatAppearance.MouseDownBackColor = SystemColors.Window;
            button_AlignRight.FlatAppearance.MouseOverBackColor = Color.FromArgb(205, 236, 255);
            button_AlignRight.FlatStyle = FlatStyle.Flat;
            button_AlignRight.Location = new Point(761, 18);
            button_AlignRight.Name = "button_AlignRight";
            button_AlignRight.Size = new Size(34, 37);
            button_AlignRight.TabIndex = 11;
            button_AlignRight.UseVisualStyleBackColor = true;
            // 
            // button_Justify
            // 
            button_Justify.FlatAppearance.BorderColor = Color.White;
            button_Justify.FlatAppearance.BorderSize = 0;
            button_Justify.FlatAppearance.MouseDownBackColor = SystemColors.Window;
            button_Justify.FlatAppearance.MouseOverBackColor = Color.FromArgb(205, 236, 255);
            button_Justify.FlatStyle = FlatStyle.Flat;
            button_Justify.Location = new Point(815, 18);
            button_Justify.Name = "button_Justify";
            button_Justify.Size = new Size(34, 37);
            button_Justify.TabIndex = 10;
            button_Justify.UseVisualStyleBackColor = true;
            // 
            // button_Center
            // 
            button_Center.FlatAppearance.BorderColor = Color.White;
            button_Center.FlatAppearance.BorderSize = 0;
            button_Center.FlatAppearance.MouseDownBackColor = SystemColors.Window;
            button_Center.FlatAppearance.MouseOverBackColor = Color.FromArgb(205, 236, 255);
            button_Center.FlatStyle = FlatStyle.Flat;
            button_Center.Location = new Point(700, 16);
            button_Center.Name = "button_Center";
            button_Center.Size = new Size(34, 37);
            button_Center.TabIndex = 9;
            button_Center.UseVisualStyleBackColor = true;
            // 
            // button_AlignLeft
            // 
            button_AlignLeft.FlatAppearance.BorderColor = Color.White;
            button_AlignLeft.FlatAppearance.BorderSize = 0;
            button_AlignLeft.FlatAppearance.MouseDownBackColor = SystemColors.Window;
            button_AlignLeft.FlatAppearance.MouseOverBackColor = Color.FromArgb(205, 236, 255);
            button_AlignLeft.FlatStyle = FlatStyle.Flat;
            button_AlignLeft.Location = new Point(640, 18);
            button_AlignLeft.Name = "button_AlignLeft";
            button_AlignLeft.Size = new Size(34, 37);
            button_AlignLeft.TabIndex = 8;
            button_AlignLeft.UseVisualStyleBackColor = true;
            // 
            // button_UnderlineText
            // 
            button_UnderlineText.FlatAppearance.BorderColor = Color.White;
            button_UnderlineText.FlatAppearance.BorderSize = 0;
            button_UnderlineText.FlatAppearance.MouseDownBackColor = SystemColors.Window;
            button_UnderlineText.FlatAppearance.MouseOverBackColor = Color.FromArgb(205, 236, 255);
            button_UnderlineText.FlatStyle = FlatStyle.Flat;
            button_UnderlineText.Location = new Point(509, 18);
            button_UnderlineText.Name = "button_UnderlineText";
            button_UnderlineText.Size = new Size(34, 37);
            button_UnderlineText.TabIndex = 7;
            button_UnderlineText.UseVisualStyleBackColor = true;
            // 
            // button_ItalicText
            // 
            button_ItalicText.FlatAppearance.BorderColor = Color.White;
            button_ItalicText.FlatAppearance.BorderSize = 0;
            button_ItalicText.FlatAppearance.MouseDownBackColor = SystemColors.Window;
            button_ItalicText.FlatAppearance.MouseOverBackColor = Color.FromArgb(205, 236, 255);
            button_ItalicText.FlatStyle = FlatStyle.Flat;
            button_ItalicText.Location = new Point(447, 18);
            button_ItalicText.Name = "button_ItalicText";
            button_ItalicText.Size = new Size(34, 37);
            button_ItalicText.TabIndex = 3;
            button_ItalicText.UseVisualStyleBackColor = true;
            // 
            // button_BoldText
            // 
            button_BoldText.FlatAppearance.BorderColor = Color.White;
            button_BoldText.FlatAppearance.BorderSize = 0;
            button_BoldText.FlatAppearance.MouseDownBackColor = SystemColors.Window;
            button_BoldText.FlatAppearance.MouseOverBackColor = Color.FromArgb(205, 236, 255);
            button_BoldText.FlatStyle = FlatStyle.Flat;
            button_BoldText.Location = new Point(383, 18);
            button_BoldText.Name = "button_BoldText";
            button_BoldText.Size = new Size(34, 37);
            button_BoldText.TabIndex = 2;
            button_BoldText.UseVisualStyleBackColor = true;
            // 
            // numericUpDown_FontSize
            // 
            numericUpDown_FontSize.Font = new Font("Tahoma", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 163);
            numericUpDown_FontSize.Location = new Point(238, 23);
            numericUpDown_FontSize.Name = "numericUpDown_FontSize";
            numericUpDown_FontSize.Size = new Size(62, 29);
            numericUpDown_FontSize.TabIndex = 1;
            // 
            // listBox_Font
            // 
            listBox_Font.Font = new Font("Tahoma", 12F, FontStyle.Regular, GraphicsUnit.Point, 163);
            listBox_Font.FormattingEnabled = true;
            listBox_Font.ItemHeight = 24;
            listBox_Font.Items.AddRange(new object[] { "Time New Roman", "Cambria" });
            listBox_Font.Location = new Point(19, 23);
            listBox_Font.Name = "listBox_Font";
            listBox_Font.Size = new Size(190, 28);
            listBox_Font.TabIndex = 0;
            // 
            // panel_ToolbarBorder
            // 
            panel_ToolbarBorder.BackColor = Color.FromArgb(57, 119, 233);
            panel_ToolbarBorder.Controls.Add(panel_Toolbar);
            panel_ToolbarBorder.Location = new Point(0, 43);
            panel_ToolbarBorder.Name = "panel_ToolbarBorder";
            panel_ToolbarBorder.Size = new Size(1912, 78);
            panel_ToolbarBorder.TabIndex = 6;
            panel_ToolbarBorder.Paint += panel_ToolbarBorder_Paint;
            // 
            // richTextBox_Content
            // 
            richTextBox_Content.Location = new Point(643, 138);
            richTextBox_Content.Name = "richTextBox_Content";
            richTextBox_Content.Size = new Size(1007, 926);
            richTextBox_Content.TabIndex = 7;
            richTextBox_Content.Text = "";
            // 
            // mainDoc
            // 
            AutoScaleDimensions = new SizeF(8F, 18F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(205, 236, 255);
            ClientSize = new Size(1913, 1055);
            Controls.Add(richTextBox_Content);
            Controls.Add(panel_ToolbarBorder);
            Controls.Add(label_DocumentName);
            Controls.Add(pictureBox_Logo);
            Controls.Add(label_AccountName);
            Controls.Add(pictureBox_Avatar);
            Controls.Add(button_Minimize);
            Controls.Add(button_Exit);
            Font = new Font("Tahoma", 9F, FontStyle.Regular, GraphicsUnit.Point, 163);
            FormBorderStyle = FormBorderStyle.None;
            HelpButton = true;
            Name = "mainDoc";
            Text = "mainDoc";
            TopMost = true;
            WindowState = FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)pictureBox_Avatar).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox_Logo).EndInit();
            panel_Toolbar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)numericUpDown_FontSize).EndInit();
            panel_ToolbarBorder.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button_Exit;
        private Button button_Minimize;
        private PictureBox pictureBox_Avatar;
        private Label label_AccountName;
        private Label label_DocumentName;
        private PictureBox pictureBox_Logo;
        private Panel panel_Toolbar;

        private void panel_ToolbarBorder_Paint(object sender, PaintEventArgs e)
        {
            SetRoundedPanel(panel_ToolbarBorder, Color.FromArgb(57, 119, 233), 10, 30, Color.FromArgb(57, 119, 233));
        }
        private void panel_Toolbar_Paint(object sender, PaintEventArgs e)
        {
            SetRoundedPanel(panel_Toolbar, Color.White, 10, 30, Color.White);
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


        private Panel panel_ToolbarBorder;
        private Button button_UnderlineText;
        private Button button_ItalicText;
        private Button button_BoldText;
        private NumericUpDown numericUpDown_FontSize;
        private ListBox listBox_Font;
        private Button button_AlignRight;
        private Button button_Justify;
        private Button button_Center;
        private Button button_AlignLeft;
        private Button button_AddLink;
        private Button button_AddTable;
        private Button button_AddPicture;
        private Button button_ShareDoc;
        private RichTextBox richTextBox_Content;
    }
}