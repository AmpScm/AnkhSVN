// $Id$
//
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
using System.Diagnostics;
using SharpSvn;

namespace Ankh.Scc
{
    interface ISvnWcReference
    {
        SvnWorkingCopy WorkingCopy { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    [DebuggerDisplay("WorkingCopy={FullPath}")]
    public sealed class SvnWorkingCopy : IEquatable<SvnWorkingCopy>, ISvnWcReference
    {
        readonly SvnItem _rootItem;
        IAnkhServiceProvider _context;
        bool _checkedUri;
        Uri _repositoryRoot;
        bool _checkedId;
        Guid? _reposId;

        /// <summary>
        /// Initializes a new instance of the <see cref="SvnWorkingCopy"/> class.
        /// </summary>
        /// <param name="rootItem">The root item.</param>
        SvnWorkingCopy(IAnkhServiceProvider context, SvnItem rootItem)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            else if (rootItem == null)
                throw new ArgumentNullException("rootItem");

            _context = context;
            _rootItem = rootItem;
        }

        /// <summary>
        /// Gets the full path.
        /// </summary>
        /// <value>The full path.</value>
        public string FullPath
        {
            get { return _rootItem.FullPath; }
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">The <paramref name="obj"/> parameter is null.</exception>
        public override bool Equals(object obj)
        {
            return Equals(obj as SvnWorkingCopy);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode(FullPath);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public bool Equals(SvnWorkingCopy other)
        {
            if((object)other == null)
                return false;
            
            return StringComparer.OrdinalIgnoreCase.Equals(other.FullPath, FullPath);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="wc1">The WC1.</param>
        /// <param name="wc2">The WC2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(SvnWorkingCopy wc1, SvnWorkingCopy wc2)
        {
            bool n1 = (object)wc1 == null;
            bool n2 = (object)wc2 == null;

            if (n1 || n2)
                return n1 && n2;

            return wc1.Equals(wc2);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="wc1">The WC1.</param>
        /// <param name="wc2">The WC2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(SvnWorkingCopy wc1, SvnWorkingCopy wc2)
        {
            bool n1 = (object)wc1 == null;
            bool n2 = (object)wc2 == null;

            if (n1 || n2)
                return !(n1 && n2);

            return !wc1.Equals(wc2);
        }

        SvnWorkingCopy ISvnWcReference.WorkingCopy
        {
            get { return this; }
        }

        internal static ISvnWcReference CalculateWorkingCopy(IAnkhServiceProvider context, SvnItem svnItem)
        {
            if (svnItem == null)
                throw new ArgumentNullException("svnItem");

            // We can assume the SvnItem is a directory; this is verified in SvnItem.WorkingCopy
            SvnItem parent = svnItem.Parent;

            if (parent != null)
            {
                if (!svnItem.IsVersioned)
                    return parent;

                if (parent.IsVersioned && !svnItem.IsNestedWorkingCopy)
                    return parent;
            }

            return new SvnWorkingCopy(context, svnItem);
        }

        public Uri RepositoryRoot
        {
            get { return _repositoryRoot ?? GetRepositoryRoot(); }
        }

        private Uri GetRepositoryRoot()
        {
            if (_checkedUri)
                return null;


            _checkedUri = true;
            using (SvnClient client = _context.GetService<ISvnClientPool>().GetNoUIClient())
            {
                return _repositoryRoot = client.GetRepositoryRoot(FullPath);
            }
        }

        public Guid RepositoryId
        {
            get { return _reposId ?? GetReposId(); }
        }

        private Guid GetReposId()
        {
            if (_checkedId)
                return Guid.Empty;

            _checkedId = true;

            // Theoretically this can connect the server (if upgraded from a really old workingcopy)
            using (SvnClient client = _context.GetService<ISvnClientPool>().GetClient())
            {
                Guid value;

                if (client.TryGetRepositoryId(FullPath, out value))
                {
                    _reposId = value;
                    return value;
                }

                return Guid.Empty;
            }
        }
    }
}
