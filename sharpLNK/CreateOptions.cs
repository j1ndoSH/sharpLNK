using CommandLine;
using ShellLink;
using ShellLink.Structures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace sharpLNK
{
    internal partial class Program
    {
        [Verb("create", HelpText = "Create a new LNK file")]
        class CreateOptions
        {
            [Option(Required = true , HelpText = "Path to the LNK file")]
            public string Filename { get; set; }

            [Option(Required = true, HelpText = "Path of Binary linked in LNK")]
            public string Target { get; set; }

            [Option(HelpText = "Commandline Arguments, use like --arguments=\"-blah\" otherwise the parsing breaks")]
            public string Arguments { get; set; }

            [Option(HelpText = "Direcotry where the LNK will start in")]
            public string Workingdirectory { get; set; }

            [Option(HelpText = "(Default: netBIOS name of host) Modify the machineID contained within the LNK")]
            public string MachineID { get; set; }

            [Option(Separator = ',', HelpText = "Timestamps in yyyy-MM-dd HH:mm:ss to set the creation,modified timestamps")]
            public IEnumerable<string> Timestomp { get; set; }

            [Option(HelpText = "Toggle overwriting if LNK file already exists")]
            public bool Force { get; set; }

            [Option(HelpText = "Toggle setting the 'hidden' file attribute")]
            public bool Hidden { get; set; }

        }

        private static void RunCreateOptions(CreateOptions opts)
        {

            string lnkPath = Environment.ExpandEnvironmentVariables(opts.Filename);

            if (File.Exists(lnkPath) && !opts.Force)
            {
                Console.WriteLine($"[!] Error: LNK File already exists at the location: {lnkPath}");
                Console.WriteLine("\t[!] Use the --force flag to overwrite the pre-existing file.");
                return;
            }

            Shortcut shortcut = new Shortcut()
            {
                LinkTargetIDList = new LinkTargetIDList()
                {
                    Path = opts.Target
                },

                LinkInfo = new LinkInfo()
                {
                    LocalBasePath = opts.Target
                },

                StringData = new StringData()
                {
                    RelativePath = GetRelativePath(lnkPath, opts.Target),
                    CommandLineArguments = opts.Arguments == null ? null : opts.Arguments,
                    WorkingDir = opts.Workingdirectory == null ? null : opts.Workingdirectory
                },

                ExtraData = new ExtraData()
                {
                    TrackerDataBlock = new TrackerDataBlock()
                    {
                        MachineID = opts.MachineID == null ? Environment.MachineName : opts.MachineID
                    }
                }
            };

            if (opts.Force)
                Console.WriteLine("[!] Overwriting existing LNK file");

            try
            {
                shortcut.WriteToFile(lnkPath);
                Console.WriteLine(GetSimplifiedShortcut(shortcut));

            }
            catch
            {
                throw new AccessViolationException();
            }


            if (opts.Hidden)
            {
                if (!File.Exists(lnkPath))
                {
                    Console.WriteLine($"[!] Error: File not found: {lnkPath}");
                    return;
                }

                File.SetAttributes(lnkPath, File.GetAttributes(lnkPath) | FileAttributes.Hidden);
                Console.WriteLine("[+] LNK File set as hidden");
            }

            if (opts.Timestomp.Any())
            {
                if (!File.Exists(lnkPath))
                {
                    Console.WriteLine($"[!] Error: File not found: {lnkPath}");
                    return;
                }

                Console.WriteLine($"[+] Performed Timestomping");

                if (opts.Timestomp.ElementAt(0) != null)
                {
                    DateTime creationTime = DateTime.Parse(opts.Timestomp.ElementAt(0));
                    File.SetCreationTime(lnkPath, creationTime);
                    Console.WriteLine($"\t[+] Set CreationTime to {creationTime.ToString()}");

                }
                else
                {
                    throw new ArgumentNullException(nameof(opts.Timestomp));
                }

                if (opts.Timestomp.ElementAt(1) != null)
                {
                    DateTime writeTime = DateTime.Parse(opts.Timestomp.ElementAt(1));
                    File.SetLastWriteTime(lnkPath, writeTime);
                    Console.WriteLine($"\t[+] Set LastWriteTime to {writeTime.ToString()}");
                }
                else
                {
                    throw new ArgumentNullException(nameof(opts.Timestomp));
                }
            }

            return;

        }
    }
}
