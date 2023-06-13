using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace VKDialogHistoryFileMerger
{
    internal static class Program
    {
        private static void Main()
        {
            var htmlFiles = Directory.GetFiles(".", "messages*.html")
                .Where(file => Regex.IsMatch(file, @"messages\d+\.html"))
                .OrderBy(file => int.Parse(Regex.Match(file, @"messages(\d+)\.html").Groups[1].Value))
                .ToArray();
            var outputFileName = "MergedDialog.html";

            var uniqueMessages = new Dictionary<string, string>();
            var doc = new HtmlDocument();
            doc.LoadHtml(File.ReadAllText(htmlFiles[0], Encoding.GetEncoding(1251)));
            var divNode = doc.DocumentNode.SelectSingleNode("//div[@class='ui_crumb']");
            var name = divNode?.InnerText;
            outputFileName = name + outputFileName;
            using (var writer = new StreamWriter(outputFileName, false, Encoding.GetEncoding(1251)))
            {
                writer.WriteLine(
                    "<!DOCTYPE html><html><head><meta charset=\"windows-1251\"><title>VK</title><link rel=\"shortcut icon\" href=\"../../favicon.ico\"><link rel=\"stylesheet\" type=\"text/css\" href=\"../../style.css\"></head><body><div class=\"wrap\"><div class=\"header\"><div class=\"page_header\"><div class=\"top_home_logo\"></div></div></div><div class=\"page_content page_block\">");
                
                    writer.WriteLine(
                        $"<div class=\"page_block_header clear_fix\"><div class=\"page_block_header_inner _header_inner\"><div class=\"ui_crumb\">{name}</div></div></div>");

                    foreach (var htmlFile in htmlFiles)
                {
                    var document = new HtmlDocument();
                    document.Load(htmlFile);

                    var itemNodes = document.DocumentNode.SelectNodes("//div[contains(@class, 'item')]");
                    if (itemNodes == null) continue;
                    foreach (var itemNode in itemNodes)
                    {
                        var messageContent = itemNode.OuterHtml;
                        var dataId = Regex.Match(messageContent, @"data-id=""(\d+)""").Groups[1].Value;
                        if (uniqueMessages.TryGetValue(dataId, out _)) continue;
                        writer.WriteLine(messageContent);
                        writer.WriteLine("</div><div class=\"page_content page_block\">");

                        uniqueMessages.Add(dataId, messageContent);
                    }
                }

                writer.WriteLine("</div></div></body></html>");
                writer.Close();
            }

            Console.WriteLine("Dialog files merged successfully!\nOutput file: " + outputFileName);
        }
    }
}