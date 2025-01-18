using Newtonsoft.Json;
using RestSharp;
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
    public partial class Translate : Form
    {
        public Translate()
        {
            InitializeComponent();

        }
        const string deepLApiKey = "a676bdf2-e6bd-43a9-a1f8-49ed4484df80:fx";
        public async Task<string> TranslateText(string text, string sourceLanguage, string targetLanguage)
        {
            try
            {
                var client = new RestClient("https://api-free.deepl.com/v2/translate");
                var request = new RestRequest
                {
                    Method = Method.Post
                };

                // Thêm các tham số bắt buộc
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddParameter("auth_key", deepLApiKey); // Thay bằng API Key của bạn
                request.AddParameter("text", text);           // Văn bản cần dịch
                request.AddParameter("source_lang", sourceLanguage.ToUpper()); // Ngôn ngữ nguồn
                request.AddParameter("target_lang", targetLanguage.ToUpper()); // Ngôn ngữ đích

                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful && response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    // Phân tích phản hồi JSON
                    var result = JsonConvert.DeserializeObject<dynamic>(response.Content);
                    return result.translations[0].text.ToString();
                }
                else
                {
                    throw new Exception($"Lỗi API: {response.Content}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Có lỗi xảy ra: {ex.Message}");
            }
        }
        public async Task<string> DetectLanguage(string text)
        {
            try
            {
                var client = new RestClient("https://api-free.deepl.com/v2/translate");
                var request = new RestRequest
                {
                    Method = Method.Post
                };

                // Thêm các tham số bắt buộc
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddParameter("auth_key", deepLApiKey); // API Key
                request.AddParameter("text", text);           // Văn bản cần nhận diện
                request.AddParameter("target_lang", "EN");   // Ngôn ngữ đích giả định

                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful && response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    // Phân tích phản hồi JSON
                    var result = JsonConvert.DeserializeObject<dynamic>(response.Content);
                    return result.translations[0].detected_source_language.ToString(); // Ngôn ngữ được nhận diện
                }
                else
                {
                    throw new Exception($"Lỗi API: {response.Content}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Có lỗi xảy ra: {ex.Message}");
            }
        }


        private async void button_translate_Click(object sender, EventArgs e)
        {
            try
            {
                // Lấy mã ngôn ngữ từ ComboBox
                string sourceLanguage = ((KeyValuePair<string, string>)comboBox_source.SelectedItem).Value;
                string targetLanguage = ((KeyValuePair<string, string>)comboBox_target.SelectedItem).Value;

                if (string.IsNullOrWhiteSpace(sourceLanguage) || string.IsNullOrWhiteSpace(targetLanguage))
                {
                    MessageBox.Show("Vui lòng chọn ngôn ngữ nguồn và ngôn ngữ đích!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Lấy văn bản từ richTextBox1
                string inputText = richTextBox_source.Text;
                if (string.IsNullOrWhiteSpace(inputText))
                {
                    MessageBox.Show("Vui lòng nhập văn bản cần dịch vào richTextBox1!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                string detectedLanguage = await DetectLanguage(inputText);

                if (detectedLanguage != sourceLanguage)
                {
                    MessageBox.Show($"Ngôn ngữ nhập vào không khớp với ngôn ngữ nguồn!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                // Gửi yêu cầu dịch
                string translatedText = await TranslateText(inputText, sourceLanguage, targetLanguage);

                if (!string.IsNullOrWhiteSpace(translatedText))
                {
                    // Hiển thị kết quả trong richTextBox2
                    richTextBox_target.Text = translatedText;
                    MessageBox.Show("Dịch thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Không nhận được kết quả dịch.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }
    }
}