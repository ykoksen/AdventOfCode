using System;
using System.Collections.Generic;
using System.Linq;
using Task2And5And7;

namespace Task2And5And7And9
{
    class Program
    {
        static void Main(string[] args)
        {
            //Task5();

            //Task2();

            //Task7();

            //Task7Test();

            //Task7_2();

            Task9_1();
        }

        private static void Task9_1()
        {
            ProgramRunner runner = new ProgramRunner(Inputs.Task91, 2);
            runner.RunProgram();
        }

        public static void Task9Test()
        {
            ProgramRunner runner = new ProgramRunner(Inputs.Task9Test);
            runner.RunProgram();
        }

        public static void Task7Test()
        {
            ChainProgram program = new ChainProgram(Inputs.Task7Test, new long[]{ 4, 3, 2, 1, 0 });
            var output = program.RunPrograms(0, false);
            Console.WriteLine("Task 7 test: " + output);
        }

        private static void Task7()
        {
            int machineNumber = 5;

            List<long> allPhaseSettings = new List<long>();

            for (int i = 0; i < machineNumber; i++)
            {
                allPhaseSettings.Add(i);
            }

            var permutations = PermutationHelper.GetPermutations(allPhaseSettings, machineNumber);

            long maxValue = -1;
            long[] usedArray = { 0 };
            foreach (var setting in permutations)
            {
                var phaseSettings = setting.ToArray();
                ChainProgram program = new ChainProgram(Inputs.Task7List.ToArray(), phaseSettings);
                var output = program.RunPrograms(0, false);
                if (output > maxValue)
                {
                    maxValue = output;
                    usedArray = phaseSettings;
                }
            }

            Console.WriteLine("Task 7 output: " + maxValue);
        }

        private static void Task7_2()
        {
            int machineNumber = 5;

            List<long> allPhaseSettings = new List<long>();

            for (int i = 0; i < machineNumber; i++)
            {
                allPhaseSettings.Add(5 + i);
            }

            var permutations = PermutationHelper.GetPermutations(allPhaseSettings, machineNumber);

            long maxValue = -1;
            foreach (var setting in permutations)
            {
                var phaseSettings = setting.ToArray();
                ChainProgram program = new ChainProgram(Inputs.Task7List.ToArray(), phaseSettings);
                var output = program.RunPrograms(0, true);
                if (output > maxValue)
                {
                    maxValue = output;
                }
            }

            Console.WriteLine("Task 7.2 output: " + maxValue);
        }

        private class PhaseNumber
        {
            public int Number;
            private readonly PhaseNumber? parent;

            public PhaseNumber(int number, PhaseNumber? parent)
            {
                Number = number;
                this.parent = parent;
            }

            public bool AddOne()
            {
                var temp = Number;
                if (temp != 4)
                {
                    Number++;
                }
                else
                {
                    var canDoIt = parent?.AddOne() ?? false;
                    if (!canDoIt)
                        return false;
                    else
                    {
                        Number = 0;
                    }
                }

                return true;
            }

            public void GetPhaseSetting(int[] array, int index)
            {
                array[index] = Number;
                parent?.GetPhaseSetting(array, ++index);
            }
        }

        private static void Task5()
        {
            int[] task52Test = { 3, 3, 1105, -1, 9, 1101, 0, 0, 12, 4, 12, 99, 1 };

            int input2 = 5;
            List<int> output = new List<int>();


            ProgramRunner runner = new ProgramRunner(Inputs.Task5List, input2);
            runner.RunProgram();

            Console.WriteLine("Task 5 output: " + runner.Output.Last());
        }

        private static void Task2()
        {
            List<long> list = new List<long>(Inputs.Task2List);

            for (int noun = 0; noun < 100; noun++)
            {
                for (int verb = 0; verb < 100; verb++)
                {
                    var input = list.ToArray();
                    input[1] = noun;
                    input[2] = verb;

                    ProgramRunner runner = new ProgramRunner(input, 45);

                    runner.RunProgram();
                    if (input[0] == 19690720)
                    {
                        Console.WriteLine("Task 2 output: " + (100 * input[1] + input[2]));
                        return;
                    }
                }
            }
        }
    }
}
