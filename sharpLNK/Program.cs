using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.IO;

namespace sharpLNK
{
    internal partial class Program
    {

        public static void DisplayBanner()
        {
            Console.WriteLine("     _                        __      __      \r\n ___| |__   __ _ _ __ _ __   / /   /\\ \\ \\/\\ /\\\r\n/ __| '_ \\ / _` | '__| '_ \\ / /   /  \\/ / //_/\r\n\\__ \\ | | | (_| | |  | |_) / /___/ /\\  / __ \\ \r\n|___/_| |_|\\__,_|_|  | .__/\\____/\\_\\ \\/\\/  \\/ \r\n                     |_|                      \r\n");
            return;
        }

        static void Main(string[] args)
        {
            var helpWriter = new StringWriter();
            var parser = new CommandLine.Parser(with => with.HelpWriter = helpWriter);
            var parsedOptions = parser.ParseArguments<ModifyOptions, DumpOptions, CreateOptions>(args);

            parsedOptions
                .WithParsed<ModifyOptions>(opts => RunModifyOptions(opts))
                .WithParsed<DumpOptions>(opts => RunDumpOptions(opts))
                .WithParsed<CreateOptions>(opts => RunCreateOptions(opts))
                .WithNotParsed(x =>
                {
                    var helpText = HelpText.AutoBuild(parsedOptions, h =>
                    {
                        h.AutoVersion = false;  // hides --version
                        h.AdditionalNewLineAfterOption = false;
                        return HelpText.DefaultParsingErrorsHandler(parsedOptions, h);
                    }, e => e, verbsIndex: true);
                    DisplayBanner();
                    Console.WriteLine(helpText);
                });
        }


    }
}
