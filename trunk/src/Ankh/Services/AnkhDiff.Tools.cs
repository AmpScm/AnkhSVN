// $Id$
//
// Copyright 2008-2009 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Scc.UI;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using SharpSvn;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;

namespace Ankh.Services
{
    partial class AnkhDiff
    {
        IList<AnkhDiffTool> _diffTools;
        IList<AnkhDiffTool> _mergeTools;
        IList<AnkhDiffTool> _patchTools;
        IList<AnkhDiffArgumentDefinition> _arguments;
        #region IAnkhDiffHandler Members

        public IList<AnkhDiffTool> DiffToolTemplates
        {
            get { return _diffTools ?? (_diffTools = GetDiffToolTemplates()); }
        }

        public IList<AnkhDiffTool> MergeToolTemplates
        {
            get { return _mergeTools ?? (_mergeTools = GetMergeToolTemplates()); }
        }

        public IList<AnkhDiffTool> PatchToolTemplates
        {
            get { return _patchTools ?? (_patchTools = GetPatchToolTemplates()); }
        }

        public IList<AnkhDiffArgumentDefinition> ArgumentDefinitions
        {
            get { return _arguments ?? (_arguments = GetArguments()); }
        }

        private IList<AnkhDiffArgumentDefinition> GetArguments()
        {
            List<AnkhDiffArgumentDefinition> args = new List<AnkhDiffArgumentDefinition>();
            args.Add(new AnkhDiffArgumentDefinition("Base", "Base Version (Diff, Merge)"));
            args.Add(new AnkhDiffArgumentDefinition("BaseName", "Title for Base (Diff, Merge)", "bname"));
            args.Add(new AnkhDiffArgumentDefinition("Mine", "My version (Diff, Merge)"));
            args.Add(new AnkhDiffArgumentDefinition("MineName", "Title for Mine (Diff, Merge)", "minename", "yname"));
            args.Add(new AnkhDiffArgumentDefinition("Theirs", "Theirs Version (Merge)"));
            args.Add(new AnkhDiffArgumentDefinition("TheirName", "Title for Theirs (Merge)", "tname", "theirsname"));
            args.Add(new AnkhDiffArgumentDefinition("Merged", "Merged Version (Merge)"));
            args.Add(new AnkhDiffArgumentDefinition("MergedName", "Title for Merged (Merge)", "mname"));

            args.Add(new AnkhDiffArgumentDefinition("PatchFile", "Patch file to apply"));
            args.Add(new AnkhDiffArgumentDefinition("ApplyToDir", "Directory to apply patch to"));

            args.Add(new AnkhDiffArgumentDefinition("AppPath(XXX)", "AppPath for registered tool XXX"));
            args.Add(new AnkhDiffArgumentDefinition("ProgramFiles", "System Program Files folder"));
            args.Add(new AnkhDiffArgumentDefinition("CommonProgramFiles", "Common Program Files folder"));
            args.Add(new AnkhDiffArgumentDefinition("HostProgramFiles", "Host architecture Program Files folder"));
            args.Add(new AnkhDiffArgumentDefinition("VSHome", "Visual Studio (shell) folder"));

            args.Sort(delegate(AnkhDiffArgumentDefinition d1, AnkhDiffArgumentDefinition d2)
            {
                return String.Compare(d1.Key, d2.Key, StringComparison.OrdinalIgnoreCase);
            });

            return new ReadOnlyCollection<AnkhDiffArgumentDefinition>(args);
        }

