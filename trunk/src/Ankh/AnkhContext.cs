using System;
using EnvDTE;

namespace Ankh
{
	/// <summary>
	/// General context object for the Ankh addin. Contains pointers to objects
	/// required by commands.
	/// </summary>
	internal class AnkhContext
	{
		public AnkhContext( EnvDTE._DTE dte, EnvDTE.AddIn addin )
		{
			this.dte = dte;
            this.addin = addin;
		}

        /// <summary>
        /// The top level automation object.
        /// </summary>
        public EnvDTE._DTE DTE
        {
            get{ return this.dte; }
        }

        /// <summary>
        /// The addin object.
        /// </summary>
        public EnvDTE.AddIn AddIn
        {
            get{ return this.addin; }
        }


        private EnvDTE._DTE dte;
        private EnvDTE.AddIn addin;

	}
}
