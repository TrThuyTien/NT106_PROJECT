using System.Net.Sockets;
using System.Net;
using System.Text;
using System.IO.Compression;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using Microsoft.VisualBasic.ApplicationServices;

namespace Server
{
    public partial class Server : Form
    {
        public Server()
        {
            InitializeComponent();
            textBox_Port.Text = "8080";
            crypto = new Crypto(pass);
        }


        TcpListener listener;
        private static List<TcpClient> clients = new List<TcpClient>();
        private static string sharedContent = ""; // Nội dung văn bản chung
        private static readonly object lockObject = new object();
        private static DateTime lastUpdateTime = DateTime.MinValue;
        private static readonly TimeSpan debounceTime = TimeSpan.FromMilliseconds(300); // Thời gian debounce
        private bool isListening = false;
        private readonly string pass = "SuperSecureSharedSecret123!";
        private Crypto crypto;
        public object Address { get; private set; }

        private CancellationTokenSource cts = new CancellationTokenSource();

        private async void button_Listen_Click(object sender, EventArgs e)
        {
            int port = int.Parse(textBox_Port.Text);
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            richTextBox_Editor.Text = $"Server đang chạy trên cổng {port}...";
            isListening = true;

            while (isListening)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                lock (clients)
                {
                    clients.Add(client);
                }
                _ = Task.Run(() => handleClientAsync(client, cts.Token));
            }
        }

