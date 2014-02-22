using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.VS
{
    public interface ILinqInterfaceDelegateService
    {
        TDelegate GetInterfaceDelegate<TDelegate>(Type fromInterface, System.Reflection.MethodInfo method, object ob) where TDelegate : class;
    }
}
