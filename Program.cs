using System;
using System.Collections.Generic;
using System.IO;

namespace FileExtension
{
    struct FileStats
    {
        public int Count { get; set; }
        public long TotalSize { get; set; }
        public double PercentCount { get; set; }
        public double PercentSize { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Корневой каталог для анализа
            string rootDirectory = @"D:\"; 
            Dictionary<string, FileStats> extensionStats = new Dictionary<string, FileStats>();

            // Анализ файлов в системе
            AnalyzeDirectory(rootDirectory, extensionStats);

            // Подсчет общего количества файлов и их размера
            int totalFiles = 0;
            long totalSize = 0;
            foreach (var stats in extensionStats.Values)
            {
                totalFiles += stats.Count;
                totalSize += stats.TotalSize;
            }

            // Подсчет процентов
            foreach (var key in extensionStats.Keys)
            {
                FileStats stats = extensionStats[key];
                stats.PercentCount = (double)stats.Count / totalFiles * 100;
                stats.PercentSize = (double)stats.TotalSize / totalSize * 100;
                extensionStats[key] = stats;
            }

            // Сортировка по количеству файлов
            var sortedStats = new List<KeyValuePair<string, FileStats>>(extensionStats);
            sortedStats.Sort((pair1, pair2) => pair2.Value.Count.CompareTo(pair1.Value.Count));

            // Вывод результатов
            Console.WriteLine("+---+------------+--------+-----------------+------------------------+--------------------+");
            Console.WriteLine("| № | Расширение | Кол-во | Общий объём в Б | % от общего количества | % от общего объёма |");
            Console.WriteLine("+---+------------+--------+-----------------+------------------------+--------------------+");

            for (int i = 0; i < Math.Min(50, sortedStats.Count); i++)
            {
                var pair = sortedStats[i];
                Console.WriteLine($"| {i + 1,1} | {pair.Key,-10} | {pair.Value.Count,6} | {pair.Value.TotalSize,15} | {pair.Value.PercentCount,22:F2} | {pair.Value.PercentSize,18:F2} |");
            }

            Console.WriteLine("+---+------------+--------+-----------------+------------------------+--------------------+");
            Console.WriteLine($"| TOTAL:         | {totalFiles,6} | {totalSize,15} | {100,22:F2} | {100,18:F2} |");
            Console.WriteLine("+----------------+--------+-----------------+------------------------+--------------------+");
        }

        static void AnalyzeDirectory(string directory, Dictionary<string, FileStats> extensionStats)
        {
            try
            {
                // Получаем файлы в текущем каталоге
                string[] files = Directory.GetFiles(directory);
                foreach (string file in files)
                {
                    string extension = Path.GetExtension(file).ToLower();
                    if (!string.IsNullOrEmpty(extension))
                    {
                        FileInfo fileInfo = new FileInfo(file);
                        if (extensionStats.ContainsKey(extension))
                        {
                            FileStats stats = extensionStats[extension];
                            stats.Count++;
                            stats.TotalSize += fileInfo.Length;
                            extensionStats[extension] = stats;
                        }
                        else
                        {
                            FileStats stats = new FileStats
                            {
                                Count = 1,
                                TotalSize = fileInfo.Length
                            };
                            extensionStats[extension] = stats;
                        }
                    }
                }

                // Рекурсивный вызов для подкаталогов
                string[] subdirectories = Directory.GetDirectories(directory);
                foreach (string subdirectory in subdirectories)
                {
                    AnalyzeDirectory(subdirectory, extensionStats);
                }
            }
            catch (Exception ex)
            {
                // Обработка ошибок доступа
                Console.WriteLine($"Ошибка при анализе каталога {directory}: {ex.Message}");
            }
        }
    }
}
