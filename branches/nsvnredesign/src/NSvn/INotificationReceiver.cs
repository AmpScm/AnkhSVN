// $Id$
using System;
using NSvn.Core;

namespace NSvn
{
    /// <summary>
    /// Defines a contract for an object that receives notification callbacks.
    /// </summary>
    [Obsolete("Will be removed in a future version")]
    public interface INotificationReceiver
    {
        void Notify( Notification notification );
    }
}