        IList<AnkhDiffTool> GetDiffToolTemplates()
        {
            List<AnkhDiffTool> tools = new List<AnkhDiffTool>();

            // Note: For TortoiseSVN use the host program files, as $(ProgramFiles) is invalid on X64 
            //       with TortoiseSVN integrated in explorer
            tools.Add(new DiffTool(this, "TortoiseMerge", "TortoiseMerge",
                RegistrySearch("SOFTWARE\\TortoiseSVN", "TMergePath")
                    ?? "$(HostProgramFiles)\\TortoiseSVN\\bin\\TortoiseMerge.exe",
                "/base:'$(Base)' /mine:'$(Mine)' /basename:'$(BaseName)' /minename:'$(MineName)'", true));

            tools.Add(new DiffTool(this, "AraxisMerge", "Araxis Merge",
                AppIdLocalServerSearch("Merge70.Application", "Compare.exe") 
                    ?? "$(ProgramFiles)\\Araxis\\Araxis Merge\\Compare.exe",
                "/wait /2 /title1:'$(BaseName)' /title2:'$(MineName)' '$(Base)' '$(Mine)'", true));

            tools.Add(new DiffTool(this, "DiffMerge", "SourceGear DiffMerge",
                RegistrySearch("SOFTWARE\\SourceGear\\SourceGear DiffMerge", "Location")
                    ?? "$(ProgramFiles)\\SourceGear\\DiffMerge\\DiffMerge.exe",
                "'$(Base)' '$(Mine)' /t1='$(BaseName)' /t2='$(MineName)'", true));

            tools.Add(new DiffTool(this, "KDiff3", "KDiff3",
                RegistrySearch("SOFTWARE\\KDiff3\\diff-ext", "diffcommand")
                    ?? "$(ProgramFiles)\\KDiff3\\KDiff3.exe",
                "'$(Base)' --fname '$(BaseName)' '$(Mine)' --fname '$(MineName)'", true));

            tools.Add(new DiffTool(this, "WinMerge", "WinMerge",
                RegistrySearch("SOFTWARE\\Thingamahoochie\\WinMerge", "Executable")
                    ?? "$(ProgramFiles)\\WinMerge\\WinMergeU.exe",
                "-e -x -ub -dl '$(BaseName)' -dr '$(MineName)' '$(base)' '$(mine)'", true));

            tools.Add(new DiffTool(this, "P4Merge", "Perforce Visual Merge",
                Path.Combine((RegistrySearch("SOFTWARE\\Perforce\\Environment", "P4INSTROOT")
                    ?? "$(ProgramFiles)\\Perforce"), "p4merge.exe"),
                    "'$(Base)' '$(Mine)'", true));

            LoadRegistryTools(DiffToolMode.Diff, tools);

            SortTools(tools);

            return new ReadOnlyCollection<AnkhDiffTool>(tools);
        }

        IList<AnkhDiffTool> GetMergeToolTemplates()
        {
            List<AnkhDiffTool> tools = new List<AnkhDiffTool>();

            tools.Add(new DiffTool(this, "TortoiseMerge", "TortoiseSVN TortoiseMerge",
                RegistrySearch("SOFTWARE\\TortoiseSVN", "TMergePath")
                    ?? "$(HostProgramFiles)\\TortoiseSVN\\bin\\TortoiseMerge.exe",
                "/base:'$(Base)' /theirs:'$(Theirs)' /mine:'$(Mine)' /merged:'$(Merged)'", true));

            tools.Add(new DiffTool(this, "AraxisMerge", "Araxis Merge",
                AppIdLocalServerSearch("Merge70.Application", "Compare.exe") 
                    ?? "$(ProgramFiles)\\Araxis\\Araxis Merge\\Compare.exe",
                "/wait /a2 /3 /title1:'$(MineName)' /title2:'$(MergedName)' " +
                    "/title3:'$(MineName)' '$(Mine)' '$(Base)' '$(Theirs)' '$(Merged)'", true));

            tools.Add(new DiffTool(this, "DiffMerge", "SourceGear DiffMerge",
                RegistrySearch("SOFTWARE\\SourceGear\\SourceGear DiffMerge", "Location")
                    ?? "$(ProgramFiles)\\SourceGear\\DiffMerge\\DiffMerge.exe",
                "/m /r='$(Merged)' '$(Mine)' '$(Base)' '$(Theirs)' " +
                    "/t1='$(MineName)' /t2='$(MergedName)' /t3='$(TheirName)'", true));

            tools.Add(new DiffTool(this, "KDiff3", "KDiff3",
                RegistrySearch("SOFTWARE\\KDiff3\\diff-ext", "diffcommand")
                    ?? "$(ProgramFiles)\\KDiff3\\KDiff3.exe",
                "-m '$(Base)' --L1 '$(BaseName)' '$(Theirs)' --L2 '$(TheirName)' " +
                    "'$(Mine)' --L3 '$(MineName)' -o '$(Merged)'", true));

            tools.Add(new DiffTool(this, "P4Merge", "Perforce Visual Merge",
                Path.Combine((RegistrySearch("SOFTWARE\\Perforce\\Environment", "P4INSTROOT")
                    ?? "$(ProgramFiles)\\Perforce"), "p4merge.exe"),
                    "'$(Theirs)' '$(Base)' '$(Mine)' '$(Merged)'", true));

            // WinMerge only has two way merge, so we diff theirs to mine to create merged
            tools.Add(new DiffTool(this, "WinMerge", "WinMerge",
                RegistrySearch("SOFTWARE\\Thingamahoochie\\WinMerge", "Executable")
                    ?? "$(ProgramFiles)\\WinMerge\\WinMergeU.exe",
                "-e -dl '$(TheirsName)' -dr '$(MineName)' " +
                    "'$(Theirs)' '$(Mine)' '$(Merged)'", true));

            LoadRegistryTools(DiffToolMode.Merge, tools);

            SortTools(tools);

            return new ReadOnlyCollection<AnkhDiffTool>(tools);
        }

