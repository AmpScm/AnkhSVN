using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

using SharpSvn;

using Ankh.Ids;
using Ankh.Scc.ProjectMap;
using Ankh.Selection;
using Ankh.VS;

namespace Ankh.Scc
{
    partial class AnkhSccProvider
    {
        /// <summary>
        /// Writes the enlistment state to the solution
        /// </summary>
        /// <param name="pPropBag">The p prop bag.</param>
        void IAnkhSccService.WriteEnlistments(IPropertyBag pPropBag)
        {
            if (!IsActive || !IsSolutionManaged)
                return;

            SortedList<string, string> values = new SortedList<string, string>(StringComparer.OrdinalIgnoreCase);

            string projectDir = SolutionDirectory;

            IAnkhSolutionSettings ss = GetService<IAnkhSolutionSettings>();
            Uri solutionUri = null;

            if (ss != null)
                projectDir = ss.ProjectRootWithSeparator;
            else
                projectDir = projectDir.TrimEnd('\\') + '\\';

            string normalizedProjectDir = SvnTools.GetNormalizedFullPath(projectDir);

            foreach (SccProjectData project in _projectMap.Values)
            {
                if (string.IsNullOrEmpty(project.ProjectDirectory) || !project.IsManaged)
                    continue; // Solution folder or unmanaged?

                bool enlist = false;
                bool enlistOptional = true;
                IVsSccProjectEnlistmentChoice projectChoice = project.VsProject as IVsSccProjectEnlistmentChoice;

                if (projectChoice != null)
                {
                    VSSCCENLISTMENTCHOICE[] choice = new VSSCCENLISTMENTCHOICE[1];

                    if (ErrorHandler.Succeeded(projectChoice.GetEnlistmentChoice(choice)))
                    {
                        switch (choice[0])
                        {
                            case VSSCCENLISTMENTCHOICE.VSSCC_EC_NEVER:
                                // Don't take any enlistment actions
                                break;
                            case VSSCCENLISTMENTCHOICE.VSSCC_EC_COMPULSORY:
                                enlist = true;
                                enlistOptional = false;
                                break;
                            case VSSCCENLISTMENTCHOICE.VSSCC_EC_OPTIONAL:
                                enlistOptional = enlist = true;
                                break;
                        }
                    }
                }

                string dir = SvnTools.GetNormalizedFullPath(project.ProjectDirectory);

                if (!enlist && dir.StartsWith(projectDir, StringComparison.OrdinalIgnoreCase)
                    || normalizedProjectDir.Equals(dir, StringComparison.OrdinalIgnoreCase))
                {
                    // The directory is below our project root, we can ignore it
                    //  - Yes we can, unless the directory is switched or nested below the root

                    // TODO: Check those conditions somewhere else and reuse here                    
                    continue;
                }

                SvnItem item = StatusCache[dir];

                if (solutionUri == null)
                {
                    SvnItem solDirItem = StatusCache[SolutionDirectory];

                    if (solDirItem != null && solDirItem.IsVersioned && solDirItem.Status != null && solDirItem.Status.Uri != null)
                        solutionUri = solDirItem.Status.Uri;
                }

                if (item == null || !item.IsVersioned || item.Status == null || item.Status.Uri == null)
                    continue;

                Uri itemUri = item.Status.Uri;

                if (solutionUri != null)
                    itemUri = solutionUri.MakeRelativeUri(itemUri);

                // This should match the directory as specified in the solution!!!
                // (It currently does, but only because we don't really support virtual folders yet)
                dir = PackageUtilities.MakeRelative(projectDir, dir);

                string name = "Project." + project.ProjectGuid.ToString("B").ToUpperInvariant();

                values[name + ".Path"] = '\"' + dir + '\"';                
                values[name + ".Uri"] = '\"' + Uri.EscapeUriString(itemUri.ToString()) + '\"';
                if (enlist)
                {
                    // To enlist a project we need its project type (to get to the project factory)
                    values[name + ".Type"] = project.ProjectTypeGuid.ToString("B").ToUpperInvariant();
                    values[name + ".Enlist"] = enlistOptional ? "Maybe" : true.ToString();
                }
            }

            // We write all values in alphabetical order to make sure we don't change the solution unnecessary
            foreach (KeyValuePair<string, string> kv in values)
            {
                object value = kv.Value;
                pPropBag.Write(kv.Key, ref value);
            }
        }

        /// <summary>
        /// Translates a physical project path to a (possibly) virtual project path.
        /// </summary>
        /// <param name="lpszEnlistmentPath">[in] The physical path (either the local path or the enlistment UNC path) to be translated.</param>
        /// <param name="pbstrProjectPath">[out] The (possibly) virtual project path.</param>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK"/>. If it fails, it returns an error code.
        /// </returns>
        public int TranslateEnlistmentPathToProjectPath(string lpszEnlistmentPath, out string pbstrProjectPath)
        {
            pbstrProjectPath = lpszEnlistmentPath;
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Translates a possibly virtual project path to a local path and an enlistment physical path.
        /// </summary>
        /// <param name="lpszProjectPath">[in] The project's (possibly) virtual path as obtained from the solution file.</param>
        /// <param name="pbstrEnlistmentPath">[out] The local path used by the solution for loading and saving the project.</param>
        /// <param name="pbstrEnlistmentPathUNC">[out] The path used by the source control system for managing the enlistment ("\\drive\path", "[drive]:\path", "file://server/path").</param>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK"/>. If it fails, it returns an error code.
        /// </returns>
        public int TranslateProjectPathToEnlistmentPath(string lpszProjectPath, out string pbstrEnlistmentPath, out string pbstrEnlistmentPathUNC)
        {
            pbstrEnlistmentPath = lpszProjectPath;
            pbstrEnlistmentPathUNC = lpszProjectPath;
            return VSConstants.S_OK;
        }
    }
}
