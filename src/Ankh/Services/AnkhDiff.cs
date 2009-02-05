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
using System.IO;
using System.Diagnostics;
using Ankh.UI;
using System.Text.RegularExpressions;
using Ankh.Scc;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using SharpSvn;
using Ankh.VS;

namespace Ankh.Services
{
    [GlobalService(typeof(IAnkhDiffHandler))]
    partial class AnkhDiff : AnkhService, IAnkhDiffHandler
    {
        public AnkhDiff(IAnkhServiceProvider context)
            : base(context)
        {
        }

        IFileStatusCache _statusCache;
        IFileStatusCache Cache
        {
            get { return _statusCache ?? (_statusCache = GetService<IFileStatusCache>()); }
        }

        /// <summary>
        /// Gets path to the diff executable while taking care of config file settings.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>The exe path.</returns>
        protected string GetDiffPath(DiffMode mode)
        {
            IAnkhConfigurationService cs = GetService<IAnkhConfigurationService>();

            switch (mode)
            {
                case DiffMode.PreferInternal:
                    return null;
                default:
                    return cs.Instance.DiffExePath;
            }
        }

        public bool RunDiff(AnkhDiffArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            else if (!args.Validate())
                throw new ArgumentException("Arguments not filled correctly", "args");

            string diffApp = this.GetDiffPath(args.Mode);


            if (string.IsNullOrEmpty(diffApp))
                return RunInternalDiff(args);

            string program;
            string arguments;
            if (!Substitute(diffApp, args, DiffToolMode.Diff, out program, out arguments))
            {
                new AnkhMessageBox(Context).Show(string.Format("Can't find diff program '{0}'", program ?? diffApp));
                return false;
            }

            // TODO: Maybe Handle file saves and program exits

            Process p = new Process();
            p.StartInfo = new ProcessStartInfo(program, arguments);

            string mergedFile = args.MineFile;

            DiffToolMonitor monitor = null;
            if (!string.IsNullOrEmpty(mergedFile))
            {
                monitor = new DiffToolMonitor(Context, mergedFile, false);

                p.EnableRaisingEvents = true;
                monitor.Register(p);
            }

            bool started = false;
            try
            {
                return started = p.Start();
            }
            finally
            {
                if (!started)
                {
                    if (monitor != null)
                        monitor.Dispose();
                }
            }
        }

        /// <summary>
        /// Gets path to the diff executable while taking care of config file settings.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>The exe path.</returns>
        protected string GetMergePath(DiffMode mode)
        {
            IAnkhConfigurationService cs = GetService<IAnkhConfigurationService>();

            switch (mode)
            {
                case DiffMode.PreferInternal:
                    return null;
                default:
                    return cs.Instance.MergeExePath;
            }
        }

        public bool RunMerge(AnkhMergeArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            else if (!args.Validate())
                throw new ArgumentException("Arguments not filled correctly", "args");

            string diffApp = this.GetMergePath(args.Mode);

            if (string.IsNullOrEmpty(diffApp))
            {
                new AnkhMessageBox(Context).Show("Please specify a merge tool in Tools->Options->SourceControl->Subversion", "AnkhSVN - No visual merge tool is available");

                return false;
            }

            string program;
            string arguments;
            if (!Substitute(diffApp, args, DiffToolMode.Merge, out program, out arguments))
            {
                new AnkhMessageBox(Context).Show(string.Format("Can't find merge program '{0}'", program));
                return false;
            }

            Process p = new Process();
            p.StartInfo = new ProcessStartInfo(program, arguments);

            string mergedFile = args.MergedFile;

            DiffToolMonitor monitor = null;
            if (!string.IsNullOrEmpty(mergedFile))
            {
                monitor = new DiffToolMonitor(Context, mergedFile, false);

                p.EnableRaisingEvents = true;
                monitor.Register(p);
            }

            bool started = false;
            try
            {
                return started = p.Start();
            }
            finally
            {
                if (!started)
                {
                    if (monitor != null)
                        monitor.Dispose();
                }
            }
        }

