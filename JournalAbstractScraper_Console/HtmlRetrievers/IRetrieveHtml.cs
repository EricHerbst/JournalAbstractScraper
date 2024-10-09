namespace JournalAbstractScraper_Console.SearchBuilders
{
    internal interface IRetrieveHtml
    {
        /// <summary>
        /// Get a list of html pages to parse based on the search instructions
        /// </summary>
        public Task<List<string>> GetHtmlPagesToParse(SearchInstructions searchInstructions, Func<string, int> GetPaginationCountFunc);
    }
}
