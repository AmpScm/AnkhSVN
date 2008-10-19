using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;
using System.ComponentModel.Design;

namespace Ankh.VS.LanguageServices
{
    [CLSCompliant(false)]
    public abstract partial class AnkhLanguageService : LanguageService, IAnkhServiceImplementation, IAnkhServiceProvider
    {
        readonly IAnkhServiceProvider _context;

        protected AnkhLanguageService(IAnkhServiceProvider context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            _context = context;
        }

        public virtual void OnPreInitialize()
        {
            // Initialize the language service api
            SetSite(GetService<IServiceContainer>());
        }

        public virtual void OnInitialize()
        {

        }

        protected internal IAnkhServiceProvider Context
        {
            get { return _context; }
        }

        #region IAnkhServiceProvider Members

        public T GetService<T>() where T : class
        {
            return Context.GetService<T>();
        }

        public T GetService<T>(Type serviceType) where T : class
        {
            return Context.GetService<T>(serviceType);
        }

        #endregion
    }

    abstract partial class AnkhViewFilter : ViewFilter
    {
        protected AnkhViewFilter(CodeWindowManager mgr, IVsTextView view)
            : base(mgr, view)
        {
        }
        public virtual void PrepareContextMenu(ref int menuId, ref Guid groupGuid, ref Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget target)
        {
            
        }
    }
}
