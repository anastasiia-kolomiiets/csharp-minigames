using System.Numerics;
namespace TaylorSeries
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Complex ===");
            Complex z = new Complex(1, 1);
            Console.WriteLine("exp(z) = " + TaylorSeries<Complex>.Exp(z));
            Console.WriteLine("sin(z) = " + TaylorSeries<Complex>.Sin(z));
            Console.WriteLine("cos(z) = " + TaylorSeries<Complex>.Cos(z));

            Console.WriteLine("\n=== Matrix ===");
            var A = new Matrix(new double[,] { { 1, 0 }, { 0, 1 } });
            var expA = TaylorSeries<Matrix>.Exp(A, 15);
            Console.WriteLine("exp(A) =");
            Console.WriteLine(expA);
        }
    }
}
