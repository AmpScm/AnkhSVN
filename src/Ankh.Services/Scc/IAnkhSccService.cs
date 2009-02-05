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
using Ankh.Selection;
using System.IO;

namespace Ankh.Scc
{
	/// <summary>
	/// 
	/// </summary>
    [CLSCompliant(false)]
	public interface IAnkhSccService
	{
		/// <summary>
		/// Gets a value indicating whether the Ankh Scc service is active
		/// </summary>
		/// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
		bool IsActive { get; }


        /// <summary>
        /// Gets or sets a boolean indicating whether te solution should be saved for changed scc settings
        /// </summary>
        bool IsSolutionDirty { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance has solution property data.
        /// </summary>
        bool HasSolutionData { get; }

        /// <summary>
        /// Called by the package when loading a managed solution
        /// </summary>
        /// <param name="asPrimarySccProvider">if set to <c>true</c> Ankh is marked as the primary SCC provider; otherwise it is running as second chair</param>
        void LoadingManagedSolution(bool asPrimarySccProvider);

        /// <summary>
        /// Marks the specified project as managed by the Scc provider
        /// </summary>
        /// <param name="project">A reference to the project or null for the solution</param>
        /// <param name="managed"></param>
        void SetProjectManaged(SvnProject project, bool managed);    

        /// <summary>
        /// Gets a boolean indicating whether the specified project (or the solution) is 
        /// managed by the Subversion Scc provider
        /// </summary>
        /// <param name="project">A reference to the project or null for the solution</param>
        /// <returns><c>true</c> if the solution is managed by the scc provider, otherwise <c>false</c></returns>
        bool IsProjectManaged(SvnProject project);

        /// <summary>
        /// Gets a value indicating whether this instance is solution managed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is solution managed; otherwise, <c>false</c>.
        /// </value>
        bool IsSolutionManaged { get; }

        /// <summary>
        /// Register the scc provider as primary scc provider in Visual Studio
        /// </summary>
        void RegisterAsPrimarySccProvider();


        /// <summary>
        /// Gets the glyph for a specific path
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        AnkhGlyph GetPathGlyph(string path);

        /// <summary>
        /// Writes the enlistment state to the solution
        /// </summary>
        /// <param name="propertyBag">The property bag.</param>
        void WriteSolutionProperties(IPropertyMap propertyBag);

        /// <summary>
        /// Loads the state of the enlistment.
        /// </summary>
        /// <param name="propertyBag">The property bag.</param>
        void ReadSolutionProperties(IPropertyMap propertyBag);

        /// <summary>
        /// Serializes the enlist data.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <param name="writeData">if set to <c>true</c> [write data].</param>
        void SerializeEnlistData(Stream store, bool writeData);

        /// <summary>
        /// Gets a boolean indicating whether to ignores the enumeration side effects flag on this project
        /// </summary>
        /// <param name="sccProject">The SCC project.</param>
        /// <returns></returns>
        bool IgnoreEnumerationSideEffects(Microsoft.VisualStudio.Shell.Interop.IVsSccProject2 sccProject);
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IPropertyMap : IDisposable
    {
        void SetValue(string key, string value);
        void SetRawValue(string key, string value);

        bool TryGetValue(string key, out string value);

        void SetQuoted(string key, string value);
        bool TryGetQuoted(string key, out string value);

        void Flush();
    }
}
