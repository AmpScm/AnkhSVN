// $Id$
//
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
using Microsoft.VisualStudio.OLE.Interop;

namespace Ankh.Scc
{
    [CLSCompliant(false)]
    public interface ISccSettingsStore
    {
        /// <summary>
        /// Gets a boolean indicating whether te solution should be saved for changed scc settings
        /// </summary>
        bool IsSolutionDirty { get; }
        /// <summary>
        /// Gets a boolean indicating whether data should be saved in the solution.
        /// </summary>
        bool HasSolutionData { get; }

        /// <summary>
        /// Gets a <see cref="IPropertyMap"/> for the specified property bag
        /// </summary>
        /// <param name="pPropBag">The p prop bag.</param>
        /// <returns></returns>
        IPropertyMap GetMap(IPropertyBag propertyBag);

        void WriteSolutionProperties(IPropertyMap map);

        void ReadSolutionProperties(IPropertyMap map);
    }

    public interface ISccStoreMap
    {
        /// <summary>
        /// Gets the project property.
        /// </summary>
        /// <param name="project">The actual name of the project</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        string GetProjectProperty(string project, string key);
        /// <summary>
        /// Sets the project property.
        /// </summary>
        /// <param name="project">The actual name of the project</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value or null to clear the property.</param>
        void SetProjectProperty(string project, string key, string value);

        /// <summary>
        /// Gets the category property.
        /// </summary>
        /// <param name="categoryId">The category id.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        string GetCategoryProperty(string categoryId, string key);
        /// <summary>
        /// Sets the category property.
        /// </summary>
        /// <param name="categoryId">The category id.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value or null to clear the property</param>
        void SetCategoryProperty(string categoryId, string key, string value);

        /// <summary>
        /// Gets the project category.
        /// </summary>
        /// <param name="project">The actual name of the project</param>
        /// <param name="category">The category.</param>
        /// <returns></returns>
        string GetProjectCategory(string project, string category);
        /// <summary>
        /// Sets the project category.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="category">The category.</param>
        /// <param name="categoryId">The category id.</param>
        void SetProjectCategory(string project, string category, string categoryId);

        /// <summary>
        /// Creates the category.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        string CreateCategory(string name);
    }
}
