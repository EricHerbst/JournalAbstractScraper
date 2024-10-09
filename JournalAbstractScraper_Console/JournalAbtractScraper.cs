﻿
using JournalAbstractScraper_Console.AbstractParsers;
using JournalAbstractScraper_Console.Exporters;
using JournalAbstractScraper_Console.Models;
using JournalAbstractScraper_Console.SearchBuilders;

namespace JournalAbstractScraper_Console
{
    internal class JournalAbtractScraper
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Beginning Abstract Retrieval... \n");

            IRetrieveHtml htmlRetriever = new PubMedHtmlRetriever();
            IParseAbstracts htmlParser = new PubmedAbstractParser();
            
            SearchInstructions searchInstructions = GetSearchInstructions();
            List<string> htmlToParse = await htmlRetriever.GetHtmlPagesToParse(searchInstructions, htmlParser.GetPaginationCount);

            if (htmlToParse == null || htmlToParse.Count <= 0)
            {
                Console.WriteLine("Error: no html to parse");
                return;
            }

            List<JournalArticle> articles = AssembleAbstracts(htmlParser, htmlToParse);

            string fileName = searchInstructions.SearchTerm + " - Abstract Results";
            ExportResults(articles, fileName);

            Console.WriteLine($"Abstract Retrieval Completed for {articles.Count} results");
        }

        // Manipulating this currently for different search params but could expand if UI is desired
        private static SearchInstructions GetSearchInstructions()
        {
            // Note about search term. For journal names, the abbreviated journal name produces more specific results than the full journal title.
            SearchInstructions searchInstructions = new SearchInstructions()
            {
                SearchTerm = "J Am Vet Med Assoc",
                StartYear = 2024,
                EndYear = 2024
            };
            return searchInstructions;
        }

        private static List<JournalArticle> AssembleAbstracts(IParseAbstracts htmlParser, List<string> htmlToParse)
        {
            List<JournalArticle> articles = new List<JournalArticle>();
            for (var i = 0; i < htmlToParse.Count; i++)
            {
                var parsedArticles = htmlParser.ParseHtmlIntoAbstracts(htmlToParse[i], true);
                articles.AddRange(parsedArticles);
            }
            return articles;
        }

        private static void ExportResults(List<JournalArticle> articles, string fileName)
        {
            if (articles.Count <= 0)
            {
                Console.WriteLine("Error: no articles to export");
                return;
            }

            IExportJournalAbstracts exporter = new CSVExporter();
            exporter.ExportAbstracts(articles, fileName);
        }


    }
}
