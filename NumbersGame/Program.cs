using System.Text;

namespace NumbersGame
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.Title = "Бики і Корови — проти комп'ютера";

            do
            {
                new GameEngine().Play();
                Console.WriteLine("\nГрати ще раз? (y — так, будь-яка інша — ні)");
            }
            while (Console.ReadKey(true).Key == ConsoleKey.Y);

            Console.WriteLine("\nДякую за гру!");
        }
    }
}
