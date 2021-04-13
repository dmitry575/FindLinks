using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FindLinks
{
    public class SearchUrl
    {
        /// <summary>
        /// Url witch need to find
        /// </summary>
        private readonly string _url;

        /// <summary>
        /// Url for page witch need download and parsing
        /// </summary>
        private readonly string _urlPage;

        private readonly HttpClient _httpClient = new HttpClient
        {
            MaxResponseContentBufferSize = 1_000_000,
        };
        public SearchUrl(string url, string urlPage)
        {
            _url = url;
            _urlPage = urlPage;
            _httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            _httpClient.DefaultRequestHeaders.Add("Accept-Language", "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");
            _httpClient.DefaultRequestHeaders.Add("Referer", "https://yandex.com");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.114 Safari/537.36");
        }

        public async Task<TypeErrors> FindAsync()
        {
            byte[] contentByte;
            try
            {
                contentByte = await _httpClient.GetByteArrayAsync(_urlPage);
            }
            catch (HttpRequestException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return TypeErrors.PageNotFound;
                }
                throw;
            }

            var count = new SearchHtml(contentByte, x => x.StartsWith(_url)).Search();

            return count > 0 ? TypeErrors.Ok : TypeErrors.UrlNotFound;
        }
    }
}
