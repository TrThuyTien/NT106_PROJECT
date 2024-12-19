using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace LoadBalance1
{
    public partial class LoadBalance : Form
    {
        private List<ServerInfo> servers;
        private TcpListener listener;
        private int loadBalancerPort = 8000;

        public LoadBalance()
        {
            servers = new List<ServerInfo>
            {
                new ServerInfo("127.0.0.1", 8080),
                new ServerInfo("127.0.0.1", 8081),
                new ServerInfo("127.0.0.1", 8082)
            };
            InitializeComponent();
        }

        private void StartLoadBalancer()
        {
            listener = new TcpListener(IPAddress.Any, loadBalancerPort);
            listener.Start();
            Thread acceptThread = new Thread(AcceptClients);
            acceptThread.Start();
        }
     
        private void AcceptClients()
        {

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                Thread clientThread = new Thread(() => HandleClient(client));
                clientThread.Start();
            }

        }

        private void HandleClient(TcpClient client)
        {
            ServerInfo server = GetServerWithLeastConnections();
            server.IncrementConnection(); // Tăng số kết nối khi có kết nối mới
            UpdateServerInfo();
            TcpClient serverClient = new TcpClient(server.IP, server.Port);
            NetworkStream clientStream = client.GetStream();
            NetworkStream serverStream = serverClient.GetStream();

            Thread clientToServer = new Thread(() => ForwardData(clientStream, serverStream, server));
            Thread serverToClient = new Thread(() => ForwardData(serverStream, clientStream, server));

            clientToServer.Start();
            serverToClient.Start();

            clientToServer.Join();
            server.DecrementConnection();
            UpdateServerInfo();
            serverToClient.Join();

            client.Close();
            serverClient.Close();


        }

        private ServerInfo GetServerWithLeastConnections()
        {
            servers.Sort((s1, s2) => s1.ConnectionCount.CompareTo(s2.ConnectionCount));
            return servers[0];
        }

        private void ForwardData(NetworkStream inputStream, NetworkStream outputStream, ServerInfo server)
        {
            byte[] buffer = new byte[1024];
            int bytesRead;
            try
            {
                while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    outputStream.Write(buffer, 0, bytesRead);
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                Console.WriteLine("Error in ForwardData: " + ex.Message);
            }
        }

        private void UpdateServerInfo()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(UpdateServerInfo));
                return;
            }

            richTextBox1.Clear();
            var serversCopy = servers.ToList();
            foreach (var server in serversCopy)
            {
                richTextBox1.AppendText($"Server {server.IP}:{server.Port} - Connections: {server.ConnectionCount}\n");
            }
        }

        public class ServerInfo
        {
            public string IP { get; }
            public int Port { get; }
            private int connectionCount;  // Thay đổi từ ConnectionCount công khai thành private
            public int ConnectionCount => connectionCount;  // Thêm thuộc tính chỉ đọc để truy cập connectionCount

            public ServerInfo(string ip, int port)
            {
                IP = ip;
                Port = port;
                connectionCount = 0;
            }

            public void IncrementConnection()
            {
                Interlocked.Increment(ref connectionCount);
            }

            public void DecrementConnection()
            {
                Interlocked.Decrement(ref connectionCount);
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "Load Balance đang chạy trên cổng: 8000";
            StartLoadBalancer();
        }
    }
}
