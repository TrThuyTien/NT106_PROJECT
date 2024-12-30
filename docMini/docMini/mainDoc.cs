using System.Drawing.Printing;
using System.IO.Compression;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using RichTextBox = System.Windows.Forms.RichTextBox;
using Task = System.Threading.Tasks.Task;
using Server;
using System.Drawing.Imaging;
using System.Security.Cryptography;
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
            // Khởi tạo menu kiểu đánh dấu
            InitializeBulletStyleMenu();
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
                printDoc.PrinterSettings.PrinterName = "Microsoft Print to PDF";
                printDoc.PrinterSettings.PrintToFile = true;
                printDoc.PrinterSettings.PrintFileName = outputPdfPath;

                int charFrom = 0; // Vị trí bắt đầu in
                int textLength = richTextBox.TextLength;

                printDoc.PrintPage += (sender, e) =>
                {
                    IntPtr hdc = IntPtr.Zero;

                    try
                    {
                        // Lấy HDC từ Graphics
                        hdc = e.Graphics.GetHdc();

                        FORMATRANGE formatRange = new FORMATRANGE
                        {
                            hdc = hdc,
                            hdcTarget = hdc,
                            rc = new RECT1
                            {
                                Left = (int)(e.MarginBounds.Left * 14.4),
                                Top = (int)(e.MarginBounds.Top * 14.4),
                                Right = (int)(e.MarginBounds.Right * 14.4),
                                Bottom = (int)(e.MarginBounds.Bottom * 14.4),
                            },
                            rcPage = new RECT1
                            {
                                Left = 0,
                                Top = 0,
                                Right = (int)(e.PageBounds.Width * 14.4),
                                Bottom = (int)(e.PageBounds.Height * 14.4),
                            },
                            chrg_cpMin = charFrom,
                            chrg_cpMax = richTextBox.TextLength
                        };

                        // Gửi FORMATRANGE để tính toán và vẽ nội dung
                        IntPtr lParam = Marshal.AllocHGlobal(Marshal.SizeOf(formatRange));
                        Marshal.StructureToPtr(formatRange, lParam, false);

                        int nextCharIndex = SendMessage(richTextBox.Handle, EM_FORMATRANGE, (IntPtr)1, lParam).ToInt32();
                        Marshal.FreeHGlobal(lParam);

                        if (nextCharIndex <= charFrom || nextCharIndex >= richTextBox.TextLength)
                        {
                            e.HasMorePages = false;
                        }
                        else
                        {
                            e.HasMorePages = true;
                            charFrom = nextCharIndex;
                        }
                    }
                    finally
                    {
                        // Giải phóng HDC
                        if (hdc != IntPtr.Zero)
                        {
                            e.Graphics.ReleaseHdc(hdc);
                        }

                        // Kết thúc in
                        SendMessage(richTextBox.Handle, EM_FORMATRANGE, IntPtr.Zero, IntPtr.Zero);
                    }
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

        private async void button_newFile_Click(object sender, EventArgs e)
        {
            // Hủy luồng nhận dữ liệu
            cancellationTokenSource.Cancel();
            await Task.Delay(100); // Đợi các tác vụ cũ dừng hẳn
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
                                "Lỗi tạo file mới",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                            textBox_FileName.Clear();
                        }
                        else
                        {
                            MessageBox.Show(
                                "Tạo tài liệu thất bại. Vui lòng thử lại!",
                                "Lỗi tạo file mới",
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
                                    "Lỗi tạo file mới",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}",
                                    "Lỗi tạo file mới",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }
            };
            tempForm.ShowDialog();
            cancellationTokenSource = new CancellationTokenSource();
            _ = Task.Run(() => UpdateContentLoop(idDoc, cancellationTokenSource.Token)).ConfigureAwait(false);
        }

        private bool isBold = false;
        private bool isItalic = false;
        private bool isUnderline = false;
        private void richTextBox_Content_TextChangedButton(object sender, EventArgs e)
        {

            if (isBold && richTextBox_Content.SelectionLength == 0)
            {
                // Kiểm soát trạng thái định dạng
                if (isFormatting) return;
                isFormatting = true;
                try
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
                finally
                {
                    // Kết thúc trạng thái định dạng
                    isFormatting = false;
                }

            }
            if (isItalic && richTextBox_Content.SelectionLength == 0)
            {
                // Kiểm soát trạng thái định dạng
                if (isFormatting) return;
                isFormatting = true;
                try
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
                finally
                {
                    // Kết thúc trạng thái định dạng
                    isFormatting = false;
                }

            }
            if (isUnderline && richTextBox_Content.SelectionLength == 0)
            {
                // Kiểm soát trạng thái định dạng
                if (isFormatting) return;
                isFormatting = true;
                try
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
                finally
                {
                    // Kết thúc trạng thái định dạng
                    isFormatting = false;
                }
            }
        }

        private void comboBox_Size_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isFormatting) return;
            isFormatting = true;

            try
            {
                if (!float.TryParse(comboBox_Size.SelectedItem.ToString(), out float newSize))
                    return;

                int selectionStart = richTextBox_Content.SelectionStart;
                int selectionLength = richTextBox_Content.SelectionLength;

                if (selectionLength > 0)
                {
                    var textFormatCache = new Dictionary<(int Start, int Length), System.Drawing.Font>();

                    int end = selectionStart + selectionLength;
                    richTextBox_Content.SuspendLayout();

                    int currentStart = selectionStart;
                    System.Drawing.Font currentFont = richTextBox_Content.SelectionFont;

                    for (int i = selectionStart; i < end; i++)
                    {
                        richTextBox_Content.Select(i, 1);
                        var charFont = richTextBox_Content.SelectionFont;

                        if (currentFont == null || !charFont.Equals(currentFont))
                        {
                            // Caching: Lưu đoạn và font hiện tại
                            if (!textFormatCache.ContainsKey((currentStart, i - currentStart)))
                            {
                                textFormatCache[(currentStart, i - currentStart)] = currentFont;
                            }

                            // Bắt đầu đoạn mới
                            currentFont = charFont;
                            currentStart = i;
                        }
                    }

                    // Xử lý đoạn cuối cùng
                    if (!textFormatCache.ContainsKey((currentStart, end - currentStart)))
                    {
                        textFormatCache[(currentStart, end - currentStart)] = currentFont;
                    }

                    // Lazy Rendering: Áp dụng các định dạng đã lưu
                    foreach (var range in textFormatCache)
                    {
                        ApplyFontToRange(range.Key.Start, range.Key.Length, newSize);
                    }

                    richTextBox_Content.Select(selectionStart, selectionLength);
                    richTextBox_Content.ResumeLayout();
                }
                else
                {
                    ApplyDefaultFont(newSize);
                }
            }
            finally
            {
                isFormatting = false;
            }
        }

        private void ApplyFontToRange(int start, int length, float newSize)
        {
            if (length <= 0) return;

            richTextBox_Content.Select(start, length);
            var currentFont = richTextBox_Content.SelectionFont;

            if (currentFont != null)
            {
                richTextBox_Content.SelectionFont = new System.Drawing.Font(
                    currentFont.FontFamily, newSize, currentFont.Style);
            }
        }

        private void ApplyDefaultFont(float newSize)
        {
            if (richTextBox_Content.SelectionFont != null)
            {
                var currentFont = richTextBox_Content.SelectionFont;
                richTextBox_Content.SelectionFont = new System.Drawing.Font(
                    currentFont.FontFamily, newSize, currentFont.Style);
            }
            else
            {
                richTextBox_Content.SelectionFont = new System.Drawing.Font("Tahoma", newSize, System.Drawing.FontStyle.Regular);
            }
        }

        private void comboBox_Font_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isFormatting) return;
            isFormatting = true;

            try
            {
                string newFontFamily = comboBox_Font.SelectedItem.ToString();
                int selectionStart = richTextBox_Content.SelectionStart;
                int selectionLength = richTextBox_Content.SelectionLength;

                if (selectionLength > 0)
                {
                    var textFormatCache = new Dictionary<(int Start, int Length), System.Drawing.Font>();

                    int end = selectionStart + selectionLength;
                    richTextBox_Content.SuspendLayout();

                    int currentStart = selectionStart;
                    System.Drawing.Font currentFont = richTextBox_Content.SelectionFont;

                    for (int i = selectionStart; i < end; i++)
                    {
                        richTextBox_Content.Select(i, 1);
                        var charFont = richTextBox_Content.SelectionFont;

                        if (currentFont == null || !charFont.Equals(currentFont))
                        {
                            // Caching: Lưu đoạn và font hiện tại
                            if (!textFormatCache.ContainsKey((currentStart, i - currentStart)))
                            {
                                textFormatCache[(currentStart, i - currentStart)] = currentFont;
                            }

                            // Bắt đầu đoạn mới
                            currentFont = charFont;
                            currentStart = i;
                        }
                    }

                    // Xử lý đoạn cuối cùng
                    if (!textFormatCache.ContainsKey((currentStart, end - currentStart)))
                    {
                        textFormatCache[(currentStart, end - currentStart)] = currentFont;
                    }

                    // Lazy Rendering: Áp dụng các định dạng đã lưu
                    foreach (var range in textFormatCache)
                    {
                        ApplyFontFamilyToRange(range.Key.Start, range.Key.Length, newFontFamily);
                    }

                    richTextBox_Content.Select(selectionStart, selectionLength);
                    richTextBox_Content.ResumeLayout();
                }
                else
                {
                    ApplyDefaultFontFamily(newFontFamily);
                }
            }
            finally
            {
                isFormatting = false;
            }
        }

        private void ApplyFontFamilyToRange(int start, int length, string newFontFamily)
        {
            if (length <= 0) return;

            richTextBox_Content.Select(start, length);
            var currentFont = richTextBox_Content.SelectionFont;

            if (currentFont != null)
            {
                richTextBox_Content.SelectionFont = new System.Drawing.Font(
                    newFontFamily, currentFont.Size, currentFont.Style);
            }
        }

        private void ApplyDefaultFontFamily(string newFontFamily)
        {
            if (richTextBox_Content.SelectionFont != null)
            {
                var currentFont = richTextBox_Content.SelectionFont;
                richTextBox_Content.SelectionFont = new System.Drawing.Font(
                    newFontFamily, currentFont.Size, currentFont.Style);
            }
            else
            {
                richTextBox_Content.SelectionFont = new System.Drawing.Font(
                    newFontFamily, 8, System.Drawing.FontStyle.Regular);
            }
        }

        private void button_Bold_Click(object sender, EventArgs e)
        {
            // Kiểm soát trạng thái định dạng
            if (isFormatting) return;
            isFormatting = true;
            try
            {
                isBold = !isBold; // Toggle trạng thái in đậm

                if (isBold)
                {
                    // Đổi màu nền hoặc viền của nút
                    button_Bold.BackColor = Color.LightGray;
                }
                else
                {
                    // Trả về màu nền mặc định
                    button_Bold.BackColor = SystemColors.Control;
                }
                if (richTextBox_Content.SelectionLength > 0)
                {
                    int selectionStart = richTextBox_Content.SelectionStart;
                    int selectionLength = richTextBox_Content.SelectionLength;

                    var textFormatCache = new Dictionary<(int Start, int Length), System.Drawing.Font>();

                    int end = selectionStart + selectionLength;
                    richTextBox_Content.SuspendLayout();

                    int currentStart = selectionStart;
                    System.Drawing.Font currentFont = richTextBox_Content.SelectionFont;

                    for (int i = selectionStart; i < end; i++)
                    {
                        richTextBox_Content.Select(i, 1);
                        var charFont = richTextBox_Content.SelectionFont;

                        if (currentFont == null || !charFont.Equals(currentFont))
                        {
                            // Caching: Lưu đoạn và font hiện tại
                            if (!textFormatCache.ContainsKey((currentStart, i - currentStart)))
                            {
                                textFormatCache[(currentStart, i - currentStart)] = currentFont;
                            }

                            // Bắt đầu đoạn mới
                            currentFont = charFont;
                            currentStart = i;
                        }
                    }

                    // Xử lý đoạn cuối cùng
                    if (!textFormatCache.ContainsKey((currentStart, end - currentStart)))
                    {
                        textFormatCache[(currentStart, end - currentStart)] = currentFont;
                    }

                    // Lazy Rendering: Áp dụng các định dạng đã lưu
                    foreach (var range in textFormatCache)
                    {
                        ApplyBoldToRange(range.Key.Start, range.Key.Length, isBold);
                    }

                    richTextBox_Content.Select(selectionStart, selectionLength);
                    richTextBox_Content.ResumeLayout();
                }



            }
            finally
            {
                // Kết thúc trạng thái định dạng
                isFormatting = false;
            }

        }
        private void ApplyBoldToRange(int start, int length, bool isBold)
        {
            if (length <= 0) return;

            richTextBox_Content.Select(start, length);
            var currentFont = richTextBox_Content.SelectionFont;

            if (currentFont != null)
            {
                System.Drawing.FontStyle newFontStyle;

                if (isBold)
                    newFontStyle = currentFont.Style | System.Drawing.FontStyle.Bold; // Thêm Bold
                else
                    newFontStyle = currentFont.Style & ~System.Drawing.FontStyle.Bold; // Loại bỏ Bold

                richTextBox_Content.SelectionFont = new System.Drawing.Font(
                    currentFont.FontFamily, currentFont.Size, newFontStyle);
            }
        }
        private void button_Italic_Click(object sender, EventArgs e)
        {
            // Kiểm soát trạng thái định dạng
            if (isFormatting) return;
            isFormatting = true;
            try
            {

                isItalic = !isItalic; // Toggle trạng thái in đậm
                if (isItalic)
                {
                    // Đổi màu nền hoặc viền của nút
                    button_Italic.BackColor = Color.LightGray;
                }
                else
                {
                    // Trả về màu nền mặc định
                    button_Italic.BackColor = SystemColors.Control;
                }
                if (richTextBox_Content.SelectionLength > 0)
                {
                    int selectionStart = richTextBox_Content.SelectionStart;
                    int selectionLength = richTextBox_Content.SelectionLength;

                    var textFormatCache = new Dictionary<(int Start, int Length), System.Drawing.Font>();

                    int end = selectionStart + selectionLength;
                    richTextBox_Content.SuspendLayout();

                    int currentStart = selectionStart;
                    System.Drawing.Font currentFont = richTextBox_Content.SelectionFont;

                    for (int i = selectionStart; i < end; i++)
                    {
                        richTextBox_Content.Select(i, 1);
                        var charFont = richTextBox_Content.SelectionFont;

                        if (currentFont == null || !charFont.Equals(currentFont))
                        {
                            // Caching: Lưu đoạn và font hiện tại
                            if (!textFormatCache.ContainsKey((currentStart, i - currentStart)))
                            {
                                textFormatCache[(currentStart, i - currentStart)] = currentFont;
                            }

                            // Bắt đầu đoạn mới
                            currentFont = charFont;
                            currentStart = i;
                        }
                    }

                    // Xử lý đoạn cuối cùng
                    if (!textFormatCache.ContainsKey((currentStart, end - currentStart)))
                    {
                        textFormatCache[(currentStart, end - currentStart)] = currentFont;
                    }

                    // Lazy Rendering: Áp dụng các định dạng đã lưu
                    foreach (var range in textFormatCache)
                    {
                        ApplyItalicToRange(range.Key.Start, range.Key.Length, isItalic);
                    }

                    richTextBox_Content.Select(selectionStart, selectionLength);
                    richTextBox_Content.ResumeLayout();

                }
            }
            finally
            {
                // Kết thúc trạng thái định dạng
                isFormatting = false;
            }

        }
        private void ApplyItalicToRange(int start, int length, bool isItalic)
        {
            if (length <= 0) return;
            richTextBox_Content.Select(start, length);
            var currentFont = richTextBox_Content.SelectionFont;

            if (currentFont != null)
            {
                System.Drawing.FontStyle newFontStyle;

                if (isItalic)
                    newFontStyle = currentFont.Style | System.Drawing.FontStyle.Italic;
                else
                    newFontStyle = currentFont.Style & ~System.Drawing.FontStyle.Italic;

                richTextBox_Content.SelectionFont = new System.Drawing.Font(
                    currentFont.FontFamily, currentFont.Size, newFontStyle);
            }
        }

        private void button_Underline_Click(object sender, EventArgs e)
        {
            // Kiểm soát trạng thái định dạng
            if (isFormatting) return;
            isFormatting = true;
            try
            {

                isUnderline = !isUnderline; // Toggle trạng thái in đậm
                if (isUnderline)
                {
                    // Đổi màu nền hoặc viền của nút
                    button_Underline.BackColor = Color.LightGray;
                }
                else
                {
                    // Trả về màu nền mặc định
                    button_Underline.BackColor = SystemColors.Control;
                }
                if (richTextBox_Content.SelectionLength > 0)
                {
                    int selectionStart = richTextBox_Content.SelectionStart;
                    int selectionLength = richTextBox_Content.SelectionLength;

                    var textFormatCache = new Dictionary<(int Start, int Length), System.Drawing.Font>();

                    int end = selectionStart + selectionLength;
                    richTextBox_Content.SuspendLayout();

                    int currentStart = selectionStart;
                    System.Drawing.Font currentFont = richTextBox_Content.SelectionFont;

                    for (int i = selectionStart; i < end; i++)
                    {
                        richTextBox_Content.Select(i, 1);
                        var charFont = richTextBox_Content.SelectionFont;

                        if (currentFont == null || !charFont.Equals(currentFont))
                        {
                            // Caching: Lưu đoạn và font hiện tại
                            if (!textFormatCache.ContainsKey((currentStart, i - currentStart)))
                            {
                                textFormatCache[(currentStart, i - currentStart)] = currentFont;
                            }

                            // Bắt đầu đoạn mới
                            currentFont = charFont;
                            currentStart = i;
                        }
                    }

                    // Xử lý đoạn cuối cùng
                    if (!textFormatCache.ContainsKey((currentStart, end - currentStart)))
                    {
                        textFormatCache[(currentStart, end - currentStart)] = currentFont;
                    }

                    // Lazy Rendering: Áp dụng các định dạng đã lưu
                    foreach (var range in textFormatCache)
                    {
                        ApplyUnderlineToRange(range.Key.Start, range.Key.Length, isUnderline);
                    }

                    richTextBox_Content.Select(selectionStart, selectionLength);
                    richTextBox_Content.ResumeLayout();

                }
            }
            finally
            {
                // Kết thúc trạng thái định dạng
                isFormatting = false;
            }
        }
        private void ApplyUnderlineToRange(int start, int length, bool isUnderline)
        {
            if (length <= 0) return;

            richTextBox_Content.Select(start, length);
            var currentFont = richTextBox_Content.SelectionFont;

            if (currentFont != null)
            {
                System.Drawing.FontStyle newFontStyle;

                if (isUnderline)
                    newFontStyle = currentFont.Style | System.Drawing.FontStyle.Underline;
                else
                    newFontStyle = currentFont.Style & ~System.Drawing.FontStyle.Underline;
                richTextBox_Content.SelectionFont = new System.Drawing.Font(
                    currentFont.FontFamily, currentFont.Size, newFontStyle);
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
            // Kiểm soát trạng thái định dạng
            if (isFormatting) return;
            isFormatting = true;
            try
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
            finally
            {
                // Kết thúc trạng thái định dạng
                isFormatting = false;
            }

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
            // Kiểm soát trạng thái định dạng
            if (isFormatting) return;
            isFormatting = true;
            try
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
            finally
            {
                // Kết thúc trạng thái định dạng
                isFormatting = false;
            }
        }
        private void button_Center_Click(object sender, EventArgs e)
        {
            // Kiểm soát trạng thái định dạng
            if (isFormatting) return;
            isFormatting = true;
            try
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
            finally
            {
                // Kết thúc trạng thái định dạng
                isFormatting = false;
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
            // Kiểm soát trạng thái định dạng
            if (isFormatting) return;
            isFormatting = true;
            try
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
            finally
            {
                // Kết thúc trạng thái định dạng
                isFormatting = false;
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
        private void button_AddLink_Click(object sender, EventArgs e)
        {
            // Kiểm soát trạng thái định dạng
            if (isFormatting) return;
            isFormatting = true;
            try
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
            finally
            {
                // Kết thúc trạng thái định dạng
                isFormatting = false;
            }

        }
        private void button_LineSpace_Click(object sender, EventArgs e)
        {
            // Kiểm soát trạng thái định dạng
            if (isFormatting) return;
            isFormatting = true;
            try
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
            finally
            {
                // Kết thúc trạng thái định dạng
                isFormatting = false;
            }

        }
        private void SetLineSpacing(System.Windows.Forms.RichTextBox richTextBox, float lineSpacing)
        {
            try
            {
                richTextBox.SuspendLayout();

                // Lưu lại vị trí chọn ban đầu
                int selectionStart = richTextBox.SelectionStart;
                int selectionLength = richTextBox.SelectionLength;

                // Áp dụng dãn dòng từng đoạn văn bản trong vùng chọn
                int endPosition = selectionStart + selectionLength;
                richTextBox.SelectionStart = selectionStart;

                while (richTextBox.SelectionStart < endPosition)
                {
                    // Đặt vùng chọn từng ký tự hoặc đoạn
                    richTextBox.SelectionLength = 1;

                    // Kiểm tra nếu font không null
                    if (richTextBox.SelectionFont != null)
                    {
                        float emHeight = richTextBox.SelectionFont.GetHeight(richTextBox.CreateGraphics());
                        int lineSpacingInPixels = (int)(emHeight * (lineSpacing - 1));

                        richTextBox.SelectionCharOffset = lineSpacingInPixels;
                    }

                    // Di chuyển con trỏ đến ký tự tiếp theo
                    richTextBox.SelectionStart += 1;
                }

                // Khôi phục lại vùng chọn ban đầu
                richTextBox.Select(selectionStart, selectionLength);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                richTextBox.ResumeLayout();
            }
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

        private List<int> lineStartIndices = new List<int>(); // Danh sách vị trí bắt đầu của các dòng đã đánh số
        public static void ApplyBulletPoints(RichTextBox richTextBox, string newBulletSymbol)
        {
            // Lưu vị trí con trỏ hiện tại
            int selectionStart = richTextBox.SelectionStart;
            int selectionLength = richTextBox.SelectionLength;

            if (selectionLength == 0)
            {
                MessageBox.Show("Vui lòng chọn văn bản cần đánh dấu đầu dòng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Tách văn bản đã chọn thành các dòng
            string selectedText = richTextBox.SelectedText;
            string[] lines = selectedText.Split(new[] { '\n' }, StringSplitOptions.None);

            // Mẫu nhận diện dấu đề mục cũ
            string bulletPattern = @"^(\s*[\u2022\u25E6\u25CF\u2219○●-]|\d+\.)\s";

            // Duyệt qua từng dòng
            int currentIndex = selectionStart;
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                // Nhận diện xem dòng có dấu đầu dòng hay không
                string newLine = line;
                Match match = Regex.Match(line, bulletPattern);
                if (match.Success)
                {
                    // Thay thế dấu đầu dòng cũ bằng dấu mới
                    newLine = $"{newBulletSymbol} {line.Substring(match.Length).TrimStart()}";
                }
                else
                {
                    // Thêm dấu đầu dòng nếu dòng không có
                    newLine = $"{newBulletSymbol} {line}";
                }

                // Chèn lại văn bản với dấu đầu dòng mới
                richTextBox.Select(currentIndex, line.Length);
                Font currentFont = richTextBox.SelectionFont; // Lưu định dạng hiện tại
                richTextBox.SelectedText = newLine;
                richTextBox.Select(currentIndex, newLine.Length);
                richTextBox.SelectionFont = currentFont;

                // Cập nhật chỉ số cho dòng tiếp theo
                currentIndex += newLine.Length + 1; // +1 cho ký tự xuống dòng
            }

            // Đặt lại vùng chọn sau khi hoàn thành
            richTextBox.Select(selectionStart, currentIndex - selectionStart - 1);
        }



        private void button_LineCounter_Click(object sender, EventArgs e)
        {
            // Kiểm soát trạng thái định dạng
            if (isFormatting) return;
            isFormatting = true;
            try
            {
                contextMenuStrip_headIcon.Show(button_LineCounter, 0, button_LineCounter.Height);
            }
            finally
            {
                // Kết thúc trạng thái định dạng
                isFormatting = false;
            }

        }


        private void button_textColor_Click(object sender, EventArgs e)
        {
            // Kiểm soát trạng thái định dạng
            if (isFormatting) return;
            isFormatting = true;
            try
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
            finally
            {
                // Kết thúc trạng thái định dạng
                isFormatting = false;
            }

        }
        private void button_HighLight_Click(object sender, EventArgs e)
        {
            // Kiểm soát trạng thái định dạng
            if (isFormatting) return;
            isFormatting = true;
            try
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
            finally
            {
                // Kết thúc trạng thái định dạng
                isFormatting = false;
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
        private async void button_ShareDoc_Click(object sender, EventArgs e)
        {
            // Hủy luồng nhận dữ liệu
            cancellationTokenSource.Cancel();
            await Task.Delay(100); // Đợi các tác vụ cũ dừng hẳn
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
                                "Lỗi share file",
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
                                    "Lỗi share file",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}",
                                    "Lỗi share file",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }
            };

            tempForm.ShowDialog();
            cancellationTokenSource = new CancellationTokenSource();
            _ = Task.Run(() => UpdateContentLoop(idDoc, cancellationTokenSource.Token)).ConfigureAwait(false);

        }

        private async void button_LoadFile_Click(object sender, EventArgs e)
        {
            // Hủy luồng nhận dữ liệu
            cancellationTokenSource.Cancel();
            await Task.Delay(100); // Đợi các tác vụ cũ dừng hẳn
            GetAllFile();
            cancellationTokenSource = new CancellationTokenSource();
            _ = Task.Run(() => UpdateContentLoop(idDoc, cancellationTokenSource.Token)).ConfigureAwait(false);
        }

        private async void button_DeleteFile_Click(object sender, EventArgs e)
        {
            // Hủy luồng nhận dữ liệu
            cancellationTokenSource.Cancel();
            await Task.Delay(100); // Đợi các tác vụ cũ dừng hẳn
            // Hiển thị hộp thoại xác nhận
            var confirmResult = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa file \"{nameDoc}\" không?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            // Nếu người dùng chọn Yes thì bắt đầu xóa
            if (confirmResult == DialogResult.Yes)
            {
                try
                {
                    // Gửi yêu cầu xóa file đến server
                    string serverResponse = await SendDeleteFileAsync();
                    if (serverResponse.StartsWith($"DELETE_FILE|{idUser}|{idDoc}|"))
                    {
                        // Phân tích gói tin phản hồi từ server
                        var responseParts = serverResponse.Split('|');
                        if (responseParts[3] == "SUCCESS")
                        {
                            MessageBox.Show(
                                $"Xóa file \"{nameDoc}\" thành công!",
                                "Thông báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );
                            richTextBox_Content.Clear();
                            richTextBox_Content.ReadOnly = true;
                            idDoc = 0;
                            nameDoc = "";
                            label_DocumentName.Text = "* Tạo file mới";
                            GetAllFile();
                        }
                        else if (responseParts[3] == "FILE_NOT_FOUND")
                        {
                            MessageBox.Show(
                                $"File \"{nameDoc}\" không tồn tại. Vui lòng load lại danh sách file.",
                                "Thông báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );
                            richTextBox_Content.Clear();
                            richTextBox_Content.ReadOnly = true;
                        }
                        else
                        {
                            MessageBox.Show(
                                "Xóa file thất bại",
                                "Lỗi",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                        }
                    }
                }
                catch (SocketException ex)
                {
                    MessageBox.Show(
                        $"Lỗi kết nối đến server: {ex.Message}",
                        "Lỗi xóa file",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Đã xảy ra lỗi: {ex.Message}",
                        "Lỗi xóa file",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
            else
            {
                // Người dùng chọn No, không thực hiện xóa
                MessageBox.Show(
                    "Hủy thao tác xóa file.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            cancellationTokenSource = new CancellationTokenSource();
            _ = Task.Run(() => UpdateContentLoop(idDoc, cancellationTokenSource.Token)).ConfigureAwait(false);
        }


        private void richTextBox_Content_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (isFormatting) return; // Ngăn chặn xung đột định dạng
                isFormatting = true;

                try
                {
                    int selectionStart = richTextBox_Content.SelectionStart;

                    // Lưu font hiện tại trước khi nhấn Enter
                    System.Drawing.Font currentFont = null;
                    if (selectionStart > 0)
                    {
                        richTextBox_Content.Select(selectionStart - 1, 1);
                        if (richTextBox_Content.SelectionFont != null)
                        {
                            currentFont = richTextBox_Content.SelectionFont;
                        }
                    }

                    // Chèn dòng mới
                    richTextBox_Content.Select(selectionStart, 0);
                    richTextBox_Content.SelectedText = "\n";

                    // Áp dụng lại kiểu chữ và cỡ chữ sau khi Enter
                    if (currentFont != null)
                    {
                        richTextBox_Content.Select(selectionStart + 1, 0); // Di chuyển đến sau dòng mới
                        richTextBox_Content.SelectionFont = new System.Drawing.Font(
                            currentFont.FontFamily,
                            currentFont.Size,
                            currentFont.Style
                        );
                    }

                    // Kiểm tra và sao chép dấu đầu dòng nếu cần
                    int currentLineIndex = richTextBox_Content.GetLineFromCharIndex(selectionStart);
                    if (currentLineIndex > 0) // Không áp dụng cho dòng đầu tiên
                    {
                        int lineStartIndex = richTextBox_Content.GetFirstCharIndexFromLine(currentLineIndex - 1);
                        int lineEndIndex = richTextBox_Content.GetFirstCharIndexFromLine(currentLineIndex) - 1;
                        if (lineEndIndex < 0) lineEndIndex = richTextBox_Content.Text.Length;

                        string previousLine = richTextBox_Content.Text.Substring(lineStartIndex, lineEndIndex - lineStartIndex);

                        // Kiểm tra dấu đầu dòng
                        string bulletPattern = @"^(\s*[\u2022\u25E6\u25CF\u2219○●-]|\d+\.)\s";
                        Regex regex = new Regex(bulletPattern);
                        Match match = regex.Match(previousLine);

                        if (match.Success) // Nếu dòng trước có dấu đầu dòng
                        {
                            string bullet = match.Value.TrimEnd();
                            richTextBox_Content.AppendText(bullet + " ");
                            richTextBox_Content.SelectionStart += bullet.Length + 1; // Đặt lại con trỏ
                        }
                    }
                }
                finally
                {
                    isFormatting = false;
                }

                e.SuppressKeyPress = true; // Ngăn hành vi mặc định của phím Enter
            }
        }
        private void richTextBox_Content_SelectionChanged(object sender, EventArgs e)
        {
            var currentFont = richTextBox_Content.SelectionFont;

            if (currentFont != null)
            {
                // Kiểm tra trạng thái in đậm
                button_Bold.BackColor = currentFont.Bold ? Color.LightGray : SystemColors.Control;

                // Kiểm tra trạng thái in nghiêng
                button_Italic.BackColor = currentFont.Italic ? Color.LightGray : SystemColors.Control;

                // Kiểm tra trạng thái gạch chân
                button_Underline.BackColor = currentFont.Underline ? Color.LightGray : SystemColors.Control;
            }
        }



        //--------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------
        // PHẦN CODE LIÊN QUAN CHUNG GIỮA LOGIC DOC VÀ CLIENT
        private void richTextBox_Content_TextChanged(object sender, EventArgs e)
        {
            richTextBox_Content_TextChangedButton(sender, e);// Gọi hàm xử lý định dạng
            UpdateFormattingButtons();
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
        private TcpClient tcpClient = null;
        private NetworkStream stream = null;
        private int serverPort = 8000;
        private string serverIP = "127.0.0.1";
        private bool isConnected = false;
        private string lastReceivedContent = ""; // Lưu nội dung lần cuối để so sánh
        private static readonly TimeSpan debounceTime = TimeSpan.FromMilliseconds(300); // Thời gian debounce
        private DateTime lastSendTime = DateTime.MinValue;
        private bool isFormatting = false; // Cờ để kiểm soát việc định dạng
        private StringBuilder pendingUpdates = new StringBuilder(); // Lưu trữ các thay đổi tạm thời
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly object connectionLock = new object(); // Khóa đồng bộ
        private readonly string sharedSecret = "SuperSecureSharedSecret123!";
        private Crypto crypto;
        // TẠO KẾT NỐI VỚI SERVER
        private async void mainDoc_LoadConnection(object sender, EventArgs e)
        {
            crypto = new Crypto(sharedSecret);
            await EnsureConnectionAsync();
        }

        // Đảm bảo kết nối luôn mở
        private async Task EnsureConnectionAsync()
        {
            lock (connectionLock)
            {
                if (isConnected) return; // Nếu đã kết nối thì không làm gì thêm
            }

            try
            {
                tcpClient?.Dispose(); // Hủy kết nối cũ nếu tồn tại
                tcpClient = new TcpClient();
                await tcpClient.ConnectAsync(serverIP, serverPort); // Thử kết nối lại
                stream = tcpClient.GetStream(); // Lấy luồng
                isConnected = true;
            }
            catch (Exception ex)
            {
                isConnected = false;
                /*MessageBox.Show($"Error connecting to server: {ex.Message}");*/
            }
        }

        // Tạo khóa riêng cho gửi và nhận dữ liệu
        private object sendLock = new object();
        private object receiveLock = new object();

        // Gửi dữ liệu với đảm bảo kết nối
        private async Task SendDataAsync(string message)
        {
            await EnsureConnectionAsync();
            if (!isConnected) return;

            try
            {
                // Mã hóa dữ liệu trước khi gửi
                byte[] encryptedData = crypto.Encrypt(message);

                // Nén dữ liệu đã mã hóa
                byte[] compressedData = Compress(encryptedData);
                byte[] lengthData = BitConverter.GetBytes(compressedData.Length);

                // Gửi độ dài dữ liệu
                lock (sendLock)
                {
                    stream.Write(lengthData, 0, lengthData.Length);
                }

                // Gửi dữ liệu đã nén
                lock (sendLock)
                {
                    stream.Write(compressedData, 0, compressedData.Length);
                }
            }
            catch (Exception ex)
            {
                isConnected = false; // Đánh dấu là mất kết nối
                MessageBox.Show($"Error sending data: {ex.Message}");
            }
        }

        // Nhận dữ liệu với đảm bảo kết nối
        private async Task SendDataAsync(string message, CancellationToken token)
        {
            await EnsureConnectionAsync();
            if (!isConnected) return;

            try
            {
                // Mã hóa dữ liệu trước khi gửi
                byte[] encryptedData = crypto.Encrypt(message);

                // Nén dữ liệu đã mã hóa
                byte[] compressedData = Compress(encryptedData);
                byte[] lengthData = BitConverter.GetBytes(compressedData.Length);

                // Gửi độ dài dữ liệu
                lock (sendLock)
                {
                    token.ThrowIfCancellationRequested(); // Kiểm tra hủy trước khi gửi
                    stream.WriteAsync(lengthData, 0, lengthData.Length, token);
                }

                // Gửi dữ liệu đã nén
                lock (sendLock)
                {
                    token.ThrowIfCancellationRequested(); // Kiểm tra hủy trước khi gửi
                    stream.WriteAsync(compressedData, 0, compressedData.Length, token);
                }
            }
            catch (OperationCanceledException)
            {
                // Tác vụ bị hủy
                throw;
            }
            catch (Exception ex)
            {
                isConnected = false; // Đánh dấu là mất kết nối
                MessageBox.Show($"Error sending data: {ex.Message}");
            }
        }
        private async Task<string> ReceiveDataAsync()
        {
            await EnsureConnectionAsync();
            if (!isConnected) throw new InvalidOperationException("Not connected to server.");

            byte[] lengthBuffer = new byte[sizeof(int)];

            try
            {
                // Đọc độ dài dữ liệu
                lock (receiveLock)
                {
                    int bytesRead = stream.Read(lengthBuffer, 0, lengthBuffer.Length);
                    if (bytesRead != sizeof(int))
                        throw new IOException("Failed to read data length.");
                }

                int compressedDataLength = BitConverter.ToInt32(lengthBuffer, 0);
                byte[] compressedData = new byte[compressedDataLength];

                int totalBytesRead = 0;
                while (totalBytesRead < compressedDataLength)
                {
                    lock (receiveLock)
                    {
                        int bytesRead = stream.Read(compressedData, totalBytesRead, compressedDataLength - totalBytesRead);
                        if (bytesRead == 0)
                            throw new IOException("Unexpected end of stream.");
                        totalBytesRead += bytesRead;
                    }
                }

                // Giải nén dữ liệu
                byte[] encryptedData = Decompress(compressedData);

                // Giải mã dữ liệu đã nhận
                return crypto.Decrypt(encryptedData);
            }
            catch (Exception ex)
            {
                isConnected = false;
                throw new IOException("Error reading data.", ex);
            }
        }


        private SemaphoreSlim receiveSemaphore = new SemaphoreSlim(1, 1);

        private async Task<string> ReceiveDataAsync(CancellationToken token)
        {
            if (!isConnected) throw new InvalidOperationException("Not connected to server.");

            byte[] lengthBuffer = new byte[sizeof(int)];

            await receiveSemaphore.WaitAsync(token); // Chờ nhận quyền truy cập
            try
            {
                token.ThrowIfCancellationRequested(); // Kiểm tra hủy trước khi đọc

                // Đọc độ dài dữ liệu
                int bytesRead = await stream.ReadAsync(lengthBuffer, 0, lengthBuffer.Length, token);
                if (bytesRead != sizeof(int))
                    throw new IOException("Failed to read data length.");

                int compressedDataLength = BitConverter.ToInt32(lengthBuffer, 0);
                byte[] compressedData = new byte[compressedDataLength];

                int totalBytesRead = 0;
                while (totalBytesRead < compressedDataLength)
                {
                    token.ThrowIfCancellationRequested(); // Kiểm tra hủy trong vòng lặp
                    bytesRead = await stream.ReadAsync(compressedData, totalBytesRead, compressedDataLength - totalBytesRead, token);
                    if (bytesRead == 0)
                        throw new IOException("Unexpected end of stream.");
                    totalBytesRead += bytesRead;
                }

                // Giải nén dữ liệu
                byte[] encryptedData = Decompress(compressedData);

                // Giải mã dữ liệu đã nhận
                return crypto.Decrypt(encryptedData);
            }
            catch (OperationCanceledException)
            {
                throw; // Tác vụ bị hủy
            }
            catch (Exception ex)
            {
                isConnected = false;
                throw new IOException("Error reading data.", ex);
            }
            finally
            {
                receiveSemaphore.Release(); // Giải phóng quyền truy cập
            }
        }



        // LẤY DANH SÁCH FILE
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
               /* MessageBox.Show($"Lỗi kết nối đến server: {ex.Message}",
                                "Lỗi",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);*/
            }
            catch (Exception ex)
            {
                /*MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}",
                                "Lỗi lấy file",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);*/
            }
        }

        private bool isTaskRunning = false;

        private async void listBox_Docs_DoubleClick(object sender, EventArgs e)
        {
            if (isTaskRunning)
            {
                MessageBox.Show("Tài liệu đang được mở, vui lòng chờ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (listBox_Docs.SelectedItem != null)
            {
                dynamic selectedItem = listBox_Docs.SelectedItem;

                string selectedDocName = selectedItem.Text;
                int selectedDocID = selectedItem.Tag;
                isTaskRunning = true; // Đặt trạng thái đang chạy
                try
                {
                    // Hủy các tác vụ liên quan đến tài liệu cũ
                    cancellationTokenSource.Cancel();
                    await Task.Delay(100); // Đợi các tác vụ cũ dừng hẳn
                    cancellationTokenSource = new CancellationTokenSource();

                    await OpenDocumentAsync(selectedDocName, selectedDocID); // Thực hiện bất đồng bộ
                }
                catch (Exception ex)
                {
                    //MessageBox.Show($"Đã xảy ra lỗi khi mở tài liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    isTaskRunning = false; // Đặt lại trạng thái khi hoàn thành
                }
            }
        }

        // MỞ TÀI LIỆU
        private async Task OpenDocumentAsync(string docName, int docID)
        {
            try
            {
                // Kiểm tra token ngay từ đầu
                if (cancellationTokenSource.IsCancellationRequested)
                {
                    return;
                }

                // Đảm bảo kết nối luôn mở
                await EnsureConnectionAsync().ConfigureAwait(false);
                if (!isConnected)
                {
                    MessageBox.Show("Không thể mở tài liệu vì không kết nối được với server.",
                                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string serverResponse = await SendContentFileRequestAsync(docID).ConfigureAwait(false);
                if (serverResponse.StartsWith($"EDIT_DOC|{docID}|{idUser}|"))
                {
                    var responseParts = serverResponse.Split('|');
                    if (responseParts.Length == 5)
                    {
                        string content = responseParts[4];
                        int editStatus = int.Parse(responseParts[3]);

                        // Đặt cờ isFormatting để ngăn sự kiện TextChanged
                        isFormatting = true;
                        try
                        {
                            // Cập nhật nội dung lên RichTextBox trên UI thread
                            richTextBox_Content.Invoke((MethodInvoker)(() =>
                            {
                                if (cancellationTokenSource.IsCancellationRequested)
                                    return;

                                richTextBox_Content.Rtf = content;
                                if (editStatus == 1) richTextBox_Content.Enabled = true;
                                else richTextBox_Content.Enabled = false;
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

                        if (!cancellationTokenSource.IsCancellationRequested)
                        {
                            // Đợi 1000ms trước khi gọi UpdateContentLoop
                            await Task.Delay(1000); // Đợi 1000ms
                            _ = Task.Run(() => UpdateContentLoop(docID, cancellationTokenSource.Token));
                        }
                    }
                    else if (responseParts.Length == 4 && responseParts[3] == "FAIL")
                    {
                        MessageBox.Show(
                            "Tài liệu này không tồn tại.\nVui lòng reload lại danh sách file.",
                            "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information
                        );
                    }
                }
                else
                {
                    /*MessageBox.Show($"Không thể mở tài liệu: phản hồi từ server không hợp lệ.\n{serverResponse}",
                                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);*/
                }
            }
            catch (OperationCanceledException)
            {
                // Tác vụ bị hủy, không cần xử lý gì thêm
            }
            catch (IOException ex)
            {
                /*MessageBox.Show($"Lỗi đọc/ghi dữ liệu: {ex.Message}",
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);*/
            }
            catch (SocketException ex)
            {
                /*isConnected = false; // Đánh dấu là mất kết nối
                MessageBox.Show($"Lỗi kết nối đến server: {ex.Message}",
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);*/
            }
            catch (Exception ex)
            {
               /* MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}",
                                "Lỗi mở file", MessageBoxButtons.OK, MessageBoxIcon.Error);*/
            }
        }

        private SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        private async Task UpdateContentLoop(int expectedDocID, CancellationToken token)
        {
            await semaphore.WaitAsync(token);  //Đảm bảo chỉ một luồng được thực thi
            try
            {

                while (!token.IsCancellationRequested)
                {
                    string response = await SendUpdateDocAsync(token);
                    // Xử lý phản hồi từ server
                    if (response.StartsWith($"UPDATE_DOC|{expectedDocID}|{idUser}|SUCCESS|"))
                    {
                        string[] parts = response.Split('|', 6);
                        if (parts.Length == 6)
                        {
                            while (isEditing)
                            {
                                await Task.Delay(500);
                            }
                            if (int.Parse(parts[4]) != idUser) UpdateRichTextBox(parts[5]); // Cập nhật nội dung
                        }
                    }
                    await Task.Delay(2000);
                }
            }
            catch (OperationCanceledException)
            {
                // Luồng bị hủy, không làm gì thêm
            }
            catch (IOException ex)
            {
                //MessageBox.Show($"Lỗi đọc dữ liệu từ server: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                //MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Giải phóng semaphore ngay cả khi có lỗi hoặc bị hủy
                semaphore.Release();
            }
        }


        // GỬI YÊU CẦU: Lấy tất cả file Doc của người dùng
        private async Task<string> SendGetAllFileRequestAsync()
        {
            string message = $"GET_ALL_FILE|{idUser}";
            await SendDataAsync(message);
            return await ReceiveDataAsync();
        }

        // GỬI YÊU CẦU: Tạo file mới tới server
        private async Task<string> SendCreateNewFileAsync(string fileName)
        {
            string message = $"NEW_FILE|{idUser}|{fileName}";
            await SendDataAsync(message);
            return await ReceiveDataAsync();
        }

        // GỬI YÊU CẦU: Chia sẻ file cho người dùng khác
        private async Task<string> SendShareFileAsync(string usernameToAdd, string mode)
        {
            string message = $"SHARE_FILE|{idUser}|{idDoc}|{usernameToAdd}|{mode}";
            await SendDataAsync(message);
            return await ReceiveDataAsync();
        }

        // GỬI YÊU CẦU: Lấy nội dung file
        private async Task<string> SendContentFileRequestAsync(int docID)
        {
            string message = $"EDIT_DOC|{docID}|{idUser}";
            await SendDataAsync(message);
            return await ReceiveDataAsync();
        }

        // GỬI YÊU CẦU: XÓA FILE
        private async Task<string> SendDeleteFileAsync()
        {
            string message = $"DELETE_FILE|{idUser}|{idDoc}";
            await SendDataAsync(message);
            return await ReceiveDataAsync();
        }

        // GỬI YÊU CẦU: UPDATE DOC

        private async Task<string> SendUpdateDocAsync(CancellationToken token)
        {
            // Kiểm tra nếu tác vụ đã bị hủy trước khi thực hiện
            token.ThrowIfCancellationRequested();

            // Chuẩn bị thông điệp cần gửi
            string message = $"UPDATE_DOC|{idDoc}|{idUser}";

            // Thực hiện gửi dữ liệu, đảm bảo hỗ trợ hủy
            await SendDataAsync(message, token);
            // Nhận dữ liệu, hỗ trợ hủy
            return await ReceiveDataAsync(token);
        }

        private bool isLocalUpdate = false; // Đánh dấu cập nhật cục bộ
        private bool isEditing = false;
        private async void richTextBox_Content_TextChangedHandler(object sender, EventArgs e)
        {
            if (isLocalUpdate) return; // Bỏ qua nếu đây là cập nhật cục bộ
            
            if (isConnected)
            {
                while (isFormatting)
                {
                    await Task.Delay(50); // Đợi đến khi định dạng hoàn tất
                }

                string updatedContent = richTextBox_Content.Rtf;

                if (updatedContent != lastReceivedContent)
                {
                    lastReceivedContent = updatedContent;

                    pendingUpdates.Clear();
                    pendingUpdates.Append(updatedContent);

                    isEditing = true;
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
                
                // Nếu có dữ liệu cần gửi
                if (pendingUpdates.Length > 0)
                {
                    try
                    {
                        // Đảm bảo kết nối đến server
                        await EnsureConnectionAsync();
                        if (!isConnected)
                        {
                            /*MessageBox.Show("Không thể gửi dữ liệu vì không kết nối được với server.",
                                            "Lỗi",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Error);*/
                            return;
                        }

                        // Tạo thông điệp cần gửi
                        string message = $"EDIT_DOC|{idDoc}|{idUser}|{pendingUpdates}";
                        await SendDataAsync(message); // Gửi dữ liệu đến server

                        pendingUpdates.Clear(); // Xóa dữ liệu đã gửi thành công
                        isEditing = false;
                    }
                    catch (IOException ioEx)
                    {
                        /*MessageBox.Show($"Lỗi đọc/ghi dữ liệu khi gửi: {ioEx.Message}",
                                        "Lỗi",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);*/
                    }
                    catch (SocketException sockEx)
                    {
                        isConnected = false; // Đánh dấu mất kết nối
                        MessageBox.Show($"Lỗi kết nối khi gửi dữ liệu: {sockEx.Message}",
                                        "Lỗi",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        /*MessageBox.Show($"Đã xảy ra lỗi khi gửi dữ liệu: {ex.Message}",
                                        "Lỗi",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);*/
                    }
                }
            }
            catch (TaskCanceledException)
            {
                // Tác vụ bị hủy, không cần xử lý thêm
            }
            catch (Exception ex)
            {
                /*MessageBox.Show($"Lỗi không xác định trong DebouncedSendAsync: {ex.Message}",
                                "Lỗi",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);*/
            }
            
        }

        private async void UpdateRichTextBox(string updatedContent)
        {
            
            // Hủy luồng nhận dữ liệu
            isLocalUpdate = true;
            bool canEdit = !richTextBox_Content.ReadOnly;
            if (richTextBox_Content.InvokeRequired)
            {
                richTextBox_Content.Invoke((MethodInvoker)(() =>
                {
                    if (richTextBox_Content.Enabled)
                        richTextBox_Content.Enabled = false;
                    if (!richTextBox_Content.ReadOnly) 
                        richTextBox_Content.ReadOnly = true;
                }));
            }
            else
            {
                if (richTextBox_Content.Enabled)
                    richTextBox_Content.Enabled = false;
                if (!richTextBox_Content.ReadOnly)
                    richTextBox_Content.ReadOnly = true;
            }
            if (!string.IsNullOrEmpty(updatedContent) && updatedContent.StartsWith(@"{\rtf"))
            {
                if (richTextBox_Content.InvokeRequired)
                {
                    richTextBox_Content.Invoke((MethodInvoker)(() =>
                    {
                        ApplyRichTextBoxContent(updatedContent);
                    }));
                }
                else
                {
                    ApplyRichTextBoxContent(updatedContent);
                }
            }
            
            if (richTextBox_Content.InvokeRequired)
            {
                richTextBox_Content.Invoke((MethodInvoker)(() =>
                {
                    richTextBox_Content.Enabled = true;
                    if (canEdit) richTextBox_Content.ReadOnly = false;
                }));
            }
            else
            {
                richTextBox_Content.Enabled = true;
                if (canEdit) richTextBox_Content.ReadOnly = false;
            }
            isLocalUpdate = false;
        }

        private void ApplyRichTextBoxContent(string content)
        {
            if (isEditing) return;
            int cursorPosition = richTextBox_Content.SelectionStart;
            isLocalUpdate = true;
            try
            {
                richTextBox_Content.Rtf = content;
                // Cập nhật nội dung
                richTextBox_Content.SelectionStart = cursorPosition; // Giữ vị trí con trỏ
            }
            finally
            {
                isLocalUpdate = false;
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