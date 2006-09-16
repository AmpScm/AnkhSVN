// $Id$
// P/Invoke declarations for OLE taken from 
// http://www.faisoncomputing.com/samples/programming_samples.htm
// with permission from the author
using System;
using System.Runtime.InteropServices;
using stdole;

namespace Utils.Ole
{
	/// <summary>
	/// By implementing this class, a WebBrowser controlling host component can expose
	/// any number of methods to HTML pages. From script on a page, you can call one
	/// of the ICustomMethods like this:
	/// 
  //  <SCRIPT language="JScript">
  //  function MyFunc(iSomeData)
  //  {
  //    external.ShowMyDialogBox();
  //  }
  //  </SCRIPT>
  ///
  /// arguments may also be passed to custom methods, using strings, ints, objects or other.
	/// </summary>
	public interface ICustomMethods : IDispatch
	{
		void MyCustomMethod([MarshalAs(UnmanagedType.BStr)] string theCaption);

    // ...add any other methods to be accessed from HTML pages
	}
}
