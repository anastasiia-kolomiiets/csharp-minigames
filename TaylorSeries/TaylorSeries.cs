using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace TaylorSeries
{
    public static class TaylorSeries<T>
    {
        private static dynamic Factorial(int n)
        {
            dynamic result = 1.0;
            for (int i = 2; i <= n; i++)
                result *= i;
            return result;
        }

        public static T Exp(T x, int terms = 20)
        {
            dynamic dx = x;
            dynamic sum = One(dx);
            dynamic power = One(dx);

            for (int n = 1; n < terms; n++)
            {
                power *= dx;
                sum += power / Factorial(n);
            }

            return sum;
        }

        public static T Sin(T x, int terms = 20)
        {
            dynamic dx = x;
            dynamic sum = Zero(dx);
            dynamic power = dx;
            dynamic sign = 1.0;

            for (int n = 1; n <= terms; n++)
            {
                int k = 2 * n - 1;
                sum += sign * power / Factorial(k);
                sign = -sign;
                power *= dx * dx;
            }

            return sum;
        }

        public static T Cos(T x, int terms = 20)
        {
            dynamic dx = x;
            dynamic sum = One(dx);
            dynamic power = One(dx);
            dynamic sign = 1.0;

            for (int n = 1; n < terms; n++)
            {
                int k = 2 * n;
                power *= dx * dx;
                sign = -sign;
                sum += sign * power / Factorial(k);
            }

            return sum;
        }

        private static dynamic One(dynamic x)
        {
            if (x is Matrix m)
                return Matrix.Identity(m.Rows);
            return 1.0;
        }

        private static dynamic Zero(dynamic x)
        {
            if (x is Matrix m)
                return new Matrix(new double[m.Rows, m.Cols]);
            return 0.0;
        }
    }
}
