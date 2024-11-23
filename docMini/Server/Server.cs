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
            using (BinaryReader reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true))
            {
                while (client.Connected)
                {
                    try
                    {
                        // Đọc độ dài nội dung từ client
                        byte[] lengthBuffer = new byte[sizeof(int)];
                        int bytesRead = await stream.ReadAsync(lengthBuffer, 0, lengthBuffer.Length);
                        if (bytesRead == 0) break; // Ngắt kết nối nếu không có dữ liệu

                        int length = BitConverter.ToInt32(lengthBuffer, 0); // Chuyển đổi độ dài thành số nguyên

                        // Kiểm tra kích thước tối đa cho phép
                        if (length > 1024 * 1024) // Ví dụ: giới hạn ở 1 MB
                        {
                            MessageBox.Show("Dữ liệu quá lớn, không thể xử lý.");
                            break;
                        }

                        byte[] buffer = new byte[length]; // Tạo buffer để đọc nội dung
                        bytesRead = await stream.ReadAsync(buffer, 0, length);
                        string update = Encoding.UTF8.GetString(buffer);

                        await ProcessRequestAsync(update, stream, client);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
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

        private async Task ProcessRequestAsync(string update, NetworkStream stream, TcpClient client)
        {
            if (update.StartsWith("SIGN_IN|"))
            {
                await HandleSignInAsync(update, stream);
            }
            else if (update.StartsWith("SIGN_UP|"))
            {
                await HandleSignUpAsync(update, stream);
            }
            else if (update.StartsWith("NEWFILE|"))
            {
                await HandleNewFileAsync(update, stream);
            }
            /*else if(update.StartsWith("EDIT_DOC|"))
            {
                await HandleEditDocumentAsync(update, stream, client);
            } */
            else
            {
                await editDocAsync(client);
            }
        }

        private async Task HandleNewFileAsync(string update, NetworkStream stream)
        {
            try
            {
                string[] parts = update.Split('|');
                if (parts.Length == 3)
                {
                    string fileName = parts[1].Trim();
                    int userId = int.Parse(parts[2]);

                    DatabaseManager dbManager = new DatabaseManager();

                    // Lấy tên người dùng (Owner) dựa trên userId
                    string owner = dbManager.GetUsernameByUserId(userId);
                    if (string.IsNullOrEmpty(owner))
                    {
                        // Gửi phản hồi khi userId không hợp lệ
                        byte[] responses = Encoding.UTF8.GetBytes("INVALID_USER");
                        await SendResponseAsync(stream, responses);
                        return;
                    }

                    // Kiểm tra xem tài liệu có trùng tên hay không
                    if (dbManager.IsDocumentExists(fileName, owner))
                    {
                        byte[] respons = Encoding.UTF8.GetBytes("DUPLICATE");
                        await SendResponseAsync(stream, respons);
                        return;
                    }

                    // Thêm tài liệu mới vào cơ sở dữ liệu
                    bool success = dbManager.AddNewDocument(fileName, owner, userId);

                    string responseMessage = success ? "SUCCESS" : "FAIL";
                    byte[] response = Encoding.UTF8.GetBytes(responseMessage);
                    await SendResponseAsync(stream, response);
                }
                else
                {
                    byte[] response = Encoding.UTF8.GetBytes("INVALID_REQUEST");
                    await SendResponseAsync(stream, response);
                }
            }
            catch (Exception ex)
            {
                byte[] response = Encoding.UTF8.GetBytes("ERROR");
                await SendResponseAsync(stream, response);

                Console.WriteLine($"Error in HandleNewFileAsync: {ex.Message}");
            }
        }

        private async Task HandleSignInAsync(string update, NetworkStream stream)
        {
            string[] parts = update.Split('|');
            if (parts.Length == 3)
            {
                string username = parts[1];
                string password = parts[2];

                DatabaseManager dbManager = new DatabaseManager();

                bool isValidUser = dbManager.ValidateUser(username, password);

                if (isValidUser)
                {
                    // Lấy ID người dùng
                    int userId = dbManager.GetUserIdByUsername(username);
                    // Tạo thông điệp phản hồi
                    string responseMessage = $"SIGN_IN|{username}|SUCCESS|{userId}";
                    byte[] response = Encoding.UTF8.GetBytes(responseMessage);
                    await SendResponseAsync(stream, response);
                }
                else
                {
                    // Gửi phản hồi thất bại
                    byte[] response = Encoding.UTF8.GetBytes($"SIGN_IN|{username}|FAIL");
                    await SendResponseAsync(stream, response);
                }
            }
        }

        private async Task HandleSignUpAsync(string update, NetworkStream stream)
        {
            string[] parts = update.Split('|');
            if (parts.Length == 4)
            {
                string username = parts[1];
                string email = parts[2];
                string password = parts[3];

                DatabaseManager dbManager = new DatabaseManager();
                bool success = dbManager.InsertUser(username, email, password);

                if (success)
                {
                    int userId = dbManager.GetUserIdByUsername(username);
                    string responseMessage = $"SIGN_UP|{username}|SUCCESS|{userId}";
                    byte[] response = Encoding.UTF8.GetBytes(responseMessage);
                    await SendResponseAsync(stream, response);
                }
                else
                {
                    byte[] response = Encoding.UTF8.GetBytes($"SIGN_UP|{username}|FAIL");
                    await SendResponseAsync(stream, response);
                }
            }
        }

        private async Task SendResponseAsync(NetworkStream stream, byte[] response)
        {
            // Gửi độ dài phản hồi
            byte[] lengthBuffer = BitConverter.GetBytes(response.Length);
            await stream.WriteAsync(lengthBuffer, 0, lengthBuffer.Length); // Gửi độ dài phản hồi
            await stream.WriteAsync(response, 0, response.Length); // Gửi phản hồi
        }

        //BẢN CÓ CSDL

        private async Task HandleEditDocumentAsync(string update, NetworkStream stream, TcpClient sender)
        {
            /*// Tách DocID từ chuỗi update
            string[] parts = update.Split('|');
            // 
            if (parts.Length < 4)
            {
                // Nếu không đúng định dạng, có thể gửi thông báo lỗi hoặc bỏ qua
                return;
            }

            string docId = parts[1]; // Lấy DocID
            string newContent = await GetContentFromDatabaseAsync(docId); // Lấy nội dung hiện tại từ cơ sở dữ liệu

            // Gửi nội dung hiện tại dưới dạng RTF tới client mới

            byte[] contentBuffer = Encoding.UTF8.GetBytes(newContent);
            await SendResponseAsync(stream, contentBuffer);

            // Xử lý cập nhật
            await ProcessUpdateAsync(update, sender, docId);*/
        }

        /*private async Task ProcessUpdateAsync(string update, TcpClient sender, string docId)
        {
            lock (lockObject)
            {
                // Kiểm tra thời gian cập nhật
                if (DateTime.Now - lastUpdateTime < debounceTime)
                {
                    return; // Bỏ qua cập nhật nếu chưa đủ thời gian debounce
                }

                // Cập nhật nội dung chung từ update
                sharedContent = update; // Cập nhật nội dung chung
                lastUpdateTime = DateTime.Now; // Cập nhật thời gian
            }

            // Lưu nội dung mới vào cơ sở dữ liệu
            DatabaseManager dbManager = new DatabaseManager();
            bool saveSuccess = await dbManager.UpdateDocumentContentAsync(docId, sharedContent);
            if (!saveSuccess)
            {
                Console.WriteLine($"Không thể lưu nội dung cho DocID: {docId}");
                return;
            }

            // Chỉ cập nhật giao diện người dùng sau khi đã xử lý xong
            await Task.Run(() => richTextBox_Editor.Invoke((Action)(() =>
            {
                richTextBox_Editor.Rtf = sharedContent; // Cập nhật giao diện với RTF
            })));

            await BroadcastUpdateAsync(update, sender);
        }*/

        /*private static async Task BroadcastUpdateAsync(string update, TcpClient sender)
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
                            // Xử lý lỗi gửi cập nhật đến client
                        }
                    }));
                }
            }

            await Task.WhenAll(tasks);
        }*/

        // BẢN KHÔNG CÓ CSDL
        private async Task editDocAsync(TcpClient client)
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
