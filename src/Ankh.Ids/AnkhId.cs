// $Id$
//
// Copyright 2008-2009 The AnkhSVN Project
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

namespace Ankh
{
    /// <summary>
    /// Container of guids used by the package and command framework
    /// </summary>
    public static class AnkhId
    {
        //************ The Package Load Key Registration ***********************************
        /// <summary>
        /// The guid the AnkhSvn package is registered with inside Visual Studio
        /// </summary>
        public const string PackageId = "604ad610-5cf9-4bd5-8acc-f49810e2efd4";
        public const string PackageDescription = "AnkhSVN - Subversion Support for Visual Studio";

        /// <summary>The package version as used in the PLK</summary>
        public const string PlkVersion = "2.0";
        /// <summary>The product name as used in the PLK</summary>
        public const string PlkProduct = "AnkhSVN";
        /// <summary>The company name as used in the PLK</summary>
        public const string PlkCompany = "AnkhSVN Core Team";
        //**********************************************************************************

        public const string AssemblyCopyright = "Copyright © AnkhSVN Team 2003-2015";
        public const string AssemblyProduct = "AnkhSVN - Subversion Support for Visual Studio";
        public const string AssemblyCompany = "AnkhSVN Team";
        //**********************************************************************************

        public const string WpfPackageId = "7e64684f-d9bb-4e26-84db-e95a09307389";
        public const string WpfPackageDescription = "AnkhSVN - Subversion Support for Visual Studio - Wpf Editor Support";

        /// <summary>The Subversion SCC Provider name as used in the solution file</summary>
        public const string SubversionSccName = "SubversionScc";
        public const string SvnOriginName = "SvnOrigin";
        public const string SccTranslateStream = SubversionSccName + "_SccTranslate";

        // Items for the VS 2010 Extension registration
        public const string ExtensionTitle = AnkhId.AssemblyProduct;
        public const string ExtensionAuthor = AnkhId.AssemblyCompany;
        public const string ExtensionDescription = "Open Source Subversion SCC Provider for Visual Studio 2005, 2008, 2010 and 2012.";
        public const string ExtensionMoreInfoUrl = "http://www.ankhsvn.net/";
        public const string ExtensionGettingStartedUrl = "http://www.ankhsvn.net/";
        public const string ExtensionReleaseNotesUrl = "http://ankhsvn.net/releasenotes";

        /// <summary>
        /// The guid used for registering the commands registered by the AnkhSvn package
        /// </summary>
        /// <remarks>Must be changed when the PackageId changes</remarks>
        public const string CommandSet = "aa61c329-d559-468f-8f0f-4f03896f704d";

        /// <summary>The SCC Provider guid (used as SCC active marker by VS)</summary>
        public const string SccProviderId = "8770915b-b235-42ec-bbc6-8e93286e59b5";
        /// <summary>The GUID of the SCC Service</summary>
        public const string SccServiceId = "d8c473d2-9634-4513-91d5-e1a671fe2df4";
        /// <summary>Language neutral SCC Provider title</summary>
        public const string SccProviderTitle = "AnkhSVN - Subversion Support for Visual Studio";

        //**********************************************************************************

        // Increase this value when you want to have AnkhCommand.MigrateSettings called on first use
        public const int MigrateVersion = 5;

        public const string AnkhServicesAvailable = "ed044a39-ac7b-4617-a466-a7c4ffa2998d";
        public const string AnkhRuntimeStarted = "8057b1af-21d8-4276-ac27-9c02a1f95bc7";

        public const string LogMessageLanguageServiceId = "1dfe69ce-7f9b-4cc5-b09b-e5bde95e9439";
        public const string LogMessageServiceName = "AnkhSVN Log Messages";
        public const string LogMessageRegistryName = "AnkhLogMessages";

