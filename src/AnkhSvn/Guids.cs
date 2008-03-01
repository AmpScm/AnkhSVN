using System;
using AnkhSvn.Ids;

namespace Ankh.VSPackage
{
    static class GuidList
    {
		public const string guidAnkhSvnPkgString = AnkhId.PackageId;
        public static readonly Guid guidAnkhSvnPkg = new Guid( guidAnkhSvnPkgString );

		public const string guidAnkhSvnCmdSetString = AnkhId.CommandSet;
        public static readonly Guid guidAnkhSvnCmdSet = new Guid(guidAnkhSvnCmdSetString);

        public const string guidAnkhSccProviderServiceString = "8770915B-B235-42ec-BBC6-8E93286E59B5";
        public static readonly Guid guidAnkhSccProviderService = new Guid( guidAnkhSccProviderServiceString );
    };
}