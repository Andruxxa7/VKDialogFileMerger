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
            .WithParsed(Merge);

        Console.ReadKey();
    }

    private static async void Merge(Options options)
    {
        if (options.AddCss == false)
        {
            Console.WriteLine("Do you want to add CSS inside the merged file or keep the link? Y\\N (or other)");
            options.AddCss = Console.ReadKey().Key is ConsoleKey.Y;
            Console.WriteLine();
        }

        Console.WriteLine(await ExistsDialogFiles(options.Path)
            ? await MergeFiles(options.Path, options.AddCss, options.OutputPath)
            : "There are no files. Move the script to the dialog directory and try again.");
    }
}