
using Aspose.Words.Drawing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xceed.Words.NET;
using GemBox.Document;
using System.Windows.Controls;
using Xceed.Document.NET;
namespace docMini
{
    public partial class mainDoc : Form
    {
        public mainDoc()
        {
            InitializeComponent();
           // ComponentInfo.SetLicense("FREE-LIMITED-KEY");

        }


        private void button_Minimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button_Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void button_newFile_Click(object sender, EventArgs e)
        {
            mainDoc mD = new mainDoc();
            mD.Show();
        }

        private void button_ShareDoc_Click(object sender, EventArgs e)
        {

        }

        private void button_Bold_Click(object sender, EventArgs e)
        {
            if (richTextBox_Content.SelectionLength > 0)
            {
                int selectionStart = richTextBox_Content.SelectionStart;
                int selectionLength = richTextBox_Content.SelectionLength;
                string selectedFont = comboBox_font.SelectedItem.ToString();

                for (int i = 0; i < selectionLength; i++)
                {
                    richTextBox_Content.Select(selectionStart + i, 1);
                    if (richTextBox_Content.SelectionFont != null)
                    {
                        System.Drawing.Font currentFont = richTextBox_Content.SelectionFont;
                        System.Drawing.FontStyle newFontStyle = richTextBox_Content.SelectionFont.Style ^ System.Drawing.FontStyle.Bold;
                        richTextBox_Content.SelectionFont = new System.Drawing.Font(currentFont.FontFamily, currentFont.Size, newFontStyle);
                    }
                    richTextBox_Content.Select(selectionStart, selectionLength);
                }
            }
        }

        private void comboBox_font_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (richTextBox_Content.SelectionLength > 0)
            {
                int selectionStart = richTextBox_Content.SelectionStart;
                int selectionLength = richTextBox_Content.SelectionLength;
                string selectedFont = comboBox_font.SelectedItem.ToString();

                for (int i = 0; i < selectionLength; i++)
                {
                    richTextBox_Content.Select(selectionStart + i, 1);
                    if (richTextBox_Content.SelectionFont != null)
                    {
                        System.Drawing.Font currentFont = richTextBox_Content.SelectionFont;
                        richTextBox_Content.SelectionFont = new System.Drawing.Font(selectedFont, currentFont.Size, currentFont.Style);
                    }
                }

                richTextBox_Content.Select(selectionStart, selectionLength);
            }
        }
        private void mainDoc_Load(object sender, EventArgs e)
        {
            // Điền danh sách các kiểu chữ vào ComboBox
            foreach (FontFamily font in FontFamily.Families)
            {
                comboBox_font.Items.Add(font.Name);
            }

            // Điền danh sách các cỡ chữ vào ComboBox (ví dụ từ 8 đến 72)
            for (int i = 8; i <= 72; i++)
            {
                comboBox_size.Items.Add(i);
            }

            // Đặt giá trị mặc định cho ComboBox
            comboBox_font.SelectedIndex = comboBox_font.FindString("Arial");
            comboBox_size.SelectedIndex = comboBox_size.FindString("12");
        }

