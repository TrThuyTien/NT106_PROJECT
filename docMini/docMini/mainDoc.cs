using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace docMini
{
    public partial class mainDoc : Form
    {
        public mainDoc()
        {
            InitializeComponent();
        }


        private void button_Minimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button_Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void panel_searchDoc_Paint(object sender, PaintEventArgs e)
        {
            Panel panel = sender as Panel;
            Color borderColor = Color.Black; // Màu viền bạn muốn

            int borderWidth = 5; // Độ dày của viền
            ControlPaint.DrawBorder(e.Graphics, panel.ClientRectangle, borderColor, borderWidth, ButtonBorderStyle.Solid,
                                    borderColor, borderWidth, ButtonBorderStyle.Solid,
                                    borderColor, borderWidth, ButtonBorderStyle.Solid,
                                    borderColor, borderWidth, ButtonBorderStyle.Solid);
        }




        private void button_newFile_Click(object sender, EventArgs e)
        {
            mainDoc mD = new mainDoc();
            mD.Show();
        }

        private void button_ShareDoc_Click(object sender, EventArgs e)
        {

        }
    }
}
