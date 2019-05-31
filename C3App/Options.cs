using CommandLine;
using CommandLine.Text;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace C3App
{
    enum CommandType { IMPORT, VIEW };

    class Options
    {
        public string commandTypeNames;

        [Option('c', "command", DefaultValue = CommandType.VIEW, Required = true, HelpText = "Command to run ('VIEW' or 'IMPORT')")]
        public CommandType Command { get; set; }

        [Option('i', "input", HelpText = "Path to file with new records to import")]
        public string InputFile { get; set; }

        [Option('b', "base", Required = true, HelpText = "Path to file with existing records to append to")]
        public string BaseFile { get; set; }

        [Option('t', "type", HelpText = "Bank record format ('USBank' or 'Chase')")]
        public string BankType { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            HelpText ht = HelpText.AutoBuild(new Options());
            return ht.ToString();
        }

        public Options()
        {
            this.commandTypeNames = string.Join(",", Enum.GetValues(typeof(CommandType)).Cast<CommandType>().Select(ct => ct.ToString()));
        }
    }
}