        private void comboBox_size_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (richTextBox_Content.SelectionLength > 0)
            {
                int selectionStart = richTextBox_Content.SelectionStart;
                int selectionLength = richTextBox_Content.SelectionLength;
                string selectedFont = comboBox_font.SelectedItem.ToString();

                for (int i = 0; i < selectionLength; i++)
                {
                    richTextBox_Content.Select(selectionStart + i, 1);
                    if (richTextBox_Content.SelectionFont != null)
                    {
                        System.Drawing.Font currentFont = richTextBox_Content.SelectionFont;
                        float newSize = float.Parse(comboBox_size.SelectedItem.ToString());
                        richTextBox_Content.SelectionFont = new System.Drawing.Font(currentFont.FontFamily, newSize, currentFont.Style);
                    }
                }
                richTextBox_Content.Select(selectionStart, selectionLength);
            }
        }

        private void button_Italic_Click(object sender, EventArgs e)
        {
            if (richTextBox_Content.SelectionLength > 0)
            {
                int selectionStart = richTextBox_Content.SelectionStart;
                int selectionLength = richTextBox_Content.SelectionLength;
                string selectedFont = comboBox_font.SelectedItem.ToString();

                for (int i = 0; i < selectionLength; i++)
                {
                    richTextBox_Content.Select(selectionStart + i, 1);
                    if (richTextBox_Content.SelectionFont != null)
                    {
                        System.Drawing.Font currentFont = richTextBox_Content.SelectionFont;
                        System.Drawing.FontStyle newFontStyle = richTextBox_Content.SelectionFont.Style ^ System.Drawing.FontStyle.Italic;
                        richTextBox_Content.SelectionFont = new System.Drawing.Font(currentFont.FontFamily, currentFont.Size, newFontStyle);
                    }
                    richTextBox_Content.Select(selectionStart, selectionLength);
                }
            }
        }

        private void button_Underline_Click(object sender, EventArgs e)
        {
            if (richTextBox_Content.SelectionLength > 0)
            {
                int selectionStart = richTextBox_Content.SelectionStart;
                int selectionLength = richTextBox_Content.SelectionLength;
                string selectedFont = comboBox_font.SelectedItem.ToString();

                for (int i = 0; i < selectionLength; i++)
                {
                    richTextBox_Content.Select(selectionStart + i, 1);
                    if (richTextBox_Content.SelectionFont != null)
                    {
                        System.Drawing.Font currentFont = richTextBox_Content.SelectionFont;
                        System.Drawing.FontStyle newFontStyle = richTextBox_Content.SelectionFont.Style ^ System.Drawing.FontStyle.Underline;
                        richTextBox_Content.SelectionFont = new System.Drawing.Font(currentFont.FontFamily, currentFont.Size, newFontStyle);
                    }
                    richTextBox_Content.Select(selectionStart, selectionLength);
                }
            }
        }

        private void button_AlignLeft_Click(object sender, EventArgs e)
        {
            richTextBox_Content.SelectionAlignment = System.Windows.Forms.HorizontalAlignment.Left;
        }

        private void button_Center_Click(object sender, EventArgs e)
        {
            richTextBox_Content.SelectionAlignment = System.Windows.Forms.HorizontalAlignment.Center;
        }

        private void button_AlignRight_Click(object sender, EventArgs e)
        {
            richTextBox_Content.SelectionAlignment = System.Windows.Forms.HorizontalAlignment.Right;
        }

        private void button_Justify_Click(object sender, EventArgs e)
        {
            /*            // Lấy đoạn văn bản được bôi đen
                        string selectedText = richTextBox_Content.SelectedText;

                        // Tạo một document mới và thêm đoạn văn bản vào
                        var document = new DocumentModel();
                        var section = new Section(document);
                        document.Sections.Add(section);

                        var paragraph = new Paragraph(document, selectedText);
                        section.Blocks.Add(paragraph);

                        // Căn lề hai bên cho đoạn văn bản
                        paragraph.ParagraphFormat.Alignment = GemBox.Document.HorizontalAlignment.Justify;

                        // Xuất đoạn văn bản đã căn lề ra richTextBox
                        richTextBox_Content.SelectedText = paragraph.Content.ToString();*/
            // Lấy đoạn văn bản được bôi đen
            string selectedText = richTextBox_Content.SelectedText;

            // Tạo một document mới và thêm đoạn văn bản vào
            var document = DocX.Create("temp.docx");
            var paragraph = document.InsertParagraph(selectedText);

            // Căn lề hai bên cho đoạn văn bản
            paragraph.Alignment = Alignment.both;

            // Sử dụng StringBuilder để lấy nội dung đã căn lề
            StringBuilder sb = new StringBuilder();
            foreach (var p in document.Paragraphs)
            {
                sb.AppendLine(p.Text);
            }

            // Xuất đoạn văn bản đã căn lề ra richTextBox
            richTextBox_Content.SelectedText = sb.ToString();
        }
    }
}
