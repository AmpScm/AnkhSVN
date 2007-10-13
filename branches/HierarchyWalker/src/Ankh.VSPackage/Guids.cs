// Guids.cs
// MUST match guids.h
using System;

namespace Ankh.VSPackage
{
    static class GuidList
    {
        public const string guidAnkh_VSPackagePkgString = "f7d21c5c-9c5e-444b-9f4f-3c8f1d4d3741";
        public static readonly Guid guidAnkh_VSPackagePkg = new Guid( guidAnkh_VSPackagePkgString );

        public const string guidAnkh_VSPackageCmdSetString = "5681e120-61af-40ed-beb3-0e59a3a22b93";
        public static readonly Guid guidAnkh_VSPackageCmdSet = new Guid(guidAnkh_VSPackageCmdSetString);

        public const string guidAnkhSccProviderServiceString = "99189f60-e93c-447b-8a38-e9f9eed41295";
        public static readonly Guid guidAnkhSccProviderService = new Guid(guidAnkhSccProviderServiceString);
    };
}