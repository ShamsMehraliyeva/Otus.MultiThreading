using System.Diagnostics;

namespace Otus.MultiThreading
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int[] array100k = GenerateArray(100_000);
            int[] array1M = GenerateArray(1_000_000);
            int[] array10M = GenerateArray(10_000_000);

            Console.WriteLine("Results for 100k array:");
            MeasurePerformance(array100k);

            Console.WriteLine("Results for 1M array:");
            MeasurePerformance(array1M);

            Console.WriteLine("Results for 10M array:");
            MeasurePerformance(array10M);
        }

        static int[] GenerateArray(int size)
        {
            Random rand = new Random();
            return Enumerable.Range(0, size).Select(_ => rand.Next(1, 100)).ToArray();
        }

        static void MeasurePerformance(int[] array)
        {
            Stopwatch stopwatch = new Stopwatch();

            // Последовательное вычисление
            stopwatch.Start();
            long sequentialResult = SequentialSum(array);
            stopwatch.Stop();
            Console.WriteLine($"Sequential sum: {sequentialResult}, Time: {stopwatch.ElapsedMilliseconds} ms");

            // Параллельное вычисление
            stopwatch.Restart();
            long parallelResult = ParallelSum(array, 4); // 4 потока
            stopwatch.Stop();
            Console.WriteLine($"Parallel sum (Thread): {parallelResult}, Time: {stopwatch.ElapsedMilliseconds} ms");

            // Параллельное вычисление с помощью LINQ
            stopwatch.Restart();
            long plinqResult = PLINQSum(array);
            stopwatch.Stop();
            Console.WriteLine($"PLINQ sum: {plinqResult}, Time: {stopwatch.ElapsedMilliseconds} ms");
        }

        // Последовательное вычисление суммы
        static long SequentialSum(int[] array)
        {
            long sum = 0;
            foreach (var item in array)
            {
                sum += item;
            }
            return sum;
        }

        // Параллельное вычисление с использованием потоков
        static long ParallelSum(int[] array, int numberOfThreads)
        {
            long sum = 0;
            int chunkSize = array.Length / numberOfThreads;
            List<Thread> threads = new List<Thread>();
            object lockObj = new object();

            for (int i = 0; i < numberOfThreads; i++)
            {
                int start = i * chunkSize;
                int end = (i == numberOfThreads - 1) ? array.Length : start + chunkSize;

                Thread thread = new Thread(() =>
                {
                    long localSum = 0;
                    for (int j = start; j < end; j++)
                    {
                        localSum += array[j];
                    }
                    lock (lockObj)
                    {
                        sum += localSum;
                    }
                });
                threads.Add(thread);
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            return sum;
        }

        // Параллельное вычисление с помощью PLINQ
        static long PLINQSum(int[] array)
        {
            return array.AsParallel().Sum(x => (long)x);
        }
    }
}
