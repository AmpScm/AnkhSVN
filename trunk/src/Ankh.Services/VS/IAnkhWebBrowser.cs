using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.VS
{
    public interface IAnkhWebBrowser
    {
        void Navigate(Uri url);
        void Navigate(Uri url, BrowserArgs args);
        void Navigate(Uri url, BrowserArgs args, out BrowserResults results);
    }

    public class BrowserArgs
    {
        __VSCREATEWEBBROWSER _createFlags;
        string _baseCaption;

        public string BaseCaption
        {
            get { return _baseCaption; }
            set { _baseCaption = value; }
        }

        [CLSCompliant(false)]
        public __VSCREATEWEBBROWSER CreateFlags
        {
            get { return _createFlags; }
            set { _createFlags = value; }
        }
    }

    public abstract class BrowserResults
    {
        [CLSCompliant(false)]
        public virtual IVsWebBrowser WebBrowser 
        { 
            get { throw new NotImplementedException(); }
        }
        [CLSCompliant(false)]
        public virtual IVsWindowFrame Frame
        {
            get { throw new NotImplementedException(); }
        }
    }
}
