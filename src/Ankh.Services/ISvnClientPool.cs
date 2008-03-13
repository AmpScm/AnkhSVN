using System;
using System.Collections.Generic;
using System.Text;
using SharpSvn;
using System.Diagnostics;

namespace Ankh
{
    public interface ISvnClientPool
    {
        /// <summary>
        /// Gets a free <see cref="SvnClient"/> instance from the pool
        /// </summary>
        /// <returns></returns>
        SvnClient GetClient();

        /// <summary>
        /// Gets a free <see cref="SvnClient"/> instance from the pool
        /// </summary>
        /// <returns></returns>
        SvnClient GetNoUIClient();

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

        // Note: All this depends on the knowledge that VC++ implements the disposable
        // pattern as it should be. IDisposable and Dispose route to Dispose(true), which we override
        // when we are sure we are returned to the pool
               
        protected SvnPoolClient(ISvnClientPool pool)
        {
            if (pool == null)
                throw new ArgumentNullException("pool");

            _pool = pool;
        }

        /// <summary>
        /// Returns the SvnClient to the pool
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && _pool != null)
                ReturnClient();
            else
                base.Dispose(disposing);
        }

        void IDisposable.Dispose()
        {
            ReturnClient();
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
            base.Dispose(true);
        }
    }
}