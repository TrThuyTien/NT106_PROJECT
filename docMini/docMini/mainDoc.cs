using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;

namespace docMini
{
    public partial class mainDoc : Form
    {
        public mainDoc()
        {
            InitializeComponent();
            /*            PagedRichTextBox pagedRichTextBox = new PagedRichTextBox(this)
                        {
                            Location = new System.Drawing.Point(10, 10),
                            Size = new Size(531, 219)
                        };
                        this.Controls.Add(richTextBox_Content);*/

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
            mainDoc mD = new mainDoc();
            mD.Show();
        }

        private void button_ShareDoc_Click(object sender, EventArgs e)
        {

        }

        private void button_Bold_Click(object sender, EventArgs e)
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
                        System.Drawing.FontStyle newFontStyle = richTextBox_Content.SelectionFont.Style ^ System.Drawing.FontStyle.Bold;
                        richTextBox_Content.SelectionFont = new System.Drawing.Font(currentFont.FontFamily, currentFont.Size, newFontStyle);
                    }
                    richTextBox_Content.Select(selectionStart, selectionLength);
                }
            }
        }
        private void comboBox_size_SelectedIndexChanged(object sender, EventArgs e)
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
        }
        private void comboBox_font_SelectedIndexChanged(object sender, EventArgs e)
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
                        richTextBox_Content.SelectionFont = new System.Drawing.Font(selectedFont, currentFont.Size, currentFont.Style);
                    }
                }

                richTextBox_Content.Select(selectionStart, selectionLength);
            }
        }
        private void button_Italic_Click(object sender, EventArgs e)
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
                        System.Drawing.FontStyle newFontStyle = richTextBox_Content.SelectionFont.Style ^ System.Drawing.FontStyle.Italic;
                        richTextBox_Content.SelectionFont = new System.Drawing.Font(currentFont.FontFamily, currentFont.Size, newFontStyle);
                    }
                    richTextBox_Content.Select(selectionStart, selectionLength);
                }
            }
        }

        private void button_Underline_Click(object sender, EventArgs e)
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
                        System.Drawing.FontStyle newFontStyle = richTextBox_Content.SelectionFont.Style ^ System.Drawing.FontStyle.Underline;
                        richTextBox_Content.SelectionFont = new System.Drawing.Font(currentFont.FontFamily, currentFont.Size, newFontStyle);
                    }
                    richTextBox_Content.Select(selectionStart, selectionLength);
                }
            }
        }

        private void button_AlignLeft_Click(object sender, EventArgs e)
        {
            richTextBox_Content.SelectionAlignment = System.Windows.Forms.HorizontalAlignment.Left;
        }

        private void button_Center_Click(object sender, EventArgs e)
        {
            richTextBox_Content.SelectionAlignment = System.Windows.Forms.HorizontalAlignment.Center;
        }

        private void button_AlignRight_Click(object sender, EventArgs e)
        {
            richTextBox_Content.SelectionAlignment = System.Windows.Forms.HorizontalAlignment.Right;
        }
        private void mainDoc_Load(object sender, EventArgs e)
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

        // PHẦN KẾT NỐI VỚI SERVER -------------------------------------------------------------------------
        private TcpClient tcpClient;
        private NetworkStream stream;
        private bool isConnected = false;
        private string lastReceivedContent = ""; // Lưu nội dung lần cuối để so sánh
        private static readonly TimeSpan debounceTime = TimeSpan.FromMilliseconds(300); // Thời gian debounce
        private DateTime lastSendTime = DateTime.MinValue;
        private bool isFormatting = false; // Cờ để kiểm soát việc định dạng

        private async void button_Connect_Click(object sender, EventArgs e)
        {
            tcpClient = new TcpClient();
            await tcpClient.ConnectAsync("127.0.0.1", 8080);
            stream = tcpClient.GetStream();
            isConnected = true;

            // Nhận nội dung hiện tại từ server
            _ = System.Threading.Tasks.Task.Run(() => ReceiveContentAsync());
            richTextBox_Content.Enabled = true;
        }

        private async System.Threading.Tasks.Task ReceiveContentAsync()
        {
            using (BinaryReader reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true))
            {
                while (isConnected)
                {
                    int length = reader.ReadInt32(); // Đọc độ dài nội dung từ server
                    byte[] buffer = reader.ReadBytes(length); // Đọc nội dung với độ dài đã cho
                    string content = Encoding.UTF8.GetString(buffer); // Chuyển đổi dữ liệu thành chuỗi RTF

                    // Chỉ cập nhật nếu nội dung thực sự thay đổi
                    if (content != lastReceivedContent)
                    {
                        lastReceivedContent = content;

                        // Tạm thời tắt cờ để ngăn `TextChanged` khi cập nhật
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

                        isFormatting = false; // Bật lại sau khi cập nhật xong
                    }
                }
            }
        }

        private async void richTextBox_Content_TextChanged(object sender, EventArgs e)
        {
            if (isConnected && !isFormatting) // Chỉ gửi khi không trong trạng thái định dạng
            {
                // Thời gian debounce - bỏ qua nếu chưa đến thời gian debounce
                if (DateTime.Now - lastSendTime < debounceTime)
                    return;

                lastSendTime = DateTime.Now;

                string updatedContent = richTextBox_Content.Rtf;
                byte[] bufferContent = Encoding.UTF8.GetBytes(updatedContent);

                using (BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true))
                {
                    writer.Write(bufferContent.Length); // Gửi độ dài nội dung
                    writer.Write(bufferContent); // Gửi nội dung
                }
            }
        }

        // ---------------------------------------------------------------------------------

    }
}