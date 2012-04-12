using System;
using System.Collections.Generic;
using System.Text;
using Ankh.VSPackage.Attributes;
using Ankh.Scc.ProjectMap;

namespace Ankh.VSPackage
{
	[ProvideProjectTypeSettings("{d2abab84-bf74-430a-b69e-9dc6d40dda17}", SccProjectFlags.ForceSccGlyphChange)] // .asproj, .dwproj
	[ProvideProjectTypeSettings("{d183a3d8-5fd8-494b-b014-37f57b35e655}", SccProjectFlags.ForceSccGlyphChange)] // .dtproj (SQL2008, SQL2008-R2)
    [ProvideProjectTypeSettings("{159641d6-6404-4a2a-ae62-294de0fe8301}", SccProjectFlags.ForceSccGlyphChange)] // .dtproj (SQL2012)
	[ProvideProjectTypeSettings("{f14b399a-7131-4c87-9e4b-1186c45ef12d}", SccProjectFlags.ForceSccGlyphChange)] // .rptproj
	[ProvideProjectTypeSettings("{999d2cb9-9277-4465-a902-1604ed3686a3}", SccProjectFlags.ForceSccGlyphChange)] // .smdlproj
	partial class AnkhSvnPackage
	{
	}
}
