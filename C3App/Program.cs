using System;
using System.IO;
using System.Windows;

using C3;
using C3.Readers;
using C3View;

namespace C3App
{
    /// <summary>
    /// Thin CLI wrapper around the C3 core library
    /// </summary>
    public class Program
    {
        private static C3Configuration config;

        [STAThread]
        static void Main(string[] args)
        {
            config = C3Configuration.LoadFromConfigurationManager();
            var options = new Options();
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                if (options.Command == CommandType.IMPORT)
                {
                    IRecordReader bankRecordReader = null;
                    try
                    {
                        bankRecordReader = new CsvRecordReader(options.BankType);
                    }
                    catch (ApplicationException e)
                    {
                        Console.WriteLine($"Unable to create '{options.BankType}' record reader:\n{e.Message}");
                        Environment.Exit(-1);
                    }
                    if (!File.Exists(options.BaseFile))
                        Console.WriteLine("Base file {0} does not exist.", options.BaseFile);
                    else if (!File.Exists(options.InputFile))
                        Console.WriteLine("Input file {0} does not exist.", options.BaseFile);
                    else
                    {
                        try
                        {
                            Updater.ClassifyAndUpdate(options.BaseFile, options.InputFile, bankRecordReader, config);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Update failed: {0}", e.Message);
                        }
                    }
                }
                if (options.Command == CommandType.VIEW)
                {
                    if (options.BaseFile == null)
                    {
                        Console.WriteLine("Base file must be specified.");
                        return;
                    }
                    if (!File.Exists(options.BaseFile))
                    {
                        Console.WriteLine($"Failed to open specified base file {options.BaseFile}");
                        return;
                    }
                    CCRecordSet records = CCRecordSet.FromFile(options.BaseFile, config);
                    new Application().Run(new C3Window(records, options.BaseFile, config));
                }
            }
        }
    }
}