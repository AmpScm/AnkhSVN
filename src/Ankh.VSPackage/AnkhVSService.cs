using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.VSPackage
{
    [GuidAttribute(Guids.guidAnkhVSPackageString)]
    public class AnkhVSService : IAnkhVSService
    {
        public AnkhVSService( Ankh_VSPackage package, SccProviderService sccProviderService )
        {
            this.sccProviderService = sccProviderService;
            this.package = package;
        }

        public void SetContext( IContext context )
        {
            this.sccProviderService.Context = context;
        }

        public VSITEMSELECTION[] GetSelection()
        {
            return this.package.GetSelection();
        }

        private SccProviderService sccProviderService;
        private Ankh_VSPackage package;

    }
}
