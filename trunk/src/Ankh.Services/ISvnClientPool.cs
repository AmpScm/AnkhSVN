using System;
using System.Collections.Generic;
using System.Text;
using SharpSvn;
using System.Diagnostics;

namespace Ankh
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>The default implementation of this service is thread safe</remarks>
    public interface ISvnClientPool
    {
        /// <summary>
        /// Gets a free <see cref="SvnClient"/> instance from the pool
        /// </summary>
        /// <returns></returns>
        SvnPoolClient GetClient();

        /// <summary>
        /// Gets a free <see cref="SvnClient"/> instance from the pool
        /// </summary>
        /// <returns></returns>
        SvnPoolClient GetNoUIClient();

        /// <summary>
        /// Returns the client.
        /// </summary>
        /// <param name="poolClient">The pool client.</param>
        /// <returns>true if the pool accepts the client, otherwise false</returns>
        bool ReturnClient(SvnPoolClient poolClient);
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class SvnPoolClient : SvnClient, IDisposable
    {
        ISvnClientPool _pool;

        // Note: We can only implement our own Dispose over the existing
        // As VC++ unconditionally calls GC.SuppressFinalize() just before returning
        // Luckily the using construct uses the last defined or IDisposable methods which we can override
               
        protected SvnPoolClient(ISvnClientPool pool)
        {
            if (pool == null)
                throw new ArgumentNullException("pool");

            _pool = pool;
        }  

        /// <summary>
        /// Returns the client to the client pool
        /// </summary>
        public new void Dispose()
        {
            ReturnClient();
        }

        void IDisposable.Dispose()
        {
            ReturnClient();
        }

        protected ISvnClientPool SvnClientPool
        {
            get { return _pool; }
        }

        /// <summary>
        /// Returns the client to the threadpool, or disposes the cleint
        /// </summary>
        protected virtual void ReturnClient()
        {
            if (!_pool.ReturnClient(this))
            {
                _pool = null;
                InnerDispose();
            }
        }

        /// <summary>
        /// Calls the original dispose method
        /// </summary>
        protected void InnerDispose()
        {
            base.Dispose(); // Includes GC.SuppressFinalize()
        }
    }
}