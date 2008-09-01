// $Id$
// P/Invoke declarations for OLE taken from 
// http://www.faisoncomputing.com/samples/programming_samples.htm
// with permission from the author

using System;
using System.Runtime.InteropServices;

namespace Utils.Ole
{
	/// <summary>
	/// Summary description for ICustomDoc.
	/// </summary>
  [ComImport(),
  InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
  GuidAttribute("3050f3f0-98b5-11cf-bb82-00aa00bdce0b")]
  public interface ICustomDoc
  {
    [PreserveSig]
    void SetUIHandler(IDocHostUIHandler theUIHandler);
  }
}
