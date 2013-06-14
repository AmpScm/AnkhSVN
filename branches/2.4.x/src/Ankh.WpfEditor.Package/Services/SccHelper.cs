using Ankh.Scc;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.WpfPackage.Services
{
    [GlobalService(typeof(ISccHelper))]
    sealed class SccHelper : AnkhService, ISccHelper
    {
        public SccHelper(IAnkhServiceProvider context)
            : base(context)
        {
        }

        public bool EnsureProjectLoaded(Guid project, bool recursive)
        {
            IVsSolution4 sln = GetService<IVsSolution4>(typeof(SVsSolution));

            if (sln == null)
                return false;

            if (recursive)
                return VSErr.Succeeded(sln.EnsureProjectIsLoaded(ref project, (uint)__VSBSLFLAGS.VSBSLFLAGS_None));
            else
                return VSErr.Succeeded(sln.ReloadProject(ref project));
        }
    }
}