        public const string UnifiedDiffLanguageServiceId = "9e5ab2dc-5a41-4d39-ab71-5dca5908aa32";
        public const string UnifiedDiffServiceName = "AnkhSVN Unified Diff";
        public const string UnifiedDiffRegistryName = "AnkhUnifiedDiff";

        public const string TriggerExtenderGuid = "83b9f93b-20e9-4ce2-8b1b-bd18f14df582";
        public const string TriggerExtenderName = "AnkhExtenderProvider";
        public const string SccExtenderGuid = "5e7f5850-6409-49f3-abf4-e9e87c011d00";

        /// <summary>
        /// The guid used for our one-and-only bitmap resource
        /// </summary>
        public const string BmpId = "9db594ca-ebdd-40e1-9e37-51b7f9ef8df0";
        public const string ExtensionRedirectId = "3cbbbf2e-22f7-4f02-af63-892bd2120485";


        public const string AnkhOutputPaneId = "ba0eec02-577c-424e-b6aa-fc8499d917ba";


        public const string RepositoryExplorerToolWindowId = "748becbe-04a1-4ffa-8e1e-46840f91a083";
        public const string PendingChangesToolWindowId = "896e815d-3862-4978-a1bc-cb6a3e70045c";
        public const string WorkingCopyExplorerToolWindowId = "a1929d7e-610a-48b0-8152-8e4aa202427f";
        public const string BlameToolWindowId = "a543ea62-696c-4c7c-ab42-78bd7267da92";
        public const string LogToolWindowId = "2fac1ebf-6b37-4be3-9a44-f9ed32d561cd";
        public const string SvnInfoToolWindowId = "c3630016-f162-4af5-b165-9f468a642e9a";

        public const string PendingChangeViewContext = "a02c6d65-1f8b-46ac-8db3-511ae5dba374";
        public const string DiffMergeViewContext = "6e66dfc4-cb72-4023-bdf5-6b139df2f19b";
        public const string SccExplorerViewContext = "fd4828f6-49a3-4e2f-bf36-8304884219ca";
        public const string LogViewContext = "0d2ca125-6cdf-407a-9c03-e8427e27f34d";

        public const string AnnotateContext = "61ade608-f2f9-4983-ae28-623de0dca5c6";
        public const string DynamicEditorId = "923f6990-98c1-4dd5-983e-25088c02975d";

        public const string EnvironmentSettingsPageGuid = "f362b52e-e8f8-43de-a02d-5072a2a96e6a";
        public const string UserToolsSettingsPageGuid = "ae1a4db8-09c3-438d-93bd-0fe37412ce34";
        public const string IssueTrackerSettingsPageGuid = "363b3e68-ee94-403d-88ba-681a7cad247a";

        public const string LanguagePreferencesId = "c4cc1bd2-a679-4746-a09e-3e83e8906d7e";
        public const string EditorFactoryId = "72faba81-d5b6-430c-bd63-b938b3dc0ced";

        /// <summary>
        /// The command set as a guid
        /// </summary>
        public static readonly Guid CommandSetGuid = new Guid(CommandSet);

        /// <summary>
        /// The package is as a guid
        /// </summary>
        public static readonly Guid PackageGuid = new Guid(PackageId);

        /// <summary>
        /// The guid for the generated Bmp
        /// </summary>
        public static readonly Guid BmpGuid = new Guid(BmpId);

        public static readonly Guid SccProviderGuid = new Guid(SccProviderId);
        public static readonly Guid SccServiceGuid = new Guid(SccServiceId);

        public static readonly Guid PendingChangeContextGuid = new Guid(PendingChangeViewContext);
        public static readonly Guid DiffMergeContextGuid = new Guid(DiffMergeViewContext);
        public static readonly Guid SccExplorerContextGuid = new Guid(SccExplorerViewContext);
        public static readonly Guid LogContextGuid = new Guid(LogViewContext);
        public static readonly Guid AnnotateContextGuid = new Guid(AnnotateContext);
    }
}
