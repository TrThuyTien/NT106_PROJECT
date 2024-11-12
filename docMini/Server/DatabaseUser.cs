using System;
using System.Data.SQLite;
using System.Security.Cryptography;
using System.Text;

public class DatabaseManager
{
    private string connectionString;

    public DatabaseManager()
    {
        connectionString = "Data Source=DataUsers.db;Version=3;";
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL UNIQUE,
                    Email TEXT NOT NULL,
                    Password TEXT NOT NULL,
                    Status INTEGER NOT NULL
                )";


            string createDocsTableQuery = @"
                CREATE TABLE IF NOT EXISTS Docs (
                    DocID INTEGER PRIMARY KEY AUTOINCREMENT,
                    LOpenTime TEXT
                )";

            string createUsersDocsTableQuery = @"
                CREATE TABLE IF NOT EXISTS Users_Docs (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserId INTEGER NOT NULL,
                    DocID INTEGER NOT NULL,
                    EditStatus INTEGER NOT NULL,
                    FOREIGN KEY(UserId) REFERENCES Users(Id),
                    FOREIGN KEY(DocID) REFERENCES Docs(DocID)
                )";

            using (var command = new SQLiteCommand(createTableQuery, connection))
            {
                command.ExecuteNonQuery();
            }

            using (var command = new SQLiteCommand(createDocsTableQuery, connection))
            {
                command.ExecuteNonQuery();
            }

            using (var command = new SQLiteCommand(createUsersDocsTableQuery, connection))
            {
                command.ExecuteNonQuery();
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

    // Gắn tài liệu với người dùng và cấp quyền
    public bool LinkUserToDoc(int userId, int docId, int editStatus)
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            string insertQuery = "INSERT INTO Users_Docs (UserId, DocID, EditStatus) VALUES (@UserId, @DocID, @EditStatus)";
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
