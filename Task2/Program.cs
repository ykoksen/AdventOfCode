using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace Task2And5And7
{
    class Program
    {
        static void Main(string[] args)
        {
            //Task5();
            
            //Task2();

            //Task7();

            //Task7Test();

            Task7_2();
        }

        public static void Task7Test()
        {
            ChainProgram program = new ChainProgram(Inputs.Task7Test, new []{ 4, 3, 2, 1, 0 });
            var output = program.RunPrograms(0, false);
            Console.WriteLine("Task 7 test: "+ output);
        }

        private static void Task7()
        {
            int machineNumber = 5;

            List<int> allPhaseSettings = new List<int>();

            for (int i = 0; i < machineNumber; i++)
            {
                allPhaseSettings.Add(i);
            }

            var permutations = PermutationHelper.GetPermutations(allPhaseSettings, machineNumber);

            int maxValue = -1;
            int[] usedArray = new[] {0};
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

            List<int> allPhaseSettings = new List<int>();

            for (int i = 0; i < machineNumber; i++)
            {
                allPhaseSettings.Add(5+i);
            }

            var permutations = PermutationHelper.GetPermutations(allPhaseSettings, machineNumber);

            int maxValue = -1;
            int[] usedArray = new[] { 0 };
            foreach (var setting in permutations)
            {
                var phaseSettings = setting.ToArray();
                ChainProgram program = new ChainProgram(Inputs.Task7List.ToArray(), phaseSettings);
                var output = program.RunPrograms(0, true);
                if (output > maxValue)
                {
                    maxValue = output;
                    usedArray = phaseSettings;
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
            int[] task52Test = {3, 3, 1105, -1, 9, 1101, 0, 0, 12, 4, 12, 99, 1};

            int input2 = 5;
            List<int> output = new List<int>();


            ProgramRunner runner = new ProgramRunner(Inputs.Task5List, input2);
            runner.RunProgram();

            Console.WriteLine("Task 5 output: " + runner.Output.Last());
        }

        private static void Task2()
        {
            List<int> list = new List<int>(Inputs.Task2List);

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

    public class ChainProgram
    {
        private readonly ReadOnlyCollection<int[]> programs;
        private readonly int[] _phaseSettings;

        public ChainProgram(int[] program, int[] phaseSettings)
        {
            var individualPrograms = phaseSettings.Select(x => program.ToArray()).ToList();
            programs = new ReadOnlyCollection<int[]>(individualPrograms);
            _phaseSettings = phaseSettings;
        }

        public int RunPrograms(int firstInput, bool feedback)
        {
            ProgramRunner[] programRunners = new ProgramRunner[_phaseSettings.Length];
            for (var index = 0; index < _phaseSettings.Length; index++)
            {
                var phaseSetting = _phaseSettings[index];
                programRunners[index] = new ProgramRunner(programs[index], phaseSetting);
            }

            programRunners[0].Input.Enqueue(firstInput);

            for (int i = 1; i < programRunners.Length; i++)
            {
                var i2 = i;
                programRunners[i - 1].ItemAddedToOutput += i1 => programRunners[i2].Input.Enqueue(i1);
            }

            if (feedback)
                programRunners[^1].ItemAddedToOutput += i => programRunners[0].Input.Enqueue(i);

            bool stillRunning = true;
            while (stillRunning)
            {
                stillRunning = false;
                foreach (var programRunner in programRunners)
                {
                    stillRunning |= programRunner.RunSingleInstruction();
                }
            }

            return programRunners[^1].Output[^1];
        }
    }

    public class ProgramRunner
    {
        public Queue<int> Input { get; }
        private int _currentPosition;

        public int[] Array { get; }

        public List<int> Output { get; }
        public event Action<int> ItemAddedToOutput;

        public ProgramRunner(int[] array, int input) : this(array, new Queue<int>(new []{input}))
        {
        }

        public ProgramRunner(int[] array, Queue<int> input)
        {
            Input = input;
            Array = array;
            Output = new List<int>();
            _currentPosition = 0;
        }

        public ProgramRunner(int[] array)
        {
            Input = new Queue<int>();
            Array = array;
            Output = new List<int>();
            _currentPosition = 0;
        }

        public static Instruction GetInstruction(int num)
        {
            List<int> listOfInts = new List<int>();
            listOfInts.Add(num % 100);
            num /= 100;

            for (int i = 0; i < 4; i++)
            {
                listOfInts.Add(num % 10);
                num /= 10;
            }

            return new Instruction(listOfInts.ToArray());
        }

        public void RunProgram()
        {
            while (true)
            {
                if (!RunSingleInstruction()) return;
            }
        }

        public bool RunSingleInstruction()
        {
            Instruction instruction = GetInstruction(Array[_currentPosition]);

            switch (instruction.OpCode)
            {
                case 1:
                    GenericOpcodeOperator((x, y) => x + y, Array, _currentPosition, instruction);
                    _currentPosition += 4;
                    break;
                case 2:
                    GenericOpcodeOperator((x, y) => x * y, Array, _currentPosition, instruction);
                    _currentPosition += 4;
                    break;
                case 3:
                    if (Input.Count == 0)
                        return true;
                    Array[Array[++_currentPosition]] = Input.Dequeue();
                    _currentPosition++;
                    break;
                case 4:
                    var value = GetValue(++_currentPosition, instruction.FirstParameter);
                    Output.Add(value);
                    ItemAddedToOutput?.Invoke(value);
                    _currentPosition++;
                    break;
                case 5:
                    Jump(ref _currentPosition, true, instruction);
                    break;
                case 6:
                    Jump(ref _currentPosition, false, instruction);
                    break;
                case 7:
                    Compare((x, y) => x < y, _currentPosition, instruction);
                    _currentPosition += 4;
                    break;
                case 8:
                    Compare((x, y) => x == y, _currentPosition, instruction);
                    _currentPosition += 4;
                    break;
                case 99:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return true;
        }

        private void Compare(Func<int, int, bool> compareMethod, int currentPosition, Instruction instruction)
        {
            var firstParameter = GetValue(++currentPosition, instruction.FirstParameter);
            var secondParameter = GetValue(++currentPosition, instruction.SecondParameter);
            Array[Array[++currentPosition]] = compareMethod(firstParameter, secondParameter) ? 1 : 0;
        }

        private void Jump(ref int currentPosition, bool ifNonZero, Instruction instruction)
        {
            bool nonZero = GetValue(++currentPosition, instruction.FirstParameter) != 0;

            if (nonZero == ifNonZero)
            {
                currentPosition = GetValue(++currentPosition, instruction.SecondParameter);
            }
            else
            {
                currentPosition += 2;
            }
        }

        public void GenericOpcodeOperator(Func<int, int, int> function, int[] array, int startPosition, Instruction instruction)
        {
            Array[Array[startPosition + 3]] = function(GetValue(startPosition + 1, instruction.FirstParameter), GetValue(startPosition + 2, instruction.SecondParameter));
        }

        public int GetValue(int position, ParameterMode mode)
        {
            return mode switch
            {
                ParameterMode.Position => Array[Array[position]],
                ParameterMode.Immediate => Array[position],
                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
            };
        }
    }

    public enum ParameterMode
    {
        Position = 0,
        Immediate = 1
    }

    public struct Instruction
    {
        public int OpCode { get; }

        public ParameterMode FirstParameter { get; }

        public ParameterMode SecondParameter { get; }

        public ParameterMode ThirdParameter { get; }

        public Instruction(int[] array)
        {
            OpCode = array[0];
            FirstParameter = (ParameterMode) array[1];
            SecondParameter = (ParameterMode)array[2];
            ThirdParameter = (ParameterMode)array[3];
        }
    }
}
