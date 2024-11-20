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
                // Xóa Users_Docs table (chỉ dùng để kiểm thử, chạy dự án thật sẽ xóa đi)
                string dropUsersDocsTableQuery = "DROP TABLE IF EXISTS Users_Docs;";
                using (var command = new SQLiteCommand(dropUsersDocsTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }

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
                        EditStatus INTEGER NOT NULL CHECK(EditStatus IN (0, 1, 2)),
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

    // Thêm tài liệu mới
    public int InsertDoc(string lastOpenTime)
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            string insertQuery = "INSERT INTO Docs (LOpenTime) VALUES (@LOpenTime); SELECT last_insert_rowid();";
            using (var command = new SQLiteCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@LOpenTime", lastOpenTime);
                return Convert.ToInt32(command.ExecuteScalar());
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
                string query = "UPDATE Documents SET Content = @Content WHERE DocID = @DocID";

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

}
