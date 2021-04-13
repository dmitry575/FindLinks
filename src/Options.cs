using CommandLine;

namespace FindLinks
{
    public class Options
    {
        [Option('f', "file", Required = true, HelpText = "Set input file with links where need search url")]
        public string FileName { get; set; }

        [Option('o', "out", Required = true, HelpText = "Set out file with result")]
        public string OutName { get; set; }

        [Option('v', "verbose", Default =false, Required = false, HelpText = "Set output to verbose messages")]
        public bool Verbose { get; set; }

        [Value(0, Required = true, HelpText = "Url for seach in tag 'a' propery href")]
        public string Url { get; set; }
    }
}
