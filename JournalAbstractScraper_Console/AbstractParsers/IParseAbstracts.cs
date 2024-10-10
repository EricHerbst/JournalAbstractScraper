
using JournalAbstractScraper_Console.Models;

namespace JournalAbstractScraper_Console.AbstractParsers
{
    /// <summary>
    /// Interface for abstract parsing. Does not assume the framework for parsing after html is fetched.
    /// </summary>
    internal interface IParseAbstracts
    {
        /// <summary>
        /// Parse the high-end pagination result of an html page. I.e. there are 45 pages of results to parse. Returns -1 if fails.
        /// </summary>
        public int GetPaginationCount(string rawHtml);

        /// <summary>
        /// Converts a raw html into a list of Journal Articles. Can filter for only those with abstract content.
        /// </summary>
        public List<JournalArticle> ParseHtmlIntoAbstracts(string rawHtml, bool filterEmptyAbstracts);
    }
}
