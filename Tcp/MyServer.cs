using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassMatrix;

namespace TcpClientServer
{
    public static class MyServer
    {
        private static Matrix matrix;
        private static Matrix invert;
        private static int size;

        public static void CreateProblem(int value)
        {
            size = value;
            matrix = new Matrix(size, size);
           // matrix.Print();
        }

        public static void Minor(int i, int j, double value)
        {
            matrix.minor[i, j] = value;
        }

        public static string GetMatrix()
        {
            string str;
            StringBuilder sb = new StringBuilder();

            for (var i = 0; i < size; i++)
            {
                for (var j = 0; j < size; j++)
                {
                    sb.Append(Convert.ToString(matrix[i, j]));
                    sb.Append(" ");
                }
            }
            str = sb.ToString();
            return str;
        }

        public static void InvertMatrix()
        {
            invert = matrix.CreateInvertibleMatrix();
            Matrix check = invert * matrix;
            Console.WriteLine("Current Matrix:");
            matrix.Print();
            Console.WriteLine("\nInvertible Matrix:");
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
