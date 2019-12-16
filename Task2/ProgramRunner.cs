using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using Task2And5And7;

namespace Task2And5And7And9
{
    public class ProgramList
    {
        private long[] _internList;
        public int Length { get; private set; }

        public ProgramList()
        {
            _internList = new long[10];
            Length = 0;
        }

        public ref long this[int address] => ref _internList[address];

        public int AddElement(long element)
        {
            if (Length == _internList.Length)
            {
                DoubleSize();
            }

            _internList[Length] = element;
            return Length++;
        }

        public void DoubleSize()
        {
            var temp = new long[_internList.Length * 2];
            _internList.CopyTo(temp, 0);
            _internList = temp;
        }

    }

    public class ProgramMemory
    {
        public static ProgramMemory CreateInstance(IEnumerable<long> initialProgram)
        {
            return new ProgramMemory(initialProgram);
        }

        private readonly Dictionary<long, int> _lookup;

        private readonly ProgramList program;

        public ref long this[long address]
        {
            get
            {
                if (!_lookup.ContainsKey(address))
                {
                    _lookup[address] = program.AddElement(0);
                }

                return ref program[_lookup[address]];
            }
        }

        public ProgramMemory()
        {
            program = new ProgramList();
            _lookup = new Dictionary<long, int>();
        }

        private ProgramMemory(IEnumerable<long> initialProgram) : this()
        {
            long indexer = 0;
            foreach (var value in initialProgram)
            {
                this[indexer++] = value;
            }
        }
    }

    public class ProgramRunner
    {
        public Queue<long> Input { get; }
        private long _currentPosition;
        private long _relativePosition = 0;

        public ProgramMemory Array { get; }

        public List<long> Output { get; }
        public event Action<long> ItemAddedToOutput;

        public ProgramRunner(long[] array, long input) : this(array, new Queue<long>(new[] { input }))
        {
        }

        public ProgramRunner(long[] array, Queue<long> input)
        {
            Input = input;
            Array = ProgramMemory.CreateInstance(array);
            Output = new List<long>();
            _currentPosition = 0;
        }

        public ProgramRunner(long[] array) : this(array, new Queue<long>())
        {
        }

        public static Instruction GetInstruction(long num)
        {
            List<long> listOfInts = new List<long>();
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
                    GenericOpcodeOperator((x, y) => x + y, _currentPosition, instruction);
                    _currentPosition += 4;
                    break;
                case 2:
                    GenericOpcodeOperator((x, y) => x * y, _currentPosition, instruction);
                    _currentPosition += 4;
                    break;
                case 3:
                    if (Input.Count == 0)
                        return true;
                    GetValue(++_currentPosition, instruction.FirstParameter) = Input.Dequeue();
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
                case 9:
                    _relativePosition += GetValue(++_currentPosition, instruction.FirstParameter);
                    ++_currentPosition;
                    break;
                case 99:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return true;
        }

        private void Compare(Func<long, long, bool> compareMethod, long currentPosition, Instruction instruction)
        {
            var firstParameter = GetValue(++currentPosition, instruction.FirstParameter);
            var secondParameter = GetValue(++currentPosition, instruction.SecondParameter);
            GetValue(++currentPosition, instruction.ThirdParameter) = compareMethod(firstParameter, secondParameter) ? 1 : 0;
        }

        private void Jump(ref long currentPosition, bool ifNonZero, Instruction instruction)
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

        public void GenericOpcodeOperator(Func<long, long, long> function, long startPosition, Instruction instruction)
        {
            GetValue(startPosition + 3, instruction.ThirdParameter) = function(GetValue(startPosition + 1, instruction.FirstParameter), GetValue(startPosition + 2, instruction.SecondParameter));
        }

        public ref long GetValue(long position, ParameterMode mode)
        {
            switch (mode)
            {
                case ParameterMode.Position:
                    return ref Array[Array[position]];
                case ParameterMode.Immediate:
                    return ref Array[position];
                case ParameterMode.Relative:
                    return ref Array[Array[position] + _relativePosition];
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }
    }
}