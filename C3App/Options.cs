using CommandLine;
using CommandLine.Text;

namespace C3App
{
    enum CommandType { IMPORT, VIEW };

    class Options
    {
        [Option('i', "input", HelpText = "File with new records to import")]
        public string InputFile { get; set; }

        [Option("c", DefaultValue = -1, Required = true, HelpText = "Command to run")]
        public CommandType Command { get; set; }

        [Option('b', "base", Required = true, HelpText = "File with existing records to append to")]
        public string BaseFile { get; set; }

        [Option('t', "type", HelpText = "Bank record format, e.g., USBank, Chase")]
        public string BankType { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            HelpText ht = HelpText.AutoBuild(new Options());
            return ht.ToString();
        }
    }
}
