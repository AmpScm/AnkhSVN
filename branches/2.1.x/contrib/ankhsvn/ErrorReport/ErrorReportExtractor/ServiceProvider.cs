using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace ErrorReportExtractor
{
    public class ServiceProvider : IServiceProvider
    {   
        #region IServiceProvider Members

        public T GetService<T>() where T : IService
        {
            IService service;
            if ( this.services.TryGetValue( typeof( T ), out service ) )
            {
                return (T)service;
            }
            else
            {
                return default(T);
            }
        }

        public void ProfferService<T>( T t ) where T : IService
        {
            this.services[ typeof( T ) ] = t;
            t.SetProgressCallback( this.callback );
        }

        public void SetProgressCallback( IProgressCallback callback )
        {
            foreach(IService service in this.services.Values)
            {
                service.SetProgressCallback( callback );
            }
        }

        #endregion

        private Dictionary<Type, IService> services = new Dictionary<Type, IService>();
        private IProgressCallback callback = new NullProgressCallback();
    }
}
