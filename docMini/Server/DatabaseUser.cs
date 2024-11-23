using System;
using System.Data.SQLite;
using System.Security.Cryptography;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class DatabaseManager
{
    private string connectionString;

    public DatabaseManager()
    {
        string dbFileName = "DataUsers.db";
        string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dbFileName);
        connectionString = $"Data Source={dbPath};Version=3;";

        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        // Check if the database file exists, and create it if it doesn’t
        if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DataUsers.db")))
        {
            SQLiteConnection.CreateFile("DataUsers.db");
        }

        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            try
            {
                // Tạo bảng Users
                string createUsersTableQuery = @"
                    CREATE TABLE IF NOT EXISTS Users (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Username TEXT NOT NULL UNIQUE,
                        Email TEXT NOT NULL,
                        Password TEXT NOT NULL,
                        Status INTEGER NOT NULL
                    )";
                using (var command = new SQLiteCommand(createUsersTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }

                // Tạo bảng Docs
                string createDocsTableQuery = @"
                    CREATE TABLE IF NOT EXISTS Docs (
                        DocID INTEGER PRIMARY KEY AUTOINCREMENT,
                        Docname TEXT NOT NULL,
                        Owner TEXT NOT NULL,
                        Content TEXT,
                        LOpenTime TEXT,
                        FOREIGN KEY(Owner) REFERENCES Users(Username)
                    )";
                using (var command = new SQLiteCommand(createDocsTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }

                // Tạo bảng Users_Docs
                string createUsersDocsTableQuery = @"
                    CREATE TABLE IF NOT EXISTS Users_Docs (
                        UserId INTEGER NOT NULL,
                        DocID INTEGER NOT NULL,
                        EditStatus INTEGER NOT NULL CHECK(EditStatus IN (0, 1)),
                        PRIMARY KEY(UserId, DocID),
                        FOREIGN KEY(UserId) REFERENCES Users(Id),
                        FOREIGN KEY(DocID) REFERENCES Docs(DocID)
                    )";
                using (var command = new SQLiteCommand(createUsersDocsTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error initializing database: " + ex.Message);
            }
        }
    }
    // Phương thức mã hóa mật khẩu
    private string HashPassword(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }

    // Thêm người dùng với mật khẩu đã mã hóa, kiểm tra trùng lặp username
    public bool InsertUser(string username, string email, string password)
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            // Kiểm tra xem username có tồn tại chưa
            string checkUserQuery = "SELECT COUNT(1) FROM Users WHERE Username = @Username";
            using (var checkCommand = new SQLiteCommand(checkUserQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@Username", username);
                int count = Convert.ToInt32(checkCommand.ExecuteScalar());
                if (count > 0)
                {
                    return false;
                }
            }

            string hashedPassword = HashPassword(password);
            string insertQuery = "INSERT INTO Users (Username, Email, Password, Status) VALUES (@Username, @Email, @Password, 1)";

            using (var command = new SQLiteCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Password", hashedPassword); // Lưu mật khẩu đã mã hóa
                command.ExecuteNonQuery();
            }
        }
        return true;
    }

    // Xác thực người dùng
    public bool ValidateUser(string username, string password)
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            string hashedPassword = HashPassword(password); // Mã hóa mật khẩu nhập vào

            string query = "SELECT COUNT(1) FROM Users WHERE Username = @Username AND Password = @Password";
            using (var command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Password", hashedPassword);

                int count = Convert.ToInt32(command.ExecuteScalar());
                return count > 0; // Nếu tìm thấy người dùng
            }
        }
    }

    // Lấy IdUser theo Username
    public int GetUserIdByUsername(string username)
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            string query = "SELECT Id FROM Users WHERE Username = @Username";
            using (var command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Username", username);

                object result = command.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : -1; // Trả về -1 nếu không tìm thấy
            }
        }
    }
    // Lấy Username theo UserId
    public string GetUsernameByUserId(int userId)
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            string query = "SELECT Username FROM Users WHERE Id = @Id";
            using (var command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", userId);

                object result = command.ExecuteScalar();
                return result != null ? result.ToString() : null; // Trả về null nếu không tìm thấy
            }
        }
    }

    // Thêm tài liệu mới
    public bool AddNewDocument(string docName, string owner, int userId)
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            // Kiểm tra xem tài liệu đã tồn tại chưa
            string checkDocQuery = "SELECT COUNT(*) FROM Docs WHERE Docname = @Docname AND Owner = @Owner";
            using (var checkCommand = new SQLiteCommand(checkDocQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@Docname", docName);
                checkCommand.Parameters.AddWithValue("@Owner", owner);

                var count = Convert.ToInt32(checkCommand.ExecuteScalar());
                if (count > 0)
                {
                    return false; // Tài liệu đã tồn tại
                }
            }

            string currentTime = DateTime.Now.ToString("HH:mm:ss dd-MM-yyyy");

            // Thêm tài liệu mới vào bảng Docs
            string insertDocQuery = @"
            INSERT INTO Docs (Docname, Owner, Content, LOpenTime)
            VALUES (@Docname, @Owner, '', @LOpenTime)";
            using (var insertCommand = new SQLiteCommand(insertDocQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@Docname", docName);
                insertCommand.Parameters.AddWithValue("@Owner", owner);
                insertCommand.Parameters.AddWithValue("@LOpenTime", currentTime);
                insertCommand.ExecuteNonQuery();
            }

            // Lấy ID của tài liệu vừa được thêm
            string getDocIdQuery = "SELECT last_insert_rowid()";
            int docId;
            using (var getDocIdCommand = new SQLiteCommand(getDocIdQuery, connection))
            {
                docId = Convert.ToInt32(getDocIdCommand.ExecuteScalar());
            }

            // Thêm liên kết vào bảng Users_Docs
            string insertUserDocQuery = @"
            INSERT INTO Users_Docs (UserId, DocID, EditStatus)
            VALUES (@UserId, @DocID, 1)";
            using (var insertUserDocCommand = new SQLiteCommand(insertUserDocQuery, connection))
            {
                insertUserDocCommand.Parameters.AddWithValue("@UserId", userId);
                insertUserDocCommand.Parameters.AddWithValue("@DocID", docId);
                insertUserDocCommand.ExecuteNonQuery();
            }

            return true;
        }
    }

    public bool IsDocumentExists(string docName, string owner)
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            string query = "SELECT COUNT(*) FROM Docs WHERE Docname = @Docname AND owner = @Owner";
            using (var command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Docname", docName);
                command.Parameters.AddWithValue("@Owner", owner);

                object result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int count))
                {
                    return count > 0; // Trả về true nếu tồn tại
                }
                return false; // Trả về false nếu không tồn tại
            }
        }
    }

    // Lấy nội dung của Doc
    public async Task<string> GetDocumentContentByIdAsync(string docId)
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            string query = "SELECT Content FROM Docs WHERE DocID = @DocID";
            using (var command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@DocID", docId);
                object result = command.ExecuteScalar();
                return result != null ? result.ToString() : string.Empty;
            }
        }
    }

    // Cập nhập tài liệu
    public async Task<bool> UpdateDocumentContentAsync(string docId, string content)
    { 
        if (string.IsNullOrEmpty(docId) || string.IsNullOrEmpty(content))
        {
            Console.WriteLine("DocID hoặc nội dung rỗng. Không thể cập nhật.");
            return false;
        }

        try
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                await connection.OpenAsync();

                // Câu lệnh SQL để cập nhật nội dung tài liệu
                string query = "UPDATE Docs SET Content = @Content WHERE DocID = @DocID";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DocID", docId);
                    command.Parameters.AddWithValue("@Content", content);

                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    
                    // Kiểm tra số dòng bị ảnh hưởng
                    if (rowsAffected > 0)
                    {
                        Console.WriteLine($"Cập nhật thành công cho DocID: {docId}");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine($"Không tìm thấy DocID: {docId} để cập nhật.");
                        return false;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi khi cập nhật tài liệu: {ex.Message}");
            return false;
        }
    }    

    // Gắn tài liệu với người dùng và cấp quyền
    public bool LinkUserToDoc(int userId, int docId, int editStatus)
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            // Sử dụng INSERT OR IGNORE để tránh chèn trùng
            string insertQuery = "INSERT OR IGNORE INTO Users_Docs (UserId, DocID, EditStatus) VALUES (@UserId, @DocID, @EditStatus)";
            using (var command = new SQLiteCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@DocID", docId);
                command.Parameters.AddWithValue("@EditStatus", editStatus);
                command.ExecuteNonQuery();
            }
        }
        return true;
    }

    // Cập nhật thời gian mở tài liệu lần cuối
    public bool DocLastOpenTime(int docId, string lastOpenTime)
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            string updateQuery = "UPDATE Docs SET LOpenTime = @LOpenTime WHERE DocID = @DocID";
            using (var command = new SQLiteCommand(updateQuery, connection))
            {
                command.Parameters.AddWithValue("@LOpenTime", lastOpenTime);
                command.Parameters.AddWithValue("@DocID", docId);
                command.ExecuteNonQuery();
            }
        }
        return true;
    }

    // Lấy quyền truy cập của người dùng với tài liệu
    public int GetUserPermission(int userId, int docId)
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            string query = "SELECT EditStatus FROM Users_Docs WHERE UserId = @UserId AND DocID = @DocID";
            using (var command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@DocID", docId);

                object result = command.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : -1;
            }
        }
    }

    public List<string> GetUserDocsByUsername(string username)
    {
        List<string> docNames = new List<string>();

        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            // Lấy Id của người dùng từ Username
            string getUserIdQuery = "SELECT Id FROM Users WHERE Username = @Username";
            int userId = -1;
            using (var command = new SQLiteCommand(getUserIdQuery, connection))
            {
                command.Parameters.AddWithValue("@Username", username);
                object result = command.ExecuteScalar();
                if (result != null)
                {
                    userId = Convert.ToInt32(result);
                }
            }

            if (userId != -1)
            {
                // Truy vấn danh sách DocID mà người dùng tham gia
                string getDocsQuery = @"
                SELECT Docs.Docname 
                FROM Docs 
                INNER JOIN Users_Docs ON Docs.DocID = Users_Docs.DocID
                WHERE Users_Docs.UserId = @Id";

                using (var command = new SQLiteCommand(getDocsQuery, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string docName = reader.GetString(0);
                            docNames.Add(docName);
                        }
                    }
                }
            }
        }

        return docNames;
    }

}