        protected string GetPatchPath(DiffMode mode)
        {
            IAnkhConfigurationService cs = GetService<IAnkhConfigurationService>();

            switch (mode)
            {
                case DiffMode.PreferInternal:
                    return null;
                default:
                    return cs.Instance.PatchExePath;
            }
        }

        public bool RunPatch(AnkhPatchArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            else if (!args.Validate())
                throw new ArgumentException("Arguments not filled correctly", "args");

            string diffApp = GetPatchPath(args.Mode);

            if (string.IsNullOrEmpty(diffApp))
            {
                new AnkhMessageBox(Context).Show("Please specify a merge tool in Tools->Options->SourceControl->Subversion", "AnkhSVN - No visual merge tool is available");

                return false;
            }

            string program;
            string arguments;
            if (!Substitute(diffApp, args, DiffToolMode.Patch, out program, out arguments))
            {
                new AnkhMessageBox(Context).Show(string.Format("Can't find patch program '{0}'", program));
                return false;
            }

            Process p = new Process();
            p.StartInfo = new ProcessStartInfo(program, arguments);

            string applyTo = args.ApplyTo;

            DiffToolMonitor monitor = null;
            if (applyTo != null)
            {
                monitor = new DiffToolMonitor(Context, applyTo, true);

                p.EnableRaisingEvents = true;
                monitor.Register(p);
            }

            bool started = false;
            try
            {
                return started = p.Start();
            }
            finally
            {
                if (!started)
                {
                    if (monitor != null)
                        monitor.Dispose();
                }
            }
        }

        public SvnUriTarget GetCopyOrigin(SvnItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            // TODO: Maybe handle cases where the parent was copied instead of the child?

            SvnUriTarget copiedFrom = null;
            using (SvnClient client = GetService<ISvnClientPool>().GetNoUIClient())
            {
                SvnInfoArgs ia = new SvnInfoArgs();
                ia.ThrowOnError = false;
                ia.Depth = SvnDepth.Empty;

                client.Info(item.FullPath, ia,
                    delegate(object sender, SvnInfoEventArgs ee)
                    {
                        if (ee.CopyFromUri != null)
                        {
                            copiedFrom = new SvnUriTarget(ee.CopyFromUri, ee.CopyFromRevision);
                        }
                    });
            }
            return copiedFrom;
        }

        sealed class DiffToolMonitor : AnkhService, IVsFileChangeEvents
        {
            uint _cookie;
            readonly string _toMonitor;
            readonly bool _monitorDir;

            public DiffToolMonitor(IAnkhServiceProvider context, string monitor, bool monitorDir)
                : base(context)
            {
                if (string.IsNullOrEmpty(monitor))
                    throw new ArgumentNullException("monitor");
                else if (!SvnItem.IsValidPath(monitor))
                    throw new ArgumentOutOfRangeException("monitor");

                _monitorDir = monitorDir;
                _toMonitor = monitor;

                IVsFileChangeEx fx = context.GetService<IVsFileChangeEx>(typeof(SVsFileChangeEx));

                _cookie = 0;
                if (fx == null)
                { }
                else if (!_monitorDir)
                {
                    if (!ErrorHandler.Succeeded(fx.AdviseFileChange(monitor,
                            (uint)(_VSFILECHANGEFLAGS.VSFILECHG_Time | _VSFILECHANGEFLAGS.VSFILECHG_Size
                            | _VSFILECHANGEFLAGS.VSFILECHG_Add | _VSFILECHANGEFLAGS.VSFILECHG_Del
                            | _VSFILECHANGEFLAGS.VSFILECHG_Attr),
                            this,
                            out _cookie)))
                    {
                        _cookie = 0;
                    }
                }
                else
                {
                    if (!ErrorHandler.Succeeded(fx.AdviseDirChange(monitor, 1, this, out _cookie)))
                    {
                        _cookie = 0;
                    }
                }
            }

            public void Dispose()
            {
                if (_cookie != 0)
                {
                    uint ck = _cookie;
                    _cookie = 0;

                    IVsFileChangeEx fx = GetService<IVsFileChangeEx>(typeof(SVsFileChangeEx));

                    if (fx != null)
                    {
                        if (!_monitorDir)
                            fx.UnadviseFileChange(ck);
                        else
                            fx.UnadviseDirChange(ck);
                    }
                }
            }

