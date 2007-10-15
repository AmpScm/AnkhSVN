using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

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

        private SccProviderService sccProviderService;
        private Ankh_VSPackage package;

    }
}
