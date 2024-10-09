
using JournalAbstractScraper_Console.Models;

namespace JournalAbstractScraper_Console.Exporters
{
    internal interface IExportJournalAbstracts
    {
        public void ExportAbstracts(List<JournalArticle> articles, string fileName);
    }
}
