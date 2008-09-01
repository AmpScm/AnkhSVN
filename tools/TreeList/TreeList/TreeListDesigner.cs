using System;
using System.Text;
using System.Windows.Forms.Design;

namespace Ankh.Tools
{
    public class TreeListDesigner : ControlDesigner
    {
        protected override void PostFilterProperties( System.Collections.IDictionary properties )
        {
            properties.Remove( "Items" );
            properties.Remove( "View" );
            base.PostFilterProperties( properties );
        }
    }
}
