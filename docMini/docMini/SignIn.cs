using docMini;
using Server;

namespace docMini
{
    public partial class SignIn : Form
    {
        public SignIn()
        {
            InitializeComponent();
        }

        private void button_SignIn_Click(object sender, EventArgs e)
        {
            string username = textbox_Username.Text;
            string password = textbox_Password.Text;

            DatabaseManager dbManager = new DatabaseManager();
            bool isValidUser = dbManager.ValidateUser(username, password);

            if (isValidUser)
            {
                MessageBox.Show("Đăng nhập thành công!");
                this.Hide();
                mainDoc mainForm = new mainDoc();
                mainForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng.");
            }
        }

        private void label_ForgotPass_Click(object sender, EventArgs e)
        {
            this.Hide();
            FogotPass form = new FogotPass();
            form.Show();
        }

        private void label_SwitchSignUp_Click(object sender, EventArgs e)
        {
            this.Hide();
            SignUp form = new SignUp();
            form.Show();
        }
    }
}