            public void Register(Process p)
            {
                p.Exited += new EventHandler(OnExited);
            }

            void OnExited(object sender, EventArgs e)
            {
                Dispose();

                if (_monitorDir)
                {
                    // TODO: Schedule status for all changed files
                }
            }

            public int DirectoryChanged(string pszDirectory)
            {
                IFileStatusCache fsc = GetService<IFileStatusCache>();

                if (fsc != null)
                {
                    fsc.MarkDirtyRecursive(SvnTools.GetNormalizedFullPath(pszDirectory));
                }

                return VSConstants.S_OK;
            }

            public int FilesChanged(uint cChanges, string[] rgpszFile, uint[] rggrfChange)
            {
                if (rgpszFile != null)
                {
                    foreach (string file in rgpszFile)
                    {
                        if (string.Equals(file, _toMonitor, StringComparison.OrdinalIgnoreCase))
                        {
                            IFileStatusMonitor m = GetService<IFileStatusMonitor>();

                            if (m != null)
                                m.ExternallyChanged(_toMonitor);

                            break;
                        }
                    }
                }

                return VSConstants.S_OK;
            }
        }


        #region Argument Substitution support

        enum DiffToolMode
        {
            None,
            Diff,
            Merge,
            Patch
        }

        private bool Substitute(string reference, AnkhDiffToolArgs args, DiffToolMode toolMode, out string program, out string arguments)
        {
            if (string.IsNullOrEmpty(reference))
                throw new ArgumentNullException("reference");
            else if (args == null)
                throw new ArgumentNullException("args");

            // Ok: We received a string with a program and arguments and windows 
            // wants a program and arguments separated. Let's find the program before substituting

            reference = reference.TrimStart();

            program = null;
            arguments = null;

            string app;
            if (!string.IsNullOrEmpty(app = AnkhDiffTool.GetToolNameFromTemplate(reference)))
            {
                // We have a predefined template. Just use it
                AnkhDiffTool tool = GetAppItem(app, toolMode);

                if (tool == null)
                    return false;
                else if (!tool.IsAvailable)
                    return false;

                program = SubstituteArguments(tool.Program, args, toolMode);
                arguments = SubstituteArguments(tool.Arguments, args, toolMode);

                return !String.IsNullOrEmpty(program) && File.Exists(program);
            }
            else if (!TrySplitPath(reference, out program, out arguments))
                return false;

            program = SubstituteArguments(program, args, toolMode);
            arguments = SubstituteArguments(arguments, args, toolMode);

            return true;
        }

        static readonly AnkhDiffArgs EmptyDiffArgs = new AnkhDiffArgs();
        private bool TrySplitPath(string cmdline, out string program, out string arguments)
        {
            if(cmdline == null)
                throw new ArgumentNullException("cmdline");

            program = arguments = null;

            cmdline = cmdline.TrimStart();

            if (cmdline.StartsWith("\""))
            {
                // Ok: The easy way:
                int nEnd = cmdline.IndexOf('\"', 1);

                if (nEnd < 0)
                    return false; // Invalid string!

                program = cmdline.Substring(1, nEnd - 1);
                arguments = cmdline.Substring(nEnd + 1).TrimStart();
                return true;
            }

            // We use the algorithm as documented by CreateProcess() in MSDN
            // http://msdn2.microsoft.com/en-us/library/ms682425(VS.85).aspx
            char[] spacers = new char[] { ' ', '\t' };
            int nFrom = 0;
            int nTok = -1;

            string file;
            
            while ((nFrom < cmdline.Length) &&
                (0 <= (nTok = cmdline.IndexOfAny(spacers, nFrom))))
            {
                program = cmdline.Substring(0, nTok);

                file = SubstituteArguments(program, EmptyDiffArgs, DiffToolMode.None);

                if (!string.IsNullOrEmpty(file) && File.Exists(file))
                {
                    arguments = cmdline.Substring(nTok + 1).TrimStart();
                    return true;
                }
                else
                    nFrom = nTok + 1;
            }

            if (nTok < 0 && nFrom <= cmdline.Length)
            {
                file = SubstituteArguments(cmdline, EmptyDiffArgs, DiffToolMode.None);

                if (!string.IsNullOrEmpty(file) && File.Exists(file))
                {
                    program = file;
                    arguments = "";
                    return true;
                }
            }


            return false;
        }

