using System;
using static VKDialogHistoryFileMergerService.VkDialogHistoryFileMergerService;

namespace VKDialogHistoryFileMerger
{
    internal static class Program
    {
        private static void Main()
        {
            if (ExistsDialogFiles().Result) MergeFiles();
            else Console.WriteLine("There are no files. Move script to dialog directory and try again.");
            Console.ReadKey();
        }
    }
}