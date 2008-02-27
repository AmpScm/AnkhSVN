using System;
using System.Collections.Generic;
using System.Text;
using SharpSvn;

namespace Ankh.UI.Helpers
{
    public static class SvnClientFactory
    {
        public static SvnClient NewClient()
        {
            SvnClient c = new SvnClient();
            return c;
        }
    }
}
