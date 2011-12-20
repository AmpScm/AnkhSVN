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

        public const string AssemblyCopyright = "Copyright © AnkhSVN Team 2003-2011";
        public const string AssemblyProduct = "AnkhSVN - Subversion Support for Visual Studio";
        public const string AssemblyCompany = "AnkhSVN Team";
        //**********************************************************************************

        /// <summary>The Subversion SCC Provider name as used in the solution file</summary>
        public const string SubversionSccName = "SubversionScc";
        public const string SccStructureName = "SccStructure";

        // Items for the VS 2010 Extension registration
        public const string ExtensionTitle = AnkhId.AssemblyProduct;
        public const string ExtensionAuthor = AnkhId.AssemblyCompany;
        public const string ExtensionDescription = "Open Source Subversion SCC Provider for Visual Studio 2005, 2008 and 2010.";
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


        //************ Special contexts managed by our Trigger package *********************        
        /// <summary>
        /// Context set by Trigger package when another Scc package is active
        /// </summary>
        public const string CtxNoOtherSccActive = "3eec2d0e-8224-4d68-9748-773e2ace8dda";
        public const string CtxNoOtherSccManaging = "644c07df-048b-44c5-aeff-cf54ad82a209";
        
        // When this context is set the full package is loaded and the trigger package is deactivated
        public const string CtxFullSccLoaded = "1142ad52-4c53-499a-afac-3f3694261f5d";
        //**********************************************************************************

        // Increase this value when you want to have AnkhCommand.MigrateSettings called on first use
        public const int MigrateVersion = 5;

        public const string AnkhServicesAvailable = "ED044A39-AC7B-4617-A466-A7C4FFA2998D";
        public const string AnkhRuntimeStarted = "8057B1AF-21D8-4276-AC27-9C02A1F95BC7";

        public const string LogMessageLanguageServiceId = "1DFE69CE-7F9B-4CC5-B09B-E5BDE95E9439";
        public const string LogMessageServiceName = "AnkhSVN Log Messages";

        public const string UnifiedDiffLanguageServiceId = "9E5AB2DC-5A41-4D39-AB71-5DCA5908AA32";
        public const string UnifiedDiffServiceName = "AnkhSVN Unified Diff";

        public const string TriggerExtenderGuid = "83B9F93B-20E9-4CE2-8B1B-BD18F14DF582";
        public const string TriggerExtenderName = "AnkhExtenderProvider";
        public const string SccExtenderGuid = "5E7F5850-6409-49F3-ABF4-E9E87C011D00";

        /// <summary>
        /// The guid used for our on-and-only bitmap resource
        /// </summary>
        public const string BmpId = "9DB594CA-EBDD-40E1-9E37-51B7F9EF8DF0";


        public const string AnkhOutputPaneId = "BA0EEC02-577C-424E-B6AA-FC8499D917BA";


        public const string RepositoryExplorerToolWindowId = "748BECBE-04A1-4FFA-8E1E-46840F91A083";
        public const string PendingChangesToolWindowId = "896E815D-3862-4978-A1BC-CB6A3E70045C";
        public const string WorkingCopyExplorerToolWindowId = "A1929D7E-610A-48B0-8152-8E4AA202427F";
        public const string BlameToolWindowId = "A543EA62-696C-4C7C-AB42-78BD7267DA92";
        public const string LogToolWindowId = "2FAC1EBF-6B37-4BE3-9A44-F9ED32D561CD";
        public const string DiffToolWindowId = "8B17630D-72A5-43AE-8105-DB31004D08AD";
        public const string SvnInfoToolWindowId = "C3630016-F162-4AF5-B165-9F468A642E9A";

        public const string PendingChangeViewContext = "A02C6D65-1F8B-46AC-8DB3-511AE5DBA374";
        public const string DiffMergeViewContext = "6E66DFC4-CB72-4023-BDF5-6B139DF2F19B";
        public const string SccExplorerViewContext = "FD4828F6-49A3-4E2F-BF36-8304884219CA";
        public const string LogViewContext = "0D2CA125-6CDF-407A-9C03-E8427E27F34D";

        public const string AnnotateContext = "61ADE608-F2F9-4983-AE28-623DE0DCA5C6";
        public const string DiffEditorId = "7C6FACCE-0C14-4A3E-BC69-15F3966EE312";
        public const string DynamicEditorId = "923F6990-98C1-4DD5-983E-25088C02975D";
        public const string DiffEditorViewId = "3D9F7A9D-F9D1-4DD6-A89F-C89312708923";

        public const string EnvironmentSettingsPageGuid = "F362B52E-E8F8-43DE-A02D-5072A2A96E6A";
        public const string UserToolsSettingsPageGuid = "AE1A4DB8-09C3-438D-93BD-0FE37412CE34";
        public const string IssueTrackerSettingsPageGuid = "363B3E68-EE94-403D-88BA-681A7CAD247A";

        public const string LanguagePreferencesId = "C4CC1BD2-A679-4746-A09E-3E83E8906D7E";
        public const string EditorFactoryId = "72FABA81-D5B6-430C-BD63-B938B3DC0CED";

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
