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
                        OwnerID INTEGER NOT NULL,
                        Content TEXT,
                        LOpenTime TEXT,
                        FOREIGN KEY(OwnerID) REFERENCES Users(Id)
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
    public int AddNewDocument(string docName, string owner, int userId)
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            // Kiểm tra xem tài liệu đã tồn tại chưa
            string checkDocQuery = "SELECT COUNT(*) FROM Docs WHERE Docname = @Docname AND OwnerID = @OwnerID";
            using (var checkCommand = new SQLiteCommand(checkDocQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@Docname", docName);
                checkCommand.Parameters.AddWithValue("@OwnerID", userId);

                var count = Convert.ToInt32(checkCommand.ExecuteScalar());
                if (count > 0)
                {
                    return 0; // Tài liệu đã tồn tại
                }
            }

            string currentTime = DateTime.Now.ToString("HH:mm:ss dd-MM-yyyy");

            // Thêm tài liệu mới vào bảng Docs
            string insertDocQuery = @"
            INSERT INTO Docs (Docname, OwnerID, Content, LOpenTime)
            VALUES (@Docname, @OwnerID, '', @LOpenTime)";
            using (var insertCommand = new SQLiteCommand(insertDocQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@Docname", docName);
                insertCommand.Parameters.AddWithValue("@OwnerID", userId);
                insertCommand.Parameters.AddWithValue("@LOpenTime", currentTime);
                insertCommand.ExecuteNonQuery();
            }

            // Lấy ID của tài liệu vừa được thêm
            string getDocIdQuery = "SELECT last_insert_rowid()";
            int docId = 0;
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

            return docId;
        }
    }

    // Thêm 1 bản ghi vào bảng User_Doc
    public bool AddUserDocLink(int userId, int docId, int mode, int ownerID)
    {
        try
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Kiểm tra xem ownerID có phải là chủ sở hữu của docId không
                string checkOwnerQuery = @"
            SELECT COUNT(*)
            FROM Docs
            WHERE DocID = @DocID AND OwnerID = @OwnerID";

                using (var checkOwnerCommand = new SQLiteCommand(checkOwnerQuery, connection))
                {
                    checkOwnerCommand.Parameters.AddWithValue("@DocID", docId);
                    checkOwnerCommand.Parameters.AddWithValue("@OwnerID", ownerID);

                    long isOwner = (long)checkOwnerCommand.ExecuteScalar();

                    // Nếu ownerID không phải chủ sở hữu, từ chối thêm
                    if (isOwner == 0)
                    {
                        Console.WriteLine("Operation denied. Provided ownerID is not the owner of the document.");
                        return false;
                    }
                }

                // Kiểm tra xem bản ghi UserId-DocID đã tồn tại hay chưa
                string checkExistQuery = @"
            SELECT COUNT(*)
            FROM Users_Docs
            WHERE UserId = @UserId AND DocID = @DocID";

                using (var checkExistCommand = new SQLiteCommand(checkExistQuery, connection))
                {
                    checkExistCommand.Parameters.AddWithValue("@UserId", userId);
                    checkExistCommand.Parameters.AddWithValue("@DocID", docId);

                    long count = (long)checkExistCommand.ExecuteScalar();

                    if (count > 0)
                    {
                        // Nếu bản ghi đã tồn tại, thực hiện UPDATE
                        string updateQuery = @"
                    UPDATE Users_Docs
                    SET EditStatus = @EditStatus
                    WHERE UserId = @UserId AND DocID = @DocID";

                        using (var updateCommand = new SQLiteCommand(updateQuery, connection))
                        {
                            updateCommand.Parameters.AddWithValue("@EditStatus", mode);
                            updateCommand.Parameters.AddWithValue("@UserId", userId);
                            updateCommand.Parameters.AddWithValue("@DocID", docId);
                            updateCommand.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        // Nếu bản ghi chưa tồn tại, thực hiện INSERT
                        string insertUserDocQuery = @"
                    INSERT INTO Users_Docs (UserId, DocID, EditStatus)
                    VALUES (@UserId, @DocID, @EditStatus)";

                        using (var insertUserDocCommand = new SQLiteCommand(insertUserDocQuery, connection))
                        {
                            insertUserDocCommand.Parameters.AddWithValue("@UserId", userId);
                            insertUserDocCommand.Parameters.AddWithValue("@DocID", docId);
                            insertUserDocCommand.Parameters.AddWithValue("@EditStatus", mode);
                            insertUserDocCommand.ExecuteNonQuery();
                        }
                    }
                }
            }

            // Trả về true nếu không có lỗi
            return true;
        }
        catch (Exception ex)
        {
            // Xử lý ngoại lệ nếu xảy ra lỗi
            Console.WriteLine($"Error adding/updating record in Users_Docs: {ex.Message}");
            return false;
        }
    }

    // Xóa 

    // Kiểm tra doc có tồn tại hay không
    public bool IsDocumentExists(string docName, int idOwner)
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            // Sử dụng đúng tên cột OwnerID thay vì Owner
            string query = "SELECT COUNT(*) FROM Docs WHERE Docname = @Docname AND OwnerID = @OwnerID";

            using (var command = new SQLiteCommand(query, connection))
            {
                // Gán tham số
                command.Parameters.AddWithValue("@Docname", docName);
                command.Parameters.AddWithValue("@OwnerID", idOwner);

                // Thực hiện truy vấn và kiểm tra kết quả
                object result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int count))
                {
                    return count > 0; // Trả về true nếu tồn tại tài liệu
                }
                return false; // Trả về false nếu không tồn tại
            }
        }
    }
    public bool IsDocumentExists(int idDoc)
    {

        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            string query = "SELECT COUNT(*) FROM Docs WHERE DocID = @DocID";

            using (var command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@DocID", idDoc);

                object result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int count))
                {
                    return count > 0; // Trả về true nếu tài liệu tồn tại
                }
                return false; // Trả về false nếu tài liệu không tồn tại
            }
        }
    }

    // Xóa file
    public int DeleteFileById(int idUser, int idDoc)
    {
        try
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Kiểm tra sự tồn tại của idDoc
                if (!IsDocumentExists(idDoc))
                {
                    Console.WriteLine($"Tài liệu với ID {idDoc} không tồn tại.");
                    return -1;
                }

                // Tạo giao dịch
                using (var transaction = connection.BeginTransaction())
                {
                    // Kiểm tra xem idUser có phải là chủ sở hữu của idDoc không
                    string checkOwnerQuery = @"
                    SELECT OwnerID 
                    FROM Docs 
                    WHERE DocID = @DocID";
                    using (var checkCommand = new SQLiteCommand(checkOwnerQuery, connection, transaction))
                    {
                        checkCommand.Parameters.AddWithValue("@DocID", idDoc);
                        var ownerId = checkCommand.ExecuteScalar();

                        if (ownerId != null && int.TryParse(ownerId.ToString(), out int ownerID) && ownerID == idUser)
                        {
                            // idUser là chủ sở hữu -> Xóa bản ghi trong Docs và Users_Docs
                            string deleteDocQuery = @"
                            DELETE FROM Docs 
                            WHERE DocID = @DocID";
                            using (var deleteDocCommand = new SQLiteCommand(deleteDocQuery, connection, transaction))
                            {
                                deleteDocCommand.Parameters.AddWithValue("@DocID", idDoc);
                                deleteDocCommand.ExecuteNonQuery();
                            }

                            string deleteUserDocsQuery = @"
                            DELETE FROM Users_Docs 
                            WHERE DocID = @DocID";
                            using (var deleteUserDocsCommand = new SQLiteCommand(deleteUserDocsQuery, connection, transaction))
                            {
                                deleteUserDocsCommand.Parameters.AddWithValue("@DocID", idDoc);
                                deleteUserDocsCommand.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            // idUser không phải là chủ sở hữu -> Xóa bản ghi trong Users_Docs
                            string deleteUserDocsQuery = @"
                            DELETE FROM Users_Docs 
                            WHERE UserId = @UserId AND DocID = @DocID";
                            using (var deleteUserDocsCommand = new SQLiteCommand(deleteUserDocsQuery, connection, transaction))
                            {
                                deleteUserDocsCommand.Parameters.AddWithValue("@UserId", idUser);
                                deleteUserDocsCommand.Parameters.AddWithValue("@DocID", idDoc);
                                deleteUserDocsCommand.ExecuteNonQuery();
                            }
                        }
                    }

                    // Commit giao dịch
                    transaction.Commit();
                }

                connection.Close();
            }

            Console.WriteLine("Xóa file thành công.");
            return 1;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Lỗi khi xóa file: " + ex.Message);
            return 0;
        }
    }



    // Lấy nội dung của Doc
    public async Task<string> GetDocumentContentByIdAsync(int docID, int userID)
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            await connection.OpenAsync();

            // Query để kiểm tra quyền truy cập và lấy nội dung tài liệu
            string query = @"
            SELECT d.Content
            FROM Docs d
            INNER JOIN Users_Docs ud ON d.DocID = ud.DocID
            WHERE d.DocID = @DocID AND ud.UserId = @UserID";

            using (var command = new SQLiteCommand(query, connection))
            {
                // Thêm tham số cho câu lệnh truy vấn
                command.Parameters.AddWithValue("@DocID", docID);
                command.Parameters.AddWithValue("@UserID", userID);

                // Thực thi câu lệnh và lấy kết quả
                object result = await command.ExecuteScalarAsync();

                // Trả về nội dung nếu tìm thấy, ngược lại trả về chuỗi rỗng
                return result != null ? result.ToString() : string.Empty;
            }
        }
    }

    // Cập nhập tài liệu
    public async Task<bool> UpdateDocumentContentAsync(int docId, string content)
    {
        if (docId < 1 || string.IsNullOrEmpty(content))
        {
            /*MessageBox.Show("DocID hoặc nội dung rỗng. Không thể cập nhật.");*/
            return false;
        }

        try
        {
            // Thêm timeout để tránh bị khóa
            var connectionStringWithTimeout = $"{connectionString};BusyTimeout=3000"; // 3 giây

            using (SQLiteConnection connection = new SQLiteConnection(connectionStringWithTimeout))
            {
                await connection.OpenAsync();

                // Đảm bảo rằng cơ sở dữ liệu không bị khóa
                await Task.Delay(50); // Tăng khoảng trễ nhỏ để giải phóng khóa nếu có giao dịch khác đang diễn ra.

                string query = "UPDATE Docs SET Content = @Content WHERE DocID = @DocID";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DocID", docId);
                    command.Parameters.AddWithValue("@Content", content);

                    int rowsAffected = await command.ExecuteNonQueryAsync();

                    if (rowsAffected > 0)
                    {
                        /*MessageBox.Show($"Cập nhật thành công cho DocID: {docId}");*/
                        return true;
                    }
                    else
                    {
                        /* MessageBox.Show($"Không tìm thấy DocID: {docId} để cập nhật.");*/
                        return false;
                    }
                }
            }
        }
        catch (SQLiteException ex) when (ex.Message.Contains("database is locked"))
        {
            // Xử lý riêng lỗi bị khóa
            /* MessageBox.Show($"Lỗi: Cơ sở dữ liệu đang bị khóa. Vui lòng thử lại sau.");*/
            return false;
        }
        catch (Exception ex)
        {
            /*MessageBox.Show($"Lỗi khi cập nhật tài liệu {docId}: {ex.Message}");*/
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

    public Dictionary<int, string> GetUserDocsByUserId(int userId)
    {
        // Khởi tạo Dictionary để lưu trữ kết quả
        Dictionary<int, string> documents = new Dictionary<int, string>();

        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            // Truy vấn danh sách DocID và Docname mà người dùng tham gia
            string getDocsQuery = @"
        SELECT Docs.DocID, Docs.Docname 
        FROM Docs 
        INNER JOIN Users_Docs ON Docs.DocID = Users_Docs.DocID
        WHERE Users_Docs.UserId = @UserId";

            using (var command = new SQLiteCommand(getDocsQuery, connection))
            {
                command.Parameters.AddWithValue("@UserId", userId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Lấy DocID và Docname từ dữ liệu đọc được
                        int docId = reader.GetInt32(0);
                        string docName = reader.GetString(1);

                        // Thêm vào Dictionary
                        documents[docId] = docName;
                    }
                }
            }
        }

        return documents;
    }

}
