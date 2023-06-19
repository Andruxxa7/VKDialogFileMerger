using System;
using static VKDialogHistoryFileMergerService.VkDialogHistoryFileMergerService;

namespace VKDialogHistoryFileMerger
{
    internal static class Program
    {
        private static void Main()
        {
            Console.WriteLine(ExistsDialogFiles().Result
                ? MergeFiles()
                : "There are no files. Move script to dialog directory and try again.");
            Console.ReadKey();
        }
    }
}