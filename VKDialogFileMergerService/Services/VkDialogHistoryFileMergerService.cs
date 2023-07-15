using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace VKDialogHistoryFileMergerService;

public static class VkDialogHistoryFileMergerService
{
    private static string _fileNamePattern = @"messages(\d+)\.html";

    public static async Task<string> MergeFiles(bool addCss, string? outputpath = null) =>
        await MergeFiles(".", addCss, outputpath);

    public static async Task<string> MergeFiles(string path, bool addCss, string? outputpath = null)
    {
        var htmlFiles = Directory.EnumerateFiles(path, "messages*.html")
            .Where(file => Regex.IsMatch(file, _fileNamePattern))
            .OrderBy(file => int.Parse(Regex.Match(file, _fileNamePattern).Groups[1].Value))
            .ToArray();
        var outputFileName = "MergedDialog.html";
        var doc = new HtmlDocument();
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        doc.LoadHtml(await File.ReadAllTextAsync(htmlFiles[0], Encoding.GetEncoding(1251)));
        var divNode = doc.DocumentNode.SelectSingleNode("//div[@class='ui_crumb']");
        outputFileName = Path.Combine(outputpath ?? string.Empty, divNode?.InnerText + outputFileName);
        var writer = new StringBuilder(2 + htmlFiles.Length * 50);
        writer.AppendLine(
            $"<!DOCTYPE html><html><head><meta charset=\"windows-1251\"><title>VK</title><link rel=\"shortcut icon\" href=\"../../favicon.ico\">{(addCss ? Resources.css : Resources.cssLink)}</head><body><div class=\"wrap\"><div class=\"header\"><div class=\"page_header\"><div class=\"top_home_logo\"></div></div></div><div class=\"page_content page_block\"><div class=\"wrap_page_content\"><div class=\"page_block_header clear_fix\"><div class=\"page_block_header_inner _header_inner\"><div class=\"ui_crumb\">{divNode?.InnerText}</div></div></div>");

        foreach (var htmlFile in htmlFiles)
        {
            var document = new HtmlDocument();
            document.Load(htmlFile, Encoding.GetEncoding(1251));

            var itemNodes = document.DocumentNode.SelectNodes("//div[contains(@class, 'item')]");
            if (itemNodes == null) continue;
            foreach (var messageContent in itemNodes.DistinctBy(item => Regex.Replace(item.InnerText, @"[\n\r\s]+", ""))
                         .Select(itemNode => itemNode.OuterHtml))
            {
                writer.AppendLine(messageContent);
            }
        }

        writer.AppendLine("</div></div></div></body></html>");
        try
        {
            await File.WriteAllTextAsync(outputFileName, writer.ToString(), Encoding.GetEncoding(1251));
        }
        catch (DirectoryNotFoundException)
        {
            return "Path not found. Input other path and try again.";
        }

        return "Dialog files merged successfully!\nOutput file: " + outputFileName;
    }

    public static Task<bool> ExistsDialogFiles(string path = ".")
    {
        try
        {
            return Task.FromResult(Directory.EnumerateFiles(path, "messages*.html")
                .Any(file => Regex.IsMatch(file, _fileNamePattern)));
        }
        catch (DirectoryNotFoundException)
        {
            return Task.FromResult(false);
        }
    }
}