using System;

namespace NSvn.Common
{
	/// <summary>
	/// Represents a login credential, ie a username/password combination
	/// </summary>
	public class Credential
	{
		public Credential( string username, string password )
		{
			this.username = username;
            this.password = password;
		}

        /// <summary>
        /// The username
        /// </summary>
        public string Username
        {
            get{ return this.username; }
        }

        /// <summary>
        /// the password
        /// </summary>
        public string Password
        {
            get{ return this.password; }
        }

        /// <summary>
        /// A special credential used to flag that this provider has
        /// exhausted all its attempts
        /// </summary>
        public static readonly Credential InvalidCredential = 
            new Credential( "", "" );

        private string username;
        private string password;

	}
}
