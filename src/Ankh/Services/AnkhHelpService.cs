// Copyright 2009 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.Collections.Generic;
using System.Text;
using Ankh.UI;
using System.Globalization;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using System.Windows.Forms;
using System.Reflection;

namespace Ankh.Services
{
    [GlobalService(typeof(IAnkhHelpService))]
    class AnkhHelpService : AnkhService, IAnkhHelpService
    {
        public AnkhHelpService(IAnkhServiceProvider context)
            : base(context)
        {
        }
        #region IAnkhDialogHelpService Members

        const string DefaultHelpBaseUrl = "https://amp-scm.com/AnkhSVN/help/";

        internal static string GetHelpBaseUrl()
        {
            Assembly assembly = typeof(AnkhHelpService).Assembly;
            object[] metadata = assembly.GetCustomAttributes(typeof(AssemblyMetadataAttribute), false);

            foreach (AssemblyMetadataAttribute item in metadata)
            {
                if (string.Equals(item.Key, "AnkhHelpBaseUrl", StringComparison.Ordinal)
                    && !string.IsNullOrWhiteSpace(item.Value))
                {
                    return item.Value.EndsWith("/", StringComparison.Ordinal)
                        ? item.Value
                        : item.Value + "/";
                }
            }

            return DefaultHelpBaseUrl;
        }

        internal static string GetHelpTopicPath(string dialogHelpTypeName)
        {
            string name = (dialogHelpTypeName ?? string.Empty).ToLowerInvariant();

            if (name.Contains("annotate") || name.Contains("blame"))
                return "annotate/";
            if (name.Contains("commonfileselectordialog") || name.Contains("unifieddiff"))
                return "diff/";
            if (name.Contains("merge"))
                return "merge/";
            if (name.Contains("commit") || name.Contains("pendingchanges") || name.Contains("changelist"))
                return "commit/";
            if (name.Contains("conflict") || name.Contains("resolve"))
                return "conflicts/";
            if (name.Contains("external"))
                return "externals/";
            if (name.Contains("checkout") || name.Contains("repository"))
                return "repository/";
            if (name.Contains("update") || name.Contains("switch") || name.Contains("revert") ||
                name.Contains("lock") || name.Contains("workingcopy") || name.Contains("cleanup"))
                return "working-copy/";
            if (name.Contains("sourcecontrol") || name.Contains("addtosubversion") ||
                name.Contains("solutionroot") || name.Contains(".scc.") || name.Contains("sccui"))
                return "source-control/";
            if (name.Contains("property"))
                return "properties/";
            if (name.Contains("issue"))
                return "issues/";
            if (name.Contains("proxy") || name.Contains("authentication") || name.Contains("tool") ||
                name.Contains("option") || name.Contains("setting"))
                return "settings/";
            if (name.Contains("error") || name.Contains("warning"))
                return "troubleshooting/";

            return string.Empty;
        }

        internal static Uri BuildHelpUri(string helpType, Version packageVersion, int lcid, string dialogHelpTypeName)
        {
            if (string.IsNullOrEmpty(helpType))
                throw new ArgumentNullException("helpType");
            if (packageVersion == null)
                throw new ArgumentNullException("packageVersion");

            Uri helpUri = new Uri(new Uri(GetHelpBaseUrl()), GetHelpTopicPath(dialogHelpTypeName));
            UriBuilder ub = new UriBuilder(helpUri);
            ub.Query = string.Format(
                CultureInfo.InvariantCulture,
                "t={0}&v={1}&l={2}&dt={3}",
                Uri.EscapeDataString(helpType),
                Uri.EscapeDataString(packageVersion.ToString()),
                lcid,
                Uri.EscapeDataString(dialogHelpTypeName ?? string.Empty));

            return ub.Uri;
        }

        public void RunHelp(VSDialogForm form)
        {
            Uri uri = BuildHelpUri(
                "dlgHelp",
                GetService<IAnkhPackage>().PackageVersion,
                CultureInfo.CurrentUICulture.LCID,
                form.DialogHelpTypeName);

            try
            {
                bool showHelpInBrowser = true;
                IVsHelpSystem help = GetService<IVsHelpSystem>(typeof(SVsHelpService));
                if (help != null)
                    showHelpInBrowser = !VSErr.Succeeded(help.DisplayTopicFromURL(uri.AbsoluteUri, (XCastUInt32)(uint)VHS_COMMAND.VHS_Default));

                if (showHelpInBrowser)
                    Help.ShowHelp(form, uri.AbsoluteUri);
            }
            catch (Exception ex)
            {
                IAnkhErrorHandler eh = GetService<IAnkhErrorHandler>();

                if (eh != null && eh.IsEnabled(ex))
                    eh.OnError(ex);
                else
                    throw;
            }
        }

        public void RunHelp(IAnkhControlWithHelp control)
        {
            Uri uri = BuildHelpUri(
                "ctrlHelp",
                GetService<IAnkhPackage>().PackageVersion,
                CultureInfo.CurrentUICulture.LCID,
                control.DialogHelpTypeName);

            try
            {
                bool showHelpInBrowser = true;
                IVsHelpSystem help = GetService<IVsHelpSystem>(typeof(SVsHelpService));
                if (help != null)
                    showHelpInBrowser = !VSErr.Succeeded(help.DisplayTopicFromURL(uri.AbsoluteUri, (XCastUInt32)(uint)VHS_COMMAND.VHS_Default));

                if (showHelpInBrowser)
                    Help.ShowHelp(control.Control, uri.AbsoluteUri);
            }
            catch (Exception ex)
            {
                IAnkhErrorHandler eh = GetService<IAnkhErrorHandler>();

                if (eh != null && eh.IsEnabled(ex))
                    eh.OnError(ex);
                else
                    throw;
            }
        }

        #endregion
    }
}
