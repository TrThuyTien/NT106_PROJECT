using System;
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

            try
            {
                string serverResponse = await SendSignUpRequestAsync(username, email, password);
                if (serverResponse.StartsWith("SUCCESS"))
                {
                    // Phân tích phản hồi từ server để lấy ID và tên người dùng
                    var responseParts = serverResponse.Split('|');
                    if (responseParts.Length >= 2)
                    {
                        string userId = responseParts[1];

                        MessageBox.Show($"Đăng ký thành công!\nUser ID: {userId}",
                                        "Thông báo",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);

                        // Chuyển sang giao diện chính
                        this.Hide();
                        new mainDoc(int.Parse(userId), username).ShowDialog();
                    }
                    else
                    {
                        throw new Exception("Phản hồi từ server không hợp lệ.");
                    }
                }
                else
                {
                    MessageBox.Show("Tên người dùng đã tồn tại hoặc có lỗi xảy ra.",
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

    }
}