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
            args.Add(new AnkhDiffArgumentDefinition("ReadOnly", "Result should be readonly (Diff, Merge)"));

            args.Add(new AnkhDiffArgumentDefinition("PatchFile", "Patch file to apply"));
            args.Add(new AnkhDiffArgumentDefinition("ApplyToDir", "Directory to apply patch to"));

            args.Add(new AnkhDiffArgumentDefinition("AppPath(XXX)", "AppPath for registered tool XXX"));
            args.Add(new AnkhDiffArgumentDefinition("Var?'YYY':'ZZZ'", "If variable Var is defined then evaluate as 'YYY', else evaluate as 'ZZZ'"));
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
            tools.Add(new DiffTool(this, "TortoiseMerge", "TortoiseSVN TortoiseMerge",
                RegistrySearch("SOFTWARE\\TortoiseSVN", "TMergePath")
                    ?? "$(HostProgramFiles)\\TortoiseSVN\\bin\\TortoiseMerge.exe",
                "/base:'$(Base)' /mine:'$(Mine)' /basename:'$(BaseName)' /minename:'$(MineName)'", true));

            tools.Add(new DiffTool(this, "AraxisMerge", "Araxis Merge",
                RelativePath(
                    AppIdLocalServerSearch("Merge7.SVNFS") ??
                    AppIdLocalServerSearch("Merge70.Application") ??
                    ShellOpenSearch("Merge.Comparison.7"), "Compare.exe")
                        ?? "$(ProgramFiles)\\Araxis\\Araxis Merge\\Compare.exe",
                "/wait /2 /title1:'$(BaseName)' /title2:'$(MineName)' '$(Base)' '$(Mine)'", true));

            tools.Add(new DiffTool(this, "DiffMerge", "SourceGear DiffMerge",
                RegistrySearch("SOFTWARE\\SourceGear\\Common\\DiffMerge\\Installer", "Location")
                    ?? RegistrySearch("SOFTWARE\\SourceGear\\SourceGear DiffMerge", "Location")
                    ?? "$(ProgramFiles)\\SourceGear\\DiffMerge\\DiffMerge.exe",
                "'$(Base)' '$(Mine)' /t1='$(BaseName)' /t2='$(MineName)' "
                + "$(ReadOnly?'/ro2')"
                , true));

            tools.Add(new DiffTool(this, "KDiff3", "KDiff3",
                RegistrySearch("SOFTWARE\\KDiff3\\diff-ext", "diffcommand")
                    ?? "$(ProgramFiles)\\KDiff3\\KDiff3.exe",
                "'$(Base)' --fname '$(BaseName)' '$(Mine)' --fname '$(MineName)'", true));

            tools.Add(new DiffTool(this, "WinMerge", "WinMerge",
                RegistrySearch("SOFTWARE\\Thingamahoochie\\WinMerge", "Executable")
                    ?? "$(ProgramFiles)\\WinMerge\\WinMergeU.exe",
                "-e -u -wl $(ReadOnly?'-wr':'') -dl '$(BaseName)' -dr '$(MineName)' '$(base)' '$(mine)'", true));

            tools.Add(new DiffTool(this, "P4Merge", "Perforce Visual Merge",
                Path.Combine((RegistrySearch("SOFTWARE\\Perforce\\Environment", "P4INSTROOT")
                    ?? "$(ProgramFiles)\\Perforce"), "p4merge.exe"),
                    "'$(Base)' '$(Mine)'", true));

            tools.Add(new DiffTool(this, "BeyondCompare", "Beyond Compare",
                RelativePath(ShellOpenSearch("BeyondCompare.Snapshot"), "BComp.exe")
                    ?? "$(ProgramFiles)\\Beyond Compare 3\\BComp.exe",
                "'$(Base)' '$(Mine)' /fv /title1='$(BaseName)' /title2='$(MineName)' /leftreadonly", true));

            tools.Add(new DiffTool(this, "ECMerge", "Elli� Computing Merge",
                RegistrySearch("SOFTWARE\\Elli� Computing\\Merge", "Path")
                    ?? "$(ProgramFiles)\\Elli� Computing\\Merge\\guimerge.exe",
                "'$(Base)' '$(Mine)' --mode=diff2 --title1='$(BaseName)' --title2='$(MineName)'", true));

            tools.Add(new DiffTool(this, "ExamDiff", "PrestoSoft ExamDiff",
                RegistrySearch("SOFTWARE\\PrestoSoft\\ExamDiff", "ExePath")
                    ?? UserRegistrySearch("SOFTWARE\\PrestoSoft\\ExamDiff", "ExePath")
                    ?? "$(ProgramFiles)\\ExamDiff\\ExamDiff.exe",
                "'$(Base)' '$(Mine)'", true));

            tools.Add(new DiffTool(this, "CompareIt", "Compare It!",
                RegistrySearch("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Compare It!_is1", "DisplayIcon")
                    ?? "$(ProgramFiles)\\Compare It!\\wincmp.exe",
                "'$(Base)' '/=$(BaseName)' '$(Mine)' '/=$(MineName)'", true));

            tools.Add(new DiffTool(this, "SlickEdit", "SlickEdit",
                RelativePath(RegistrySearch("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\App Paths\\vs.exe", ""), "VSDiff.exe")
                    ?? "$(ProgramFiles)\\SlickEdit\\win\\VSDiff.exe",
                "-r1 $(ReadOnly?'-r2 ')'$(Base)' '$(Mine)'", true));

            tools.Add(new DiffTool(this, "DevartCodeCompare", "Devart CodeCompare",
                RelativePath(ClsIdServerSearch(new Guid("{8C2B8E72-E398-482D-8609-FC606231624C}")), "CodeCompare.exe")
                    ?? RelativePath(RegistrySearch("SOFTWARE\\Devart\\Code Compare", "HelpFile"), "CodeCompare.exe")
                    ?? RelativePath(RegistrySearch("SOFTWARE\\Devart\\CodeCompare", "HelpFile"), "CodeCompare.exe")
                    ?? "$(HostProgramFiles)\\Devart\\Code Compare\\CodeCompare.exe",
                "/WAIT /SC=SVN /t1='$(BaseName)' /t2='$(MineName)' '$(Base)' '$(Mine)'", true));

            tools.Add(new DiffTool(this, "ComparePlusPlus", "Coodesoft Compare++",
                RelativePath(AppIdLocalServerSearch("CompareEnter.Connect"), "Compare++.exe")
                    ?? "$(HostProgramFiles)\\Coode Software\\Compare++\\Compare++.exe",
                "'$(Base)' '$(Mine)'", true));

            tools.Add(new DiffTool(this, "SemanticMerge", "Semanticmerge",
                SubPath(RegistrySearch("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\SemanticMerge", "InstallLocation")
                             ?? UserRegistrySearch("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\SemanticMerge", "InstallLocation"),
                             "semanticmergetool.exe")
                    ?? "$(LocalAppData)\\PlasticSCM4\\semanticmerge\\semanticmergetool.exe",
                "--source='$(Base)' --destination='$(Mine)' --srcsymbolicname='$(BaseName)' --dstsymbolicname='$(MineName)'", true));

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
                "/base:'$(Base)' /theirs:'$(Theirs)' /mine:'$(Mine)' /merged:'$(Merged)' " +
                "/basename:'$(BaseName)' /theirsname:'$(TheirsName)' /minename:'$(MineName)' "+
                "/mergedname:'$(MergedName)'", true));

            tools.Add(new DiffTool(this, "AraxisMerge", "Araxis Merge",
                RelativePath(
                    AppIdLocalServerSearch("Merge7.SVNFS") ??
                    AppIdLocalServerSearch("Merge70.Application") ??
                    ShellOpenSearch("Merge.Comparison.7"), "Compare.exe")
                        ?? "$(ProgramFiles)\\Araxis\\Araxis Merge\\Compare.exe",
                "/wait /a2 /3 /title1:'$(MineName)' /title2:'$(MergedName)' " +
                    "/title3:'$(MineName)' '$(Mine)' '$(Base)' '$(Theirs)' '$(Merged)'", true));

            tools.Add(new DiffTool(this, "DiffMerge", "SourceGear DiffMerge",
                RegistrySearch("SOFTWARE\\SourceGear\\Common\\DiffMerge\\Installer", "Location")
                    ?? RegistrySearch("SOFTWARE\\SourceGear\\SourceGear DiffMerge", "Location")
                    ?? "$(ProgramFiles)\\SourceGear\\DiffMerge\\DiffMerge.exe",
                "/m /r='$(Merged)' '$(Mine)' '$(Base)' '$(Theirs)' " +
                    "/t1='$(MineName)' /t2='$(MergedName)' /t3='$(TheirName)'" +
                    "$(ResolveConflictOn='0')", true));

            tools.Add(new DiffTool(this, "KDiff3", "KDiff3",
                RegistrySearch("SOFTWARE\\KDiff3\\diff-ext", "diffcommand")
                    ?? "$(ProgramFiles)\\KDiff3\\KDiff3.exe",
                "-m '$(Base)' --L1 '$(BaseName)' '$(Theirs)' --L2 '$(TheirName)' " +
                    "'$(Mine)' --L3 '$(MineName)' -o '$(Merged)'", true));

            // WinMerge only has two way merge, so we diff theirs to mine to create merged
            tools.Add(new DiffTool(this, "WinMerge", "WinMerge (2-Way)",
                RegistrySearch("SOFTWARE\\Thingamahoochie\\WinMerge", "Executable")
                    ?? "$(ProgramFiles)\\WinMerge\\WinMergeU.exe",
                "-e -u -wl -dl '$(TheirsName)' -dr '$(MineName)' " +
                    "'$(Theirs)' '$(Mine)' '$(Merged)'", true));

            tools.Add(new DiffTool(this, "P4Merge", "Perforce Visual Merge",
                Path.Combine((RegistrySearch("SOFTWARE\\Perforce\\Environment", "P4INSTROOT")
                    ?? "$(ProgramFiles)\\Perforce"), "p4merge.exe"),
                    "'$(Base)' '$(Theirs)' '$(Mine)' '$(Merged)'", true));

            tools.Add(new DiffTool(this, "BeyondCompare3W", "Beyond Compare Pro (3-Way)",
                RelativePath(ShellOpenSearch("BeyondCompare.Snapshot"), "BComp.exe")
                    ?? "$(ProgramFiles)\\Beyond Compare 3\\BComp.exe",
                "'$(Mine)' '$(Theirs)' '$(Base)' '$(Merged)' " +
                "/title1='$(MineName)' /title2='$(TheirsName)' " +
                "/title3='$(BaseName)' /title4='$(MergedName)' ", true));

            tools.Add(new DiffTool(this, "BeyondCompare2W", "Beyond Compare (2-Way)",
                RelativePath(ShellOpenSearch("BeyondCompare.Snapshot"), "BComp.exe")
                    ?? "$(ProgramFiles)\\Beyond Compare 3\\BComp.exe",
                "'$(Mine)' '$(Theirs)' /mergeoutput='$(Merged)' " +
                "/title1='$(MineName)' /title2='$(TheirsName)' ", true));

            tools.Add(new DiffTool(this, "ECMerge", "Elli� Computing Merge",
                RegistrySearch("SOFTWARE\\Elli� Computing\\Merge", "Path")
                    ?? "$(ProgramFiles)\\Elli� Computing\\Merge\\guimerge.exe",
                "'$(Base)' '$(Mine)' '$(Theirs)' --to='$(Merged)' --mode=merge3 " +
                "--title0='$(BaseName)' --title1='$(MineName)' --title2='$(TheirsName)' " +
                "--to-title='$(MergedName)'", true));

            tools.Add(new DiffTool(this, "SlickEdit", "SlickEdit",
                RelativePath(RegistrySearch("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\App Paths\\vs.exe", ""), "VSMerge.exe")
                    ?? "$(ProgramFiles)\\SlickEdit\\win\\vsmerge.exe",
                "-smart '$(Base)' '$(Mine)' '$(Theirs)' '$(Merged)'", true));

            tools.Add(new DiffTool(this, "DevartCodeCompare", "Devart CodeCompare",
                RelativePath(ClsIdServerSearch(new Guid("{8C2B8E72-E398-482D-8609-FC606231624C}")), "CodeCompare.exe")
                    ?? SubPath(RegistrySearch("SOFTWARE\\Devart\\Code Compare", "HelpFile"), "CodeMerge.exe")
                    ?? SubPath(RegistrySearch("SOFTWARE\\Devart\\CodeCompare", "HelpFile"), "CodeMerge.exe")
                    ?? "$(HostProgramFiles)\\Devart\\Code Compare\\CodeMerge.exe",
                "/WAIT /SC=SVN /REMOVEFILES '/BF=$(Base)' '/MF=$(Mine)' '/MT=$(MineName)' " +
                "'/TF=$(Theirs)' '/TT=$(TheirsName)' '/RF=$(Merged)'", true));

            tools.Add(new DiffTool(this, "SemanticMerge", "Semanticmerge",
                SubPath(RegistrySearch("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\SemanticMerge", "InstallLocation")
                        ?? UserRegistrySearch("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\SemanticMerge", "InstallLocation"),
                        "semanticmergetool.exe")
                ?? "$(LocalAppData)\\PlasticSCM4\\semanticmerge\\semanticmergetool.exe",
                "--base='$(Base)' --source='$(Theirs)' --destination='$(Mine)' --result='$(Merged)' " +
                "--basesymbolicname='$(BaseName)' --srcsymbolicname='$(TheirsName)' --dstsymbolicname='$(MineName)'", true));


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
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");
            else if (value == null) // Allow ""
                throw new ArgumentNullException("value");

            using (RegistryKey k = Registry.LocalMachine.OpenSubKey(key, false))
            {
                if (k == null)
                    return null;

                string path = k.GetValue(value) as string;

                if (string.IsNullOrEmpty(path))
                    return null;

                return GetAppLocation(path);
            }
        }

        static string UserRegistrySearch(string key, string value)
        {
            using (RegistryKey k = Registry.CurrentUser.OpenSubKey(key, false))
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
            if (!VSErr.Succeeded(NativeMethods.CLSIDFromProgID(appId, out clsid)))
                return null;

            return ClsIdServerSearch(clsid);
        }

        static string ClsIdServerSearch(Guid clsid)
        {
            using (RegistryKey rk = Registry.ClassesRoot.OpenSubKey("CLSID\\" + clsid.ToString("B") + "\\LocalServer32", false))
            {
                if (rk != null)
                {
                    string app = rk.GetValue("") as string;

                    if (!string.IsNullOrEmpty(app))
                        return GetAppLocation(app);
                }
            }

            using (RegistryKey rk = Registry.ClassesRoot.OpenSubKey("CLSID\\" + clsid.ToString("B") + "\\InprocServer32", false))
            {
                if (rk != null)
                {
                    string app = rk.GetValue("") as string;

                    if (!string.IsNullOrEmpty(app))
                        return GetAppLocation(app);
                }
            }

            return null;
        }

        string ShellOpenSearch(string className)
        {
            using (RegistryKey rk = Registry.ClassesRoot.OpenSubKey(className + "\\shell\\open\\command", false))
            {
                if (rk == null)
                    return null;

                string cmdLine = rk.GetValue("") as string;

                if (string.IsNullOrEmpty(cmdLine))
                    return null;

                string program, args;
                if (!SvnTools.TrySplitCommandLine(cmdLine, SubstituteEmpty, out program, out args))
                    return null;
                else
                    return program;
            }
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

        static string RelativePath(string origin, string relativePath)
        {
            if (string.IsNullOrEmpty(origin))
                return null;
            else if (string.IsNullOrEmpty(relativePath))
                return origin;

            string r = SvnTools.GetNormalizedFullPath(Path.Combine(Path.Combine(origin, ".."), relativePath));

            if (File.Exists(r))
                return r;
            else
                return null;
        }

        static string SubPath(string origin, string relativePath)
        {
            if (string.IsNullOrEmpty(origin))
                return null;
            else if (string.IsNullOrEmpty(relativePath))
                return origin;

            string r = SvnTools.GetNormalizedFullPath(Path.Combine(origin, relativePath));

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
            using (RegistryKey rk = Registry.LocalMachine.OpenSubKey("SOFTWARE\\AnkhSVN\\AnkhSVN\\CurrentVersion\\Tools\\" + diffToolMode.ToString(), false))
            {
                if (rk == null)
                    return;

                foreach (string name in rk.GetSubKeyNames())
                {
                    using(RegistryKey sk = rk.OpenSubKey(name, false))
                    {
                        string title = sk.GetValue("") as string ?? name;
                        string program = sk.GetValue("Program") as string;
                        string arguments = sk.GetValue("Arguments") as string;

                        if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(program) && !string.IsNullOrEmpty(arguments))
                        {
                            bool found = false;
                            foreach (AnkhDiffTool dt in tools)
                            {
                                if (dt.Name == name)
                                {
                                    found = true;
                                    break;
                                }
                            }

                            if (!found)
                                tools.Add(new DiffTool(Context, name, title, program, arguments));
                        }
                    }
                }
            }
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
                            _isAvailable = SvnTools.TryFindApplication(diff.SubstituteArguments(Program, new AnkhDiffArgs(), DiffToolMode.None), out program);
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
