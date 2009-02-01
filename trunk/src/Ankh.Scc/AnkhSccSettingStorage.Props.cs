using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace Ankh.Scc
{
    static class SccProjectProps
    {

        public static readonly ReadOnlyCollection<string> All = new ReadOnlyCollection<string>(
            new HybridCollection<string>(
            new string[]
            {
            }, StringComparer.OrdinalIgnoreCase));
    }

    static class SccCategoryProps
    {
        public static readonly ReadOnlyCollection<string> All = new ReadOnlyCollection<string>(
            new HybridCollection<string>(
            new string[]
            {
            }, StringComparer.OrdinalIgnoreCase));
    }

    static class SccCategories
    {
        public static readonly ReadOnlyCollection<string> All = new ReadOnlyCollection<string>(
            new HybridCollection<string>(
            new string[]
            {
            }, StringComparer.OrdinalIgnoreCase));
    }

    partial class AnkhSccSettingStorage
    {
    }
}
