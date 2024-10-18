using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Server
{
    public partial class Server : Form
    {
        public Server()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {   
            var ipEndPoint = new IPEndPoint(IPAddress.Any, 8000);
            TcpListener listener = new(ipEndPoint);

            try
            {
                listener.Start();

                using TcpClient handler = await listener.AcceptTcpClientAsync();
                await using NetworkStream stream = handler.GetStream();

                var message = $"📅 {DateTime.Now} 🕛";
                var dateTimeBytes = Encoding.UTF8.GetBytes(message);
                await stream.WriteAsync(dateTimeBytes);

                
            }
            finally
            {
                listener.Stop();
            }
        }
    }
}
