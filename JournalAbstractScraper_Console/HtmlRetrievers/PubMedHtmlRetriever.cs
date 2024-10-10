using System.Text;

namespace JournalAbstractScraper_Console.SearchBuilders
{
    internal class PubMedHtmlRetriever : IRetrieveHtml
    {
        private static HttpClient _httpClient;

        private static string _baseURL = "https://pubmed.ncbi.nlm.nih.gov/";
        private static string _dateFilter = "&filter=dates.";
        private static string _abstractFormatFilter = "&format=abstract";
        private static string _pageIndexFilter = "&page=";
        private static string _resultCountFilter = "&size=";

        public PubMedHtmlRetriever()
        {
            // Could reconsider DI if scaling to a LOT of retrievers. For now this works well.
            _httpClient = new HttpClient();
        }

        public async Task<List<string>> GetHtmlPagesToParse(SearchInstructions searchInstructions, Func<string, int> GetPaginationCountFunc)
        {          
            List<string> htmlToParse = new List<string>();

            string initialSearchTerm = BuildSearchUrlForJournal(searchInstructions);
            string initialHtml = await GetWebDataFromSite(initialSearchTerm);
            if (initialHtml == null) return null;
            htmlToParse.Add(initialHtml);

            int paginationCount = GetPaginationCountFunc(initialHtml);
            Console.WriteLine($"Retrieving results for {paginationCount} pages...");

            if (paginationCount < 0)
            {
                Console.WriteLine("Pagination retreival failed, returning first result");
                return htmlToParse;
            }

            if (paginationCount <= 1)
                return htmlToParse;

            List<Task<string>> pageSearchesToAdd = CollectAllPagesToSearch(searchInstructions, paginationCount);
            await Task.WhenAll(pageSearchesToAdd).ContinueWith(tasks =>
            {
                foreach (var data in tasks.Result)
                {
                    htmlToParse.Add(data);
                }
            });

            return htmlToParse;
        }

        private List<Task<string>> CollectAllPagesToSearch(SearchInstructions searchInstructions, int paginationCount)
        {
            // Continue requesting pages. Begin at page 2.
            List<Task<string>> pageSearchesToAdd = new List<Task<string>>();
            for (int i = 2; i < paginationCount; i++)
            {
                string searchTerm = BuildSearchUrlForJournal(searchInstructions);
                searchTerm += $"{_pageIndexFilter}{i}";

                var searchTask = GetWebDataFromSite(searchTerm);
                pageSearchesToAdd.Add(searchTask);
            }

            return pageSearchesToAdd;
        }

        // Build the url for the search criterea
        public string BuildSearchUrlForJournal(SearchInstructions searchInstructions)
        {
            StringBuilder searchString = new StringBuilder();

            // Create basic search with journal name
            searchString.Append(_baseURL);
            searchString.Append("?term=");
            searchString.Append(searchInstructions.SearchTerm);

            // Apply date range to filter
            searchString.Append(_dateFilter);
            string startDateString = $"{searchInstructions.StartYear}%2F{1}%2F{1}";
            searchString.Append(startDateString);
            searchString.Append("-");
            string endDateString = $"{searchInstructions.EndYear}%2F{12}%2F{31}";
            searchString.Append(endDateString);

            // Enforce abstract format of results for scraping
            searchString.Append(_abstractFormatFilter);

            // Page size filter for num results per page. Options are 10, 20, 50, 100, 200 on the website.
            searchString.Append(_resultCountFilter + 200);

            return searchString.ToString();
        }

        private async Task<string> GetWebDataFromSite(string targetURL)
        {
            Console.WriteLine($"Running Search For: \n {targetURL} \n");
            try
            {
                // Requires user agent or request will fail
                using HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, targetURL);
                requestMessage.Headers.Add("User-Agent", "User-Agent-Here");
                HttpResponseMessage response = await _httpClient.SendAsync(requestMessage);
                string content = await response.Content.ReadAsStringAsync();
                return content;
            }
            catch (Exception ex)
            {
                Console.WriteLine("PubMed html retrieval failed: " + ex.Message);
                throw;
            }
        }

    }
}
