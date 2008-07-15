﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Ankh.Scc
{
    interface ISvnWcReference
    {
        SvnWorkingCopy WorkingCopy { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    [DebuggerDisplay("Path={FullPath}")]
    public sealed class SvnWorkingCopy : IEquatable<SvnWorkingCopy>, ISvnWcReference
    {
        readonly SvnItem _rootItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="SvnWorkingCopy"/> class.
        /// </summary>
        /// <param name="rootItem">The root item.</param>
        public SvnWorkingCopy(SvnItem rootItem)
        {
            if (rootItem == null)
                throw new ArgumentNullException("rootItem");

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
        bool IEquatable<SvnWorkingCopy>.Equals(SvnWorkingCopy other)
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

        internal static ISvnWcReference CalculateWorkingCopy(SvnItem svnItem)
        {
            if (svnItem == null)
                throw new ArgumentNullException("svnItem");

            // We can assume the SvnItem is a directory; this is verified in SvnItem.WorkingCopy
            SvnItem parent = svnItem.Parent;

            if (parent != null)
            {
                if (!svnItem.IsVersioned)
                    return parent;

                if (parent.IsVersioned && !parent.IsNestedWorkingCopy)
                    return parent;
            }

            return new SvnWorkingCopy(svnItem);
        }
    }
}
