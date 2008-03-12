using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using Ankh.UI;

namespace Ankh.Commands
{
    [Command(AnkhSvn.Ids.AnkhCommand.MarkProjectDirty)]
    public class MarkProjectDirty : ICommandHandler
    {
        public void OnUpdate(CommandUpdateEventArgs e)
        {
            e.Enabled = true;
        }

        public void OnExecute(CommandEventArgs e)
        {
            // Mark Scc Glyphs dirty, to force reload of glyphs from StatusCache
            IAnkhPackage package = e.Context.GetService<IAnkhPackage>();

            string commandString = e.Argument as string;
            IVsSolution sol = (IVsSolution)package.GetService(typeof(IVsSolution));
            if (sol != null)
            {
                IEnumHierarchies enumerator;
                Guid empty = Guid.Empty;
                if (VSConstants.S_OK == sol.GetProjectEnum((uint)(__VSENUMPROJFLAGS.EPF_ALLPROJECTS | __VSENUMPROJFLAGS.EPF_LOADEDINSOLUTION), ref empty, out enumerator))
                {
                    IVsHierarchy[] rgelt = new IVsHierarchy[1];
                    uint pceltFetched = 0;
                    while (enumerator.Next(1, rgelt, out pceltFetched) == VSConstants.S_OK &&
                           pceltFetched == 1)
                    {
                        IVsSccProject2 sccProject2 = rgelt[0] as IVsSccProject2;
                        if (sccProject2 != null)
                        {
                            // The commented code updates only the projects that contain the file. Not sure if this is beneficial
                            // Figuring this out can be very expensive, and getting a status is usually just a hashtable lookup

                            //IVsProject2 proj = sccProject2 as IVsProject2;
                            //int pfFound = -1;
                            //VSDOCUMENTPRIORITY[] priorities = new VSDOCUMENTPRIORITY[0];
                            //uint pItemId;

                            //if (proj == null || 
                            //    (commandString != null && 
                            //    proj.IsDocumentInProject(commandString, out pfFound, priorities, out pItemId) == VSConstants.S_OK))
                            //{
                            //    if (pfFound != 0)
                            sccProject2.SccGlyphChanged(0, null, null, null);
                            //}
                        }
                    }
                }
            }
        }
    }
}
