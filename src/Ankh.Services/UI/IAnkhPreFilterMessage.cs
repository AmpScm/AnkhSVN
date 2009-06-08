using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Ankh.UI
{
    public interface IAnkhPreFilterMessage
    {
        bool PreFilterMessage(ref Message msg);
    }
}
