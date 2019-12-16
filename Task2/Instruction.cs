using Task2And5And7;

namespace Task2And5And7And9
{
    public struct Instruction
    {
        public int OpCode { get; }

        public ParameterMode FirstParameter { get; }

        public ParameterMode SecondParameter { get; }

        public ParameterMode ThirdParameter { get; }

        public Instruction(long[] array)
        {
            OpCode = (int)array[0];
            FirstParameter = (ParameterMode) array[1];
            SecondParameter = (ParameterMode)array[2];
            ThirdParameter = (ParameterMode)array[3];
        }
    }
}