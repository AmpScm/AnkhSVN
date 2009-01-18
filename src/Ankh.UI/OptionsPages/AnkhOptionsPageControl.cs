using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Ankh.UI.OptionsPages
{
    /// <summary>
    /// 
    /// </summary>
    public class AnkhOptionsPageControl : UserControl
    {
        IAnkhServiceProvider _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnkhOptionsPageControl"/> class.
        /// </summary>
        protected AnkhOptionsPageControl()
        {
            Size = new System.Drawing.Size(400, 271);
        }

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>The context.</value>
        public IAnkhServiceProvider Context
        {
            get { return _context; }
            set { _context = value; OnContextChanged(EventArgs.Empty); }
        }

        /// <summary>
        /// Raises the <see cref="E:ContextChanged"/> event.
        /// </summary>
        /// <param name="eventArgs">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnContextChanged(EventArgs eventArgs)
        {

        }

        /// <summary>
        /// Loads the settings.
        /// </summary>
        public virtual void LoadSettings()
        {
        }

        /// <summary>
        /// Saves the settings.
        /// </summary>
        public virtual void SaveSettings()
        {
        }
    }
}
