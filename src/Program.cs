using CommandLine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace FindLinks
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine($"Start program search url on pages");
            Console.WriteLine($"Version: {Assembly.GetExecutingAssembly().GetName().Version}");
            Console.WriteLine("");

            var options = new Options();
            var arguments = Parser.Default.ParseArguments<Options>(args)
                .WithParsed(x => options = x);

            var searchLinks = new SearchLinks(options);
            await searchLinks.FindAsync();

            searchLinks.PrintResult();
        }

        static void HandleParseError(IEnumerable<Error> errors)
        {
            foreach (var error in errors)
            {
                Console.WriteLine(error.Tag);
            }
        }
    }
}
