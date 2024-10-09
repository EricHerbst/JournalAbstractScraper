
using JournalAbstractScraper_Console.Models;
using System.Text;

namespace JournalAbstractScraper_Console.Exporters
{
    internal class CSVExporter : IExportJournalAbstracts
    {
        public void ExportAbstracts(List<JournalArticle> articles, string fileName)
        {
            if (articles == null || articles.Count <= 0) return;

            StringBuilder articleBuilder = new StringBuilder();
            foreach (var article in articles)
            {
                articleBuilder.AppendLine(article.Title);
                articleBuilder.AppendLine(article.Authors);
                articleBuilder.AppendLine($"{article.Journal} {article.Source}");
                articleBuilder.AppendLine(article.DoiLink);
                articleBuilder.AppendLine(article.Abstract);
                articleBuilder.AppendLine("\n\n");
            }

            System.IO.File.WriteAllText($"{fileName}.csv", articleBuilder.ToString());
        }
    }
}
