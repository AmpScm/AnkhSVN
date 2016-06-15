using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ankh.VS;
using System.Drawing;
using Ankh.Commands;

namespace Ankh.Scc
{
    partial class SvnSccProvider
    {
        static string StripComponents(string info, int len)
        {
            bool broken = false;

            while(info.Length > len)
            {
                int n = info.IndexOf('/');

                if (n <= 0)
                    break;

                info = info.Substring(n + 1);
                broken = true;
            }

            return broken ? ".." + info : info;
        }
        #region Branch
        protected override string BranchName
        {
            get
            {
                SvnItem item = GetService<IAnkhSolutionSettings>().ProjectRootSvnItem;

                if (item == null || item.Uri == null)
                    return "";

                return StripComponents("^/" + item.WorkingCopy.RepositoryRoot.MakeRelativeUri(item.Uri).ToString().TrimEnd('/'), 20);
            }
        }
        protected override string BranchDetail
        {
            get
            {
                SvnItem item = GetService<IAnkhSolutionSettings>().ProjectRootSvnItem;

                if (item == null || item.Uri == null)
                    return "";

                return "^/" + item.WorkingCopy.RepositoryRoot.MakeRelativeUri(item.Uri).ToString();
            }
        }

        protected override ImageReference BranchIcon
        {
            get
            {
                return base.BranchIcon;
            }
        }
        #endregion

        #region PendingChange

        protected override void OnPendingChangesClicked(Point clickedElement)
        {
            GetService<IAnkhCommandService>().PostExecCommand(AnkhCommand.ShowPendingChanges);
        }

        protected override int PendingChangeCount
        {
            get
            {
                return GetService<IPendingChangesManager>().PendingChanges.Count;
            }
        }

        protected override string PendingChangeLabel
        {
            get
            {
                return base.PendingChangeLabel;
            }
        }

        protected override string PendingChangeDetail
        {
            get
            {
                return base.PendingChangeDetail;
            }
        }
        #endregion

        #region Repository

        protected override void OnRepositoryUIClicked(Point clickedElement)
        {
            GetService<IAnkhCommandService>().PostExecCommand(AnkhCommand.SolutionSwitchDialog);
        }

        protected override string RepositoryName
        {
            get
            {
                SvnItem item = GetService<IAnkhSolutionSettings>().ProjectRootSvnItem;

                if (item == null || item.Uri == null)
                    return "";

                return item.WorkingCopy.RepositoryRoot.PathAndQuery.TrimEnd('/').TrimStart('/');
            }
        }

        protected override string RepositoryDetail
        {
            get
            {
                SvnItem item = GetService<IAnkhSolutionSettings>().ProjectRootSvnItem;

                if (item == null || item.Uri == null)
                    return "";

                return item.WorkingCopy.RepositoryRoot.ToString();
            }
        }

        protected override ImageReference RepositoryIcon
        {
            get
            {
                return base.RepositoryIcon;
            }
        }
#endregion
    }
}
