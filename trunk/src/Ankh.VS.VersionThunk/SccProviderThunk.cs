using Ankh.Scc;
using System;
using System.Text;

namespace Ankh.Scc
{
    public abstract partial class SccProviderThunk : SccProvider
    {
        [CLSCompliant(false)]
        protected SccProviderThunk(IAnkhServiceProvider context, SccProjectMap projectMap)
            : base(context, projectMap)
        {

        }
    }
}
