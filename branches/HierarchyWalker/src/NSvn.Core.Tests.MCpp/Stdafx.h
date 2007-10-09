// $Id$
// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently,
// but are changed infrequently

#pragma once

#using <mscorlib.dll>
#using <NSvn.Common.dll>

// In VS2003, we add a reference to the project, in 2002, the #using is needed
#if _MSC_VER < 1310
#using <NUnit.Framework.dll>
#endif


