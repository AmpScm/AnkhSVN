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

//************ The Package Load Key Registration ***********************************
		/// <summary>
		/// The guid the AnkhSvn package is registered with inside Visual Studio
		/// </summary>
		public const string PackageId  = "604ad610-5cf9-4bd5-8acc-f49810e2efd4";

        public const string PlkVersion = "2.0";
        public const string PlkProduct = "AnkhSvn";
        public const string PlkCompany = "AnkhSvn";
//**********************************************************************************

		/// <summary>
		/// The guid used for registering the commands registered by the AnkhSvn package
		/// </summary>
		/// <remarks>Must be changed when the PackageId changes</remarks>
		public const string CommandSet = "aa61c329-d559-468f-8f0f-4f03896f704d";


        public const string SccProviderId = "8770915b-b235-42ec-bbc6-8e93286e59b5";   

		/// <summary>
		/// The guid used for our on-and-only bitmap resource
		/// </summary>
		public const string BmpId = "9db594ca-ebdd-40e1-9e37-51b7f9ef8df0";


		public const string AnkhOutputPaneId = "ba0eec02-577c-424e-b6aa-fc8499d917ba";
		

        public const string RepositoryExplorerToolWindowId = "748becbe-04a1-4ffa-8e1e-46840f91a083";
        public const string PendingChangesToolWindowId = "896e815d-3862-4978-a1bc-cb6a3e70045c";
        public const string WorkingCopyExplorerToolWindowId = "a1929d7e-610a-48b0-8152-8e4aa202427f";



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

		public static readonly Guid AnkhOutputPaneGuid = new Guid(AnkhOutputPaneId);

        public static readonly Guid SccProviderGuid = new Guid(SccProviderId);
	}
}
