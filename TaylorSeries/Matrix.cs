using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaylorSeries
{
    public class Matrix
    {
        public double[,] Data;
        public int Rows => Data.GetLength(0);
        public int Cols => Data.GetLength(1);

        public Matrix(double[,] data)
        {
            Data = data;
        }

        public static Matrix operator +(Matrix a, Matrix b)
        {
            var res = new double[a.Rows, a.Cols];
            for (int i = 0; i < a.Rows; i++)
                for (int j = 0; j < a.Cols; j++)
                    res[i, j] = a.Data[i, j] + b.Data[i, j];
            return new Matrix(res);
        }

        public static Matrix operator *(Matrix a, Matrix b)
        {
            var res = new double[a.Rows, b.Cols];
            for (int i = 0; i < a.Rows; i++)
                for (int j = 0; j < b.Cols; j++)
                    for (int k = 0; k < a.Cols; k++)
                        res[i, j] += a.Data[i, k] * b.Data[k, j];
            return new Matrix(res);
        }

        public static Matrix operator *(double scalar, Matrix a)
        {
            var res = new double[a.Rows, a.Cols];
            for (int i = 0; i < a.Rows; i++)
                for (int j = 0; j < a.Cols; j++)
                    res[i, j] = scalar * a.Data[i, j];
            return new Matrix(res);
        }

        // ✅ новий оператор ділення матриці на скаляр
        public static Matrix operator /(Matrix a, double scalar)
        {
            var res = new double[a.Rows, a.Cols];
            for (int i = 0; i < a.Rows; i++)
                for (int j = 0; j < a.Cols; j++)
                    res[i, j] = a.Data[i, j] / scalar;
            return new Matrix(res);
        }

        public static Matrix Identity(int size)
        {
            var data = new double[size, size];
            for (int i = 0; i < size; i++)
                data[i, i] = 1;
            return new Matrix(data);
        }

        public override string ToString()
        {
            string s = "";
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                    s += $"{Data[i, j],8:F3}";
                s += "\n";
            }
            return s;
        }
    }
}
