using System.Collections.ObjectModel;
using System.Linq;
using Task2And5And7And9;

namespace Task2And5And7
{
    public class ChainProgram
    {
        private readonly ReadOnlyCollection<long[]> programs;
        private readonly long[] _phaseSettings;

        public ChainProgram(long[] program, long[] phaseSettings)
        {
            var individualPrograms = phaseSettings.Select(x => program.ToArray()).ToList();
            programs = new ReadOnlyCollection<long[]>(individualPrograms);
            _phaseSettings = phaseSettings;
        }

        public long RunPrograms(long firstInput, bool feedback)
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
}