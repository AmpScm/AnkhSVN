using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Ankh.VS.Extenders
{
    /// <summary>
    /// 
    /// </summary>
    [ComVisible(true)] // This class must be public or the extender won't accept it.
    public sealed class SvnSolutionExtender : SvnItemExtender
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SvnSolutionExtender"/> class.
        /// </summary>
        /// <param name="extendeeObject">The extendee object.</param>
        /// <param name="provider">The provider.</param>
        /// <param name="extenderSite">The extender site.</param>
        /// <param name="cookie">The cookie.</param>
        /// <param name="catId">The cat id.</param>
        internal SvnSolutionExtender(object extendeeObject, AnkhExtenderProvider provider, EnvDTE.IExtenderSite extenderSite, int cookie, string catId)
            : base(extendeeObject, provider, extenderSite, cookie, catId)
        {
        }
    }
}
