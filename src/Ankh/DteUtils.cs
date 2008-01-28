using System;
using EnvDTE;

namespace Ankh
{
    /// <summary>
    /// Summary description for DteUtils.
    /// </summary>
    public abstract class DteUtils
    {
        private DteUtils()
        {
            // nothing here
        }

        public static bool IsSolutionItemsOrMiscItemsProject(Project proj)
        {
            return (String.Compare(proj.Kind, SolutionItemsKind) == 0 ||
                String.Compare(proj.Kind, MiscItemsKind) == 0);
        }

        public static object GetProjectItemObject( ProjectItem item )
        {
            try
            {
                return item.Object;
            }

            catch ( Exception )
            {
                return null;
            }
        }

        public const string SolutionItemsKind = "{66A26720-8FB5-11D2-AA7E-00C04F688DDE}";
        public const string MiscItemsKind = "{66A2671D-8FB5-11D2-AA7E-00C04F688DDE}";
        public const string WebProjects2005Kind = "{E24C65DC-7377-472b-9ABA-BC803B73C61A}";
        public const string EnterpriseTemplateProjectKind = "{7D353B21-6E36-11D2-B35A-0000F81F0C06}";
        public const string EnterpriseTemplateProjectItemKind = "{EA6618E8-6E24-4528-94BE-6889FE16485C}";
        public const string SolutionFolderKind = "{66A26720-8FB5-11D2-AA7E-00C04F688DDE}";


       
    }
}
