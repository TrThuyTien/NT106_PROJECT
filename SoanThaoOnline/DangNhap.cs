using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoanThaoOnline
{
    public partial class DangNhap : Form
    {
        public DangNhap()
        {
            InitializeComponent();
        }

        private void DangNhap_Load(object sender, EventArgs e)
        {

        }

        private void button_DangNhap_Click(object sender, EventArgs e)
        {
            App app = new App();
            this.Hide();
            app.Show();
        }

        private void button_DangKy_Click(object sender, EventArgs e)
        {
            DangKy dangKy = new DangKy();
            dangKy.Show();
        }

        private void button_QuenMatKhau_Click(object sender, EventArgs e)
        {
            QuenMatKhau quenmatkhau = new QuenMatKhau();
            quenmatkhau.Show();
        }
    }
}
