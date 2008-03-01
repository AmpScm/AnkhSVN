using System;
using AnkhSvn.Ids;

namespace Ankh.VSPackage
{
    static class GuidList
    {
		public const string guidAnkhSvnPkgString = AnkhId.PackageId;
		public const string guidAnkhSvnCmdSetString = AnkhId.CommandSet;

        public static readonly Guid guidAnkhSvnCmdSet = new Guid(guidAnkhSvnCmdSetString);
    };
}