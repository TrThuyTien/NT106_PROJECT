using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
namespace SoanThaoOnline
{
    public partial class App : Form
    {
        public App()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var ipEndPoint = new IPEndPoint(IPAddress.Parse("120.0.0.1"), 8000);

            using TcpClient client = new();
            await client.ConnectAsync(ipEndPoint);
            await using NetworkStream stream = client.GetStream();

            var buffer = new byte[1_024];
            int received = await stream.ReadAsync(buffer);

            var message = Encoding.UTF8.GetString(buffer, 0, received);
            richTextBox1.Text = message;
        }
    }
}