        Regex _re;

        private string SubstituteArguments(string arguments, AnkhDiffToolArgs diffArgs, DiffToolMode toolMode)
        {
            if (diffArgs == null)
                throw new ArgumentNullException("diffArgs");

            if(_re == null)
                _re = new Regex(@"(\%(?<pc>[a-zA-Z0-9()_]+)(\%|\b))|(\$\((?<vs>[a-zA-Z0-9_-]*)(\((?<arg>[a-zA-Z0-9_-]*)\))?\))" +
                "|(\\$\\(\\?(?<if>[a-zA-Z0-9_-]*):'(?<ifbody>([^']|(''))*)'\\))");

            return _re.Replace(arguments, new Replacer(this, diffArgs, toolMode).Replace);
        }

        sealed class Replacer
        {
            readonly AnkhDiff _context;
            readonly AnkhDiffToolArgs _toolArgs;
            readonly AnkhDiffArgs _diffArgs;
            readonly AnkhMergeArgs _mergeArgs;
            readonly AnkhPatchArgs _patchArgs;
            readonly DiffToolMode _toolMode;

            public Replacer(AnkhDiff context, AnkhDiffToolArgs args, DiffToolMode toolMode)
            {
                if (context == null)
                    throw new ArgumentNullException("context");
                else if (args == null)
                    throw new ArgumentNullException("args");

                _context = context;
                _toolArgs = args;
                _diffArgs = args as AnkhDiffArgs;
                _mergeArgs = args as AnkhMergeArgs;
                _patchArgs = args as AnkhPatchArgs;
                _toolMode = toolMode;
            }

            AnkhDiffArgs DiffArgs
            {
                get { return _diffArgs; }
            }

            AnkhMergeArgs MergeArgs
            {
                get { return _mergeArgs; }
            }

            AnkhPatchArgs PatchArgs
            {
                get { return _patchArgs; }
            }

            public string Replace(Match match)
            {
                string key;
                string value;
                bool vsStyle = true;

                if (match.Groups["pc"].Length > 1)
                {
                    vsStyle = false;
                    key = match.Groups["pc"].Value;
                }
                else if (match.Groups["vs"].Length > 1)
                    key = match.Groups["vs"].Value;
                else if (match.Groups["if"].Length > 1)
                {
                    if (!TryGetValue(match.Groups["if"].Value, true, "", out value))
                        return "";
                    
                    value = match.Groups["ifbody"].Value.Replace("''", "'");

                    return _context.SubstituteArguments(value, _diffArgs, _toolMode);
                }
                else
                    return match.Value; // Don't replace if not matched

                string arg = match.Groups["arg"].Value ?? "";
                TryGetValue(key, vsStyle, arg, out value);

                return value ?? "";
            }

