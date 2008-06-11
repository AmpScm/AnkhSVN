// $Id$
using System.Windows.Forms;
using System;

using IServiceProvider = System.IServiceProvider;
using SharpSvn;
using Ankh.UI.Services;
using Ankh.Selection;
using Ankh.UI;

namespace Ankh
{
    public interface IContext : IAnkhServiceProvider
    {
        /// <summary>
        /// The output pane.
        /// </summary>
        OutputPaneWriter OutputPane { get; }

        /// <summary>
        /// The ISvnClientPool object used by the NSvn objects.
        /// </summary>
        ISvnClientPool ClientPool { get; }

        /// <summary>
        /// Should be called before starting any lengthy operation
        /// </summary>
        IDisposable StartOperation( string description );        
    }
}
