namespace JournalAbstractScraper_Console.SearchBuilders
{
    internal class SearchInstructions
    {
        /// <summary>
        /// The search term desired.
        /// </summary>
        public string SearchTerm;

        /// <summary>
        /// The year to start the search for. Defaults to Jan 1st of the year.
        /// </summary>
        public int StartYear;

        /// <summary>
        /// The year to end the search for. Defaults to Dec 31st of the year.
        /// </summary>
        public int EndYear;
    }
}