            bool TryGetValue(string key, bool vsStyle, string arg, out string value)
            {
                if (key == null)
                    throw new ArgumentNullException("key");

                key = key.ToUpperInvariant();
                value = null;

                string v;
                switch (key)
                {
                    case "BASE":
                        if (DiffArgs != null)
                            value = DiffArgs.BaseFile;
                        else
                            return false;
                        break;
                    case "BNAME":
                    case "BASENAME":
                        if (DiffArgs != null)
                            value = DiffArgs.BaseTitle ?? Path.GetFileName(DiffArgs.BaseFile);
                        else
                            return false;
                        break;
                    case "MINE":
                        if (DiffArgs != null)
                            value = DiffArgs.MineFile;
                        else
                            return false;
                        break;
                    case "YNAME":
                    case "MINENAME":
                        if (DiffArgs != null)
                            value = DiffArgs.MineTitle ?? Path.GetFileName(DiffArgs.MineFile);
                        else
                            return false;
                        break;

                    case "THEIRS":
                        if (MergeArgs != null)
                            value = MergeArgs.TheirsFile;
                        else
                            return false;
                        break;
                    case "TNAME":
                    case "THEIRNAME":
                    case "THEIRSNAME":
                        if (MergeArgs != null)
                            value = MergeArgs.TheirsTitle ?? Path.GetFileName(MergeArgs.TheirsFile);
                        else
                            return false;
                        break;
                    case "MERGED":
                        if (MergeArgs != null)
                            value = MergeArgs.MergedFile;
                        else
                            return false;
                        break;
                    case "MERGEDNAME":
                    case "MNAME":
                        if (MergeArgs != null)
                            value = MergeArgs.MergedTitle ?? Path.GetFileName(MergeArgs.MergedFile);
                        else
                            return false;
                        break;

                    case "PATCHFILE":
                        if (PatchArgs != null)
                            value = PatchArgs.PatchFile;
                        else
                            return false;
                        break;
                    case "APPLYTODIR":
                        if (PatchArgs != null)
                            value = PatchArgs.ApplyTo;
                        else
                            return false;
                        break;
                    case "APPPATH":
                        v = _context.GetAppPath(arg, _toolMode);
                        value = _context.SubstituteArguments(v ?? "", DiffArgs, _toolMode);
                        break;
                    case "APPTEMPLATE":
                        v = _context.GetAppTemplate(arg, _toolMode);
                        value = _context.SubstituteArguments(v ?? "", DiffArgs, _toolMode);
                        break;
                    case "PROGRAMFILES":
                        // Return the environment variable if using environment variable style
                        value = (vsStyle ? null : Environment.GetEnvironmentVariable(key)) ?? Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                        break;
                    case "COMMONPROGRAMFILES":
                        // Return the environment variable if using environment variable style
                        value = (vsStyle ? null : Environment.GetEnvironmentVariable(key)) ?? Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles);
                        break;
                    case "HOSTPROGRAMFILES":
                        // Use the WOW64 program files directory if available, otherwise just program files
                        value = Environment.GetEnvironmentVariable("PROGRAMW6432") ?? Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                        break;
                    case "VSHOME":
                        IVsSolution sol = _context.GetService<IVsSolution>(typeof(SVsSolution));
                        if (sol == null)
                            return false;
                        object val;
                        if (ErrorHandler.Succeeded(sol.GetProperty((int)__VSSPROPID.VSSPROPID_InstallDirectory, out val)))
                            value = val as string;
                        return true;
                    default:
                        // Just replace with "" if unknown
                        v = Environment.GetEnvironmentVariable(key);
                        if (!string.IsNullOrEmpty(v))
                            value = v;
                        return (value != null);
                }

                return true;
            }
        }

        #endregion

        public string GetTitle(SvnItem target, SvnRevision revision)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            if (revision == null)
                throw new ArgumentNullException("revision");

