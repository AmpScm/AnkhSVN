// $Id$
// P/Invoke declarations for OLE taken from 
// http://www.faisoncomputing.com/samples/programming_samples.htm
// with permission from the author
using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Utils.Ole
{
  [ComImport,
  Guid("00000112-0000-0000-C000-000000000046"),
  InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
  ]

  public interface IOleObject
  {
    void SetClientSite(IOleClientSite pClientSite);
    void GetClientSite(ref IOleClientSite ppClientSite);
    void SetHostNames(object szContainerApp, object szContainerObj);
    void Close(uint dwSaveOption);
    void SetMoniker(uint dwWhichMoniker, object pmk);
    void GetMoniker(uint dwAssign, uint dwWhichMoniker, object ppmk);
    void InitFromData(IDataObject pDataObject, bool fCreation, uint dwReserved);
    void GetClipboardData(uint dwReserved, ref IDataObject ppDataObject);
    void DoVerb(uint iVerb, uint lpmsg, object pActiveSite, uint lindex, uint hwndParent, uint lprcPosRect);
    void EnumVerbs(ref object ppEnumOleVerb);
    void Update();
    void IsUpToDate();
    void GetUserClassID(uint pClsid);
    void GetUserType(uint dwFormOfType, uint pszUserType);
    void SetExtent(uint dwDrawAspect, uint psizel);
    void GetExtent(uint dwDrawAspect, uint psizel);
    void Advise(object pAdvSink, uint pdwConnection);
    void Unadvise(uint dwConnection);
    void EnumAdvise(ref object ppenumAdvise);
    void GetMiscStatus(uint dwAspect,uint pdwStatus);
    void SetColorScheme(object pLogpal);
};
    
}
