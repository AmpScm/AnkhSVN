// $Id$
// P/Invoke declarations for OLE taken from 
// http://www.faisoncomputing.com/samples/programming_samples.htm
// with permission from the author
using System;
using System.Runtime.InteropServices;

namespace Utils.Ole
{
  [ComImport,
  Guid("b722bcc7-4e68-101b-a2bc-00aa00404770"),
  InterfaceType(ComInterfaceType.InterfaceIsIUnknown) ]
  public interface IOleDocumentSite
  {
    void ActivateMe(ref object pViewToActivate);
  }
}
