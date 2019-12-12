using System;
using System.Collections.Generic;
using System.Linq;

namespace Task4
{
    class Program
    {
        static void Main(string[] args)
        {
            List<int> numbers = new List<int>();

            PasswordNumber pwn = new PasswordNumber(125776);

            while (true)
            {
                int value = pwn.GetNext();

                if (value >= 579381)
                    break;

                numbers.Add(value);
            }

            Console.WriteLine(numbers.Count);
        }
    }

    public class Digit
    {
        public Digit Previous { get; }
        public Digit Next { get; }

        public Digit Last => Next?.Last ?? this; 

        public Digit(Digit previous, int number, int count)
        {
            Previous = previous;
            Number = number/(int)Math.Pow(10,count);
            if (count > 0)
            {
                Next = new Digit(this, number-Number*(int)Math.Pow(10, count), --count);
            }
        }

        public int Number { get; set; }

        public void AddOne()
        {
            Number++;

            if (Number == 10)
            {
                Previous.AddOne();
                Number = Previous.Number;
            }
        }

        public int GetNumber(int count)
        {
            return Number * (int)Math.Pow(10, count) + (Next?.GetNumber(count - 1) ?? 0);
        }

        public void AddAll(List<int> numbers)
        {
            numbers.Add(Number);
            Next?.AddAll(numbers);
        }

        public bool ContainsTwoInRow()
        {
            var success = Number == Next?.Number;

            return success || (Next?.ContainsTwoInRow() ?? false);
        }
    }

    public class PasswordNumber
    {
        private Digit First { get; }

        public PasswordNumber(int minimumNumber)
        {
            First = new Digit(null, minimumNumber, (int)Math.Log10(minimumNumber));
        }

        private void AddOne()
        {
            First.Last.AddOne();
        }

        private Digit[] GetAll()
        {
            List<Digit> digits = new List<Digit>();

            var current = First;

            do
            {
                digits.Add(current);
                current = current.Next;
            } while (current != null);

            return digits.ToArray();
        }

        public int GetNext()
        {
            AddOne();

            while (GetAll().GroupBy(x => x.Number).All(y => y.Count() != 2))
            {
                AddOne();
            }

            return First.GetNumber(5);
        }
    }
}
