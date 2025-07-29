using CommandLine;
using ShellLink;
using System;
using System.IO;


namespace sharpLNK
{
    internal partial class Program
    {
        [Verb("modify", HelpText = "Modify an existing LNK file.")]
        class ModifyOptions
        {
            [Option(Required = true , HelpText = "Path to the LNK file")]
            public string Filename { get; set; }

            [Option(HelpText = "Path of Binary linked in LNK")]
            public string Target { get; set; }

            [Option(HelpText = "Commandline Arguments, use like --arguments=\"-blah\" otherwise the parsing breaks")]
            public string Arguments { get; set; }

            [Option(HelpText = "Directory where the LNK will start in")]
            public string Workingdirectory { get; set; }

            [Option(HelpText = "Modify machineID (netBIOS name) contained within the LNK")]
            public string MachineID { get; set; }

            [Option(HelpText = "Toggle setting the 'last modified timestamp' to the original one (before modifying)")]
            public bool Timestomp { get; set; }

            [Option(HelpText = "Toggle setting the 'hidden' file attribute")]
            public bool Hidden { get; set; }
        }

        public static string GetRelativePath(string fromPath, string toPath)
        {
            if (string.IsNullOrEmpty(fromPath)) throw new ArgumentNullException("fromPath");
            if (string.IsNullOrEmpty(toPath)) throw new ArgumentNullException("toPath");

            fromPath = Path.GetFullPath(fromPath);
            toPath = Path.GetFullPath(toPath);

            Uri fromUri = new Uri(fromPath);
            Uri toUri = new Uri(toPath);

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);
            string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            relativePath = relativePath.Replace('/', Path.DirectorySeparatorChar);

            return relativePath;
        }

        private static void RunModifyOptions(ModifyOptions opts)
        {

            string lnkPath = Environment.ExpandEnvironmentVariables(opts.Filename);


            if (!File.Exists(lnkPath))
            {
                Console.WriteLine($"[!] Error: File not found: {lnkPath}");
                return;
            }

            DateTime originalLastWriteTime = File.GetLastWriteTime(lnkPath);

            Shortcut originalLnk = Shortcut.ReadFromFile(lnkPath);
            Shortcut modifiedLNK = Shortcut.ReadFromFile(lnkPath);

            if (opts.Target != null)
            {
                modifiedLNK.LinkTargetIDList.Path = opts.Target;
                modifiedLNK.LinkInfo.LocalBasePath = opts.Target;
                modifiedLNK.StringData.RelativePath = GetRelativePath(lnkPath, modifiedLNK.LinkTargetIDList.Path);
                Console.WriteLine($"[+] Changes made to LNK Path/LocalBasePath");
                Console.WriteLine($"\t[+] {originalLnk.LinkTargetIDList.Path} -> {modifiedLNK.LinkTargetIDList.Path}");
            }

            if (opts.Arguments != null)
            {
                modifiedLNK.StringData.CommandLineArguments = opts.Arguments;
                Console.WriteLine($"[+] Changes made to LNK Arguments");
                Console.WriteLine($"\t[+] {originalLnk.StringData.CommandLineArguments} -> {modifiedLNK.StringData.CommandLineArguments}");
            }


            if (opts.Workingdirectory != null)
            {
                modifiedLNK.StringData.WorkingDir = opts.Workingdirectory;
                Console.WriteLine($"[+] Changes made to LNK WorkingDirectory");
                Console.WriteLine($"\t[+] {originalLnk.StringData.WorkingDir} -> {modifiedLNK.StringData.WorkingDir}");
            }

            if (opts.MachineID != null)
            {
                modifiedLNK.ExtraData.TrackerDataBlock.MachineID = opts.MachineID;
                Console.WriteLine($"[+] Changes made to LNK machineID trackerdata");
                Console.WriteLine($"\t[+] {originalLnk.ExtraData.TrackerDataBlock.MachineID} -> {modifiedLNK.ExtraData.TrackerDataBlock.MachineID}");
            }

            modifiedLNK.WriteToFile(lnkPath);

            if (opts.Hidden)
            {
                File.SetAttributes(lnkPath, File.GetAttributes(lnkPath) | FileAttributes.Hidden);
                Console.WriteLine("[+] LNK File set as hidden");
            }

            if (opts.Timestomp)
            {
                File.SetLastWriteTime(lnkPath, originalLastWriteTime);
                Console.WriteLine($"[+] Timestomped original LastWriteTime - {originalLastWriteTime}");
            }

            return;
        }
    }
}
