using System;
using CommandLine;
using VKDialogHistoryFileMerger.Models;
using static VKDialogHistoryFileMergerService.VkDialogHistoryFileMergerService;

namespace VKDialogHistoryFileMerger;

internal static class Program
{
    private static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(options =>
            {
                if (options.AddCss == false)
                {
                    Console.WriteLine(
                        "Do you want to add CSS inside the merged file or keep the link? Y\\N (or other)");
                    options.AddCss = Console.ReadKey().KeyChar == 'Y';
                    Console.WriteLine();
                }

                Console.WriteLine(ExistsDialogFiles(options.Path).Result
                    ? MergeFiles(options.Path, options.AddCss, options.OutputPath)
                    : "There are no files. Move the script to the dialog directory and try again.");
            });

        Console.ReadKey();
    }
}