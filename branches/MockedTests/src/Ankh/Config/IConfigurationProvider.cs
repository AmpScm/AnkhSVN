using System;

namespace Ankh
{
    interface IConfigurationProvider
    {
        Ankh.Config.ConfigLoader ConfigLoader { get;}
        Ankh.Config.Config Configuration { get;}
    }
    
}
