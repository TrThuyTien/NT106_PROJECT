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
                try
                {
                    while (client.Connected)
                    {
                        byte[] lengthBuffer = new byte[sizeof(int)];
                        int bytesRead = await stream.ReadAsync(lengthBuffer, 0, lengthBuffer.Length);
                        if (bytesRead == 0) // Connection closed
                            break;

                        int length = BitConverter.ToInt32(lengthBuffer, 0);
                        byte[] buffer = new byte[length];
                        bytesRead = await stream.ReadAsync(buffer, 0, length);

                        if (bytesRead < length) // Handle partial reads
                            break;

                        string update = Encoding.UTF8.GetString(buffer);
                        await ProcessRequestAsync(update, stream, client);
                    }
                }
                catch (IOException ioEx)
                {
                    Console.WriteLine($"IO Exception: {ioEx.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"General Exception: {ex.Message}");
                }
                finally
                {
                    CloseClientConnection(client); // Ensure proper cleanup
                }
            }
        }


        private async Task ProcessRequestAsync(string update, NetworkStream stream, TcpClient client)
        {
            await Task.Run(() => richTextBox_Editor.Invoke((Action)(() =>
            {
                richTextBox_Editor.Text += update + Environment.NewLine; // Cập nhật giao diện với RTF
            })));
            if (update.StartsWith("SIGN_IN|"))
            {
                await HandleSignInAsync(update, stream);
            }
            else if (update.StartsWith("SIGN_UP|"))
            {
                await HandleSignUpAsync(update, stream);
            }
            else if (update.StartsWith("NEW_FILE|"))
            {
                await HandleNewFileAsync(update, stream);
            }
            else if (update.StartsWith("EDIT_DOC|"))
            {
                await HandleEditDocumentAsync(update, stream, client);
               
            }
            else if (update.StartsWith("GET_ALL_FILE|"))
            {
                await HandleGetAllFileAsync(update, stream);
            }
        }

        private async Task HandleNewFileAsync(string update, NetworkStream stream)
        {
            try
            {
                string[] parts = update.Split('|');
                if (parts.Length == 3)
                {
                    string fileName = parts[2].Trim();
                    int userId = int.Parse(parts[1]);

                    DatabaseManager dbManager = new DatabaseManager();

                    // Lấy tên người dùng (Owner) dựa trên userId
                    string owner = dbManager.GetUsernameByUserId(userId);

                    // Kiểm tra xem tài liệu có trùng tên hay không
                    if (dbManager.IsDocumentExists(fileName, userId))
                    {
                        byte[] respons = Encoding.UTF8.GetBytes($"NEW_FILE|{userId}|DUPLICATE");
                        await SendResponseAsync(stream, respons);
                        return;
                    }

                    // Thêm tài liệu mới vào cơ sở dữ liệu, thành công trả về idDoc, thất bại trả về 0
                    int idDoc = dbManager.AddNewDocument(fileName, owner, userId);

                    // Gửi phản hồi
                    if (idDoc > 0)
                    {
                        string responseMessage = $"NEW_FILE|{userId}|SUCCESS|{idDoc}";
                        byte[] response = Encoding.UTF8.GetBytes(responseMessage);
                        await SendResponseAsync(stream, response);
                    }
                    else
                    {
                        string responseMessage = $"NEW_FILE|{userId}|FAIL";
                        byte[] response = Encoding.UTF8.GetBytes(responseMessage);
                        await SendResponseAsync(stream, response);
                    }
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

        private async Task HandleGetAllFileAsync(string update, NetworkStream stream)
        {
            string[] parts = update.Split('|');
            if (parts.Length == 2)
            {
                int idUser = int.Parse(parts[1]);

                DatabaseManager dbManager = new DatabaseManager();

                // Gọi hàm lấy danh sách tài liệu mà người dùng đã tham gia
                Dictionary<int, string> userDocs = dbManager.GetUserDocsByUserId(idUser);
                
                if (userDocs != null)
                {
                    // Chuyển Dictionary thành chuỗi với định dạng "DocID|Docname" mỗi cặp trên một dòng
                    string allFileName = string.Join(Environment.NewLine, userDocs.Select(doc => $"{doc.Key}@{doc.Value}"));
                   
                    string responseMessage = $"GET_ALL_FILE|{idUser}|SUCCESS|{allFileName}";
                    byte[] response = Encoding.UTF8.GetBytes(responseMessage);

                    await SendResponseAsync(stream, response);
                }
                else
                {
                    byte[] response = Encoding.UTF8.GetBytes($"GET_ALL_FILE|{idUser}|FAIL|");
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
            DatabaseManager dbManager = new DatabaseManager();
            // Tách DocID từ chuỗi update
            string[] parts = update.Split('|');
            int docID = int.Parse(parts[1]); // Lấy DocID
            int userID = int.Parse(parts[2]); // Lấy UserID
            string newContent = "";
            
            // Lấy nội dung hiện tại gửi tới client
            if (parts.Length == 3)
            {
                
                // Lấy nội dung hiện tại từ cơ sở dữ liệu
                newContent = await dbManager.GetDocumentContentByIdAsync(docID, userID);
                string reponseMessage = $"EDIT_DOC|{docID}|{userID}|" + newContent;
                // Gửi nội dung hiện tại dưới dạng RTF tới client mới
                byte[] contentBuffer = Encoding.UTF8.GetBytes(reponseMessage);
                await SendResponseAsync(stream, contentBuffer);
                return;
            }
            // Xử lý cập nhật
            newContent = parts[3];
            await ProcessUpdateAsync(newContent, sender, docID);
        }

        private async Task ProcessUpdateAsync(string update, TcpClient sender, int docId)
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
                MessageBox.Show($"Không thể lưu nội dung cho DocID: {docId}");
                return;
            }

            string broadcastMessage = $"EDIT_DOC|{docId}|" + sharedContent;
            await BroadcastUpdateAsync(broadcastMessage, sender);
        }
        
        private async Task BroadcastUpdateAsync(string update, TcpClient sender)
        {
            byte[] updateBuffer = Encoding.UTF8.GetBytes(update);
            List<TcpClient> clientsCopy;

            lock (clients)
            {
                clientsCopy = new List<TcpClient>(clients);
            }

            foreach (TcpClient client in clientsCopy)
            {
                if (client != sender)
                {
                    try
                    {
                        NetworkStream stream = client.GetStream();
                        await SendResponseAsync(stream, updateBuffer);
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine($"Broadcast IO Error: {ex.Message}");
                        CloseClientConnection(client); // Cleanup disconnected clients
                    }
                    catch (SocketException ex)
                    {
                        Console.WriteLine($"Broadcast Socket Error: {ex.Message}");
                        CloseClientConnection(client);
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
