using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ClassMatrix;

namespace TcpClientServer
{
    public static class ServerLogic
    {
        static List<ServerConnectionHandler> _clients = new List<ServerConnectionHandler>();
        static TcpListener _server = null;

        private static Thread _listenerThread;
        static readonly object ClientLockObject = new object();
        static readonly object IteratorObject = new object();

        private static int size;
        private static int i = 0;
        private static int j = 0;
        private static int problem = 0;
        public static void Run(string ipAddress, int port, int matrix_size)
        {
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");
            _server = new TcpListener(localAddr, port);
            size = matrix_size;

            MyServer.CreateProblem(size);

            // запуск слушателя
            _server.Start();
            _listenerThread = new Thread(ConnectionRegistrator);
            _listenerThread.Start();

            //while (true)
            //{
            //    string msg = Console.ReadLine();
            //    if (msg==null)
            //        continue;
            //    int num = 0;
            //    lock (ClientLockObject)
            //    {
            //        foreach (var client in _clients)
            //        {
            //            client.SendMessage($"{num++} - {msg}");
            //        }
            //    }
            //}
        }

        public static void Solving()
        {
            lock (IteratorObject)
            {
                problem++;
                if (problem == size * size)
                    MyServer.InvertMatrix();
            }
        }

        public static int Iterator()
        {
            int value;

            lock (IteratorObject)
            {
                if (i >= size)
                    return -1;
                Console.WriteLine("Start [{0}, {1}] ", i, j);
                value = i * 100 + j;
                j++;
                if (j == size)
                {
                    i++;
                    j = 0;
                }
            }

            return (value);
        }

        public static void ConnectionRegistrator()
        {
            while (true)
            {
                Console.WriteLine("Ожидание подключений... ");

                // получаем входящее подключение
                TcpClient client = _server.AcceptTcpClient();
                Console.WriteLine("Подключен клиент.");

                ServerConnectionHandler handler;
                lock (ClientLockObject)
                {
                    handler = new ServerConnectionHandler(client);
                    _clients.Add(handler);
                }

                handler.SendMessage(Convert.ToString(size));
                handler.SendMessage(MyServer.GetMatrix());
                //handler.SendMessage(Iterator());

                /*
                // закрываем поток
                stream.Close();
                // закрываем подключение
                client.Close();*/
            }
        }
    }
}
