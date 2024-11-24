using System.IO.Compression;
using System.Net.Sockets;
using System.Text;
using System.Runtime.InteropServices;
using System.Data.SQLite;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using Microsoft.Office.Interop.Word;
using Task = System.Threading.Tasks.Task;
using Server;
using System.Windows.Controls;
using System.Drawing.Printing;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Menu;
namespace docMini
{
    public partial class mainDoc : Form
    {
        int idUser;
        int idDoc;
        string nameUser;
        string nameDoc;
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

        private void listBox_Docs_DoubleClick(object sender, EventArgs e)
        {
            if (listBox_Docs.SelectedItem != null)
            {
                // Lấy đối tượng đã chọn
                dynamic selectedItem = listBox_Docs.SelectedItem;

                // Lấy tên tài liệu và ID tài liệu
                string selectedDocName = selectedItem.Text; // Tên tài liệu
                int selectedDocID = selectedItem.Tag; // ID tài liệu
                idDoc = selectedDocID;
                nameDoc = selectedDocName;
                label_DocumentName.Text = selectedDocName;
                // Thực hiện hành động khi chọn tài liệu (ví dụ: mở tài liệu)
                OpenDocumentAsync(selectedDocName, selectedDocID);
            }
        }
        private async void OpenDocumentAsync(string docName, int docID)
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

                string serverResponse = await SendContentFileRequestAsync();
                if (serverResponse.StartsWith($"EDIT_DOC|{idDoc}|{idUser}"))
                {
                    var responseParts = serverResponse.Split('|');
                    if (responseParts.Length == 4)
                    {
                        string content = responseParts[3];

                        // Hiển thị nội dung lên RichTextBox
                        richTextBox_Content.Invoke((MethodInvoker)(() =>
                        {
                            richTextBox_Content.Rtf = content;
                            richTextBox_Content.ReadOnly = false;
                            richTextBox_Content.Enabled = true;
                        }));

                    }
                }
                else
                {
                    MessageBox.Show("Không thể mở tài liệu: phản hồi từ server không hợp lệ.",
                                    "Lỗi",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
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

        
        private async void mainDoc_LoadConnection(object sender, EventArgs e)
        {
            try
            {
                tcpClient = new TcpClient();
                await tcpClient.ConnectAsync(serverIP, serverPort);
                isConnected = true;
                // Nhận nội dung tài liệu liên tục từ server
                _ = receiveContentAsync();

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

        // Gửi yêu cầu lấy nội dung file
        private async Task<string> SendContentFileRequestAsync()
        {
            if (tcpClient == null || !tcpClient.Connected)
            {
                throw new InvalidOperationException("Không có kết nối với server.");
            }

            string message = $"EDIT_DOC|{idDoc}|{idUser}";
            await SendDataAsync(message);
            return await ReceiveDataAsync();
        }


        private async Task SendDataAsync(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            byte[] lengthData = BitConverter.GetBytes(data.Length);

            // Gửi độ dài dữ liệu trước
            await stream.WriteAsync(lengthData, 0, lengthData.Length);
            // Gửi dữ liệu chính
            await stream.WriteAsync(data, 0, data.Length);
        }

        private async Task<string> ReceiveDataAsync()
        {
            // Đọc độ dài dữ liệu phản hồi
            byte[] lengthBuffer = new byte[sizeof(int)];
            int bytesRead = await stream.ReadAsync(lengthBuffer, 0, lengthBuffer.Length);
            if (bytesRead != sizeof(int))
            {
                throw new Exception("Không thể đọc kích thước phản hồi từ server.");
            }
            int responseLength = BitConverter.ToInt32(lengthBuffer, 0);

            // Đọc nội dung phản hồi
            byte[] responseBuffer = new byte[responseLength];
            bytesRead = await stream.ReadAsync(responseBuffer, 0, responseBuffer.Length);
            if (bytesRead != responseLength)
            {
                throw new Exception("Phản hồi từ server không đầy đủ.");
            }
            return Encoding.UTF8.GetString(responseBuffer, 0, bytesRead);
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

        private async Task receiveContentAsync()
        {
            using (BinaryReader reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true))
            {
                while (isConnected)
                {
                    try
                    {
                        int length = reader.ReadInt32();
                        byte[] buffer = reader.ReadBytes(length);

                        // Nhận dữ liệu
                        string responseMessage = Encoding.UTF8.GetString(buffer);
                        if (responseMessage.StartsWith($"EDIT_DOC|{idDoc}|"))
                        {
                            var responseParts = responseMessage.Split('|');
                            if (responseParts.Length == 3)
                            { 
                                string content = responseParts[2];
                                if (richTextBox_Content.InvokeRequired)
                                {
                                    richTextBox_Content.Invoke((MethodInvoker)(() =>
                                    {
                                        richTextBox_Content.Rtf = content;
                                    }));
                                }
                                richTextBox_Content.Enabled = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Main Doc Error receiving data: {ex.Message}");
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

        private void button_LineCounter_Click(object sender, EventArgs e)
        {
            string[] lines = richTextBox_Content.Lines;
            var nonEmptyLines = lines.Where(line => !string.IsNullOrWhiteSpace(line)).ToArray();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < nonEmptyLines.Length; i++)
            {
                sb.Append(i + 1).Append(". ").Append(nonEmptyLines[i]).Append("\n");
            }
            richTextBox_Content.Text = sb.ToString();
            richTextBox_Content.ResumeLayout();
        }
        // ---------------------------------------------------------------------------------

    }
}
