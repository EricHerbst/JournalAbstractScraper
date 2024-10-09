
namespace JournalAbstractScraper_Console.Models
{
    internal class JournalArticle
    {
        /// <summary>
        /// A link to the abstract referenced via sage DOI (digital object identifier)
        /// </summary>
        public string DoiLink;

        /// <summary>
        /// Contains the data of pub, issue and pages
        /// </summary>
        public string Source;

        /// <summary>
        /// The abbreviated journal name
        /// </summary>
        public string Journal;

        /// <summary>
        /// The full title of the article
        /// </summary>
        public string Title;

        /// <summary>
        /// The list of authors as a single string without affiliations
        /// </summary>
        public string Authors;

        /// <summary>
        /// The publiched abstract of the article
        /// </summary>
        public string Abstract;
    }
}
