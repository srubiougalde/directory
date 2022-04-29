using System.Net;
using HtmlAgilityPack;

namespace DirectoryApi.Helpers
{
    public class FeedHelper
    {
        public static IEnumerable<HtmlNode> ReadHeadings(string url)
        {
            try
            {
                var httpClient = new HttpClient
                {
                    BaseAddress = new Uri(url)
                };
                var result = httpClient.GetAsync("").Result;

                if (result.StatusCode == HttpStatusCode.OK || result.StatusCode == HttpStatusCode.Accepted)
                {
                    var strData = result.Content.ReadAsStringAsync().Result;

                    HtmlDocument htmlDocument = new();
                    htmlDocument.LoadHtml(strData);

                    List<HtmlNode> headings = new();
                    headings.AddRange(htmlDocument.DocumentNode.Descendants("h1"));
                    headings.AddRange(htmlDocument.DocumentNode.Descendants("h3"));
                    return headings;
                }
            }
            catch (Exception e)
            {
                // ToDo. Log errors                
                return null;
            }

            return null;
        }
    }
}