using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TcpClientServer
{
    public class ServerConnectionHandler
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private Thread _listenerThread;
        private Boolean connection = true;
        private int i;
        private int j;

        public ServerConnectionHandler(TcpClient tcpClient)
        {
            _client = tcpClient;
            _stream = tcpClient.GetStream();
            _listenerThread = new Thread(GetMessages);
            _listenerThread.Start();
        }

        public int I
        {
            get => i;
            set => i = value;
        }

        public int J
        {
            get => j;
            set => j = value;
        }

        public void GetMessages()
        {
            while (connection)
            {
                //Console.WriteLine("Виток ServerConnectionHandler-GetMessages");

                var bufferSize = _client.ReceiveBufferSize;
                byte[] instream = new byte[bufferSize];
                _stream.Read(instream, 0, bufferSize);
                string returnData = Encoding.UTF8.GetString(instream);
                returnData = returnData.Trim((char)0);
                Console.WriteLine($"Received message: \"{returnData}\"");

                if (! returnData.Contains("init"))
                {
                    MyServer.Minor(I, J, Convert.ToDouble(returnData));
                    ServerLogic.Solving();
                }

                int val = ServerLogic.Iterator();
                if (val < 0)
                {
                    this.SendMessage(Convert.ToString(val));
                    this.Close();
                    connection = false;
                    break;
                }
                else
                {
                    this.I = val / 100;
                    this.J = val % 100;
                    this.SendMessage(Convert.ToString(val));
                }   
            }
        }

        public void SendMessage(string msg)
        {
            byte[] data = Encoding.UTF8.GetBytes(msg);
            _stream.Write(data, 0, data.Length);
            _stream.Flush();
        }

        public void Close()
        {
            _stream.Close();
            _client.Close();
        }

    }
}
