using HtmlAgilityPack;
using System;
using System.Text;

namespace FindLinks
{
    /// <summary>
    /// Find in html
    /// </summary>
    public class SearchHtml
    {
        private readonly byte[] _content;
        private readonly Func<string, bool> _compare;

        public SearchHtml(byte[] content, Func<string, bool> compare)
        {
            _content = content;
            _compare = compare;
        }

        /// <summary>
        /// Search count of compare url
        /// </summary>
        public int Search()
        {
            if (_content == null || _content.Length <= 0)
            {
                return 0;
            }

            HtmlDocument hap = new HtmlDocument();
            hap.LoadHtml(Encoding.UTF8.GetString(_content));
            HtmlNodeCollection nodes = hap.DocumentNode.SelectNodes("//a");
            int res = 0;

            if (nodes != null)
            {

                foreach (HtmlNode node in nodes)
                {
                    string url = node.GetAttributeValue("href", "");
                    if (_compare(url))
                    {
                        res++;
                    }
                }
            }

            return res;
        }
    }
}
