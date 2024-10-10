
using JournalAbstractScraper_Console.Models;

namespace JournalAbstractScraper_Console.Exporters
{
    internal interface IExportJournalAbstracts
    {
        /// <summary>
        /// Exports a list of journal articles with the chosen file name
        /// </summary>
        public void ExportAbstracts(List<JournalArticle> articles, string fileName);
    }
}
