using System;
using System.IO.Compression;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using Server;

namespace docMini
{
    public partial class SignUp : Form
    {
        public SignUp()
        {
            InitializeComponent();
        }

        private void label_SwitchSignIn_Click(object sender, EventArgs e)
        {
            this.Hide();
            SignIn form = new SignIn();
            form.Show();
        }
        TcpClient client;
        NetworkStream stream;
        private async void button_SignUp_Click(object sender, EventArgs e)
        {
            string username = textbox_Username.Text;
            string email = textbox_Email.Text;
            string password = textbox_Password.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (checkmail(email))
            {
                MessageBox.Show("Email hợp lệ.");
            }
            else
            {
                MessageBox.Show("Email không hợp lệ.");
            }

            try
            {
                string serverResponse = await SendSignUpRequestAsync(username, email, password);
                if (serverResponse.StartsWith($"SIGN_UP|{username}|"))
                {
                    // Phân tích gói tin phản hồi từ server để lấy ID nếu thành công
                    var responseParts = serverResponse.Split('|');
                    if (responseParts[2] == "SUCCESS")
                    {
                        string userId = responseParts[3];

                        MessageBox.Show($"Đăng ký thành công!",
                                        "Thông báo",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);

                        // Chuyển sang giao diện chính
                        this.Hide();
                        mainDoc mainForm = new mainDoc(int.Parse(userId), username);
                        mainForm.ShowDialog();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show(
                            "Tên người dùng đã tồn tại hoặc có lỗi xảy ra.",
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

        private async Task<string> SendSignUpRequestAsync(string username, string email, string password)
        {
            string serverIp = "127.0.0.1";
            int serverPort = 8080;

            using (client = new TcpClient())
            {
                await client.ConnectAsync(serverIp, serverPort);
                using (stream = client.GetStream())
                {
                    // Gửi yêu cầu đăng ký
                    string message = $"SIGN_UP|{username}|{email}|{password}";
                    await SendDataAsync(message);

                    // Nhận phản hồi từ server
                    string serverResponse = await ReceiveDataAsync();
                    return serverResponse;
                }
            }
        }

        private async Task SendDataAsync(string message)
        {
            // Chuyển đổi chuỗi thành byte
            byte[] data = Encoding.UTF8.GetBytes(message);

            // Nén dữ liệu
            byte[] compressedData = Compress(data);

            // Gửi độ dài dữ liệu nén
            byte[] lengthData = BitConverter.GetBytes(compressedData.Length);
            await stream.WriteAsync(lengthData, 0, lengthData.Length);

            // Gửi dữ liệu nén
            await stream.WriteAsync(compressedData, 0, compressedData.Length);
        }


        private async Task<string> ReceiveDataAsync()
        {
            // Đọc độ dài dữ liệu nén
            byte[] lengthBuffer = new byte[sizeof(int)];
            int bytesRead = await stream.ReadAsync(lengthBuffer, 0, lengthBuffer.Length);
            if (bytesRead != sizeof(int))
            {
                throw new Exception("Không thể đọc kích thước dữ liệu từ server.");
            }
            int compressedLength = BitConverter.ToInt32(lengthBuffer, 0);

            // Đọc dữ liệu nén
            byte[] compressedData = new byte[compressedLength];
            bytesRead = await stream.ReadAsync(compressedData, 0, compressedData.Length);
            if (bytesRead != compressedLength)
            {
                throw new Exception("Không thể đọc đầy đủ dữ liệu từ server.");
            }

            // Giải nén dữ liệu
            byte[] decompressedData = Decompress(compressedData);

            // Chuyển đổi byte thành chuỗi UTF-8
            return Encoding.UTF8.GetString(decompressedData, 0, decompressedData.Length);
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


        private void checkbox_Showpass_CheckedChanged(object sender, EventArgs e)
        {
            if (checkbox_Showpass.Checked)
            {
                textbox_Password.UseSystemPasswordChar = false;
            }
            else
            {
                textbox_Password.UseSystemPasswordChar = true;
            }
        }

        private bool checkmail(string email)
        {
            try
            {
                var check = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                return System.Text.RegularExpressions.Regex.IsMatch(email, check);
            }
            catch
            {
                return false;
            }
        }
    }
}