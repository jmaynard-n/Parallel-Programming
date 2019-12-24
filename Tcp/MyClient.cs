using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassMatrix;

namespace TcpClientServer
{
    public static class MyClient
    {
        private static Matrix matrix;
        private static int size;

        public delegate void MinorHandler(double value);
        public static event MinorHandler Notify;

        public static int Size
        { set => size = value; }

        public static void GetMatrix(string str)
        {
            matrix = new Matrix(size, size);

            int num;
            var i = 0;
            var j = 0;

            string[] split = str.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in split)
            {
                num = Convert.ToInt32(s);
                //Console.WriteLine(num + " " + i + " " + j);
                matrix[i, j] = num;
                j++;
                if (j == size)
                {
                    j = 0;
                    i++;
                }
            }
            //matrix.Print();
        }

        public static void CalcProblem(string val)
        {
            double result = matrix.Minor(Convert.ToInt32(val));
            //Console.WriteLine($"calculated :: {result}");
            Notify?.Invoke(result);
        }
    }
}
