using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using ClassMatrix;

namespace Processes
{
    class Program
    {
        static Matrix matrix;
        static Matrix invert;
        private static int finishedProcess = 0;
        private static int problemSize;
        private static Process[,] saved;
        private static int i = 0;
        private static int j = 0;
        private static object LockObject = new object();

        static int Main(string[] args)
        {
            if (args.Length == 0)
                return MainProcess();
            else
                return SubProcess(args);
        }

        static int MainProcess()
        {
            int matrix_size = GetSize();
            int process_size = GetProcesses();
            matrix = new Matrix(matrix_size, matrix_size);
            problemSize = matrix_size * matrix_size;
            saved = new Process[matrix_size, matrix_size];

            matrix.ToFile();

            for (var k = 0; k < process_size; k++)
                NewTask();

            Console.WriteLine("This is Main Process!");
            Console.WriteLine($"File: {Process.GetCurrentProcess().MainModule.ModuleName}");

            Console.ReadKey();
            return 0;
        }

        private static void NewTask()
        {
            lock (saved)
            {
                if (i >= matrix.M)
                    return;
                Console.WriteLine("Start [{0}, {1}] ", i, j);
                //int value = i * 100 + j;
                //Process process = new Process();

                //process.StartInfo.FileName = Process.GetCurrentProcess().MainModule.ModuleName;
                //process.StartInfo.Arguments = Convert.ToString(value);
                //process.StartInfo.CreateNoWindow = true;
                //process.StartInfo.UseShellExecute = false;

                //process.EnableRaisingEvents = true;
                //process.Exited += ProcessOnExited;

                //saved[i, j] = process;
                //saved[i, j].Start();

                ProcessWrap pr = new ProcessWrap(i, j);
                pr.Start();

                j++;
                if (j == matrix.N)
                {
                    i++;
                    j = 0;
                }
            }
        }

        //private static void ProcessOnExited(object sender, EventArgs e)
        private static void ProcessOnExited(int ret, ProcessWrap pr)
        {
            finishedProcess++;
            Console.WriteLine("Process {0} finised!", finishedProcess);
            try
            {
                matrix.minor[pr.I, pr.J] = ret;
            }
            finally
            {
                pr.End();
            }
            if (finishedProcess == problemSize)
            {
                //for (var i = 0; i < matrix.N; i++)
                //{
                //    for (var j = 0; j < matrix.N; j++)
                //    {
                //        try
                //        {
                //            int ret = saved[i, j].ExitCode;
                //            matrix.minor[i, j] = ret;
                //        }
                //        finally
                //        {
                //            saved[i, j].Dispose();
                //        }
                //    }
                //}

                InvertMatrix();
            }
            else
                NewTask();
        }

        static int SubProcess(string[] args)
        {
            Matrix tempMatrix = new Matrix("Matrix.txt");
            int value = Convert.ToInt32(args[0]);
            Console.WriteLine(value);
            int res = (int)tempMatrix.Minor(value);
            //Thread.Sleep(5000);
            return res;
        }

        public static void InvertMatrix()
        {
            invert = matrix.CreateInvertibleMatrix();
            Matrix check = invert * matrix;
            Console.WriteLine("Current Matrix:");
            matrix.Print();
            Console.WriteLine("Invertible Matrix:");
            invert.Print();
            check.Print();

            var checkres = check.CheckIdentity();
            if (checkres != 1)
                Console.WriteLine("Oops! Something went wrong! A mistake has occured!");
            else
                Console.WriteLine("Invertible Matrix has been calculated successesfuly!");
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

        static int GetProcesses()
        {
            int size = 0;

            Console.WriteLine("Введите количество порождаемых процессов.\n Примечание: введенное число должно быть в пределах от 0 до 15");
            size = Convert.ToInt32(Console.ReadLine());
            while (size <= 0 || size > 15)
            {
                Console.WriteLine("Введенное число не оппадает в интервал. Попробуйте еще раз");
                size = Convert.ToInt32(Console.ReadLine());
            }
            return size;
        }
    }

    class ProcessWrap
    {
        private Process process;
        public delegate void ExitedHandler(int ret, ProcessWrap pr);
        public event ExitedHandler Notify;

        private int i;
        public int I 
        { 
            get { return i; } 
            set { i = value; } 
        }
        private int j;
        public int J 
        { 
            get { return j; } 
            set { j = value; }
        }

        public ProcessWrap(int i, int j)
        {
            process = new Process();

            int value = i * 100 + j;

            process.StartInfo.FileName = Process.GetCurrentProcess().MainModule.ModuleName;
            process.StartInfo.Arguments = Convert.ToString(value);
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;

            process.EnableRaisingEvents = true;
            process.Exited += ProcessExited;

            this.I = i;
            this.J = j;
        }

        private void ProcessExited(object sender, EventArgs e)
        {
            int ret = process.ExitCode;
            Notify?.Invoke(ret, this);
        }

        public void Start()
        {
            process.Start();
        }

        public void End()
        {
            process.Dispose();
        }
    }
}