        IList<AnkhDiffTool> GetPatchToolTemplates()
        {
            List<AnkhDiffTool> tools = new List<AnkhDiffTool>();

            tools.Add(new DiffTool(this, "TortoiseMerge", "TortoiseSVN TortoiseMerge",
                RegistrySearch("SOFTWARE\\TortoiseSVN", "TMergePath")
                    ?? "$(HostProgramFiles)\\TortoiseSVN\\bin\\TortoiseMerge.exe",
                "/diff:'$(PatchFile)' /patchpath:'$(ApplyToDir)'", true));

            LoadRegistryTools(DiffToolMode.Patch, tools);

            SortTools(tools);

            return new ReadOnlyCollection<AnkhDiffTool>(tools);
        }

        static string RegistrySearch(string key, string value)
        {
            using (RegistryKey k = Registry.LocalMachine.OpenSubKey(key))
            {
                if (k == null)
                    return null;

                string path = k.GetValue(value) as string;

                if (string.IsNullOrEmpty(path))
                    return null;

                return GetAppLocation(path);
            }
        }

        static string AppIdLocalServerSearch(string appId)
        {
            if (string.IsNullOrEmpty(appId))
                throw new ArgumentNullException("appId");

            Guid clsid;
            if (!ErrorHandler.Succeeded(NativeMethods.CLSIDFromProgID(appId, out clsid)))
                return null;

            using(RegistryKey rk = Registry.ClassesRoot.OpenSubKey("CLSID\\" + clsid.ToString("B") + "\\LocalServer32"))
            {
                if(rk == null)
                    return null;

                string app = rk.GetValue("") as string;

                if(!string.IsNullOrEmpty(app))
                    return GetAppLocation(app);                
            }

            using (RegistryKey rk = Registry.ClassesRoot.OpenSubKey("CLSID\\" + clsid.ToString("B") + "\\InprocServer32"))
            {
                if (rk == null)
                    return null;

                string app = rk.GetValue("") as string;

                if (!string.IsNullOrEmpty(app))
                    return GetAppLocation(app);
            }

            return null;
        }

        private static string GetAppLocation(string app)
        {
            if (string.IsNullOrEmpty(app))
                throw new ArgumentNullException("app");

            app = app.Trim();

            if (app.Length == 0)
                return null;

            if (app[0] == '\"')
            {
                int n = app.IndexOf('\"', 1);

                if (n > 0)
                    app = app.Substring(1, n - 1).Trim();
                else
                    app = app.Substring(1).Trim();
            }

            if (app.Contains("%"))
                app = Environment.ExpandEnvironmentVariables(app);

            return SvnTools.GetNormalizedFullPath(app);
        }

