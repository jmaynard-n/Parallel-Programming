using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TcpClientServer
{
    public static class ClientLogic
    {
        private static TcpClient _client;
        private static NetworkStream _serverStream = default(NetworkStream);

        private static Thread _listenerThread;

        private static Boolean connection = true;
        public static void Run(string ipAddress, int port)
        {
            try
            {
                int myNum = new Random().Next(1000000);
                _client = new TcpClient();
                _client.Connect(ipAddress, port);

                byte[] data = GetMesageBytes($"init {myNum}");
                StringBuilder response = new StringBuilder();
                _serverStream = _client.GetStream();

                MyClient.Notify += SendMessage;
                GetSize();
                GetMatrix();

                NetworkStream stream = _client.GetStream();
                Console.WriteLine("Отправка сообщения инициализации");
                stream.Write(data, 0, data.Length);
                stream.Flush();
                Console.WriteLine("Сообщение инициализации отправлено");

                _listenerThread = new Thread(GetMessages);
                _listenerThread.Start();

                while (connection) { }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
            }

            Console.WriteLine("Запрос завершен...");
            Console.Read();
        }

        private static void GetSize()
        {
            var bufferSize = _client.ReceiveBufferSize;
            byte[] instream = new byte[bufferSize];

            _serverStream.Read(instream, 0, bufferSize);
            string returnData = Encoding.UTF8.GetString(instream);
            returnData = returnData.Trim((char)0);
            Console.WriteLine($"Matrix size {returnData}");
            MyClient.Size= Convert.ToInt32(returnData);
        }

        private static void GetMatrix()
        {
            var bufferSize = _client.ReceiveBufferSize;
            byte[] instream = new byte[bufferSize];

            _serverStream.Read(instream, 0, bufferSize);
            string returnData = Encoding.UTF8.GetString(instream);
            returnData = returnData.Trim((char)0);
            Console.WriteLine("Matrix received");

            MyClient.GetMatrix(returnData);
            Console.WriteLine("Matrix saved");
        }

        private static void GetMessages()
        {
            while (connection)
            {
                var bufferSize = _client.ReceiveBufferSize;
                byte[] instream = new byte[bufferSize];

                _serverStream.Read(instream, 0, bufferSize);
                string returnData = Encoding.UTF8.GetString(instream);
                returnData = returnData.Trim((char)0);
                Console.WriteLine($"Received problem: |{returnData}|");

                if (Convert.ToInt32(returnData) == -1)
                {
                    Console.WriteLine("!!");
                    connection = false;
                    Close();
                    //Thread.CurrentThread.ThreadState();
                }
                else
                    MyClient.CalcProblem(returnData);
            }
        }

        public static void Close()
        {
            Console.WriteLine("Завершение сеанса...");
            _client.Close();
            _serverStream.Close();
        }

        private static byte[] GetMesageBytes(string msg)
        {
            return Encoding.UTF8.GetBytes(msg);
        }

        private static void SendMessage(double value)
        {
            string msg = Convert.ToString(value);
            byte[] data = GetMesageBytes(msg);
            NetworkStream stream = _client.GetStream();
            //Console.WriteLine("Отправка сообщения");
            stream.Write(data, 0, data.Length);
            stream.Flush();
            Console.WriteLine("Сообщение отправлено");
        }
    }
}
