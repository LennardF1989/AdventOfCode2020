using System.IO;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode2020.Days
{
    public static class Day25
    {
        public static void StartA()
        {
            //var lines = File.ReadAllLines("Content\\Day25_Test.txt");
            var lines = File.ReadAllLines("Content\\Day25.txt");

            var publicKeys = lines.Select(int.Parse).ToList();

            long subjectNumberStart = 7;
            long subjectNumber = 1;

            int loops = 0;

            int loopSize1 = 0;
            int loopSize2 = 0;

            while(true)
            {
                subjectNumber = (subjectNumberStart * subjectNumber) % 20201227;

                loops++;

                if (subjectNumber == publicKeys[0])
                {
                    loopSize1 = loops;
                }
                else if (subjectNumber == publicKeys[1])
                {
                    loopSize2 = loops;
                }

                if (loopSize1 > 0 && loopSize2 > 0)
                {
                    break;
                }
            }

            subjectNumberStart = publicKeys[0];
            long encryptionKey1 = 1;

            for(var i = 0; i < loopSize2; i++)
            {
                encryptionKey1 = (subjectNumberStart * encryptionKey1) % 20201227;
            }

            subjectNumberStart = publicKeys[1];
            long encryptionKey2 = 1;
            for(var i = 0; i < loopSize1; i++)
            {
                encryptionKey2 = (subjectNumberStart * encryptionKey2) % 20201227;
            }

            if (encryptionKey1 == encryptionKey2)
            {
                Logger.Info($"Day 25A: {encryptionKey1}");
            }
            else
            {
                Logger.Debug("Encryption incorrect!");
            }
        }
    }
}
