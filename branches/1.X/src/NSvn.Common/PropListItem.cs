// $Id$
using System;

namespace NSvn.Common
{
    /// <summary>
    /// Represents an item in a property list
    /// </summary>
    public class PropListItem
    {
        public PropListItem( string nodeName, PropertyDictionary properties )
        {
            this.properties = properties;
            this.nodeName = nodeName;			
        }

        public PropertyDictionary Properties
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.properties; }
        }

        public string NodeName
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.nodeName; }
        }


        private PropertyDictionary properties;
        private string nodeName;
    }
}
