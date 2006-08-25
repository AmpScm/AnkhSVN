//$Id$
using System;

namespace Ankh.UI
{
    /// <summary>
    /// Interface which has to be implemented by property editor user controls
    /// </summary>
    internal interface IPropertyEditor
    {
        /// <summary>
        /// Whether the property editor is in a valid state
        /// </summary>
        bool Valid
        {
            get;
        }
    
        /// <summary>
        /// Gets or sets the property item
        /// </summary>
        PropertyItem PropertyItem
        {
            get;
            set;
        }

        /// <summary>
        /// Reset the property editor to its default state
        /// </summary>
        void Reset();

        
        /// <summary>
        /// Fired whenever the editor's state changes
        /// </summary>
        event EventHandler Changed;

    }
}

