using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TcpClientServer
{
    class Program
    {
        static int matrix_size;
        static int clients;
        static void Main(string[] args)
        {
            string ipAddress = "127.0.0.1";
            int port = 11111;
            if (args.Length == 0)
            {
                matrix_size = GetSize();
                clients = GetClients();
                Thread thread = new Thread(StartCients);
                thread.Start();
                ServerLogic.Run(ipAddress, port, matrix_size);
            }
            else
            {
                Console.WriteLine("Клиент инициализирован!");
                Thread.Sleep(500);
                Console.WriteLine("Начало логической части клиента!");
                ClientLogic.Run(ipAddress, port);
            }

        }

        static int GetSize()
        {
            int size = 0;

            Console.WriteLine("Введите размерность матрицы.\n Примечание: введенное число должно быть в пределах от 0 до 100");
            size = Convert.ToInt32(Console.ReadLine());
            while (size <= 0 || size > 100)
            {
                Console.WriteLine("Введенное число не оппадает в интервал. Попробуйте еще раз");
                size = Convert.ToInt32(Console.ReadLine());
            }
            return size;
        }

        static int GetClients()
        {
            int size = 0;

            Console.WriteLine("Введите количество подключаемых клиентов.\n Примечание: введенное число должно быть в пределах от 0 до 15");
            size = Convert.ToInt32(Console.ReadLine());
            while (size <= 0 || size > 15)
            {
                Console.WriteLine("Введенное число не оппадает в интервал. Попробуйте еще раз");
                size = Convert.ToInt32(Console.ReadLine());
            }
            return size;
        }
        static void StartCients()
        {
            Thread.Sleep(1000);
            Console.WriteLine("Начало запуска дополнительных экзмпляров!");
            for (int i = 0; i < clients; i++)
            {
                Process process = new Process();
                process.StartInfo.FileName = Process.GetCurrentProcess().MainModule.ModuleName;
                process.StartInfo.Arguments = "first second third 1 2 3";
                process.StartInfo.CreateNoWindow = false;
                process.StartInfo.UseShellExecute = true;

                process.EnableRaisingEvents = true;


                //process.StartInfo.RedirectStandardOutput = true;
                //process.OutputDataReceived += ProcessOnOutputDataReceived;

                process.Start();
            }
        }
    }
}
