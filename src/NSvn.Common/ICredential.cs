using System;

namespace NSvn.Common
{
	/// <summary>
	/// Represents a credential to be returned from an IAuthenticationProvider
	/// </summary>
	public interface ICredential
    {
        /// <summary>
        /// The type of credential
        /// </summary>
        string Kind 
        { 
            get;
        }
            
        /// <summary>
        /// For internal use - creates an svn credential from this object
        /// </summary>
        IntPtr GetCredential( IntPtr pool );
    }
}