        private async Task handleClientAsync(TcpClient client, CancellationToken token)
        {
            try
            {
                NetworkStream stream = client.GetStream();

                using (BinaryReader reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true))
                {
                    while (!token.IsCancellationRequested && client.Connected)
                    {
                        // Bước 1: Đọc độ dài dữ liệu đã nén
                        byte[] lengthBuffer = await ReadMessageAsync(stream, sizeof(int));
                        int compressedLength = BitConverter.ToInt32(lengthBuffer, 0);

                        // Bước 2: Đọc dữ liệu đã nén
                        byte[] compressedData = await ReadMessageAsync(stream, compressedLength);

                        // Bước 3: Giải nén dữ liệu
                        byte[] encryptedData = Decompress(compressedData);

                        // Bước 4: Giải mã dữ liệu
                        byte[] decryptedData = Encoding.UTF8.GetBytes(crypto.Decrypt(encryptedData));

                        // Bước 5: Chuyển dữ liệu giải mã thành chuỗi
                        string update = Encoding.UTF8.GetString(decryptedData);

                        // Bước 6: Xử lý yêu cầu
                        await ProcessRequestAsync(update, stream, client);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                CloseClientConnection(client);
            }
        }


        private async Task<byte[]> ReadMessageAsync(NetworkStream stream, int length)
        {
            byte[] buffer = new byte[length];
            int bytesRead = 0;
            while (bytesRead < length)
            {
                int read = await stream.ReadAsync(buffer, bytesRead, length - bytesRead);
                if (read == 0) throw new IOException("Connection closed during read.");
                bytesRead += read;
            }
            return buffer;
        }

        private async Task ProcessRequestAsync(string update, NetworkStream stream, TcpClient client)
        {

            if (update.StartsWith("SIGN_IN|"))
            {
                await HandleSignInAsync(update, stream, client);
            }
            else if (update.StartsWith("SIGN_UP|"))
            {
                await HandleSignUpAsync(update, stream, client);
            }
            else if (update.StartsWith("NEW_FILE|"))
            {
                await HandleNewFileAsync(update, stream, client);
            }
            else if (update.StartsWith("EDIT_DOC|"))
            {
                await HandleEditDocumentAsync(update, stream, client);
            }
            else if (update.StartsWith("GET_ALL_FILE|"))
            {
                await HandleGetAllFileAsync(update, stream, client);
            }
            else if (update.StartsWith("SHARE_FILE|"))
            {
                await HandleShareDocumentAsync(update, stream, client);
            }
            else if (update.StartsWith("DELETE_FILE|"))
            {
                await HandleDeleteDocumentAsync(update, stream, client);
            }
        }

        // XÓA FILE
        private async Task HandleDeleteDocumentAsync(string update, NetworkStream stream, TcpClient client)
        {
            try
            {
                string[] parts = update.Split('|');
                if (parts.Length == 3)
                {
                    int userID = int.Parse(parts[1].Trim());
                    int docID = int.Parse(parts[2].Trim());


                    DatabaseManager dbManager = new DatabaseManager();

                    // Lấy trạng thái xóa file
                    int success = dbManager.DeleteFileById(userID, docID);
                    if (success == -1)
                    {
                        byte[] response = Encoding.UTF8.GetBytes($"DELETE_FILE|{userID}|{docID}|FILE_NOT_FOUND");
                        await SendResponseAsync(stream, response, client);
                        return;
                    }
                    else if (success == 1)
                    {
                        string responseMessage = $"DELETE_FILE|{userID}|{docID}|SUCCESS";
                        byte[] response = Encoding.UTF8.GetBytes(responseMessage);
                        await SendResponseAsync(stream, response, client);
                        return;
                    }
                    else
                    {
                        string responseMessage = $"DELETE_FILE|{userID}|{docID}|FAIL";
                        byte[] response = Encoding.UTF8.GetBytes(responseMessage);
                        await SendResponseAsync(stream, response, client);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in HandleShareDocumentAsync: {ex.Message}");
            }
        }

        // SHARE_FILE
        private async Task HandleShareDocumentAsync(string update, NetworkStream stream, TcpClient client)
        {
            try
            {
                string[] parts = update.Split('|');
                if (parts.Length == 5)
                {
                    int ownerID = int.Parse(parts[1].Trim());
                    int docID = int.Parse(parts[2].Trim());
                    string userNameToAdd = parts[3].Trim();
                    string mode = parts[4].Trim();

                    // "Chỉnh sửa" -> editStatus = 1, "Xem" -> editStatus = 0
                    int editStatus = mode.Equals("Chỉnh sửa", StringComparison.OrdinalIgnoreCase) ? 1 : 0;

                    DatabaseManager dbManager = new DatabaseManager();

                    // Lấy ID người dùng
                    int idUserToAdd = dbManager.GetUserIdByUsername(userNameToAdd);
                    if (idUserToAdd < 1)
                    {
                        byte[] response = Encoding.UTF8.GetBytes($"SHARE_FILE|{ownerID}|{docID}|USER_NOT_FOUND");
                        await SendResponseAsync(stream, response, client);
                        /*await Task.Run(() => richTextBox_Editor.Invoke((Action)(() =>
                        {
                            richTextBox_Editor.Text += "Server : " + $"SHARE_FILE|{ownerID}|{docID}|USER_NOT_FOUND" + Environment.NewLine; // Cập nhật giao diện với RTF
                        })));*/
                        return;
                    }

                    // Thêm vào bảng User_Doc
                    bool success = dbManager.AddUserDocLink(idUserToAdd, docID, editStatus, ownerID);

                    if (success)
                    {
                        string responseMessage = $"SHARE_FILE|{ownerID}|{docID}|SUCCESS";
                        byte[] response = Encoding.UTF8.GetBytes(responseMessage);
                        await SendResponseAsync(stream, response, client);
                        /*await Task.Run(() => richTextBox_Editor.Invoke((Action)(() =>
                        {
                            richTextBox_Editor.Text += "Server : " + $"SHARE_FILE|{ownerID}|{docID}|SUCCESS" + Environment.NewLine; // Cập nhật giao diện với RTF
                        })));*/
                    }
                    else
                    {
                        string responseMessage = $"SHARE_FILE|{ownerID}|{docID}|FAIL";
                        byte[] response = Encoding.UTF8.GetBytes(responseMessage);
                        await SendResponseAsync(stream, response, client);
                        /*await Task.Run(() => richTextBox_Editor.Invoke((Action)(() =>
                        {
                            richTextBox_Editor.Text += "Server : " + $"SHARE_FILE|{ownerID}|{docID}|FAIL" + Environment.NewLine; // Cập nhật giao diện với RTF
                        })));*/
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in HandleShareDocumentAsync: {ex.Message}");
            }
        }

        // NEW_FILE
        private async Task HandleNewFileAsync(string update, NetworkStream stream, TcpClient client)
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
                        await SendResponseAsync(stream, respons, client);
                        /*await Task.Run(() => richTextBox_Editor.Invoke((Action)(() =>
                        {
                            richTextBox_Editor.Text += "Server : " + $"NEW_FILE|{userId}|DUPLICATE" + Environment.NewLine; // Cập nhật giao diện với RTF
                        })));*/
                        return;
                    }

                    // Thêm tài liệu mới vào cơ sở dữ liệu, thành công trả về idDoc, thất bại trả về 0
                    int idDoc = dbManager.AddNewDocument(fileName, owner, userId);

                    // Gửi phản hồi
                    if (idDoc > 0)
                    {
                        string responseMessage = $"NEW_FILE|{userId}|SUCCESS|{idDoc}";
                        byte[] response = Encoding.UTF8.GetBytes(responseMessage);
                        await SendResponseAsync(stream, response, client);
                        /*await Task.Run(() => richTextBox_Editor.Invoke((Action)(() =>
                        {
                            richTextBox_Editor.Text += "Server : " + $"NEW_FILE|{userId}|SUCCESS|{idDoc}" + Environment.NewLine; // Cập nhật giao diện với RTF
                        })));*/
                    }
                    else
                    {
                        string responseMessage = $"NEW_FILE|{userId}|FAIL";
                        byte[] response = Encoding.UTF8.GetBytes(responseMessage);
                        await SendResponseAsync(stream, response, client);
                        /*await Task.Run(() => richTextBox_Editor.Invoke((Action)(() =>
                        {
                            richTextBox_Editor.Text += "Server : " + $"NEW_FILE|{userId}|FAIL" + Environment.NewLine; // Cập nhật giao diện với RTF
                        })));*/
                    }
                }
            }
            catch (Exception ex)
            {
                byte[] response = Encoding.UTF8.GetBytes("ERROR");
                await SendResponseAsync(stream, response, client);
                /*await Task.Run(() => richTextBox_Editor.Invoke((Action)(() =>
                {
                    richTextBox_Editor.Text += "Server : " + "ERROR" + Environment.NewLine; // Cập nhật giao diện với RTF
                })));*/
                Console.WriteLine($"Error in HandleNewFileAsync: {ex.Message}");
            }
        }

        // SIGN_IN
        private async Task HandleSignInAsync(string update, NetworkStream stream, TcpClient client)
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
                    await SendResponseAsync(stream, response, client);
                    /*await Task.Run(() => richTextBox_Editor.Invoke((Action)(() =>
                    {
                        richTextBox_Editor.Text += "Server : " + $"SIGN_IN|{username}|SUCCESS|{userId}" + Environment.NewLine; // Cập nhật giao diện với RTF
                    })));*/
                }
                else
                {
                    // Gửi phản hồi thất bại
                    byte[] response = Encoding.UTF8.GetBytes($"SIGN_IN|{username}|FAIL");
                    await SendResponseAsync(stream, response, client);
                    /*await Task.Run(() => richTextBox_Editor.Invoke((Action)(() =>
                    {
                        richTextBox_Editor.Text += "Server : " + $"SIGN_IN|{username}|FAIL" + Environment.NewLine; // Cập nhật giao diện với RTF
                    })));*/
                }
            }
        }

        // SIGN_UP
        private async Task HandleSignUpAsync(string update, NetworkStream stream, TcpClient client)
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
                    await SendResponseAsync(stream, response, client);
                }
                else
                {
                    byte[] response = Encoding.UTF8.GetBytes($"SIGN_UP|{username}|FAIL");
                    await SendResponseAsync(stream, response, client);
                }
            }
        }

        // GET_ALL_FILE
        private async Task HandleGetAllFileAsync(string update, NetworkStream stream, TcpClient client)
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

                    await SendResponseAsync(stream, response, client);
                }
                else
                {
                    byte[] response = Encoding.UTF8.GetBytes($"GET_ALL_FILE|{idUser}|FAIL|");
                    await SendResponseAsync(stream, response, client);
                }
            }
        }

        // EDIT_DOC
        private async Task HandleEditDocumentAsync(string update, NetworkStream stream, TcpClient sender)
        {
            DatabaseManager dbManager = new DatabaseManager();
            // Tách DocID từ chuỗi update
            string[] parts = update.Split('|');
            int docID = int.Parse(parts[1]); // Lấy DocID
            int userID = int.Parse(parts[2]); // Lấy UserID
            string newContent = "";
            bool documentExists = dbManager.IsDocumentExists(docID);
            if (!documentExists)
            {
                string reponseMessage = $"EDIT_DOC|{docID}|{userID}|FAIL";
                byte[] contentBuffer = Encoding.UTF8.GetBytes(reponseMessage);
                await SendResponseAsync(stream, contentBuffer, sender);
                return;
            }
            else
            {
                // Lấy nội dung hiện tại gửi tới client
                if (parts.Length == 3)
                {

                    // Lấy nội dung hiện tại từ cơ sở dữ liệu
                    newContent = await dbManager.GetDocumentContentByIdAsync(docID, userID);
                    // Lấy quyền truy cập file
                    int editStatus = dbManager.GetUserPermission(userID, docID);
                    string reponseMessage = $"EDIT_DOC|{docID}|{userID}|{editStatus}|" + newContent;
                    // Gửi nội dung hiện tại dưới dạng RTF tới client mới
                    byte[] contentBuffer = Encoding.UTF8.GetBytes(reponseMessage);
                    await SendResponseAsync(stream, contentBuffer, sender);
                    /*await Task.Run(() => richTextBox_Editor.Invoke((Action)(() =>
                    {
                        richTextBox_Editor.Text += "Server : " + $"EDIT_DOC|{docID}|{userID}|{editStatus}|" + newContent + Environment.NewLine; // Cập nhật giao diện với RTF
                    })));*/
                    return;
                }
                // Xử lý cập nhật
                newContent = parts[3];
                await ProcessUpdateAsync(newContent, sender, docID);
            }
        }

        // GỬI PHẢN HỒI
        private async Task SendResponseAsync(NetworkStream stream, byte[] response, TcpClient client)
        {
            // Chuyển response từ byte[] -> string -> Encrypt
            byte[] encryptedResponse = crypto.Encrypt(Encoding.UTF8.GetString(response));

            // Nén dữ liệu phản hồi đã mã hóa
            byte[] compressedResponse = Compress(encryptedResponse);

            // Chuyển đổi độ dài dữ liệu đã nén thành mảng byte
            byte[] lengthBuffer = BitConverter.GetBytes(compressedResponse.Length);

            // Gửi độ dài dữ liệu
            await stream.WriteAsync(lengthBuffer, 0, lengthBuffer.Length);

            // Gửi dữ liệu đã nén
            await stream.WriteAsync(compressedResponse, 0, compressedResponse.Length);
        }


        // UPDATE NỘI DUNG DOCUMENT
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
                MessageBox.Show($"Server: Không thể lưu nội dung cho DocID: {docId}");
                return;
            }
            string broadcastMessage = $"UPDATE_DOC|{docId}|" + sharedContent;
            await BroadcastUpdateAsync(broadcastMessage, sender);
        }

        // BROADCAST TỚI CÁC CLIENT
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
                        await SendResponseAsync(stream, updateBuffer, client);
                        /*await Task.Run(() => richTextBox_Editor.Invoke((Action)(() =>
                        {
                            richTextBox_Editor.Text += "Server broadcast: " + update + Environment.NewLine; // Cập nhật giao diện với RTF
                        })));*/
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