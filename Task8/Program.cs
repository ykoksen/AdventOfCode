using System;
using System.Collections.Generic;
using System.Linq;

namespace Task8
{
    class Program
    {
        static void Main(string[] args)
        {
            TaskA();

            Console.WriteLine("Hello World!");
        }

        private static void TaskA()
        {
            CustomImage img = new CustomImage(6, 25);
            img.Parse(Parse(Input.InputTask1));

            ImageLayer fewestZeroes = new ImageLayer(img);
            int leastZero = int.MaxValue;

            foreach (var imageLayer in img.Layers)
            {
                int zeroes = imageLayer.Lines.Sum(x => x.Count(y => y == 0));
                if (zeroes < leastZero)
                {
                    leastZero = zeroes;
                    fewestZeroes = imageLayer;
                }
            }

            var flat = fewestZeroes.Lines.SelectMany(x => x).ToArray();
            var result = flat.Count(x => x == 1) * flat.Count(y => y == 2);

            Console.WriteLine($"Result of 8.1: " + result);

            var bitmap = img.FinalLayer().GetPicture();

            bitmap.Save("Test.bmp");
        }

        private static void TestIt()
        {
            CustomImage img = new CustomImage(2, 3);
            img.Parse(Parse(Input.TestInput));

        }

        public static List<int> Parse(string input)
        {
            return new List<int>(input.Select(x => int.Parse(x.ToString())));
        }
    }
}
