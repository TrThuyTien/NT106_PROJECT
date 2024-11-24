using DocumentFormat.OpenXml.Vml.Spreadsheet;
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
    public partial class addLink : Form
    {
        public string LinkText { get; set; }
        public string LinkAddress { get; set; }
        public addLink(string selectedText = "")
        {
            InitializeComponent();
            if (!string.IsNullOrEmpty(selectedText))
            {
                richTextBox_Display.Text = selectedText;
                richTextBox_Display.Enabled = false; // Disable editing if text is selected
            }
        }

        private void button_ok_Click(object sender, EventArgs e)
        {
            LinkText = richTextBox_Display.Text;
            LinkAddress = richTextBox_Address.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
