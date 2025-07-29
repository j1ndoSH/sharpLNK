using CommandLine;
using ShellLink;
using System;
using System.IO;

namespace sharpLNK
{
    internal partial class Program
    {
        [Verb("dump", HelpText = "Dump data stored in a LNK file.")]
        class DumpOptions
        {
            [Option(Required = true, HelpText = "Path to the LNK file")]
            public string Filename { get; set; }

            //maybe drop stdout flag all together
            [Option(Default = true , HelpText = "Write data directly to stdout")]
            public bool Stdout { get; set; }

            [Option(HelpText = "Path to outfile for dumped information")]
            public string Outfile { get; set; }

            [Option(HelpText = "Extract all the data within the LNK")]
            public bool Verbose { get; set; }

        }

        private static void RunDumpOptions(DumpOptions opts)
        {
            string lnkPath = Environment.ExpandEnvironmentVariables(opts.Filename);

            var lnkContent = Shortcut.ReadFromFile(lnkPath);

            if (opts.Stdout)
            {
                Console.WriteLine($"[+] Writing extracted LNK data to stdout ...");

                if (opts.Verbose)
                {
                    Console.WriteLine($"\t[+] Verbose Output enabled");
                    Console.WriteLine(lnkContent);
                }
                else
                {
                    Console.WriteLine($"\t[-] Verbose Output disabled");
                    Console.WriteLine(GetSimplifiedShortcut(lnkContent));
                }
            }

            if (opts.Outfile != null)
            {
                string outfilePath = Environment.ExpandEnvironmentVariables(opts.Outfile);
                Console.WriteLine($"[+] Writing contents LNK to file \r\n\t[+] LNK Path: {lnkPath} \r\n\t[+] Outfile: {outfilePath}");

                string fileContent;

                if (opts.Verbose)
                {
                    Console.WriteLine($"\t[+] Verbose Output enabled");
                    fileContent = lnkContent.ToString();
                }
                else
                {
                    Console.WriteLine($"\t[-] Verbose Output disabled");
                    fileContent = GetSimplifiedShortcut(lnkContent);
                }

                try
                {
                    File.WriteAllText(outfilePath, fileContent);
                }
                catch
                {
                    throw new AccessViolationException();
                }
            }

        }

        private static string GetSimplifiedShortcut(Shortcut lnkContent)
        {
            string output = "\n[+] Simplified Shortcut Information\r\n"
                + $"\t[*] Target: {lnkContent.LinkTargetIDList?.Path ?? "(null)"}\r\n"
                + $"\t[*] Arguments: {lnkContent.StringData?.CommandLineArguments ?? "(null)"}\r\n"
                + $"\t[*] Working Directory: {lnkContent.StringData?.WorkingDir ?? "(null)"}\r\n"
                + $"\t[*] Window Style: {lnkContent.ShowCommand.ToString()}\r\n"
                + $"\t[*] Flags: {lnkContent.LinkFlags}\r\n"
                + $"\t[*] MachineID: {lnkContent.ExtraData.TrackerDataBlock.MachineID}\r\n";

            if (lnkContent.LinkInfo.VolumeID?.DriveSerialNumber != null)
                output += $"\t[*] DriveSerialNumber: {lnkContent.LinkInfo.VolumeID.DriveSerialNumber}\r\n";

            return output;
        }
    }
}
