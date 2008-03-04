using System;
using System.Collections.Generic;
using System.Text;

namespace AnkhBot
{
    interface ICommand
    {
        void Execute( CommandArgs args );
    }
}
