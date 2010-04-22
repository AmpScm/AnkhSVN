using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Ankh
{
    public static class AnkhCatId
    {
        public const string CscFileBrowse = "{8D58E6AF-ED4E-48B0-8C7B-C74EF0735451}";
        public const string CscFolderBrowse = "{914FE278-054A-45DB-BF9E-5F22484CC84C}";
        public const string CscProjectBrowse = "{4EF9F003-DE95-4D60-96B0-212979F2A857}";
        public const string VbFileBrowse = "{EA5BD05D-3C72-40A5-95A0-28A2773311CA}";
        public const string VbFolderBrowse = "{932DC619-2EAA-4192-B7E6-3D15AD31DF49}";
        public const string VbProjectBrowse = "{E0FDC879-C32A-4751-A3D3-0B3824BD575F}";
        public const string VjFileBrowse = "{E6FDF869-F3D1-11D4-8576-0002A516ECE8}";
        public const string VjFolderBrowse = "{E6FDF86A-F3D1-11D4-8576-0002A516ECE8}";
        public const string VjProjectBrowse = "{E6FDF86C-F3D1-11D4-8576-0002A516ECE8}";
        public const string SolutionBrowse = "{A2392464-7C22-11D3-BDCA-00C04F688E50}";
        public const string CppFileBrowse = "{EE8299C9-19B6-4F20-ABEA-E1FD9A33B683}";
        public const string CppProjectBrowse = "{EE8299CB-19B6-4F20-ABEA-E1FD9A33B683}";
        public const string GenericProject = "{610D4611-D0D5-11D2-8599-006097C68E81}";

        // From VSLangProj.dll
        public const string ExtProjectBrowse = "{610D4614-D0D5-11D2-8599-006097C68E81}";
        public const string ExtFileBrowse = "{610D4615-D0D5-11D2-8599-006097C68E81}";


        [ComVisible(true)]
        public interface IAnkhInternalExtenderProvider
        {
            bool CanExtend(object extendeeObject, string catId);
            object GetExtender(object extendeeObject, string catId, IDisposable disposer);
        }
    }
}
