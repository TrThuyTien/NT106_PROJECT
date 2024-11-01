using docMini;

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
            this.Hide();
            new mainDoc().ShowDialog();
            this.Show();
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
