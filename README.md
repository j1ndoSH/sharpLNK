# sharpLNK
---

sharpLNK is a small C# program to modify certain aspects of pre-existing LNK files, create new LNKs and/or print said information so that one knows what they are about to overwrite.

I created this primarily because I wanted a little project where I work with C#. As such the way I did some things might be questionable. The idea itself was inspired by discussions about tooling during my daily work. 

# Help
---
```
λ .\sharpLNK.exe --help
     _                        __      __
 ___| |__   __ _ _ __ _ __   / /   /\ \ \/\ /\
/ __| '_ \ / _` | '__| '_ \ / /   /  \/ / //_/
\__ \ | | | (_| | |  | |_) / /___/ /\  / __ \
|___/_| |_|\__,_|_|  | .__/\____/\_\ \/\/  \/
                     |_|

sharpLNK 1.0.0.0
by @j1ndoSH

  modify    Modify an existing LNK file.
  dump      Dump data stored in a LNK file.
  create    Create a new LNK file
  help      Display more information on a specific command.
```

## create
---

When creating a LNK file with sharpLNK it will look different then when you create one with Powershell (e.g `WScript.Shell`). Most notably create the LNK created by Powershell (if the targeted file can be resolved) will be "fully" populated with metadata such as `KnownFolderID`, `DriveSerialNumber` and other less relevant data. A resolvable LNK created by Powershell will correctly populate the `CreationTime`,`AccessTime`, `WriteTime` for the file the LNK is targeting. When using sharpLNK these timestamps will currently all be set to `01/01/1601 00:00:00`.

```
λ .\sharpLNK.exe create --help
     _                        __      __
 ___| |__   __ _ _ __ _ __   / /   /\ \ \/\ /\
/ __| '_ \ / _` | '__| '_ \ / /   /  \/ / //_/
\__ \ | | | (_| | |  | |_) / /___/ /\  / __ \
|___/_| |_|\__,_|_|  | .__/\____/\_\ \/\/  \/
                     |_|

sharpLNK 1.0.0.0
by @j1ndoSH

  filename            Required. Path to the LNK file
  target              Required. Path of Binary linked in LNK
  arguments           Commandline Arguments, recommended to use like
                      --arguments="-blah"
  workingdirectory    Direcotry where the LNK will start in
  machineid           (Default: netBIOS name of host) Modify the machineID
                      contained within the LNK
  timestomp           Timestamps in yyyy-MM-dd HH:mm:ss to set the
                      creation,modified timestamps
  force               Toggle overwriting if LNK file already exists
  hidden              Toggle setting the 'hidden' file attribute
  help                Display more information on a specific command.
```

```
λ .\sharpLNK.exe create --filename "test.lnk" --target "C:\Windows\System32\cmd.exe" --arguments="/c calc.exe" --timestomp "2024-01-01 11:11:11,2024-02-02 22:22:22"

[+] Simplified Shortcut Information
        [*] Target: C:\Windows\System32\cmd.exe
        [*] Arguments: /c calc.exe
        [*] Working Directory: (null)
        [*] Window Style: SW_SHOWNORMAL
        [*] Flags: HasLinkTargetIDList, HasLinkInfo, HasRelativePath, HasArguments, IsUnicode
        [*] MachineID: COMMANDO

[+] Performed Timestomping
        [+] Set CreationTime to 01/01/2024 11:11:11
        [+] Set LastWriteTime to 02/02/2024 22:22:22
```

## modify
---
```
λ .\sharpLNK.exe modify --help
     _                        __      __
 ___| |__   __ _ _ __ _ __   / /   /\ \ \/\ /\
/ __| '_ \ / _` | '__| '_ \ / /   /  \/ / //_/
\__ \ | | | (_| | |  | |_) / /___/ /\  / __ \
|___/_| |_|\__,_|_|  | .__/\____/\_\ \/\/  \/
                     |_|

sharpLNK 1.0.0.0
by @j1ndoSH

  filename            Required. Path to the LNK file
  target              Path of Binary linked in LNK
  arguments           Commandline Arguments, recommended to use like
                      --arguments="-blah"
  workingdirectory    Directory where the LNK will start in
  machineid           Modify machineID (netBIOS name) contained within the LNK
  timestomp           Toggle setting the 'last modified timestamp' to the
                      original one (before modifying)
  hidden              Toggle setting the 'hidden' file attribute
  help                Display more information on a specific command.
```

## dump
---
```
λ .\sharpLNK.exe dump --help
     _                        __      __
 ___| |__   __ _ _ __ _ __   / /   /\ \ \/\ /\
/ __| '_ \ / _` | '__| '_ \ / /   /  \/ / //_/
\__ \ | | | (_| | |  | |_) / /___/ /\  / __ \
|___/_| |_|\__,_|_|  | .__/\____/\_\ \/\/  \/
                     |_|

sharpLNK 1.0.0.0
by @j1ndoSH

  filename    Required. Path to the LNK file
  stdout      (Default: true) Write data directly to stdout
  outfile     Path to outfile for dumped information
  verbose     Extract all the data within the LNK
  help        Display more information on a specific command.
```

# References
---
- [https://github.com/securifybv/ShellLink](https://github.com/securifybv/ShellLink)
- [https://github.com/commandlineparser/commandline](https://github.com/commandlineparser/commandline)
- A similar project [https://github.com/slyd0g/LNKMod](https://github.com/slyd0g/LNKMod) 
