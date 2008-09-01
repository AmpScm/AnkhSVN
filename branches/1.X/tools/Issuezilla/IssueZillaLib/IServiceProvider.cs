using System;
using System.Collections.Generic;
using System.Text;

namespace Fines.IssueZillaLib
{
    public interface IServiceProvider
    {
        T GetService<T>() where T : class;
    }
}
