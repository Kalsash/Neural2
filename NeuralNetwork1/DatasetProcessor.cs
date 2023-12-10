using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace NeuralNetwork1
{
    using FastBitmap;
    public enum LetterType : byte { SH = 0, N, G, E, P, T, TS, Z, A, SOFT, Undef };
    public class DatasetProcessor
    {
        public static string LetterTypeToString(LetterType type)
        {
            switch (type)
            {
                case LetterType.SH:
                    return "Ш";
                case LetterType.N:
                    return "Н";
                case LetterType.G:
                    return "Г";
                case LetterType.E:
                    return "Е";
                case LetterType.SOFT:
                    return "Ь";
                case LetterType.Z:
                    return "З";
                case LetterType.T:
                    return "Т";
                case LetterType.TS:
                    return "Ц";
                case LetterType.P:
                    return "П";
                case LetterType.A:
                    return "А";
                case LetterType.Undef:
                    return "Неизвестно";
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
        
        private const string databaseLocation = "..\\..\\dataset";
        private Random random;
        public int LetterCount { get; set; }

        private Dictionary<LetterType, List<string>> structure;
        public DatasetProcessor()
        {
            random = new Random();
            structure = new Dictionary<LetterType, List<string>>();
            structure[LetterType.E] = new List<string>();
            structure[LetterType.G] = new List<string>();
            structure[LetterType.N] = new List<string>();
            structure[LetterType.SOFT] = new List<string>();
            structure[LetterType.SH] = new List<string>();
            structure[LetterType.A] = new List<string>();
            structure[LetterType.P] = new List<string>();
            structure[LetterType.T] = new List<string>();
            structure[LetterType.Z] = new List<string>();
            structure[LetterType.TS] = new List<string>();
            foreach (var letter in structure)
            {
                DirectoryInfo d = new DirectoryInfo(databaseLocation + $"\\{LetterTypeToString(letter.Key)}");
                letter.Value.AddRange(d.GetFiles("*.jpeg").Select(f => f.FullName));
            }
        }

        // Здесь мы полагаемся чисто на рандом
        public SamplesSet getTestDataset(int count)
        {
            // for (int i = 0; i < 100; i++)
            SamplesSet set = new SamplesSet();
            int blockWidth = 10;
            int blockHeight = 30;
            for (int type = 0; type < LetterCount; type++)
            {
                for (int i = 0; i < 100; i++)
                {
                    var sample = structure[(LetterType)type][random.Next(structure[(LetterType)type].Count)];
                    double[] input = new double[blockWidth * blockHeight];

                    using (FastBitmap fb = new FastBitmap(new Bitmap(sample)))
                    {
                        for (int blockX = 0; blockX < blockHeight; blockX++)
                        {
                            for (int blockY = 0; blockY < blockWidth; blockY++)
                            {
                                int startX = blockX * blockWidth;
                                int startY = blockY * blockHeight;
                                int endX = startX + blockWidth;
                                int endY = startY + blockHeight;

                                for (int x = startX; x < endX; x++)
                                {
                                    int currentSequence = 0;
                                    int maxSequence = 0;

                                    for (int y = startY; y < endY; y++)
                                    {
                                        if (fb[x, y].ToArgb() != Color.White.ToArgb())
                                        {
                                            currentSequence++;

                                            if (currentSequence > maxSequence)
                                            {
                                                maxSequence = currentSequence;
                                            }
                                        }
                                        else
                                        {
                                            currentSequence = 0;
                                        }
                                    }

                                    int blockIndex = blockX * blockWidth + blockY;
                                    input[blockIndex] = maxSequence;
                                }
                            }
                        }
                    }

                    set.AddSample(new Sample(input, LetterCount, (LetterType)type));
                }
            }
            set.shuffle();
            return set;
        }

        public SamplesSet getTrainDataset(int count)
        {
            SamplesSet set = new SamplesSet();
            int blockWidth = 10;
            int blockHeight = 30;

            for (int type = 0; type < LetterCount; type++)
            {
                for (int i = 0; i < count / LetterCount; i++)
                {
                    var sample = structure[(LetterType)type][random.Next(structure[(LetterType)type].Count)];
                    double[] input = new double[blockWidth * blockHeight];

                    using (FastBitmap fb = new FastBitmap(new Bitmap(sample)))
                    {
                        for (int blockX = 0; blockX < blockHeight; blockX++)
                        {
                            for (int blockY = 0; blockY < blockWidth; blockY++)
                            {
                                int startX = blockX * blockWidth;
                                int startY = blockY * blockHeight;
                                int endX = startX + blockWidth;
                                int endY = startY + blockHeight;

                                for (int x = startX; x < endX; x++)
                                {
                                    int currentSequence = 0;
                                    int maxSequence = 0;

                                    for (int y = startY; y < endY; y++)
                                    {
                                        if (fb[x, y].ToArgb() != Color.White.ToArgb())
                                        {
                                            currentSequence++;

                                            if (currentSequence > maxSequence)
                                            {
                                                maxSequence = currentSequence;
                                            }
                                        }
                                        else
                                        {
                                            currentSequence = 0;
                                        }
                                    }

                                    int blockIndex = blockX * blockWidth + blockY;
                                    input[blockIndex] = maxSequence;
                                }
                            }
                        }
                    }

                    set.AddSample(new Sample(input, LetterCount, (LetterType)type));
                }
            }

            set.shuffle();
            return set;
        }
        public Tuple<Sample, Bitmap> getSample()
        {
            var type = (LetterType) random.Next(LetterCount);
            var sample = structure[type][random.Next(structure[type].Count)];
            double[] input = new double[300];
            var bitmap = new Bitmap(sample);
            int blockWidth = 10;
            int blockHeight = 30;
            using (FastBitmap fb = new FastBitmap(new Bitmap(sample)))
            {
                for (int blockX = 0; blockX < blockHeight; blockX++)
                {
                    for (int blockY = 0; blockY < blockWidth; blockY++)
                    {
                        int startX = blockX * blockWidth;
                        int startY = blockY * blockHeight;
                        int endX = startX + blockWidth;
                        int endY = startY + blockHeight;

                        for (int x = startX; x < endX; x++)
                        {
                            int currentSequence = 0;
                            int maxSequence = 0;

                            for (int y = startY; y < endY; y++)
                            {
                                if (fb[x, y].ToArgb() != Color.White.ToArgb())
                                {
                                    currentSequence++;

                                    if (currentSequence > maxSequence)
                                    {
                                        maxSequence = currentSequence;
                                    }
                                }
                                else
                                {
                                    currentSequence = 0;
                                }
                            }

                            int blockIndex = blockX * blockWidth + blockY;
                            input[blockIndex] = maxSequence;
                        }
                    }
                }
            }
            return Tuple.Create<Sample, Bitmap>(new Sample(input,LetterCount,type),bitmap);
        }

        public Sample getSample(Bitmap bitmap)
        {
            double[] input = new double[300];
            int blockWidth = 10;
            int blockHeight = 30;
            using (FastBitmap fb = new FastBitmap(bitmap))
            {
                for (int blockX = 0; blockX < blockHeight; blockX++)
                {
                    for (int blockY = 0; blockY < blockWidth; blockY++)
                    {
                        int startX = blockX * blockWidth;
                        int startY = blockY * blockHeight;
                        int endX = startX + blockWidth;
                        int endY = startY + blockHeight;

                        for (int x = startX; x < endX; x++)
                        {
                            int currentSequence = 0;
                            int maxSequence = 0;

                            for (int y = startY; y < endY; y++)
                            {
                                if (fb[x, y].ToArgb() != Color.White.ToArgb())
                                {
                                    currentSequence++;

                                    if (currentSequence > maxSequence)
                                    {
                                        maxSequence = currentSequence;
                                    }
                                }
                                else
                                {
                                    currentSequence = 0;
                                }
                            }

                            int blockIndex = blockX * blockWidth + blockY;
                            input[blockIndex] = maxSequence;
                        }
                    }
                }
            }

            return new Sample(input, LetterCount);
        }
    }
}