// $Id$
using System;

namespace NSvn.Common
{
    /// <summary>
    /// Represents a credential to be returned from an IAuthenticationProvider
    /// </summary>
    public interface ICredential
    {
        /// <summary>
        /// For internal use - creates an svn credential from this object
        /// </summary>
        IntPtr GetCredential( IntPtr pool );

        string Kind
        { get; }
    }
}