            return GetTitle(target.Name, revision);
        }

        public string GetTitle(SvnTarget target, SvnRevision revision)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            if (revision == null)
                throw new ArgumentNullException("revision");

            return GetTitle(target.FileName, revision);
        }

        static string GetTitle(string fileName, SvnRevision revision)
        {
            string strRev = revision.RevisionType == SvnRevisionType.Time ?
                revision.Time.ToLocalTime().ToString("g") : revision.ToString();

            return fileName + " - " + strRev;
        }

        static string PathSafeRevision(SvnRevision revision)
        {
            if (revision.RevisionType == SvnRevisionType.Time)
                return revision.Time.ToLocalTime().ToString("yyyyMMdd_hhmmss");
            return revision.ToString();
        }

        string GetName(string filename, SvnRevision rev)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException("filename");
            else if (rev == null)
                throw new ArgumentNullException("rev");

            return (Path.GetFileNameWithoutExtension(filename) + "." + PathSafeRevision(rev) + Path.GetExtension(filename)).Trim('.');
        }

        string GetTempPath(string filename, SvnRevision rev)
        {
            string name = GetName(filename, rev);
            string file;
            if (_lastDir == null || !Directory.Exists(_lastDir) || File.Exists(file = Path.Combine(_lastDir, name)))
            {
                _lastDir = GetService<IAnkhTempDirManager>().GetTempDir();

                file = Path.Combine(_lastDir, name);
            }

            return file;
        }

        string _lastDir;
        public string GetTempFile(SvnItem target, SharpSvn.SvnRevision revision, bool withProgress)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            else if (revision == null)
                throw new ArgumentNullException("revision");

            string file = GetTempPath(target.Name, revision);

            if (target.NodeKind != SvnNodeKind.File)
                throw new InvalidOperationException("Can't create a tempfile from a directory");

            ProgressRunnerResult r = GetService<IProgressRunner>().RunModal("Getting file",
                delegate(object sender, ProgressWorkerArgs aa)
                {
                    SvnWriteArgs wa = new SvnWriteArgs();
                    wa.Revision = revision;

                    using (Stream s = File.Create(file))
                        aa.Client.Write(new SvnPathTarget(target.FullPath), s, wa);
                });

            if (!r.Succeeded)
                return null; // User canceled

            if (File.Exists(file))
                File.SetAttributes(file, FileAttributes.ReadOnly); // A readonly file does not allow editting from many diff tools

            return file;
        }

        public string GetTempFile(SharpSvn.SvnTarget target, SharpSvn.SvnRevision revision, bool withProgress)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            else if (revision == null)
                throw new ArgumentNullException("revision");

            string file = GetTempPath(target.FileName, revision);
            bool unrelated = false;

            ProgressRunnerResult r = GetService<IProgressRunner>().RunModal("Getting file",
                delegate(object sender, ProgressWorkerArgs aa)
                {
                    SvnWriteArgs wa = new SvnWriteArgs();
                    wa.Revision = revision;
                    wa.AddExpectedError(SvnErrorCode.SVN_ERR_CLIENT_UNRELATED_RESOURCES);

                    using (Stream s = File.Create(file))
                        if (!aa.Client.Write(target, s, wa))
                        {
                            if (wa.LastException.SvnErrorCode == SvnErrorCode.SVN_ERR_CLIENT_UNRELATED_RESOURCES)
                                unrelated = true;
                        }
                });

            if (!r.Succeeded || unrelated)
                return null; // User canceled

            if (File.Exists(file))
                File.SetAttributes(file, FileAttributes.ReadOnly); // A readonly file does not allow editting from many diff tools

            return file;
        }

        public string[] GetTempFiles(SvnTarget target, SvnRevision from, SvnRevision to, bool withProgress)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            else if (from == null)
                throw new ArgumentNullException("from");
            else if (to == null)
                throw new ArgumentNullException("to");

            // TODO: Replace with SvnClient.FileVersions call when to = from+1

            string f1;
            string f2;

            if (from.RevisionType == SvnRevisionType.Number && to.RevisionType == SvnRevisionType.Number && from.Revision + 1 == to.Revision)
            {
                f1 = GetTempPath(target.FileName, from);
                f2 = GetTempPath(target.FileName, to);

                int n = 0;
                ProgressRunnerResult r = Context.GetService<IProgressRunner>().RunModal("Getting revisions",
                    delegate(object sender, ProgressWorkerArgs e)
                    {
                        SvnFileVersionsArgs ea = new SvnFileVersionsArgs();
                        ea.Start = from;
                        ea.End = to;

                        e.Client.FileVersions(target, ea,
                            delegate(object sender2, SvnFileVersionEventArgs e2)
                            {
                                if (n++ == 0)
                                    e2.WriteTo(f1);
                                else
                                    e2.WriteTo(f2);
                            });
                    });

                if (!r.Succeeded)
                    return null;

                if (n != 2)
                {
                    // Sloooooow workaround for SvnBridge / Codeplex

                    f1 = GetTempFile(target, from, withProgress);
                    if (f1 == null)
                        return null; // Canceled
                    f2 = GetTempFile(target, to, withProgress);
                }
            }
            else
            {
                f1 = GetTempFile(target, from, withProgress);
                if (f1 == null)
                    return null; // Canceled
                f2 = GetTempFile(target, to, withProgress);
            }

            if (string.IsNullOrEmpty(f1) || string.IsNullOrEmpty(f2))
                return null;

            string[] files = new string[] { f1, f2 };

            foreach (string f in files)
            {
                if (File.Exists(f))
                    File.SetAttributes(f, FileAttributes.ReadOnly);
            }

            return files;
        }
    }
}
