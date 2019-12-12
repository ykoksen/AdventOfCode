using System;
using System.Linq;

namespace Task1
{
    class Program
    {
        

        static void Main(string[] args)
        {
            Console.WriteLine(Constants.input.Select(x => GetFuelAdv(x)).Sum());
        }

        public static int GetFuelSimple(int input)
        {
            var result = input / 3 - 2;
            return Math.Max(0, result);
        }

        public static int GetFuelAdv(int input)
        {
            int temp = GetFuelSimple(input);
            if (temp == 0)
                return 0;

            return temp + GetFuelAdv(temp);
        }
    }
}
