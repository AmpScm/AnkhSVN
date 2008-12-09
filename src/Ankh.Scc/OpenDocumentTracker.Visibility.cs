using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using Ankh.Scc.ProjectMap;
using Ankh.Selection;

namespace Ankh.Scc
{
    partial class OpenDocumentTracker
    {
        public int OnBeforeDocumentWindowShow(uint docCookie, int fFirstShow, IVsWindowFrame pFrame)
        {
            ProjectMap.SccDocumentData dd;

            if (TryGetDocument(docCookie, out dd))
            {
                dd.CheckDirty();

                if (dd.IsProjectPropertyPageHost)
                {
                    ProjectPropertyPageFixup(dd);
                }
            }
            return VSConstants.S_OK;
        }        

        public int OnAfterDocumentWindowHide(uint docCookie, IVsWindowFrame pFrame)
        {
            ProjectMap.SccDocumentData dd;

            if (docCookie != 0 && TryGetDocument(docCookie, out dd))
            {
                dd.CheckDirty();

                if (dd.IsProjectPropertyPageHost)
                {
                    ProjectPropertyPageFixup(dd);
                }
            }
            return VSConstants.S_OK;
        }

        private void ProjectPropertyPageFixup(ProjectMap.SccDocumentData dd)
        {
            // Ok, we have a project setting page here.
            // Project settings pages break (our) SCC handling in a number of ways
            //   * It creates an editor buffer for the AssemblyInfo file, but does 
            //     not notify changes or tell the user that you should save the file
            //   * It makes the project dirty without notifying
            //   * Saving the setting pages doesn't save your projec
            //
            // To work around this we poll all files in the current project for dirty

            IVsSccProject2 prj = dd.Hierarchy as IVsSccProject2;

            if (prj != null)
            {
                SvnProject project = new Ankh.Selection.SvnProject(null, prj);

                foreach (string file in GetService<IProjectFileMapper>().GetAllFilesOf(project))
                {
                    SccDocumentData itemData;
                    if (_docMap.TryGetValue(file, out itemData))
                        itemData.CheckDirty();
                }
            }
        }
    }
}
