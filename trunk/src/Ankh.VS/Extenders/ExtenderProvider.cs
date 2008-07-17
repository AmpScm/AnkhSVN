using System;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Collections;
using Ankh.Ids;
using Ankh.Selection;


namespace Ankh.VS.Extenders
{
    /// <summary>
    /// This is the class factory for extender objects
    /// </summary>
    public class AnkhExtenderProvider : EnvDTE.IExtenderProvider, IDisposable
    {
        public const string ServiceName = AnkhId.ExtenderProviderName;
        #region CATIDs
        public const string CATID_CscFileBrowse = "8d58e6af-ed4e-48b0-8c7b-c74ef0735451";
        public const string CATID_CscFolderBrowse = "914fe278-054a-45db-bf9e-5f22484cc84c";
        public const string CATID_CscProjectBrowse = "4ef9f003-de95-4d60-96b0-212979f2a857";
        public const string CATID_VbFileBrowse = "ea5bd05d-3c72-40a5-95a0-28a2773311ca";
        public const string CATID_VbFolderBrowse = "932dc619-2eaa-4192-b7e6-3d15ad31df49";
        public const string CATID_VbProjectBrowse = "e0fdc879-c32a-4751-a3d3-0b3824bd575f";
        public const string CATID_VjFileBrowse = "e6fdf869-f3d1-11d4-8576-0002a516ece8";
        public const string CATID_VjFolderBrowse = "e6fdf86a-f3d1-11d4-8576-0002a516ece8";
        public const string CATID_VjProjectBrowse = "e6fdf86c-f3d1-11d4-8576-0002a516ece8";
        public const string CATID_SolutionBrowse = "a2392464-7c22-11d3-bdca-00c04f688e50";
        public const string CATID_CcFileBrowse = "ee8299c9-19b6-4f20-abea-e1fd9a33b683";
        public const string CATID_CcProjectBrowse = "ee8299cb-19b6-4f20-abea-e1fd9a33b683";
        public const string CATID_GenericProject = "610d4611-d0d5-11d2-8599-006097c68e81";
        #endregion

        readonly IAnkhServiceProvider _context;
        int[] _cookies;

        public AnkhExtenderProvider(IAnkhServiceProvider context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;

            EnvDTE._DTE dte = _context.GetService<EnvDTE._DTE>();
            EnvDTE.ObjectExtenders extenders;
            if (dte != null && ((extenders = dte.ObjectExtenders) != null))
            {
                _cookies = new int[CATIDS.Length];
                int n = 0;
                foreach (string catid in CATIDS)
                {
                    string cid = new Guid(catid).ToString("B");

                    _cookies[n++] = extenders.RegisterExtenderProvider(cid, ServiceName, this, ServiceName);
                }
            }
        }

        public void Dispose()
        {
            if (_cookies == null)
                return;
            EnvDTE._DTE dte = _context.GetService<EnvDTE._DTE>();
            EnvDTE.ObjectExtenders extenders;
            if (dte != null && ((extenders = dte.ObjectExtenders) != null))
                foreach (int cookie in _cookies)
                {
                    extenders.UnregisterExtenderProvider(cookie);
                }

            _cookies = null;
        }

        public bool CanExtend(string ExtenderCATID, string ExtenderName, object ExtendeeObject)
        {
            ISelectionContext selection = _context.GetService<ISelectionContext>();

            if (selection != null)
            {
                bool first = true;
                foreach (SvnItem item in selection.GetSelectedSvnItems(false))
                {
                    if (!item.IsVersioned && !item.IsVersionable)
                        return false;
                    else if (first)
                        first = false;
                    else
                        return true;
                }

                if (!first)
                    return true;
            }

            return false;
        }

        [CLSCompliant(false)]
        public object GetExtender(string ExtenderCATID, string ExtenderName, object ExtendeeObject, EnvDTE.IExtenderSite ExtenderSite, int Cookie)
        {
            ISelectionContext selection = _context.GetService<ISelectionContext>();

            if (selection != null)
            {
                SvnItem selected = null;

                foreach (SvnItem item in selection.GetSelectedSvnItems(false))
                {
                    if (!item.IsVersioned && !item.IsVersionable)
                        return false;

                    selected = item;
                    break;
                }

                if (selected == null)
                    return null;

                return new SvnItemExtender(selected, _context);
            }

            return null;
        }

        private readonly static string[] CATIDS = new string[]{
        CATID_CscFileBrowse,
        CATID_CscFolderBrowse,
        CATID_CscProjectBrowse,
        CATID_VbFileBrowse,
        CATID_VbFolderBrowse,
        CATID_VbProjectBrowse,
        CATID_VjFileBrowse,
        CATID_VjFolderBrowse,
        CATID_VjProjectBrowse,
        CATID_SolutionBrowse,
        CATID_CcFileBrowse,
        CATID_CcProjectBrowse,
        CATID_GenericProject
        };
    }
}
