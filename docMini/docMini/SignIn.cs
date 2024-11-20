using docMini;
using Server;
using System.Net.Sockets;
using System.Text;

namespace docMini
{
    public partial class SignIn : Form
    {
        public SignIn()
        {
            InitializeComponent();
        }

        TcpClient client;
        NetworkStream stream;
        private async void button_SignIn_Click(object sender, EventArgs e)
        {
            string username = textbox_Username.Text;
            string password = textbox_Password.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string serverResponse = await SendSignInRequestAsync(username, password);
                if (serverResponse.StartsWith("SUCCESS"))
                {
                    // Phân tích phản hồi từ server để lấy ID và tên người dùng
                    var responseParts = serverResponse.Split('|');
                    MessageBox.Show(serverResponse);
                    if (responseParts.Length >= 2)
                    {
                        string userId = responseParts[1];

                        MessageBox.Show($"Đăng nhập thành công!\nUser ID: {userId}\nUsername: {username}",
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
                        throw new Exception("Phản hồi từ server không hợp lệ.");
                    }
                }
                else
                {
                    MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng.",
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
    }
}