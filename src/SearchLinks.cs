using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;

namespace FindLinks
{
    public enum TypeErrors : int
    {
        Ok = 1,
        UrlNotFound = 2,
        PageNotFound = 3,
        ErrorGetPage = 4
    };

    public class SearchLinks
    {
        /// <summary>
        /// Max threads for parsing pages
        /// </summary>
        private const int MaxThreads = 5;

        /// <summary>
        /// Options from user
        /// </summary>
        private readonly Options _options;

        /// <summary>
        /// Result of searching url
        /// </summary>
        private readonly ConcurrentDictionary<TypeErrors, List<string>> _results;

        /// <summary>
        /// Queue of task of working searching
        /// </summary>
        private readonly TaskQueue _taskQueue;

        public SearchLinks(Options options)
        {
            _options = options;
            _results = new ConcurrentDictionary<TypeErrors, List<string>>();


            _taskQueue = new TaskQueue(MaxThreads);
        }

        public async Task FindAsync()
        {
            if (!CheckParams())
            {
                return;
            }
            Write($"try to find link: {_options.Url}");

            List<string> listUrl = await GetUrlsAsync();
            ClearResult();

            var tasks = new Task[listUrl.Count];

            for (int i = 0; i < listUrl.Count; i++)
            {
                var url = listUrl[i];
                tasks[i] = _taskQueue.Enqueue(async () => await TodoWorkAsync(url));
            }

            Task.WaitAll(tasks);
        }

        /// <summary>
        /// Cleaning results
        /// </summary>
        private void ClearResult()
        {
            _results.Clear();
            foreach (var type in Enum.GetValues(typeof(TypeErrors)).Cast<TypeErrors>())
            {
                _results[type] = new List<string>();
            }
        }

        /// <summary>
        /// Reqeuest to page and find url in tag a
        /// </summary>
        /// <param name="url">Url needs parsing</param>
        private async Task TodoWorkAsync(string url)
        {
            try
            {
                Write($"starting parsing: {url}");
                var res = await new SearchUrl(_options.Url, url).FindAsync();
                _results[res].Add(url);
                Write($"result: {res} - {url}");
            }
            catch (Exception e)
            {
                Write($"error parsing page: {url}, {e.Message}");
                _results[TypeErrors.ErrorGetPage].Add(url);
            }
        }

        /// <summary>
        /// Check some params in Options
        /// </summary>
        private bool CheckParams()
        {
            if (!File.Exists(_options.FileName))
            {
                Write($"File not exists: {_options.FileName}");
                return false;
            }

            if (string.IsNullOrEmpty(_options.Url))
            {
                Write($"Ulr is empty");
                return false;
            }

            return true;
        }

        private async Task<List<string>> GetUrlsAsync() => (await File.ReadAllLinesAsync(_options.FileName)).ToList();

        private void Write(string str)
        {
            if (_options.Verbose)
            {
                Console.WriteLine(str);
            }
        }

        public void PrintResult()
        {
            var sb = new StringBuilder();
            foreach (var res in _results)
            {
                sb.Append(res.Key);
                sb.AppendFormat(" - {0}", res.Value.Count);
                sb.Append("\r\n");
                AddToSb(res.Value, sb);
            }
            File.WriteAllText(_options.OutName, sb.ToString());
            Write("");
            Write(sb.ToString());
        }

        private void AddToSb(List<string > urls, StringBuilder sb)
        {
            foreach (string url in urls)
            {
                sb.Append("\t");
                sb.Append(url);
                sb.Append("\r\n");
            }
        }
    }
}
