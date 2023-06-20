using System;
using static VKDialogHistoryFileMergerService.VkDialogHistoryFileMergerService;

namespace VKDialogHistoryFileMerger
{
    internal static class Program
    {
        private static void Main()
        {
            Console.WriteLine("Do you want to add css inside merged file or keep link? Y\\N(or other");
            var addCss = Console.ReadKey().KeyChar == 'Y';
            Console.WriteLine();
            Console.WriteLine(ExistsDialogFiles().Result
                ? MergeFiles(addCss)
                : "There are no files. Move script to dialog directory and try again.");
            Console.ReadKey();
        }
    }
}