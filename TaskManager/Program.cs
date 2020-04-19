using System;
using ClassMatrix;
using System.Threading;
using System.Threading.Tasks;
using TaskManagerRefact.Core;

namespace Threads
{
    class Program
    {
        static Matrix matrix;
        static Matrix invert;

        static void Main(string[] args)
        {
            int matrix_size = GetSize();
            int threads = GetThreads();
            matrix = new Matrix(matrix_size, matrix_size);

            var threadManager = new TaskManager(threads);
            threadManager.TaskManagerWorkDone += WorkDone;

            for (var i = 0; i < matrix_size; i++)
            {
                for (var j = 0; j < matrix_size; j++)
                {
                    int value = (i) * 100 + j;
                    threadManager.AddTask(() => matrix.Minor(value));
                }
            }
            Console.WriteLine("Ожидание завершения");
            Console.ReadKey();
        }
        
        public static int GetSize()
        {
            int value;

            Console.WriteLine("Enter the size of the matrix:");
            Console.WriteLine("\tNote: size must be in the interval [1; 100]\n" +
                "\tIf not, it will automaticlly set to the nearest possible value");
            Console.Write("size = ");

            value = Convert.ToInt32(Console.ReadLine());

            if (value <= 0)
                value = 32;
            else if (value > 100)
                value = 100;

            return value;
        }

        public static int GetThreads()
        {
            int size = 0;

            Console.WriteLine("Введите количество потоков.\n Примечание: введенное число должно быть в пределах от 0 до 15");
            size = Convert.ToInt32(Console.ReadLine());
            while (size <= 0 || size > 30)
            {
                Console.WriteLine("Введенное число не попадает в интервал. Попробуйте еще раз");
                size = Convert.ToInt32(Console.ReadLine());
            }
            return size;
        }

        public static void WorkDone()
        {
            Console.WriteLine("Менеджер завершил все задачи.");
            InvertMatrix();
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
    }
}
