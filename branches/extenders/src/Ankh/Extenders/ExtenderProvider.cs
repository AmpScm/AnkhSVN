using System;
using System.Runtime.InteropServices;
using EnvDTE;
using VSLangProj;
using System.Reflection;
using System.Collections;
using NSvn;

namespace Ankh.Extenders
{
    /// <summary>
    /// This is the class factory for extender objects
    /// </summary>
    [GuidAttribute("BAB7BC93-6097-486e-B29D-CFEA4AB9107B"), ProgId("Ankh.ExtenderProvider")]	
    public class ExtenderProvider : IExtenderProvider
    {
        private ExtenderProvider( AnkhContext context )
        {
            this.context = context;
        }

        #region IExtenderProvider Members

        public object GetExtender(string ExtenderCATID, string ExtenderName, object ExtendeeObject, IExtenderSite ExtenderSite, int Cookie)
        {
            ResourceGathererVisitor v = new ResourceGathererVisitor();
            this.context.SolutionExplorer.VisitSelectedItems( v, false );
            
            TestExtender extender = new TestExtender();
            extender.Resource = (WorkingCopyResource)v.WorkingCopyResources[0];

            return extender;
        }

        public bool CanExtend(string ExtenderCATID, string ExtenderName, object ExtendeeObject)
        {
            VersionedVisitor v = new VersionedVisitor();
            this.context.SolutionExplorer.VisitSelectedItems( v, false );
            return v.IsVersioned;
        }

        #endregion


        /// <summary>
        /// Registers the extender provider for all the cathegories we want.
        /// </summary>
        internal static void Register( AnkhContext context )
        {
            ExtenderProvider.provider = new ExtenderProvider( context );

            _DTE dte = context.DTE;
            cookies.Add ( dte.ObjectExtenders.RegisterExtenderProvider( 
                PrjBrowseObjectCATID.prjCATIDCSharpFileBrowseObject, "Ankh", provider, string.Empty ) );
        }

        
        /// <summary>
        /// Unregister all extender provider registrations.
        /// </summary>
        /// <param name="dte"></param>
        internal static void Unregister( _DTE dte )
        {
            foreach( int cookie in cookies )
                dte.ObjectExtenders.UnregisterExtenderProvider( cookie );
        }

        private static ExtenderProvider provider;
        private static ArrayList cookies = new ArrayList();

        private AnkhContext context;

    }
}
