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

        /// <summary>
        /// Gets path to the diff executable while taking care of config file settings.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>The exe path.</returns>
        protected virtual string GetDiffPath(DiffMode mode)
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
            if (!Substitute(diffApp, args, out program, out arguments))
            {
				new AnkhMessageBox(Context).Show(string.Format("Can't find diff program '{0}'", program));
                return false;
            }

            // TODO: Maybe Handle file saves and program exits

            Process p = new Process();
            p.StartInfo = new ProcessStartInfo(program, arguments);

            string mergedFile = args.MineFile;

            DiffToolMonitor monitor = null;
            if (!string.IsNullOrEmpty(mergedFile))
            {
                monitor = new DiffToolMonitor(Context, mergedFile);

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
        protected virtual string GetMergePath(DiffMode mode)
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

        IFileStatusCache _statusCache;
        IFileStatusCache Cache
        {
            get { return _statusCache ?? (_statusCache = GetService<IFileStatusCache>()); }
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
            if (!Substitute(diffApp, args, out program, out arguments))
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
                monitor = new DiffToolMonitor(Context, mergedFile);

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
            readonly string _fileToMonitor;

            public DiffToolMonitor(IAnkhServiceProvider context, string monitor)
                : base(context)
            {
                if (string.IsNullOrEmpty(monitor))
                    throw new ArgumentNullException("monitor");
                else if (!SvnItem.IsValidPath(monitor))
                    throw new ArgumentOutOfRangeException("monitor");

                _fileToMonitor = monitor;

                IVsFileChangeEx fx = context.GetService<IVsFileChangeEx>(typeof(SVsFileChangeEx));


                if (fx == null || !ErrorHandler.Succeeded(fx.AdviseFileChange(monitor,
                        (uint)(_VSFILECHANGEFLAGS.VSFILECHG_Time | _VSFILECHANGEFLAGS.VSFILECHG_Size
                        | _VSFILECHANGEFLAGS.VSFILECHG_Add | _VSFILECHANGEFLAGS.VSFILECHG_Del
                        | _VSFILECHANGEFLAGS.VSFILECHG_Attr),
                        this,
                        out _cookie)))
                {
                    _cookie = 0;
                }
            }

            public void Dispose()
            {
                if (_cookie != 0)
                {
                    uint ck = _cookie;
                    _cookie = 0;

                    IVsFileChangeEx fx = GetService<IVsFileChangeEx>(typeof(SVsFileChangeEx));

                    if(fx != null)
                        fx.UnadviseFileChange(ck);
                }
            }

            public void Register(Process p)
            {
                p.Exited += new EventHandler(OnExited);
            }

            void OnExited(object sender, EventArgs e)
            {
                Dispose();
            }            

            public int DirectoryChanged(string pszDirectory)
            {
                return VSConstants.S_OK;
            }

            public int FilesChanged(uint cChanges, string[] rgpszFile, uint[] rggrfChange)
            {
                if(rgpszFile != null)
                {
                    foreach(string file in rgpszFile)
                    {
                        if(string.Equals(file, _fileToMonitor, StringComparison.OrdinalIgnoreCase))
                        {
                            IFileStatusMonitor m = GetService<IFileStatusMonitor>();

                            if (m != null)
                                m.ExternallyChanged(_fileToMonitor);

                            break;
                        }
                    }
                }

                return VSConstants.S_OK;
            }


        }


        #region Argument Substitution support
        private bool Substitute(string reference, AnkhDiffArgs args, out string program, out string arguments)
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

            if (reference.StartsWith("\""))
            {
                // Ok: The easy way:
                int nEnd = reference.IndexOf('\"', 1);

                if (nEnd < 0)
                    return false; // Invalid string!

                program = reference.Substring(1, nEnd - 1);
                reference = reference.Substring(nEnd + 1).TrimStart();

                program = SubstituteArguments(program, args);

                if (string.IsNullOrEmpty(program) || !File.Exists(program))
                    return false; // File not found
            }
            else
            {
                // We use the algorithm as documented by CreateProcess() in MSDN
                // http://msdn2.microsoft.com/en-us/library/ms682425(VS.85).aspx

                char[] spacers = new char[] { ' ', '\t' };
                int nFrom = 0;
                int nTok;

                while ((nFrom < reference.Length) && 
                    (0 <= (nTok = reference.IndexOfAny(spacers, nFrom))))
                {
                    string f = reference.Substring(0, nTok);

                    f = SubstituteArguments(f, args);

                    if (!string.IsNullOrEmpty(f) && File.Exists(f))
                    {
                        program = f;
                        reference = reference.Substring(nTok + 1).TrimStart();
                        break;
                    }
                    else
                        nFrom = nTok + 1;
                }
            }

            if (string.IsNullOrEmpty(program))
                return false; // Couldn't detect program

            arguments = SubstituteArguments(reference, args);

            return true;
        }

        private string SubstituteArguments(string arguments, AnkhDiffArgs diffArgs)
        {
            return Regex.Replace(arguments, @"(\%(?<pc>[a-zA-Z0-9_]+)(\%|\b))|(\$\((?<vs>[a-zA-Z0-9_-]*)\))",
                new Replacer(diffArgs).Replace);
        }

        sealed class Replacer
        {
            readonly AnkhDiffArgs _diffArgs;
            readonly AnkhMergeArgs _mergeArgs;

            public Replacer(AnkhDiffArgs args)
            {
                if (args == null)
                    throw new ArgumentNullException("args");

                _diffArgs = args;
                _mergeArgs = args as AnkhMergeArgs;
            }

            AnkhDiffArgs DiffArgs
            {
                get { return _diffArgs; }
            }

            AnkhMergeArgs MergeArgs
            {
                get { return _mergeArgs; }
            }

            public string Replace(Match match)
            {
                string key;

                if (match.Groups["pc"].Length > 1)
                    key = match.Groups["pc"].Value;
                else if (match.Groups["vs"].Length > 1)
                    key = match.Groups["vs"].Value;
                else
                    return match.Value; // Don't replace if not matched

                key = key.ToUpperInvariant();
                switch (key)
                {
                    case "BASE":
                        return DiffArgs.BaseFile;
                    case "BNAME":
                    case "BASENAME":
                        return DiffArgs.BaseTitle ?? Path.GetFileName(DiffArgs.BaseFile);

                    case "MINE":
                        return DiffArgs.MineFile;
                    case "YNAME":
                    case "MINENAME":
                        return DiffArgs.MineTitle ?? Path.GetFileName(DiffArgs.MineFile);

                    default:
                        if (MergeArgs != null)
                            switch (key)
                            {
                                case "THEIRS":
                                    return MergeArgs.TheirsFile;
                                case "TNAME":
                                case "THEIRNAME":
                                case "THEIRSNAME":
                                    return MergeArgs.TheirsTitle ?? Path.GetFileName(MergeArgs.TheirsFile);

                                case "MERGED":
                                    return MergeArgs.MergedFile;
                                case "MERGEDNAME":
                                case "MNAME":
                                    return MergeArgs.MergedTitle ?? Path.GetFileName(MergeArgs.MergedFile); ;
                            }

                        // Just replace with "" if unknown
                        string v = Environment.GetEnvironmentVariable(key);
                        if (!string.IsNullOrEmpty(v))
                            return v;

                        return "";
                }
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

        string _lastDir;
        public string GetTempFile(SvnItem target, SharpSvn.SvnRevision revision, bool withProgress)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            else if (revision == null)
                throw new ArgumentNullException("revision");

            
            string name = target.NameWithoutExtension + "." + PathSafeRevision(revision) + target.Extension;

            string file;
            if(_lastDir == null || !Directory.Exists(_lastDir) || File.Exists(file = Path.Combine(_lastDir, name)))
            {
                _lastDir = GetService<IAnkhTempDirManager>().GetTempDir();

                file = Path.Combine(_lastDir, name);
            }

            if(target.NodeKind != SvnNodeKind.File)
                throw new InvalidOperationException("Can't create a tempfile from a directory");

            GetService<IProgressRunner>().Run("Getting file",
                delegate(object sender, ProgressWorkerArgs aa)
                {
                    SvnWriteArgs wa = new SvnWriteArgs();
                    wa.Revision = revision;

                    using (Stream s = File.Create(file))
                        aa.Client.Write(new SvnPathTarget(target.FullPath), s, wa);
                });

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

            string targetName = target.FileName;
            string name = Path.GetFileNameWithoutExtension(targetName) + "." + PathSafeRevision(revision) + Path.GetExtension(targetName);

            string file;
            if (_lastDir == null || !Directory.Exists(_lastDir) || File.Exists(file = Path.Combine(_lastDir, name)))
            {
                _lastDir = GetService<IAnkhTempDirManager>().GetTempDir();

                file = Path.Combine(_lastDir, name);
            }

            GetService<IProgressRunner>().Run("Getting file",
                delegate(object sender, ProgressWorkerArgs aa)
                {
                    SvnWriteArgs wa = new SvnWriteArgs();
                    wa.Revision = revision;

                    using(Stream s = File.Create(file))
                        aa.Client.Write(target, s, wa);
                });

            if (File.Exists(file))
                File.SetAttributes(file, FileAttributes.ReadOnly); // A readonly file does not allow editting from many diff tools

            return file;
        }        
    }
}
