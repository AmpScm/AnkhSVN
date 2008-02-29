using System;
using System.Collections.Generic;
using System.Text;

namespace AnkhSvn.Ids
{
	/// <summary>
	/// Container of guids used by the package and command framework
	/// </summary>
	public static class AnkhId
	{
		// These guid's are used via this definition 
		// through the Package, the Gui, etc.

		// Changing these corrupts all kinds of registrations
		/// <summary>
		/// The guid the AnkhSvn package is registered with inside Visual Studio
		/// </summary>
		/// <remarks>Must match the GUID in the PLK. If you might ever change this guid 
		/// you must also change the <see cref="CommandSet"/> guid</remarks>
		public const string PackageId  = "604ad610-5cf9-4bd5-8acc-f49810e2efd4";
		/// <summary>
		/// The guid used for registering the commands registered by the AnkhSvn package
		/// </summary>
		/// <remarks>Must be changed when the PackageId changes</remarks>
		public const string CommandSet = "aa61c329-d559-468f-8f0f-4f03896f704d";

		/// <summary>
		/// 
		/// </summary>
		public const string BmpId = "9db594ca-ebdd-40e1-9e37-51b7f9ef8df0";
		


		/// <summary>
		/// The command set as a guid
		/// </summary>
		public static readonly Guid CommandSetGuid = new Guid(CommandSet);

		/// <summary>
		/// The package is as a guid
		/// </summary>
		public static readonly Guid PackageGuid = new Guid(PackageId);

		/// <summary>
		/// The guid for the generated Bmp
		/// </summary>
		public static readonly Guid BmpGuid = new Guid(BmpId);
	}
}
