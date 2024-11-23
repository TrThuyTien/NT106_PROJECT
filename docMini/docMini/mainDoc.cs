using System.IO.Compression;
using System.Net.Sockets;
using System.Text;
using System.Runtime.InteropServices;
using System.Data.SQLite;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
namespace docMini
{
    public partial class mainDoc : Form
    {
        int idUser;
        string nameUser;
        public mainDoc(int userID, string userName)
        {
            InitializeComponent();
            idUser = userID;
            nameUser = userName;
            richTextBox_Content.ReadOnly= true;
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
        private void button_Bold_Click(object sender, EventArgs e)
        {
            isBold = !isBold; // Toggle the bold state

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
                        System.Drawing.FontStyle newFontStyle = currentFont.Style ^ System.Drawing.FontStyle.Bold;
                        richTextBox_Content.SelectionFont = new System.Drawing.Font(currentFont.FontFamily, currentFont.Size, newFontStyle);
                    }
                }
                richTextBox_Content.Select(selectionStart, selectionLength);
            }
        }
        private void richTextBox_Content_TextChangedButton(object sender, EventArgs e)
        {
            if (isBold && richTextBox_Content.SelectionLength == 0)
            {
                int selectionStart = richTextBox_Content.SelectionStart;
                richTextBox_Content.Select(selectionStart - 0, 1);//g
                if (richTextBox_Content.SelectionFont != null)
                {
                    System.Drawing.Font currentFont = richTextBox_Content.SelectionFont;
                    System.Drawing.FontStyle newFontStyle = currentFont.Style | System.Drawing.FontStyle.Bold;
                    richTextBox_Content.SelectionFont = new System.Drawing.Font(currentFont.FontFamily, currentFont.Size, newFontStyle);
                }
                richTextBox_Content.Select(selectionStart, 0);
            }
            if (isItalic && richTextBox_Content.SelectionLength == 0)
            {
                int selectionStart = richTextBox_Content.SelectionStart;
                richTextBox_Content.Select(selectionStart - 0, 1);//u
                if (richTextBox_Content.SelectionFont != null)
                {
                    System.Drawing.Font currentFont = richTextBox_Content.SelectionFont;
                    System.Drawing.FontStyle newFontStyle = currentFont.Style | System.Drawing.FontStyle.Italic;
                    richTextBox_Content.SelectionFont = new System.Drawing.Font(currentFont.FontFamily, currentFont.Size, newFontStyle);
                }
                richTextBox_Content.Select(selectionStart, 0);
            }
            if (isUnderline && richTextBox_Content.SelectionLength == 0)
            {
                int selectionStart = richTextBox_Content.SelectionStart;
                richTextBox_Content.Select(selectionStart - 0, 1); //b
                if (richTextBox_Content.SelectionFont != null)
                {
                    System.Drawing.Font currentFont = richTextBox_Content.SelectionFont;
                    System.Drawing.FontStyle newFontStyle = currentFont.Style | System.Drawing.FontStyle.Underline;
                    richTextBox_Content.SelectionFont = new System.Drawing.Font(currentFont.FontFamily, currentFont.Size, newFontStyle);
                }
                richTextBox_Content.Select(selectionStart, 0);
            }
        }
        private void comboBox_Size_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (richTextBox_Content.SelectionLength > 0)
            {
                int selectionStart = richTextBox_Content.SelectionStart;
                int selectionLength = richTextBox_Content.SelectionLength;
                string selectedFont = comboBox_Font.SelectedItem.ToString();

                for (int i = 0; i < selectionLength; i++)
                {
                    richTextBox_Content.Select(selectionStart + i, 1);
                    if (richTextBox_Content.SelectionFont != null)
                    {
                        System.Drawing.Font currentFont = richTextBox_Content.SelectionFont;
                        float newSize = float.Parse(comboBox_Size.SelectedItem.ToString());
                        richTextBox_Content.SelectionFont = new System.Drawing.Font(currentFont.FontFamily, newSize, currentFont.Style);
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
                    richTextBox_Content.SelectionFont = new System.Drawing.Font(currentFont.FontFamily, currentFontSize, currentFont.Style);
                }
                richTextBox_Content.Select(selectionStart, 0);
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
        private void button_Italic_Click(object sender, EventArgs e)
        {
            isItalic = !isItalic;

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
                        System.Drawing.FontStyle newFontStyle = currentFont.Style ^ System.Drawing.FontStyle.Italic;
                        richTextBox_Content.SelectionFont = new System.Drawing.Font(currentFont.FontFamily, currentFont.Size, newFontStyle);
                    }
                }
                richTextBox_Content.Select(selectionStart, selectionLength);
            }
        }
        private void button_Underline_Click(object sender, EventArgs e)
        {
            isUnderline = !isUnderline;
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
                        System.Drawing.FontStyle newFontStyle = currentFont.Style ^ System.Drawing.FontStyle.Underline;
                        richTextBox_Content.SelectionFont = new System.Drawing.Font(currentFont.FontFamily, currentFont.Size, newFontStyle);
                    }
                }
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

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

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

            // Đặt giá trị mặc định cho ComboBox
            comboBox_Font.SelectedIndex = comboBox_Font.FindString("Tahoma");
            comboBox_Size.SelectedIndex = comboBox_Size.FindString("12");
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
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Rich Text Format|*.rtf";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                SaveRtf(sfd.FileName);
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

        private void button_AddTable_Click(object sender, EventArgs e)
        {

        }


        // ---------------------------------------------------------------------------------

    }
}
