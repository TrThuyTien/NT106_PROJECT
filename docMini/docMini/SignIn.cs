using docMini;
using Server;
using System.IO.Compression;
using System.Net.Sockets;
using System.Text;

namespace docMini
{
    public partial class SignIn : Form
    {
        public SignIn()
        {
            InitializeComponent();
            textbox_Username.KeyDown += TextBox_KeyDown;
            textbox_Password.KeyDown += TextBox_KeyDown;
        }
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) // Kiểm tra nếu nhấn phím Enter
            {
                e.SuppressKeyPress = true; // Ngăn tiếng 'bíp' khi nhấn Enter
                button_SignIn_Click(button_SignIn, EventArgs.Empty); // Gọi sự kiện Click
            }
        }

        TcpClient client;
        NetworkStream stream;
        private async void button_SignIn_Click(object sender, EventArgs e)
        {
            string username = textbox_Username.Text;
            string password = textbox_Password.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show(
                    "Vui lòng nhập đầy đủ thông tin.", 
                    "Thông báo", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Warning
                );
                return;
            }

            try
            {
                string serverResponse = await SendSignInRequestAsync(username, password);
                if (serverResponse.StartsWith($"SIGN_IN|{username}|")) {
                    // Phân tích gói tin phản hồi từ server để lấy ID nếu thành công
                    var responseParts = serverResponse.Split('|');
                    if (responseParts[2] == "SUCCESS")
                    {
                        string userId = responseParts[3];

                        MessageBox.Show($"Đăng nhập thành công!",
                                        "Thông báo",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);

                        // Chuyển sang giao diện chính
                        this.Hide();
                        mainDoc mainForm = new mainDoc(int.Parse(userId), username);
                        mainForm.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng.",
                                    "Lỗi",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                    }
                } 
                else
                {
                    MessageBox.Show("Day khong phai la phan hoi dung can nhan");
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

        private async Task<string> SendSignInRequestAsync(string username, string password)
        {
            string serverIp = "127.0.0.1";
            int serverPort = 8080;

            using (client = new TcpClient())
            {
                await client.ConnectAsync(serverIp, serverPort);
                using (stream = client.GetStream())
                {
                    // Gửi yêu cầu đăng nhập
                    string message = $"SIGN_IN|{username}|{password}";
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
        private void label_ForgotPass_Click(object sender, EventArgs e)
        {
            this.Hide();
            FogotPass form = new FogotPass();
            form.Show();
        }

        private void label_SwitchSignUp_Click(object sender, EventArgs e)
        {
            this.Hide();
            SignUp form = new SignUp();
            form.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mainDoc mainDoc = new mainDoc(1, "admin");
            mainDoc.Show();
        }
    }
}