using System.IO.Compression;
using System.Net.Sockets;
using System.Text;
using System.Runtime.InteropServices;
using System.Data.SQLite;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Drawing.Printing;
namespace docMini
{
    public partial class mainDoc : Form
    {
        int idUser;
        string nameUser;
        public mainDoc(int userID, string userName)
        {
            InitializeComponent();
            InitializeTableContextMenu();
            richTextBox_Content.LinkClicked += new LinkClickedEventHandler(richTextBox_Content_LinkClicked);
            idUser = userID;
            nameUser = userName;
            richTextBox_Content.ReadOnly = true;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FORMATRANGE
        {
            public IntPtr hdc;       // HDC để vẽ nội dung
            public IntPtr hdcTarget; // HDC mục tiêu (là máy in)
            public RECT rc;          // Kích thước vùng vẽ
            public RECT rcPage;      // Kích thước toàn bộ trang
            public int chrg_cpMin;   // Vị trí ký tự bắt đầu
            public int chrg_cpMax;   // Vị trí ký tự kết thúc
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private const int EM_FORMATRANGE = 0x0439; // Mã thông điệp EM_FORMATRANGE
        private void SaveRichTextBoxAsPdf(RichTextBox richTextBox, string outputPdfPath)
        {
            using (PrintDocument printDoc = new PrintDocument())
            {
                // Chọn máy in "Microsoft Print to PDF"
                printDoc.PrinterSettings.PrinterName = "Microsoft Print to PDF";
                printDoc.PrinterSettings.PrintToFile = true;
                printDoc.PrinterSettings.PrintFileName = outputPdfPath;

                printDoc.PrintPage += (sender, e) =>
                {
                    // Lấy HDC từ máy in
                    IntPtr hdc = e.Graphics.GetHdc();

                    // Cấu hình FORMATRANGE
                    FORMATRANGE formatRange = new FORMATRANGE
                    {
                        hdc = hdc,
                        hdcTarget = hdc,
                        rc = new RECT
                        {
                            Left = 0,
                            Top = 0,
                            Right = (int)(e.PageBounds.Width * 14.4),  // Đơn vị TWIPS
                            Bottom = (int)(e.PageBounds.Height * 14.4) // Đơn vị TWIPS
                        },
                        rcPage = new RECT
                        {
                            Left = 0,
                            Top = 0,
                            Right = (int)(e.PageBounds.Width * 14.4),
                            Bottom = (int)(e.PageBounds.Height * 14.4)
                        },
                        chrg_cpMin = 0,  // Vị trí bắt đầu in
                        chrg_cpMax = -1  // In toàn bộ nội dung
                    };

                    // Gửi thông điệp EM_FORMATRANGE để vẽ nội dung
                    IntPtr lParam = Marshal.AllocHGlobal(Marshal.SizeOf(formatRange));
                    Marshal.StructureToPtr(formatRange, lParam, false);
                    SendMessage(richTextBox.Handle, EM_FORMATRANGE, (IntPtr)1, lParam);

                    // Kết thúc in và giải phóng tài nguyên
                    SendMessage(richTextBox.Handle, EM_FORMATRANGE, IntPtr.Zero, IntPtr.Zero);
                    Marshal.FreeHGlobal(lParam);
                    e.Graphics.ReleaseHdc(hdc);
                };

                // Bắt đầu in
                try
                {
                    printDoc.Print();
                    MessageBox.Show("Lưu file PDF thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi lưu file PDF: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void button_Minimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button_Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void button_newFile_Click(object sender, EventArgs e)
        {
            // Tạo form tạm thời
            Form tempForm = new Form
            {
                Text = "Tạo Tài Liệu Mới",
                Width = 400,
                Height = 200,
                StartPosition = FormStartPosition.CenterParent
            };

            // Tạo textbox để nhập tên tài liệu
            TextBox textBox_FileName = new TextBox
            {
                PlaceholderText = "Nhập tên tài liệu...",
                Dock = DockStyle.Top,
                Margin = new Padding(10),
            };

            // Tạo nút "Tạo Tài Liệu"
            Button button_Create = new Button
            {
                Text = "Tạo Tài Liệu",
                Dock = DockStyle.Bottom,
                Height = 40
            };

            // Thêm các điều khiển vào form tạm thời
            tempForm.Controls.Add(textBox_FileName);
            tempForm.Controls.Add(button_Create);

            // Xử lý sự kiện nhấn nút "Tạo Tài Liệu"
            button_Create.Click += async (s, args) =>
            {
                string fileName = textBox_FileName.Text.Trim();

                if (string.IsNullOrWhiteSpace(fileName))
                {
                    MessageBox.Show("Tên tài liệu không được để trống!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string request = $"NEWFILE|{fileName}|{idUser}";

                try
                {
                    NetworkStream stream = tcpClient.GetStream();
                    byte[] requestBuffer = Encoding.UTF8.GetBytes(request);
                    await stream.WriteAsync(BitConverter.GetBytes(requestBuffer.Length), 0, sizeof(int));
                    await stream.WriteAsync(requestBuffer, 0, requestBuffer.Length);

                    // Đọc phản hồi từ server
                    byte[] responseBuffer = new byte[1024];
                    int responseLength = await stream.ReadAsync(responseBuffer, 0, responseBuffer.Length);
                    string response = Encoding.UTF8.GetString(responseBuffer, 0, responseLength);

                    if (response.StartsWith("SUCCESS"))
                    {
                        MessageBox.Show("Tạo tài liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        tempForm.Close();
                        richTextBox_Content.ReadOnly = false;
                    }
                    else if (response.StartsWith("INVALID_USER"))
                    {
                        MessageBox.Show("Không tìm thấy người dùng!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        textBox_FileName.Clear();
                    }
                    else if (response.StartsWith("DUPLICATE"))
                    {
                        MessageBox.Show("Tên tài liệu đã tồn tại. Vui lòng nhập tên khác!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        textBox_FileName.Clear();
                    }
                    else
                    {
                        MessageBox.Show("Tạo tài liệu thất bại. Vui lòng thử lại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        textBox_FileName.Clear();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi kết nối server: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            tempForm.ShowDialog();
        }

        private void button_ShareDoc_Click(object sender, EventArgs e)
        {

        }

        private string currentFont = "Tahoma";
        private float currentFontSize = 12f;
        private bool isBold = false;
        private bool isItalic = false;
        private bool isUnderline = false;
        private void richTextBox_Content_TextChangedButton(object sender, EventArgs e)
        {
            if (isBold && richTextBox_Content.SelectionLength == 0)
            {
                int selectionStart = richTextBox_Content.SelectionStart;

                if (selectionStart > 0) // Đảm bảo không tràn chỉ mục
                {
                    // Lấy ký tự trước đó
                    richTextBox_Content.Select(selectionStart - 1, 1);
                    if (richTextBox_Content.SelectionFont != null)
                    {
                        System.Drawing.Font currentFont = richTextBox_Content.SelectionFont;
                        System.Drawing.FontStyle newFontStyle = currentFont.Style | System.Drawing.FontStyle.Bold;
                        richTextBox_Content.SelectionFont = new System.Drawing.Font(currentFont.FontFamily, currentFont.Size, newFontStyle);
                    }

                    // Đặt lại con trỏ
                    richTextBox_Content.Select(selectionStart, 0);
                }
            }
            if (isItalic && richTextBox_Content.SelectionLength == 0)
            {
                int selectionStart = richTextBox_Content.SelectionStart;

                if (selectionStart > 0) // Đảm bảo không tràn chỉ mục
                {
                    // Lấy ký tự trước đó
                    richTextBox_Content.Select(selectionStart - 1, 1);
                    if (richTextBox_Content.SelectionFont != null)
                    {
                        System.Drawing.Font currentFont = richTextBox_Content.SelectionFont;
                        System.Drawing.FontStyle newFontStyle = currentFont.Style | System.Drawing.FontStyle.Italic;
                        richTextBox_Content.SelectionFont = new System.Drawing.Font(currentFont.FontFamily, currentFont.Size, newFontStyle);
                    }

                    // Đặt lại con trỏ
                    richTextBox_Content.Select(selectionStart, 0);
                }
            }
            if (isUnderline && richTextBox_Content.SelectionLength == 0)
            {
                int selectionStart = richTextBox_Content.SelectionStart;

                if (selectionStart > 0) // Đảm bảo không tràn chỉ mục
                {
                    // Lấy ký tự trước đó
                    richTextBox_Content.Select(selectionStart - 1, 1);
                    if (richTextBox_Content.SelectionFont != null)
                    {
                        System.Drawing.Font currentFont = richTextBox_Content.SelectionFont;
                        System.Drawing.FontStyle newFontStyle = currentFont.Style | System.Drawing.FontStyle.Underline;
                        richTextBox_Content.SelectionFont = new System.Drawing.Font(currentFont.FontFamily, currentFont.Size, newFontStyle);
                    }

                    // Đặt lại con trỏ
                    richTextBox_Content.Select(selectionStart, 0);
                }
            }

        }
        private System.Drawing.Font ApplyFontSizeToSelection(System.Drawing.Font originalFont, float newSize)
        {
            if (originalFont != null)
            {
                return new System.Drawing.Font(originalFont.FontFamily, newSize, originalFont.Style);
            }
            return new System.Drawing.Font("Tahoma", newSize); // Font mặc định nếu không có font
        }
        private void comboBox_Size_SelectedIndexChanged(object sender, EventArgs e)
        {
            float newSize;
            if (!float.TryParse(comboBox_Size.SelectedItem.ToString(), out newSize))
                return; // Thoát nếu không thể chuyển đổi kích thước

            if (richTextBox_Content.SelectionLength > 0)
            {
                // Áp dụng kích thước mới cho vùng chọn
                int selectionStart = richTextBox_Content.SelectionStart;
                int selectionLength = richTextBox_Content.SelectionLength;

                richTextBox_Content.SelectionFont = ApplyFontSizeToSelection(richTextBox_Content.SelectionFont, newSize);
                richTextBox_Content.Select(selectionStart, selectionLength); // Đặt lại vùng chọn
            }
            else
            {
                // Không có vùng chọn, áp dụng font mới cho ký tự tiếp theo
                int selectionStart = richTextBox_Content.SelectionStart;

                if (richTextBox_Content.SelectionFont != null)
                {
                    System.Drawing.Font currentFont = richTextBox_Content.SelectionFont;
                    richTextBox_Content.SelectionFont = new System.Drawing.Font(currentFont.FontFamily, newSize, currentFont.Style);
                }
            }
        }
        private void comboBox_Font_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentFont = comboBox_Font.SelectedItem.ToString(); // Update the current font

            if (richTextBox_Content.SelectionLength > 0)
            {
                int selectionStart = richTextBox_Content.SelectionStart;
                int selectionLength = richTextBox_Content.SelectionLength;

                for (int i = 0; i < selectionLength; i++)
                {
                    richTextBox_Content.Select(selectionStart + i, 1);
                    if (richTextBox_Content.SelectionFont != null)
                    {
                        System.Drawing.Font currentFont = richTextBox_Content.SelectionFont;
                        richTextBox_Content.SelectionFont = new System.Drawing.Font(this.currentFont, currentFont.Size, currentFont.Style);
                    }
                }

                richTextBox_Content.Select(selectionStart, selectionLength);
            }
            if (richTextBox_Content.SelectionLength == 0)
            {
                int selectionStart = richTextBox_Content.SelectionStart;
                richTextBox_Content.Select(selectionStart - 0, 1);
                if (richTextBox_Content.SelectionFont != null)
                {
                    System.Drawing.Font currentFont = richTextBox_Content.SelectionFont;
                    richTextBox_Content.SelectionFont = new System.Drawing.Font(this.currentFont, currentFont.Size, currentFont.Style);
                }
                richTextBox_Content.Select(selectionStart, 0);
            }
        }
        private void button_Bold_Click(object sender, EventArgs e)
        {
            isBold = !isBold; // Toggle trạng thái in đậm

            if (richTextBox_Content.SelectionLength > 0)
            {
                int selectionStart = richTextBox_Content.SelectionStart;
                int selectionLength = richTextBox_Content.SelectionLength;

                // Lặp qua từng ký tự được chọn
                for (int i = 0; i < selectionLength; i++)
                {
                    richTextBox_Content.Select(selectionStart + i, 1);

                    if (richTextBox_Content.SelectionFont != null)
                    {
                        System.Drawing.Font currentFont = richTextBox_Content.SelectionFont;
                        System.Drawing.FontStyle newFontStyle;

                        // Thêm hoặc loại bỏ FontStyle.Bold
                        if (currentFont.Bold)
                            newFontStyle = currentFont.Style & ~System.Drawing.FontStyle.Bold; // Loại bỏ Bold
                        else
                            newFontStyle = currentFont.Style | System.Drawing.FontStyle.Bold; // Thêm Bold

                        // Cập nhật font mới
                        richTextBox_Content.SelectionFont = new System.Drawing.Font(currentFont.FontFamily, currentFont.Size, newFontStyle);
                    }
                }

                // Đặt lại vùng chọn ban đầu
                richTextBox_Content.Select(selectionStart, selectionLength);
            }
        }
        private void button_Italic_Click(object sender, EventArgs e)
        {
            isItalic = !isItalic; // Toggle trạng thái in đậm

            if (richTextBox_Content.SelectionLength > 0)
            {
                int selectionStart = richTextBox_Content.SelectionStart;
                int selectionLength = richTextBox_Content.SelectionLength;

                // Lặp qua từng ký tự được chọn
                for (int i = 0; i < selectionLength; i++)
                {
                    richTextBox_Content.Select(selectionStart + i, 1);

                    if (richTextBox_Content.SelectionFont != null)
                    {
                        System.Drawing.Font currentFont = richTextBox_Content.SelectionFont;
                        System.Drawing.FontStyle newFontStyle;

                        // Thêm hoặc loại bỏ FontStyle.Bold
                        if (currentFont.Italic)
                            newFontStyle = currentFont.Style & ~System.Drawing.FontStyle.Italic; // Loại bỏ Bold
                        else
                            newFontStyle = currentFont.Style | System.Drawing.FontStyle.Italic; // Thêm Bold

                        // Cập nhật font mới
                        richTextBox_Content.SelectionFont = new System.Drawing.Font(currentFont.FontFamily, currentFont.Size, newFontStyle);
                    }
                }

                // Đặt lại vùng chọn ban đầu
                richTextBox_Content.Select(selectionStart, selectionLength);
            }
        }

        private void button_Underline_Click(object sender, EventArgs e)
        {
            isUnderline = !isUnderline; // Toggle trạng thái in đậm

            if (richTextBox_Content.SelectionLength > 0)
            {
                int selectionStart = richTextBox_Content.SelectionStart;
                int selectionLength = richTextBox_Content.SelectionLength;

                // Lặp qua từng ký tự được chọn
                for (int i = 0; i < selectionLength; i++)
                {
                    richTextBox_Content.Select(selectionStart + i, 1);

                    if (richTextBox_Content.SelectionFont != null)
                    {
                        System.Drawing.Font currentFont = richTextBox_Content.SelectionFont;
                        System.Drawing.FontStyle newFontStyle;

                        // Thêm hoặc loại bỏ FontStyle.Bold
                        if (currentFont.Underline)
                            newFontStyle = currentFont.Style & ~System.Drawing.FontStyle.Underline; // Loại bỏ Bold
                        else
                            newFontStyle = currentFont.Style | System.Drawing.FontStyle.Underline; // Thêm Bold

                        // Cập nhật font mới
                        richTextBox_Content.SelectionFont = new System.Drawing.Font(currentFont.FontFamily, currentFont.Size, newFontStyle);
                    }
                }

                // Đặt lại vùng chọn ban đầu
                richTextBox_Content.Select(selectionStart, selectionLength);
            }
        }


        private void button_AlignLeft_Click(object sender, EventArgs e)
        {

            richTextBox_Content.SelectionAlignment = System.Windows.Forms.HorizontalAlignment.Left;

        }
        private const int EM_SETTYPOGRAPHYOPTIONS = 0x04CA;
        private const int TO_ADVANCEDTYPOGRAPHY = 0x1;
        private const int EM_SETPARAFORMAT = 0x447;
        private const int PFM_ALIGNMENT = 0x0008;
        private const int PFA_JUSTIFY = 0x4;



        private void button_Justify_Click(object sender, EventArgs e)
        {
            // Kích hoạt Typography Options cho RichTextBox
            SendMessage(richTextBox_Content.Handle, EM_SETTYPOGRAPHYOPTIONS, (IntPtr)TO_ADVANCEDTYPOGRAPHY, IntPtr.Zero);

            // Tạo cấu trúc PARAFORMAT2 và đặt căn lề hai bên
            PARAFORMAT2 paraFormat = new PARAFORMAT2();
            paraFormat.cbSize = (uint)Marshal.SizeOf(paraFormat);
            paraFormat.dwMask = PFM_ALIGNMENT;
            paraFormat.wAlignment = PFA_JUSTIFY;

            // Áp dụng căn lề hai bên cho đoạn văn bản đang được bôi đen
            IntPtr lParam = Marshal.AllocHGlobal(Marshal.SizeOf(paraFormat));
            Marshal.StructureToPtr(paraFormat, lParam, false);
            SendMessage(richTextBox_Content.Handle, EM_SETPARAFORMAT, IntPtr.Zero, lParam);
            Marshal.FreeHGlobal(lParam);
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct PARAFORMAT2
        {
            public uint cbSize;
            public uint dwMask;
            public short wNumbering;
            public short wEffects;
            public int dxStartIndent;
            public int dxRightIndent;
            public int dxOffset;
            public short wAlignment;
            public short cTabCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public int[] rgxTabs;
            public int dySpaceBefore;
            public int dySpaceAfter;
            public int dyLineSpacing;
            public short sStyle;
            public byte bLineSpacingRule;
            public byte bOutlineLevel;
            public short wShadingWeight;
            public short wShadingStyle;
            public short wNumberingStart;
            public short wNumberingStyle;
            public short wNumberingTab;
            public short wBorderSpace;
            public short wBorderWidth;
            public short wBorders;
        }


        private void button_AlignRight_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.HorizontalAlignment previousAlignment = System.Windows.Forms.HorizontalAlignment.Left;
            if (richTextBox_Content.SelectionAlignment == System.Windows.Forms.HorizontalAlignment.Right)
            {
                richTextBox_Content.SelectionAlignment = previousAlignment;
            }
            else
            {
                previousAlignment = richTextBox_Content.SelectionAlignment;
                richTextBox_Content.SelectionAlignment = System.Windows.Forms.HorizontalAlignment.Right;
            }
        }
        private void button_Center_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.HorizontalAlignment previousAlignment = System.Windows.Forms.HorizontalAlignment.Left;
            if (richTextBox_Content.SelectionAlignment == System.Windows.Forms.HorizontalAlignment.Center)
            {
                richTextBox_Content.SelectionAlignment = previousAlignment;
            }
            else
            {
                previousAlignment = richTextBox_Content.SelectionAlignment;
                richTextBox_Content.SelectionAlignment = System.Windows.Forms.HorizontalAlignment.Center;
            }
        }

        private void mainDoc_LoadFormat(object sender, EventArgs e)
        {
            // Điền danh sách các kiểu chữ vào ComboBox
            foreach (FontFamily font in FontFamily.Families)
            {
                comboBox_Font.Items.Add(font.Name);
            }

            // Điền danh sách các cỡ chữ vào ComboBox (ví dụ từ 8 đến 72)
            for (int i = 8; i <= 72; i++)
            {
                comboBox_Size.Items.Add(i);
            }
        }

        private async void button_AddPicture_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Get the path of specified file
                    string filePath = openFileDialog.FileName;

                    // Load the image
                    System.Drawing.Image img = System.Drawing.Image.FromFile(filePath);

                    // Copy the image to the clipboard
                    Clipboard.SetImage(img);

                    // Paste the image into the RichTextBox
                    richTextBox_Content.Paste();
                }
            }
        }
        private static string InsertTableInRichTextBox(int rows, int cols, int width)
        {
            StringBuilder stringTableRtf = new StringBuilder();
            stringTableRtf.Append(@"{\rtf1\ansi\deff0 ");

            for (int i = 0; i < rows; i++)
            {
                stringTableRtf.Append(@"\trowd");  // Start a new row

                for (int j = 0; j < cols; j++)
                {
                    int cellWidth = (j + 1) * width;  // Define cell width
                    stringTableRtf.Append(@"\clbrdrt\brdrth\brdrw20"); // Top border
                    stringTableRtf.Append(@"\clbrdrl\brdrth\brdrw20"); // Left border
                    stringTableRtf.Append(@"\clbrdrb\brdrth\brdrw20"); // Bottom border
                    stringTableRtf.Append(@"\clbrdrr\brdrth\brdrw20"); // Right border
                    stringTableRtf.Append(@"\cellx" + cellWidth.ToString());
                }

                // End the row and cell
                stringTableRtf.Append(@"\intbl \cell \row");
            }

            stringTableRtf.Append(@"\pard");
            stringTableRtf.Append(@"}");

            return stringTableRtf.ToString();
        }
        private void button_AddTable_Click(object sender, EventArgs e)
        {
            contextMenu_Table.Show(button_AddTable, 0, button_AddTable.Height);

        }
        private void button_AddLink_Click(object sender, EventArgs e)
        {
            string selectedText = richTextBox_Content.SelectedText; // Lấy đoạn văn bản được chọn
            addLink insertLinkForm = new addLink(selectedText); // Mở form nhập liên kết

            if (insertLinkForm.ShowDialog() == DialogResult.OK)
            {
                string linkText = insertLinkForm.LinkText; // Văn bản hiển thị
                string linkAddress = insertLinkForm.LinkAddress; // Địa chỉ liên kết

                if (!string.IsNullOrEmpty(selectedText))
                {
                    // Thay thế văn bản được chọn bằng liên kết
                    int selectionStart = richTextBox_Content.SelectionStart;
                    int selectionLength = richTextBox_Content.SelectionLength;
                    string rtfLink = $@"{{\rtf1\ansi {{\field{{\*\fldinst HYPERLINK ""{linkAddress}""}}{{\fldrslt {linkText}}}}}}}";
                    richTextBox_Content.SelectedRtf = rtfLink;
                    richTextBox_Content.SelectionStart = selectionStart; // Đặt lại con trỏ
                    richTextBox_Content.SelectionLength = selectionLength;
                }
                else
                {
                    // Thêm liên kết tại vị trí con trỏ
                    int selectionStart = richTextBox_Content.SelectionStart;
                    string rtfLink = $@"{{\rtf1\ansi {{\field{{\*\fldinst HYPERLINK ""{linkAddress}""}}{{\fldrslt {linkText}}}}}}}";
                    richTextBox_Content.SelectedRtf = rtfLink;
                    richTextBox_Content.SelectionStart = selectionStart + linkText.Length; // Đặt con trỏ sau liên kết
                }
            }
        }
        private void SaveRtf(string filePath)
        {
            try
            {
                richTextBox_Content.SaveFile(filePath, RichTextBoxStreamType.RichText);
                MessageBox.Show("File saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        private void button_Save_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Portable Document Format (*.pdf)|*.pdf|Rich Text Format (*.rtf)|*.rtf";
                saveFileDialog.Title = "Save File";

                saveFileDialog.FileName = "Document";
                if (saveFileDialog.FilterIndex == 1)
                {
                    saveFileDialog.FileName += ".pdf";
                }
                else
                {
                    saveFileDialog.FileName += ".rtf";
                }

                if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;

                    try
                    {
                        if (Path.GetExtension(filePath).ToLower() == ".pdf")
                        {
                            SaveRichTextBoxAsPdf(richTextBox_Content, filePath);
                        }
                        else
                        {
                            SaveRtf(filePath);
                        }

                        MessageBox.Show("File saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
        }
        private void LoadRtf(string filePath)
        {
            try
            {
                richTextBox_Content.LoadFile(filePath, RichTextBoxStreamType.RichText);
                MessageBox.Show("File loaded successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        private void button_Open_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Rich Text Format|*.rtf",
                Title = "Open a Rich Text File"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                LoadRtf(filePath);
            }
        }

        // PHẦN CODE LIÊN QUAN CHUNG GIỮA LOGIC DOC VÀ CLIENT
        private async void richTextBox_Content_TextChanged(object sender, EventArgs e)
        {
            richTextBox_Content_TextChangedButton(sender, e); // Gọi hàm xử lý định dạng
            richTextBox_Content_TextChangedHandler(sender, e); // Gọi hàm xử lý cập nhật nội dung
        }

        private void mainDoc_Load(object sender, EventArgs e)
        {
            mainDoc_LoadFormat(sender, e); // Load defaut
            mainDoc_LoadInfoUser(sender, e);
        }

        private void mainDoc_LoadInfoUser(object sender, EventArgs e)
        {
            label_NameAccount.Text = nameUser;
        }

        //------------------------------------------------------------------------------------
        // PHẦN KẾT NỐI VỚI SERVER -------------------------------------------------------------------------
        private TcpClient tcpClient;
        private NetworkStream stream;
        private bool isConnected = false;
        private string lastReceivedContent = ""; // Lưu nội dung lần cuối để so sánh
        private static readonly TimeSpan debounceTime = TimeSpan.FromMilliseconds(300); // Thời gian debounce
        private DateTime lastSendTime = DateTime.MinValue;
        private bool isFormatting = false; // Cờ để kiểm soát việc định dạng
        private StringBuilder pendingUpdates = new StringBuilder(); // Lưu trữ các thay đổi tạm thời
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private async void button_Connect_Click(object sender, EventArgs e)
        {
            try
            {
                tcpClient = new TcpClient();
                await tcpClient.ConnectAsync("127.0.0.1", 8080);
                stream = tcpClient.GetStream();
                isConnected = true;

                // Nhận nội dung hiện tại từ server
                _ = System.Threading.Tasks.Task.Run(() => receiveContentAsync());
                richTextBox_Content.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error connecting to server: {ex.Message}");
            }
        }
        private void richTextBox_ListFile_TextChanged(object sender, EventArgs e)
        {
            DatabaseManager dbManager = new DatabaseManager();

            // Gọi hàm lấy danh sách tài liệu mà người dùng đã tham gia
            List<string> userDocs = dbManager.GetUserDocsByUsername(nameUser);

            // Cập nhật danh sách tài liệu trong RichTextBox
            richTextBox_ListFile.Clear(); // Xóa nội dung cũ
            foreach (var doc in userDocs)
            {
                richTextBox_ListFile.AppendText(doc + Environment.NewLine);
            }
        }

        private async void richTextBox_Content_TextChangedHandler(object sender, EventArgs e)
        {
            if (isConnected && !isFormatting)
            {
                // Cập nhật nội dung trực tiếp lên RichTextBox cục bộ
                string updatedContent = richTextBox_Content.Rtf;

                // Nếu nội dung thay đổi so với lần cuối
                if (updatedContent != lastReceivedContent)
                {
                    // Lưu nội dung mới nhất vào lastReceivedContent để hiển thị cục bộ
                    lastReceivedContent = updatedContent;

                    // Thay vì gửi ngay, lưu nội dung vào pendingUpdates
                    pendingUpdates.Clear();
                    pendingUpdates.Append(updatedContent);

                    // Thực hiện gửi sau khoảng debounce
                    _ = debouncedSendAsync();
                }
            }
        }

        private async Task debouncedSendAsync()
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;

            try
            {
                await Task.Delay(debounceTime, token);

                if (pendingUpdates.Length > 0)
                {
                    byte[] bufferContent = Encoding.UTF8.GetBytes(pendingUpdates.ToString());
                    byte[] compressedContent = Compress(bufferContent);

                    try
                    {
                        using (BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true))
                        {
                            writer.Write(compressedContent.Length);
                            writer.Write(compressedContent);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error sending data: {ex.Message}");
                        isConnected = false;
                    }

                    pendingUpdates.Clear();
                }
            }
            catch (TaskCanceledException)
            {
                // Tác vụ bị hủy, không cần xử lý gì thêm
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in DebouncedSendAsync: {ex.Message}");
            }
        }

        private async System.Threading.Tasks.Task receiveContentAsync()
        {
            using (BinaryReader reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true))
            {
                while (isConnected)
                {
                    try
                    {
                        int length = reader.ReadInt32();
                        byte[] buffer = reader.ReadBytes(length);

                        // Giải nén dữ liệu nhận được
                        string content = Encoding.UTF8.GetString(Decompress(buffer));

                        // Chỉ cập nhật nếu nội dung thực sự thay đổi
                        if (content != lastReceivedContent)
                        {
                            lastReceivedContent = content;

                            // Tạm thời tắt cờ định dạng để tránh xung đột
                            isFormatting = true;

                            // Cập nhật RichTextBox trên luồng giao diện
                            if (richTextBox_Content.InvokeRequired)
                            {
                                richTextBox_Content.Invoke((MethodInvoker)(() =>
                                {
                                    richTextBox_Content.Rtf = content;
                                }));
                            }
                            else
                            {
                                richTextBox_Content.Rtf = content;
                            }

                            isFormatting = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error receiving data: {ex.Message}");
                        isConnected = false;
                        break; // Thoát khỏi vòng lặp nếu có lỗi
                    }
                }
            }
        }

        // Phương thức nén dữ liệu
        private static byte[] Compress(byte[] data)
        {
            using (var outputStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(outputStream, CompressionMode.Compress))
                {
                    gzipStream.Write(data, 0, data.Length);
                }
                return outputStream.ToArray();
            }
        }

        // Phương thức giải nén dữ liệu
        private static byte[] Decompress(byte[] compressedData)
        {
            using (var inputStream = new MemoryStream(compressedData))
            using (var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress))
            using (var outputStream = new MemoryStream())
            {
                gzipStream.CopyTo(outputStream);
                return outputStream.ToArray();
            }
        }


        // ---------------------------------------------------------------------------------

    }
}
