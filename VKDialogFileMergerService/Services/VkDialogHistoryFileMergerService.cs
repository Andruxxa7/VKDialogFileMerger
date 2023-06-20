using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace VKDialogHistoryFileMergerService
{
    public static class VkDialogHistoryFileMergerService
    {
        public static string MergeFiles(bool addCss, string? outputpath = null) => MergeFiles(".", addCss, outputpath);

        public static string MergeFiles(string path, bool addCss, string? outputpath = null)
        {
            var htmlFiles = Directory.GetFiles(path, "messages*.html")
                .Where(file => Regex.IsMatch(file, @"messages\d+\.html"))
                .OrderBy(file => int.Parse(Regex.Match(file, @"messages(\d+)\.html").Groups[1].Value))
                .ToArray();
            var outputFileName = "MergedDialog.html";
            var doc = new HtmlDocument();
            doc.LoadHtml(File.ReadAllText(htmlFiles[0], Encoding.GetEncoding(1251)));
            var divNode = doc.DocumentNode.SelectSingleNode("//div[@class='ui_crumb']");
            outputFileName = (outputpath is null ? string.Empty : $"{outputpath}\\") + divNode?.InnerText +
                             outputFileName;
            var writer = new StringBuilder();
            writer.Append(
                $"<!DOCTYPE html><html><head><meta charset=\"windows-1251\"><title>VK</title><link rel=\"shortcut icon\" href=\"../../favicon.ico\">{(addCss ? Resources.css : Resources.cssLink)}</head><body><div class=\"wrap\"><div class=\"header\"><div class=\"page_header\"><div class=\"top_home_logo\"></div></div></div><div class=\"page_content page_block\"><div class=\"wrap_page_content\"><div class=\"page_block_header clear_fix\"><div class=\"page_block_header_inner _header_inner\"><div class=\"ui_crumb\">{divNode?.InnerText}</div></div></div>");

            foreach (var htmlFile in htmlFiles)
            {
                var document = new HtmlDocument();
                document.Load(htmlFile);

                var itemNodes = document.DocumentNode.SelectNodes("//div[contains(@class, 'item')]");
                if (itemNodes == null) continue;
                foreach (var itemNode in itemNodes)
                {
                    var messageContent = itemNode.OuterHtml;
                    if (writer.ToString().Contains(messageContent)) continue;
                    writer.Append(messageContent);
                }
            }

            writer.Append("</div></div></div></body></html>");
            File.WriteAllText(outputFileName, writer.ToString(), Encoding.GetEncoding(1251));
            return "Dialog files merged successfully!\nOutput file: " + outputFileName;
        }

        public static Task<bool> ExistsDialogFiles(string path = ".") => Task.FromResult(Directory
            .GetFiles(path, "messages*.html").Any(file => Regex.IsMatch(file, @"messages\d+\.html")));
    }
}