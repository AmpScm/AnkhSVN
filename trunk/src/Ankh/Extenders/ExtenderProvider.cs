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
            this.extender = new ResourceExtender();
        }

        #region IExtenderProvider Members

        public object GetExtender(string ExtenderCATID, string ExtenderName, object ExtendeeObject, IExtenderSite ExtenderSite, int Cookie)
        {
            try
            {
                // get the selected items
                ResourceGathererVisitor v = new ResourceGathererVisitor();
                this.context.SolutionExplorer.VisitSelectedItems( v, false );
            
                // have the extender know about the selected item.
                this.extender.Resource = (WorkingCopyResource)v.WorkingCopyResources[0];

                return this.extender;
            }
            catch( Exception ex )
            {
                Error.Handle( ex );
                throw;
            }
        }

        public bool CanExtend(string ExtenderCATID, string ExtenderName, object ExtendeeObject)
        {
            try
            {
                // ensure all selected items are versioned
                VersionedVisitor v = new VersionedVisitor();
                this.context.SolutionExplorer.VisitSelectedItems( v, false );
                return v.IsVersioned;
            }
            catch( Exception ex )
            {
                Error.Handle( ex );
                throw;
            }
        }

        #endregion


        /// <summary>
        /// Registers the extender provider for all the cathegories we want.
        /// </summary>
        internal static void Register( AnkhContext context )
        {
            ExtenderProvider.provider = new ExtenderProvider( context );

            _DTE dte = context.DTE;
            foreach( string catid in CATIDS )
            {
                // cookies need to be stored so we can unregister the extender provider later
                cookies.Add ( dte.ObjectExtenders.RegisterExtenderProvider( 
                    catid, "Ankh", provider, string.Empty ) );
            }
        }

        
        /// <summary>
        /// Unregister all extender provider registrations.
        /// </summary>
        /// <param name="dte"></param>
        internal static void Unregister( _DTE dte )
        {
            // use the stored cookies to unregister the registered providers.
            foreach( int cookie in cookies )
                dte.ObjectExtenders.UnregisterExtenderProvider( cookie );
        }

        private static ExtenderProvider provider;
        private static ArrayList cookies = new ArrayList();

        private AnkhContext context;

        private ResourceExtender extender;

        private readonly static string[] CATIDS = new string[]{
                                                                  "{8D58E6AF-ED4E-48B0-8C7B-C74EF0735451}", // C# File Browse
                                                                  "{914FE278-054A-45DB-BF9E-5F22484CC84C}", // C# folder browse
                                                                  "{4EF9F003-DE95-4d60-96B0-212979F2A857}", // C# Project browse
                                                                  "{EA5BD05D-3C72-40A5-95A0-28A2773311CA}", // VB file browse
                                                                  "{932DC619-2EAA-4192-B7E6-3D15AD31DF49}", // VB folder browse
                                                                  "{E0FDC879-C32A-4751-A3D3-0B3824BD575F}", // VB project browse
                                                                  "{E6FDF869-F3D1-11D4-8576-0002A516ECE8}", // VJ# file browse
                                                                  "{E6FDF86A-F3D1-11D4-8576-0002A516ECE8}", // VJ# folder browse
                                                                  "{E6FDF86C-F3D1-11D4-8576-0002A516ECE8}", // VJ# project browse
                                                                  "{A2392464-7C22-11d3-BDCA-00C04F688E50}", // solution browse object
                                                                  "{610d4611-d0d5-11d2-8599-006097c68e81}" // generic project
                                                              };



        }
}
