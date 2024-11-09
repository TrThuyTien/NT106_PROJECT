using System.Net.Sockets;
using System.Net;
using System.Text;
using System.IO.Compression;

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
        private static readonly TimeSpan debounceTime = TimeSpan.FromMilliseconds(300); // Thời gian debounce
        private bool isListening = false;

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
                byte[] contentBuffer = Compress(Encoding.UTF8.GetBytes(sharedContent));
                writer.Write(contentBuffer.Length); // Gửi độ dài nội dung
                writer.Write(contentBuffer); // Gửi nội dung

                // Nhận các cập nhật từ client
                while (client.Connected)
                {
                    try
                    {
                        int length = reader.ReadInt32(); // Đọc độ dài nội dung từ client
                        byte[] buffer = reader.ReadBytes(length); // Đọc nội dung với độ dài đã cho

                        // Giải nén dữ liệu nhận được
                        string update = Encoding.UTF8.GetString(Decompress(buffer));
                        await processUpdateAsync(update, client);
                    }
                    catch (Exception ex)
                    {
                        // Xử lý ngoại lệ nếu có (Ví dụ: ngắt kết nối)
                        /*Console.WriteLine($"Client disconnected: {ex.Message}");*/
                        break;
                    }
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
            }

            // Chỉ cập nhật giao diện người dùng sau khi đã xử lý xong
            await Task.Run(() => richTextBox_Editor.Invoke((Action)(() =>
            {
                richTextBox_Editor.Rtf = sharedContent; // Cập nhật giao diện với RTF
            })));

            await broadcastUpdateAsync(update, sender);
        }

        private static async Task broadcastUpdateAsync(string update, TcpClient sender)
        {
            byte[] updateBuffer = Compress(Encoding.UTF8.GetBytes(update));
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
                        try
                        {
                            using (BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true))
                            {
                                writer.Write(updateBuffer.Length); // Gửi độ dài nội dung
                                writer.Write(updateBuffer); // Gửi nội dung
                            }
                        }
                        catch (Exception ex)
                        {
                            /*Console.WriteLine($"Error sending update to client: {ex.Message}");*/
                        }
                    }));
                }
            }

            await Task.WhenAll(tasks);
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
                    /*Console.WriteLine($"Error closing stream: {ex.Message}");*/
                }

                try
                {
                    // Đóng kết nối TcpClient
                    client.Close();
                }
                catch (Exception ex)
                {
                    // Xử lý ngoại lệ nếu có
                    /*Console.WriteLine($"Error closing TcpClient: {ex.Message}");*/
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
