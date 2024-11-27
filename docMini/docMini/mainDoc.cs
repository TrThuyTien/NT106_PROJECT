using System.Drawing.Printing;
using System.IO.Compression;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using RichTextBox = System.Windows.Forms.RichTextBox;
using Task = System.Threading.Tasks.Task;
namespace docMini
{
    public partial class mainDoc : Form
    {
        int idUser = 0;
        int idDoc = 0;
        string nameUser = "";
        string nameDoc = "";
        public mainDoc(int userID, string userName)
        {
            InitializeComponent();
            InitializeTableContextMenu();
            SetRichTextBoxPadding(richTextBox_Content, 65, 65, 65, 65);
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
            public RECT1 rc;          // Kích thước vùng vẽ
            public RECT1 rcPage;      // Kích thước toàn bộ trang
            public int chrg_cpMin;   // Vị trí ký tự bắt đầu
            public int chrg_cpMax;   // Vị trí ký tự kết thúc
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT1
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        private struct RECT2
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private const int EM_FORMATRANGE = 0x0439; // Mã thông điệp EM_FORMATRANGE
        private void SaveRichTextBoxAsPdf(System.Windows.Forms.RichTextBox richTextBox, string outputPdfPath)
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
                        rc = new RECT1
                        {
                            Left = 0,
                            Top = 0,
                            Right = (int)(e.PageBounds.Width * 14.4),  // Đơn vị TWIPS
                            Bottom = (int)(e.PageBounds.Height * 14.4) // Đơn vị TWIPS
                        },
                        rcPage = new RECT1
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
        private const int EM_SETRECT = 0xB3;
        private void SetRichTextBoxPadding(RichTextBox rtb, int left, int top, int right, int bottom)
        {
            var rect = new RECT2
            {
                Left = left,
                Top = top,
                Right = rtb.Width - right,
                Bottom = rtb.Height - bottom
            };

            // Gửi thông điệp EM_SETRECT để thiết lập vùng chỉnh sửa
            IntPtr lParam = Marshal.AllocHGlobal(Marshal.SizeOf(rect));
            Marshal.StructureToPtr(rect, lParam, true);
            SendMessage(rtb.Handle, EM_SETRECT, IntPtr.Zero, lParam);
            Marshal.FreeHGlobal(lParam);
        }
        private void button_Minimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button_Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button_newFile_Click(object sender, EventArgs e)
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
            System.Windows.Forms.TextBox textBox_FileName = new System.Windows.Forms.TextBox
            {
                PlaceholderText = "Nhập tên tài liệu...",
                Dock = DockStyle.Top,
                Margin = new Padding(10),
            };

            // Tạo nút "Tạo Tài Liệu"
            System.Windows.Forms.Button button_Create = new System.Windows.Forms.Button
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

                try
                {
                    string serverResponse = await SendCreateNewFileAsync(fileName);
                    if (serverResponse.StartsWith($"NEW_FILE|{idUser}|"))
                    {
                        // Phân tích gói tin phản hồi từ server để lấy ID nếu thành công
                        var responseParts = serverResponse.Split('|');
                        if (responseParts[2] == "SUCCESS")
                        {
                            idDoc = int.Parse(responseParts[3]);

                            MessageBox.Show(
                                "Tạo tài liệu thành công!",
                                "Thông báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );
                            tempForm.Close();
                            richTextBox_Content.ReadOnly = false;
                            label_DocumentName.Text = fileName;
                            richTextBox_Content.Clear();
                            GetAllFile();
                        }
                        else if (responseParts[2] == "DUPLICATE")
                        {
                            MessageBox.Show(
                                "Tên tài liệu đã tồn tại. Vui lòng nhập tên khác!",
                                "Lỗi",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                            textBox_FileName.Clear();
                        }
                        else
                        {
                            MessageBox.Show(
                                "Tạo tài liệu thất bại. Vui lòng thử lại!",
                                "Lỗi",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                            textBox_FileName.Clear();
                        }
                    }
                }
                catch (SocketException ex)
                {
                    MessageBox.Show($"Lỗi kết nối đến server: {ex.Message}",
                                    "Lỗi",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}",
                                    "Lỗi",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }
            };
            tempForm.ShowDialog();
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
                int selectionStart = richTextBox_Content.SelectionStart;
                int selectionLength = richTextBox_Content.SelectionLength;

                // Áp dụng kích thước mới cho từng ký tự trong vùng chọn
                for (int i = 0; i < selectionLength; i++)
                {
                    richTextBox_Content.Select(selectionStart + i, 1); // Chọn từng ký tự
                    if (richTextBox_Content.SelectionFont != null)
                    {
                        System.Drawing.Font currentFont = richTextBox_Content.SelectionFont;
                        richTextBox_Content.SelectionFont = new System.Drawing.Font(
                            currentFont.FontFamily, newSize, currentFont.Style
                        );
                    }
                }

                // Đặt lại vùng chọn ban đầu
                richTextBox_Content.Select(selectionStart, selectionLength);
            }
            else
            {
                // Không có vùng chọn, áp dụng font mới cho văn bản nhập sau này
                if (richTextBox_Content.SelectionFont != null)
                {
                    System.Drawing.Font currentFont = richTextBox_Content.SelectionFont;

                    // Áp dụng cỡ chữ mới và loại bỏ các định dạng nếu cần
                    richTextBox_Content.SelectionFont = new System.Drawing.Font(
                        currentFont.FontFamily,
                        newSize,
                        System.Drawing.FontStyle.Regular // Đặt về định dạng cơ bản (không in đậm, in nghiêng)
                    );
                }
                else
                {
                    // Trường hợp không có SelectionFont, đặt font mặc định
                    richTextBox_Content.SelectionFont = new System.Drawing.Font(
                        "Tahoma", newSize, System.Drawing.FontStyle.Regular
                    );
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
            comboBox_Font.SelectedIndex = comboBox_Font.FindString("Tahoma");
            comboBox_Size.SelectedIndex = comboBox_Size.FindString("8");
        }

        private void button_AddPicture_Click(object sender, EventArgs e)
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
        private void button_LineSpace_Click(object sender, EventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Nhập chỉ số cách dòng:", "Cài đặt khoảng cách dòng", "1", -1, -1);

            // Kiểm tra giá trị nhập vào có hợp lệ không
            if (float.TryParse(input, out float lineSpacing))
            {
                SetLineSpacing(richTextBox_Content, lineSpacing);
            }
            else
            {
                MessageBox.Show("Vui lòng nhập một số hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void SetLineSpacing(System.Windows.Forms.RichTextBox richTextBox, float lineSpacing)
        {
            richTextBox.SuspendLayout();

            // Lấy toàn bộ nội dung của RichTextBox
            richTextBox.SelectAll();

            // Thiết lập khoảng cách dòng thông qua thuộc tính SelectionCharOffset
            float emHeight = richTextBox.SelectionFont.GetHeight(richTextBox.CreateGraphics());
            int lineSpacingInPixels = (int)(emHeight * (lineSpacing - 1));

            richTextBox.SelectionCharOffset = lineSpacingInPixels;

            // Đặt lại vùng chọn ban đầu
            richTextBox.DeselectAll();
            richTextBox.ResumeLayout();
        }

        private const int EM_GETCHARFORMAT = 0x043A;
        private const int EM_SETCHARFORMAT = 0x0444;
        private const int SCF_SELECTION = 0x0001;

        [StructLayout(LayoutKind.Sequential)]
        private struct CHARFORMAT2
        {
            public uint cbSize;
            public uint dwMask;
            public uint dwEffects;
            public int yHeight;
            public int yOffset;
            public uint crTextColor;
            public byte bCharSet;
            public byte bPitchAndFamily;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szFaceName;
            public ushort wWeight;
            public short sSpacing;
            public int crBackColor;
            public int lcid;
            public int dwReserved;
            public short sStyle;
            public short wKerning;
            public byte bUnderlineType;
            public byte bAnimation;
            public byte bRevAuthor;
            public byte bReserved1;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, ref CHARFORMAT2 lParam);

        public static void ApplyBulletPoints(RichTextBox richTextBox)
        {
            int selectionStart = richTextBox.SelectionStart;
            int selectionLength = richTextBox.SelectionLength;

            string[] lines = richTextBox.SelectedText.Split(new[] { '\n' }, StringSplitOptions.None);

            int currentPosition = selectionStart;

            foreach (var line in lines)
            {
                // Lấy định dạng hiện tại của dòng
                var charFormat = new CHARFORMAT2();
                charFormat.cbSize = (uint)Marshal.SizeOf(charFormat);

                IntPtr result = SendMessage(richTextBox.Handle, EM_GETCHARFORMAT, new IntPtr(SCF_SELECTION), ref charFormat);
                if (result == IntPtr.Zero)
                {
                    System.Windows.Forms.MessageBox.Show("Lỗi khi lấy định dạng.");
                    return;
                }

                // Thêm số thứ tự
                string bullet = $"{Array.IndexOf(lines, line) + 1}. ";
                richTextBox.Select(currentPosition, 0);
                richTextBox.SelectedText = bullet;

                // Áp dụng lại định dạng gốc
                currentPosition += bullet.Length;
                richTextBox.Select(currentPosition, line.Length);
                SendMessage(richTextBox.Handle, EM_SETCHARFORMAT, new IntPtr(SCF_SELECTION), ref charFormat);

                // Cập nhật vị trí tiếp theo
                currentPosition += line.Length + 1; // +1 cho ký tự xuống dòng
            }
        }
        private void button_LineCounter_Click(object sender, EventArgs e)
        {
            ApplyBulletPoints(richTextBox_Content);
        }

        private void button_textColor_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    // Áp dụng màu chữ cho đoạn văn bản được chọn
                    richTextBox_Content.SelectionColor = colorDialog.Color;
                }
            }
        }
        private void button_HighLight_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    // Áp dụng màu nền cho đoạn văn bản được chọn
                    richTextBox_Content.SelectionBackColor = colorDialog.Color;
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

                saveFileDialog.FileName = "newFile";
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


        private void button_ShareDoc_Click(object sender, EventArgs e)
        {
            // Tạo form tạm thời
            Form tempForm = new Form
            {
                Text = "Chia sẻ tài liệu",
                Width = 400,
                Height = 250,
                StartPosition = FormStartPosition.CenterParent
            };

            // Tạo TextBox để nhập tên người dùng
            System.Windows.Forms.TextBox textBox_UserName = new System.Windows.Forms.TextBox
            {
                PlaceholderText = "Nhập tên người dùng...",
                Dock = DockStyle.Top,
                Margin = new Padding(10),
            };

            // Tạo ComboBox để chọn chế độ chia sẻ
            System.Windows.Forms.ComboBox comboBox_Mode = new System.Windows.Forms.ComboBox
            {
                Dock = DockStyle.Top,
                Margin = new Padding(10),
                DropDownStyle = ComboBoxStyle.DropDownList,
            };

            // Thêm các tùy chọn vào ComboBox
            comboBox_Mode.Items.Add("Chỉnh sửa");
            comboBox_Mode.Items.Add("Xem");
            comboBox_Mode.SelectedIndex = 0; // Mặc định chọn "Chỉnh sửa"

            // Tạo nút "Chia sẻ Tài Liệu"
            System.Windows.Forms.Button button_Share = new System.Windows.Forms.Button
            {
                Text = "Chia sẻ",
                Dock = DockStyle.Bottom,
                Height = 40
            };

            // Thêm các điều khiển vào form tạm thời
            tempForm.Controls.Add(textBox_UserName);
            tempForm.Controls.Add(comboBox_Mode);
            tempForm.Controls.Add(button_Share);

            // Xử lý sự kiện nhấn nút "Chia sẻ tài liệu"
            button_Share.Click += async (s, args) =>
            {
                string userToAdd = textBox_UserName.Text.Trim();
                string mode = comboBox_Mode.SelectedItem.ToString();

                if (string.IsNullOrWhiteSpace(userToAdd))
                {
                    MessageBox.Show("Tên người dùng không được để trống!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                try
                {
                    string serverResponse = await SendShareFileAsync(userToAdd, mode);
                    if (serverResponse.StartsWith($"SHARE_FILE|{idUser}|{idDoc}|"))
                    {
                        // Phân tích gói tin phản hồi từ server để lấy ID nếu thành công
                        var responseParts = serverResponse.Split('|');
                        if (responseParts[3] == "SUCCESS")
                        {
                            MessageBox.Show(
                                "Chia sẻ tài liệu thành công!",
                                "Thông báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );
                            tempForm.Close();
                        }
                        else if (responseParts[3] == "USER_NOT_FOUND")
                        {
                            MessageBox.Show(
                                "Người dùng không tồn tại. Vui lòng kiểm tra lại.",
                                "Thông báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );
                            textBox_UserName.Clear();
                        }
                        else
                        {
                            MessageBox.Show(
                                "Bạn không có quyền share tài liệu này",
                                "Lỗi",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                            tempForm.Close();
                        }
                    }
                }
                catch (SocketException ex)
                {
                    MessageBox.Show($"Lỗi kết nối đến server: {ex.Message}",
                                    "Lỗi",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}",
                                    "Lỗi",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }
            };

            tempForm.ShowDialog();
        }


        // PHẦN CODE LIÊN QUAN CHUNG GIỮA LOGIC DOC VÀ CLIENT
        private void richTextBox_Content_TextChanged(object sender, EventArgs e)
        {
            richTextBox_Content_TextChangedButton(sender, e); // Gọi hàm xử lý định dạng
            richTextBox_Content_TextChangedHandler(sender, e); // Gọi hàm xử lý cập nhật nội dung
        }

        private void mainDoc_Load(object sender, EventArgs e)
        {
            mainDoc_LoadFormat(sender, e); // Load defaut
            mainDoc_LoadInfoUser(sender, e); // Load userName in mainDoc
            mainDoc_LoadConnection(sender, e); // Mở kết nối
            mainDoc_LoadGetAllFile(sender, e); // Load tất cả các file

            // Custome list file
            listBox_Docs.DrawMode = DrawMode.OwnerDrawFixed;
            listBox_Docs.ItemHeight = 50; // Tăng chiều cao hàng

            // Sự kiện vẽ item
            listBox_Docs.DrawItem += (s, e) =>
            {
                e.DrawBackground();
                if (e.Index >= 0)
                {
                    // Thêm icon và căn giữa theo chiều dọc
                    System.Drawing.Image icon = Properties.Resources.doc;
                    int iconSize = 32; // Kích thước icon
                    int iconTop = e.Bounds.Top + (e.Bounds.Height - iconSize) / 2; // Căn giữa icon theo chiều dọc
                    e.Graphics.DrawImage(icon, e.Bounds.Left + 5, iconTop, iconSize, iconSize);

                    // Lấy chỉ trường Text
                    dynamic item = listBox_Docs.Items[e.Index];
                    string displayText = item.Text; // Chỉ lấy giá trị từ Text

                    // Vẽ text
                    e.Graphics.DrawString(
                        displayText,
                        e.Font,
                        Brushes.Black,
                        e.Bounds.Left + iconSize + 10, // Dịch sang phải sau icon
                        e.Bounds.Top + (e.Bounds.Height - e.Font.Height) / 2 // Căn giữa text theo chiều dọc
                    );
                }
                e.DrawFocusRectangle();
            };



        }

        private void mainDoc_LoadGetAllFile(object sender, EventArgs e)
        {
            GetAllFile();
        }

        private void mainDoc_LoadInfoUser(object sender, EventArgs e)
        {
            label_NameAccount.Text = nameUser;
        }

        //------------------------------------------------------------------------------------
        // PHẦN KẾT NỐI VỚI SERVER -------------------------------------------------------------------------
        private TcpClient tcpClient;
        private NetworkStream stream;
        private int serverPort = 8080;
        private string serverIP = "127.0.0.1";
        private bool isConnected = false;
        private string lastReceivedContent = ""; // Lưu nội dung lần cuối để so sánh
        private static readonly TimeSpan debounceTime = TimeSpan.FromMilliseconds(300); // Thời gian debounce
        private DateTime lastSendTime = DateTime.MinValue;
        private bool isFormatting = false; // Cờ để kiểm soát việc định dạng
        private StringBuilder pendingUpdates = new StringBuilder(); // Lưu trữ các thay đổi tạm thời
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private async void GetAllFile()
        {
            try
            {
                string serverResponse = await SendGetAllFileRequestAsync();
                if (serverResponse.StartsWith($"GET_ALL_FILE|{idUser}|"))
                {
                    var responseParts = serverResponse.Split('|');
                    if (responseParts[2] == "SUCCESS")
                    {
                        string allFileName = responseParts[3];

                        // Khởi tạo Dictionary để lưu kết quả
                        Dictionary<int, string> userDocs = new Dictionary<int, string>();
                        string[] lines = allFileName.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                        foreach (string line in lines)
                        {
                            string[] parts = line.Split('@');
                            if (parts.Length == 2)
                            {
                                if (int.TryParse(parts[0], out int docId))
                                {
                                    string docName = parts[1];
                                    userDocs[docId] = docName;
                                }
                            }
                        }

                        // Cập nhật ListBox với dữ liệu mới
                        listBox_Docs.Items.Clear();

                        foreach (var doc in userDocs)
                        {
                            // Tạo một đối tượng mới để lưu trữ thông tin
                            var item = new { Text = doc.Value, Tag = doc.Key }; // Sử dụng anonymous object

                            // Thêm mục vào ListBox
                            listBox_Docs.Items.Add(item);
                        }

                        // Để hiển thị tên tài liệu trong ListBox
                        listBox_Docs.DisplayMember = "Text";
                        listBox_Docs.ValueMember = "Tag";
                    }
                    else
                    {
                        MessageBox.Show(
                            "Tải tài liệu thất bại. Vui lòng thử lại!",
                            "Lỗi",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                }
            }
            catch (SocketException ex)
            {
                MessageBox.Show($"Lỗi kết nối đến server: {ex.Message}",
                                "Lỗi",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}",
                                "Lỗi",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private async void listBox_Docs_DoubleClick(object sender, EventArgs e)
        {
            if (listBox_Docs.SelectedItem != null)
            {
                dynamic selectedItem = listBox_Docs.SelectedItem;

                string selectedDocName = selectedItem.Text;
                int selectedDocID = selectedItem.Tag;

                label_DocumentName.Text = selectedDocName;

                // Hủy các tác vụ liên quan đến tài liệu cũ
                cancellationTokenSource.Cancel();
                await Task.Delay(100); // Đợi các tác vụ cũ dừng hẳn
                cancellationTokenSource = new CancellationTokenSource();

                // Mở tài liệu mới
                await OpenDocumentAsync(selectedDocName, selectedDocID, cancellationTokenSource.Token);
            }
        }


        private async Task OpenDocumentAsync(string docName, int docID, CancellationToken token)
        {
            try
            {
                if (!isConnected || tcpClient == null || !tcpClient.Connected)
                {
                    await ReconnectToServerAsync();
                    if (!isConnected)
                    {
                        MessageBox.Show("Không thể mở tài liệu vì không kết nối được với server.");
                        return;
                    }
                }

                /*MessageBox.Show("Client gửi: " + $"EDIT_DOC|{docID}|{idUser}|");*/
                string serverResponse = await SendContentFileRequestAsync(docID);
                if (serverResponse.StartsWith($"EDIT_DOC|{docID}|{idUser}|"))
                {
                    /*MessageBox.Show("Nhận được phản hồi");*/
                    var responseParts = serverResponse.Split('|');
                    if (responseParts.Length == 5)
                    {
                        string content = responseParts[4];
                        int editStatus = int.Parse(responseParts[3]);

                        // Đặt cờ isFormatting để ngăn sự kiện TextChanged
                        isFormatting = true;
                        try
                        {
                            // Cập nhật nội dung lên RichTextBox
                            richTextBox_Content.Invoke((MethodInvoker)(() =>
                            {
                                richTextBox_Content.Rtf = content;
                                richTextBox_Content.Enabled = true;
                                richTextBox_Content.ReadOnly = editStatus != 1;
                            }));
                        }
                        finally
                        {
                            // Đảm bảo cờ được tắt
                            isFormatting = false;
                        }

                        // Gán tài liệu mới
                        idDoc = docID;
                        nameDoc = docName;
                        label_DocumentName.Text = nameDoc;
                        // Bắt đầu nhận nội dung mới
                        _ = Task.Run(() => ReceiveContentAsync(docID, token), token);
                    }
                }
                else
                {
                    MessageBox.Show($"Không thể mở tài liệu: phản hồi từ server không hợp lệ.\n{serverResponse}",
                                    "Lỗi",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }
            }
            catch (OperationCanceledException)
            {
                // Tác vụ bị hủy, không cần xử lý gì thêm
            }
            catch (SocketException ex)
            {
                MessageBox.Show($"Lỗi kết nối đến server: {ex.Message}",
                                "Lỗi",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}",
                                "Lỗi",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }



        private async void mainDoc_LoadConnection(object sender, EventArgs e)
        {
            try
            {
                tcpClient = new TcpClient();
                await tcpClient.ConnectAsync(serverIP, serverPort);
                isConnected = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error connecting to server: {ex.Message}");
            }
        }

        // Lấy tất cả file Doc của người dùng

        private async Task<string> SendGetAllFileRequestAsync()
        {
            using (tcpClient = new TcpClient())
            {
                await tcpClient.ConnectAsync(serverIP, serverPort);
                using (stream = tcpClient.GetStream())
                {
                    // Gửi yêu cầu tạo file
                    string message = $"GET_ALL_FILE|{idUser}";
                    await SendDataAsync(message);
                    // Nhận phản hồi từ server
                    string serverResponse = await ReceiveDataAsync();
                    return serverResponse;
                }
            }
        }

        // Gửi yêu cầu tạo file mới tới server
        private async Task<string> SendCreateNewFileAsync(string fileName)
        {
            using (tcpClient = new TcpClient())
            {
                await tcpClient.ConnectAsync(serverIP, serverPort);
                using (stream = tcpClient.GetStream())
                {
                    // Gửi yêu cầu tạo file
                    string message = $"NEW_FILE|{idUser}|{fileName}";
                    await SendDataAsync(message);
                    // Nhận phản hồi từ server
                    string serverResponse = await ReceiveDataAsync();
                    return serverResponse;
                }
            }
        }

        // Gửi yêu cầu chia sẻ file cho người dùng khác
        private async Task<string> SendShareFileAsync(string usernameToAdd, string mode)
        {
            using (tcpClient = new TcpClient())
            {
                await tcpClient.ConnectAsync(serverIP, serverPort);
                using (stream = tcpClient.GetStream())
                {
                    // Gửi yêu cầu tạo file
                    string message = $"SHARE_FILE|{idUser}|{idDoc}|{usernameToAdd}|{mode}";
                    await SendDataAsync(message);
                    // Nhận phản hồi từ server
                    string serverResponse = await ReceiveDataAsync();
                    return serverResponse;
                }
            }
        }

        // Gửi yêu cầu lấy nội dung file
        private async Task<string> SendContentFileRequestAsync(int docID)
        {
            if (tcpClient == null || !tcpClient.Connected)
            {
                throw new InvalidOperationException("Không có kết nối với server.");
            }

            string message = $"EDIT_DOC|{docID}|{idUser}";
            await SendDataAsync(message);
            return await ReceiveDataAsync();
        }


        private async Task SendDataAsync(string message)
        {
            // Chuyển đổi chuỗi thành mảng byte
            byte[] data = Encoding.UTF8.GetBytes(message);

            // Nén dữ liệu
            byte[] compressedData = Compress(data);

            // Chuyển đổi độ dài dữ liệu đã nén thành mảng byte
            byte[] lengthData = BitConverter.GetBytes(compressedData.Length);

            // Gửi độ dài dữ liệu trước
            await stream.WriteAsync(lengthData, 0, lengthData.Length);

            // Gửi dữ liệu đã nén
            await stream.WriteAsync(compressedData, 0, compressedData.Length);
        }


        private async Task<string> ReceiveDataAsync()
        {
            // Đọc độ dài dữ liệu từ server
            byte[] lengthBuffer = new byte[sizeof(int)];
            int bytesRead = await stream.ReadAsync(lengthBuffer, 0, lengthBuffer.Length);
            if (bytesRead != sizeof(int))
            {
                throw new Exception("Không thể đọc kích thước phản hồi từ server.");
            }

            // Chuyển đổi mảng byte thành số nguyên (độ dài dữ liệu)
            int compressedDataLength = BitConverter.ToInt32(lengthBuffer, 0);

            // Đọc dữ liệu nén từ server
            byte[] compressedData = new byte[compressedDataLength];
            bytesRead = await stream.ReadAsync(compressedData, 0, compressedData.Length);
            if (bytesRead != compressedDataLength)
            {
                throw new Exception("Phản hồi từ server không đầy đủ.");
            }

            // Giải nén dữ liệu
            byte[] decompressedData = Decompress(compressedData);

            // Chuyển đổi mảng byte đã giải nén thành chuỗi và trả về
            return Encoding.UTF8.GetString(decompressedData);
        }

        private void richTextBox_Content_TextChangedHandler(object sender, EventArgs e)

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

            try
            {
                // Chờ debounce
                await Task.Delay(debounceTime);

                if (pendingUpdates.Length > 0)
                {
                    try
                    {
                        // Kiểm tra và mở lại kết nối nếu cần
                        if (tcpClient == null || !tcpClient.Connected)
                        {
                            await ReconnectToServerAsync();
                        }

                        if (tcpClient != null && tcpClient.Connected)
                        {
                            string message = $"EDIT_DOC|{idDoc}|{idUser}|" + pendingUpdates.ToString();
                            await SendDataAsync(message); // Gửi dữ liệu
                        }
                        else
                        {
                            MessageBox.Show("Không thể gửi dữ liệu vì không kết nối được với server.");
                            isConnected = false;
                        }

                        pendingUpdates.Clear(); // Xóa dữ liệu đã gửi
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error sending data: {ex.Message}");
                        isConnected = false;
                    }
                }
            }
            catch (TaskCanceledException)
            {
                // Tác vụ bị hủy, không cần xử lý gì thêm
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Main Doc Error in DebouncedSendAsync: {ex.Message}");
            }
        }

        /// Hàm mở lại kết nối với server nếu kết nối bị mất
        private async Task ReconnectToServerAsync()
        {
            try
            {
                if (tcpClient != null)
                {
                    tcpClient.Dispose(); // Hủy kết nối cũ nếu tồn tại
                }

                tcpClient = new TcpClient();
                await tcpClient.ConnectAsync(serverIP, serverPort); // Kết nối lại với server
                stream = tcpClient.GetStream(); // Lấy stream mới
                isConnected = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Main Doc Không thể kết nối lại với server: {ex.Message}");
                isConnected = false;
            }
        }

        private async Task ReceiveContentAsync(int expectedDocID, CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    // Kiểm tra kết nối
                    if (!isConnected || tcpClient == null || !tcpClient.Connected)
                        break;

                    // Đọc độ dài dữ liệu đã nén
                    byte[] lengthBuffer = new byte[sizeof(int)];
                    int bytesRead = await stream.ReadAsync(lengthBuffer, 0, lengthBuffer.Length, token);
                    if (bytesRead != sizeof(int))
                    {
                        throw new Exception("Nhan du lieu: Không thể đọc kích thước dữ liệu từ server.");
                    }

                    int compressedLength = BitConverter.ToInt32(lengthBuffer, 0);

                    // Đọc dữ liệu đã nén
                    byte[] compressedData = new byte[compressedLength];
                    bytesRead = await stream.ReadAsync(compressedData, 0, compressedData.Length, token);
                    if (bytesRead != compressedLength)
                    {
                        throw new Exception("Nhan du lieu Không thể đọc đầy đủ dữ liệu từ server.");
                    }

                    // Giải nén dữ liệu
                    byte[] decompressedData = Decompress(compressedData);

                    // Chuyển đổi dữ liệu đã giải nén sang chuỗi
                    string response = Encoding.UTF8.GetString(decompressedData);

                    // Chỉ xử lý nếu DocID khớp với tài liệu hiện tại
                    if (response.StartsWith($"UPDATE_DOC|{expectedDocID}|"))
                    {
                        string[] parts = response.Split('|', 3);
                        if (parts.Length == 3)
                        {
                            string updatedContent = parts[2];

                            // Kiểm tra nếu nội dung là RTF hợp lệ
                            if (!string.IsNullOrEmpty(updatedContent) && updatedContent.StartsWith(@"{\rtf"))
                            {
                                isFormatting = true;
                                try
                                {
                                    richTextBox_Content.Invoke((MethodInvoker)(() =>
                                    {
                                        richTextBox_Content.Rtf = updatedContent;
                                    }));
                                }
                                catch (ArgumentException ex)
                                {
                                    MessageBox.Show($"Lỗi khi hiển thị nội dung RTF: {ex.Message}");
                                }
                                finally
                                {
                                    isFormatting = false;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Nội dung nhận được không phải là RTF hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Hủy tác vụ: không cần xử lý thêm
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi nhận dữ liệu từ server: {ex.Message}",
                                "Lỗi",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }



        private void ProcessMessages(Queue<string> messageQueue)
        {
            while (messageQueue.Count > 0)
            {
                string message;
                lock (messageQueue)
                {
                    message = messageQueue.Dequeue(); // Lấy thông điệp ra khỏi hàng đợi
                }

                // Xử lý nội dung thông điệp
                var responseParts = message.Split('|');
                if (responseParts.Length == 3)
                {
                    string content = responseParts[2];

                    // Cập nhật RichTextBox (chỉ 1 lần duy nhất)
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
