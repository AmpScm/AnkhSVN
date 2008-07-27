using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Scc.UI;
using System.IO;
using System.Diagnostics;
using Ankh.UI;
using System.Text.RegularExpressions;

namespace Ankh.Services
{
    class AnkhDiff : AnkhService, IAnkhDiffHandler
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
        protected virtual string GetDiffPath(bool forceExternal)
        {
            IAnkhConfigurationService cs = GetService<IAnkhConfigurationService>();

            if (forceExternal || !cs.Instance.ChooseDiffMergeManual)
                return cs.Instance.DiffExePath;
            else
                return null;
        }

        #region IAnkhDiffHandler Members

        public bool RunDiff(AnkhDiffArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            else if (!args.Validate())
                throw new ArgumentException("Arguments not filled correctly", "args");

            bool forceExternal = (args.Mode & DiffMode.PreferExternal) != 0;





            string quotedLeftPath = args.BaseFile;
            string quotedRightPath = args.MineFile;
            string diffString = this.GetDiffPath(forceExternal);

            string program;
            string arguments;
            if (!Substitute(diffString, args, out program, out arguments))
            {
                new AnkhMessageBox(Context).Show("Can't find diff program");
                return false;
            }

            // TODO: Maybe Handle file saves and program exits

            Process.Start(program, arguments);
            return true;
        }

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

                if (program.IndexOf('%') >= 0)
                    program = Environment.ExpandEnvironmentVariables(program);

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

                while (0 <= (nTok = reference.IndexOfAny(spacers, nFrom)))
                {
                    string f = reference.Substring(0, nTok);

                    if (f.IndexOf('%') >= 0)
                        f = Environment.ExpandEnvironmentVariables(f);

                    if (!string.IsNullOrEmpty(f) && File.Exists(f))
                    {
                        program = f;
                        reference = reference.Substring(nTok + 1).TrimStart();
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
            return Regex.Replace(arguments, @"(\%(?<pc>[a-zA-Z0-9_]+)\b)|(\$\((?<vs>[a-zA-Z0-9_-]*)\))",
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
                                    return MergeArgs.TheirsTitle ?? Path.GetFileName(MergeArgs.TheirsFile);

                                case "MERGED":
                                    return MergeArgs.MergedFile;
                                case "MERGEDNAME":
                                case "MNAME":
                                    return MergeArgs.MergedTitle;
                            }

                        // Just replace with "" if unknown
                        return "";
                }
            }
        }

        public bool RunMerge(AnkhMergeArgs args)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