        static string AppIdLocalServerSearch(string appId, string relativePath)
        {
            string r = AppIdLocalServerSearch(appId);

            if (string.IsNullOrEmpty(r))
                return null;

            r = SvnTools.GetNormalizedFullPath(Path.Combine(Path.Combine(r, ".."), relativePath));

            if (File.Exists(r))
                return r;
            else
                return null;
        }

        private static void SortTools(List<AnkhDiffTool> tools)
        {
            tools.Sort(delegate(AnkhDiffTool t1, AnkhDiffTool t2)
            {
                if (t1.IsAvailable != t2.IsAvailable)
                    return t1.IsAvailable ? -1 : 1;

                return string.Compare(t1.Title, t2.Title, StringComparison.OrdinalIgnoreCase);
            });
        }

        #endregion

        string GetAppPath(string appName, DiffToolMode toolMode)
        {
            AnkhDiffTool tool = GetAppItem(appName, toolMode);

            if (tool != null)
                return tool.Program;

            return null;
        }

        string GetAppTemplate(string appName, DiffToolMode toolMode)
        {
            AnkhDiffTool tool = GetAppItem(appName, toolMode);

            if (tool != null)
                return string.Format("\"{0}\" {1}", tool.Program, tool.Arguments);

            return null;
        }

        AnkhDiffTool GetAppItem(string appName, DiffToolMode toolMode)
        {
            IList<AnkhDiffTool> tools;

            switch (toolMode)
            {
                case DiffToolMode.Diff:
                    tools = DiffToolTemplates;
                    break;
                case DiffToolMode.Merge:
                    tools = MergeToolTemplates;
                    break;
                case DiffToolMode.Patch:
                    tools = PatchToolTemplates;
                    break;
                default:
                    return null;
            }

            foreach (AnkhDiffTool tool in tools)
            {
                if (string.Equals(tool.Name, appName, StringComparison.OrdinalIgnoreCase))
                    return tool;
            }

            return null;
        }

        private void LoadRegistryTools(DiffToolMode diffToolMode, List<AnkhDiffTool> tools)
        {
            //throw new NotImplementedException();
        }

        sealed class DiffTool : AnkhDiffTool
        {
            readonly IAnkhServiceProvider _context;
            readonly string _name;
            readonly string _title;
            readonly string _program;
            readonly string _arguments;

            public DiffTool(IAnkhServiceProvider context, string name, string title, string program, string arguments)
            {
                if (context == null)
                    throw new ArgumentNullException("context");
                else if (string.IsNullOrEmpty(name))
                    throw new ArgumentNullException("name");
                else if (String.IsNullOrEmpty(program))
                    throw new ArgumentNullException("program");
                else if (String.IsNullOrEmpty(arguments))
                    throw new ArgumentNullException("arguments");

                _context = context;
                _name = name;
                _title = title ?? name;
                _program = program;
                _arguments = arguments;
            }

            public DiffTool(IAnkhServiceProvider context, string name, string title, string program, string arguments, bool replaceQuod)
                : this(context, name, title, program, replaceQuod ? (arguments ?? "").Replace('\'', '"') : arguments)
            { }

            public override string Name
            {
                get { return _name; }
            }

            public override string Title
            {
                get { return _title; }
            }

            bool? _isAvailable;
            public override bool IsAvailable
            {
                get
                {
                    if (!_isAvailable.HasValue)
                        try
                        {
                            AnkhDiff diff = _context.GetService<AnkhDiff>(typeof(IAnkhDiffHandler));
                            string program;
                            string args;
                            _isAvailable = diff.Substitute(Program, new AnkhDiffArgs(), DiffToolMode.None, out program, out args);
                        }
                        finally
                        {
                            if (!_isAvailable.HasValue)
                                _isAvailable = false;
                        }

                    return _isAvailable.Value;
                }
            }

            public override string Program
            {
                get { return _program; }
            }

            public override string Arguments
            {
                get { return _arguments; }
            }            
        }

        static partial class NativeMethods
        {
            [DllImport("ole32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
            public static extern int CLSIDFromProgID([MarshalAs(UnmanagedType.LPWStr)] string lpszProgID, out Guid pclsid);
        }
    }
}
