using System;
using System.Windows.Forms;
using Server;

namespace docMini
{
    public partial class SignUp : Form
    {
        private DatabaseManager dbManager;
        public SignUp()
        {
            InitializeComponent();
            dbManager = new DatabaseManager();
        }

        private void label_SwitchSignIn_Click(object sender, EventArgs e)
        {
            this.Hide();
            SignIn form = new SignIn();
            form.Show();
        }

        private void button_SignUp_Click(object sender, EventArgs e)
        {
            string username = textbox_Username.Text;
            string email = textbox_Email.Text;
            string password = textbox_Password.Text;
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool success = dbManager.InsertUser(username, email, password);
            if (success)
            {
                MessageBox.Show("Đăng ký thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Hide();
                new mainDoc().ShowDialog();
                new SignIn().ShowDialog();
            }
            else
            {
                MessageBox.Show("Tên người dùng đã tồn tại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}