// Copyright 2008 The AnkhSVN Project
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

namespace Ankh.Commands
{
    public interface IAnkhCommandStates
    {
        bool UIShellAvailable { get; }

        bool CodeWindow { get; }

        bool Debugging { get; }

        bool DesignMode { get; }

        bool Dragging { get; }

        bool EmptySolution { get; }

        bool FullScreenMode { get; }

        bool NoSolution { get; }

        bool SolutionBuilding { get; }

        bool SolutionExists { get; }

        bool SolutionHasMultipleProjects { get; }

        bool SolutionHasSingleProject { get; }

        bool SccManagerLoaded { get; }

        bool SccProviderActive { get; }

        bool OtherSccProviderActive { get; }

        bool SccEnlistingInProject { get; }

        bool SccEnableOpenFromScc { get; }

        bool NotBuildingAndNotDebugging { get; }

        bool SolutionOrProjectUpgrading { get; }

        // bool DataSourceWindowSupported { get; }

        bool DataSourceWindowAutoVisible { get; }

        bool ToolboxInitialized { get; }

        bool SolutionExistsAndNotBuildingAndNotDebugging { get; }

        /// <summary>VS2010+</summary>
        bool SolutionExistsAndFullyLoaded { get; }

        /// <summary>VS2010+</summary>
        bool SolutionOpening { get; }

        /// <summary>VS2010+</summary>
        bool ProjectRetargeting { get; }

        /// <summary>VS2010+</summary>
        bool HistoricalDebugging { get; }

        /// <summary>VS11+</summary>
        bool FirstLaunchSetup { get; }

        /// <summary>VS11+</summary>
        bool BackgroundProjectLoad { get; }

        bool SolutionExplorerActive { get; }

        bool ClassViewerActive { get; }

        bool PendingChangesActive { get; }

        /// <summary>VS11+</summary>
        bool ThemeLight { get; }

        /// <summary>VS11+</summary>
        bool ThemeDark { get; }

        /// <summary>VS11+</summary>
        bool ThemeDefined { get; }

        bool ShiftDown { get; }
    }
}
