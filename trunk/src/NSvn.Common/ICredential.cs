using System;

namespace NSvn.Common
{
	/// <summary>
	/// Summary description for ICredential.
	/// </summary>
	public interface ICredential
    {
        string Kind 
        { 
            get;
        }
            
        IntPtr GetCredential( IntPtr pool );
    }
}
