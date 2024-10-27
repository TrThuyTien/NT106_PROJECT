using miniDoc;

namespace docMini
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void button_DangNhap_Click(object sender, EventArgs e)
        {
            mainDoc main = new mainDoc();
            this.Hide();
            main.Show();
        }

        private void button_DangKy_Click(object sender, EventArgs e)
        {
            SignIn sign = new SignIn();
            sign.Show();
        }

        private void button_QuenMatKhau_Click(object sender, EventArgs e)
        {
            FogotPass fogotPass = new FogotPass();
            fogotPass.Show();
        }
    }
}
