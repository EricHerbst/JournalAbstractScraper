
using HtmlAgilityPack;
using JournalAbstractScraper_Console.Models;
using System.Text;
using static System.Collections.Specialized.BitVector32;

namespace JournalAbstractScraper_Console.AbstractParsers
{
    /// <summary>
    /// Parses raw pubMed html info into a Journal Articles.
    /// </summary>
    internal class PubmedAbstractParser : IParseAbstracts
    {
        private readonly string NotAvailableAbbreviation = "N/A";

        public int GetPaginationCount(string rawHtml)
        {
            if (rawHtml == null) return 0;

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(rawHtml);

            HtmlNode paginationSection = htmlDoc.DocumentNode.SelectSingleNode(".//label[contains(@class, 'of-total-pages')]");
            if (paginationSection == null) return 1;
            
            string pageNumbers = string.Concat(paginationSection.InnerHtml?.Where(char.IsDigit));
            if (pageNumbers == null) return 1;

            try
            {
                return int.Parse(pageNumbers);
            }
            catch
            {
                return 1;
            }
        }

        // Parsing note. Use // to search from document begin. Use .// to search from current node.
        public List<JournalArticle> ParseHtmlIntoAbstracts(string rawHtml, bool filterEmptyAbstracts)
        {
            List<JournalArticle> articles = new List<JournalArticle>();

            if (rawHtml == null || rawHtml.Length <= 0)
                return articles;

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(rawHtml);

            // Get all abstract sections for individual parsing
            IEnumerable<HtmlNode> abstractSections = htmlDoc.DocumentNode.SelectNodes("//article[contains(@class, 'article-overview')]");
            if (abstractSections == null) return articles;

            foreach (HtmlNode abstractSection in abstractSections)
            {
                if (filterEmptyAbstracts && AbstractIsEmpty(abstractSection)) continue;

                JournalArticle article = AssembleArticleFromSection(abstractSection);
                articles.Add(article);
            }
  
            return articles;
        }

        private JournalArticle AssembleArticleFromSection(HtmlNode section)
        {
            JournalArticle article = new JournalArticle();
            article.DoiLink = ExtractLinkFromSection(section);
            article.Source = ExtractSourceFromSection(section);
            article.Journal = ExtractJournalNameFromSection(section);
            article.Title = ExtractTitleFromSection(section);
            article.Authors = ExtractAuthorsFromSection(section);
            article.Abstract = AssembleAbstractFromSection(section);
            return article;
        }

        private string ExtractLinkFromSection(HtmlNode section)
        {
            HtmlNode doiSection = section.SelectSingleNode(".//a[contains(@data-ga-action, 'DOI')]");

            if (doiSection == null || doiSection.InnerHtml == null)
                return NotAvailableAbbreviation;

            string link = "https://journals.sagepub.com/doi/" + doiSection.InnerHtml.Trim();
            return link;
        }

        private string ExtractSourceFromSection(HtmlNode section)
        {
            HtmlNode sourceSection = section.SelectSingleNode(".//div[contains(@class, 'article-source')]");

            if (sourceSection == null || sourceSection.InnerHtml == null)
                return NotAvailableAbbreviation;

            HtmlNode journalCitNode = sourceSection.SelectSingleNode(".//span[contains(@class, 'cit')]");
            string citation = journalCitNode?.InnerHtml?.Trim() ?? NotAvailableAbbreviation;
            return citation;
        }

        private string ExtractJournalNameFromSection(HtmlNode section)
        {
            HtmlNode sourceSection = section.SelectSingleNode(".//div[contains(@class, 'article-source')]");

            if (sourceSection == null || sourceSection.InnerHtml == null)
                return NotAvailableAbbreviation;

            HtmlNode journalNameNode = sourceSection.SelectSingleNode(".//button[contains(@ref, 'linksrc=journal_actions_btn')]");
            string journalName = journalNameNode?.InnerHtml?.Trim() ?? NotAvailableAbbreviation;
            return journalName;
        }

        private string ExtractTitleFromSection(HtmlNode section)
        {
            // Get the pub med ID to use in the anchor link tag search
            HtmlNode pmidNode = section.SelectSingleNode(".//strong[contains(@title, 'PubMed ID')]");
            string pmidString = pmidNode?.InnerText;
     
            if (pmidString == null)
                return NotAvailableAbbreviation;

            IEnumerable<HtmlNode> headerNodesWithChild = section.SelectNodes(".//h1[contains(@class, 'heading-title')]");
            foreach (var headerNode in headerNodesWithChild)
            {
                HtmlNode titleNode = headerNode.SelectSingleNode($"//a[contains(@data-article-id, '{pmidString}')]");
                if (titleNode != null)
                {
                    string title = titleNode?.InnerHtml?.Trim() ?? NotAvailableAbbreviation;
                    return title;
                }
            }

            return NotAvailableAbbreviation;
        }

        private string ExtractAuthorsFromSection(HtmlNode section)
        {
            HtmlNode authorsNode = section.SelectSingleNode(".//div[contains(@class, 'inline-authors')]");

            if (authorsNode == null || authorsNode.InnerHtml == null)
                return NotAvailableAbbreviation;

            // Extract author names from section, ignoring affiliations, superscripts, links, etc.
            StringBuilder builder = new StringBuilder();
            HtmlNodeCollection authorNameNodes = authorsNode.SelectNodes(".//a[contains(@class, 'full-name')]");
            if (authorNameNodes == null)
                return NotAvailableAbbreviation;

            // Author names can go on forever and we only need the first for searchability
            var firstAuthor = authorNameNodes.First();
            if (firstAuthor != null && firstAuthor.InnerText.Length > 0)
            {
                builder.Append(firstAuthor.InnerText);
                if (authorNameNodes.Count > 1)
                {
                    builder.Append(" et al.");      // Indicate there's more than one via publishing standards
                }
            }
            
            return builder.ToString().Trim();
        }

        private bool AbstractIsEmpty(HtmlNode section)
        {
            // Class only exists when abstract is empty
            HtmlNode emptyAbstractNode = section.SelectSingleNode(".//em[contains(@class, 'empty-abstract')]");
            return emptyAbstractNode != null;
        }

        private string AssembleAbstractFromSection(HtmlNode section)
        {
            HtmlNode abstractNode = section.SelectSingleNode(".//div[contains(@class, 'abstract')]");

            // If abstract is empty, skip meta data and return the 'not avaialble' inner text
            HtmlNode emptyAbstractNode = abstractNode.SelectSingleNode(".//em[contains(@class, 'empty-abstract')]");
            if (emptyAbstractNode != null)
            {
                return emptyAbstractNode.InnerHtml?.Trim() ?? "Empty";
            }
            
            // Combine the paragraph texts into one string
            StringBuilder builder = new StringBuilder();
            var paragraphNodes = abstractNode.SelectNodes(".//p");
            foreach (var paragraphNode in paragraphNodes)
            {
                string paragraphText = paragraphNode.InnerText.Trim();
                builder.Append(paragraphText);
            }

            return builder.ToString();
        }
    }
}
