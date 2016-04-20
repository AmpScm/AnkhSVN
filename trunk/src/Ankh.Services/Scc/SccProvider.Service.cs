using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using IObjectWithSite = Microsoft.VisualStudio.OLE.Interop.IObjectWithSite;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using IOleConnectionPoint = Microsoft.VisualStudio.OLE.Interop.IConnectionPoint;
using IOleConnectionPointContainer = Microsoft.VisualStudio.OLE.Interop.IConnectionPointContainer;


namespace Ankh.Scc
{
    partial class SccProvider : IAnkhServiceProvider, IAnkhServiceImplementation
    {
        readonly IAnkhServiceProvider _context;

        void IAnkhServiceImplementation.OnInitialize()
        {
            OnInitialize();
        }

        void IAnkhServiceImplementation.OnPreInitialize()
        {
            OnPreInitialize();
        }

        object IServiceProvider.GetService(Type serviceType)
        {
            return GetService(serviceType);
        }

        protected object GetService(Type serviceType)
        {
            return _context.GetService(serviceType);
        }

        T IAnkhServiceProvider.GetService<T>()
        {
            return GetService<T>();
        }

        protected T GetService<T>() where T : class
        {
            return _context.GetService<T>();
        }

        T IAnkhServiceProvider.GetService<T>(Type serviceType)
        {
            return GetService<T>(serviceType);
        }

        protected T GetService<T>(Type serviceType) where T : class
        {
            return _context.GetService<T>(serviceType);
        }
    }
}
