using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Server
{
    public partial class Server : Form
    {
        public Server()
        {
            InitializeComponent();
        }


        TcpListener listener;
        private static List<TcpClient> clients = new List<TcpClient>();
        private static string sharedContent = ""; // Nội dung văn bản chung
        private static readonly object lockObject = new object();
        private static DateTime lastUpdateTime = DateTime.MinValue;
        private static readonly TimeSpan debounceTime = TimeSpan.FromMilliseconds(300); // Tăng nhẹ thời gian debounce
        private bool isListening = false;
        private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1); // Semaphore để kiểm soát truy cập

        private async void button_Listen_Click(object sender, EventArgs e)
        {
            listener = new TcpListener(IPAddress.Any, 8080);
            listener.Start();
            richTextBox_Editor.Text = "Server đang chạy trên cổng 8080...";
            isListening = true;
            while (isListening)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                lock (clients)
                {
                    clients.Add(client);
                }
                _ = Task.Run(() => handleClientAsync(client));
            }
        }

        private async Task handleClientAsync(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            using (BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true))
            using (BinaryReader reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true))
            {
                // Gửi nội dung hiện tại dưới dạng RTF tới client mới
                byte[] contentBuffer = Encoding.UTF8.GetBytes(sharedContent);
                writer.Write(contentBuffer.Length); // Gửi độ dài nội dung
                writer.Write(contentBuffer); // Gửi nội dung

                // Nhận các cập nhật từ client
                while (client.Connected)
                {
                    int length = reader.ReadInt32(); // Đọc độ dài nội dung từ client
                    byte[] buffer = reader.ReadBytes(length); // Đọc nội dung với độ dài đã cho

                    // Chuyển đổi dữ liệu thành chuỗi RTF
                    string update = Encoding.UTF8.GetString(buffer);
                    await processUpdateAsync(update, client);
                }
            }

            // Ngắt kết nối client
            lock (clients)
            {
                clients.Remove(client);
            }
        }

        private async Task processUpdateAsync(string update, TcpClient sender)
        {
            lock (lockObject)
            {
                // Kiểm tra thời gian cập nhật
                if (DateTime.Now - lastUpdateTime < debounceTime)
                {
                    return; // Bỏ qua cập nhật nếu chưa đủ thời gian debounce
                }

                sharedContent = update; // Cập nhật nội dung chung
                lastUpdateTime = DateTime.Now; // Cập nhật thời gian
                richTextBox_Editor.Rtf = update; // Cập nhật giao diện với RTF
            }

            await broadcastUpdateAsync(update, sender);
        }

        private static async Task broadcastUpdateAsync(string update, TcpClient sender)
        {
            byte[] updateBuffer = Encoding.UTF8.GetBytes(update);
            List<TcpClient> clientsCopy;

            lock (clients)
            {
                // Tạo một bản sao của danh sách clients
                clientsCopy = new List<TcpClient>(clients);
            }

            List<Task> tasks = new List<Task>();
            foreach (TcpClient client in clientsCopy)
            {
                if (client != sender)
                {
                    NetworkStream stream = client.GetStream();
                    tasks.Add(Task.Run(async () =>
                    {
                        await semaphore.WaitAsync(); // Đảm bảo rằng chỉ một client cập nhật tại một thời điểm
                        try
                        {
                            using (BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true))
                            {
                                writer.Write(updateBuffer.Length); // Gửi độ dài nội dung
                                writer.Write(updateBuffer); // Gửi nội dung
                            }
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    }));
                }
            }

            await Task.WhenAll(tasks);
        }


        private void CloseClientConnection(TcpClient client)
        {
            if (client != null)
            {
                try
                {
                    // Đóng luồng mạng
                    if (client.GetStream() != null)
                    {
                        client.GetStream().Close();
                    }
                }
                catch (Exception ex)
                {
                    // Xử lý ngoại lệ nếu có
                    Console.WriteLine($"Error closing stream: {ex.Message}");
                }

                try
                {
                    // Đóng kết nối TcpClient
                    client.Close();
                }
                catch (Exception ex)
                {
                    // Xử lý ngoại lệ nếu có
                    Console.WriteLine($"Error closing TcpClient: {ex.Message}");
                }

                lock (clients)
                {
                    // Xóa client khỏi danh sách kết nối
                    if (clients.Contains(client))
                    {
                        clients.Remove(client);
                    }
                }
            }
        }

        private void button_Stop_Click(object sender, EventArgs e)
        {
            // Dừng lắng nghe trên TcpListener
            if (listener != null)
            {
                isListening = false;
                listener.Stop(); // Dừng listener
                listener = null; // Giải phóng tài nguyên
            }

            // Đóng tất cả các kết nối client
            lock (clients)
            {
                foreach (var client in clients)
                {
                    CloseClientConnection(client); // Gọi hàm đã tạo để đóng kết nối
                }
                clients.Clear(); // Xóa danh sách client
            }

            // Cập nhật giao diện người dùng
            richTextBox_Editor.Text = "Server đã dừng.";
        }
    }
}
