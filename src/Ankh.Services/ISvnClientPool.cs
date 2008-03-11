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
        SvnPoolClient GetClient();

        /// <summary>
        /// Returns the client.
        /// </summary>
        /// <param name="poolClient">The pool client.</param>
        /// <returns>true if the pool accepts the client, otherwise false</returns>
        bool ReturnClient(SvnPoolClient poolClient);
    }

    public abstract class SvnPoolClient : SvnClient, IDisposable
    {
        ISvnClientPool _pool;

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
            if (disposing || _pool == null)
                ReturnClient();
            else
                base.Dispose(disposing);
        }

        public new void Dispose()
        {
            ReturnClient();
        }

        void IDisposable.Dispose()
        {
            ReturnClient();
        }

        protected virtual void ReturnClient()
        {
            if (!_pool.ReturnClient(this))
            {
                _pool = null;
                InnerDispose();
            }
        }

        protected void InnerDispose()
        {
            base.Dispose(true);
        }
    }
}