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
    }
}
