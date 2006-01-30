using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.Extended
{
    public class Extension : IExtension
    {
        #region IExtension Members

        public void Initialize( IContext context )
        {
            TrackProjectDocuments tpd = new TrackProjectDocuments( context );

        }

        #endregion
}
}
